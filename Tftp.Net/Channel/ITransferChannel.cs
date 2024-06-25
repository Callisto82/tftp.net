using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Tftp.Net.Channel
{
    public delegate void TftpCommandHandler(ITftpCommand command, EndPoint endpoint);
    public delegate void TftpChannelErrorHandler(TftpTransferError error);

    public interface ITransferChannel : IDisposable
    {
        event TftpCommandHandler OnCommandReceived;
        event TftpChannelErrorHandler OnError;

        EndPoint RemoteEndpoint { get; set; }

        void Open();
        void Send(ITftpCommand command);
    }
}
