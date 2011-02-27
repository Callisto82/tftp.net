using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tftp.Net.Transfer.States
{
    class SentLastPacket : StateThatExpectsMessagesFromDefaultEndPoint
    {
        private readonly ushort lastBlockNumber;

        public SentLastPacket(TftpTransfer context, ushort lastBlockNumber)
            : base(context)
        {
            this.lastBlockNumber = lastBlockNumber;
        }

        public override void OnAcknowledgement(Acknowledgement command)
        {
            //Drop acks that are not related to the last block
            if (command.BlockNumber != lastBlockNumber)
                return;

            //We received an Ack for the last block. We're done here :)
            Context.RaiseOnFinished();
            Context.SetState(new Closed(Context));
        }

        public override void OnError(Error command)
        {
            Context.SetState(new ReceivedError(Context, command));
        }

        public override void OnCancel()
        {
            Context.SetState(new CancelledByUser(Context));
        }
    }
}
