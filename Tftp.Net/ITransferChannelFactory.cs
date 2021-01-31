using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Tftp.Net.Channel;

namespace Tftp.Net
{
    public interface ITransferChannelFactory
    {
        ITransferChannel CreateServer(EndPoint localAddress);
        ITransferChannel CreateConnection(EndPoint remoteAddress);
    }
}
