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
        private TransferOptionSet options;

        [Test]
        public void ReadsTransferSize()
        {
            Parse(new TransferOption("tsize", "0"));
            Assert.IsTrue(options.IncludesTransferSizeOption);
            Assert.AreEqual(0, options.TransferSize);
        }

        [Test]
        public void RejectsNegativeTransferSize()
        {
            Parse(new TransferOption("tsize", "-1"));
            Assert.IsFalse(options.IncludesTransferSizeOption);
        }

        [Test]
        public void RejectsNonIntegerTransferSize()
        {
            Parse(new TransferOption("tsize", "abc"));
            Assert.IsFalse(options.IncludesTransferSizeOption);
        }

        private void Parse(TransferOption option)
        {
            options = new TransferOptionSet(new TransferOption[] { option });
        }
    }
}
