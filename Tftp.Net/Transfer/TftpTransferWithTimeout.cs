using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Tftp.Net.Transfer
{
    class TftpTransferWithTimeout : TftpTransfer
    {
        private Timer timer;

        public TftpTransferWithTimeout()
        {
            timer = new Timer(TimerCallback, null, 15, 15);
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
