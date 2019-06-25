using System;
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
            Assert.Throws<ArgumentException>(
                () => new TftpErrorPacket(123, null));
        }

        [Test]
        public void RejectsEmptyMessage()
        {
            Assert.Throws<ArgumentException>(
                () => new TftpErrorPacket(123, ""));
        }
    }
}
