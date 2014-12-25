using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tftp.Net.Channel;
using System.Net.Sockets;
using NUnit.Framework;
using System.Net;
using System.Threading;

namespace Tftp.Net.UnitTests
{
    [TestFixture]
    class UdpChannel_Test : ITransferChannel_Test
    {
        protected override Channel.ITransferChannel CreateConnection()
        {
            return new UdpChannel(new UdpClient(0));
        }

        [Test]
        public void SendsRealUdpPackets()
        {
            var remote = OpenRemoteUdpClient();

            using (ITransferChannel conn = new UdpChannel(new UdpClient(0)))
            {
                conn.Open();
                conn.RemoteEndpoint = remote.Client.LocalEndPoint;
                conn.Send(new Acknowledgement(1));
            }

            AssertBytesReceived(remote, TimeSpan.FromMilliseconds(500));
        }

        private void AssertBytesReceived(UdpClient remote, TimeSpan timeout)
        {
            double msecs = timeout.TotalMilliseconds;
            while (msecs > 0)
            {
                if (remote.Available > 0)
                    return;

                Thread.Sleep(50);
                msecs -= 50;
            }

            Assert.Fail("Remote client did not receive anything within " + timeout.TotalMilliseconds + "ms");
        }

        private UdpClient OpenRemoteUdpClient()
        {
            return new UdpClient(new IPEndPoint(IPAddress.Loopback, 0));
        }

    }
}
