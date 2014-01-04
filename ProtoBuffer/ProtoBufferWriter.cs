using System.Collections.Generic;
using System.IO;
namespace ProtoBuffer
{
    public sealed class ProtoBufferWriter
    {
        private MemoryStream _memorystream;
        /// <summary>
        /// 构造函数
        /// ProtoBuffer写入类
        /// </summary>
        public ProtoBufferWriter()
        {
            _memorystream = new MemoryStream();
        }

        public void Write(int fieldNumber, bool v)
        {
            Write(new ProtoBufferObject(fieldNumber, v));
        }

        public void Write(int fieldNumber,IEnumerable<bool> enumerable)
        {
            foreach (bool v in enumerable)
            {
                Write(fieldNumber,v);
            }
        }

        public void Write(int fieldNumber, int v)
        {
            Write(new ProtoBufferObject(fieldNumber, v));
        }

        public void Write(int fieldNumber,IEnumerable<int> enumerable)
        {
            foreach (int v in enumerable)
            {
                Write(fieldNumber, v);
            }
        }

        public void Write(int fieldNumber, long v)
        {
            Write(new ProtoBufferObject(fieldNumber, v));
        }

        public void Write(int fieldNumber, IEnumerable<long> enumerable)
        {
            foreach (long v in enumerable)
            {
                Write(fieldNumber, v);
            }
        }



        public void Write(int fieldNumber, float v)
        {
            Write(new ProtoBufferObject(fieldNumber, v));
        }

        public void Write(int fieldNumber, IEnumerable<float> enumerable)
        {
            foreach (float v in enumerable)
            {
                Write(fieldNumber, v);
            }
        }

        public void Write(int fieldNumber, double v)
        {
            Write(new ProtoBufferObject(fieldNumber, v));
        }

        public void Write(int fieldNumber, IEnumerable<double> enumerable)
        {
            foreach (double v in enumerable)
            {
                Write(fieldNumber, v);
            }
        }

        public void Write(int fieldNumber, string v)
        {
            Write(new ProtoBufferObject(fieldNumber, v));
        }
        public void Write(int fieldNumber, IEnumerable<string> enumerable)
        {
            foreach (string v in enumerable)
            {
                Write(fieldNumber, v);
            }
        }

        public void Write(int fieldNumber, byte[] v)
        {
            Write(new ProtoBufferObject(fieldNumber, v));
        }

        public void Write(int fieldNumber, IEnumerable<byte[]> enumerable)
        {
            foreach (byte[] v in enumerable)
            {
                Write(fieldNumber, v);
            }
        }

        public void Write(int fieldNumber,ISendable sendable)
        {
            Write(fieldNumber,sendable.GetProtoBufferBytes());
        }
        public void Write(int fieldNumber, IEnumerable<ISendable> sendables)
        {
            foreach (ISendable sendable in sendables)
            {
                Write(fieldNumber,sendable);
            }
        }
        public void Write(ProtoBufferObject obj)
        {
            _memorystream.Write(obj.Bytes, 0, obj.Bytes.Length);
        }
        /// <summary>
        /// 得到ProtoBuffer字节数组
        /// 就是需要发送的字节数组
        /// </summary>
        /// <returns></returns>
        public byte[] GetProtoBufferBytes()
        {
            return _memorystream.ToArray();
        }
    }
}
