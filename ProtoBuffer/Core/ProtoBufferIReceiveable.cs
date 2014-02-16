namespace ProtoBuffer
{
    /// <summary>
    /// 从ProtoBuffer数据中读取消息的接口
    /// </summary>
    public interface IReceiveable
    {
        void ParseFrom(byte[] buffer, int offset, int size);

        void ParseFrom(byte[] buffer);
    }
}
