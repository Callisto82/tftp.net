using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Tftp.Net.Transfer;

namespace Tftp.Net.Transfer.States
{
    class StartIncomingRead : BaseState
    {
        public StartIncomingRead(TftpTransfer context, IEnumerable<TransferOption> optionsRequestedByClient)
            : base(context) {
                Context.SetActiveTransferOptions(optionsRequestedByClient);
        }

        public override void OnStart()
        {
            Context.FillOrDisableTransferSizeOption();

            //Do we have any acknowledged options?
            List<TransferOption> options = Context.GetActiveTransferOptions();
            if (options.Count > 0)
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
