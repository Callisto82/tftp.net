using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Tftp.Net.Channel;

namespace Tftp.Net.UnitTests
{
    [TestFixture]
    abstract class ITransferChannel_Test
    {
        protected abstract ITransferChannel CreateConnection();

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void SendWithNullParameter()
        {
            using (ITransferChannel conn = CreateConnection())
            {
                conn.Open();
                conn.Send(null);
            }
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void SendOnClosedParameter()
        {
            using (ITransferChannel conn = CreateConnection())
            {
                conn.Send(new Acknowledgement(1));
            }
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void SendOnNotConnectedConnectionParameter()
        {
            using (ITransferChannel conn = CreateConnection())
            {
                conn.Open();
                conn.Send(new Acknowledgement(1));
            }
        }
    }
}
