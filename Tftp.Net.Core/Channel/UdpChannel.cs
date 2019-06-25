using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Diagnostics;

namespace Tftp.Net.Channel
{
    class UdpChannel : ITransferChannel
    {
        public event TftpCommandHandler OnCommandReceived;
        public event TftpChannelErrorHandler OnError;

        private IPEndPoint endpoint;
        private UdpClient client;
        private readonly CommandSerializer serializer = new CommandSerializer();
        private readonly CommandParser parser = new CommandParser();

        public UdpChannel(UdpClient client)
        {
            this.client = client;
            this.endpoint = null;
        }

        public void Open()
        {
            if (client == null)
                throw new ObjectDisposedException("UdpChannel");

            client.BeginReceive(UdpReceivedCallback, null);
        }

        void UdpReceivedCallback(IAsyncResult result)
        {
            IPEndPoint endpoint = new IPEndPoint(0, 0);
            ITftpCommand command = null;

            try
            {
                byte[] data = null;

                lock (this)
                {
                    if (client == null)
                        return;

                    data = client.EndReceive(result, ref endpoint);
                }
                command = parser.Parse(data);
            }
            catch (SocketException e)
            {
                //Handle receive error
                RaiseOnError(new NetworkError(e));
            }
            catch (TftpParserException e2)
            {
                //Handle parser error
                RaiseOnError(new NetworkError(e2));
            }

            if (command != null)
            {
                RaiseOnCommand(command, endpoint);
            }

            lock (this)
            {
                if (client != null)
                    client.BeginReceive(UdpReceivedCallback, null);
            }
        }

        private void RaiseOnCommand(ITftpCommand command, IPEndPoint endpoint)
        {
            if (OnCommandReceived != null)
                OnCommandReceived(command, endpoint);
        }

        private void RaiseOnError(TftpTransferError error)
        {
            if (OnError != null)
                OnError(error);
        }

        public void Send(ITftpCommand command)
        {
            if (client == null)
                throw new ObjectDisposedException("UdpChannel");

            if (endpoint == null)
                throw new InvalidOperationException("RemoteEndpoint needs to be set before you can send TFTP commands.");

            using (MemoryStream stream = new MemoryStream())
            {
                serializer.Serialize(command, stream);
                byte[] data = stream.GetBuffer();
                client.Send(data, (int)stream.Length, endpoint);
            }
        }

        public void Dispose()
        {
            lock (this)
            {
                if (this.client == null)
                    return;

                client.Close();
                this.client = null;
            }
        }

        public EndPoint RemoteEndpoint
        {
            get
            {
                return endpoint;
            }

            set
            {
                if (!(value is IPEndPoint))
                    throw new NotSupportedException("UdpChannel can only connect to IPEndPoints.");

                if (client == null)
                    throw new ObjectDisposedException("UdpChannel");

                this.endpoint = (IPEndPoint)value;
            }
        }
    }
}
