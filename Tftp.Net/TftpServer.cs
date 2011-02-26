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

    /// <summary>
    /// A simple TFTP server class
    /// </summary>
    public class TftpServer : IDisposable
    {
        /// <summary>
        /// Fired when the server receives a new read request.
        /// </summary>
        public event TftpServerEventHandler OnReadRequest;

        /// <summary>
        /// Fired when the server receives a new write request.
        /// </summary>
        public event TftpServerEventHandler OnWriteRequest;

        /// <summary>
        /// Server port that we're listening on.
        /// </summary>
        private readonly ITftpChannel serverSocket;

        public TftpServer(IPEndPoint localAddress)
        {
            serverSocket = TftpChannelFactory.CreateServer(localAddress);
            serverSocket.OnCommandReceived += new TftpCommandHandler(serverSocket_OnCommandReceived);
        }

        /// <summary>
        /// Start accepting incoming connections.
        /// </summary>
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
            ITftpTransfer transfer = request is ReadRequest ? (ITftpTransfer)new LocalReadTransfer(channel, request.Filename, request.Options) : new LocalWriteTransfer(channel, request.Filename, request.Options);

            if (command is ReadRequest)
                RaiseOnReadRequest(transfer, endpoint);
            else if (command is WriteRequest)
                RaiseOnWriteRequest(transfer, endpoint);
            else
                throw new Exception("Unexpected tftp transfer request: " + command);
        }

        #region IDisposable
        public void Dispose()
        {
            serverSocket.Dispose();
        }
        #endregion

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

