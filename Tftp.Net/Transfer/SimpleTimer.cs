using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tftp.Net.Transfer
{
    /// <summary>
    /// Simple implementation of a timer.
    /// </summary>
    class SimpleTimer
    {
        /// <summary>
        /// Next DateTime at which a timeout will be triggered.
        /// </summary>
        private DateTime nextTimeout;

        /// <summary>
        /// After how much time will a timeout be triggered?
        /// </summary>
        private readonly TimeSpan timeout;

        public SimpleTimer(TimeSpan timeout)
        {
            this.timeout = timeout;
            Restart();
        }

        /// <summary>
        /// Restarts the timer
        /// </summary>
        public void Restart()
        {
            this.nextTimeout = DateTime.Now.Add(timeout);
        }

        /// <summary>
        /// Returns true of the timeout was triggered since the last call to IsTimeout()
        /// </summary
        public bool IsTimeout()
        {
            bool ret = DateTime.Now >= nextTimeout;

            if (ret)
                nextTimeout = DateTime.MaxValue;

            return ret;
        }
    }
}
