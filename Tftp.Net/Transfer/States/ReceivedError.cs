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

        public ReceivedError(Error error)
        {
            TftpErrorPacket errorReceived;

            /* Create the Error Package while handling the case where the sender */
            /* did not provide an error message. */
            try
            {
                errorReceived = new TftpErrorPacket(error.ErrorCode, error.Message);
            }
            catch(ArgumentException)
            {
                errorReceived = new TftpErrorPacket(error.ErrorCode, "No Message Provided");
            }

            this.error = errorReceived;
        }

        public ReceivedError(TftpTransferError error)
        {
            this.error = error;
        }

        public override void OnStateEnter()
        {
            TftpTrace.Trace("Received error: " + error, Context);
            Context.RaiseOnError(error);
            Context.SetState(new Closed());
        }
    }
}
