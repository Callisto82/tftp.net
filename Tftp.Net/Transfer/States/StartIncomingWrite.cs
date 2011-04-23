using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Tftp.Net.TransferOptions;

namespace Tftp.Net.Transfer.States
{
    class StartIncomingWrite : BaseState
    {
        public StartIncomingWrite(TftpTransfer context)
            : base(context) { }

        public override void OnStart()
        {
            //Do we have any acknowledged options?
            TransferOptionHandlers.HandleAcceptedOptions(Context, Context.Options);
            if (Context.Options.Count(x => x.IsAcknowledged) > 0)
            {
                Context.SetState(new SendOptionAcknowledgementForWriteRequest(Context));
            }
            else
            {
                //Start receiving
                Context.SetState(new AcknowledgeWriteRequest(Context));
            }
        }

        public override void OnCancel(TftpErrorPacket reason)
        {
            Context.SetState(new CancelledByUser(Context, reason));
        }
    }
}
