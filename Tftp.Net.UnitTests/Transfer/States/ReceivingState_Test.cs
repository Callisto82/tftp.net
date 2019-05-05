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
            transfer.SetState(new Receiving());
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
        public void BlockCounterWrapsAroundToZero()
        {
            TransferUntilBlockCounterWrapIsAboutToWrap();

            transfer.OnCommand(new Data(0, new byte[1]));

            Assert.AreEqual(0, (transfer.SentCommands.Last() as Acknowledgement).BlockNumber);
        }

        [Test]
        public void BlockCounterWrapsAroundToOne()
        {
            transfer.BlockCounterWrapping = BlockCounterWrapAround.ToOne;
            TransferUntilBlockCounterWrapIsAboutToWrap();

            transfer.OnCommand(new Data(1, new byte[1]));

            Assert.AreEqual(1, (transfer.SentCommands.Last() as Acknowledgement).BlockNumber);
        }

        private void TransferUntilBlockCounterWrapIsAboutToWrap()
        {
            transfer.BlockSize = 1;
            for (int i = 1; i <= 65535; i++)
                transfer.OnCommand(new Data((ushort)i, new byte[1]));
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
            transfer.OnProgress += delegate(ITftpTransfer t, TftpTransferProgress progress) { Assert.AreEqual(transfer, t); Assert.IsTrue(progress.TransferredBytes > 0); onProgressWasCalled = true; };

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

        [Test]
        public void TimeoutWhenNoDataIsReceivedAndRetryCountIsExceeded()
        {
            TransferStub transferWithLowTimeout = new TransferStub(new MemoryStream(new byte[5000]));
            transferWithLowTimeout.RetryTimeout = new TimeSpan(0);
            transferWithLowTimeout.RetryCount = 1;
            transferWithLowTimeout.SetState(new Receiving());

            transferWithLowTimeout.OnTimer();
            Assert.IsFalse(transferWithLowTimeout.HadNetworkTimeout);
            transferWithLowTimeout.OnTimer();
            Assert.IsTrue(transferWithLowTimeout.HadNetworkTimeout);
        }
    }
}
