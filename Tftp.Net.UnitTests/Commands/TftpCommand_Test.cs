using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Tftp.Net.UnitTests
{
    [TestFixture]
    class TftpCommand_Test
    {
        [Test]
        public void CreateAck()
        {
            Acknowledgement command = new Acknowledgement(100);
            Assert.AreEqual(command.BlockNumber, 100);
        }

        [Test]
        public void CreateError()
        {
            Error command = new Error(123, "Hallo Welt");
            Assert.AreEqual(command.ErrorCode, 123);
            Assert.AreEqual(command.Message, "Hallo Welt");
        }

        [Test]
        public void CreateReadRequest()
        {
            ReadRequest command = new ReadRequest(@"C:\bla\blub.txt", TftpModeType.octet);
            Assert.AreEqual(command.Filename, @"C:\bla\blub.txt");
            Assert.AreEqual(command.Mode, TftpModeType.octet);
        }

        [Test]
        public void CreateWriteRequest()
        {
            WriteRequest command = new WriteRequest(@"C:\bla\blub.txt", TftpModeType.octet);
            Assert.AreEqual(command.Filename, @"C:\bla\blub.txt");
            Assert.AreEqual(command.Mode, TftpModeType.octet);
        }

        [Test]
        public void CreateData()
        {
            byte[] data = new byte[] { 1, 2, 3 };
            Data command = new Data(150, data);
            Assert.AreEqual(command.BlockNumber, 150);
            Assert.AreEqual(command.Bytes, data);
        }

    }
}
