using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Tftp.Net.UnitTests
{
    [TestFixture]
    class ErrorFromRemoteEndpoint_Test
    {
        [Test]
        public void Create()
        {
            TftpErrorPacket error = new TftpErrorPacket(123, "Test Message");
            Assert.AreEqual(123, error.ErrorCode);
            Assert.AreEqual("Test Message", error.ErrorMessage);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateInvalidMessage1()
        {
            TftpErrorPacket error = new TftpErrorPacket(123, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateInvalidMessage2()
        {
            TftpErrorPacket error = new TftpErrorPacket(123, "");
        }
    }
}
