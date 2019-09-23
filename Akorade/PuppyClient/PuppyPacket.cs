using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Doumi.PuppyServer
{
    internal sealed class PuppyPacket
    {
        internal int Position { get; set; }
        internal byte OpCode { get; }
        internal List<byte> Data { get; }

        internal int Length => Data.Count;
        //length opcode data
        //00 00 , 00 , 00 00 00 00 00 00 00 00

        internal PuppyPacket(byte[] buffer)
        {
            ushort length = (ushort)((buffer[0] << 8) + buffer[1]);
            OpCode = buffer[2];
            Position = 0;

            Data = new List<byte>(length - 3);
            Data.AddRange(buffer.Skip(3));
        }

        internal PuppyPacket(byte opCode)
        {
            Position = 0;
            OpCode = opCode;
            Data = new List<byte>();
        }

        internal byte[] ReadBytes(int length)
        {
            if (Position + length > Data.Count)
                throw new EndOfStreamException();

            byte[] readData = new byte[length];
            for (int i = 0; i < length; i++)
                readData[i] = Data[i + Position];

            Position += length;
            return readData;
        }
        internal byte ReadByte()
        {
            if (Position + 1 > Length)
                throw new EndOfStreamException();

            return Data[Position++];
        }
        internal sbyte ReadSByte()
        {
            if (Position + 1 > Length)
                throw new EndOfStreamException();

            return (sbyte)Data[Position++];
        }
        internal bool ReadBoolean()
        {
            if (Position + 1 > Length)
                throw new EndOfStreamException();

            return Data[Position++] != 0;
        }
        internal short ReadInt16()
        {
            if (Position + 2 > Length)
                throw new EndOfStreamException();

            return (short)((Data[Position++] << 8) | Data[Position++]);
        }
        internal ushort ReadUInt16()
        {
            if (Position + 2 > Length)
                throw new EndOfStreamException();

            return (ushort)((Data[Position++] << 8) | Data[Position++]);
        }
        internal int ReadInt32()
        {
            if (Position + 4 > Length)
                throw new EndOfStreamException();

            return (int)((Data[Position++] << 24) | (Data[Position++] << 16) | (Data[Position++] << 8) | Data[Position++]);
        }
        internal uint ReadUInt32()
        {
            if (Position + 2 > Length)
                throw new EndOfStreamException();

            return (uint)((Data[Position++] << 24) | (Data[Position++] << 16) | (Data[Position++] << 8) | Data[Position++]);
        }
        internal string ReadString() => Encoding.GetEncoding(949).GetString(ReadBytes(Length - Position));

        internal string ReadString8()
        {
            if (Position + 1 > Length)
                throw new EndOfStreamException();

            int lengthPrefix = ReadByte();
            if (Position + lengthPrefix > Length)
            {
                Position--;
                throw new EndOfStreamException();
            }

            return Encoding.GetEncoding(949).GetString(ReadBytes(lengthPrefix));
        }
        internal string ReadString16()
        {
            if (Position + 2 > Length)
                throw new EndOfStreamException();

            int lengthPrefix = ReadUInt16();
            if (Position + lengthPrefix > Length)
            {
                Position -= 2;
                throw new EndOfStreamException();
            }

            return Encoding.GetEncoding(949).GetString(ReadBytes(lengthPrefix));
        }

        internal byte[] ReadData()
        {
            if (Position + 1 > Length)
                throw new EndOfStreamException();

            var data = new List<byte>();
            byte b = ReadByte();

            while (b != 0)
            {
                data.Add(b);
                b = ReadByte();
            }

            return data.ToArray();
        }
        internal byte[] ReadData8()
        {
            if (Position + 1 > Length)
                throw new EndOfStreamException();

            int lengthPrefix = ReadByte();
            if (Position + lengthPrefix > Length)
            {
                Position--;
                throw new EndOfStreamException();
            }

            return ReadBytes(lengthPrefix);
        }
        internal byte[] ReadData16()
        {
            if (Position + 2 > Length)
                throw new EndOfStreamException();

            int lengthPrefix = ReadUInt16();
            if (Position + lengthPrefix > Length)
            {
                Position -= 2;
                throw new EndOfStreamException();
            }
            return ReadBytes(lengthPrefix);
        }


        internal void Write(params byte[] buffer)
        {
            Data.Write(Position, buffer, 0, buffer.Length);
            Position += buffer.Length;
        }

        internal void WriteByte(byte value) => Write(value);
        internal void WriteSByte(sbyte value) => Write((byte)value);
        internal void WriteBoolean(bool value) => Write((byte)(value ? 1 : 0));
        internal void WriteInt16(short value) => Write((byte)(value >> 8), (byte)value);
        internal void WriteUInt16(ushort value) => Write((byte)(value >> 8), (byte)value);
        internal void WriteInt32(int value) => Write((byte)(value >> 24), (byte)(value >> 16), (byte)(value >> 8), (byte)value);
        internal void WriteUInt32(uint value) => Write((byte)(value >> 24), (byte)(value >> 16), (byte)(value >> 8), (byte)value);

        internal void WriteString(string value, bool terminate = false)
        {
            Write(Encoding.GetEncoding(949).GetBytes(value));
            if (terminate)
                WriteByte(10);
        }
        internal void WriteString8(string value)
        {
            byte[] writeData = Encoding.GetEncoding(949).GetBytes(value);
            if (writeData.Length > byte.MaxValue)
                throw new ArgumentOutOfRangeException("value", value, $@"String must be {byte.MaxValue} chars or less");

            WriteByte((byte)writeData.Length);
            Write(writeData);
        }
        internal void WriteString16(string value)
        {
            byte[] writeData = Encoding.GetEncoding(949).GetBytes(value);
            if (writeData.Length > ushort.MaxValue)
                throw new ArgumentOutOfRangeException("value", value, $@"String must be {ushort.MaxValue} chars or less");

            WriteUInt16((ushort)writeData.Length);
            Write(writeData);
        }
        internal void WriteData(byte[] value, bool terminate = false)
        {
            Write(value);
            if (terminate)
                WriteByte(0);
        }
        internal void WriteData8(byte[] value)
        {
            WriteByte((byte)value.Length);
            WriteData(value);
        }

        internal void WriteData16(byte[] value)
        {
            WriteUInt16((ushort)value.Length);
            WriteData(value);
        }

        internal byte[] ToArray()
        {
            byte[] data = new byte[Length + 3];
            data[0] = (byte)((Length + 3) >> 8);
            data[1] = (byte)(Length + 3);
            data[2] = OpCode;
            Data.CopyTo(0, data, 3, Length);
            return data;
        }
        internal string GetHexString()
        {
            int resultLength = Length + 1;
            byte[] resultData = new byte[resultLength];
            resultData[0] = OpCode;
            Data.CopyTo(0, resultData, 1, Length);
            return BitConverter.ToString(resultData).Replace('-', ' ');
        }
        internal string GetAsciiString(bool replaceNewline = true)
        {
            string resultStr = string.Empty;
            byte[] buffer = new byte[Length + 1];
            buffer[0] = OpCode;
            Data.CopyTo(0, buffer, 1, Length);
            for (int i = 0; i < buffer.Length; i++)
            {
                byte charCode = buffer[i];
                resultStr += ((charCode == 10 || charCode == 13) && !replaceNewline) ? (char)charCode
                    : (charCode < 32 || (charCode > 126)) ? '.'
                    : (char)charCode;
            }
            return resultStr;
        }
    }
}
