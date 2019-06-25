using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Tftp.Net
{
    public delegate void TftpEventHandler(ITftpTransfer transfer);
    public delegate void TftpProgressHandler(ITftpTransfer transfer, TftpTransferProgress progress);
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
        /// Requested TFTP transfer mode. For outgoing transfers, this member may be used to set the transfer mode.
        /// </summary>
        TftpTransferMode TransferMode { get; set; }

        /// <summary>
        /// Transfer blocksize. Set this member to control the TFTP blocksize option (RFC 2349).
        /// </summary>
        int BlockSize { get; set; }

        /// <summary>
        /// Timeout after which commands are sent again.
        /// This member is also transmitted as the TFTP timeout interval option (RFC 2349).
        /// </summary>
        TimeSpan RetryTimeout { get; set; }

        /// <summary>
        /// Number of times that a RetryTimeout may occour before the transfer is cancelled with a TimeoutError.
        /// </summary>
        int RetryCount { get; set; }

        /// <summary>
        /// Tftp can transfer up to 65535 blocks. After that, the block counter wraps to either zero or one, depending on the expectations of the client.
        /// </summary>
        BlockCounterWrapAround BlockCounterWrapping { get; set; }

        /// <summary>
        /// Expected transfer size in bytes. 0 if size is unknown.
        /// </summary>
        long ExpectedSize { get; set; }

        /// <summary>
        /// Filename for the transferred file.
        /// </summary>
        String Filename { get; }

        /// <summary>
        /// You can set your own object here to associate custom data with this transfer.
        /// </summary>
        object UserContext { get; set; }

        /// <summary>
        /// Call this function to start the transfer.
        /// </summary>
        /// <param name="data">The stream from which data is either read (when sending) or written to (when receiving).</param>
        void Start(Stream data);

        /// <summary>
        /// Cancel the currently running transfer, possibly sending the provided reason to the remote endpoint.
        /// </summary>
        void Cancel(TftpErrorPacket reason);
    }
}
