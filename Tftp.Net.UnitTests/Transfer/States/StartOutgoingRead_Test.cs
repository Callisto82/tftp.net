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
    class StartOutgoingRead_Test
    {
        [Test]
        public void CanEnter()
        {
            TransferStub transfer = new TransferStub();
            transfer.SetState(new StartOutgoingRead(transfer));
        }

        [Test]
        public void CanCancel()
        {
            TransferStub transfer = new TransferStub();
            transfer.SetState(new StartOutgoingRead(transfer));
            transfer.Cancel();
            Assert.IsInstanceOf<Closed>(transfer.State);
        }

        [Test]
        public void IgnoresCommands()
        {
            TransferStub transfer = new TransferStub();
            transfer.SetState(new StartOutgoingRead(transfer));
            transfer.OnCommand(new Error(5, "Hallo Welt"));
            Assert.IsInstanceOf<StartOutgoingRead>(transfer.State);
        }

        [Test]
        public void CanStart()
        {
            TransferStub transfer = new TransferStub();
            transfer.SetState(new StartOutgoingRead(transfer));
            transfer.Start(new MemoryStream());

            Assert.IsTrue(transfer.CommandWasSent(typeof(ReadRequest)));
            Assert.IsInstanceOf<SendReadRequest>(transfer.State);
        }
    }
}
