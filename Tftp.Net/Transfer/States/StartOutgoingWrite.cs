using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Tftp.Net.Transfer.States
{
    class StartOutgoingWrite : BaseState
    {
        public StartOutgoingWrite(TftpTransfer context)
            : base(context) { }

        public override void OnStart()
        {
            Context.SetState(new SendWriteRequest(Context));
        }

        public override void OnCancel(TftpErrorPacket reason)
        {
            Context.SetState(new Closed(Context));
        }
    }
}
