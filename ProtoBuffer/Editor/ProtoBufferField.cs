using System;
using System.Collections.Generic;
using System.Text;

namespace ProtoBuffer
{
    public class ProtoBufferField
    {

        /// <summary>
        /// 必须的类型：
        /// Required
        /// Optional
        /// Repeated
        /// </summary>
        public RequiredType RequiredType { get; private set; }

        /// <summary>
        /// 数据类型：
        /// Null,
        /// Class,
        /// Enum,
        /// Bool,
        /// Int,
        /// Long,
        /// Float,
        /// Double,
        /// String,
        /// Bytes  
        /// </summary>
        public DataType DataType { get; private set; }

        /// <summary>
        /// 数据类型名称
        /// class name
        /// enum name
        /// bool
        /// int
        /// long 
        /// float
        /// double
        /// string
        /// byte[]
        /// </summary>
        public string DataName { get; private set; }

        /// <summary>
        /// 数据名称
        /// </summary>
        public string Name { get; private  set; }

        /// <summary>
        /// 域标志数
        /// </summary>
        public int FieldNumber { get; private set; }

        public List<string> Summarys { get; private set; }

        public ProtoBufferMessage Message { get; private set; }

        public List<Line> Lines { get; private set; } 

        public ProtoBufferField(ProtoBufferMessage msg,List<Line> lines)
        {
            Summarys = new List<string>();
            Message = msg;
            Lines = lines;
            FieldNumber = 0;
        }
        
        internal void Parse()
        {
            Summarys.Clear();
            List<Line> list = new List<Line>();
            foreach (Line line in Lines)
            {
                if (line.Content.StartsWith("//"))
                {
                    if (line.Content.Length > 2)
                    {
                        Summarys.Add(line.Content.Substring(2).Trim());
                    }
                }
                else
                {
                    list.Add(line);
                }
            }
            ParseInfos(list);

            if (FieldNumber == 0)
            {
                throw new ProtoBufferException(string.Format("在文件名{0}，message:{1},FieldNumber解析错误。",Message.File.FileName,Message.Name));                    
                
            }
            foreach (ProtoBufferField field in Message.Fields)
            {
                if (Equals(field))
                {
                    continue;
                }
                if (field.FieldNumber == FieldNumber)
                {
                    throw new ProtoBufferException(string.Format("在文件名{0}，message:{1},FieldNumber:{2}重复。{3}",Message.File.FileName,Message.Name,FieldNumber,list[0].ToString()));                    
                }
                if (Name.Equals(field.Name))
                {
                    throw new ProtoBufferException(string.Format("在文件名{0}，message:{1},名称{2}重复,FieldNumber:{3}。{4}", Message.File.FileName, Message.Name, Name, FieldNumber,list[0].ToString()));
                }
            }
            
        }
        private void ParseInfos(List<Line> lines)
        {
            if (lines.Count != 1)
            {
                throw new ProtoBufferException("解析field出错。超过一行有效的field");
            }
            Line line = lines[0];

            if (Message.DataType == DataType.Class)
            {
                if (!line.Content.Contains("="))
                {
                    throw new ProtoBufferException(string.Format("解析field出现异常,缺少=:{0}", line.ToString()));
                }

                string[] strings = line.Content.Split(new string[] {" ",";","="}, StringSplitOptions.RemoveEmptyEntries);
                
                if (strings.Length < 4)
                {
                    throw new ProtoBufferException(string.Format("解析field出现异常：{0}", line.ToString()));
                }
                

                string s = strings[0];
                if (s.Equals("required"))
                {
                    RequiredType = RequiredType.Required;
                }
                else if(s.Equals("optional"))
                {
                    RequiredType = RequiredType.Optional;
                }
                else if(s.Equals("repeated"))
                {
                    RequiredType = RequiredType.Repeated;
                }
                else
                {
                    throw new ProtoBufferException(string.Format("解析field出错，缺少关键词required,optional 或者 repeated:{0}",line.ToString()));
                }
                
                s = strings[1];

                if (s.Equals("int32"))
                {
                    DataType = DataType.Int;
                    DataName = "int";
                }
                else if(s.Equals("int64"))
                {
                    DataType = DataType.Long;
                    DataName = "long";
                }
                else if(s.Equals("bool"))
                {
                    DataType = DataType.Bool;
                    DataName = "bool";
                }
                else if(s.Equals("float"))
                {
                    DataType = DataType.Float;
                    DataName = "float";
                }
                else if(s.Equals("double"))
                {
                    DataType = DataType.Double;
                    DataName = "double";
                }
                else if(s.Equals("string"))
                {
                    DataType = DataType.String;
                    DataName = "string";
                }
                else if(s.Equals("bytes"))
                {
                    DataType = DataType.Bytes;
                    DataName = "byte[]";
                }
                else
                {
                    DataType = FindDataType(s, line);
                    if (DataType == DataType.Class || DataType == DataType.Enum)
                    {
                        DataName = s;
                    }
                    else
                    {
                        throw new ProtoBufferException(string.Format("不能正确解析field:{0}",line.ToString()));
                    }
                    
                }

                Name = strings[2];

                if (KeyWord.IsKeyWord(Name))
                {
                    throw new ProtoBufferException(string.Format("含有c#关键字:{0}",line.ToString()));
                }
                int fieldNumer;
                if (!int.TryParse(strings[3], out fieldNumer))
                {
                    throw new ProtoBufferException(string.Format("解析field出现异常,fieldNumber异常：{0}", line.ToString()));
                }
                FieldNumber = fieldNumer;

            }
            else if(Message.DataType == DataType.Enum)
            {
                if (!line.Content.Contains("="))
                {
                    throw new ProtoBufferException(string.Format("解析field出现异常,缺少=:{0}", line.ToString()));
                }
                string[] strings = line.Content.Split(new string[] {" ",";","="}, StringSplitOptions.RemoveEmptyEntries);
                if (strings.Length < 2)
                {
                    throw new ProtoBufferException(string.Format("解析field出现异常：{0}",line.ToString()));
                }
                
                int fieldNumer;
                if (! int.TryParse(strings[1],out fieldNumer))
                {
                    throw new ProtoBufferException(string.Format("解析field出现异常,fieldNumber异常：{0}", line.ToString()));
                }
                RequiredType = RequiredType.Required;
                DataType = DataType.Enum;
                DataName = "enum";
                Name = strings[0];
                if (KeyWord.IsKeyWord(Name))
                {
                    throw new ProtoBufferException(string.Format("含有c#关键字:{0}", line.ToString()));
                }
                FieldNumber = fieldNumer;
            }
            else
            {
                throw new ProtoBufferException("Message的DataType出现异常");
            }

            if (Message.DataType == DataType.Enum)
            {
                return;
            }
            //如果是class就要对命名进行检测
            for (int i = 0; i < Name.Length; i++)
            {
                if (i == 0)
                {
                    if (!Char.IsLower(Name[i]))
                    {
                        throw new ProtoBufferException(string.Format("field名称不正确，首字母必须是小写。{0}", line.ToString()));
                    }
                }
                else
                {
                    if (Char.IsLower(Name[i]) || Char.IsDigit(Name[i]) || Name[i].Equals('_'))
                    {
                    }
                    else
                    {
                        throw new ProtoBufferException(string.Format("field名称不正确，出现非法字符。{0}", line.ToString()));
                    }
                }
            }
        }

