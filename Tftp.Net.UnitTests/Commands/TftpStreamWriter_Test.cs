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
        private MemoryStream ms;
        private TftpStreamWriter tested;

        [SetUp]
        public void Setup()
        {
            ms = new MemoryStream();
            tested = new TftpStreamWriter(ms);
        }

        [Test]
        public void WritesSingleBytes()
        {
            tested.WriteByte(1);
            tested.WriteByte(2);
            tested.WriteByte(3);

            Assert.AreEqual(3, ms.Length);
            Assert.AreEqual(1, ms.GetBuffer()[0]);
            Assert.AreEqual(2, ms.GetBuffer()[1]);
            Assert.AreEqual(3, ms.GetBuffer()[2]);
        }

        [Test]
        public void WritesShorts()
        {
            tested.WriteUInt16(0x0102);
            tested.WriteUInt16(0x0304);

            Assert.AreEqual(4, ms.Length);
            Assert.AreEqual(1, ms.GetBuffer()[0]);
            Assert.AreEqual(2, ms.GetBuffer()[1]);
            Assert.AreEqual(3, ms.GetBuffer()[2]);
            Assert.AreEqual(4, ms.GetBuffer()[3]);
        }

        [Test]
        public void WritesArrays()
        {
            tested.WriteBytes(new byte[3] { 3, 4, 5 });

            Assert.AreEqual(3, ms.Length);
            Assert.AreEqual(3, ms.GetBuffer()[0]);
            Assert.AreEqual(4, ms.GetBuffer()[1]);
            Assert.AreEqual(5, ms.GetBuffer()[2]);
        }
    }
}

