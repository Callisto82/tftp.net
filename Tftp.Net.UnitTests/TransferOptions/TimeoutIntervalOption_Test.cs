using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using Tftp.Net.Transfer;

namespace Tftp.Net.UnitTests.TransferOptions
{
    [TestFixture]
    class TimeoutIntervalOption_Test
    {
        private TransferOptionSet options;

        [Test]
        public void AcceptsValidTimeout()
        {
            Parse(new TransferOption("timeout", "10"));
            Assert.IsTrue(options.IncludesTimeoutOption);
            Assert.AreEqual(10, options.Timeout);
        }

        [Test]
        public void AcceptsMinTimeout()
        {
            Parse(new TransferOption("timeout", "1"));
            Assert.IsTrue(options.IncludesTimeoutOption);
            Assert.AreEqual(1, options.Timeout);
        }

        [Test]
        public void AcceptsMaxTimeout()
        {
            Parse(new TransferOption("timeout", "255"));
            Assert.IsTrue(options.IncludesTimeoutOption);
            Assert.AreEqual(255, options.Timeout);
        }

        [Test]
        public void RejectsTimeoutTooLow()
        {
            Parse(new TransferOption("timeout", "0"));
            Assert.IsFalse(options.IncludesTimeoutOption);
            Assert.AreEqual(5, options.Timeout);
        }

        [Test]
        public void RejectsTimeoutTooHigh()
        {
            Parse(new TransferOption("timeout", "256"));
            Assert.IsFalse(options.IncludesTimeoutOption);
            Assert.AreEqual(5, options.Timeout);
        }

        [Test]
        public void RejectsNonIntegerTimeout()
        {
            Parse(new TransferOption("timeout", "blub"));
            Assert.IsFalse(options.IncludesTimeoutOption);
            Assert.AreEqual(5, options.Timeout);
        }

        private void Parse(TransferOption option)
        {
            options = new TransferOptionSet(new TransferOption[] { option });
        }
    }
}
