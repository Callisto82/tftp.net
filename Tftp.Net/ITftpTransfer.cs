using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Tftp.Net
{
    public delegate void TftpEventHandler(ITftpTransfer transfer);
    public delegate void TftpProgressHandler(ITftpTransfer transfer, int bytesTransferred);
    public delegate void TftpErrorHandler(ITftpTransfer transfer, TftpTransferError error);

    public enum TftpTransferMode
    {
        netascii,
        octet,
        mail
    }

    public interface ITftpTransfer : IDisposable
    {
        event TftpProgressHandler OnProgress;
        event TftpEventHandler OnFinished; 
        event TftpErrorHandler OnError;

        /// <summary>
        /// TFTP transfer options. For outgoing transfers, these can be modified.
        /// For incoming transfer requests, set the IsAcknowledged flag on all options that you would like to acknowledge.
        /// </summary>
        ITftpTransferOptions Options { get; }

        /// <summary>
        /// Requested TFTP transfer mode. For outgoing transfers, this member may be used to set the transfer mode.
        /// </summary>
        TftpTransferMode TransferMode { get; set; }

        /// <summary>
        /// Transfer blocksize.
        /// </summary>
        int BlockSize { get; }

        /// <summary>
        /// Timeout after which commands are sent again.
        /// </summary>
        TimeSpan Timeout { get; set; }

        /// <summary>
        /// Filename for the transferred file.
        /// </summary>
        String Filename { get; }

        /// <summary>
        /// You can set your own object here to associated custom data with this transfer.
        /// </summary>
        object UserContext { get; set; }

        /// <summary>
        /// Call this function to start the transfer.
        /// </summary>
        /// <param name="data">The stream from which data is either read (when sending) or written to (when receiving).</param>
        void Start(Stream data);

        /// <summary>
        /// Cancel the currently running transfer.
        /// </summary>
        void Cancel();
    }
}
