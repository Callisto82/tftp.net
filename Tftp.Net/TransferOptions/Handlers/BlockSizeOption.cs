using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tftp.Net.Transfer;

namespace Tftp.Net.TransferOptions.Handlers
{
    /// <summary>
    /// Implements RFC 2348 (TFTP Blocksize Option)
    /// </summary>
    class BlockSizeOption : ITftpTransferOptionHandler
    {
        public bool ApplyOption(ITftpTransfer transfer, ITftpTransferOption option)
        {
            if (option.Name != "blksize" || !(transfer is TftpTransfer))
                return false;

            int blockSize;
            if (!int.TryParse(option.Value, out blockSize))
                return false;

            //Only accept block sizes in the range [8, 65464]
            if (blockSize < 8 || blockSize > 65464)
                return false;

            ((TftpTransfer)transfer).BlockSize = blockSize;
            return true;
        }
    }
}
