using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;

namespace Tftp.Net.UnitTests
{
    [TestFixture]
    class TftpCommandParserAndSerializer_Test
    {
        private byte[] Serialize(ITftpCommand command)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                CommandSerializer serializer = new CommandSerializer();
                serializer.Serialize(command, stream);
                byte[] commandAsBytes = stream.GetBuffer();
                Array.Resize(ref commandAsBytes, (int)stream.Length);
                return commandAsBytes;
            }
        }

        [Test]
        public void ParsesAck()
        {
            Acknowledgement original = new Acknowledgement(10);
            CommandParser parser = new CommandParser();

            Acknowledgement parsed = (Acknowledgement)parser.Parse(Serialize(original));
            Assert.AreEqual(original.BlockNumber, parsed.BlockNumber);
        }

        [Test]
        public void ParsesError()
        {
            Error original = new Error(15, "Hallo Welt");
            CommandParser parser = new CommandParser();

            Error parsed = (Error)parser.Parse(Serialize(original));
            Assert.AreEqual(original.ErrorCode, parsed.ErrorCode);
            Assert.AreEqual(original.Message, parsed.Message);
        }

        [Test]
        public void ParsesReadRequest()
        {
            ReadRequest original = new ReadRequest("Hallo Welt.txt", TftpTransferMode.netascii, null);
            CommandParser parser = new CommandParser();

            ReadRequest parsed = (ReadRequest)parser.Parse(Serialize(original));
            Assert.AreEqual(original.Filename, parsed.Filename);
            Assert.AreEqual(original.Mode, parsed.Mode);
        }

        [Test]
        public void ParsesWriteRequest()
        {
            WriteRequest original = new WriteRequest("Hallo Welt.txt", TftpTransferMode.netascii, null);
            CommandParser parser = new CommandParser();

            WriteRequest parsed = (WriteRequest)parser.Parse(Serialize(original));
            Assert.AreEqual(original.Filename, parsed.Filename);
            Assert.AreEqual(original.Mode, parsed.Mode);
        }

        [Test]
        public void ParsesData()
        {
            byte[] data = { 12, 15, 19, 0, 4 };
            Data original = new Data(123, data);
            CommandParser parser = new CommandParser();

            Data parsed = (Data)parser.Parse(Serialize(original));
            Assert.AreEqual(original.BlockNumber, parsed.BlockNumber);
            Assert.AreEqual(original.Bytes.Length, parsed.Bytes.Length);

            for (int i = 0; i < original.Bytes.Length; i++)
                Assert.AreEqual(original.Bytes[i], parsed.Bytes[i]);
        }
    }
}
