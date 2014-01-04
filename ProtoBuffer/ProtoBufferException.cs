using System;
namespace ProtoBuffer
{
    /// <summary>
    /// ProtoBuffer 异常类
    /// </summary>
    public class ProtoBufferException:Exception
    {
        public ProtoBufferException() : base()
        {
        }

        public ProtoBufferException(string message) : base(message)
        {
        }

        public ProtoBufferException(string message, Exception innerExceprion) : base(message, innerExceprion)
        {
        }
    }
}
