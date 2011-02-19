using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;

namespace Tftp.Net.UnitTests
{
    [TestFixture]
    class TftpStreamWriter_Test
    {
        [Test]
        public void TestWriteByte()
        {
            MemoryStream ms = new MemoryStream();
            using (TftpStreamWriter writer = new TftpStreamWriter(ms))
            {
                writer.WriteByte(1);
                writer.WriteByte(2);
                writer.WriteByte(3);

                Assert.AreEqual(3, ms.Length);
                Assert.AreEqual(1, ms.GetBuffer()[0]);
                Assert.AreEqual(2, ms.GetBuffer()[1]);
                Assert.AreEqual(3, ms.GetBuffer()[2]);
            }
        }

        [Test]
        public void TestWriteUInt16()
        {
            MemoryStream ms = new MemoryStream();
            using (TftpStreamWriter writer = new TftpStreamWriter(ms))
            {
                writer.WriteUInt16(0x0102);
                writer.WriteUInt16(0x0304);

                Assert.AreEqual(4, ms.Length);
                Assert.AreEqual(1, ms.GetBuffer()[0]);
                Assert.AreEqual(2, ms.GetBuffer()[1]);
                Assert.AreEqual(3, ms.GetBuffer()[2]);
                Assert.AreEqual(4, ms.GetBuffer()[3]);
            }
        }

        [Test]
        public void TestWriteBytes()
        {
            MemoryStream ms = new MemoryStream();
            using (TftpStreamWriter writer = new TftpStreamWriter(ms))
            {
                writer.WriteBytes(new byte[3] { 3, 4, 5 });

                Assert.AreEqual(3, ms.Length);
                Assert.AreEqual(3, ms.GetBuffer()[0]);
                Assert.AreEqual(4, ms.GetBuffer()[1]);
                Assert.AreEqual(5, ms.GetBuffer()[2]);
            }
        }
    }
}

