using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;

namespace Tftp.Net.UnitTests
{
    [TestFixture]
    class TftpCommandParser_Test
    {
        private byte[] CommandToBytes(ITftpCommand command)
        {
            using (MemoryStream stream = new MemoryStream())
            using (TftpStreamWriter writer = new TftpStreamWriter(stream))
            {
                command.Write(writer);
                byte[] commandAsBytes = stream.GetBuffer();
                Array.Resize(ref commandAsBytes, (int)stream.Length);
                return commandAsBytes;
            }
        }

        [Test]
        public void ParseAck()
        {
            Acknowledgement original = new Acknowledgement(10);
            TftpCommandParser parser = new TftpCommandParser();

            Acknowledgement parsed = (Acknowledgement)parser.Parse(CommandToBytes(original));
            Assert.AreEqual(original.BlockNumber, parsed.BlockNumber);
        }

        [Test]
        public void ParseError()
        {
            Error original = new Error(15, "Hallo Welt");
            TftpCommandParser parser = new TftpCommandParser();

            Error parsed = (Error)parser.Parse(CommandToBytes(original));
            Assert.AreEqual(original.ErrorCode, parsed.ErrorCode);
            Assert.AreEqual(original.Message, parsed.Message);
        }

        [Test]
        public void ParseReadRequest()
        {
            ReadRequest original = new ReadRequest("Hallo Welt.txt", TftpModeType.netascii);
            TftpCommandParser parser = new TftpCommandParser();

            ReadRequest parsed = (ReadRequest)parser.Parse(CommandToBytes(original));
            Assert.AreEqual(original.Filename, parsed.Filename);
            Assert.AreEqual(original.Mode, parsed.Mode);
        }

        [Test]
        public void ParseWriteRequest()
        {
            WriteRequest original = new WriteRequest("Hallo Welt.txt", TftpModeType.netascii);
            TftpCommandParser parser = new TftpCommandParser();

            WriteRequest parsed = (WriteRequest)parser.Parse(CommandToBytes(original));
            Assert.AreEqual(original.Filename, parsed.Filename);
            Assert.AreEqual(original.Mode, parsed.Mode);
        }

        [Test]
        public void ParseData()
        {
            byte[] data = { 12, 15, 19, 0, 4 };
            Data original = new Data(123, data);
            TftpCommandParser parser = new TftpCommandParser();

            Data parsed = (Data)parser.Parse(CommandToBytes(original));
            Assert.AreEqual(original.BlockNumber, parsed.BlockNumber);
            Assert.AreEqual(original.Bytes.Length, parsed.Bytes.Length);

            for (int i = 0; i < original.Bytes.Length; i++)
                Assert.AreEqual(original.Bytes[i], parsed.Bytes[i]);
        }
    }
}
