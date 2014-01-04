using System;
using System.Globalization;
namespace ProtoBuffer
{
    /// <summary>
    /// 提供float 的序列化和反序列化
    /// <example>
    /// <para>Bit32 bit32 = 1.0f;</para>
    /// <para>float value = bit32;</para>
    /// </example>
    /// </summary>
    sealed class Bit32 : ProtoBufferValue
    {
        private Bit32()
        {
            
        }
        public Bit32(byte[] buffer, int offset)
        {
            if (buffer == null)
            {
                throw new ProtoBufferException("buffer = null");
            }
            if (offset + 4 > buffer.Length)
            {
                throw new ProtoBufferException("buffer 的长度不够(至少是offer + 4)");
            }
            Bytes = new byte[4];
            Array.Copy(buffer,offset,Bytes,0,4);
            Value = BitConverter.ToSingle(Bytes, 0);
            
        }
        public static implicit operator Bit32(float value)
        {
            return new Bit32(){Value = value,Bytes = BitConverter.GetBytes(value)};
        }
        public static implicit operator float(Bit32 value)
        {
            return (float)value.Value;
        }
        public override string ToString()
        {
            float value = (float) Value;
            return value.ToString(CultureInfo.InvariantCulture);
        }
    }
}
