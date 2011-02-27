using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Tftp.Net.TransferOptions;

namespace Tftp.Net.UnitTests.Transfer
{
    [TestFixture]
    class TransferOptionHandlers_Test
    {
        private TransferStub transfer;
        private List<ITftpTransferOptionHandler> defaultHandlers;

        [SetUp]
        public void Setup()
        {
            transfer = new TransferStub();
            defaultHandlers = TransferOptionHandlers.All.ToList(); //Backup default handlers
            RemoveAllHandlers();
        }

        [TearDown]
        public void TearDown()
        {
            RemoveAllHandlers();

            //Restore default handlers
            foreach (ITftpTransferOptionHandler handler in defaultHandlers)
                TransferOptionHandlers.Add(handler);
        }

        private void RemoveAllHandlers()
        {
            foreach (ITftpTransferOptionHandler handler in TransferOptionHandlers.All.ToList())
                TransferOptionHandlers.Remove(handler);
        }

        [Test]
        public void EmptyOptionList()
        {
            List<ITftpTransferOption> options = new List<ITftpTransferOption>();

            //must not throw an exception
            TransferOptionHandlers.HandleAcceptedOptions(transfer, options);
        }

        [Test]
        public void UnrecognizedOptions()
        {
            List<ITftpTransferOption> options = new List<ITftpTransferOption>();
            options.Add(new TransferOption("test", "micha"));

            //must not throw an exception
            TransferOptionHandlers.HandleAcceptedOptions(transfer, options);
            Assert.IsFalse(options.First().IsAcknowledged);
        }

        [Test]
        public void RecognizedOption()
        {
            List<ITftpTransferOption> options = new List<ITftpTransferOption>();
            options.Add(new TransferOption("test", "micha"));

            TransferOptionHandlers.Add(new OptionHandlerStub("test"));
            TransferOptionHandlers.HandleAcceptedOptions(transfer, options);
            Assert.IsTrue(options.First().IsAcknowledged);
        }

        [Test]
        public void AddHandler()
        {
            ITftpTransferOptionHandler handler = new OptionHandlerStub("test");

            Assert.IsFalse(TransferOptionHandlers.All.Contains(handler));
            TransferOptionHandlers.Add(handler);
            Assert.IsTrue(TransferOptionHandlers.All.Contains(handler));
        }

        [Test]
        public void RemoveHandler()
        {
            ITftpTransferOptionHandler handler = new OptionHandlerStub("test");
            TransferOptionHandlers.Add(handler);
            Assert.IsTrue(TransferOptionHandlers.All.Contains(handler));

            TransferOptionHandlers.Remove(handler);
            Assert.IsFalse(TransferOptionHandlers.All.Contains(handler));
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddNullHandler()
        {
            TransferOptionHandlers.Add(null);
        }
    }
}
