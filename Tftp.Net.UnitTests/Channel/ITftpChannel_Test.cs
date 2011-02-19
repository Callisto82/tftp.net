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
        protected abstract ITftpChannel CreateConnection();

        [Test]
        public void Create()
        {
            using (ITftpChannel conn = CreateConnection())
            {
                Assert.False(conn.IsOpen);
                conn.Open();
                Assert.True(conn.IsOpen);
            }
        }

        [Test]
        public void DisabledAfterDispose()
        {
            ITftpChannel conn = CreateConnection();
            conn.Open();
            Assert.True(conn.IsOpen);
            conn.Dispose();
            Assert.False(conn.IsOpen);
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void SendWithNullParameter()
        {
            using (ITftpChannel conn = CreateConnection())
            {
                conn.Open();
                conn.Send(null);
            }
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void SendOnClosedParameter()
        {
            using (ITftpChannel conn = CreateConnection())
            {
                conn.Send(new Acknowledgement(1));
            }
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void SendOnNotConnectedConnectionParameter()
        {
            using (ITftpChannel conn = CreateConnection())
            {
                conn.Open();
                conn.Send(new Acknowledgement(1));
            }
        }
    }
}
