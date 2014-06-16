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
using Tftp.Net.Trace;

namespace Tftp.Net.Transfer
{
    class TftpTransfer : ITftpTransfer
    {
        protected ITransferState state;
        protected TftpTransferOptions tftpOptions = new TftpTransferOptions();
        protected readonly IChannel connection;

        public Stream InputOutputStream { get; protected set; }

        public TftpTransfer(IChannel connection, String filename)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");

            this.Filename = filename;
            this.state = null;
            this.RetryCount = 5;

            this.connection = connection;
            this.connection.OnCommandReceived += new TftpCommandHandler(connection_OnCommandReceived);
            this.connection.OnError += new TftpChannelErrorHandler(connection_OnError);
            this.connection.Open();
        }

        void connection_OnError(TftpTransferError error)
        {
            lock (this)
            {
                RaiseOnError(error);
            }
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

            state = DecorateForLogging(newState);
            state.OnStateEnter();
        }

        protected virtual ITransferState DecorateForLogging(ITransferState state)
        {
            return TftpTrace.Enabled ? new LoggingStateDecorator(state, this) : state;
        }

        internal IChannel GetConnection()
        {
            return connection;
        }

        internal void RaiseOnProgress(int bytesTransferred)
        {
            if (OnProgress != null)
                OnProgress(this, new TftpTransferProgress(bytesTransferred, ExpectedSize));
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

        internal void SetActiveTransferOptions(IEnumerable<TransferOption> options)
        {
            tftpOptions.SetActiveOptions(options);
        }

        internal List<TransferOption> GetActiveTransferOptions()
        {
            return tftpOptions.GetActiveOptions();
        }

        public override string ToString()
        {
            return GetHashCode() + " (" + Filename + ")";
        }

        internal void FillOrDisableTransferSizeOption()
        {
            try
            {
                if (InputOutputStream.Length > 0)
                    tftpOptions.TransferSize = (int)InputOutputStream.Length;
            }
            catch (NotSupportedException) { }
            finally
            {
                if (tftpOptions.TransferSize <= 0)
                    tftpOptions.IsTransferSizeOptionActive = false;
            }
        }

        #region ITftpTransfer

        public event TftpProgressHandler OnProgress;
        public event TftpEventHandler OnFinished;
        public event TftpErrorHandler OnError;

        public string Filename { get; private set; }
        public int RetryCount { get; set; }
        public virtual TftpTransferMode TransferMode { get; set; }
        public virtual TimeSpan RetryTimeout 
        {
            get { return tftpOptions.Timeout; }
            set { tftpOptions.Timeout = value; }
        }

        public virtual int ExpectedSize 
        {
            get { return tftpOptions.TransferSize; }
            set { tftpOptions.TransferSize = value; }
        }

        public virtual int BlockSize 
        { 
            get { return tftpOptions.BlockSize; }
            set { tftpOptions.BlockSize = value; }
        }

        public object UserContext { get; set; }

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

        public void Cancel(TftpErrorPacket reason)
        {
            lock (this)
            {
                state.OnCancel(reason);
            }
        }

        public virtual void Dispose()
        {
            lock (this)
            {
                Cancel(new TftpErrorPacket(0, "ITftpTransfer has been disposed."));

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
