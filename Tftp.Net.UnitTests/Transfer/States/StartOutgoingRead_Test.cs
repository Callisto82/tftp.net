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
        private TransferStub transfer;

        [SetUp]
        public void Setup()
        {
            transfer = new TransferStub();
            transfer.SetState(new StartOutgoingRead());
        }

        [Test]
        public void CanCancel()
        {
            transfer.Cancel(TftpErrorPacket.IllegalOperation);
            Assert.IsInstanceOf<Closed>(transfer.State);
        }

        [Test]
        public void IgnoresCommands()
        {
            transfer.OnCommand(new Error(5, "Hallo Welt"));
            Assert.IsInstanceOf<StartOutgoingRead>(transfer.State);
        }

        [Test]
        public void CanStart()
        {
            transfer.Start(new MemoryStream());
            Assert.IsTrue(transfer.CommandWasSent(typeof(ReadRequest)));
            Assert.IsInstanceOf<SendReadRequest>(transfer.State);
        }
    }
}
