using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Tftp.Net.Transfer.States;

namespace Tftp.Net.UnitTests
{
    [TestFixture]
    class ReceivedErrorState_Test
    {
        private TransferStub transfer;

        [SetUp]
        public void Setup()
        {
            transfer = new TransferStub();
            transfer.SetState(new ReceivedError(transfer, 123, "Error"));
        }

        [Test]
        public void CallsOnError()
        {
            bool OnErrorWasCalled = false;
            TransferStub transfer = new TransferStub();
            transfer.OnError += delegate(ITftpTransfer t, ushort code, string error)
            { 
                OnErrorWasCalled = true;
                Assert.AreEqual(transfer, t);
                Assert.AreEqual(123, code);
                Assert.AreEqual("My Error", error);
            };

            Assert.IsFalse(OnErrorWasCalled);
            transfer.SetState(new ReceivedError(transfer, 123, "My Error"));
            Assert.IsTrue(OnErrorWasCalled);
        }

        [Test]
        public void TransitionsToClosed()
        {
            Assert.IsInstanceOf<Closed>(transfer.State);
        }
    }
}
