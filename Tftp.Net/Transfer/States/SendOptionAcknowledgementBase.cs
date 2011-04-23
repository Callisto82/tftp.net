using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tftp.Net.Transfer.States;

namespace Tftp.Net.Transfer
{
    class SendOptionAcknowledgementBase : StateThatExpectsMessagesFromDefaultEndPoint
    {
        private readonly SimpleTimer timer;

        public SendOptionAcknowledgementBase(TftpTransfer context)
            : base(context)
        {
            timer = new SimpleTimer(context.Timeout);
        }

        public override void OnStateEnter()
        {
            SendAcknowledgement();
            timer.Restart();
        }

        public override void OnTimer()
        {
            if (timer.IsTimeout())
            {
                //We didn't get an acknowledgement in time. Re-send the option acknowledgement
                SendAcknowledgement();
                timer.Restart();
            }
        }

        public override void OnError(Error command)
        {
            Context.SetState(new ReceivedError(Context, command));
        }

        public override void OnCancel(TftpErrorPacket reason)
        {
            Context.SetState(new CancelledByUser(Context, reason));
        }

        private void SendAcknowledgement()
        {
            //Send an option acknowledgement
            Context.GetConnection().Send(new OptionAcknowledgement(Context.Options.Where(x => x.IsAcknowledged)));
        }
    }
}
