using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Tftp.Net.Transfer.States
{
    class StartOutgoingRead : BaseState
    {
        public override void OnStart()
        {
            Context.SetState(new SendReadRequest());
        }

        public override void OnCancel(TftpErrorPacket reason)
        {
            Context.SetState(new Closed());
        }
    }
}
