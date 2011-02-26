using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tftp.Net.Channel;
using Tftp.Net.Transfer.States;

namespace Tftp.Net.Transfer
{
    class RemoteWriteTransfer : TransferWithTimeout
    {
        public RemoteWriteTransfer(ITftpChannel connection, String filename)
            : base(connection, filename)
        {
            Options = new TransferOptions();
            SetState(new StartOutgoingWrite(this));
        }
    }
}
