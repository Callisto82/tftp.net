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
    public class TftpClient
    {
        private readonly IPEndPoint serverAddress;

        public TftpClient(IPEndPoint serverAddress)
        {
            this.serverAddress = serverAddress;
        }

        public ITftpTransfer Receive(String filename)
        {
            ITftpChannel channel = TftpChannelFactory.CreateConnection(serverAddress);
            return new RemoteReadTransfer(channel, filename);
        }

        public ITftpTransfer Send(String filename)
        {
            throw new NotImplementedException("Remote write requests are not yet supported.");
            //ITftpChannel channel = TftpChannelFactory.CreateConnection(serverAddress);
            //return new RemoteReadTransfer(channel, filename);
        }
    }
}
