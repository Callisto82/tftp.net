using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Tftp.Net.Transfer.States
{
    class SendWriteRequest : BaseState
    {
        private readonly SimpleTimer timer;

        public SendWriteRequest(TftpTransfer context)
            : base(context) 
        {
            timer = new SimpleTimer(Context.Timeout);
        }

        public override void OnStateEnter()
        {
            SendRequest();
        }

        public override void OnTimer()
        {
            //Re-send the write request
            if (timer.IsTimeout())
                SendRequest();
        }

        private void SendRequest()
        {
            WriteRequest request = new WriteRequest(Context.Filename, Context.TransferMode, Context.Options);
            Context.GetConnection().Send(request);
            timer.Restart();
        }

        public override void OnCommand(ITftpCommand command, System.Net.EndPoint endpoint)
        {
            if (command is Acknowledgement && (command as Acknowledgement).BlockNumber == 0)
            {
                //Switch to the endpoint that we received from the server
                Context.GetConnection().RemoteEndpoint = endpoint;

                //Start sending packets
                Context.SetState(new Sending(Context));
            }
            else
            if (command is Error)
            {
                //The server denied our request
                Error error = (Error)command;
                Context.SetState(new ReceivedError(Context, error.ErrorCode, error.Message));
            }
            else
                base.OnCommand(command, endpoint);
        }

        public override void OnCancel()
        {
            Context.SetState(new CancelledByUser(Context));
        }
    }
}
