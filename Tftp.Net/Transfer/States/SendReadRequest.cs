using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Tftp.Net.Channel;
using System.Net;

namespace Tftp.Net.Transfer.States
{
    class SendReadRequest : BaseState
    {
        private readonly Stream stream;

        public SendReadRequest(TftpTransfer context, Stream stream)
            : base(context) 
        {
            this.stream = stream;
        }

        public override void OnStateEnter()
        {
            //Send a read request to the server
            ReadRequest request = new ReadRequest(Context.Filename, TftpModeType.octet);
            Context.GetConnection().Send(request);
        }

        public override void OnCommand(ITftpCommand command, EndPoint endpoint)
        {
            if (command is Data)
            {
                //The server acknowledged our read request.
                //Fix out remote endpoint
                Context.GetConnection().SetRemoteEndPoint(endpoint);

                //Switch to the receiving state...
                ITftpState nextState = new Receiving(Context, stream);
                Context.SetState(nextState);

                //...and let it handle the data packet
                nextState.OnCommand(command, endpoint);
            }
            else if (command is Error)
            {
                Context.SetState(new ReceivedError(Context, ((Error)command).ErrorCode, ((Error)command).Message));
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
