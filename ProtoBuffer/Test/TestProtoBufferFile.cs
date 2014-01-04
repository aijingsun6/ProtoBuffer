using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;


namespace ProtoBuffer.Test
{

    public class TestProtoBufferFile
    {

        private List<string> list;
        private string fileName = "test.proto";


        [Test]
        public void SetUp()
        {
            list = new List<string>();
            list.Add("package com.morln.game;");
            list.Add("// 游客设备登陆。");
            list.Add("message DeviceLogin {");
            list.Add("// 设备的唯一识别ID。");
            list.Add("required string device_uid = 1;");
            list.Add("// 客户端信息。");
            list.Add("required ClientInfo client_info = 2;");
            list.Add("//Message summary");
            list.Add("message Message {");
            list.Add("}");
            list.Add("}");
            list.Add("//enum summary");
            list.Add("enum MyEnum{");
            list.Add("}");
        }

        [Test]
        public void TestFile()
        {

            


        }
    }
}
