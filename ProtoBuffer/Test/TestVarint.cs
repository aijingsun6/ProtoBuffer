using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
namespace ProtoBuffer.Test
{
    class TestVarint
    {
        private const int RANDOM_COUNT = 10000;

        public TestVarint()
        {
            
        }

        [Test]
        public void TestSerialize()
        {
            int origin;
            Varint varint;
            byte[] data;
            int result;
            Random random = new Random(DateTime.Now.Millisecond);


            for (int i = 0; i < RANDOM_COUNT; i++)
            {
                origin = random.Next(int.MinValue, int.MaxValue);
                varint = origin;
                data = varint.Bytes;
                Varint newOne = new Varint(data,0);
                result = newOne;
                Assert.AreEqual(origin,result);
            }


        }


        

    }
}
