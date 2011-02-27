using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Tftp.Net.Transfer.States;
using System.IO;
using Tftp.Net.Channel;
using System.Net;

namespace Tftp.Net.Transfer
{
    class LoggingStateDecorator : ITransferState
    {
        private readonly ITransferState decoratee;

        public LoggingStateDecorator(ITransferState decoratee)
        {
            this.decoratee = decoratee;
        }

        public String GetStateName()
        {
            return "[" + decoratee.GetType().Name + "]";
        }

        public void OnStateEnter()
        {
            Trace.WriteLine(GetStateName() + " OnStateEnter");
            decoratee.OnStateEnter();
        }

        public void OnStart()
        {
            Trace.WriteLine(GetStateName() + " OnStart");
            decoratee.OnStart();
        }

        public void OnCancel()
        {
            Trace.WriteLine(GetStateName() + " OnCancel");
            decoratee.OnCancel();
        }

        public void OnCommand(ITftpCommand command, EndPoint endpoint)
        {
            Trace.WriteLine(GetStateName() + " OnCommand: " + command + " from " + endpoint);
            decoratee.OnCommand(command, endpoint);
        }

        public void OnTimer()
        {
            Trace.WriteLine(GetStateName() + " OnTimer");
            decoratee.OnTimer();
        }
    }
}
