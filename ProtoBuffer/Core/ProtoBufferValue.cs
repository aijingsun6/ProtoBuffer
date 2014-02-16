namespace ProtoBuffer
{
   
    /// <summary>
    /// ProtoBuffer 基本值类型
    /// <para>提供int long bool float double string byte[] 的序列化和反序列化</para>
    /// </summary>
    public abstract class ProtoBufferValue
    {
        /// <summary>
        /// 真实的值
        /// <para>可以是long,float,double,string,byte[]</para>
        /// <para>如果是bool, 则存储的是1或者0</para>
        /// <para>如果是int,存储的还是long</para>
        /// </summary>
        protected object Value { get; set; }

        /// <summary>
        /// 序列化后的byte[]
        /// </summary>
        public byte[] Bytes { get; protected set; }


        public static implicit operator bool(ProtoBufferValue value)
        {
            if (value is Varint)
            {
                bool result = value as Varint;
                return result;
            }
            else
            {
                throw new ProtoBufferException("is not varint");
            }
        }
        public static implicit operator int(ProtoBufferValue value)
        {
            if (value is Varint)
            {
                int result = value as Varint;
                return result;
            }
            else
            {
                throw new ProtoBufferException("is not varint");
            }
        }
        public static implicit operator long(ProtoBufferValue value)
        {
            if (value is Varint)
            {
                long result = value as Varint;
                return result;
            }
            else
            {
                throw new ProtoBufferException("is not varint");
            }
        }
        public static implicit operator float(ProtoBufferValue value)
        {
            if (value is Bit32)
            {
                float result = value as Bit32;
                return result;
            }
            else
            {
                throw new ProtoBufferException("is not Bit32");
            }
        }
        public static implicit operator double(ProtoBufferValue value)
        {
            if (value is Bit64)
            {
                double result = value as Bit64;
                return result;
            }
            else
            {
                throw new ProtoBufferException("is not Bit64");
            }
        }
        public static implicit operator string(ProtoBufferValue value)
        {
            if (value is LenghDelimited)
            {
                string result = value as LenghDelimited;
                return result;
            }
            else
            {
                throw new ProtoBufferException("is not LenghDelimited");
            }
        }
        public static implicit operator byte[](ProtoBufferValue value)
        {
            if (value is LenghDelimited)
            {
                byte[] result = value as LenghDelimited;
                return result;
            }
            else
            {
                throw new ProtoBufferException("is not LenghDelimited");
            }
        }

        public static implicit operator ProtoBufferValue(bool value)
        {
            Varint result = value;
            return result;
        }
        public static implicit operator ProtoBufferValue(int value)
        {
            Varint result = value;
            return result;
        }
        public static implicit operator ProtoBufferValue(long value)
        {
            Varint result = value;
            return result;
        }
        public static implicit operator ProtoBufferValue(float value)
        {
            Bit32 result = value;
            return result;
        }
        public static implicit operator ProtoBufferValue(double value)
        {
            Bit64 result = value;
            return result;
        }
        public static implicit operator ProtoBufferValue(string value)
        {
            LenghDelimited result = value;
            return result;
        }
        public static implicit operator ProtoBufferValue(byte[] value)
        {
            LenghDelimited result = value;
            return result;
        }

       
    }
}
