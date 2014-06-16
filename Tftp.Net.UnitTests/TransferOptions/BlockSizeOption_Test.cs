using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Tftp.Net.Transfer;

namespace Tftp.Net.UnitTests.TransferOptions
{
    [TestFixture]
    class BlockSizeOption_Test
    {
        private TftpTransferOptions options;

        [SetUp]
        public void Setup()
        {
            options = new TftpTransferOptions();
        }

        [Test]
        public void AcceptRegularOption()
        {
            Parse(new TransferOption("blksize", "16"));
            Assert.IsTrue(options.IsBlockSizeOptionActive);
            Assert.AreEqual(16, options.BlockSize);
        }

        [Test]
        public void IgnoreUnknownOption()
        {
            Parse(new TransferOption("blub", "16"));
            Assert.AreEqual(512, options.BlockSize);
            Assert.IsFalse(options.IsBlockSizeOptionActive);
        }

        [Test]
        public void IgnoreInvalidValue()
        {
            Parse(new TransferOption("blksize", "not-a-number"));
            Assert.AreEqual(512, options.BlockSize);
            Assert.IsFalse(options.IsBlockSizeOptionActive);
        }

        [Test]
        public void AcceptMinBlocksize()
        {
            Parse(new TransferOption("blksize", "8"));
            Assert.IsTrue(options.IsBlockSizeOptionActive);

            Parse(new TransferOption("blksize", "7"));
            Assert.IsFalse(options.IsBlockSizeOptionActive);
        }

        [Test]
        public void AcceptMaxBlocksize()
        {
            Parse(new TransferOption("blksize", "65464"));
            Assert.IsTrue(options.IsBlockSizeOptionActive);

            Parse(new TransferOption("blksize", "65465"));
            Assert.IsFalse(options.IsBlockSizeOptionActive);
        }

        private void Parse(TransferOption option)
        {
            options.SetActiveOptions(new TransferOption[] { option });
        }
    }
}
