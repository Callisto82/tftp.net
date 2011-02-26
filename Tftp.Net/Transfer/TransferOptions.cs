using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tftp.Net.Transfer
{
    class TransferOptions : ITftpTransferOptions
    {
        private readonly Dictionary<String, TftpTransferOption> options = new Dictionary<string, TftpTransferOption>();

        public void Set(IEnumerable<TftpTransferOption> initialOptions)
        {
            this.options.Clear();

            foreach (TftpTransferOption option in initialOptions)
                options.Add(option.Name, option);
        }

        public void Add(string name, string value)
        {
            if (options.ContainsKey(name))
                throw new ArgumentException("Option is already present.");

            options.Add(name, new TftpTransferOption(name, value));
        }

        public void Remove(String name)
        {
            options.Remove(name);
        }

        public IEnumerator<TftpTransferOption> GetEnumerator()
        {
            return options.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    class ReadonlyOptions : ITftpTransferOptions
    {
        private readonly ITftpTransferOptions decoratee;

        public ReadonlyOptions(ITftpTransferOptions decoratee)
        {
            this.decoratee = decoratee;
        }

        public void Add(string name, string value)
        {
            throw new NotSupportedException();
        }

        public IEnumerator<TftpTransferOption> GetEnumerator()
        {
            return decoratee.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return decoratee.GetEnumerator();
        }
    }
}
