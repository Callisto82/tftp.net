using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tftp.Net.TransferOptions
{
    class TransferOptionsIncoming : TransferOptionsBase
    {
        public TransferOptionsIncoming(IEnumerable<ITftpTransferOption> requestedOptions)
        {
            foreach (ITftpTransferOption option in requestedOptions)
                options.Add(option.Name, new TransferOption(option.Name, option.Value));
        }

        public override void Request(string option, string value)
        {
            throw new NotSupportedException("Transfer options can only be requested for outgoing transfers.");
        }
    }
}
