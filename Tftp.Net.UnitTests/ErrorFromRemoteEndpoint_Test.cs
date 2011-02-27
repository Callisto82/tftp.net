using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Tftp.Net.UnitTests
{
    [TestFixture]
    class ErrorFromRemoteEndpoint_Test
    {
        [Test]
        public void Create()
        {
            ErrorFromRemoteEndpoint error = new ErrorFromRemoteEndpoint(123, "Test Message");
            Assert.AreEqual(123, error.ErrorCode);
            Assert.AreEqual("Test Message", error.ErrorMessage);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateInvalidMessage1()
        {
            ErrorFromRemoteEndpoint error = new ErrorFromRemoteEndpoint(123, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateInvalidMessage2()
        {
            ErrorFromRemoteEndpoint error = new ErrorFromRemoteEndpoint(123, "");
        }
    }
}
