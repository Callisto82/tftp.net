using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tftp.Net.Transfer.States;
using Tftp.Net.Channel;

namespace Tftp.Net.Transfer
{
    class LocalReadTransfer : TftpTransfer
    {
        public LocalReadTransfer(ITftpChannel connection, string filename)
            : base(connection, filename)
        {
            SetState(new StartIncomingRead(this));
        }
    }
}
