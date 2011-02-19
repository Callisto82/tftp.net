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

namespace Tftp.Net.Transfer
{
    class TftpTransfer : ITftpTransfer
    {
        protected ITftpState state;
        protected readonly ITftpChannel connection;

        public TftpTransfer(ITftpChannel connection, String filename)
            : this(connection, filename, null) { }

        public TftpTransfer(ITftpChannel connection, String filename, ITftpState initialState)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");

            this.Filename = filename;
            this.state = null;

            if (initialState != null)
                SetState(initialState);

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
        public string Filename { get; private set; }

        public void Start(Stream data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            lock (this)
            {
                state.OnStart(data);
            }
        }

        public void Cancel()
        {
            lock (this)
            {
                state.OnCancel();
            }
        }

        public void Dispose()
        {
            lock (this)
            {
                Cancel();
                connection.Dispose();
            }
        }

        #endregion
    }
}
