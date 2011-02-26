using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tftp.Net.Transfer.States;
using Tftp.Net.Channel;

namespace Tftp.Net.Transfer
{
    class RemoteReadTransfer : TransferWithTimeout
    {
        public RemoteReadTransfer(ITftpChannel connection, String filename)
            : base(connection, filename)
        {
            Options = new TransferOptions();
            SetState(new StartOutgoingRead(this));
        }
    }
}
