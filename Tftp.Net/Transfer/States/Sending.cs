using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Tftp.Net.Trace;

namespace Tftp.Net.Transfer.States
{
    class Sending : StateThatExpectsMessagesFromDefaultEndPoint
    {
        private readonly SimpleTimer timer;
        private byte[] lastSentPacket;
        private ushort lastBlockNumber;
        private int bytesSent = 0;

        public Sending(TftpTransfer context)
            : base(context)
        {
            timer = new SimpleTimer(context.Timeout);
            lastSentPacket = new byte[context.BlockSize];
        }

        public override void OnTimer()
        {
            if (timer.IsTimeout())
            {
                //We didn't get an acknowledgement in time. Re-send the last data packet
                TftpTrace.Trace("Timeout. Resending last packet.", Context);
                SendPacket(lastBlockNumber, lastSentPacket);
                timer.Restart();
            }
        }

        public override void OnStateEnter()
        {
 	         SendNextPacket(1);
             timer.Restart();
        }

        public override void OnAcknowledgement(Acknowledgement command)
        {
            //Drop acknowledgments for other packets than the previous one
            if (command.BlockNumber != lastBlockNumber)
                return;

            //Notify our observers about our progress
            bytesSent += lastSentPacket.Length;

            SendNextPacket((ushort)(lastBlockNumber + 1));
            timer.Restart();

            Context.RaiseOnProgress(bytesSent);
        }

        public override void OnError(Error command)
        {
            Context.SetState(new ReceivedError(Context, command));
        }

        public override void OnCancel(TftpErrorPacket reason)
        {
            Context.SetState(new CancelledByUser(Context, reason));
        }

        #region Helper Methods
        private void SendNextPacket(ushort blockNumber)
        {
            if (Context.InputOutputStream == null)
                return;

            int packetLength = Context.InputOutputStream.Read(lastSentPacket, 0, lastSentPacket.Length);
            lastBlockNumber = blockNumber;

            if (packetLength != lastSentPacket.Length)
            {
                //This means we just sent the last packet
                Array.Resize(ref lastSentPacket, packetLength);
                Context.SetState(new SentLastPacket(Context, lastBlockNumber));
            }

            SendPacket(blockNumber, lastSentPacket);
        }

        private void SendPacket(ushort blockNumber, byte[] data)
        {
            ITftpCommand dataCommand = new Data(blockNumber, data);
            Context.GetConnection().Send(dataCommand);
        }
        #endregion
    }
}
