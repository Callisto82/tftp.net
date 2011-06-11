using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tftp.Net.Trace;

namespace Tftp.Net.Transfer.States
{
    class ReceivedError : BaseState
    {
        private readonly TftpTransferError error;

        public ReceivedError(TftpTransfer context, Error error)
            : this(context, new TftpErrorPacket(error.ErrorCode, error.Message)) { }

        public ReceivedError(TftpTransfer context, TftpTransferError error)
            : base(context)
        {
            this.error = error;
        }

        public override void OnStateEnter()
        {
            TftpTrace.Trace("Received error: " + error, Context);
            Context.RaiseOnError(error);
            Context.SetState(new Closed(Context));
        }
    }
}
