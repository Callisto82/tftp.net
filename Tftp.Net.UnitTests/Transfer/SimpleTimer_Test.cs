using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Tftp.Net.Transfer;
using System.Threading;
using System.Threading.Tasks;

namespace Tftp.Net.UnitTests.Transfer
{
    [TestFixture]
    class SimpleTimer_Test
    {
        [Test]
        public void TimesOutWhenTimeoutIsReached()
        {
            SimpleTimer timer = new SimpleTimer(new TimeSpan(100));
            Assert.IsFalse(timer.IsTimeout());
            Task.Delay(200).Wait();
            Assert.IsTrue(timer.IsTimeout());
        }

        [Test]
        public void RestartingResetsTimeout()
        {
            SimpleTimer timer = new SimpleTimer(new TimeSpan(100));
            Assert.IsFalse(timer.IsTimeout());
            Task.Delay(200).Wait();
            Assert.IsTrue(timer.IsTimeout());
            timer.Restart();
            Assert.IsFalse(timer.IsTimeout());
        }

        [Test]
        public void ImmediateTimeout()
        {
            SimpleTimer timer = new SimpleTimer(new TimeSpan(0));
            Assert.IsTrue(timer.IsTimeout());
        }
    }
}

