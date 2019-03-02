using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using NUnit.Framework;
using Tftp.Net.Trace;

namespace Tftp.Net.UnitTests.Trace
{
    [TestFixture]
    class TftpTrace_Test
    {
        class TraceListenerMock : TraceListener
        {
            public bool WriteWasCalled = false;

            public override void Write(string message) { WriteWasCalled = true; }
            public override void WriteLine(string message) { WriteWasCalled = true; }
        }

        private TraceListenerMock listener;

        [SetUp]
        public void Setup()
        {
            listener = new TraceListenerMock();
            System.Diagnostics.Trace.Listeners.Add(listener);
        }

        [TearDown]
        public void Teardown()
        {
            System.Diagnostics.Trace.Listeners.Remove(listener);
            TftpTrace.Enabled = false;
        }

        [Test]
        public void CallsTrace()
        {
            TftpTrace.Enabled = true;
            Assert.IsFalse(listener.WriteWasCalled);
            TftpTrace.Trace("Test", new TransferStub());
            Assert.IsTrue(listener.WriteWasCalled);
        }

        [Test]
        public void DoesNotWriteWhenDisabled()
        {
            TftpTrace.Enabled = false;
            TftpTrace.Trace("Test", new TransferStub());
            Assert.IsFalse(listener.WriteWasCalled);
        }
    }
}
