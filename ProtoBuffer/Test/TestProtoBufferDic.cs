using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;


namespace ProtoBuffer.Test
{
    class TestProtoBufferDic
    {
        public TestProtoBufferDic()
        {
            
        }

        [Test]
        public void TestMessage()
        {
            
            ProtoBufferDic dic = new ProtoBufferDic("dic");

            List<string> list = new List<string>();

            list.Add("package com.morln.game;");

            list.Add("option java_package = \"com.morln.game.newvs.command\";");

            list.Add("// Foo summary");
            list.Add("message Foo {");
            list.Add("// a_int summary");
            list.Add("required int32 a_int = 1;");
            list.Add("// a_long summary");
            list.Add("required int64 a_long = 2;");
            list.Add("// a_bool summary");
            list.Add("required bool a_bool = 3;");
            list.Add("// a_float summary");
            list.Add("required float a_float = 4;");
            list.Add("// a_double summary");
            list.Add("required double a_double = 5;");
            list.Add("// a_string summary");
            list.Add("required string a_string = 6;");
            list.Add("// a_bytes summary");
            list.Add("required bytes a_bytes = 7;");
            list.Add("}");

            
            ProtoBufferFile file = new ProtoBufferFile("foo.proto",dic,list);
            dic.AddFile(file);
            dic.Parse();

            Assert.AreEqual("foo.proto",dic.Files[0].FileName);
            Assert.AreEqual(1,dic.Files[0].Messages.Count);
            Assert.AreEqual(DataType.Class,dic.Files[0].Messages[0].DataType);        
            Assert.AreEqual(7,dic.Files[0].Messages[0].Fields.Count);



            // field 1

            ProtoBufferField field = dic.Files[0].Messages[0].Fields[0];

            Assert.AreEqual(1,field.Summarys.Count);
            Assert.AreEqual("a_int summary",field.Summarys[0]);
            Assert.AreEqual(RequiredType.Required,field.RequiredType);
            Assert.AreEqual(DataType.Int,field.DataType);
            Assert.AreEqual("int",field.DataName);
            Assert.AreEqual("a_int",field.Name);
            Assert.AreEqual(1,field.FieldNumber);

            field = dic.Files[0].Messages[0].Fields[1];

            Assert.AreEqual(1, field.Summarys.Count);
            Assert.AreEqual("a_long summary", field.Summarys[0]);
            Assert.AreEqual(RequiredType.Required, field.RequiredType);
            Assert.AreEqual(DataType.Long, field.DataType);
            Assert.AreEqual("long", field.DataName);
            Assert.AreEqual("a_long", field.Name);
            Assert.AreEqual(2, field.FieldNumber);

            field = dic.Files[0].Messages[0].Fields[2];

            Assert.AreEqual(1, field.Summarys.Count);
            Assert.AreEqual("a_bool summary", field.Summarys[0]);
            Assert.AreEqual(RequiredType.Required, field.RequiredType);
            Assert.AreEqual(DataType.Bool, field.DataType);
            Assert.AreEqual("bool", field.DataName);
            Assert.AreEqual("a_bool", field.Name);
            Assert.AreEqual(3, field.FieldNumber);

            field = dic.Files[0].Messages[0].Fields[3];

            Assert.AreEqual(1, field.Summarys.Count);
            Assert.AreEqual("a_float summary", field.Summarys[0]);
            Assert.AreEqual(RequiredType.Required, field.RequiredType);
            Assert.AreEqual(DataType.Float, field.DataType);
            Assert.AreEqual("float", field.DataName);
            Assert.AreEqual("a_float", field.Name);
            Assert.AreEqual(4, field.FieldNumber);

            field = dic.Files[0].Messages[0].Fields[4];

            Assert.AreEqual(1, field.Summarys.Count);
            Assert.AreEqual("a_double summary", field.Summarys[0]);
            Assert.AreEqual(RequiredType.Required, field.RequiredType);
            Assert.AreEqual(DataType.Double, field.DataType);
            Assert.AreEqual("double", field.DataName);
            Assert.AreEqual("a_double", field.Name);
            Assert.AreEqual(5, field.FieldNumber);

            field = dic.Files[0].Messages[0].Fields[5];

            Assert.AreEqual(1, field.Summarys.Count);
            Assert.AreEqual("a_string summary", field.Summarys[0]);
            Assert.AreEqual(RequiredType.Required, field.RequiredType);
            Assert.AreEqual(DataType.String, field.DataType);
            Assert.AreEqual("string", field.DataName);
            Assert.AreEqual("a_string", field.Name);
            Assert.AreEqual(6, field.FieldNumber);

            field = dic.Files[0].Messages[0].Fields[6];

            Assert.AreEqual(1, field.Summarys.Count);
            Assert.AreEqual("a_bytes summary", field.Summarys[0]);
            Assert.AreEqual(RequiredType.Required, field.RequiredType);
            Assert.AreEqual(DataType.Bytes, field.DataType);
            Assert.AreEqual("byte[]", field.DataName);
            Assert.AreEqual("a_bytes", field.Name);
            Assert.AreEqual(7, field.FieldNumber);
        }

