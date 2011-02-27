using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tftp.Net.TransferOptions
{
    /// <summary>
    /// Implement this interface and call TransferOptionHandlers.Add(...) to handle a custom TFTP transfer option (RFC 2347).
    /// </summary>
    interface ITftpTransferOptionHandler
    {
        /// <summary>
        /// Return true to acknowledge the given option, false otherwise.
        /// </summary>
        bool Acknowledge(ITftpTransfer transfer, ITftpTransferOption option);
    }
}
