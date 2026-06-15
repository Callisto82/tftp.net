using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Tftp.Net.Channel
{
    static class TransferChannelFactory
    {
        public static ITransferChannel CreateServer(EndPoint localAddress)
        {
            if (localAddress is IPEndPoint)
                return CreateServerUdp((IPEndPoint)localAddress);

            throw new NotSupportedException("Unsupported endpoint type.");
        }

        public static ITransferChannel CreateConnection(EndPoint remoteAddress)
        {
            if (remoteAddress is IPEndPoint)
                return CreateConnectionUdp((IPEndPoint)remoteAddress);

            throw new NotSupportedException("Unsupported endpoint type.");
        }

        public static ITransferChannel CreateConnection(IPEndPoint remoteAddress, IPAddress localAddress)
        {
            IPEndPoint localEndpoint = new IPEndPoint(localAddress, 0);
            UdpChannel channel = new UdpChannel(new UdpClient(localEndpoint));
            channel.RemoteEndpoint = remoteAddress;
            return channel;
        }

        #region UDP connections

        private static ITransferChannel CreateServerUdp(IPEndPoint localAddress)
        {
            UdpClient socket = new UdpClient(localAddress);
            return new UdpChannel(socket);
        }

        private static ITransferChannel CreateConnectionUdp(IPEndPoint remoteAddress)
        {
            return CreateConnection(remoteAddress, localAddress: IPAddress.Any);
        }

        #endregion
    }
}
