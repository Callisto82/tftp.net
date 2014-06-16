using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tftp.Net.Transfer;

namespace Tftp.Net.Transfer
{
    class TftpTransferOptions
    {
        public bool IsBlockSizeOptionActive;
        public int BlockSize;

        public bool IsTimeoutOptionActive;
        public TimeSpan Timeout;

        public bool IsTransferSizeOptionActive;
        public int TransferSize = 0;

        public TftpTransferOptions()
        {
            SetDefaultValues();
            IsBlockSizeOptionActive = IsTimeoutOptionActive = IsTransferSizeOptionActive = true;
        }

        private void SetDefaultValues()
        {
            BlockSize = 512;
            Timeout = TimeSpan.FromSeconds(5);
        }

        public void SetActiveOptions(IEnumerable<TransferOption> options)
        {
            SetDefaultValues();
            IsBlockSizeOptionActive = IsTimeoutOptionActive = IsTransferSizeOptionActive = false;

            foreach (TransferOption option in options)
            {
                Parse(option);
            }
        }

        private void Parse(TransferOption option)
        {
            switch (option.Name)
            {
                case "blksize":
                    IsBlockSizeOptionActive = ParseBlockSizeOption(option.Value);
                    break;

                case "timeout":
                    IsTimeoutOptionActive = ParseTimeoutOption(option.Value);
                    break;

                case "tsize":
                    IsTransferSizeOptionActive = ParseTransferSizeOption(option.Value);
                    break;
            }
        }

        public List<TransferOption> GetActiveOptions()
        {
            List<TransferOption> result = new List<TransferOption>();
            if (IsBlockSizeOptionActive)
                result.Add(new TransferOption("blksize", BlockSize.ToString()));

            if (IsTimeoutOptionActive)
                result.Add(new TransferOption("timeout", Timeout.Seconds.ToString()));

            if (IsTransferSizeOptionActive)
                result.Add(new TransferOption("tsize", TransferSize.ToString()));

            return result;
        }

        private bool ParseTransferSizeOption(string value)
        {
            return int.TryParse(value, out TransferSize) && TransferSize >= 0;
        }

        private bool ParseTimeoutOption(string value)
        {
            int timeout;
            if (!int.TryParse(value, out timeout))
                return false;

            //Only accept timeouts in the range [1, 255]
            if (timeout < 1 || timeout > 255)
                return false;

            Timeout = new TimeSpan(0, 0, timeout);
            return true;
        }

        private bool ParseBlockSizeOption(string value)
        {
            int blockSize;
            if (!int.TryParse(value, out blockSize))
                return false;

            //Only accept block sizes in the range [8, 65464]
            if (blockSize < 8 || blockSize > 65464)
                return false;

            BlockSize = blockSize;
            return true;
        }
    }
}
