using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tftp.Net.Channel;
using System.Net;

namespace Tftp.Net.Transfer.States
{
    class StateThatExpectsMessagesFromDefaultEndPoint : BaseState, ITftpCommandVisitor
    {
        public StateThatExpectsMessagesFromDefaultEndPoint(TftpTransfer context)
            : base(context) { }

        public override void OnCommand(ITftpCommand command, EndPoint endpoint)
        {
            command.Visit(this);
        }

        public virtual void OnReadRequest(ReadRequest command)
        {
            throw new NotImplementedException();
        }

        public virtual void OnWriteRequest(WriteRequest command)
        {
            throw new NotImplementedException();
        }

        public virtual void OnData(Data command)
        {
            throw new NotImplementedException();
        }

        public virtual void OnAcknowledgement(Acknowledgement command)
        {
            throw new NotImplementedException();
        }

        public virtual void OnError(Error command)
        {
            throw new NotImplementedException();
        }
    }
}
