using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;

namespace Tftp.Net.UnitTests
{
    [TestFixture]
    class TftpStreamReader_Test
    {
        private byte[] Data = { 0x00, 0x01, 0x02, 0x03 };

        [Test]
        public void TestReadByte()
        {
            MemoryStream ms = new MemoryStream(Data);
            using (TftpStreamReader reader = new TftpStreamReader(ms))
            {
                Assert.AreEqual(0x00, reader.ReadByte());
                Assert.AreEqual(0x01, reader.ReadByte());
                Assert.AreEqual(0x02, reader.ReadByte());
                Assert.AreEqual(0x03, reader.ReadByte());
            }
        }

        [Test]
        public void TestReadUInt16()
        {
            MemoryStream ms = new MemoryStream(Data);
            using (TftpStreamReader reader = new TftpStreamReader(ms))
            {
                Assert.AreEqual(0x0001, reader.ReadUInt16());
                Assert.AreEqual(0x0203, reader.ReadUInt16());
            }
        }

        [Test]
        public void TestReadBytes1()
        {
            MemoryStream ms = new MemoryStream(Data);
            using (TftpStreamReader reader = new TftpStreamReader(ms))
            {
                byte[] bytes = reader.ReadBytes(2);
                Assert.AreEqual(2, bytes.Length);
                Assert.AreEqual(0x00, bytes[0]);
                Assert.AreEqual(0x01, bytes[1]);
            }
        }

        [Test]
        public void TestReadBytes2()
        {
            MemoryStream ms = new MemoryStream(Data);
            using (TftpStreamReader reader = new TftpStreamReader(ms))
            {
                byte[] bytes = reader.ReadBytes(4);
                Assert.AreEqual(4, bytes.Length);
                Assert.AreEqual(0x00, bytes[0]);
                Assert.AreEqual(0x01, bytes[1]);
                Assert.AreEqual(0x02, bytes[2]);
                Assert.AreEqual(0x03, bytes[3]);
            }
        }

        [Test]
        public void TestReadBytes3()
        {
            MemoryStream ms = new MemoryStream(Data);
            using (TftpStreamReader reader = new TftpStreamReader(ms))
            {
                byte[] bytes = reader.ReadBytes(10);
                Assert.AreEqual(4, bytes.Length);
                Assert.AreEqual(0x00, bytes[0]);
                Assert.AreEqual(0x01, bytes[1]);
                Assert.AreEqual(0x02, bytes[2]);
                Assert.AreEqual(0x03, bytes[3]);
            }
        }
    }
}

