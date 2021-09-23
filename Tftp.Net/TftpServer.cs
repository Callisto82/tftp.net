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
    public delegate void TftpServerErrorHandler(TftpTransferError error);

    /// <summary>
    /// A simple TFTP server class. <code>Dispose()</code> the server to close the socket that it listens on.
    /// </summary>
    public class TftpServer : IDisposable
    {
        public const int DEFAULT_SERVER_PORT = 69;

        /// <summary>
        /// Fired when the server receives a new read request.
        /// </summary>
        public event TftpServerEventHandler OnReadRequest;

        /// <summary>
        /// Fired when the server receives a new write request.
        /// </summary>
        public event TftpServerEventHandler OnWriteRequest;

        /// <summary>
        /// Fired when the server encounters an error (for example, a non-parseable request)
        /// </summary>
        public event TftpServerErrorHandler OnError;

        /// <summary>
        /// Server port that we're listening on.
        /// </summary>
        private readonly ITransferChannel serverSocket;

        /// <summary>
        /// Keep the address of the local interface so that UDP packets do not get lost into wrong one
        /// </summary>
        private readonly IPAddress localInterface;

        public TftpServer(IPEndPoint localAddress)
        {
            if (localAddress == null)
                throw new ArgumentNullException("localAddress");

            serverSocket = TransferChannelFactory.CreateServer(localAddress);
            serverSocket.OnCommandReceived += new TftpCommandHandler(serverSocket_OnCommandReceived);
            serverSocket.OnError += new TftpChannelErrorHandler(serverSocket_OnError);
            localInterface = localAddress.Address;
        }

        public TftpServer(IPAddress localAddress)
            : this(localAddress, DEFAULT_SERVER_PORT)
        {
        }

        public TftpServer(IPAddress localAddress, int port)
            : this(new IPEndPoint(localAddress, port))
        {
        }

        public TftpServer(int port)
            : this(new IPEndPoint(IPAddress.Any, port))
        {
        }

        public TftpServer()
            : this(DEFAULT_SERVER_PORT)
        {
        }


        /// <summary>
        /// Start accepting incoming connections.
        /// </summary>
        public void Start()
        {
            serverSocket.Open();
        }

        void serverSocket_OnError(TftpTransferError error)
        {
            RaiseOnError(error);
        }

        private void serverSocket_OnCommandReceived(ITftpCommand command, EndPoint endpoint)
        {
            //Ignore all other commands
            if (!(command is ReadOrWriteRequest))
                return;

            //Open a connection to the client
            ITransferChannel channel = TransferChannelFactory.CreateConnection(endpoint, new IPEndPoint(localInterface, 0));

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

        private void RaiseOnError(TftpTransferError error)
        {
            if (OnError != null)
                OnError(error);
        }

        private void RaiseOnWriteRequest(ITftpTransfer transfer, EndPoint client)
        {
            if (OnWriteRequest != null)
            {
                OnWriteRequest(transfer, client);
            }
            else
            {
                transfer.Cancel(new TftpErrorPacket(0, "Server did not provide a OnWriteRequest handler."));
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
                transfer.Cancel(new TftpErrorPacket(0, "Server did not provide a OnReadRequest handler."));
            }
        }
    }
}

