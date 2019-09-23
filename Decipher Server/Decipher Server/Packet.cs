using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decipher_Server
{
    internal sealed class Packet
    {
        internal int Position
        {
            get;
            set;
        }

        internal byte OpCode
        {
            get;
        }

        internal List<byte> Data
        {
            get;
        }

        internal int Length => Data.Count;

        internal Packet(byte[] buffer)
        {
            ushort num = (ushort)((buffer[0] << 8) + buffer[1]);
            OpCode = buffer[2];
            Position = 0;
            Data = new List<byte>(num - 3);
            Data.AddRange(buffer.Skip(3));
        }

        internal Packet(byte opCode)
        {
            Position = 0;
            OpCode = opCode;
            Data = new List<byte>();
        }

        internal byte[] ReadBytes(int length)
        {
            if (Position + length > Data.Count)
            {
                throw new EndOfStreamException();
            }
            byte[] array = new byte[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = Data[i + Position];
            }
            Position += length;
            return array;
        }

        internal byte ReadByte()
        {
            if (Position + 1 > Length)
            {
                throw new EndOfStreamException();
            }
            return Data[Position++];
        }

        internal sbyte ReadSByte()
        {
            if (Position + 1 > Length)
            {
                throw new EndOfStreamException();
            }
            return (sbyte)Data[Position++];
        }

        internal bool ReadBoolean()
        {
            if (Position + 1 > Length)
            {
                throw new EndOfStreamException();
            }
            return Data[Position++] != 0;
        }

        internal short ReadInt16()
        {
            if (Position + 2 > Length)
            {
                throw new EndOfStreamException();
            }
            return (short)((Data[Position++] << 8) | Data[Position++]);
        }

        internal ushort ReadUInt16()
        {
            if (Position + 2 > Length)
            {
                throw new EndOfStreamException();
            }
            return (ushort)((Data[Position++] << 8) | Data[Position++]);
        }

        internal int ReadInt32()
        {
            if (Position + 4 > Length)
            {
                throw new EndOfStreamException();
            }
            return (Data[Position++] << 24) | (Data[Position++] << 16) | (Data[Position++] << 8) | Data[Position++];
        }

        internal uint ReadUInt32()
        {
            if (Position + 2 > Length)
            {
                throw new EndOfStreamException();
            }
            return (uint)((Data[Position++] << 24) | (Data[Position++] << 16) | (Data[Position++] << 8) | Data[Position++]);
        }

        internal string ReadString()
        {
            return Encoding.GetEncoding(949).GetString(ReadBytes(Length - Position));
        }

        internal string ReadString8()
        {
            if (Position + 1 > Length)
            {
                throw new EndOfStreamException();
            }
            int num = ReadByte();
            if (Position + num > Length)
            {
                Position--;
                throw new EndOfStreamException();
            }
            return Encoding.GetEncoding(949).GetString(ReadBytes(num));
        }

        internal string ReadString16()
        {
            if (Position + 2 > Length)
            {
                throw new EndOfStreamException();
            }
            int num = ReadUInt16();
            if (Position + num > Length)
            {
                Position -= 2;
                throw new EndOfStreamException();
            }
            return Encoding.GetEncoding(949).GetString(ReadBytes(num));
        }

        internal byte[] ReadData()
        {
            if (Position + 1 > Length)
            {
                throw new EndOfStreamException();
            }
            List<byte> list = new List<byte>();
            for (byte b = ReadByte(); b != 0; b = ReadByte())
            {
                list.Add(b);
            }
            return list.ToArray();
        }

        internal byte[] ReadData8()
        {
            if (Position + 1 > Length)
            {
                throw new EndOfStreamException();
            }
            int num = ReadByte();
            if (Position + num > Length)
            {
                Position--;
                throw new EndOfStreamException();
            }
            return ReadBytes(num);
        }

        internal byte[] ReadData16()
        {
            if (Position + 2 > Length)
            {
                throw new EndOfStreamException();
            }
            int num = ReadUInt16();
            if (Position + num > Length)
            {
                Position -= 2;
                throw new EndOfStreamException();
            }
            return ReadBytes(num);
        }

        internal void Write(params byte[] buffer)
        {
            Data.Write(Position, buffer, 0, buffer.Length);
            Position += buffer.Length;
        }

        internal void WriteByte(byte value)
        {
            Write(value);
        }

        internal void WriteSByte(sbyte value)
        {
            Write((byte)value);
        }

        internal void WriteBoolean(bool value)
        {
            Write((byte)(value ? 1 : 0));
        }

        internal void WriteInt16(short value)
        {
            Write((byte)(value >> 8), (byte)value);
        }

        internal void WriteUInt16(ushort value)
        {
            Write((byte)(value >> 8), (byte)value);
        }

        internal void WriteInt32(int value)
        {
            Write((byte)(value >> 24), (byte)(value >> 16), (byte)(value >> 8), (byte)value);
        }

        internal void WriteUInt32(uint value)
        {
            Write((byte)(value >> 24), (byte)(value >> 16), (byte)(value >> 8), (byte)value);
        }

        internal void WriteString(string value, bool terminate = false)
        {
            Write(Encoding.GetEncoding(949).GetBytes(value));
            if (terminate)
            {
                WriteByte(10);
            }
        }

        internal void WriteString8(string value)
        {
            byte[] bytes = Encoding.GetEncoding(949).GetBytes(value);
            if (bytes.Length > 255)
            {
                throw new ArgumentOutOfRangeException("value", value, $"String must be {byte.MaxValue} chars or less");
            }
            WriteByte((byte)bytes.Length);
            Write(bytes);
        }

        internal void WriteString16(string value)
        {
            byte[] bytes = Encoding.GetEncoding(949).GetBytes(value);
            if (bytes.Length > 65535)
            {
                throw new ArgumentOutOfRangeException("value", value, $"String must be {ushort.MaxValue} chars or less");
            }
            WriteUInt16((ushort)bytes.Length);
            Write(bytes);
        }

        internal void WriteData(byte[] value, bool terminate = false)
        {
            Write(value);
            if (terminate)
            {
                WriteByte(0);
            }
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
            byte[] array = new byte[Length + 3];
            array[0] = (byte)(Length + 3 >> 8);
            array[1] = (byte)(Length + 3);
            array[2] = OpCode;
            Data.CopyTo(0, array, 3, Length);
            return array;
        }

        internal string GetHexString()
        {
            byte[] array = new byte[Length + 1];
            array[0] = OpCode;
            Data.CopyTo(0, array, 1, Length);
            return BitConverter.ToString(array).Replace('-', ' ');
        }

        internal string GetAsciiString(bool replaceNewline = true)
        {
            string text = string.Empty;
            byte[] array = new byte[Length + 1];
            array[0] = OpCode;
            Data.CopyTo(0, array, 1, Length);
            foreach (byte b in array)
            {
                text += ((char)(((b == 10 || b == 13) && !replaceNewline) ? b : ((b < 32 || b > 126) ? 46 : b))).ToString();
            }
            return text;
        }
    }
}
