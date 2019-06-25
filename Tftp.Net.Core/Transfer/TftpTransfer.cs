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
        protected readonly ITransferChannel connection;
        protected Timer timer;

        public TransferOptionSet ProposedOptions { get; set; }
        public TransferOptionSet NegotiatedOptions { get; private set; }
        public bool WasStarted { get; private set; }
        public Stream InputOutputStream { get; protected set; }

        public TftpTransfer(ITransferChannel connection, String filename, ITransferState initialState)
        {
            this.ProposedOptions = TransferOptionSet.NewDefaultSet();
            this.Filename = filename;
            this.RetryCount = 5;
            this.timer = new Timer(timer_OnTimer, null, 500, 500);
            this.SetState(initialState);
            this.connection = connection;
            this.connection.OnCommandReceived += new TftpCommandHandler(connection_OnCommandReceived);
            this.connection.OnError += new TftpChannelErrorHandler(connection_OnError);
            this.connection.Open();
        }

        private void timer_OnTimer(object context)
        {
            lock (this)
            {
                state.OnTimer();
            }
        }

        private void connection_OnCommandReceived(ITftpCommand command, EndPoint endpoint)
        {
            lock (this)
            {
                state.OnCommand(command, endpoint);
            }
        }

        private void connection_OnError(TftpTransferError error)
        {
            lock (this)
            {
                RaiseOnError(error);
            }
        }

        internal virtual void SetState(ITransferState newState)
        {
            state = DecorateForLogging(newState);
            state.Context = this;
            state.OnStateEnter();
        }

        protected virtual ITransferState DecorateForLogging(ITransferState state)
        {
            return TftpTrace.Enabled ? new LoggingStateDecorator(state, this) : state;
        }

        internal ITransferChannel GetConnection()
        {
            return connection;
        }

        internal void RaiseOnProgress(long bytesTransferred)
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

        internal void FinishOptionNegotiation(TransferOptionSet negotiated)
        {
            NegotiatedOptions = negotiated;
            if (!NegotiatedOptions.IncludesBlockSizeOption)
                NegotiatedOptions.BlockSize = TransferOptionSet.DEFAULT_BLOCKSIZE;

            if (!NegotiatedOptions.IncludesTimeoutOption)
                NegotiatedOptions.Timeout = TransferOptionSet.DEFAULT_TIMEOUT_SECS;
        }

        public override string ToString()
        {
            return GetHashCode() + " (" + Filename + ")";
        }

        internal void FillOrDisableTransferSizeOption()
        {
            try
            {
                ProposedOptions.TransferSize = (int)InputOutputStream.Length;
            }
            catch (NotSupportedException) { }
            finally
            {
                if (ProposedOptions.TransferSize <= 0)
                    ProposedOptions.IncludesTransferSizeOption = false;
            }
        }

        #region ITftpTransfer

        public event TftpProgressHandler OnProgress;
        public event TftpEventHandler OnFinished;
        public event TftpErrorHandler OnError;

        public string Filename { get; private set; }
        public int RetryCount { get; set; }
        public virtual TftpTransferMode TransferMode { get; set; }
        public object UserContext { get; set; }
        public virtual TimeSpan RetryTimeout 
        {
            get { return TimeSpan.FromSeconds(NegotiatedOptions != null ? NegotiatedOptions.Timeout : ProposedOptions.Timeout); }
            set { ThrowExceptionIfTransferAlreadyStarted(); ProposedOptions.Timeout = value.Seconds; }
        }

        public virtual long ExpectedSize 
        {
            get { return NegotiatedOptions != null ? NegotiatedOptions.TransferSize : ProposedOptions.TransferSize; }
            set { ThrowExceptionIfTransferAlreadyStarted(); ProposedOptions.TransferSize = value; }
        }

        public virtual int BlockSize 
        {
            get { return NegotiatedOptions != null ? NegotiatedOptions.BlockSize : ProposedOptions.BlockSize; }
            set { ThrowExceptionIfTransferAlreadyStarted(); ProposedOptions.BlockSize = value; }
        }

        private BlockCounterWrapAround wrapping = BlockCounterWrapAround.ToZero;
        public virtual BlockCounterWrapAround BlockCounterWrapping
        {
            get { return wrapping; }
            set { ThrowExceptionIfTransferAlreadyStarted(); wrapping = value; }
        }

        private void ThrowExceptionIfTransferAlreadyStarted()
        {
            if (WasStarted)
                throw new InvalidOperationException("You cannot change tftp transfer options after the transfer has been started.");
        }

        public void Start(Stream data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            if (WasStarted)
                throw new InvalidOperationException("This transfer has already been started.");

            this.WasStarted = true;
            this.InputOutputStream = data;

            lock (this)
            {
                state.OnStart();
            }
        }

        public void Cancel(TftpErrorPacket reason)
        {
            if (reason == null)
                throw new ArgumentNullException("reason");

            lock (this)
            {
                state.OnCancel(reason);
            }
        }

        public virtual void Dispose()
        {
            lock (this)
            {
                timer.Dispose();
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
