using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tftp.Net.Transfer.States;
using Tftp.Net.Trace;

namespace Tftp.Net.Transfer
{
    class SendOptionAcknowledgementBase : StateThatExpectsMessagesFromDefaultEndPoint
    {
        public SendOptionAcknowledgementBase(TftpTransfer context)
            : base(context)
        {
        }

        public override void OnStateEnter()
        {
            //Send an option acknowledgement
            SendAndRepeat(new OptionAcknowledgement(Context.NegotiatedOptions.ToOptionList()));
        }

        public override void OnError(Error command)
        {
            Context.SetState(new ReceivedError(Context, command));
        }

        public override void OnCancel(TftpErrorPacket reason)
        {
            Context.SetState(new CancelledByUser(Context, reason));
        }
    }
}
