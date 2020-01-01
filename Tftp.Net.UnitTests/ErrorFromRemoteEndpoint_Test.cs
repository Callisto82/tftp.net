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
        public void CanBeCreatedWithValidValues()
        {
            TftpErrorPacket error = new TftpErrorPacket(123, "Test Message");
            Assert.AreEqual(123, error.ErrorCode);
            Assert.AreEqual("Test Message", error.ErrorMessage);
        }

        [Test]
        public void RejectsNullMessage()
        {
            Assert.That(() => new TftpErrorPacket(123, null), Throws.ArgumentException);
        }

        [Test]
        public void RejectsEmptyMessage()
        {
            Assert.That(() => new TftpErrorPacket(123, ""), Throws.ArgumentException);
        }
    }
}
