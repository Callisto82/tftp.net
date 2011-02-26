using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Tftp.Net.Transfer;

namespace Tftp.Net
{
    interface ITftpCommand
    {
        void Visit(ITftpCommandVisitor visitor);
        void Write(TftpStreamWriter writer);
    }

    interface ITftpCommandVisitor
    {
        void OnReadRequest(ReadRequest command);
        void OnWriteRequest(WriteRequest command);
        void OnData(Data command);
        void OnAcknowledgement(Acknowledgement command);
        void OnError(Error command);
        void OnOptionAcknowledgement(OptionAcknowledgement command);
    }

    abstract class ReadOrWriteRequest
    {
        private readonly ushort opCode;

        public String Filename { get; private set; }
        public TftpTransferMode Mode { get; private set; }
        public IEnumerable<TftpTransferOption> Options { get; private set; }

        protected ReadOrWriteRequest(ushort opCode, String filename, TftpTransferMode mode, IEnumerable<TftpTransferOption> options)
        {
            this.opCode = opCode;
            this.Filename = filename;
            this.Mode = mode;
            this.Options = options;
        }

        public void Write(TftpStreamWriter writer)
        {
            writer.WriteUInt16(opCode);
            writer.WriteBytes(Encoding.ASCII.GetBytes(Filename));
            writer.WriteByte(0);
            writer.WriteBytes(Encoding.ASCII.GetBytes(Mode.ToString()));
            writer.WriteByte(0);

            if (Options != null)
            {
                foreach (TftpTransferOption option in Options)
                {
                    writer.WriteBytes(Encoding.ASCII.GetBytes(option.Name));
                    writer.WriteByte(0);
                    writer.WriteBytes(Encoding.ASCII.GetBytes(option.Value));
                    writer.WriteByte(0);
                }
            }
        }
    }

    class ReadRequest : ReadOrWriteRequest, ITftpCommand
    {
        public const ushort OpCode = 1;

        public ReadRequest(String filename, TftpTransferMode mode, IEnumerable<TftpTransferOption> options)
            : base(OpCode, filename, mode, options) { }

        public void Visit(ITftpCommandVisitor visitor)
        {
            visitor.OnReadRequest(this);
        }
    }

    class WriteRequest : ReadOrWriteRequest, ITftpCommand
    {
        public const ushort OpCode = 2;

        public WriteRequest(String filename, TftpTransferMode mode, IEnumerable<TftpTransferOption> options)
            : base(OpCode, filename, mode, options) { }

        public void Visit(ITftpCommandVisitor visitor)
        {
            visitor.OnWriteRequest(this);
        }
    }

    class Data : ITftpCommand
    {
        public const ushort OpCode = 3;

        public ushort BlockNumber { get; private set; }
        public byte[] Bytes { get; private set; }

        public Data(ushort blockNumber, byte[] data)
        {
            this.BlockNumber = blockNumber;
            this.Bytes = data;
        }

        public void Visit(ITftpCommandVisitor visitor)
        {
            visitor.OnData(this);
        }

        public void Write(TftpStreamWriter writer)
        {
            writer.WriteUInt16(OpCode);
            writer.WriteUInt16(BlockNumber);
            writer.WriteBytes(Bytes);
        }
    }

    class Acknowledgement : ITftpCommand
    {
        public const ushort OpCode = 4;

        public ushort BlockNumber { get; private set; }

        public Acknowledgement(ushort blockNumber)
        {
            this.BlockNumber = blockNumber;
        }

        public void Visit(ITftpCommandVisitor visitor)
        {
            visitor.OnAcknowledgement(this);
        }

        public void Write(TftpStreamWriter writer)
        {
            writer.WriteUInt16(OpCode);
            writer.WriteUInt16(BlockNumber);
        }
    }

    class Error : ITftpCommand
    {
        public const ushort OpCode = 5;

        public ushort ErrorCode { get; private set; }
        public String Message { get; private set; }

        public Error(ushort errorCode, String message)
        {
            this.ErrorCode = errorCode;
            this.Message = message;
        }

        public void Visit(ITftpCommandVisitor visitor)
        {
            visitor.OnError(this);
        }

        public void Write(TftpStreamWriter writer)
        {
            writer.WriteUInt16(OpCode);
            writer.WriteUInt16(ErrorCode);
            writer.WriteBytes(Encoding.ASCII.GetBytes(Message));
            writer.WriteByte(0);
        }
    }

    class OptionAcknowledgement : ITftpCommand
    {
        public const ushort OpCode = 6;
        public IEnumerable<TftpTransferOption> Options { get; private set; }

        public OptionAcknowledgement(IEnumerable<TftpTransferOption> options)
        {
            this.Options = options;
        }

        public void Visit(ITftpCommandVisitor visitor)
        {
            visitor.OnOptionAcknowledgement(this);
        }

        public void Write(TftpStreamWriter writer)
        {
            writer.WriteUInt16(OpCode);

            foreach (TftpTransferOption option in Options)
            {
                writer.WriteBytes(Encoding.ASCII.GetBytes(option.Name));
                writer.WriteByte(0);
                writer.WriteBytes(Encoding.ASCII.GetBytes(option.Value));
                writer.WriteByte(0);
            }
        }
    }
}
