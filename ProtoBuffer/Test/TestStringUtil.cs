using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;


namespace ProtoBuffer.Test
{
    class TestStringUtil
    {
        
        [Test]
        public void TestFirstUpper()
        {
            string origin = "a";
            string result = StringUtil.ToFirstUpper(origin);
            Assert.AreEqual("A",result);

            origin = "a1b2c3";
            result = StringUtil.ToFirstUpper(origin);
            Assert.AreEqual("A1b2c3",result);

//            origin = "7";
//            result = StringUtil.ToFirstUpper(origin);

        }
    }
}
