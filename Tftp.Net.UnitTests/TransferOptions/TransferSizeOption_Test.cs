using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Tftp.Net.Transfer;
using System.IO;

namespace Tftp.Net.UnitTests.TransferOptions
{
    [TestFixture]
    class TransferSizeOption_Test
    {
        private TftpTransferOptions options;

        [SetUp]
        public void Setup()
        {
            options = new TftpTransferOptions();
        }

        [Test]
        public void ReadsTransferSize()
        {
            Parse(new TransferOption("tsize", "0"));
            Assert.IsTrue(options.IsTransferSizeOptionActive);
            Assert.AreEqual(0, options.TransferSize);
        }

        [Test]
        public void RejectsNegativeTransferSize()
        {
            Parse(new TransferOption("tsize", "-1"));
            Assert.IsFalse(options.IsTransferSizeOptionActive);
        }

        [Test]
        public void RejectsNonIntegerTransferSize()
        {
            Parse(new TransferOption("tsize", "abc"));
            Assert.IsFalse(options.IsTransferSizeOptionActive);
        }

        private void Parse(TransferOption option)
        {
            options.SetActiveOptions(new TransferOption[] { option });
        }
    }
}
