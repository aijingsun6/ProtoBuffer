using System;
using System.Text;
namespace ProtoBuffer
{
    /// <summary>
    /// 提供string 和 byte[] 的序列化和反序列化策略
    /// </summary>
    sealed class LenghDelimited : ProtoBufferValue
    {

        private LenghDelimited()
        {
            
        }
        public LenghDelimited(byte[] buffer, int offset, int size)
        {
            if (buffer == null)
            {
                throw new ProtoBufferException("buffer = null");
            }

            if (offset + size > buffer.Length)
            {
                throw new ProtoBufferException("buffer 的长度不够");
            }
            Value = new byte[size];
            Array.Copy(buffer,offset,Value as byte[],0,size);
            Bytes = new byte[size];
            Array.Copy(buffer,offset,Bytes,0,size);
        }
        public static implicit operator LenghDelimited(string value)
        {
            return new LenghDelimited(){Value = Encoding.UTF8.GetBytes(value),Bytes = Encoding.UTF8.GetBytes(value)};
        }
        public static implicit operator LenghDelimited(byte[] value)
        {
            return new LenghDelimited(){Value = value,Bytes = value};
        }


        public static implicit operator string(LenghDelimited value)
        {
            if (value.Value is byte[])
            {
                return Encoding.UTF8.GetString(value.Value as byte[]);
            }
            else
            {
                throw new ProtoBufferException("value 不是byte[]");
            }
        }
        public static implicit operator byte[](LenghDelimited value)
        {
            if (value.Value is byte[])
            {
                return value.Value as byte[];
            }
            else
            {
                throw new ProtoBufferException("value 不是byte[]");
            }
        }
        public override string ToString()
        {
            return string.Format("StringBytes,data length = {0}",Bytes.Length);
        }
    }
}
