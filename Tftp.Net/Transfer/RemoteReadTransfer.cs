using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tftp.Net.Transfer.States;
using Tftp.Net.Channel;
using Tftp.Net.Transfer;

namespace Tftp.Net.Transfer
{
    class RemoteReadTransfer : TransferWithTimeout
    {
        public RemoteReadTransfer(IChannel connection, String filename)
            : base(connection, filename)
        {
            SetState(new StartOutgoingRead(this));
        }

        public override int ExpectedSize
        {
            get { return base.ExpectedSize; }
            set { throw new NotSupportedException("You cannot set the expected size of a file that is remotely transferred to this system."); }
        }
    }
}
