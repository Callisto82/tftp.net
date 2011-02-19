﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Tftp.Net.Channel
{
    delegate void TftpCommandHandler(ITftpCommand command, EndPoint endpoint);

    interface ITftpChannel : IDisposable
    {
        event TftpCommandHandler OnCommandReceived;
        bool IsOpen { get; }

        void Open();
        void SetRemoteEndPoint(EndPoint endpoint);
        void Send(ITftpCommand command);
    }
}
