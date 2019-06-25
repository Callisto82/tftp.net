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
        public TftpTransfer Context { get; set; }

        public virtual void OnStateEnter()
        {
            //no-op
        }

        public virtual void OnStart()
        {
        }

        public virtual void OnCancel(TftpErrorPacket reason)
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
