using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
namespace ProtoBuffer.Test
{
    class TestNormal
    {
        [Test]
        public void TestStack()
        {
            Stack<string> stack = new Stack<string>();


            stack.Push("1");
            stack.Push("2");
            stack.Push("3");
            stack.Push("4");
            List<string> list = new List<string>();
            
            while (stack.Count >0)
            {        
                list.Insert(0,stack.Pop());
            }
            foreach (string s in list)
            {
                Console.WriteLine(s);
            }
            

        }

        [Test]
        public void TestSubString()
        {
            string str = "//12345";

            Assert.AreEqual("12345", str.Substring(2));
        }

    }
}
