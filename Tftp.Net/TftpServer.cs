using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using Tftp.Net.Channel;
using Tftp.Net.Transfer;

namespace Tftp.Net
{
    public delegate void TftpServerEventHandler(ITftpTransfer transfer, EndPoint client);
    public class TftpServer : IDisposable
    {
        public event TftpServerEventHandler OnReadRequest;
        public event TftpServerEventHandler OnWriteRequest;

        private readonly ITftpChannel serverSocket;
        private readonly IPEndPoint localAddress;

        public TftpServer(IPEndPoint localAddress)
        {
            this.localAddress = localAddress;
            serverSocket = TftpChannelFactory.CreateServer(localAddress);
            serverSocket.OnCommandReceived += new TftpCommandHandler(serverSocket_OnCommandReceived);
        }

        public void Start()
        {
            serverSocket.Open();
        }

        void serverSocket_OnCommandReceived(ITftpCommand command, EndPoint endpoint)
        {
            //Ignore all other commands
            if (!(command is ReadOrWriteRequest))
                return;

            //Open a connection to the client
            ITftpChannel channel = TftpChannelFactory.CreateConnection(endpoint);

            //Create a wrapper for the transfer request
            ReadOrWriteRequest request = (ReadOrWriteRequest)command;
            ITftpTransfer transfer = request is ReadRequest ? (ITftpTransfer)new LocalReadTransfer(channel, request.Filename) : new LocalWriteTransfer(channel, request.Filename);

            if (command is ReadRequest)
                RaiseOnReadRequest(transfer, endpoint);
            else if (command is WriteRequest)
                RaiseOnWriteRequest(transfer, endpoint);
            else
                throw new Exception("Unexpected tftp transfer request: " + command);
        }

        public void Dispose()
        {
            serverSocket.Dispose();
        }

        private void RaiseOnWriteRequest(ITftpTransfer transfer, EndPoint client)
        {
            if (OnWriteRequest != null)
            {
                OnWriteRequest(transfer, client);
            }
            else
            {
                transfer.Cancel();
            }
        }

        private void RaiseOnReadRequest(ITftpTransfer transfer, EndPoint client)
        {
            if (OnReadRequest != null)
            {
                OnReadRequest(transfer, client);
            }
            else
            {
                transfer.Cancel();
            }
        }
    }
}

