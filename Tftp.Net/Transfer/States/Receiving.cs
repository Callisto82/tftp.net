using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Tftp.Net.Transfer.States
{
    class Receiving : StateThatExpectsMessagesFromDefaultEndPoint
    {
        private readonly int blockSize;
        private ushort nextBlockNumber;
        private int bytesReceived = 0;

        public Receiving(TftpTransfer context)
            : base(context)
        {
            this.nextBlockNumber = 1;
            this.blockSize = context.BlockSize;
        }

        public override void OnData(Data command)
        {
            if (command.BlockNumber == nextBlockNumber)
            {
                //We received a new block of data
                Context.InputOutputStream.Write(command.Bytes, 0, command.Bytes.Length);
                SendAcknowledgement(command.BlockNumber);

                //Was that the last block of data?
                if (command.Bytes.Length < blockSize)
                {
                    Context.RaiseOnFinished();
                    Context.SetState(new Closed(Context));
                }
                else
                {
                    nextBlockNumber++;
                    bytesReceived += command.Bytes.Length;
                    Context.RaiseOnProgress(bytesReceived);
                }
            }
            else
            if (command.BlockNumber == nextBlockNumber - 1)
            {
                //We received the previous block again. Re-sent the acknowledgement
                SendAcknowledgement(command.BlockNumber);
            }
        }

        public override void OnCancel(TftpErrorPacket reason)
        {
            Context.SetState(new CancelledByUser(Context, reason));
        }

        public override void OnError(Error command)
        {
            Context.SetState(new ReceivedError(Context, command));
        }

        private void SendAcknowledgement(ushort blockNumber)
        {
            Acknowledgement ack = new Acknowledgement(blockNumber);
            Context.GetConnection().Send(ack);
            ResetTimeout();
        }
    }
}
