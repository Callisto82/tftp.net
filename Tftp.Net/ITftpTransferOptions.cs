using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tftp.Net
{
    /// <summary>
    /// A single transfer options according to RFC2347.
    /// </summary>
    public class TftpTransferOption
    {
        /// <summary>
        /// Name of the option
        /// </summary>
        public String Name { get; private set; }

        /// <summary>
        /// Value for the option
        /// </summary>
        public String Value { get; private set; }

        /// <summary>
        /// Set this flag if you would like to acknowledge the option (for incoming options).
        /// </summary>
        public bool IsAcknowledged { get; set; }

        public TftpTransferOption(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }
    }

    /// <summary>
    /// Transfer options according to RFC2347.
    /// </summary>
    public interface ITftpTransferOptions : IEnumerable<TftpTransferOption>
    {
        void Add(String name, String value);
    }
}
