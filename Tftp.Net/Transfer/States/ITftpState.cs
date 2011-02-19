using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Tftp.Net.Channel;
using System.Net;

namespace Tftp.Net.Transfer.States
{
    interface ITftpState
    {
        //Called by TftpTransfer
        void OnStateEnter();

        //Called if the user calls TftpTransfer.Start() or TftpTransfer.Cancel()
        void OnStart(Stream data);
        void OnCancel();

        //Called when a command is received
        void OnCommand(ITftpCommand command, EndPoint endpoint);
    }
}
