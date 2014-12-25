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
        private TftpStreamReader tested;

        [SetUp]
        public void Setup()
        {
            MemoryStream ms = new MemoryStream(Data);
            tested = new TftpStreamReader(ms);
        }

        [Test]
        public void ReadsSingleBytes()
        {
            Assert.AreEqual(0x00, tested.ReadByte());
            Assert.AreEqual(0x01, tested.ReadByte());
            Assert.AreEqual(0x02, tested.ReadByte());
            Assert.AreEqual(0x03, tested.ReadByte());
        }

        [Test]
        public void ReadsShorts()
        {
            Assert.AreEqual(0x0001, tested.ReadUInt16());
            Assert.AreEqual(0x0203, tested.ReadUInt16());
        }

        [Test]
        public void ReadsIntoSmallerArrays()
        {
            byte[] bytes = tested.ReadBytes(2);
            Assert.AreEqual(2, bytes.Length);
            Assert.AreEqual(0x00, bytes[0]);
            Assert.AreEqual(0x01, bytes[1]);
        }

        [Test]
        public void ReadsIntoArraysWithPerfectSize()
        {
            byte[] bytes = tested.ReadBytes(4);
            Assert.AreEqual(4, bytes.Length);
            Assert.AreEqual(0x00, bytes[0]);
            Assert.AreEqual(0x01, bytes[1]);
            Assert.AreEqual(0x02, bytes[2]);
            Assert.AreEqual(0x03, bytes[3]);
        }

        [Test]
        public void ReadsIntoLargerArrays()
        {
            byte[] bytes = tested.ReadBytes(10);
            Assert.AreEqual(4, bytes.Length);
            Assert.AreEqual(0x00, bytes[0]);
            Assert.AreEqual(0x01, bytes[1]);
            Assert.AreEqual(0x02, bytes[2]);
            Assert.AreEqual(0x03, bytes[3]);
        }
    }
}

