using System.Collections.Generic;
namespace ProtoBuffer
{
    /// <summary>
    /// protobuf的读写类
    /// </summary>
    public sealed class ProtoBufferReader
    {
        private readonly List<ProtoBufferObject> _list = new List<ProtoBufferObject>();

        public List<ProtoBufferObject> ProtoBufferObjs
        {
            get { return _list; }
        } 
       

        /// <summary>
        /// 构造函数
        /// ProtoBuffer读取类
        /// </summary>
        public ProtoBufferReader()
        {
            
        }
       /// <summary>
       /// 构造函数。
       /// 
       /// 
       /// </summary>
       /// 
       /// <exception cref="ProtoBuffer.ProtoBufferException"></exception>
       /// <param name="buf"></param>
        public ProtoBufferReader(byte[] buf) : this()
        {
            Read(buf);
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="buf">buffer</param>
        /// <param name="offset">offset</param>
        /// <param name="size">size</param>
        /// <exception cref="ProtoBuffer.ProtoBufferException"></exception>
        public ProtoBufferReader(byte[] buf, int offset, int size) : this()
        {
            Read(buf,offset,size);
        }

        /// <summary>
        /// 从字节数组中读取数据
        /// </summary>
        /// <param name="buf"></param>
        public void Read(byte[] buf)
        {
            Read(buf, 0, buf.Length);
        }
        /// <summary>
        /// 从字节数组中读取数据
        /// </summary>
        /// <param name="buf">字节数组</param>
        /// <param name="offset">开始位置</param>
        /// <param name="size">需要读取的数据的大小</param>
        public void Read(byte[] buf, int offset, int size)
        {
            _list.Clear();
            int tmpOffset = offset;
            while (tmpOffset < offset + size)
            {
                ProtoBufferObject obj = new ProtoBufferObject(buf, tmpOffset);                
                _list.Add(obj);
                tmpOffset += obj.Bytes.Length;
            }
        } 
    }
}
