using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Tftp.Net.Transfer.States;
using System.IO;
using Tftp.Net.Transfer;

namespace Tftp.Net.UnitTests.Transfer.States
{
    [TestFixture]
    class StartIncomingWriteState_Test
    {
        private TransferStub transfer;

        [SetUp]
        public void Setup()
        {
            transfer = new TransferStub();
            transfer.SetState(new StartIncomingWrite(new TransferOption[0]));
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
            Assert.IsInstanceOf<StartIncomingWrite>(transfer.State);
        }

        [Test]
        public void CanStartWithoutOptions()
        {
            transfer.Start(new MemoryStream(new byte[50000]));

            Assert.IsTrue(transfer.CommandWasSent(typeof(Acknowledgement)));
            Assert.IsInstanceOf<AcknowledgeWriteRequest>(transfer.State);
        }

        [Test]
        public void CanStartWithOptions()
        {
            transfer.SetState(new StartIncomingWrite(new TransferOption[] { new TransferOption("blksize", "999") }));
            Assert.AreEqual(999, transfer.BlockSize);
            transfer.Start(new MemoryStream(new byte[50000]));
            OptionAcknowledgement cmd = (OptionAcknowledgement)transfer.SentCommands.Last();
            cmd.Options.Contains(new TransferOption("blksize", "999"));
            Assert.IsInstanceOf<SendOptionAcknowledgementForWriteRequest>(transfer.State);
        }
    }
}
