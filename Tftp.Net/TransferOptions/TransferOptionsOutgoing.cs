using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tftp.Net.TransferOptions
{
    class TransferOptionsOutgoing : TransferOptionsBase
    {
        public override void Request(string option, string value)
        {
            options.Add(option, new TransferOption(option, value));
        }
    }
}
