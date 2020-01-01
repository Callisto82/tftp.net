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
        public void RejectsInvalidName1()
        {
            Assert.That(() => new TransferOption("", "Hallo Welt"), Throws.ArgumentException);
        }

        [Test]
        public void RejectsInvalidName2()
        {
            Assert.That(() => new TransferOption(null, "Hallo Welt"), Throws.ArgumentException);
        }

        [Test]
        public void RejectsInvalidValue()
        {
            Assert.That(() => new TransferOption("Test", null), Throws.ArgumentNullException);
        }

        [Test]
        public void AcceptsEmptyValue()
        {
            //Must not throw any exceptions
            TransferOption option = new TransferOption("Test", "");
        }
    }
}


