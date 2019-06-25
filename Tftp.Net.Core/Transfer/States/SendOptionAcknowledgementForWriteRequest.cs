using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tftp.Net.Transfer.States
{
    class SendOptionAcknowledgementForWriteRequest : SendOptionAcknowledgementBase
    {
        public override void OnData(Data command)
        {
            if (command.BlockNumber == 1)
            {
                //The client confirmed the options, so let's start receiving
                ITransferState nextState = new Receiving();
                Context.SetState(nextState);
                nextState.OnCommand(command, Context.GetConnection().RemoteEndpoint);
            }
        }
    }
}
