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
            transfer.SetState(new ReceivedError(new TftpErrorPacket(123, "Error")));
        }

        [Test]
        public void CallsOnError()
        {
            bool OnErrorWasCalled = false;
            TransferStub transfer = new TransferStub();
            transfer.OnError += delegate(ITftpTransfer t, TftpTransferError error)
            { 
                OnErrorWasCalled = true;
                Assert.AreEqual(transfer, t);

                Assert.IsInstanceOf<TftpErrorPacket>(error);

                Assert.AreEqual(123, ((TftpErrorPacket)error).ErrorCode);
                Assert.AreEqual("My Error", ((TftpErrorPacket)error).ErrorMessage);
            };

            Assert.IsFalse(OnErrorWasCalled);
            transfer.SetState(new ReceivedError(new TftpErrorPacket(123, "My Error")));
            Assert.IsTrue(OnErrorWasCalled);
        }

        [Test]
        public void TransitionsToClosed()
        {
            Assert.IsInstanceOf<Closed>(transfer.State);
        }
    }
}
