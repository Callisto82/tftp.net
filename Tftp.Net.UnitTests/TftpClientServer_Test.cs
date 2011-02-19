using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using System.Threading;
using System.Net;

namespace Tftp.Net.UnitTests
{
    [TestFixture]
    class TftpClientServer_Test
    {
        public byte[] DemoData = { 1, 2, 3 };
        private bool TransferHasFinished = false;

        [Test]
        public void ClientsReadsFromServer()
        {
            using (TftpServer server = new TftpServer(new IPEndPoint(IPAddress.Loopback, 69)))
            {
                server.OnReadRequest += new TftpServerEventHandler(server_OnReadRequest);
                server.Start();

                TftpClient client = new TftpClient(new IPEndPoint(IPAddress.Loopback, 69));
                using (ITftpTransfer transfer = client.Receive("Demo File"))
                {
                    MemoryStream ms = new MemoryStream();
                    transfer.OnFinished += new TftpEventHandler(transfer_OnFinished);
                    transfer.Start(ms);

                    Thread.Sleep(500);
                    Assert.IsTrue(TransferHasFinished);
                }
            }
        }

        void transfer_OnFinished(ITftpTransfer transfer)
        {
            TransferHasFinished = true;
        }

        void server_OnReadRequest(ITftpTransfer transfer, EndPoint client)
        {
            transfer.Start(new MemoryStream(DemoData));
        }
    }
}
