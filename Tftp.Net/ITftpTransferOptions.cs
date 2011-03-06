using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tftp.Net
{
    /// <summary>
    /// A collection of Transfer options according to RFC2347.
    /// </summary>
    public interface ITftpTransferOptions : IEnumerable<ITftpTransferOption>
    {
        /// <summary>
        /// Request an additional transfer option.
        /// </summary>
        void Request(String option, String value);
    }
}
