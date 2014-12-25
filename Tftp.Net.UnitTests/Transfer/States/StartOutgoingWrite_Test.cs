using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Tftp.Net.Transfer.States;
using System.IO;

namespace Tftp.Net.UnitTests.Transfer.States
{
    [TestFixture]
    class StartOutgoingWrite_Test 
    {
        private TransferStub transfer;

        [SetUp]
        public void Setup()
        {
            transfer = new TransferStub();
            transfer.SetState(new StartOutgoingWrite());
        }

        [Test]
        public void CanCancel()
        {
            transfer.Cancel(TftpErrorPacket.IllegalOperation);
            Assert.IsInstanceOf<Closed>(transfer.State);
        }

        [Test]
        public void IgnoresCommands()
        {
            transfer.OnCommand(new Error(5, "Hallo Welt"));
            Assert.IsInstanceOf<StartOutgoingWrite>(transfer.State);
        }

        [Test]
        public void CanStart()
        {
            transfer.Start(new MemoryStream());
            Assert.IsInstanceOf<SendWriteRequest>(transfer.State);
        }

        [Test]
        public void FillsTransferSizeIfPossible()
        {
            transfer.ExpectedSize = 123;
            transfer.Start(new StreamThatThrowsExceptionWhenReadingLength());
            Assert.IsTrue(WasTransferSizeOptionRequested());
        }

        [Test]
        public void FillsTransferSizeFromStreamIfPossible()
        {
            transfer.Start(new MemoryStream(new byte[] {1}));
            Assert.IsTrue(WasTransferSizeOptionRequested());
        }

        [Test]
        public void DoesNotFillTransferSizeWhenNotAvailable()
        {
            transfer.Start(new StreamThatThrowsExceptionWhenReadingLength());
            Assert.IsFalse(WasTransferSizeOptionRequested());
        }

        private bool WasTransferSizeOptionRequested()
        {
            WriteRequest wrq = (WriteRequest)transfer.SentCommands.Last();
            return wrq.Options.Any(x => x.Name == "tsize");
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
