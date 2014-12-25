using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tftp.Net.Transfer;
using Tftp.Net.Transfer.States;
using Tftp.Net.Channel;
using System.Net;
using System.IO;

namespace Tftp.Net.UnitTests.Transfer
{
    class TransferStub : TftpTransfer
    {
        private ChannelStub Channel { get { return (ChannelStub)connection; } }
        public List<ITftpCommand> SentCommands { get { return Channel.SentCommands; } }
        public bool HadNetworkTimeout { get; set; }

        public TransferStub(MemoryStream stream)
            : base(new ChannelStub(), "dummy.txt") 
        {
            base.InputOutputStream = stream;
            HadNetworkTimeout = false;
            this.OnError += new TftpErrorHandler(TransferStub_OnError);
        }

        void TransferStub_OnError(ITftpTransfer transfer, TftpTransferError error)
        {
            if (error is TimeoutError)
                HadNetworkTimeout = true;
        }

        public TransferStub()
            : this(null) { }

        public ITransferState State
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

        protected override ITransferState DecorateForLogging(ITransferState state)
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
}