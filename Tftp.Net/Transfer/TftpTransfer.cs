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

namespace Tftp.Net.Transfer
{
    class TftpTransfer : ITftpTransfer
    {
        private const int DEFAULT_BLOCKSIZE = 512;
        protected ITftpState state;
        protected readonly ITftpChannel connection;

        public Stream InputOutputStream { get; protected set; }

        public TftpTransfer(ITftpChannel connection, String filename)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");

            this.Filename = filename;
            this.state = null;
            this.OptionsBackend = new TransferOptions();
            this.Options = OptionsBackend;

            this.connection = connection;
            this.connection.OnCommandReceived += new TftpCommandHandler(connection_OnCommandReceived);
            this.connection.Open();
        }

        private void connection_OnCommandReceived(ITftpCommand command, EndPoint endpoint)
        {
            lock (this)
            {
                state.OnCommand(command, endpoint);
            }
        }

        internal virtual void SetState(ITftpState newState)
        {
            if (newState == null)
                throw new ArgumentNullException("newState");

            state = Decorate(newState);
            state.OnStateEnter();
        }

        protected virtual ITftpState Decorate(ITftpState state)
        {
            return new LoggingStateDecorator(state);
        }

        internal ITftpChannel GetConnection()
        {
            return connection;
        }

        internal void RaiseOnProgress(int bytesTransferred)
        {
            if (OnProgress != null)
                OnProgress(this, bytesTransferred);
        }

        internal void RaiseOnError(ushort code, String error)
        {
            if (OnError != null)
                OnError(this, code, error);
        }

        internal void RaiseOnFinished()
        {
            if (OnFinished != null)
                OnFinished(this);
        }


        #region ITftpTransfer

        public event TftpProgressHandler OnProgress;
        public event TftpEventHandler OnFinished;
        public event TftpErrorHandler OnError;

        public object UserContext { get; set; }
        public TimeSpan Timeout { get; set; }
        public TransferOptions OptionsBackend { get; private set; }
        public ITftpTransferOptions Options { get; protected set; }
        public string Filename { get; private set; }

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

        private int GetBlockSize()
        {
            return DEFAULT_BLOCKSIZE;
        }

        public int BlockSize
        {
            get { return GetBlockSize(); }
        }

        public virtual TftpTransferMode TransferMode { get;  set; }

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

        public void RemoveOptionsThatWereNotAcknowledged()
        {
            foreach (TftpTransferOption option in OptionsBackend.Where(x => !x.IsAcknowledged).ToList())
                OptionsBackend.Remove(option.Name);
        }

        public void SetOptionsAcknowledged(IEnumerable<TftpTransferOption> acknowledgedOptions)
        {
            foreach (TftpTransferOption option in Options)
                option.IsAcknowledged = acknowledgedOptions.Any(x => x.Name == option.Name);
        }
    }
}
