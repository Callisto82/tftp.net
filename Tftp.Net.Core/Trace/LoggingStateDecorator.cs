using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Tftp.Net.Transfer.States;
using System.IO;
using Tftp.Net.Channel;
using System.Net;
using Tftp.Net.Trace;
using Tftp.Net.Transfer;

namespace Tftp.Net.Trace
{
    class LoggingStateDecorator : ITransferState
    {
        public TftpTransfer Context 
        {
            get { return decoratee.Context; }
            set { decoratee.Context = value; }
        }

        private readonly ITransferState decoratee;
        private readonly TftpTransfer transfer;

        public LoggingStateDecorator(ITransferState decoratee, TftpTransfer transfer)
        {
            this.decoratee = decoratee;
            this.transfer = transfer;
        }

        public String GetStateName()
        {
            return "[" + decoratee.GetType().Name + "]";
        }

        public void OnStateEnter()
        {
            TftpTrace.Trace(GetStateName() + " OnStateEnter", transfer);
            decoratee.OnStateEnter();
        }

        public void OnStart()
        {
            TftpTrace.Trace(GetStateName() + " OnStart", transfer);
            decoratee.OnStart();
        }

        public void OnCancel(TftpErrorPacket reason)
        {
            TftpTrace.Trace(GetStateName() + " OnCancel: " + reason, transfer);
            decoratee.OnCancel(reason);
        }

        public void OnCommand(ITftpCommand command, EndPoint endpoint)
        {
            TftpTrace.Trace(GetStateName() + " OnCommand: " + command + " from " + endpoint, transfer);
            decoratee.OnCommand(command, endpoint);
        }

        public void OnTimer()
        {
            decoratee.OnTimer();
        }
    }
}
