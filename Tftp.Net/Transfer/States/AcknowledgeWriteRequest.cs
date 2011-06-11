using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tftp.Net.Trace;

namespace Tftp.Net.Transfer.States
{
    class AcknowledgeWriteRequest : StateThatExpectsMessagesFromDefaultEndPoint
    {
        private readonly SimpleTimer timer;

        public AcknowledgeWriteRequest(TftpTransfer context)
            : base(context) 
        {
            timer = new SimpleTimer(context.Timeout);
        }

        public override void OnStateEnter()
        {
            Context.GetConnection().Send(new Acknowledgement(0));
            timer.Restart();
        }

        public override void OnTimer()
        {
            if (timer.IsTimeout())
            {
                TftpTrace.Trace("Timeout. Resending acknowledgment of write request.", Context);
                Context.GetConnection().Send(new Acknowledgement(0));
                timer.Restart();
            }
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
