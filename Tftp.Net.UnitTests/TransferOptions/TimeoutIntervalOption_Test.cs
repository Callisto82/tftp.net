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
        private TftpTransferOptions options;

        [SetUp]
        public void Setup()
        {
            options = new TftpTransferOptions();
        }

        [Test]
        public void AcceptsValidTimeout()
        {
            Parse(new TransferOption("timeout", "10"));
            Assert.IsTrue(options.IsTimeoutOptionActive);
            Assert.AreEqual(TimeSpan.FromSeconds(10), options.Timeout);
        }

        [Test]
        public void AcceptsMinTimeout()
        {
            Parse(new TransferOption("timeout", "1"));
            Assert.IsTrue(options.IsTimeoutOptionActive);
            Assert.AreEqual(TimeSpan.FromSeconds(1), options.Timeout);
        }

        [Test]
        public void AcceptsMaxTimeout()
        {
            Parse(new TransferOption("timeout", "255"));
            Assert.IsTrue(options.IsTimeoutOptionActive);
            Assert.AreEqual(TimeSpan.FromSeconds(255), options.Timeout);
        }

        [Test]
        public void RejectsTimeoutTooLow()
        {
            Parse(new TransferOption("timeout", "0"));
            Assert.IsFalse(options.IsTimeoutOptionActive);
            Assert.AreEqual(TimeSpan.FromSeconds(5), options.Timeout);
        }

        [Test]
        public void RejectsTimeoutTooHigh()
        {
            Parse(new TransferOption("timeout", "256"));
            Assert.IsFalse(options.IsTimeoutOptionActive);
            Assert.AreEqual(TimeSpan.FromSeconds(5), options.Timeout);
        }

        [Test]
        public void RejectsNonIntegerTimeout()
        {
            Parse(new TransferOption("timeout", "blub"));
            Assert.IsFalse(options.IsTimeoutOptionActive);
            Assert.AreEqual(TimeSpan.FromSeconds(5), options.Timeout);
        }

        private void Parse(TransferOption option)
        {
            options.SetActiveOptions(new TransferOption[] { option });
        }
    }
}
