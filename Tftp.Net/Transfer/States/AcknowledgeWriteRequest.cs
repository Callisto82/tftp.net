using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tftp.Net.Trace;

namespace Tftp.Net.Transfer.States
{
    class AcknowledgeWriteRequest : StateThatExpectsMessagesFromDefaultEndPoint
    {
        public AcknowledgeWriteRequest(TftpTransfer context)
            : base(context) 
        {
        }

        public override void OnStateEnter()
        {
            SendAndRepeat(new Acknowledgement(0));
        }

        public override void OnData(Data command)
        {
            ITransferState nextState = new Receiving(Context);
            Context.SetState(nextState);
            nextState.OnCommand(command, Context.GetConnection().RemoteEndpoint);
        }

        public override void OnCancel(TftpErrorPacket reason)
        {
            Context.SetState(new CancelledByUser(Context, reason));
        }

        public override void OnError(Error command)
        {
            Context.SetState(new ReceivedError(Context, command));
        }
    }
}
