using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace Tftp.Net.SampleServer
{
    class Program
    {
        private static String ServerDirectory;

        static void Main(string[] args)
        {
            ServerDirectory = Environment.CurrentDirectory;

            Console.WriteLine("Running TFTP server for directory: " + ServerDirectory);
            Console.WriteLine();
            Console.WriteLine("Press any key to close the server.");

            using (TftpServer server = new TftpServer(new IPEndPoint(IPAddress.Any, 69)))
            {
                server.OnReadRequest += new TftpServerEventHandler(server_OnReadRequest);
                server.OnWriteRequest += new TftpServerEventHandler(server_OnWriteRequest);
                server.Start();
                Console.Read();
            }
        }

        static void server_OnWriteRequest(ITftpTransfer transfer, EndPoint client)
        {
            Console.WriteLine("[" + transfer.Filename + "] Write request from "+ client + " for " + transfer.Filename);
            DumpOptions(transfer);
            Console.WriteLine("[" + transfer.Filename + "] Denying request.");
            transfer.Cancel(TftpErrorPacket.IllegalOperation);
        }

        static void DumpOptions(ITftpTransfer transfer)
        {
            foreach(ITftpTransferOption option in transfer.Options)
                Console.WriteLine("[" + transfer.Filename + "] Option request: " + option.Name + "=" + option.Value);
        }

        static void server_OnReadRequest(ITftpTransfer transfer, EndPoint client)
        {
            Console.WriteLine("[" + transfer.Filename + "] Read request from " + client + " for " + transfer.Filename);
            DumpOptions(transfer);

            String path = Path.Combine(ServerDirectory, transfer.Filename);
            FileInfo file = new FileInfo(path);

            //Is the file within the server directory?
            if (!file.FullName.ToLower().StartsWith(ServerDirectory.ToLower()))
            {
                Console.WriteLine("[" + transfer.Filename + "] Denying request because the file is outside the server directory.");
                transfer.Cancel(TftpErrorPacket.AccessViolation);
                return;
            }

            //Does the file exist at all?
            if (!file.Exists)
            {
                Console.WriteLine("[" + transfer.Filename + "] Denying request because the file does not exist.");
                transfer.Cancel(TftpErrorPacket.FileNotFound);
                return;
            }

            //Ok, start the transfer
            Stream str = new FileStream(file.FullName, FileMode.Open);
            Console.WriteLine("[" + transfer.Filename + "] Accepting request");
            transfer.OnProgress += new TftpProgressHandler(transfer_OnProgress);
            transfer.OnError += new TftpErrorHandler(transfer_OnError);
            transfer.OnFinished += new TftpEventHandler(transfer_OnFinished);
            transfer.Start(str);
        }

        static void transfer_OnError(ITftpTransfer transfer, TftpTransferError error)
        {
            Console.WriteLine("[" + transfer.Filename + "] Error: " + error);
        }

        static void transfer_OnFinished(ITftpTransfer transfer)
        {
            Console.WriteLine("[" + transfer.Filename + "] Finished.");
        }

        static void transfer_OnProgress(ITftpTransfer transfer, int bytesTransferred)
        {
            Console.WriteLine("[" + transfer.Filename + "] Progress: " + bytesTransferred + " bytes");
        }
    }
}
