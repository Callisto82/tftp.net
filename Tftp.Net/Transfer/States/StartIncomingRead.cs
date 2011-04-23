using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Tftp.Net.TransferOptions;

namespace Tftp.Net.Transfer.States
{
    class StartIncomingRead : BaseState
    {
        public StartIncomingRead(TftpTransfer context)
            : base(context) {}

        public override void OnStart()
        {
            //Do we have any acknowledged options?
            TransferOptionHandlers.HandleAcceptedOptions(Context, Context.Options);
            if (Context.Options.Count(x => x.IsAcknowledged) > 0)
            {
                Context.SetState(new SendOptionAcknowledgementForReadRequest(Context));
            }
            else
            {
                //Otherwise just start sending
                Context.SetState(new Sending(Context));
            }
        }

        public override void OnCancel(TftpErrorPacket reason)
        {
            Context.SetState(new CancelledByUser(Context, reason));
        }
    }
}
