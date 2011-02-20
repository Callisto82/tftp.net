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
    class StartOutgoingWrite_Test 
    {
        private TransferStub transfer;

        [SetUp]
        public void Setup()
        {
            transfer = new TransferStub();
            transfer.SetState(new StartOutgoingWrite(transfer));
        }

        [Test]
        public void CanCancel()
        {
            transfer.Cancel();
            Assert.IsInstanceOf<Closed>(transfer.State);
        }

        [Test]
        public void IgnoresCommands()
        {
            transfer.OnCommand(new Error(5, "Hallo Welt"));
            Assert.IsInstanceOf<StartOutgoingWrite>(transfer.State);
        }

        [Test]
        public void CanStart()
        {
            transfer.Start(new MemoryStream());
            Assert.IsInstanceOf<StartingOutgoingWrite>(transfer.State);
        }
    }
}
