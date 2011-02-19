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
        [Test]
        public void CanEnter()
        {
            TransferStub transfer = new TransferStub();
            transfer.SetState(new Sending(transfer, new MemoryStream(new byte[5000])));
        }

        [Test]
        public void SendsPacket()
        {
            TransferStub transfer = new TransferStub();
            transfer.SetState(new Sending(transfer, new MemoryStream(new byte[5000])));
            Assert.IsTrue(transfer.CommandWasSent(typeof(Data)));
        }

        [Test]
        public void HandlesAcknowledgment()
        {
            TransferStub transfer = new TransferStub();
            transfer.SetState(new Sending(transfer, new MemoryStream(new byte[5000])));
            
            transfer.SentCommands.Clear();
            Assert.IsFalse(transfer.CommandWasSent(typeof(Data)));

            transfer.OnCommand(new Acknowledgement(1));
            Assert.IsTrue(transfer.CommandWasSent(typeof(Data)));
        }

        [Test]
        public void IgnoresWrongAcknowledgment()
        {
            TransferStub transfer = new TransferStub();
            transfer.SetState(new Sending(transfer, new MemoryStream(new byte[5000])));

            transfer.SentCommands.Clear();
            Assert.IsFalse(transfer.CommandWasSent(typeof(Data)));

            transfer.OnCommand(new Acknowledgement(0));
            Assert.IsFalse(transfer.CommandWasSent(typeof(Data)));
        }

        [Test]
        public void RaisesProgress()
        {
            bool onProgressWasCalled = false;
            TransferStub transfer = new TransferStub();
            transfer.SetState(new Sending(transfer, new MemoryStream(new byte[5000])));
            transfer.OnProgress += delegate(ITftpTransfer t, int bytesTransferred) { Assert.AreEqual(transfer, t); Assert.IsTrue(bytesTransferred > 0); onProgressWasCalled = true; };

            Assert.IsFalse(onProgressWasCalled);
            transfer.OnCommand(new Acknowledgement(1));
            Assert.IsTrue(onProgressWasCalled);
        }

        [Test]
        public void CanCancel()
        {
            TransferStub transfer = new TransferStub();
            transfer.SetState(new Sending(transfer, new MemoryStream(new byte[5000])));
            transfer.Cancel();
            Assert.IsTrue(transfer.CommandWasSent(typeof(Error)));
            Assert.IsInstanceOf<Closed>(transfer.State);
        }

        [Test]
        public void HandlesError()
        {
            bool onErrorWasCalled = false;
            TransferStub transfer = new TransferStub();
            transfer.SetState(new Sending(transfer, new MemoryStream(new byte[5000])));
            transfer.OnError += delegate(ITftpTransfer t, ushort code, string error) { onErrorWasCalled = true; };

            Assert.IsFalse(onErrorWasCalled);
            transfer.OnCommand(new Error(123, "Test Error"));
            Assert.IsTrue(onErrorWasCalled);

            Assert.IsInstanceOf<Closed>(transfer.State);
        }
    }
}
