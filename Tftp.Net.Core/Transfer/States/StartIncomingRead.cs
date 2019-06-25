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
        private readonly IEnumerable<TransferOption> optionsRequestedByClient;

        public StartIncomingRead(IEnumerable<TransferOption> optionsRequestedByClient)
        {
            this.optionsRequestedByClient = optionsRequestedByClient;
        }

        public override void OnStateEnter()
        {
            Context.ProposedOptions = new TransferOptionSet(optionsRequestedByClient);
        }

        public override void OnStart()
        {
            Context.FillOrDisableTransferSizeOption();
            Context.FinishOptionNegotiation(Context.ProposedOptions);
            List<TransferOption> options = Context.NegotiatedOptions.ToOptionList();
            if (options.Count > 0)
            {
                Context.SetState(new SendOptionAcknowledgementForReadRequest());
            }
            else
            {
                //Otherwise just start sending
                Context.SetState(new Sending());
            }
        }

        public override void OnCancel(TftpErrorPacket reason)
        {
            Context.SetState(new CancelledByUser(reason));
        }
    }
}
