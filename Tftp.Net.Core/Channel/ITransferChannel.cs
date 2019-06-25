using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Tftp.Net.Channel
{
    delegate void TftpCommandHandler(ITftpCommand command, EndPoint endpoint);
    delegate void TftpChannelErrorHandler(TftpTransferError error);

    interface ITransferChannel : IDisposable
    {
        event TftpCommandHandler OnCommandReceived;
        event TftpChannelErrorHandler OnError;

        EndPoint RemoteEndpoint { get; set; }

        void Open();
        void Send(ITftpCommand command);
    }
}
