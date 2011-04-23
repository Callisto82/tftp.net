using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Tftp.Net.Transfer.States
{
    class StartOutgoingRead : BaseState
    {
        public StartOutgoingRead(TftpTransfer context)
            : base(context) { }

        public override void OnStart()
        {
            Context.SetState(new SendReadRequest(Context));
        }

        public override void OnCancel(TftpErrorPacket reason)
        {
            Context.SetState(new Closed(Context));
        }
    }
}
