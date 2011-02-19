using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Tftp.Net.Transfer;
using Tftp.Net.Transfer.States;

namespace Tftp.Net.UnitTests
{
    [TestFixture]
    class ClosedState_Test
    {
        [Test]
        public void CanEnter()
        {
            TransferStub transfer = new TransferStub();
            transfer.SetState(new Closed(transfer));
        }

        [Test]
        public void CanNotCancel()
        {
            TransferStub transfer = new TransferStub();
            transfer.SetState(new Closed(transfer));
            transfer.Cancel();
            Assert.IsInstanceOf<Closed>(transfer.State);
        }

        [Test]
        public void IgnoresCommands()
        {
            TransferStub transfer = new TransferStub();
            transfer.SetState(new Closed(transfer));
            transfer.OnCommand(new Error(10, "Test"));
            Assert.IsInstanceOf<Closed>(transfer.State);
        }
    }
}
