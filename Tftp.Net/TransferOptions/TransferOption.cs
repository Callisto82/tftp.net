using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tftp.Net.TransferOptions
{
    /// <summary>
    /// A single transfer options according to RFC2347.
    /// </summary>
    public class TransferOption : ITftpTransferOption
    {
        #region ITftpTransferOption

        public String Name { get; private set; }
        public String Value { get; set; }
        public bool IsAcknowledged { get; internal set; }

        #endregion

        internal TransferOption(string name, string value)
        {
            if (String.IsNullOrEmpty(name))
                throw new ArgumentException("name must not be null or empty.");

            if (value == null)
                throw new ArgumentNullException("value must not be null.");

            this.Name = name;
            this.Value = value;
        }

        public override string ToString()
        {
            return Name + "=" + Value;
        }
    }
}
