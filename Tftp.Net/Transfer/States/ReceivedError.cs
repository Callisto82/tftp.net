using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tftp.Net.Transfer.States
{
    class ReceivedError : BaseState
    {
        private readonly ushort code;
        private readonly string error;

        public ReceivedError(TftpTransfer context, ushort code, String error)
            : base(context)
        {
            this.error = error;
            this.code = code;
        }

        public override void OnStateEnter()
        {
            Context.RaiseOnError(code, error);
            Context.SetState(new Closed(Context));
        }
    }
}