        [Test]
        public void TestEnum()
        {

            ProtoBufferDic dic = new ProtoBufferDic("dic");

            List<string> list = new List<string>();

            list.Add("package com.morln.game;");

            list.Add("enum Foo {");
            list.Add("ONE = 1;");
            list.Add("TWO = 2;");
            list.Add("THREE = 3;");
            list.Add("}");


            ProtoBufferFile file = new ProtoBufferFile("foo.proto", dic, list);
            dic.AddFile(file);
            dic.Parse();


            ProtoBufferMessage msg = dic.Files[0].Messages[0];
            Assert.AreEqual(DataType.Enum,msg.DataType);
            Assert.AreEqual(3,msg.Fields.Count);


            ProtoBufferField field = msg.Fields[0];
            Assert.AreEqual(DataType.Enum,field.DataType);
            Assert.AreEqual("enum",field.DataName);
            Assert.AreEqual("ONE",field.Name);
            Assert.AreEqual(1,field.FieldNumber);

            field = msg.Fields[1];
            Assert.AreEqual(DataType.Enum, field.DataType);
            Assert.AreEqual("enum", field.DataName);
            Assert.AreEqual("TWO", field.Name);
            Assert.AreEqual(2, field.FieldNumber);

            field = msg.Fields[2];
            Assert.AreEqual(DataType.Enum, field.DataType);
            Assert.AreEqual("enum", field.DataName);
            Assert.AreEqual("THREE", field.Name);
            Assert.AreEqual(3, field.FieldNumber);
        }
        
        [Test]
        public void TestMultiMsg()
        {

            ProtoBufferDic dic = new ProtoBufferDic("dic");

            List<string> list = new List<string>();

            list.Add("package com.morln.game;");

            list.Add("message Packet {");
            list.Add("required Foo foo = 1;");
            list.Add("}");


            list.Add("enum Foo {");
            list.Add("ONE = 1;");
            list.Add("TWO = 2;");
            list.Add("THREE = 3;");
            list.Add("}");

            ProtoBufferFile file = new ProtoBufferFile("foo.proto", dic, list);
            dic.AddFile(file);
            dic.Parse();


            ProtoBufferMessage msg;
            ProtoBufferField field;

            msg = dic.Files[0].Messages[0];
            Assert.AreEqual("com.morln.game",msg.NameSpace);
            Assert.AreEqual(DataType.Class,msg.DataType);
            Assert.AreEqual("Packet",msg.Name);
            Assert.AreEqual(1, msg.Fields.Count);

            field = msg.Fields[0];
            Assert.AreEqual(RequiredType.Required,field.RequiredType);
            Assert.AreEqual(DataType.Enum,field.DataType);
            Assert.AreEqual("Foo",field.DataName);
            Assert.AreEqual("foo",field.Name);
            Assert.AreEqual(1,field.FieldNumber);

            msg = dic.Files[0].Messages[1];
            Assert.AreEqual("com.morln.game", msg.NameSpace);
            Assert.AreEqual(DataType.Enum, msg.DataType);
            Assert.AreEqual("Foo", msg.Name);
            Assert.AreEqual(3, msg.Fields.Count);


            field = msg.Fields[0];
            Assert.AreEqual(DataType.Enum, field.DataType);
            Assert.AreEqual("enum", field.DataName);
            Assert.AreEqual("ONE", field.Name);
            Assert.AreEqual(1, field.FieldNumber);

            field = msg.Fields[1];
            Assert.AreEqual(DataType.Enum, field.DataType);
            Assert.AreEqual("enum", field.DataName);
            Assert.AreEqual("TWO", field.Name);
            Assert.AreEqual(2, field.FieldNumber);

            field = msg.Fields[2];
            Assert.AreEqual(DataType.Enum, field.DataType);
            Assert.AreEqual("enum", field.DataName);
            Assert.AreEqual("THREE", field.Name);
            Assert.AreEqual(3, field.FieldNumber); 
        }



