using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Tftp.Net.Transfer.States
{
    class StartingOutgoingWrite : BaseState
    {
        public StartingOutgoingWrite(TftpTransfer context)
            : base(context) 
        { }

        public override void OnStateEnter()
        {
            Context.GetConnection().Send(new WriteRequest(Context.Filename, TftpModeType.octet));
        }

        public override void OnCommand(ITftpCommand command, System.Net.EndPoint endpoint)
        {
            if (command is Acknowledgement && (command as Acknowledgement).BlockNumber == 0)
            {
                //Switch to the endpoint that we received from the server
                Context.GetConnection().SetRemoteEndPoint(endpoint);

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
