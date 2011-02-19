using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Tftp.Net.Transfer.States
{
    class Sending : StateThatExpectsMessagesFromDefaultEndPoint
    {
        private readonly Stream input;
        private byte[] lastSentPacket = new byte[512];
        private ushort lastBlockNumber;
        private int bytesSent = 0;

        public Sending(TftpTransfer context, Stream input)
            : base(context)
        {
            this.input = input;
        }

        public override void OnStateEnter()
        {
 	         SendNextPacket(1);
        }

        public override void OnAcknowledgement(Acknowledgement command)
        {
            //Drop acknowledgments for other packets than the previous one
            if (command.BlockNumber != lastBlockNumber)
                return;

            //Notify our observers about our progress
            bytesSent += lastSentPacket.Length;
            Context.RaiseOnProgress(bytesSent);

            SendNextPacket(++lastBlockNumber);
        }

        public override void OnError(Error command)
        {
            Context.SetState(new ReceivedError(Context, command.ErrorCode, command.Message));
        }

        public override void OnCancel()
        {
            Context.SetState(new CancelledByUser(Context));
        }

        #region Helper Methods
        private void SendNextPacket(ushort blockNumber)
        {
            int packetLength = input.Read(lastSentPacket, 0, lastSentPacket.Length);
            lastBlockNumber = blockNumber;

            if (packetLength != lastSentPacket.Length)
            {
                //This means we just sent the last packet
                Array.Resize(ref lastSentPacket, packetLength);
                Context.SetState(new SentLastPacket(Context, lastBlockNumber));
            }

            SendPacket(blockNumber, lastSentPacket);
        }

        private void SendPacket(ushort blockNumber, byte[] lastSentPacket)
        {
            ITftpCommand dataCommand = new Data(blockNumber, lastSentPacket);
            Context.GetConnection().Send(dataCommand);
        }
        #endregion
    }
}
