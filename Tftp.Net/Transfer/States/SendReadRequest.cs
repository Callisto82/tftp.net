using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Tftp.Net.Channel;
using System.Net;
using Tftp.Net.Transfer;
using Tftp.Net.Trace;

namespace Tftp.Net.Transfer.States
{
    class SendReadRequest : StateWithNetworkTimeout
    {
        public SendReadRequest(TftpTransfer context)
            : base(context)  { }

        public override void OnStateEnter()
        {
            //Send a read request to the server
            SendRequest();
        }

        private void SendRequest()
        {
            ReadRequest request = new ReadRequest(Context.Filename, Context.TransferMode, Context.ProposedOptions.ToOptionList());
            SendAndRepeat(request);
        }

        public override void OnCommand(ITftpCommand command, EndPoint endpoint)
        {
            if (command is Data || command is OptionAcknowledgement)
            {
                //The server acknowledged our read request.
                //Fix out remote endpoint
                Context.GetConnection().RemoteEndpoint = endpoint;
            }

            if (command is Data)
            {
                if (Context.NegotiatedOptions == null)
                    Context.FinishOptionNegotiation(TransferOptionSet.NewEmptySet());

                //Switch to the receiving state...
                ITransferState nextState = new Receiving(Context);
                Context.SetState(nextState);

                //...and let it handle the data packet
                nextState.OnCommand(command, endpoint);
            }
            else if (command is OptionAcknowledgement)
            {
                //Check which options were acknowledged
                Context.FinishOptionNegotiation(new TransferOptionSet( (command as OptionAcknowledgement).Options ));

                //the server acknowledged our options. Confirm the final options
                SendAndRepeat(new Acknowledgement(0));
            }
            else if (command is Error)
            {
                Context.SetState(new ReceivedError(Context, (Error)command));
            }
            else
                base.OnCommand(command, endpoint);
        }

        public override void OnCancel(TftpErrorPacket reason)
        {
            Context.SetState(new CancelledByUser(Context, reason));
        }
    }
}
