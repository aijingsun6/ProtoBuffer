using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ProtoBuffer
{
    
    class ProtoBufferMessageWriter
    {

        #region common

        private const string Tap0Left = "{";
        private const string Tap0Right = "}";

        private const string Tap1Left = "\t{";
        private const string Tap1Right = "\t}";

        private const string Tap2Left = "\t\t{";
        private const string Tap2Right = "\t\t}";

        private const string Tap3Left = "\t\t\t{";
        private const string Tap3Right = "\t\t\t}";

        private const string Tap4Left = "\t\t\t\t{";
        private const string Tap4Right = "\t\t\t\t}";

        private const string Tap5Left = "\t\t\t\t\t{";
        private const string Tap5Right = "\t\t\t\t\t}";

        private const string Tap1 = "\t";
        private const string Tap2 = "\t\t";
        private const string Tap3 = "\t\t\t";
        private const string Tap4 = "\t\t\t\t";
        private const string Tap5 = "\t\t\t\t\t";
        private const string Tap6 = "\t\t\t\t\t\t";

        #endregion


        public LanguageType LanguageType { get; set; }

        public bool NeedCheckRequired { get; set; }

        public ProtoBufferMessageWriter()
        {
            
        }

        public ProtoBufferMessageWriter(LanguageType languageType = LanguageType.CShape,bool needCheckRequired = false)
        {
            LanguageType = languageType;
            NeedCheckRequired = needCheckRequired;
        }



        public void WriteMessage(ProtoBufferMessage message,string path)
        {

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string nameSpace = Namespace(message.NameSpace);
            string[] strings = nameSpace.Split(new string[] {"."}, StringSplitOptions.RemoveEmptyEntries);
            StringBuilder sb = new StringBuilder();
            sb.Append(path);
            foreach (string s in strings)
            {
                sb.Append(string.Format("\\{0}", s));
            }
            if (!Directory.Exists(sb.ToString()))
            {
                Directory.CreateDirectory(sb.ToString());
            }

            sb.Append(string.Format("\\{0}.cs",message.Name));
            string fullFileName = sb.ToString();
            if (File.Exists(fullFileName))
            {
                File.Delete(fullFileName);
            }
            using (FileStream fs = new FileStream(fullFileName, FileMode.CreateNew))
            {
                try
                {
                    WriteMessage(message,fs);
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    fs.Close();
                }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="stream"></param>
        /// <exception cref="System.Exception">写入到stream</exception>
        public void WriteMessage(ProtoBufferMessage message, Stream stream)
        {
           
            using (StreamWriter writer = new StreamWriter(stream,Encoding.UTF8))
            {

                try
                {
                    switch (LanguageType)
                    {
                        case LanguageType.CShape:
                            WriteMessageCShape(message, writer);
                            break;
                        case LanguageType.Java:
                            WriteMessageJava(message,writer);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                catch (Exception exception)
                {
                    throw exception;
                }
                finally
                {
                    writer.Close();
                }

            }
        }
        private void WriteMessageCShape(ProtoBufferMessage message, StreamWriter writer)
        {
            //写入依赖库
            writer.WriteLine(string.Format("using System;"));
            writer.WriteLine(string.Format("using System.Collections.Generic;"));
            writer.WriteLine(string.Format("using System.Text;"));
            writer.WriteLine(string.Format("using ProtoBuffer;"));
            foreach (ProtoBufferFile dependent in message.Dependents)
            {
                if (message.NameSpace.Equals(dependent.NameSpace))
                {
                    continue;
                }
                writer.WriteLine(string.Format("using {0};", Namespace(dependent.NameSpace)));
            }
            //写入命名空间
            writer.WriteLine(string.Format("namespace {0}", Namespace(message.File.NameSpace)));
           
            //命名空间开始
            writer.WriteLine(Tap0Left);
            //class 的注释
            WriteSummaryCShape(writer,message.Summarys,Tap1);


            if (message.DataType == DataType.Class)
            {

                writer.WriteLine(string.Format("{0}public class {1} : ISendable, IReceiveable",Tap1,message.Name));
                writer.WriteLine(Tap1Left);
                WritePropertyCShape(writer,message);
                WriteConstructorCShape(writer,message);
                WriteCheckRequiredCShape(writer,message);
                WriteSendableCShape(writer,message);
                WriteReceiveableCShape(writer,message);
                Write2StringCShape(writer,message);
                writer.WriteLine(Tap1Right);
            }
            else if(message.DataType == DataType.Enum)
            {
                
                writer.WriteLine(string.Format("{0}public enum {1}",Tap1,message.Name));
                writer.WriteLine(Tap1Left);
                foreach (ProtoBufferField field in message.Fields)
                {
                    WriteSummaryCShape(writer,field.Summarys,Tap2);
                    writer.WriteLine(string.Format("{0}{1} = {2}",Tap2,field.Name,field.FieldNumber));
                }
                writer.WriteLine(Tap1Right);
            }
            //命名空间结束
            writer.WriteLine(Tap0Right);
        }
        /// <summary>
        /// C# 写入注释
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="summarys"></param>
        /// <param name="tap"></param>
        private void WriteSummaryCShape(StreamWriter writer, List<string> summarys, string tap)
        {
            writer.WriteLine(string.Format("{0}/// <summary>", tap));
            foreach (string summary in summarys)
            {
                writer.WriteLine(string.Format("{0}/// {1}", tap, summary));
            }
            writer.WriteLine(string.Format("{0}/// </summary>", tap));
        }
        /// <summary>
        /// java 向writer写入message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="writer"></param>
        private void WriteMessageJava(ProtoBufferMessage message, StreamWriter writer)
        {
            
        }
        /// <summary>
        /// c#写入属性
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="message"></param>
        private void WritePropertyCShape(StreamWriter writer,ProtoBufferMessage message)
        {

            foreach (ProtoBufferField field in message.Fields)
            {

                string requiredCheckName = CheckRequiredName(field.Name, field.RequiredType);
                string requiredMemberName = MemberName(field.Name, field.RequiredType);
                string requiredPropertyName = PropertyName(field.Name, field.RequiredType);

                switch (field.RequiredType)
                {
                    case RequiredType.Required:
                        writer.WriteLine(string.Format("{0}private bool {1}", Tap2, requiredCheckName) + "{get;set;}");
                        break;
                    case RequiredType.Optional:
                        writer.WriteLine(string.Format("{0}public bool {1}", Tap2, requiredCheckName) + "{get;private set;}");
                        break;
                    case RequiredType.Repeated:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                if (field.RequiredType != RequiredType.Repeated)
                {
                    writer.WriteLine(string.Format("{0}private {1} {2};", Tap2, field.DataName, requiredMemberName));
                }
                else
                {
                    writer.WriteLine(string.Format("{0}private List<{1}> {2};", Tap2, field.DataName, requiredMemberName));
                }
                WriteSummaryCShape(writer, field.Summarys, Tap2);
                
                if (field.RequiredType != RequiredType.Repeated)
                {
                    writer.WriteLine(string.Format("{0}public {1} {2}", Tap2, field.DataName, requiredPropertyName));
                }
                else
                {
                    writer.WriteLine(string.Format("{0}public List<{1}> {2}", Tap2, field.DataName, requiredPropertyName));
                }
                writer.WriteLine(Tap2Left);
                //get
                writer.WriteLine(string.Format("{0}get", Tap3));
                writer.WriteLine(Tap3Left);
                writer.WriteLine(string.Format("{0}return {1};", Tap4, requiredMemberName));
                writer.WriteLine(Tap3Right);
                //end get

                //set
                writer.WriteLine(string.Format("{0}set", Tap3));
                writer.WriteLine(Tap3Left);
                switch (field.DataType)
                {
                    case DataType.Enum:

                    case DataType.Bool:

                    case DataType.Int:

                    case DataType.Long:

                    case DataType.Float:

                    case DataType.Double:
                        if (field.RequiredType == RequiredType.Repeated)
                        {
                            writer.WriteLine(string.Format("{0}if(value != null)", Tap4));
                            writer.WriteLine(Tap4Left);
                            writer.WriteLine(string.Format("{0}{1} = value;", Tap5, requiredMemberName));
                            writer.WriteLine(Tap4Right);
                        }
                        else
                        {
                            writer.WriteLine(string.Format("{0}{1} = true;", Tap4, requiredCheckName));
                            writer.WriteLine(string.Format("{0}{1} = value;", Tap4, requiredMemberName));
                        } 
                        break;
                    case DataType.String:
                    case DataType.Bytes:
                    case DataType.Class:
                        writer.WriteLine(string.Format("{0}if(value != null)", Tap4));
                        writer.WriteLine(Tap4Left);
                        if (field.RequiredType != RequiredType.Repeated)
                        {
                            writer.WriteLine(string.Format("{0}{1} = true;", Tap5, requiredCheckName));
                        }
                        writer.WriteLine(string.Format("{0}{1} = value;", Tap5, requiredMemberName));
                        writer.WriteLine(Tap4Right);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                writer.WriteLine(Tap3Right);
                //end set
                writer.WriteLine(Tap2Right);
                writer.WriteLine();
                
            }
            
        }
        /// <summary>
        /// c# 写构造函数
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="message"></param>
        private void WriteConstructorCShape(StreamWriter writer, ProtoBufferMessage message)
        {
            List<ProtoBufferField> requiredFields = new List<ProtoBufferField>();
            List<ProtoBufferField> repeatedFields = new List<ProtoBufferField>();

            foreach (ProtoBufferField field in message.Fields)
            {
                switch (field.RequiredType)
                {
                    case RequiredType.Required:
                        requiredFields.Add(field);
                        break;
                    case RequiredType.Optional:
                        break;
                    case RequiredType.Repeated:
                        repeatedFields.Add(field);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            //写入空构造函数
            WriteSummaryCShape(writer,message.Summarys,Tap2);
            writer.WriteLine(string.Format("{0}public {1}()",Tap2,message.Name));
            writer.WriteLine(Tap2Left);
            foreach (ProtoBufferField field in repeatedFields)
            {
                string proprtyName = PropertyName(field.Name, field.RequiredType);
                writer.WriteLine(string.Format("{0}{1} = new List<{2}>();",Tap3,proprtyName,field.DataName));
            }
            writer.WriteLine(Tap2Right);
            writer.WriteLine();
            //写入带有必须参数的构造函数
            if (requiredFields.Count == 0)
            {
                return;
            }
            WriteSummaryCShape(writer, message.Summarys, Tap2);
            writer.WriteLine(string.Format("{0}public {1}",Tap2,message.Name));
            writer.WriteLine(string.Format("{0}(",Tap2));
            for (int i = 0; i <  requiredFields.Count; i++)
            {
                string memberName = MemberName(requiredFields[i].Name, requiredFields[i].RequiredType);
                if (i == requiredFields.Count -1)
                {
                    writer.WriteLine(string.Format("{0}{1} {2}",Tap3,requiredFields[i].DataName,memberName));
                }
                else
                {
                    writer.WriteLine(string.Format("{0}{1} {2},", Tap3, requiredFields[i].DataName, memberName));
                }
            }
            writer.WriteLine(string.Format("{0}):this()",Tap2));
            writer.WriteLine(Tap2Left);
            foreach (ProtoBufferField field in requiredFields)
            {
                string proprtyName = PropertyName(field.Name, field.RequiredType);
                string memberName = MemberName(field.Name, field.RequiredType);
                writer.WriteLine(string.Format("{0}{1} = {2};",Tap3,proprtyName,memberName));
            }
            writer.WriteLine(Tap2Right);
        }
        /// <summary>
        /// c# 写入required field检查
        /// </summary>
        private void WriteCheckRequiredCShape(StreamWriter writer, ProtoBufferMessage message)
        {
            if (!NeedCheckRequired)
            {
                return;
            }
            writer.WriteLine(string.Format("{0}private void CheckRequiredFields()",Tap2));
            writer.WriteLine(Tap2Left);
            List<ProtoBufferField> requiredFields = new List<ProtoBufferField>();
            foreach (ProtoBufferField field in message.Fields)
            {
                switch (field.RequiredType)
                {
                    case RequiredType.Required:
                        requiredFields.Add(field);
                        break;
                    case RequiredType.Optional:
                        break;
                    case RequiredType.Repeated:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            foreach (ProtoBufferField field in requiredFields)
            {
                string checkName = CheckRequiredName(field.Name, field.RequiredType);

                writer.WriteLine(string.Format("{0}if( !{1})",Tap3,checkName));
                writer.WriteLine(Tap3Left);
                writer.WriteLine(string.Format("{0}throw new ProtoBufferException(\"missing required field,name:\" + \"{1}\" +\",fieldNumber:\" +{2});",Tap4,field.Name,field.FieldNumber));
                writer.WriteLine(Tap3Right);
            }
            writer.WriteLine(Tap2Right);

        }

        /// <summary>
        /// c# 写入GetProtoBufferBytes
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="message"></param>
        private void WriteSendableCShape(StreamWriter writer, ProtoBufferMessage message)
        {
            writer.WriteLine(string.Format("{0}public byte[] GetProtoBufferBytes()",Tap2));
            writer.WriteLine(Tap2Left);
            if (NeedCheckRequired)
            {
                writer.WriteLine(string.Format("{0}CheckRequiredFields();",Tap3));
            }
            writer.WriteLine(string.Format("{0}ProtoBufferWriter writer = new ProtoBufferWriter();",Tap3));
            foreach (ProtoBufferField field in message.Fields)
            {
                string proptertyName = PropertyName(field.Name, field.RequiredType);
                string checkName = CheckRequiredName(field.Name, field.RequiredType);
                switch (field.DataType)
                {
                    case DataType.Enum:

                        switch (field.RequiredType)
                        {
                            case RequiredType.Required:
                                writer.WriteLine(string.Format("{0}writer.Write({1},(int){2});", Tap3, field.FieldNumber, proptertyName));
                                break;
                            case RequiredType.Optional:
                                writer.WriteLine(string.Format("{0}if({1})",Tap3,checkName));
                                writer.WriteLine(Tap3Left);
                                writer.WriteLine(string.Format("{0}writer.Write({1},(int){2});", Tap4, field.FieldNumber, proptertyName));
                                writer.WriteLine(Tap3Right);
                                break;
                            case RequiredType.Repeated:
                                writer.WriteLine(string.Format("{0}foreach({1} v in {2})",Tap3,field.DataName,proptertyName));
                                writer.WriteLine(Tap3Left);
                                writer.WriteLine(string.Format("{0}writer.Write({1},(int)v);", Tap4, field.FieldNumber));
                                writer.WriteLine(Tap3Right);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        break;

                    case DataType.Class:
                       
                    case DataType.Bool:
                        
                    case DataType.Int:
                        
                    case DataType.Long:
                        
                    case DataType.Float:
                        
                    case DataType.Double:
                        
                    case DataType.String:
                        
                    case DataType.Bytes:
                        switch (field.RequiredType)
                        {
                            case RequiredType.Required:
                                writer.WriteLine(string.Format("{0}writer.Write({1},{2});", Tap3, field.FieldNumber, proptertyName));
                                break;
                            case RequiredType.Optional:
                                writer.WriteLine(string.Format("{0}if({1})",Tap3,checkName));
                                writer.WriteLine(Tap3Left);
                                writer.WriteLine(string.Format("{0}writer.Write({1},{2});", Tap4, field.FieldNumber, proptertyName));
                                writer.WriteLine(Tap3Right);
                                break;
                            case RequiredType.Repeated:
                                writer.WriteLine(string.Format("{0}foreach({1} v in {2})", Tap3, field.DataName, proptertyName));
                                writer.WriteLine(Tap3Left);
                                writer.WriteLine(string.Format("{0}writer.Write({1},v);", Tap4, field.FieldNumber));
                                writer.WriteLine(Tap3Right);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            writer.WriteLine(string.Format("{0}return writer.GetProtoBufferBytes();",Tap3));
            writer.WriteLine(Tap2Right);
        }
        /// <summary>
        /// c#写入ParseFrom
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="message"></param>
        private void WriteReceiveableCShape(StreamWriter writer, ProtoBufferMessage message)
        {
            writer.WriteLine(string.Format("{0}public void ParseFrom(byte[] buffer)", Tap2));
            writer.WriteLine(Tap2Left);
            writer.WriteLine(string.Format("{0} ParseFrom(buffer, 0, buffer.Length);",Tap3));
            writer.WriteLine(Tap2Right);

            writer.WriteLine(string.Format("{0}public void ParseFrom(byte[] buffer, int offset, int size)",Tap2));
            writer.WriteLine(Tap2Left);
            writer.WriteLine(string.Format("{0}if (buffer == null) return;",Tap3));
            writer.WriteLine(string.Format("{0}ProtoBufferReader reader = new ProtoBufferReader(buffer,offset,size);",Tap3));
            writer.WriteLine(string.Format("{0}foreach (ProtoBufferObject obj in reader.ProtoBufferObjs)",Tap3));
            writer.WriteLine(Tap3Left);//begin forach
            writer.WriteLine(string.Format("{0}switch (obj.FieldNumber)", Tap4));
            writer.WriteLine(Tap4Left);//begin switch
            foreach (ProtoBufferField field in message.Fields)
            {
                string propertyName = PropertyName(field.Name, field.RequiredType);
                string memberName = MemberName(field.Name, field.RequiredType).Replace("List","");
                writer.WriteLine(string.Format("{0}case {1}:", Tap5, field.FieldNumber));
                switch (field.RequiredType)
                {
                    case RequiredType.Required:
                    case RequiredType.Optional:

                        switch (field.DataType)
                        {
                            case DataType.Class:
                                writer.WriteLine(string.Format("{0}{1} = new {2}();", Tap6, propertyName,field.DataName));
                                writer.WriteLine(string.Format("{0}{1}.ParseFrom(obj.Value);",Tap6,propertyName));
                                break;
                           
                            case DataType.Bool:                             
                            case DataType.Int:
                            case DataType.Long:
                            case DataType.Float:
                            case DataType.Double:
                            case DataType.String:  
                            case DataType.Bytes:
                                writer.WriteLine(string.Format("{0}{1} = obj.Value;", Tap6, propertyName));
                                break;
                            case DataType.Enum:
                                writer.WriteLine(string.Format("{0}{1} = ({2})((int)obj.Value);", Tap6, propertyName, field.DataName));
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        break;
                    case RequiredType.Repeated:
                        switch (field.DataType)
                        {
                            case DataType.Class:
                                writer.WriteLine(string.Format("{0} var {1}= new {2}();", Tap6,memberName,field.DataName));
                                writer.WriteLine(string.Format("{0}{1}.ParseFrom(obj.Value);", Tap6, memberName));
                                writer.WriteLine(string.Format("{0}{1}.Add({2});",Tap6,propertyName,memberName));
                                break;
                            case DataType.Bool:
                            case DataType.Int:
                            case DataType.Long:
                            case DataType.Float:
                            case DataType.Double:
                            case DataType.String:
                            case DataType.Bytes:
                                writer.WriteLine(string.Format("{0}{1}.Add(obj.Value);", Tap6, propertyName));
                                break;
                            case DataType.Enum:
                                writer.WriteLine(string.Format("{0}{1}.Add(({2})((int)obj.Value));", Tap6, propertyName, field.DataName));
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                writer.WriteLine(string.Format(string.Format("{0}break;", Tap6)));
            }
            writer.WriteLine(string.Format("{0}default:", Tap5));
            writer.WriteLine(string.Format(string.Format("{0}break;", Tap6)));
            writer.WriteLine(Tap4Right);//end switch
            writer.WriteLine(Tap3Right);//end forach
            if (NeedCheckRequired)
            {
                writer.WriteLine(string.Format("{0}CheckRequiredFields();", Tap3));
            }
            writer.WriteLine(Tap2Right);
        }
        /// <summary>
        /// c# 写入ToString
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="message"></param>
        private void Write2StringCShape(StreamWriter writer, ProtoBufferMessage message)
        {
            writer.WriteLine(string.Format("{0}public override string ToString()", Tap2));
            writer.WriteLine(Tap2Left);
            writer.WriteLine(string.Format("{0}StringBuilder sb = new StringBuilder();", Tap3));
            writer.WriteLine(string.Format("{0}sb.Append", Tap3) + "(\"{\");");
            for (int i = 0; i < message.Fields.Count; i++)
            {
                ProtoBufferField field = message.Fields[i];
                string checkName = CheckRequiredName(field.Name, field.RequiredType);
                string propertyName = PropertyName(field.Name, field.RequiredType);
                if (i == message.Fields.Count - 1)
                {
                    switch (field.RequiredType)
                    {
                        case RequiredType.Required:
                            writer.WriteLine(string.Format("{0}sb.Append(\"{1} : \" + {1});", Tap3, propertyName));
                            break;
                        case RequiredType.Optional:
                            writer.WriteLine(string.Format("{0}if({1})", Tap3, checkName));
                            writer.WriteLine(Tap3Left);
                            writer.WriteLine(string.Format("{0}sb.Append(\"{1} : \" + {1});", Tap4, propertyName));
                            writer.WriteLine(Tap3Right);
                            break;
                        case RequiredType.Repeated:
                            writer.WriteLine(string.Format("{0}sb.Append(\"{1} : [\");", Tap3, propertyName));

                            writer.WriteLine(string.Format("{0}for(int i = 0; i < {1}.Count;i ++)", Tap3, propertyName));
                            writer.WriteLine(Tap3Left);
                            writer.WriteLine(string.Format("{0}if(i == {1}.Count -1)", Tap4, propertyName));
                            writer.WriteLine(Tap4Left);
                            writer.WriteLine(string.Format("{0}sb.Append({1}[i]);", Tap5, propertyName));
                            writer.WriteLine(string.Format("{0}else", Tap4Right));
                            writer.WriteLine(Tap4Left);
                            writer.WriteLine(string.Format("{0}sb.Append({1}[i] + \",\");", Tap5, propertyName));
                            writer.WriteLine(Tap4Right);
                            writer.WriteLine(Tap3Right);
                            writer.WriteLine(string.Format("{0}sb.Append(\"]\");", Tap3));
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                else
                {
                    switch (field.RequiredType)
                    {
                        case RequiredType.Required:
                            writer.WriteLine(string.Format("{0}sb.Append(\"{1} : \" + {1} + \",\");", Tap3, propertyName));
                            break;
                        case RequiredType.Optional:
                            writer.WriteLine(string.Format("{0}if({1})", Tap3, checkName));
                            writer.WriteLine(Tap3Left);
                            writer.WriteLine(string.Format("{0}sb.Append(\"{1} : \" + {1} +\",\");", Tap4, propertyName));
                            writer.WriteLine(Tap3Right);
                            break;
                        case RequiredType.Repeated:
                            writer.WriteLine(string.Format("{0}sb.Append(\"{1} : [\");", Tap3, propertyName));

                            writer.WriteLine(string.Format("{0}for(int i = 0; i < {1}.Count;i ++)", Tap3, propertyName));
                            writer.WriteLine(Tap3Left);
                            writer.WriteLine(string.Format("{0}if(i == {1}.Count -1)", Tap4, propertyName));
                            writer.WriteLine(Tap4Left);
                            writer.WriteLine(string.Format("{0}sb.Append({1}[i]);", Tap5, propertyName));
                            writer.WriteLine(string.Format("{0}else", Tap4Right));
                            writer.WriteLine(Tap4Left);
                            writer.WriteLine(string.Format("{0}sb.Append({1}[i] + \",\");", Tap5, propertyName));
                            writer.WriteLine(Tap4Right);
                            writer.WriteLine(Tap3Right);
                            writer.WriteLine(string.Format("{0}sb.Append(\"],\");", Tap3));
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            writer.WriteLine(string.Format("{0}sb.Append", Tap3) + "(\"}\");");
            writer.WriteLine(string.Format("{0}return sb.ToString();", Tap3));
            writer.WriteLine(Tap2Right);
        }

        /// <summary>
        /// 根据语言种类得到命名空间
        /// </summary>
        /// <param name="nameSpace"></param>
        /// <returns></returns>
        private string Namespace(string nameSpace)
        {
            string result = null;
            switch (LanguageType)
            {
                case LanguageType.CShape:
                    string[] strings = nameSpace.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                    StringBuilder sb = new StringBuilder();

                    for (int i = 0; i < strings.Length; i++)
                    {
                        if (i == strings.Length - 1)
                        {
                            sb.Append(StringUtil.ToFirstUpper(strings[i]));
                        }
                        else
                        {
                            sb.Append(StringUtil.ToFirstUpper(strings[i])).Append(".");
                        }
                    }
                    result = sb.ToString();
                    break;
                case LanguageType.Java:
                    result = nameSpace;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return result;
        }

        /// <summary>
        /// 得到检查required的名称
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string CheckRequiredName(string name,RequiredType requiredType)
        {
            string result = null;
            switch (LanguageType)
            {
                case LanguageType.CShape:
                    result = "Has" + PropertyName(name,requiredType);
                    break;
                case LanguageType.Java:
                    result = "has" + PropertyName(name,requiredType);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return result;
        }
        /// <summary>
        /// 成员名称。采用驼峰格式
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string MemberName(string name,RequiredType requiredType)
        {
            string[] strings = name.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < strings.Length; i++)
            {
                if (i == 0)
                {
                    sb.Append(strings[i]);
                }
                else
                {
                    sb.Append(StringUtil.ToFirstUpper(strings[i]));
                }

            }

            if (requiredType == RequiredType.Repeated)
            {
                sb.Append("List");
            }
            return sb.ToString();
        }




        /// <summary>
        /// 属性名称，仅仅用于c#
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string PropertyName(string name,RequiredType requiredType)
        {

            string[] strings = name.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < strings.Length; i++)
            {
                sb.Append(StringUtil.ToFirstUpper(strings[i]));
            }
            if (requiredType == RequiredType.Repeated)
            {
                sb.Append("List");
            }
            return sb.ToString();

        }







    }
}