        [Test]
        public void TestMultiFiles()
        {

            ProtoBufferDic dic = new ProtoBufferDic("dic");
            ProtoBufferFile file;
            List<string> list ;

            list = new List<string>();
            list.Add("package com.morln.game;");
            
            list.Add("message Foo {");

            list.Add("required bool a_required_bool = 1;");
            list.Add("required int32 a_required_int = 2;");
            list.Add("required int64 a_required_long = 3;");
            list.Add("required float a_required_float = 4;");
            list.Add("required double a_required_double = 5;");
            list.Add("required string a_required_string = 6;");
            list.Add("required bytes a_required_bytes = 7;");

            list.Add("optional bool a_optional_bool = 8;");
            list.Add("optional int32 a_optional_int = 9;");
            list.Add("optional int64 a_optional_long = 10;");
            list.Add("optional float a_optional_float = 11;");
            list.Add("optional double a_optional_double = 12;");
            list.Add("optional string a_optional_string = 13;");
            list.Add("optional bytes a_optional_bytes = 14;");

            list.Add("repeated bool a_repeated_bool = 15;");
            list.Add("repeated int32 a_repeated_int = 16;");
            list.Add("repeated int64 a_repeated_long = 17;");
            list.Add("repeated float a_repeated_float = 18;");
            list.Add("repeated double a_repeated_double = 19;");
            list.Add("repeated string a_repeated_string = 20;");
            list.Add("repeated bytes a_repeated_bytes = 21;");


            list.Add("}");


            file = new ProtoBufferFile("foo.proto",dic,list);
            dic.AddFile(file);
            dic.Parse();

            ProtoBufferMessage msg = dic.Message("com.morln.game","Foo");

            ProtoBufferMessageWriter writer = new ProtoBufferMessageWriter();
            writer.NeedCheckRequired = true;
            MemoryStream memoryStream = new MemoryStream();
            writer.WriteMessage(msg,memoryStream);
            string content = UTF8Encoding.UTF8.GetString(memoryStream.ToArray());
            Console.WriteLine(content);

        }
        [Test]
        public void TestWriteEnum()
        {

            ProtoBufferDic dic = new ProtoBufferDic("dic");
            ProtoBufferFile file;
            List<string> list;

            list = new List<string>();
            list.Add("package com.morln.game;");
            list.Add("enum FooEnum {");
            list.Add("ONE = 1;");

            list.Add("}");

            list.Add("message Foo{");
            list.Add("required FooEnum required_enum = 1;");
            list.Add("optional FooEnum optional_enum = 2;");
            list.Add("repeated FooEnum repeated_enum = 3;");
            list.Add("}");
            file = new ProtoBufferFile("foo.proto", dic, list);
            dic.AddFile(file);
            dic.Parse();

            ProtoBufferMessage msg = dic.Message("com.morln.game", "Foo");

            ProtoBufferMessageWriter writer = new ProtoBufferMessageWriter();
            writer.NeedCheckRequired = true;
            MemoryStream memoryStream = new MemoryStream();
            writer.WriteMessage(msg, memoryStream);
            string content = UTF8Encoding.UTF8.GetString(memoryStream.ToArray());
            Console.WriteLine(content);
        }
       
        [Test]
        public void TestMultiNamespace()
        {
            ProtoBufferDic dic = new ProtoBufferDic();
            List<string> list;
            ProtoBufferFile file;

            list = new List<string>();
            list.Add("package com.morln.fooenum;");
            list.Add("enum FooEnum{");
            list.Add("}");
            file = new ProtoBufferFile("fooenum.proto", dic, list);
            dic.AddFile(file);
            list = new List<string>();
            list.Add("package com.morln.foo;");
            list.Add("message Foo{");
            list.Add("}");
            file = new ProtoBufferFile("foo.proto", dic, list);
            dic.AddFile(file);

            list = new List<string>();
            list.Add("package com.morln.packet;");
            list.Add("import fooenum.proto;");
            list.Add("import foo.proto");
            list.Add("message Packet{");
            list.Add("required FooEnum a_foo_enum = 1");
            list.Add("required Foo foo = 2");
            list.Add("repeated int32 value = 3;");
            list.Add("}");
            
            file = new ProtoBufferFile("packet.proto", dic, list);
            dic.AddFile(file);
            
            
            dic.Parse();

            ProtoBufferMessage msg = dic.Message("com.morln.packet", "Packet");

            ProtoBufferMessageWriter writer = new ProtoBufferMessageWriter();
            writer.NeedCheckRequired = true;
            MemoryStream memoryStream = new MemoryStream();
            writer.WriteMessage(msg, memoryStream);
            string content = UTF8Encoding.UTF8.GetString(memoryStream.ToArray());
            Console.WriteLine(content);

        }

       [Test]
        public void TestLocalFiles()
       {
           string path = "E:\\work\\newvs\\Proto";

            
           ProtoBufferDic dic = new ProtoBufferDic(path);

           dic.OutNameSpace = "assets.sanxiao.communication.proto";

           dic.Parse();

           ProtoBufferMessageWriter writer = new ProtoBufferMessageWriter();

           foreach (ProtoBufferMessage msg in dic.Messages)
           {
               writer.WriteMessage(msg, path);
               
           }           

        }

    }
}
