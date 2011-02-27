using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Tftp.Net.Transfer
{
    abstract class TransferWithTimeout : TftpTransfer
    {
        private Timer timer;
        private const int CALLBACK_FREQUENCY_MSECS = 500;

        public TransferWithTimeout(Channel.IChannel connection, string filename)
            : base(connection, filename)
        {
            timer = new Timer(TimerCallback, null, CALLBACK_FREQUENCY_MSECS, CALLBACK_FREQUENCY_MSECS);
        }

        void TimerCallback(object state)
        {
            lock (this)
            {
                //Are we already dispoed?
                if (timer == null)
                    return;

                this.state.OnTimer();
            }
        }

        public override void Dispose()
        {
            lock (this)
            {
                if (timer != null)
                {
                    timer.Dispose();
                    timer = null;
                }

                base.Dispose();
            }
        }
    }
}
