using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Net;
using Tftp.Net.Transfer;
using Tftp.Net.Transfer.States;
using Tftp.Net.Channel;
using System.Threading;
using Tftp.Net.TransferOptions;
using Tftp.Net.Trace;

namespace Tftp.Net.Transfer
{
    class TftpTransfer : ITftpTransfer
    {
        private const int DEFAULT_BLOCKSIZE = 512;

        protected ITransferState state;
        protected readonly IChannel connection;

        public Stream InputOutputStream { get; protected set; }

        public TftpTransfer(IChannel connection, String filename)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");

            this.BlockSize = DEFAULT_BLOCKSIZE;
            this.Filename = filename;
            this.state = null;
            this.Timeout = new TimeSpan(0, 0, 5); // set the default timeout to 5 seconds

            this.connection = connection;
            this.connection.OnCommandReceived += new TftpCommandHandler(connection_OnCommandReceived);
            this.connection.OnError += new TftpChannelErrorHandler(connection_OnError);
            this.connection.Open();
        }

        void connection_OnError(TftpTransferError error)
        {
            RaiseOnError(error);
        }

        private void connection_OnCommandReceived(ITftpCommand command, EndPoint endpoint)
        {
            lock (this)
            {
                state.OnCommand(command, endpoint);
            }
        }

        internal virtual void SetState(ITransferState newState)
        {
            if (newState == null)
                throw new ArgumentNullException("newState");

            state = Decorate(newState);
            state.OnStateEnter();
        }

        protected virtual ITransferState Decorate(ITransferState state)
        {
            return new LoggingStateDecorator(state, this);
        }

        internal IChannel GetConnection()
        {
            return connection;
        }

        internal void RaiseOnProgress(int bytesTransferred)
        {
            if (OnProgress != null)
                OnProgress(this, bytesTransferred);
        }

        internal void RaiseOnError(TftpTransferError error)
        {
            if (OnError != null)
                OnError(this, error);
        }

        internal void RaiseOnFinished()
        {
            if (OnFinished != null)
                OnFinished(this);
        }

        public override string ToString()
        {
            return GetHashCode() + " (" + Filename + ")";
        }


        #region ITftpTransfer

        public event TftpProgressHandler OnProgress;
        public event TftpEventHandler OnFinished;
        public event TftpErrorHandler OnError;

        public object UserContext { get; set; }
        public TimeSpan Timeout { get; set; }
        public ITftpTransferOptions Options { get; protected set; }
        public string Filename { get; private set; }
        public int BlockSize { get; set; }
        public virtual TftpTransferMode TransferMode { get; set; }

        public void Start(Stream data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            if (InputOutputStream != null)
                throw new InvalidOperationException("This transfer has already been started.");

            this.InputOutputStream = data;

            lock (this)
            {
                state.OnStart();
            }
        }

        public void Cancel()
        {
            lock (this)
            {
                state.OnCancel();
            }
        }

        public virtual void Dispose()
        {
            lock (this)
            {
                Cancel();

                if (InputOutputStream != null)
                {
                    InputOutputStream.Close();
                    InputOutputStream = null;
                }

                connection.Dispose();
            }
        }

        #endregion
    }
}
