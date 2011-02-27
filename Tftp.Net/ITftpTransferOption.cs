using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tftp.Net
{
    /// <summary>
    /// A single transfer options according to RFC2347.
    /// </summary>
    public interface ITftpTransferOption
    {
        /// <summary>
        /// Name of the option
        /// </summary>
        String Name { get; }

        /// <summary>
        /// Value for the option
        /// </summary>
        String Value { get; }

        /// <summary>
        /// Did we acknowledge the option?
        /// </summary>
        bool IsAcknowledged { get; }
    }
}
