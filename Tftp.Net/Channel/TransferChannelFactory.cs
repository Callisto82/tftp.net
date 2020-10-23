using System;
using System.Net;
using System.Net.Sockets;

namespace Tftp.Net.Channel
{
    class TransferChannelFactory : ITransferChannelFactory
    {
        public static ITransferChannelFactory GetUdpFactory()
        {
            return new TransferChannelFactory();
        }

        public ITransferChannel CreateServer(EndPoint localAddress)
        {
            if (localAddress is IPEndPoint)
                return CreateServerUdp((IPEndPoint)localAddress);

            throw new NotSupportedException("Unsupported endpoint type.");
        }

        public ITransferChannel CreateConnection(EndPoint remoteAddress)
        {
            if (remoteAddress is IPEndPoint)
                return CreateConnectionUdp((IPEndPoint)remoteAddress);

            throw new NotSupportedException("Unsupported endpoint type.");
        }

        #region UDP connections

        private ITransferChannel CreateServerUdp(IPEndPoint localAddress)
        {
            UdpClient socket = new UdpClient(localAddress);
            return new UdpChannel(socket);
        }

        private ITransferChannel CreateConnectionUdp(IPEndPoint remoteAddress)
        {
            IPEndPoint localAddress = new IPEndPoint(IPAddress.Any, 0);
            UdpChannel channel = new UdpChannel(new UdpClient(localAddress));
            channel.RemoteEndpoint = remoteAddress;
            return channel;
        }
        #endregion
    }
}
