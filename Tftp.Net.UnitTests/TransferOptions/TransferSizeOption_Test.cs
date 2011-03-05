using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Tftp.Net.TransferOptions.Handlers;
using Tftp.Net.TransferOptions;
using System.IO;

namespace Tftp.Net.UnitTests.TransferOptions
{
    [TestFixture]
    class TransferSizeOption_Test
    {
        private TransferStub transfer;
        private TransferSizeOption transferSizeOption;

        [SetUp]
        public void Setup()
        {
            transfer = new TransferStub(new MemoryStream(new byte[4001]));
            transferSizeOption = new TransferSizeOption();
        }

        [Test]
        public void OptionIsSupportedByDefault()
        {
            Assert.IsTrue(TransferOptionHandlers.All.Any(x => x is TransferSizeOption));
        }

        [Test]
        public void ReadsTransferSize()
        {
            TransferOption option = new TransferOption("tsize", "");
            Assert.IsTrue(transferSizeOption.ApplyOption(transfer, option));
            Assert.AreEqual("4001", option.Value);
        }

        [Test]
        public void CannotReadTransferSize()
        {
            TransferStub stub = new TransferStub(null);
            TransferOption option = new TransferOption("tsize", "");
            Assert.IsFalse(transferSizeOption.ApplyOption(stub, option));
        }

        [Test]
        public void ErrorReadingTransferSize()
        {
            TransferStub stub = new TransferStub(new StreamThatThrowsExceptionWhenReadingLength());
            TransferOption option = new TransferOption("tsize", "");
            Assert.IsFalse(transferSizeOption.ApplyOption(stub, option));
        }

        private class StreamThatThrowsExceptionWhenReadingLength : MemoryStream
        {
            public override long Length
            {
                get
                {
                    throw new NotSupportedException();
                }
            }
        }
    }
}
