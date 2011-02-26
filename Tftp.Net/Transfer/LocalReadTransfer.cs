using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tftp.Net.Transfer.States;
using Tftp.Net.Channel;

namespace Tftp.Net.Transfer
{
    class LocalReadTransfer : TransferWithTimeout
    {
        public LocalReadTransfer(ITftpChannel connection, string filename, IEnumerable<TftpTransferOption> options)
            : base(connection, filename)
        {
            this.OptionsBackend.Set(options);
            this.Options = new ReadonlyOptions(OptionsBackend);
            SetState(new StartIncomingRead(this));
        }

        public override TftpTransferMode TransferMode
        {
            get { return base.TransferMode; }
            set { throw new NotSupportedException("Cannot change the transfer mode for incoming transfers. The transfer mode is determined by the client."); }
        }
    }
}
