using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Tftp.Net.Transfer.States;
using System.IO;
using Tftp.Net.Transfer;

namespace Tftp.Net.UnitTests
{
    [TestFixture]
    class StartIncomingReadState_Test
    {
        private TransferStub transfer;

        [SetUp]
        public void Setup()
        {
            transfer = new TransferStub();
            transfer.SetState(new StartIncomingRead(new TransferOption[] { new TransferOption("tsize", "0") }));
        }

        [TearDown]
        public void Teardown()
        {
        }

        [Test]
        public void CanCancel()
        {
            transfer.Cancel(TftpErrorPacket.IllegalOperation);
            Assert.IsTrue(transfer.CommandWasSent(typeof(Error)));
            Assert.IsInstanceOf<Closed>(transfer.State);
        }

        [Test]
        public void IgnoresCommands()
        {
            transfer.OnCommand(new Error(5, "Hallo Welt"));
            Assert.IsInstanceOf<StartIncomingRead>(transfer.State);
        }

        [Test]
        public void CanStartWithoutOptions()
        {
            transfer.SetState(new StartIncomingRead(new TransferOption[0]));
            transfer.Start(new MemoryStream(new byte[50000]));
            Assert.IsInstanceOf<Sending>(transfer.State);
        }

        [Test]
        public void CanStartWithOptions()
        {
            //Simulate that we got a request for a option
            transfer.SetState(new StartIncomingRead( new TransferOption[] { new TransferOption("blksize", "999") }));
            Assert.AreEqual(999, transfer.BlockSize);
            transfer.Start(new MemoryStream(new byte[50000]));
            Assert.IsInstanceOf<SendOptionAcknowledgementForReadRequest>(transfer.State);
            OptionAcknowledgement cmd = (OptionAcknowledgement)transfer.SentCommands.Last();
            cmd.Options.Contains(new TransferOption("blksize", "999"));
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
            transfer.Start(new MemoryStream(new byte[] { 1 }));
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
            OptionAcknowledgement oack = transfer.SentCommands.Last() as OptionAcknowledgement;
            return oack != null && oack.Options.Any(x => x.Name == "tsize");
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
