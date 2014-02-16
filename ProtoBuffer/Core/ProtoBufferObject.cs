using System;
namespace ProtoBuffer
{
    /// <summary>
    /// protobuf 最基本的结构 
    /// 
    /// </summary>
    public class ProtoBufferObject
    {
        public int FieldNumber { get; private set; }

        private WireType WireType { get; set; }

        public ProtoBufferValue Value { get; private set; }

        public byte[] Bytes { get; private set; }

        public ProtoBufferObject(int fieldNumber, bool value):this(fieldNumber,value?1:0)
        {
            
        }
        public ProtoBufferObject(int fieldNumber, int value):this(fieldNumber,(long)value)
        {
            
        }
        public ProtoBufferObject(int fieldNumber,long value)
        {
            FieldNumber = fieldNumber;
            WireType = WireType.Varint;
            Value = value;
            BuildBytes();
        }
        public ProtoBufferObject(int fieldNumber,float value)
        {
            FieldNumber = fieldNumber;
            WireType = WireType.Bit32;
            Value = value;
            BuildBytes();
        }
        public ProtoBufferObject(int fieldNumber, double value)
        {
            FieldNumber = fieldNumber;
            WireType = WireType.Bit64;
            Value = value;
            BuildBytes();
        }
        public ProtoBufferObject(int fieldNumber, string value)
        {
            if (value == null)
            {
                throw new ProtoBufferException(string.Format("异常：fieldNumber:{0}的string为null",fieldNumber));
            }
            FieldNumber = fieldNumber;
            WireType = WireType.LengthDelimited;   
            Value = value;
            BuildBytes();
        }
        public ProtoBufferObject(int fieldNumber, byte[] value)
        {
            if (value == null)
            {
                throw new ProtoBufferException(string.Format("异常：fieldNumber:{0}的byte[]为null", fieldNumber));
            }
            FieldNumber = fieldNumber;
            WireType = WireType.LengthDelimited;     
            Value = value;
            BuildBytes();
        }
        public ProtoBufferObject(byte[] buffer, int offset)
        {
            if (buffer == null)
            {
                throw new ProtoBufferException(string.Format("字节数组是空"));
            }
            if (offset > buffer.Length)
            {
                throw new ProtoBufferException(string.Format("offset > 字节数组的长度。"));
            }
            int tmpOffset = offset;
            Varint header = new Varint(buffer,tmpOffset);
            tmpOffset += header.Bytes.Length;
            int headerValue = header;
            FieldNumber = headerValue >> 3;
            WireType = (WireType) ((byte) headerValue & 0x07);
            switch (WireType)
            {
                case WireType.Varint:
                    Value = new Varint(buffer,tmpOffset);;
                    tmpOffset += Value.Bytes.Length;
                    Bytes = new byte[tmpOffset-offset];
                    Array.Copy(buffer,offset,Bytes,0,Bytes.Length);
                    break;
                case WireType.Bit64:
                    Value = new Bit64(buffer, tmpOffset);
                    tmpOffset += Value.Bytes.Length;
                    Bytes = new byte[tmpOffset-offset];
                    Array.Copy(buffer,offset,Bytes,0,Bytes.Length);
                    break;
                case WireType.LengthDelimited:
                    Varint strleng = new Varint(buffer,tmpOffset);
                    int leng = strleng;
                    tmpOffset += strleng.Bytes.Length;
                    Value = new LenghDelimited(buffer, tmpOffset, leng);
                    tmpOffset += Value.Bytes.Length;
                    Bytes = new byte[tmpOffset-offset];
                    Array.Copy(buffer,offset,Bytes,0,Bytes.Length);
                    break;
                case WireType.Bit32:
                    Value = new Bit32(buffer,tmpOffset);
                    tmpOffset += Value.Bytes.Length;
                    Bytes = new byte[tmpOffset-offset];
                    Array.Copy(buffer,offset,Bytes,0,Bytes.Length);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void BuildBytes()
        {
            ProtoBufferValue header = FieldNumber<<3|(int)WireType;
            byte[] data0 = header.Bytes;

            switch (WireType)
            {
                case WireType.Bit64:
                case WireType.Bit32:
                case WireType.Varint:
                    byte[] data1 = Value.Bytes;
                    Bytes = new byte[data0.Length + data1.Length];
                    Array.Copy(data0,0,Bytes,0,data0.Length);
                    Array.Copy(data1,0,Bytes,data0.Length,data1.Length);
                    break;
                case WireType.LengthDelimited:
                    ProtoBufferValue leng = Value.Bytes.Length;
                    byte[] strBytes = leng.Bytes;
                    byte[] data2 = Value.Bytes;
                    Bytes= new byte[data0.Length + strBytes.Length + data2.Length];
                    Array.Copy(data0,0,Bytes,0,data0.Length);
                    Array.Copy(strBytes,0,Bytes,data0.Length,strBytes.Length);
                    Array.Copy(data2,0,Bytes,data0.Length+strBytes.Length,data2.Length);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        public override string ToString()
        {
            return string.Format("({0},{1})",FieldNumber,Value.ToString());
        }
    }
}
