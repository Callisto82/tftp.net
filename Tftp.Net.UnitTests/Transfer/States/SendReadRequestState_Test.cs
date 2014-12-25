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
    class SendReadRequestState_Test
    {
        private MemoryStream ms;
        private TransferStub transfer;

        [SetUp]
        public void Setup()
        {
            ms = new MemoryStream();
            transfer = new TransferStub(ms);
            transfer.SetState(new SendReadRequest());
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
        public void HandlesData()
        {            
            transfer.OnCommand(new Data(1, new byte[10]));
            Assert.IsTrue(transfer.CommandWasSent(typeof(Acknowledgement)));
            Assert.IsInstanceOf<Closed>(transfer.State);
            Assert.AreEqual(10, ms.Length);
        }

        [Test]
        public void HandlesOptionAcknowledgement()
        {
            transfer.BlockSize = 999;
            transfer.OnCommand(new OptionAcknowledgement(new TransferOption[] { new TransferOption("blksize", "999") }));
            Assert.IsTrue(transfer.CommandWasSent(typeof(Acknowledgement)));
            Assert.AreEqual(999, transfer.BlockSize);
        }

        [Test]
        public void HandlesMissingOptionAcknowledgement()
        {
            transfer.BlockSize = 999;
            transfer.OnCommand(new Data(1, new byte[10]));
            Assert.AreEqual(512, transfer.BlockSize);
        }

        [Test]
        public void SendsRequest()
        {
            Assert.IsTrue(transfer.CommandWasSent(typeof(ReadRequest)));
        }

        [Test]
        public void ResendsRequest()
        {
            TransferStub transferWithLowTimeout = new TransferStub(new MemoryStream());
            transferWithLowTimeout.RetryTimeout = new TimeSpan(0);
            transferWithLowTimeout.SetState(new SendReadRequest());

            Assert.IsTrue(transferWithLowTimeout.CommandWasSent(typeof(ReadRequest)));
            transferWithLowTimeout.SentCommands.Clear();

            transferWithLowTimeout.OnTimer();
            Assert.IsTrue(transferWithLowTimeout.CommandWasSent(typeof(ReadRequest)));
        }

        [Test]
        public void TimeoutWhenNoAnswerIsReceivedAndRetryCountIsExceeded()
        {
            TransferStub transferWithLowTimeout = new TransferStub(new MemoryStream(new byte[5000]));
            transferWithLowTimeout.RetryTimeout = new TimeSpan(0);
            transferWithLowTimeout.RetryCount = 1;
            transferWithLowTimeout.SetState(new SendReadRequest());

            transferWithLowTimeout.OnTimer();
            Assert.IsFalse(transferWithLowTimeout.HadNetworkTimeout);
            transferWithLowTimeout.OnTimer();
            Assert.IsTrue(transferWithLowTimeout.HadNetworkTimeout);
        }
    }
}
