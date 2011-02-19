using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tftp.Net.Channel;
using System.Net.Sockets;
using NUnit.Framework;
using System.Net;

namespace Tftp.Net.UnitTests
{
    [TestFixture]
    class UdpChannel_Test : ITftpChannel_Test
    {
        private const int TEST_PORT = 69;

        protected override Channel.ITftpChannel CreateConnection()
        {
            return new UdpChannel(new UdpClient(TEST_PORT));
        }

        [Test]
        public void Send()
        {
            UdpClient client = new UdpClient(new IPEndPoint(IPAddress.Any, 0));

            using (ITftpChannel conn = new UdpChannel(client))
            {
                conn.Open();
                conn.SetRemoteEndPoint(new IPEndPoint(IPAddress.Loopback, 69));
                conn.Send(new Acknowledgement(1));
            }
        }
    }
}
