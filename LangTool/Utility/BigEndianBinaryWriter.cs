using System;
using System.IO;
using System.Text;

namespace LangTool.Utility
{
    public class BigEndianBinaryWriter : BinaryWriter
    {
        public BigEndianBinaryWriter(Stream output) : base(output)
        {
            
        }

        public BigEndianBinaryWriter(Stream output, Encoding encoding)
            : base(output, encoding)
        {
            
        }

        public BigEndianBinaryWriter(Stream output, Encoding encoding, bool leaveOpen)
            : base(output, encoding, leaveOpen)
        {
            
        }

        private byte[] Reverse(byte[] b)
        {
            Array.Reverse(b);
            return b;
        }

        public override void Write(float value)
        {
            Write(Reverse(BitConverter.GetBytes(value)));
        }

        public override void Write(ulong value)
        {
            Write(Reverse(BitConverter.GetBytes(value)));
        }

        public override void Write(long value)
        {
            Write(Reverse(BitConverter.GetBytes(value)));
        }

        public override void Write(uint value)
        {
            Write(Reverse(BitConverter.GetBytes(value)));
        }

        public override void Write(int value)
        {
            Write(Reverse(BitConverter.GetBytes(value)));
        }

        public override void Write(ushort value)
        {
            Write(Reverse(BitConverter.GetBytes(value)));
        }

        public override void Write(short value)
        {
            Write(Reverse(BitConverter.GetBytes(value)));
        }

        public override void Write(decimal value)
        {
            int[] bits = decimal.GetBits(value);
            byte[] bytes = new byte[4 * sizeof(int)];
            Buffer.BlockCopy(bits, 0, bytes, 0, bytes.Length);
            Write(Reverse(bytes));
        }

        public override void Write(double value)
        {
            Write(Reverse(BitConverter.GetBytes(value)));
        }
    }
}