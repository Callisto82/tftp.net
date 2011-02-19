using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Tftp.Net
{
    interface ITftpCommand
    {
        void Visit(ITftpCommandVisitor visitor);
        void Write(TftpStreamWriter writer);
    }

    enum TftpModeType
    {
        netascii,
        octet,
        mail
    }

    interface ITftpCommandVisitor
    {
        void OnReadRequest(ReadRequest command);
        void OnWriteRequest(WriteRequest command);
        void OnData(Data command);
        void OnAcknowledgement(Acknowledgement command);
        void OnError(Error command);
    }

    abstract class ReadOrWriteRequest
    {
        private readonly ushort opCode;

        public String Filename { get; private set; }
        public TftpModeType Mode { get; private set; }

        protected ReadOrWriteRequest(ushort opCode, String filename, TftpModeType mode)
        {
            this.opCode = opCode;
            this.Filename = filename;
            this.Mode = mode;
        }

        public void Write(TftpStreamWriter writer)
        {
            writer.WriteUInt16(opCode);
            writer.WriteBytes(Encoding.ASCII.GetBytes(Filename));
            writer.WriteByte(0);
            writer.WriteBytes(Encoding.ASCII.GetBytes(Mode.ToString()));
            writer.WriteByte(0);
        }
    }

    class ReadRequest : ReadOrWriteRequest, ITftpCommand
    {
        public const ushort OpCode = 1;

        public ReadRequest(String filename, TftpModeType mode)
            : base(OpCode, filename, mode) { }

        public void Visit(ITftpCommandVisitor visitor)
        {
            visitor.OnReadRequest(this);
        }
    }

    class WriteRequest : ReadOrWriteRequest, ITftpCommand
    {
        public const ushort OpCode = 2;

        public WriteRequest(String filename, TftpModeType mode)
            : base(OpCode, filename, mode) { }

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
}
