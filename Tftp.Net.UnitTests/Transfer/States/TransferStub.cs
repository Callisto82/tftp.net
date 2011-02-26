using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tftp.Net.Transfer;
using Tftp.Net.Transfer.States;
using Tftp.Net.Channel;
using System.Net;
using System.IO;

namespace Tftp.Net.UnitTests
{
    class TransferStub : TftpTransfer
    {
        private ChannelStub Channel { get { return (ChannelStub)connection; } }
        public List<ITftpCommand> SentCommands { get { return Channel.SentCommands; } }

        public TransferStub(MemoryStream stream)
            : base(new ChannelStub(), "dummy.txt") 
        {
            base.InputOutputStream = stream;
        }

        public TransferStub()
            : base(new ChannelStub(), "dummy.txt") { }

        public ITftpState State
        {
            get { return state; }
        }

        public void OnCommand(ITftpCommand command)
        {
            State.OnCommand(command, GetConnection().RemoteEndpoint);
        }

        public bool CommandWasSent(Type commandType)
        {
            return SentCommands.Any(x => x.GetType().IsAssignableFrom(commandType));
        }

        protected override ITftpState Decorate(ITftpState state)
        {
            return state;
        }

        public void OnTimer()
        {
            state.OnTimer();
        }

        public override void Dispose()
        {
            //Dont dispose the input/output stream during unit tests
            this.InputOutputStream = null;

            base.Dispose();
        }
    }

    class ChannelStub : ITftpChannel
    {
        public event TftpCommandHandler OnCommandReceived;
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
