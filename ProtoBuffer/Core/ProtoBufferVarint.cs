using System;
using System.Globalization;

namespace ProtoBuffer
{
    /// <summary>
    /// Base 128 Varints序列化策略
    /// <para>如何进行序列化，请参照：https://developers.google.com/protocol-buffers/docs/encoding?hl=zh-CN </para>
    /// </summary>
    sealed class Varint : ProtoBufferValue
    {
        private Varint()
        {
            
        }
        /// <summary>
        /// 构造函数
        ///  <exception cref="ProtoBuffer.ProtoBufferException">buffer=null,或者从offset开始不是一个有效的varint</exception>
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        public Varint(byte[] buffer,int offset)
        {
            Value = GetVarintValue(buffer, offset);
            Bytes = GetVarintData(buffer, offset);
        }
        public static implicit operator Varint(long value)
        {
            return new Varint() {Value = value,Bytes = GetVarintData(value)};
        }
        public static implicit operator Varint(int value)
        {
            return new Varint(){Value = (long)value,Bytes = GetVarintData(value)};
        }
        public static implicit operator Varint(bool value)
        {
            if (value)
            {
                return new Varint(){Value = (long)1,Bytes = GetVarintData(1)};
            }
            else
            {
                return new Varint(){Value = (long)0,Bytes = GetVarintData(0)};
            }
        }
        public static implicit operator long(Varint value)
        {
            return (long)value.Value;
        }
        public static implicit operator int(Varint value)
        {
            long tmp = (long) value.Value;
            if (tmp > int.MaxValue)
            {
                throw new ProtoBufferException("value > int.MaxValue");
            }
            if (tmp < int.MinValue)
            {
                throw new ProtoBufferException("value < int.MinValue");
            }
            return (int) tmp;
        }
        public static implicit operator bool(Varint value)
        {
            long tmp = (long) value.Value;
            if (tmp == 1)
            {
                return true;
            }
            if (tmp == 0)
            {
                return false;
            }
            throw new ProtoBufferException("varint is ");
        }
        /// <summary>
        /// 对long进行序列化
        /// </summary>
        /// <param name="v">需要序列化的值</param>
        /// <returns>序列化后的byte[]</returns>
        private static byte[] GetVarintData(long v)
        {
            if (v >= 0)
            {
                byte[] tmp = new byte[10];
                int offset = 0;
                while (v >= (1 << 7))
                {
                    byte b = (byte)(0x80 | v);
                    tmp[offset] = b;
                    offset++;
                    v = v >> 7;
                }
                tmp[offset] = (byte)v;
                byte[] result = new byte[offset + 1];
                Array.Copy(tmp, 0, result, 0, result.Length);
                return result;
            }
            else
            {
                byte[] data = new byte[10];
                for (int i = 0; i < 9; i++)
                {
                    data[i] = (byte)255;
                }
                data[9] = (byte)1;
                byte[] tmp = GetVarintData(-v - 1);
                for (int i = 0; i < tmp.Length; i++)
                {
                    int b = data[i] - tmp[i];
                    if (b < 1 << 7)
                    {
                        b += 1 << 7;
                    }
                    data[i] = (byte)b;
                }
                return data;
            }
        }
        /// <summary>
        /// 对byte[] 进行反序列化。
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <exception cref="ProtoBuffer.ProtoBufferException">buffer=null,或者从offset开始不是一个有效的varint</exception>
        /// <returns>结果是varint的真实序列化的值（byte[]）</returns>
        private static byte[] GetVarintData(byte[] buffer, int offset)
        {
            if (buffer == null)
            {
                throw new ProtoBufferException("buffer = null");
            }

            bool all = true;
            int leng = 1;
            for (int i = 0; i < 10; i++)
            {
                if (buffer[offset + i] < (1 << 7))
                {
                    all = false;
                    break;
                }
                else
                {
                    leng++;
                }
            }
            if (all)
            {
                throw new ProtoBufferException("这不是一个有效的varint,前10字节都 > 128");
            }
            byte[] result = new byte[leng];
            Array.Copy(buffer, offset, result, 0, result.Length);
            return result;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        private static long GetVarintValue(byte[] buffer, int offset)
        {
            byte[] data = GetVarintData(buffer, offset);
            if (data.Length < 10)
            {
                long result = 0;
                for (int i = 0; i < data.Length; i++)
                {
                    result += (long)(0x7f & data[i]) << (7 * i);
                }
                return result;
            }
            else
            {
                byte[] tmp = new byte[10] { 255, 255, 255, 255, 255, 255, 255, 255, 255, 1 };
                long result = 0;
                for (int i = 0; i < 10; i++)
                {
                    result += (long)((tmp[i] - buffer[offset + i])) << (7 * i);
                }
                return -result - 1;
            }
        }
        public override string ToString()
        {
            long result = (long)Value;
            return result.ToString(CultureInfo.InvariantCulture);
        }
    }
}
