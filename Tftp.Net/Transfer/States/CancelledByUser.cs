using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tftp.Net.Transfer.States
{
    class CancelledByUser : BaseState
    {
        public CancelledByUser(TftpTransfer context)
            : base(context) { }

        public override void OnStateEnter()
        {
            Error command = new Error(0, "Transfer cancelled by user.");
            Context.GetConnection().Send(command);
            Context.SetState(new Closed(Context));
        }
    }
}
