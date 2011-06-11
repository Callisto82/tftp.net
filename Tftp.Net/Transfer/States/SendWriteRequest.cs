﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Tftp.Net.TransferOptions;
using Tftp.Net.Trace;

namespace Tftp.Net.Transfer.States
{
    class SendWriteRequest : BaseState
    {
        private readonly SimpleTimer timer;

        public SendWriteRequest(TftpTransfer context)
            : base(context) 
        {
            timer = new SimpleTimer(Context.Timeout);
        }

        public override void OnStateEnter()
        {
            SendRequest();
        }

        public override void OnTimer()
        {
            //Re-send the write request
            if (timer.IsTimeout())
            {
                TftpTrace.Trace("Timeout. Resending write request.", Context);
                SendRequest();
            }
        }

        private void SendRequest()
        {
            WriteRequest request = new WriteRequest(Context.Filename, Context.TransferMode, Context.Options);
            Context.GetConnection().Send(request);
            timer.Restart();
        }

        public override void OnCommand(ITftpCommand command, System.Net.EndPoint endpoint)
        {
            if (command is OptionAcknowledgement)
            {
                OptionAcknowledgement ackCommand = (OptionAcknowledgement)command;
                TransferOptionHandlers.HandleAcceptedOptions(Context, ackCommand.Options);
            }

            if (command is OptionAcknowledgement || (command is Acknowledgement && (command as Acknowledgement).BlockNumber == 0))
            {
                //Switch to the endpoint that we received from the server
                Context.GetConnection().RemoteEndpoint = endpoint;

                //Start sending packets
                Context.SetState(new Sending(Context));
            }
            else
            if (command is Error)
            {
                //The server denied our request
                Error error = (Error)command;
                Context.SetState(new ReceivedError(Context, error));
            }
            else
                base.OnCommand(command, endpoint);
        }

        public override void OnCancel(TftpErrorPacket reason)
        {
            Context.SetState(new CancelledByUser(Context, reason));
        }
    }
}
