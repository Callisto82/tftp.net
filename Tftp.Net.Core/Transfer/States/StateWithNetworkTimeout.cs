using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tftp.Net.Trace;

namespace Tftp.Net.Transfer.States
{
    class StateWithNetworkTimeout : BaseState
    {
        private SimpleTimer timer;
        private ITftpCommand lastCommand;
        private int retriesUsed = 0;

        public override void OnStateEnter()
        {
            timer = new SimpleTimer(Context.RetryTimeout);
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
                    Context.SetState(new ReceivedError(error));
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
