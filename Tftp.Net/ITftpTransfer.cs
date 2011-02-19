using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Tftp.Net
{
    public delegate void TftpEventHandler(ITftpTransfer transfer);
    public delegate void TftpProgressHandler(ITftpTransfer transfer, int bytesTransferred);
    public delegate void TftpErrorHandler(ITftpTransfer transfer, ushort code, string error);

    public interface ITftpTransfer : IDisposable
    {
        event TftpProgressHandler OnProgress;
        event TftpEventHandler OnFinished; 
        event TftpErrorHandler OnError;

        String Filename { get; }
        object UserContext { get; set; }

        void Start(Stream data);
        void Cancel();
    }
}
