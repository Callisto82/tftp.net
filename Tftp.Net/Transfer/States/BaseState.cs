using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

namespace Tftp.Net.Transfer.States
{
    class BaseState : ITransferState
    {
        protected TftpTransfer Context { get; private set; }

        public BaseState(TftpTransfer context)
        {
            this.Context = context;
        }

        public virtual void OnStateEnter()
        {
            //no-op
        }

        public virtual void OnStart()
        {
        }

        public virtual void OnCancel()
        {
        }

        public virtual void OnCommand(ITftpCommand command, EndPoint endpoint)
        {
        }

        public virtual void OnTimer()
        {
            //Ignore timer events
        }
    }
}
