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
    class SendWriteRequest_Test
    {
        private TransferStub transfer;

        [SetUp]
        public void Setup()
        {
            transfer = new TransferStub(new MemoryStream(new byte[5000]));
            transfer.SetState(new SendWriteRequest());
        }

        [Test]
        public void CanCancel()
        {
            transfer.Cancel(TftpErrorPacket.IllegalOperation);
            Assert.IsInstanceOf<Closed>(transfer.State);
            Assert.IsTrue(transfer.CommandWasSent(typeof(Error)));
        }

        [Test]
        public void SendsWriteRequest()
        {
            TransferStub transfer = new TransferStub(new MemoryStream(new byte[5000]));
            transfer.SetState(new SendWriteRequest());
            Assert.IsTrue(transfer.CommandWasSent(typeof(WriteRequest)));
        }

        [Test]
        public void HandlesAcknowledgement()
        {
            transfer.OnCommand(new Acknowledgement(0));
            Assert.IsInstanceOf<Sending>(transfer.State);
        }

        [Test]
        public void IgnoresWrongAcknowledgement()
        {
            transfer.OnCommand(new Acknowledgement(5));
            Assert.IsInstanceOf<SendWriteRequest>(transfer.State);
        }

        [Test]
        public void HandlesOptionAcknowledgement()
        {
            transfer.BlockSize = 999;
            transfer.OnCommand(new OptionAcknowledgement(new TransferOption[] { new TransferOption("blksize", "800") }));
            Assert.AreEqual(800, transfer.BlockSize);
        }

        [Test]
        public void HandlesMissingOptionAcknowledgement()
        {
            transfer.BlockSize = 999;
            transfer.OnCommand(new Acknowledgement(0));
            Assert.AreEqual(512, transfer.BlockSize);
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
        public void ResendsRequest()
        {
            TransferStub transferWithLowTimeout = new TransferStub(new MemoryStream());
            transferWithLowTimeout.RetryTimeout = new TimeSpan(0);
            transferWithLowTimeout.SetState(new SendWriteRequest());

            Assert.IsTrue(transferWithLowTimeout.CommandWasSent(typeof(WriteRequest)));
            transferWithLowTimeout.SentCommands.Clear();

            transferWithLowTimeout.OnTimer();
            Assert.IsTrue(transferWithLowTimeout.CommandWasSent(typeof(WriteRequest)));
        }

        [Test]
        public void TimeoutWhenNoAnswerIsReceivedAndRetryCountIsExceeded()
        {
            TransferStub transferWithLowTimeout = new TransferStub(new MemoryStream(new byte[5000]));
            transferWithLowTimeout.RetryTimeout = new TimeSpan(0);
            transferWithLowTimeout.RetryCount = 1;
            transferWithLowTimeout.SetState(new SendWriteRequest());

            transferWithLowTimeout.OnTimer();
            Assert.IsFalse(transferWithLowTimeout.HadNetworkTimeout);
            transferWithLowTimeout.OnTimer();
            Assert.IsTrue(transferWithLowTimeout.HadNetworkTimeout);
        }
    }
}
