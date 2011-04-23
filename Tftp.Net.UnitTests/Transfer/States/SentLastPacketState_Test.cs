using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Tftp.Net.Transfer.States;

namespace Tftp.Net.UnitTests
{
    [TestFixture]
    class SentLastPacketState_Test
    {
        private TransferStub transfer;

        [SetUp]
        public void Setup()
        {
            transfer = new TransferStub();
            transfer.SetState(new SentLastPacket(transfer, 20));
        }

        [Test]
        public void HandlesCancel()
        {
            transfer.Cancel(TftpErrorPacket.IllegalOperation);
            Assert.IsInstanceOf<Closed>(transfer.State); 
        }

        [Test]
        public void HandlesError()
        {
            bool OnErrorWasCalled = false;
            transfer.OnError += delegate(ITftpTransfer t, TftpTransferError error) { OnErrorWasCalled = true; };
            Assert.IsFalse(OnErrorWasCalled);
            transfer.OnCommand(new Error(123, "Error Message"));
            Assert.IsTrue(OnErrorWasCalled);
            Assert.IsInstanceOf<Closed>(transfer.State);
        }

        [Test]
        public void HandlesAcknowledgement()
        {
            bool OnFinishedWasCalled = false;
            transfer.OnFinished += delegate(ITftpTransfer t) { OnFinishedWasCalled = true; };
            Assert.IsFalse(OnFinishedWasCalled);
            transfer.OnCommand(new Acknowledgement(20));
            Assert.IsTrue(OnFinishedWasCalled);
            Assert.IsInstanceOf<Closed>(transfer.State);
        }

        [Test]
        public void IgnoresWrongAcknowledgement()
        {
            transfer.OnCommand(new Acknowledgement(19));
            Assert.IsInstanceOf<SentLastPacket>(transfer.State);
        }
    }
}
