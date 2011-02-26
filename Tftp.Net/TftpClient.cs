using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Tftp.Net.Channel;
using Tftp.Net.Transfer.States;
using Tftp.Net.Transfer;

namespace Tftp.Net
{
    /// <summary>
    /// A TFTP client that can connect to a TFTP server.
    /// </summary>
    public class TftpClient
    {
        private readonly IPEndPoint remoteAddress;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="remoteAddress">Address of the server that you would like to connect to.</param>
        public TftpClient(IPEndPoint remoteAddress)
        {
            this.remoteAddress = remoteAddress;
        }

        /// <summary>
        /// GET (receive) a file from the server.
        /// You have to call start on the returned ITftpTransfer to start the transfer.
        /// </summary>
        public ITftpTransfer Receive(String filename)
        {
            ITftpChannel channel = TftpChannelFactory.CreateConnection(remoteAddress);
            return new RemoteReadTransfer(channel, filename);
        }

        /// <summary>
        /// PUT (write) a file from the server.
        /// You have to call start on the returned ITftpTransfer to start the transfer.
        /// </summary>
        public ITftpTransfer Send(String filename)
        {
            ITftpChannel channel = TftpChannelFactory.CreateConnection(remoteAddress);
            return new RemoteWriteTransfer(channel, filename);
        }
    }
}
