using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tftp.Net.Transfer;

namespace Tftp.Net.Trace
{
    public static class TftpTrace
    {
        public static bool Enabled { get; set; }

        static TftpTrace()
        {
            Enabled = true;
        }

        internal static void Trace(String message, TftpTransfer transfer)
        {
            if (!Enabled)
                return;

            System.Diagnostics.Trace.WriteLine(message, transfer.ToString());
        }
    }
}
