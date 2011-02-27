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
    class SendingState_Test
    {
        private TransferStub transfer;

        [SetUp]
        public void Setup()
        {
            transfer = new TransferStub(new MemoryStream(new byte[5000]));
            transfer.SetState(new Sending(transfer));
        }

        [Test]
        public void SendsPacket()
        {
            Assert.IsTrue(transfer.CommandWasSent(typeof(Data)));
        }

        [Test]
        public void ResendsPacket()
        {
            TransferStub transferWithLowTimeout = new TransferStub(new MemoryStream(new byte[5000]));
            transferWithLowTimeout.Timeout = new TimeSpan(0);
            transferWithLowTimeout.SetState(new Sending(transferWithLowTimeout));

            Assert.IsTrue(transferWithLowTimeout.CommandWasSent(typeof(Data)));
            transferWithLowTimeout.SentCommands.Clear();

            transferWithLowTimeout.OnTimer();
            Assert.IsTrue(transferWithLowTimeout.CommandWasSent(typeof(Data)));
        }

        [Test]
        public void HandlesAcknowledgment()
        {
            transfer.SentCommands.Clear();
            Assert.IsFalse(transfer.CommandWasSent(typeof(Data)));

            transfer.OnCommand(new Acknowledgement(1));
            Assert.IsTrue(transfer.CommandWasSent(typeof(Data)));
        }

        [Test]
        public void IgnoresWrongAcknowledgment()
        {
            transfer.SentCommands.Clear();
            Assert.IsFalse(transfer.CommandWasSent(typeof(Data)));

            transfer.OnCommand(new Acknowledgement(0));
            Assert.IsFalse(transfer.CommandWasSent(typeof(Data)));
        }

        [Test]
        public void RaisesProgress()
        {
            bool onProgressWasCalled = false;
            transfer.OnProgress += delegate(ITftpTransfer t, int bytesTransferred) { Assert.AreEqual(transfer, t); Assert.IsTrue(bytesTransferred > 0); onProgressWasCalled = true; };

            Assert.IsFalse(onProgressWasCalled);
            transfer.OnCommand(new Acknowledgement(1));
            Assert.IsTrue(onProgressWasCalled);
        }

        [Test]
        public void CanCancel()
        {
            transfer.Cancel();
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
