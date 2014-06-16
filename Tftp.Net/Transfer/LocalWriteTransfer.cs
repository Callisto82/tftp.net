using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tftp.Net.Transfer.States;
using Tftp.Net.Channel;
using Tftp.Net.Transfer;

namespace Tftp.Net.Transfer
{
    class LocalWriteTransfer : TransferWithTimeout
    {
        public LocalWriteTransfer(IChannel connection, string filename, IEnumerable<TransferOption> options)
            : base(connection, filename)
        {
            SetState(new StartIncomingWrite(this, options));
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

        public override int ExpectedSize
        {
            get { return base.ExpectedSize; }
            set { throw new NotSupportedException("You cannot set the expected size of a file that is remotely transferred to this system."); }
        }
    }
}
