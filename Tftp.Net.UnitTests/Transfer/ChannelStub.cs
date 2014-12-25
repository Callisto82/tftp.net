using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Tftp.Net.Channel;

namespace Tftp.Net.UnitTests.Transfer
{
    class ChannelStub : IChannel
    {
        public event TftpCommandHandler OnCommandReceived;
        public event TftpChannelErrorHandler OnError;
        public bool IsOpen { get; private set; }
        public EndPoint RemoteEndpoint { get; set; }
        public readonly List<ITftpCommand> SentCommands = new List<ITftpCommand>();

        public ChannelStub()
        {
            IsOpen = false;
            RemoteEndpoint = new IPEndPoint(IPAddress.Loopback, 69);
        }

        public void Open()
        {
            IsOpen = true;
        }

        public void RaiseCommandReceived(ITftpCommand command, EndPoint endpoint)
        {
            if (OnCommandReceived != null)
                OnCommandReceived(command, endpoint);
        }

        public void RaiseOnError(TftpTransferError error)
        {
            if (OnError != null)
                OnError(error);
        }

        public void Send(ITftpCommand command)
        {
            SentCommands.Add(command);
        }

        public void Dispose()
        {
            IsOpen = false;
        }
    }
}
