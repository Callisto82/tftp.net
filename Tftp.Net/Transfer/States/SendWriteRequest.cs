using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Tftp.Net.Transfer;
using Tftp.Net.Trace;

namespace Tftp.Net.Transfer.States
{
    class SendWriteRequest : StateWithNetworkTimeout
    {
        public SendWriteRequest(TftpTransfer context)
            : base(context) 
        {
        }

        public override void OnStateEnter()
        {
            SendRequest();
        }

        private void SendRequest()
        {
            WriteRequest request = new WriteRequest(Context.Filename, Context.TransferMode, Context.GetActiveTransferOptions());
            SendAndRepeat(request);
        }

        public override void OnCommand(ITftpCommand command, System.Net.EndPoint endpoint)
        {
            if (command is OptionAcknowledgement)
            {
                OptionAcknowledgement ackCommand = (OptionAcknowledgement)command;
                Context.SetActiveTransferOptions(ackCommand.Options);
                BeginSendingTo(endpoint);
            }
            else
            if (command is Acknowledgement && (command as Acknowledgement).BlockNumber == 0)
            {
                Context.SetActiveTransferOptions(new TransferOption[0]);
                BeginSendingTo(endpoint);
            }
            else
            if (command is Error)
            {
                //The server denied our request
                Error error = (Error)command;
                Context.SetState(new ReceivedError(Context, error));
            }
            else
                base.OnCommand(command, endpoint);
        }

        private void BeginSendingTo(System.Net.EndPoint endpoint)
        {
            //Switch to the endpoint that we received from the server
            Context.GetConnection().RemoteEndpoint = endpoint;

            //Start sending packets
            Context.SetState(new Sending(Context));
        }

        public override void OnCancel(TftpErrorPacket reason)
        {
            Context.SetState(new CancelledByUser(Context, reason));
        }
    }
}
