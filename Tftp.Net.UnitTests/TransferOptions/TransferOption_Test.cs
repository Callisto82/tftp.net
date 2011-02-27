using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Tftp.Net.TransferOptions;

namespace Tftp.Net.UnitTests.Transfer
{
    [TestFixture]
    class TransferOption_Test
    {
        [Test]
        public void Create()
        {
            TransferOption option = new TransferOption("Test", "Hallo Welt");
            Assert.AreEqual("Test", option.Name);
            Assert.AreEqual("Hallo Welt", option.Value);
            Assert.IsFalse(option.IsAcknowledged);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void InvalidName1()
        {
            TransferOption option = new TransferOption("", "Hallo Welt");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void InvalidName2()
        {
            TransferOption option = new TransferOption(null, "Hallo Welt");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InvalidValue()
        {
            TransferOption option = new TransferOption("Test", null);
        }

        [Test]
        public void EmptyValue()
        {
            //Must not throw any exceptions
            TransferOption option = new TransferOption("Test", "");
        }
    }
}


