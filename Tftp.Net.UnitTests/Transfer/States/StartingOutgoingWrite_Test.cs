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
    class StartingOutgoingWrite_Test
    {
        private TransferStub transfer;

        [SetUp]
        public void Setup()
        {
            transfer = new TransferStub(new MemoryStream(new byte[5000]));
            transfer.SetState(new StartingOutgoingWrite(transfer));
        }

        [Test]
        public void CanCancel()
        {
            transfer.Cancel();
            Assert.IsInstanceOf<Closed>(transfer.State);
            Assert.IsTrue(transfer.CommandWasSent(typeof(Error)));
        }

        [Test]
        public void SendsWriteRequest()
        {
            TransferStub transfer = new TransferStub(new MemoryStream(new byte[5000]));
            transfer.SetState(new StartingOutgoingWrite(transfer));
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
            Assert.IsInstanceOf<StartingOutgoingWrite>(transfer.State);
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
    }
}
