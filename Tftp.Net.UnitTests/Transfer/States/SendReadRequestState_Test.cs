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
        [Test]
        public void CanEnter()
        {
            TransferStub transfer = new TransferStub();
            transfer.SetState(new SendReadRequest(transfer, new MemoryStream()));
        }

        [Test]
        public void CanCancel()
        {
            TransferStub transfer = new TransferStub();
            transfer.SetState(new SendReadRequest(transfer, new MemoryStream()));
            transfer.Cancel();
            Assert.IsTrue(transfer.CommandWasSent(typeof(Error)));
            Assert.IsInstanceOf<Closed>(transfer.State);
        }

        [Test]
        public void HandlesError()
        {
            bool onErrorWasCalled = false;
            TransferStub transfer = new TransferStub();
            transfer.OnError += delegate(ITftpTransfer t, ushort code, string error) { onErrorWasCalled = true; };
            transfer.SetState(new SendReadRequest(transfer, new MemoryStream()));

            Assert.IsFalse(onErrorWasCalled);
            transfer.OnCommand(new Error(123, "Test Error"));
            Assert.IsTrue(onErrorWasCalled);

            Assert.IsInstanceOf<Closed>(transfer.State);
        }

        [Test]
        public void HandlesData()
        {
            TransferStub transfer = new TransferStub();
            MemoryStream ms = new MemoryStream();
            transfer.SetState(new SendReadRequest(transfer, ms));

            transfer.OnCommand(new Data(1, new byte[10]));
            Assert.IsTrue(transfer.CommandWasSent(typeof(Acknowledgement)));
            Assert.IsInstanceOf<Closed>(transfer.State);
            Assert.AreEqual(10, ms.Length);
        }
    }
}
