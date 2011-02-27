using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tftp.Net.TransferOptions
{
    /// <summary>
    /// Collects all instances of ITftpTransferOptionHandler.
    /// </summary>
    static class TransferOptionHandlers
    {
        private static readonly LinkedList<ITftpTransferOptionHandler> all = new LinkedList<ITftpTransferOptionHandler>();

        public static IEnumerable<ITftpTransferOptionHandler> All { get { return all; } }

        /// <summary>
        /// Register default transfer option handlers
        /// </summary>
        static TransferOptionHandlers()
        {
            Add(new BlockSizeOption());
        }

        /// <summary>
        /// Call this method to register a custom transfer option handler.
        /// </summary>
        public static void Add(ITftpTransferOptionHandler handler)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            all.AddLast(handler);
        }

        /// <summary>
        /// Call this method to remove a previously registers option handler.
        /// </summary>
        /// <param name="handler"></param>
        public static void Remove(ITftpTransferOptionHandler handler)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            all.Remove(handler);
        }

        /// <summary>
        /// This method is called with to dermine accepted options (for incoming transfers) or for all accepted options
        /// for outgoing transfers.
        /// </summary>
        internal static void HandleAcceptedOptions(ITftpTransfer transfer, IEnumerable<ITftpTransferOption> options)
        {
            foreach (TransferOption option in options)
            {
                bool wasAcknowledged = All.Any(x => x.Acknowledge(transfer, option));
                option.IsAcknowledged = wasAcknowledged;
            }
        }
    }
}
