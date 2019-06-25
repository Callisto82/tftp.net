using System;
using NUnit.Framework;

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
            Assert.Throws<ArgumentException>(
                () => new TransferOption("", "Hallo Welt"));
        }

        [Test]
        public void RejectsInvalidName2()
        {
            Assert.Throws<ArgumentException>(
                () => new TransferOption(null, "Hallo Welt"));
        }

        [Test]
        public void RejectsInvalidValue()
        {
            Assert.Throws<ArgumentNullException>(
                () => new TransferOption("Test", null));
        }

        [Test]
        public void AcceptsEmptyValue()
        {
            //Must not throw any exceptions
            TransferOption option = new TransferOption("Test", "");
        }
    }
}


