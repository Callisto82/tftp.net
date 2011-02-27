using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tftp.Net.Transfer.States
{
    class ReceivedError : BaseState
    {
        private readonly TftpTransferError error;

        public ReceivedError(TftpTransfer context, Error error)
            : this(context, new ErrorFromRemoteEndpoint(error.ErrorCode, error.Message)) { }

        public ReceivedError(TftpTransfer context, TftpTransferError error)
            : base(context)
        {
            this.error = error;
        }

        public override void OnStateEnter()
        {
            Context.RaiseOnError(error);
            Context.SetState(new Closed(Context));
        }
    }
}
