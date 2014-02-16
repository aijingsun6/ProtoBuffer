using System;
using System.Collections.Generic;
using System.Text;

namespace ProtoBuffer
{
    public class KeyWord
    {
         
        private static readonly List<string> Keys = new List<string>();

        public static bool IsKeyWord(string word)
        {
            if (Keys.Count == 0)
            {
                Keys.Add("abstract");
                Keys.Add("as");
                Keys.Add("base");
                Keys.Add("bool");
                Keys.Add("break");
                Keys.Add("byte");
                Keys.Add("case");
                Keys.Add("catch");
                Keys.Add("char");
                Keys.Add("checked");
                Keys.Add("class");
                Keys.Add("const");
                Keys.Add("continue");
                Keys.Add("decimal");
                Keys.Add("default");
                Keys.Add("delegate");
                Keys.Add("do");
                Keys.Add("double");
                Keys.Add("else");
                Keys.Add("enum");
                Keys.Add("event");
                Keys.Add("explicit");
                Keys.Add("extern");
                Keys.Add("false");
                Keys.Add("finally");
                Keys.Add("fixed");
                Keys.Add("float");
                Keys.Add("for");
                Keys.Add("foreach");
                Keys.Add("goto");
                Keys.Add("if");
                Keys.Add("implicit");
                Keys.Add("in");
                Keys.Add("int");
                Keys.Add("interface");
                Keys.Add("internal");
                Keys.Add("is");
                Keys.Add("lock");
                Keys.Add("long");
                Keys.Add("namespace");
                Keys.Add("new");
                Keys.Add("null");
                Keys.Add("object");
                Keys.Add("Object");
                Keys.Add("operator");
                Keys.Add("out");
                Keys.Add("override");
                Keys.Add("params");
                Keys.Add("private");
                Keys.Add("protected");
                Keys.Add("public");
                Keys.Add("readonly");
                Keys.Add("ref");
                Keys.Add("return");
                Keys.Add("sbyte");
                Keys.Add("sealed");
                Keys.Add("short");
                Keys.Add("sizeof");
                Keys.Add("stackalloc");
                Keys.Add("static");
                Keys.Add("string");
                Keys.Add("struct");
                Keys.Add("switch");
                Keys.Add("this");
                Keys.Add("throw");
                Keys.Add("true");
                Keys.Add("try");
                Keys.Add("typeof");
                Keys.Add("uint");
                Keys.Add("ulong");
                Keys.Add("unchecked");
                Keys.Add("unsafe");
                Keys.Add("ushort");
                Keys.Add("using");
                Keys.Add("virtual");
                Keys.Add("void");
                Keys.Add("volatile");
                Keys.Add("while");
            }
            return Keys.Contains(word);
        }
        


    }
}
