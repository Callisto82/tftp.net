using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Net;

namespace Tftp.Net.SampleClient
{
    class Program
    {
        private static AutoResetEvent TransferFinishedEvent = new AutoResetEvent(false);

        static void Main(string[] args)
        {
            //Setup a TftpClient instance
            IPEndPoint serverAddress = new IPEndPoint(IPAddress.Loopback, 69);
            TftpClient client = new TftpClient(serverAddress);

            //Prepare a simple transfer (GET test.dat)
            ITftpTransfer transfer = client.Receive("EUPL-EN.pdf");

            //Capture the events that may happen during the transfer
            transfer.OnProgress += new TftpProgressHandler(transfer_OnProgress);
            transfer.OnFinished += new TftpEventHandler(transfer_OnFinshed);
            transfer.OnError += new TftpErrorHandler(transfer_OnError);

            //Start the transfer and write the data that we're downloading into a memory stream
            Stream stream = new MemoryStream();
            transfer.Start(stream);

            //Wait for the transfer to finish
            TransferFinishedEvent.WaitOne();
            Console.ReadKey();
        }

        static void transfer_OnProgress(ITftpTransfer transfer, TftpTransferProgress progress)
        {
            Console.WriteLine("Transfer running. Progress: " + progress);
        }

        static void transfer_OnError(ITftpTransfer transfer, TftpTransferError error)
        {
            Console.WriteLine("Transfer failed: " + error);
            TransferFinishedEvent.Set();
        }

        static void transfer_OnFinshed(ITftpTransfer transfer)
        {
            Console.WriteLine("Transfer succeeded.");
            TransferFinishedEvent.Set();
        }
    }
}
