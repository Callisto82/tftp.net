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

        public override void OnStart(Stream data)
        {
            Context.SetState(new SendReadRequest(Context, data));
        }

        public override void OnCancel()
        {
            Context.SetState(new Closed(Context));
        }
    }
}