        private DataType FindDataType(string name,Line line)
        {
            DataType result = DataType.Null;
            //现在这个文件（ProtoBufferFile）寻找
            result = FindDataTypeInFile(line,name, Message.File);

            if (result == DataType.Null)
            {
                
            }
            else
            {
                return result;
            }
            //在该文件的依赖文件中寻找
            foreach (ProtoBufferFile file in Message.File.Import)
            {
                result = FindDataTypeInFile(line,name, file);
                if (result == DataType.Null)
                {

                }
                else
                {
                    //添加到命名dependent

                    if (!file.Equals(Message.File))
                    {
                        if (!Message.Dependents.Contains(file))
                        {
                            Message.Dependents.Add(file);
                        }
                    }
                   
                    break;
                }
            }
            if (result == DataType.Null)
            {
                throw new ProtoBufferException(string.Format("在{0}无法找到类型名称{1}",line.ToString(),name));
            }
            return result;
        }

        private DataType FindDataTypeInFile(Line line, string name, ProtoBufferFile file)
        {
            DataType result = DataType.Null;
            foreach (ProtoBufferMessage msg in file.Messages)
            {
                if (msg.Name.Equals(name))
                {
                    result = msg.DataType;
                    
#if DEBUG

                    Console.WriteLine(string.Format("文件名：{0}中的第{1}行：{2}在文件{3}中找到对应类型：{4}",line.File.FileName,line.LineNumber,name,file.FileName,result == DataType.Class ?"message":"enum"));
#endif
                    break;
                }
            }
            return result;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            sb.Append(string.Format("RequiredType = {0},DataType = {1},DataName = {2},Name = {3},FieldNumber = {4},",RequiredType.ToString(),DataType.ToString(),DataName,Name,FieldNumber));

            sb.Append("Summarys:[");

            for (int i = 0; i < Summarys.Count; i++)
            {
                if (i == Summarys.Count -1)
                {
                    sb.Append(Summarys[i]);
                }
                else
                {
                    sb.Append(Summarys[i]).Append(",");
                }
            }
            sb.Append("]");
            sb.Append("}");
            return sb.ToString();
        }
    }
}
