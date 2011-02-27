using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tftp.Net.Channel;
using Tftp.Net.Transfer.States;
using Tftp.Net.TransferOptions;

namespace Tftp.Net.Transfer
{
    class RemoteWriteTransfer : TransferWithTimeout
    {
        public RemoteWriteTransfer(IChannel connection, String filename)
            : base(connection, filename)
        {
            Options = new TransferOptionsOutgoing();
            SetState(new StartOutgoingWrite(this));
        }
    }
}
