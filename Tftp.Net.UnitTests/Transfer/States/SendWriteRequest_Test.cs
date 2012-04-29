using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Tftp.Net.Transfer.States;
using System.IO;
using Tftp.Net.TransferOptions;

namespace Tftp.Net.UnitTests
{
    [TestFixture]
    class SendWriteRequest_Test
    {
        private TransferStub transfer;
        private OptionHandlerStub optionHandler;

        [SetUp]
        public void Setup()
        {
            transfer = new TransferStub(new MemoryStream(new byte[5000]));
            transfer.SetState(new SendWriteRequest(transfer));

            optionHandler = new OptionHandlerStub("blub");
            TransferOptionHandlers.Add(optionHandler);
        }

        [TearDown]
        public void TearDown()
        {
            TransferOptionHandlers.Remove(optionHandler);
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
            transfer.SetState(new SendWriteRequest(transfer));
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
            //Option handler for "blub" is registered in the setup-method.
            transfer.Options.Request("blub", "bla");
            Assert.IsFalse(transfer.Options.First().IsAcknowledged);
            Assert.IsFalse(optionHandler.AcknowledgeWasCalled);
            transfer.OnCommand(new OptionAcknowledgement(transfer.Options));
            Assert.IsTrue(transfer.Options.First().IsAcknowledged);
            Assert.IsTrue(optionHandler.AcknowledgeWasCalled);
            Assert.IsInstanceOf<Sending>(transfer.State);
        }

        [Test]
        public void HandlesMissingOptionAcknowledgement()
        {
            transfer.Options.Request("blub", "bla");
            Assert.IsFalse(transfer.Options.First().IsAcknowledged);
            transfer.OnCommand(new Acknowledgement(0));
            Assert.IsFalse(transfer.Options.First().IsAcknowledged);
            Assert.IsInstanceOf<Sending>(transfer.State);
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
            transferWithLowTimeout.SetState(new SendWriteRequest(transferWithLowTimeout));

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
            transferWithLowTimeout.SetState(new SendWriteRequest(transferWithLowTimeout));

            transferWithLowTimeout.OnTimer();
            Assert.IsFalse(transferWithLowTimeout.HadNetworkTimeout);
            transferWithLowTimeout.OnTimer();
            Assert.IsTrue(transferWithLowTimeout.HadNetworkTimeout);
        }
    }
}
