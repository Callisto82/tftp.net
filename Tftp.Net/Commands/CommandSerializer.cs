using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tftp.Net.Commands
{
    /// <summary>
    /// Serializes an ITftpCommand into a stream of bytes.
    /// </summary>
    class CommandSerializer
    {
        /// <summary>
        /// Call this method to serialize the given <code>command</code> using the given <code>writer</code>.
        /// </summary>
        public static void Serialize(ITftpCommand command, TftpStreamWriter writer)
        {
            CommandComposerVisitor visitor = new CommandComposerVisitor(writer);
            command.Visit(visitor);
        }

        private class CommandComposerVisitor : ITftpCommandVisitor
        {
            private readonly TftpStreamWriter writer;

            public CommandComposerVisitor(TftpStreamWriter writer)
            {
                if (writer == null)
                    throw new ArgumentNullException("writer");

                this.writer = writer;
            }

            private void OnReadOrWriteRequest(ReadOrWriteRequest command)
            {
                writer.WriteBytes(Encoding.ASCII.GetBytes(command.Filename));
                writer.WriteByte(0);
                writer.WriteBytes(Encoding.ASCII.GetBytes(command.Mode.ToString()));
                writer.WriteByte(0);

                if (command.Options != null)
                {
                    foreach (ITftpTransferOption option in command.Options)
                    {
                        writer.WriteBytes(Encoding.ASCII.GetBytes(option.Name));
                        writer.WriteByte(0);
                        writer.WriteBytes(Encoding.ASCII.GetBytes(option.Value));
                        writer.WriteByte(0);
                    }
                }
            }

            public void OnReadRequest(ReadRequest command)
            {
                writer.WriteUInt16(ReadRequest.OpCode);
                OnReadOrWriteRequest(command);
            }

            public void OnWriteRequest(WriteRequest command)
            {
                writer.WriteUInt16(WriteRequest.OpCode);
                OnReadOrWriteRequest(command);
            }

            public void OnData(Data command)
            {
                writer.WriteUInt16(Data.OpCode);
                writer.WriteUInt16(command.BlockNumber);
                writer.WriteBytes(command.Bytes);
            }

            public void OnAcknowledgement(Acknowledgement command)
            {
                writer.WriteUInt16(Acknowledgement.OpCode);
                writer.WriteUInt16(command.BlockNumber);
            }

            public void OnError(Error command)
            {
                writer.WriteUInt16(Error.OpCode);
                writer.WriteUInt16(command.ErrorCode);
                writer.WriteBytes(Encoding.ASCII.GetBytes(command.Message));
                writer.WriteByte(0);
            }

            public void OnOptionAcknowledgement(OptionAcknowledgement command)
            {
                writer.WriteUInt16(OptionAcknowledgement.OpCode);

                foreach (ITftpTransferOption option in command.Options)
                {
                    writer.WriteBytes(Encoding.ASCII.GetBytes(option.Name));
                    writer.WriteByte(0);
                    writer.WriteBytes(Encoding.ASCII.GetBytes(option.Value));
                    writer.WriteByte(0);
                }
            }
        }
    }
}
