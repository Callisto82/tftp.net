using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Tftp.Net.Transfer.States
{
    class StartOutgoingWrite : BaseState
    {
        public override void OnStart()
        {
            Context.FillOrDisableTransferSizeOption();
            Context.SetState(new SendWriteRequest());
        }

        public override void OnCancel(TftpErrorPacket reason)
        {
            Context.SetState(new Closed());
        }
    }
}
