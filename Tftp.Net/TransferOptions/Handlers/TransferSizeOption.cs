using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tftp.Net.Transfer;

namespace Tftp.Net.TransferOptions.Handlers
{
    /// <summary>
    /// Implements the transfer size option according to RFC 2349.
    /// </summary>
    class TransferSizeOption : ITftpTransferOptionHandler
    {
        public bool ApplyOption(ITftpTransfer transfer, ITftpTransferOption option)
        {
            if (option.Name != "tsize" || !(option is TransferOption) || !(transfer is TftpTransfer))
                return false;

            TransferOption tftpOption = (TransferOption)option;
            TftpTransfer tftpTransfer = (TftpTransfer)transfer;
            if (tftpTransfer.InputOutputStream == null)
                return false;

            try
            {
                tftpOption.Value = tftpTransfer.InputOutputStream.Length.ToString();
            }
            catch (Exception)
            {
                //Error while reading stream length
                return false;
            }
            
            return true;
        }
    }
}
