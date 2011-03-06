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

    /// <summary>
    /// Represents a single data transfer between a TFTP server and client.
    /// </summary>
    public interface ITftpTransfer : IDisposable
    {
        /// <summary>
        /// Event that is being called while data is being transferred.
        /// </summary>
        event TftpProgressHandler OnProgress;

        /// <summary>
        /// Event that will be called once the data transfer is finished.
        /// </summary>
        event TftpEventHandler OnFinished; 

        /// <summary>
        /// Event that will be called if there is an error during the data transfer.
        /// Currently, this will return instances of ErrorFromRemoteEndpoint or NetworkError.
        /// </summary>
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
