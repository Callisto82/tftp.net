using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Tftp.Net.Channel;

namespace Tftp.Net.UnitTests
{
    [TestFixture]
    abstract class ITftpChannel_Test
    {
        protected abstract IChannel CreateConnection();

        [Test]
        public void Create()
        {
            using (IChannel conn = CreateConnection())
            {
                Assert.False(conn.IsOpen);
                conn.Open();
                Assert.True(conn.IsOpen);
            }
        }

        [Test]
        public void DisabledAfterDispose()
        {
            IChannel conn = CreateConnection();
            conn.Open();
            Assert.True(conn.IsOpen);
            conn.Dispose();
            Assert.False(conn.IsOpen);
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void SendWithNullParameter()
        {
            using (IChannel conn = CreateConnection())
            {
                conn.Open();
                conn.Send(null);
            }
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void SendOnClosedParameter()
        {
            using (IChannel conn = CreateConnection())
            {
                conn.Send(new Acknowledgement(1));
            }
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void SendOnNotConnectedConnectionParameter()
        {
            using (IChannel conn = CreateConnection())
            {
                conn.Open();
                conn.Send(new Acknowledgement(1));
            }
        }
    }
}
