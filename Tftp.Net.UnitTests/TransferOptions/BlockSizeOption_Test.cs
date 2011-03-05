using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Tftp.Net.TransferOptions;
using Tftp.Net.TransferOptions.Handlers;

namespace Tftp.Net.UnitTests.TransferOptions
{
    [TestFixture]
    class BlockSizeOption_Test
    {
        private TransferStub transfer;
        private BlockSizeOption blockSizeOption;

        [SetUp]
        public void Setup()
        {
            transfer = new TransferStub();
            blockSizeOption = new BlockSizeOption();
        }

        [Test]
        public void OptionIsSupportedByDefault()
        {
            Assert.IsTrue(TransferOptionHandlers.All.Any(x => x is BlockSizeOption));
        }

        [Test]
        public void AcceptRegularOption()
        {
            Assert.AreNotEqual(16, transfer.BlockSize);
            ITftpTransferOption option = new TransferOption("blksize", "16");
            Assert.IsTrue(blockSizeOption.ApplyOption(transfer, option));
            Assert.AreEqual(16, transfer.BlockSize);
        }

        [Test]
        public void IgnoreUnknownOption()
        {
            ITftpTransferOption option = new TransferOption("blub", "16");
            Assert.IsFalse(blockSizeOption.ApplyOption(transfer, option));
        }

        [Test]
        public void IgnoreInvalidValue()
        {
            ITftpTransferOption option = new TransferOption("blksize", "not-a-number");
            Assert.IsFalse(blockSizeOption.ApplyOption(transfer, option));
        }

        [Test]
        public void AcceptMinBlocksize()
        {
            ITftpTransferOption option = new TransferOption("blksize", "8");
            Assert.IsTrue(blockSizeOption.ApplyOption(transfer, option));

            option = new TransferOption("blksize", "7");
            Assert.IsFalse(blockSizeOption.ApplyOption(transfer, option));
        }

        [Test]
        public void AcceptMaxBlocksize()
        {
            ITftpTransferOption option = new TransferOption("blksize", "65464");
            Assert.IsTrue(blockSizeOption.ApplyOption(transfer, option));

            option = new TransferOption("blksize", "65465");
            Assert.IsFalse(blockSizeOption.ApplyOption(transfer, option));
        }
    }
}
