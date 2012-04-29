using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tftp.Net.Transfer;

namespace Tftp.Net.TransferOptions.Handlers
{
    /// <summary>
    /// Implements the TFTP timeout option, as defined in RFC 2349
    /// </summary>
    class TimeoutIntervalOption : ITftpTransferOptionHandler
    {
        public bool ApplyOption(ITftpTransfer transfer, ITftpTransferOption option)
        {
            if (option.Name != "timeout")
                return false;

            int timeout;
            if (!int.TryParse(option.Value, out timeout))
                return false;

            //Only accept timeouts in the range [1, 255]
            if (timeout < 1 || timeout > 255)
                return false;

            transfer.RetryTimeout = new TimeSpan(0, 0, timeout);
            return true;
        }
    }
}
