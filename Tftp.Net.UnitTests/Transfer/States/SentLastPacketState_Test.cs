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
        [Test]
        public void CanEnter()
        {
            TransferStub transfer = new TransferStub();
            transfer.SetState(new SentLastPacket(transfer, 20));
        }

        [Test]
        public void HandlesCancel()
        {
            TransferStub transfer = new TransferStub();
            transfer.SetState(new SentLastPacket(transfer, 20));
            transfer.Cancel();
            Assert.IsInstanceOf<Closed>(transfer.State); 
        }

        [Test]
        public void HandlesError()
        {
            bool OnErrorWasCalled = false;
            TransferStub transfer = new TransferStub();
            transfer.OnError += delegate(ITftpTransfer t, ushort code, string error) { OnErrorWasCalled = true; };
            transfer.SetState(new SentLastPacket(transfer, 20));
            Assert.IsFalse(OnErrorWasCalled);
            transfer.OnCommand(new Error(123, "Error Message"));
            Assert.IsTrue(OnErrorWasCalled);
            Assert.IsInstanceOf<Closed>(transfer.State);
        }

        [Test]
        public void HandlesAcknowledgement()
        {
            bool OnFinishedWasCalled = false;
            TransferStub transfer = new TransferStub();
            transfer.OnFinished += delegate(ITftpTransfer t) { OnFinishedWasCalled = true; };

            transfer.SetState(new SentLastPacket(transfer, 20));
            Assert.IsFalse(OnFinishedWasCalled);
            transfer.OnCommand(new Acknowledgement(20));
            Assert.IsTrue(OnFinishedWasCalled);

            Assert.IsInstanceOf<Closed>(transfer.State);
        }

        [Test]
        public void IgnoresWrongAcknowledgement()
        {
            TransferStub transfer = new TransferStub();
            transfer.SetState(new SentLastPacket(transfer, 20));
            transfer.OnCommand(new Acknowledgement(19));
            Assert.IsInstanceOf<SentLastPacket>(transfer.State);
        }
    }
}
