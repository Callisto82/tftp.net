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
    class LoggingStateDecorator : ITftpState
    {
        private readonly ITftpState decoratee;

        public LoggingStateDecorator(ITftpState decoratee)
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
            Trace.WriteLine(GetStateName() + " OnCommand: " + command);
            decoratee.OnCommand(command, endpoint);
        }
    }
}
