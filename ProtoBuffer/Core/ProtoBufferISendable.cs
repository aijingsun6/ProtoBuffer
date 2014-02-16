
namespace ProtoBuffer
{
    /// <summary>
    /// 将类序列化为ProtoBuffer字节数组类的接口
    /// </summary>
    public interface ISendable
    {
        /// <summary>
        /// 得到可发送的ProtoBuffer字节数据
        /// </summary>
        /// <returns></returns>
        byte[] GetProtoBufferBytes();
    }
}
