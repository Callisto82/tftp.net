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
    class UdpChannel_Test
    {
        private UdpChannel tested;

        [SetUp]
        public void Setup()
        {
            tested = new UdpChannel(new UdpClient(0));
        }

        [TearDown]
        public void Teardown()
        {
            tested.Dispose();
        }

        [Test]
        public void SendsRealUdpPackets()
        {
            var remote = OpenRemoteUdpClient();

            tested.Open();
            tested.RemoteEndpoint = remote.Client.LocalEndPoint;
            tested.Send(new Acknowledgement(1));

            AssertBytesReceived(remote, TimeSpan.FromMilliseconds(500));
        }

        [Test]
        public void DeniesSendingOnClosedConnections()
        {
            Assert.Throws<InvalidOperationException>(
                () => tested.Send(new Acknowledgement(1)));
        }

        [Test]
        public void DeniesSendingWhenNoRemoteAddressIsSet()
        {
            tested.Open();

            Assert.Throws<InvalidOperationException>(
                () => tested.Send(new Acknowledgement(1)));
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
