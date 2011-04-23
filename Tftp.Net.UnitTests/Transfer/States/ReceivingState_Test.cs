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
    class ReceivingState_Test
    {
        private MemoryStream ms;
        private TransferStub transfer;

        [SetUp]
        public void Setup()
        {
            ms = new MemoryStream();
            transfer = new TransferStub(ms);
            transfer.SetState(new Receiving(transfer));
        }

        [Test]
        public void ReceivesPacket()
        {
            transfer.OnCommand(new Data(1, new byte[100]));
            Assert.IsTrue(transfer.CommandWasSent(typeof(Acknowledgement)));
            Assert.AreEqual(100, ms.Length);
        }

        [Test]
        public void SendsAcknowledgement()
        {
            transfer.OnCommand(new Data(1, new byte[100]));
            Assert.IsTrue(transfer.CommandWasSent(typeof(Acknowledgement)));
        }

        [Test]
        public void IgnoresWrongPackets()
        {
            transfer.OnCommand(new Data(2, new byte[100]));
            Assert.IsFalse(transfer.CommandWasSent(typeof(Acknowledgement)));
            Assert.AreEqual(0, ms.Length);
        }

        [Test]
        public void RaisesFinished()
        {
            bool onFinishedWasCalled = false;
            transfer.OnFinished += delegate(ITftpTransfer t) { Assert.AreEqual(transfer, t); onFinishedWasCalled = true; };

            Assert.IsFalse(onFinishedWasCalled);
            transfer.OnCommand(new Data(1, new byte[100]));
            Assert.IsTrue(onFinishedWasCalled);
            Assert.IsInstanceOf<Closed>(transfer.State);
        }


        [Test]
        public void RaisesProgress()
        {
            bool onProgressWasCalled = false;
            transfer.OnProgress += delegate(ITftpTransfer t, int bytesTransferred) { Assert.AreEqual(transfer, t); Assert.IsTrue(bytesTransferred > 0); onProgressWasCalled = true; };

            Assert.IsFalse(onProgressWasCalled);
            transfer.OnCommand(new Data(1, new byte[1000]));
            Assert.IsTrue(onProgressWasCalled);
            Assert.IsInstanceOf<Receiving>(transfer.State);
        }

        [Test]
        public void CanCancel()
        {
            transfer.Cancel(TftpErrorPacket.IllegalOperation);
            Assert.IsTrue(transfer.CommandWasSent(typeof(Error)));
            Assert.IsInstanceOf<Closed>(transfer.State);
        }

        [Test]
        public void HandlesError()
        {
            bool onErrorWasCalled = false;
            transfer.OnError += delegate(ITftpTransfer t, TftpTransferError error) { onErrorWasCalled = true; };

            Assert.IsFalse(onErrorWasCalled);
            transfer.OnCommand(new Error(123, "Test Error"));
            Assert.IsTrue(onErrorWasCalled);

            Assert.IsInstanceOf<Closed>(transfer.State);
        }
    }
}
