using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
namespace ProtoBuffer.Test
{
    class TestProtoBufferObj
    {
        private const int RANDOM_COUNT = 10000;

        public TestProtoBufferObj()
        {
            
        }
        [Test]
        public void TestSerilize()
        {
            int originFieldNumber = 1;
            int origin = 150;
            byte[] data;
            int resultFieldNumber;
            int result;
            Random random = new Random(DateTime.Now.Millisecond);

            for (int i = 0; i < RANDOM_COUNT; i++)
            {
                
                ProtoBufferObject obj = new ProtoBufferObject(originFieldNumber,origin);

                data = obj.Bytes;

                obj = new ProtoBufferObject(data,0);

                resultFieldNumber = obj.FieldNumber;

                result = obj.Value;

                Assert.AreEqual(originFieldNumber, resultFieldNumber);

                Assert.AreEqual(origin,result);

            }
            

            




        }
    }
}
