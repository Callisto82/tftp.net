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
            IPEndPoint receiveFrom = new IPEndPoint(IPAddress.Loopback, 69);
            TftpClient client = new TftpClient(receiveFrom);
            ITftpTransfer transfer = client.Receive("test.dat");
            transfer.OnProgress += new TftpProgressHandler(transfer_OnProgress);
            transfer.OnFinished += new TftpEventHandler(transfer_OnFinshed);
            transfer.OnError += new TftpErrorHandler(transfer_OnError);

            MemoryStream stream = new MemoryStream();
            transfer.UserContext = stream;
            transfer.Start(stream);

            TransferFinishedEvent.WaitOne();
            Console.ReadKey();
        }

        static void transfer_OnProgress(ITftpTransfer transfer, int bytesTransferred)
        {
            Console.WriteLine("Transfer running. Got " + bytesTransferred + " bytes.");
        }

        static void transfer_OnError(ITftpTransfer transfer, ushort code, string error)
        {
            Console.WriteLine("Transfer failed. Code: " + code + ", reason: " + error);
            TransferFinishedEvent.Set();
        }

        static void transfer_OnFinshed(ITftpTransfer transfer)
        {
            Console.WriteLine("Transfer succeeded. Loaded " + ((Stream)transfer.UserContext).Length + " bytes from server.");
            TransferFinishedEvent.Set();
        }
    }
}
