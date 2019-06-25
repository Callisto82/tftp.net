using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tftp.Net.Transfer.States;
using Tftp.Net.Channel;
using Tftp.Net.Transfer;

namespace Tftp.Net.Transfer
{
    class RemoteReadTransfer : TftpTransfer
    {
        public RemoteReadTransfer(ITransferChannel connection, String filename)
            : base(connection, filename, new StartOutgoingRead())
        {
        }

        public override long ExpectedSize
        {
            get { return base.ExpectedSize; }
            set { throw new NotSupportedException("You cannot set the expected size of a file that is remotely transferred to this system."); }
        }
    }
}
