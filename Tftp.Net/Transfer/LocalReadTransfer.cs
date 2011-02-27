using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tftp.Net.Transfer.States;
using Tftp.Net.Channel;
using Tftp.Net.TransferOptions;

namespace Tftp.Net.Transfer
{
    class LocalReadTransfer : TransferWithTimeout
    {
        public LocalReadTransfer(IChannel connection, string filename, IEnumerable<ITftpTransferOption> options)
            : base(connection, filename)
        {
            this.Options = new TransferOptionsIncoming(options);
            SetState(new StartIncomingRead(this));
        }

        public override TftpTransferMode TransferMode
        {
            get { return base.TransferMode; }
            set { throw new NotSupportedException("Cannot change the transfer mode for incoming transfers. The transfer mode is determined by the client."); }
        }
    }
}
