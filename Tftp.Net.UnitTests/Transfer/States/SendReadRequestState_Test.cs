using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Tftp.Net.Transfer.States;
using System.IO;

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
            transfer.SetState(new SendReadRequest(transfer));
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
            transfer.OnError += delegate(ITftpTransfer t, ushort code, string error) { onErrorWasCalled = true; };

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
            transfer.Options.Add("blub", "bla");
            Assert.IsFalse(transfer.Options.First().IsAcknowledged);
            transfer.OnCommand(new OptionAcknowledgement(transfer.Options));
            Assert.IsTrue(transfer.Options.First().IsAcknowledged);
            Assert.IsTrue(transfer.CommandWasSent(typeof(Acknowledgement)));
            Assert.IsInstanceOf<SendReadRequest>(transfer.State);
        }

        [Test]
        public void HandlesMissingOptionAcknowledgement()
        {
            transfer.Options.Add("blub", "bla");
            Assert.IsFalse(transfer.Options.First().IsAcknowledged);
            transfer.OnCommand(new Data(1, new byte[10]));
            Assert.AreEqual(0, transfer.Options.Count());
            Assert.IsInstanceOf<Closed>(transfer.State);
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
            transferWithLowTimeout.Timeout = new TimeSpan(0);
            transferWithLowTimeout.SetState(new SendReadRequest(transferWithLowTimeout));

            Assert.IsTrue(transferWithLowTimeout.CommandWasSent(typeof(ReadRequest)));
            transferWithLowTimeout.SentCommands.Clear();

            transferWithLowTimeout.OnTimer();
            Assert.IsTrue(transferWithLowTimeout.CommandWasSent(typeof(ReadRequest)));
        }
    }
}
