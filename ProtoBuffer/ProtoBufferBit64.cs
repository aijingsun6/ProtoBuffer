using System;
using System.Globalization;

namespace ProtoBuffer
{
    /// <summary>
    /// 提供double的序列化和反序列化
    /// </summary>
    sealed class Bit64:ProtoBufferValue
    {
        private Bit64()
        {
            
        }
        public Bit64(byte[] buffer, int offset)
        {
            if (buffer == null)
            {
                throw new ProtoBufferException("buffer = null");
            }

            if (offset + 8 > buffer.Length)
            {
                throw new ProtoBufferException("buffer的长度不够（至少是offset + 8）");
            }
            Bytes = new byte[8];
            Array.Copy(buffer,offset,Bytes,0,8);
            Value = BitConverter.ToDouble(Bytes, 0);
        }
        public static implicit operator Bit64(double value)
        {
            return new Bit64(){Value = value,Bytes = BitConverter.GetBytes(value)};
        }
        public static implicit operator double(Bit64 value)
        {
            return (double) value.Value;
        }
        public override string ToString()
        {
            double result = (double) Value;
            return result.ToString(CultureInfo.InvariantCulture);
        }
    }
}
