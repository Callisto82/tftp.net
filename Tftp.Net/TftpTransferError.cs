using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tftp.Net
{
    public abstract class TftpTransferError
    {
        public abstract override String ToString();
    }

    public class ErrorFromRemoteEndpoint : TftpTransferError
    {
        public ushort ErrorCode { get; private set; }
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
