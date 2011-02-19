using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Tftp.Net.Transfer.States
{
    class StartIncomingWrite : BaseState
    {
        public StartIncomingWrite(TftpTransfer context)
            : base(context) { }

        public override void OnStart()
        {
            //Acknowledge the write request
            Context.GetConnection().Send(new Acknowledgement(0));

            //And start receiving
            Context.SetState(new Receiving(Context));
        }

        public override void OnCancel()
        {
            Context.SetState(new CancelledByUser(Context));
        }
    }
}
