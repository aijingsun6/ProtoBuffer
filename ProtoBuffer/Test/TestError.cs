using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
namespace ProtoBuffer.Test
{
    class TestError
    {
        public TestError()
        {
            
        }

        [Test]
        public void TestFileNameError()
        {
            ProtoBufferDic dic = new ProtoBufferDic("dic");

            ProtoBufferFile file;
            List<string> list;

            list = new List<string>();
            list.Add("package com.morln.game;");
            list.Add("message Foo {");
            list.Add("required int32 a_int = 1;");
            list.Add("}");
            file = new ProtoBufferFile("foo", dic, list);
            dic.AddFile(file);
            dic.Parse();
        }

        [Test]
        public void TestNameSpace()
        {
            ProtoBufferDic dic = new ProtoBufferDic("dic");

            ProtoBufferFile file;
            List<string> list;

            list = new List<string>();
            //list.Add("package com.morln.game;");
            list.Add("message Foo {");
            list.Add("required int32 a_int = 1;");
            list.Add("}");
            file = new ProtoBufferFile("foo.proto", dic, list);
            dic.AddFile(file);
            dic.Parse();
        }

        [Test]
        public void TestImport()
        {
            ProtoBufferDic dic = new ProtoBufferDic("dic");

            ProtoBufferFile file;
            List<string> list;

            list = new List<string>();
            list.Add("package com.morln.game;");
            list.Add("import \"pro.proto\"");
            list.Add("message Foo {");
            list.Add("required int32 a_int = 1;");
            list.Add("}");
            file = new ProtoBufferFile("foo.proto", dic, list);
            dic.AddFile(file);
            dic.Parse();
        }

        /// <summary>
        /// 在文件中出现无效的行
        /// </summary>
        [Test]
        public void TestInvalidLineInFile()
        {
            ProtoBufferDic dic = new ProtoBufferDic("dic");

            ProtoBufferFile file;
            List<string> list;

            list = new List<string>();
            list.Add("package com.morln.game;");
            list.Add("message Foo {");
            list.Add("required int32 a_int = 1;");
            list.Add("}");
            list.Add("//invalid line");
            list.Add("//invalid line");
            file = new ProtoBufferFile("foo.proto", dic, list);
            dic.AddFile(file);
            dic.Parse();
        }

        [Test]
        public void TestInvalidLineInMessage()
        {
            ProtoBufferDic dic = new ProtoBufferDic("dic");

            ProtoBufferFile file;
            List<string> list;

            list = new List<string>();
            list.Add("package com.morln.game;");
            list.Add("message Foo {");           
            list.Add("required int32 a_int = 1;");
            list.Add("//invalid line");
            list.Add("}");           
            file = new ProtoBufferFile("foo.proto", dic, list);
            dic.AddFile(file);
            dic.Parse();

            Assert.AreEqual(1,dic.Files.Count);
        }
        [Test]
        public void TestInvalidSyntax()
        {

            ProtoBufferDic dic = new ProtoBufferDic("dic");
            ProtoBufferFile file;
            List<string> list;
            list = new List<string>();
            list.Add("package com.morln.game;");
            list.Add("message Foo ");
            list.Add("{");
            list.Add("required int32 a_int = 1;");
            list.Add("}");
            file = new ProtoBufferFile("foo.proto", dic, list);
            dic.AddFile(file);
            dic.Parse();

            Assert.AreEqual(1, dic.Files.Count);
        }
    }
}
