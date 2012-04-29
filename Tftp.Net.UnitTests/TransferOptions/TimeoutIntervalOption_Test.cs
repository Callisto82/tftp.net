using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Tftp.Net.TransferOptions.Handlers;
using System.IO;
using Tftp.Net.TransferOptions;

namespace Tftp.Net.UnitTests.TransferOptions
{
    [TestFixture]
    class TimeoutIntervalOption_Test
    {
        private TransferStub transfer;
        private TimeoutIntervalOption timeoutOption;

        [SetUp]
        public void Setup()
        {
            transfer = new TransferStub(new MemoryStream(new byte[4001]));
            timeoutOption = new TimeoutIntervalOption();
        }

        [Test]
        public void OptionIsSupportedByDefault()
        {
            Assert.IsTrue(TransferOptionHandlers.All.Any(x => x is TimeoutIntervalOption));
        }

        [Test]
        public void AcceptsValidTimeout()
        {
            Assert.IsTrue(timeoutOption.ApplyOption(transfer, new TransferOption("timeout", "10")));
            Assert.AreEqual(10, transfer.RetryTimeout.TotalSeconds);
        }

        [Test]
        public void AcceptsMinTimeout()
        {
            Assert.IsTrue(timeoutOption.ApplyOption(transfer, new TransferOption("timeout", "1")));
            Assert.AreEqual(1, transfer.RetryTimeout.TotalSeconds);
        }

        [Test]
        public void AcceptsMaxTimeout()
        {
            Assert.IsTrue(timeoutOption.ApplyOption(transfer, new TransferOption("timeout", "255")));
            Assert.AreEqual(255, transfer.RetryTimeout.TotalSeconds);
        }

        [Test]
        public void RejectsTimeoutTooLow()
        {
            Assert.IsFalse(timeoutOption.ApplyOption(transfer, new TransferOption("timeout", "0")));
            Assert.AreNotEqual(0, transfer.RetryTimeout.TotalSeconds);
        }

        [Test]
        public void RejectsTimeoutTooHigh()
        {
            Assert.IsFalse(timeoutOption.ApplyOption(transfer, new TransferOption("timeout", "256")));
            Assert.AreNotEqual(256, transfer.RetryTimeout.TotalSeconds);
        }

        [Test]
        public void RejectsNonIntegerTimeout()
        {
            Assert.IsFalse(timeoutOption.ApplyOption(transfer, new TransferOption("timeout", "blub")));
        }
    }
}
