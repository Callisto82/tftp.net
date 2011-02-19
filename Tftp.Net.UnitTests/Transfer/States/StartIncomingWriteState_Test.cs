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
    class StartIncomingWriteState_Test
    {
        [Test]
        public void CanEnter()
        {
            TransferStub transfer = new TransferStub();
            transfer.SetState(new StartIncomingWrite(transfer));
        }

        [Test]
        public void CanCancel()
        {
            TransferStub transfer = new TransferStub();
            transfer.SetState(new StartIncomingWrite(transfer));
            transfer.Cancel();
            Assert.IsTrue(transfer.CommandWasSent(typeof(Error)));
            Assert.IsInstanceOf<Closed>(transfer.State);
        }

        [Test]
        public void IgnoresCommands()
        {
            TransferStub transfer = new TransferStub();
            transfer.SetState(new StartIncomingWrite(transfer));
            transfer.OnCommand(new Error(5, "Hallo Welt"));
            Assert.IsInstanceOf<StartIncomingWrite>(transfer.State);
        }

        [Test]
        public void CanStart()
        {
            TransferStub transfer = new TransferStub();
            transfer.SetState(new StartIncomingWrite(transfer));
            transfer.Start(new MemoryStream(new byte[50000]));

            Assert.IsTrue(transfer.CommandWasSent(typeof(Acknowledgement)));
            Assert.IsInstanceOf<Receiving>(transfer.State);
        }
    }
}
