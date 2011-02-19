using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Tftp.Net
{
    class TftpCommandParser
    {
        public ITftpCommand Parse(byte[] message)
        {
            try
            {
                return ParseInternal(message);
            }
            catch (TftpParserException e)
            {
                //Rethrow
                throw e;
            }
            catch (Exception e2)
            {
                throw new TftpParserException(e2);
            }
        }

        private ITftpCommand ParseInternal(byte[] message)
        {
            using (TftpStreamReader reader = new TftpStreamReader(new MemoryStream(message)))
            {
                ushort opcode = reader.ReadUInt16();
                switch (opcode)
                {
                    case ReadRequest.OpCode:
                        return ParseReadRequest(reader);

                    case WriteRequest.OpCode:
                        return ParseWriteRequest(reader);

                    case Data.OpCode:
                        return ParseData(reader);

                    case Acknowledgement.OpCode:
                        return ParseAcknowledgement(reader);

                    case Error.OpCode:
                        return ParseError(reader);

                    default:
                        throw new TftpParserException("Invalid opcode");
                }
            }
        }

        private Error ParseError(TftpStreamReader reader)
        {
            ushort errorCode = reader.ReadUInt16();
            String message = ParseNullTerminatedString(reader);
            return new Error(errorCode, message);
        }

        private Acknowledgement ParseAcknowledgement(TftpStreamReader reader)
        {
            ushort blockNumber = reader.ReadUInt16();
            return new Acknowledgement(blockNumber);
        }

        private Data ParseData(TftpStreamReader reader)
        {
            ushort blockNumber = reader.ReadUInt16();
            byte[] data = reader.ReadBytes(10000);
            return new Data(blockNumber, data);
        }

        private WriteRequest ParseWriteRequest(TftpStreamReader reader)
        {
            String filename = ParseNullTerminatedString(reader);
            TftpModeType mode = ParseModeType(ParseNullTerminatedString(reader));
            return new WriteRequest(filename, mode);
        }

        private ReadRequest ParseReadRequest(TftpStreamReader reader)
        {
            String filename = ParseNullTerminatedString(reader);
            TftpModeType mode = ParseModeType(ParseNullTerminatedString(reader));
            return new ReadRequest(filename, mode);
        }

        private String ParseNullTerminatedString(TftpStreamReader reader)
        {
            byte b;
            StringBuilder str = new StringBuilder();
            while ((b = reader.ReadByte()) != 0)
            {
                str.Append((char)b);
            }

            return str.ToString();
        }

        private TftpModeType ParseModeType(String mode)
        {
            mode = mode.ToLowerInvariant();

            if (mode == "netascii")
                return TftpModeType.netascii;

            if (mode == "mail")
                return TftpModeType.mail;

            if (mode == "octet")
                return TftpModeType.octet;

            throw new TftpParserException("Unknown mode type: " + mode);
        }
    }

    class TftpParserException : Exception
    {
        public TftpParserException(String message)
            : base(message) { }

        public TftpParserException(Exception e)
            : base("Error while parsing message.", e) { }
    }
}
