using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tftp.Net
{
    /// <summary>
    /// Transfer options according to RFC2347.
    /// </summary>
    public interface ITftpTransferOptions : IEnumerable<ITftpTransferOption>
    {
        void Request(String option, String value);
    }
}
