using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tftp.Net.Transfer.States;
using Tftp.Net.Channel;
using Tftp.Net.Transfer;

namespace Tftp.Net.Transfer
{
    class LocalReadTransfer : TftpTransfer
    {
        public LocalReadTransfer(ITransferChannel connection, string filename, IEnumerable<TransferOption> options)
            : base(connection, filename, new StartIncomingRead(options)) 
        {
        }

        public override TftpTransferMode TransferMode
        {
            get { return base.TransferMode; }
            set { throw new NotSupportedException("Cannot change the transfer mode for incoming transfers. The transfer mode is determined by the client."); }
        }

        public override int BlockSize
        {
            get { return base.BlockSize; }
            set { throw new NotSupportedException("For incoming transfers, the blocksize is determined by the client."); }
        }

        public override TimeSpan RetryTimeout
        {
            get { return base.RetryTimeout; }
            set { throw new NotSupportedException("For incoming transfers, the retry timeout is determined by the client."); }
        }
    }
}
