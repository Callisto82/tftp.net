using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Tftp.Net.Transfer;
using System.Threading;

namespace Tftp.Net.UnitTests.Transfer
{
    [TestFixture]
    class SimpleTimer_Test
    {
        [Test]
        public void TimeoutDoesNotStayActive()
        {
            SimpleTimer timer = new SimpleTimer(new TimeSpan(100));
            Assert.IsFalse(timer.IsTimeout());
            Thread.Sleep(200);
            Assert.IsTrue(timer.IsTimeout());
            Assert.IsFalse(timer.IsTimeout());
        }

        [Test]
        public void Reset()
        {
            SimpleTimer timer = new SimpleTimer(new TimeSpan(100));
            Assert.IsFalse(timer.IsTimeout());
            Thread.Sleep(200);
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

