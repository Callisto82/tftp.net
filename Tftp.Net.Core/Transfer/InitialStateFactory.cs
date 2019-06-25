using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tftp.Net.Transfer.States;

namespace Tftp.Net.Transfer
{
    static class InitialStateFactory
    {
        /*public enum TransferInitiatedBy
        {
            LocalSystem,
            RemoteSystem
        }

        public enum TransferMode
        {
            Read,
            Write
        }

        public static ITftpState GetInitialState(TransferInitiatedBy direction, TransferMode mode)
        {
            if (direction == TransferInitiatedBy.RemoteSystem && mode == TransferMode.Read)
            {
                return new StartIncomingRead(this);
            }

            if (direction == TransferInitiatedBy.RemoteSystem && mode == TransferMode.Write)
            {
                return new StartIncomingWrite(this);
            }

            if (direction == TransferInitiatedBy.LocalSystem && mode == TransferMode.Read)
            {
                return new StartOutgoingRead(this);
            }

            if (direction == TransferInitiatedBy.LocalSystem && mode == TransferMode.Write)
            {

            }

            return null;
        }*/

    }
}
