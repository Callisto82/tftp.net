using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tftp.Net.Trace;

namespace Tftp.Net.Transfer.States
{
    class StateWithNetworkTimeout : BaseState
    {
        private readonly SimpleTimer timer;
        private ITftpCommand lastCommand;
        private int retriesUsed;

        public StateWithNetworkTimeout(TftpTransfer context)
            : base(context) 
        {
            timer = new SimpleTimer(context.RetryTimeout);
            retriesUsed = 0;
        }

        public override void OnTimer()
        {
            if (timer.IsTimeout())
            {
                TftpTrace.Trace("Network timeout.", Context);
                timer.Restart();

                if (retriesUsed++ >= Context.RetryCount)
                {
                    TftpTransferError error = new TimeoutError(Context.RetryTimeout, Context.RetryCount);
                    Context.SetState(new ReceivedError(Context, error));
                }
                else
                    HandleTimeout();
            }
        }

        private void HandleTimeout()
        {
            if (lastCommand != null)
            {
                Context.GetConnection().Send(lastCommand);
            }
        }

        protected void SendAndRepeat(ITftpCommand command)
        {
            Context.GetConnection().Send(command);
            lastCommand = command;
            ResetTimeout();
        }

        protected void ResetTimeout() 
        {
            timer.Restart();
            retriesUsed = 0;
        }
    }
}
