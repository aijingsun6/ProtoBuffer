using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
namespace ProtoBuffer.Test
{
    class TestReaderAndWriter
    {
        public TestReaderAndWriter()
        {
            
        }
        [Test]
        public void Test()
        {
            ProtoBufferWriter writer = new ProtoBufferWriter();
 
            writer.Write(1,1);

            writer.Write(2,1);

            writer.Write(2,2);

            writer.Write(3,"sunaijing");

            byte[] data = writer.GetProtoBufferBytes();

            ProtoBufferReader reader = new ProtoBufferReader(data);

            Assert.AreEqual(4,reader.ProtoBufferObjs.Count);





        }
    }
}
