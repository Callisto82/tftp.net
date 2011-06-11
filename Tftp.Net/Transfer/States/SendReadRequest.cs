using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Tftp.Net.Channel;
using System.Net;
using Tftp.Net.TransferOptions;
using Tftp.Net.Trace;

namespace Tftp.Net.Transfer.States
{
    class SendReadRequest : BaseState
    {
        private readonly SimpleTimer timer;

        public SendReadRequest(TftpTransfer context)
            : base(context) 
        {
            timer = new SimpleTimer(Context.Timeout);
        }

        public override void OnStateEnter()
        {
            //Send a read request to the server
            SendRequest();
        }

        private void SendRequest()
        {
            ReadRequest request = new ReadRequest(Context.Filename, Context.TransferMode, Context.Options);
            Context.GetConnection().Send(request);
            timer.Restart();
        }

        public override void OnTimer()
        {
            //Re-send the read request
            if (timer.IsTimeout())
            {
                TftpTrace.Trace("Timeout. Resending read request.", Context);
                SendRequest();
            }
        }

        public override void OnCommand(ITftpCommand command, EndPoint endpoint)
        {
            if (command is Data)
            {
                //The server acknowledged our read request.
                //Fix out remote endpoint
                Context.GetConnection().RemoteEndpoint = endpoint;

                //Switch to the receiving state...
                ITransferState nextState = new Receiving(Context);
                Context.SetState(nextState);

                //...and let it handle the data packet
                nextState.OnCommand(command, endpoint);
            }
            else if (command is OptionAcknowledgement)
            {
                //the server acknowledged our options. Confirm the final options
                Context.GetConnection().Send(new Acknowledgement(0));

                //Check which options were acknowledged
                OptionAcknowledgement ackCommand = (OptionAcknowledgement)command;
                TransferOptionHandlers.HandleAcceptedOptions(Context, ackCommand.Options);
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
