using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Tftp.Net.Transfer.States;

namespace Tftp.Net.UnitTests
{
    [TestFixture]
    class CancelledByUserState_Test
    {
        [Test]
        public void CanEnter()
        {
            TransferStub transfer = new TransferStub();
            transfer.SetState(new CancelledByUser(transfer));
        }

        [Test]
        public void SendsErrorToClient()
        {
            TransferStub transfer = new TransferStub();
            transfer.SetState(new CancelledByUser(transfer));
            Assert.IsTrue(transfer.CommandWasSent(typeof(Error)));
        }

        [Test]
        public void TransitionsToClosedState()
        {
            TransferStub transfer = new TransferStub();
            transfer.SetState(new CancelledByUser(transfer));
            Assert.IsInstanceOf<Closed>(transfer.State);
        }
    }
}
