using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tftp.Net
{
    /// <summary>
    /// Base class for all errors that may be passed to <code>ITftpTransfer.OnError</code>.
    /// </summary>
    public abstract class TftpTransferError
    {
        public abstract override String ToString();
    }

    /// <summary>
    /// Errors that are sent from the remote party using the TFTP Error Packet are represented
    /// by this class.
    /// </summary>
    public class ErrorFromRemoteEndpoint : TftpTransferError
    {
        /// <summary>
        /// Error code that was sent from the other party.
        /// </summary>
        public ushort ErrorCode { get; private set; }

        /// <summary>
        /// Error description that was sent by the other party.
        /// </summary>
        public string ErrorMessage { get; private set; }

        public ErrorFromRemoteEndpoint(ushort errorCode, string errorMessage)
        {
            if (String.IsNullOrEmpty(errorMessage))
                throw new ArgumentException("You must provide an errorMessage.");

            this.ErrorCode = errorCode;
            this.ErrorMessage = errorMessage;
        }

        public override string ToString()
        {
            return ErrorCode + " - " + ErrorMessage;
        }
    }

    /// <summary>
    /// Network errors (i.e. socket exceptions) are represented by this class.
    /// </summary>
    public class NetworkError : TftpTransferError
    {
        public Exception Exception { get; private set; }

        public NetworkError(Exception exception)
        {
            this.Exception = exception;
        }

        public override string ToString()
        {
            return Exception.ToString();
        }

    }
}
