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
    class StartIncomingReadState_Test
    {
        private TransferStub transfer;

        [SetUp]
        public void Setup()
        {
            transfer = new TransferStub();
            transfer.SetState(new StartIncomingRead(transfer));
        }

        [Test]
        public void CanCancel()
        {
            transfer.Cancel();
            Assert.IsTrue(transfer.CommandWasSent(typeof(Error)));
            Assert.IsInstanceOf<Closed>(transfer.State);
        }

        [Test]
        public void IgnoresCommands()
        {
            transfer.OnCommand(new Error(5, "Hallo Welt"));
            Assert.IsInstanceOf<StartIncomingRead>(transfer.State);
        }

        [Test]
        public void CanStart()
        {
            transfer.Start(new MemoryStream(new byte[50000]));
            Assert.IsInstanceOf<Sending>(transfer.State);
        }

        [Test]
        public void CanStartWithOptions()
        {
            //Simulate that we're acknowledging an option
            transfer.Options.Add("blksize", "123");
            transfer.Options.First().IsAcknowledged = true;

            transfer.Start(new MemoryStream(new byte[50000]));
            Assert.IsInstanceOf<SendOptionAcknowledgementForReadRequest>(transfer.State);
        }
    }
}
