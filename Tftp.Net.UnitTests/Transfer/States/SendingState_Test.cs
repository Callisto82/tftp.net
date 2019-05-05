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
            transfer.SetState(new Sending());
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
            transferWithLowTimeout.RetryTimeout = new TimeSpan(0);
            transferWithLowTimeout.SetState(new Sending());

            Assert.IsTrue(transferWithLowTimeout.CommandWasSent(typeof(Data)));
            transferWithLowTimeout.SentCommands.Clear();

            transferWithLowTimeout.OnTimer();
            Assert.IsTrue(transferWithLowTimeout.CommandWasSent(typeof(Data)));
        }

        [Test]
        public void TimeoutWhenNoAnswerIsReceivedAndRetryCountIsExceeded()
        {
            TransferStub transferWithLowTimeout = new TransferStub(new MemoryStream(new byte[5000]));
            transferWithLowTimeout.RetryTimeout = new TimeSpan(0);
            transferWithLowTimeout.RetryCount = 1;
            transferWithLowTimeout.SetState(new Sending());

            transferWithLowTimeout.OnTimer();
            Assert.IsFalse(transferWithLowTimeout.HadNetworkTimeout);
            transferWithLowTimeout.OnTimer();
            Assert.IsTrue(transferWithLowTimeout.HadNetworkTimeout);
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
        public void IncreasesBlockCountWithEachAcknowledgement()
        {
            Assert.AreEqual(1, (transfer.SentCommands.Last() as Data).BlockNumber);

            transfer.OnCommand(new Acknowledgement(1));

            Assert.AreEqual(2, (transfer.SentCommands.Last() as Data).BlockNumber);
        }
        
        [Test]
        public void BlockCountWrapsAroundTo0()
        {
            SetupTransferThatWillWrapAroundBlockCount();

            RunTransferUntilBlockCount(65535);
            transfer.OnCommand(new Acknowledgement(65535));

            Assert.AreEqual(0, (transfer.SentCommands.Last() as Data).BlockNumber);
        }

        [Test]
        public void BlockCountWrapsAroundTo1()
        {
            SetupTransferThatWillWrapAroundBlockCount();
            transfer.BlockCounterWrapping = BlockCounterWrapAround.ToOne;

            RunTransferUntilBlockCount(65535);
            transfer.OnCommand(new Acknowledgement(65535));

            Assert.AreEqual(1, (transfer.SentCommands.Last() as Data).BlockNumber);
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
            transfer.OnProgress += delegate(ITftpTransfer t, TftpTransferProgress progress) { Assert.AreEqual(transfer, t); Assert.IsTrue(progress.TransferredBytes > 0); onProgressWasCalled = true; };

            Assert.IsFalse(onProgressWasCalled);
            transfer.OnCommand(new Acknowledgement(1));
            Assert.IsTrue(onProgressWasCalled);
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

        private void SetupTransferThatWillWrapAroundBlockCount()
        {
            transfer = new TransferStub(new MemoryStream(new byte[70000]));
            transfer.BlockSize = 1;
            transfer.SetState(new Sending());
        }

        private void RunTransferUntilBlockCount(int targetBlockCount)
        {
            while ((transfer.SentCommands.Last() as Data).BlockNumber != targetBlockCount)
                transfer.OnCommand(new Acknowledgement((transfer.SentCommands.Last() as Data).BlockNumber));
        }
    }
}
