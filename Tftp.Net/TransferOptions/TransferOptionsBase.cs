using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tftp.Net.TransferOptions
{
    abstract class TransferOptionsBase : ITftpTransferOptions
    {
        protected readonly Dictionary<String, TransferOption> options = new Dictionary<string, TransferOption>();

        public IEnumerator<ITftpTransferOption> GetEnumerator()
        {
            return options.Values.Cast<ITftpTransferOption>().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public abstract void Request(string option, string value);
    }
}
