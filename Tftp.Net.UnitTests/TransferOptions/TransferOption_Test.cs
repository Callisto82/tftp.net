using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Tftp.Net.Transfer;

namespace Tftp.Net.UnitTests.TransferOptions
{
    [TestFixture]
    class TransferOption_Test
    {
        [Test]
        public void CanBeCreatedWithValidNameAndValue()
        {
            TransferOption option = new TransferOption("Test", "Hallo Welt");
            Assert.AreEqual("Test", option.Name);
            Assert.AreEqual("Hallo Welt", option.Value);
            Assert.IsFalse(option.IsAcknowledged);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void RejectsInvalidName1()
        {
            TransferOption option = new TransferOption("", "Hallo Welt");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void RejectsInvalidName2()
        {
            TransferOption option = new TransferOption(null, "Hallo Welt");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RejectsInvalidValue()
        {
            TransferOption option = new TransferOption("Test", null);
        }

        [Test]
        public void AcceptsEmptyValue()
        {
            //Must not throw any exceptions
            TransferOption option = new TransferOption("Test", "");
        }
    }
}


