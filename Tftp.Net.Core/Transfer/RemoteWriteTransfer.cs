using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tftp.Net.Channel;
using Tftp.Net.Transfer.States;
using Tftp.Net.Transfer;

namespace Tftp.Net.Transfer
{
    class RemoteWriteTransfer : TftpTransfer
    {
        public RemoteWriteTransfer(ITransferChannel connection, String filename)
            : base(connection, filename, new StartOutgoingWrite())
        {
        }
    }
}
