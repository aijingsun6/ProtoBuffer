using System;
using System.Collections.Generic;
using System.Text;

namespace ProtoBuffer
{
    public class ProtoBufferMessage
    {
        /// <summary>
        /// 依赖条件(命名空间)
        /// </summary>
        public List<ProtoBufferFile> Dependents { get; private set; }
        /// <summary>
        /// 注释
        /// </summary>
        public List<string> Summarys { get; private set; } 
        /// <summary>
        /// message 名称。
        /// message可以是class 也可以是enum
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 是否检查无效行
        /// </summary>
        public bool NeedUseLessLineCheck
        {
            get { return File.Dic.NeedUseLessLineCheck; }
        }

        private DataType _dataType;

        /// <summary>
        /// 类型。
        /// 可以是class 也可以是 enum
        /// </summary>
        public DataType DataType
        {
            get { return _dataType; }

            private set
            {
                if (value == DataType.Class || value == DataType.Enum)
                {
                    _dataType = value;
                }
                else
                {
                    throw new ProtoBufferException("数据类型只能是enum或者是class");
                }
            }
        }
        /// <summary>
        /// 文件
        /// </summary>
        public ProtoBufferFile File { get; private set; }

        
        public List<ProtoBufferField> Fields { get;private set; }

        public List<Line> Lines { get; private set; }
        /// <summary>
        /// 命名空间
        /// </summary>
        public string NameSpace
        {
            get { return File.NameSpace; }
        }

        public ProtoBufferMessage(ProtoBufferFile file,List<Line> lines)
        {
            Dependents = new List<ProtoBufferFile>();
            Summarys = new List<string>();
            Fields = new List<ProtoBufferField>();
            File = file;
            Lines = lines;
        }
        
        /// <summary>
        /// 不包含parse field
        /// </summary>
        internal void Parse()
        {
            Dependents.Clear();
            Summarys.Clear();
            Fields.Clear();
            List<List<Line>> fields = ParseFieldLine(ParseMsgElement());

            foreach (List<Line> list in fields)
            {
                ProtoBufferField field = new ProtoBufferField(this,list);
                Fields.Add(field);
            }
        }

        

        /// <summary>
        /// 移除msg想关信息，只留下field相关信息
        /// </summary>
        /// <returns></returns>
        private List<Line> ParseMsgElement()
        {
            List<Line> lines = new List<Line>();

            bool hasBegin = false;
            foreach (Line line in Lines)
            {
                if (hasBegin)
                {
                    
                    if (line.Content.StartsWith("}"))
                    {
                        hasBegin = false;
                    }
                    else
                    {
                        lines.Add(line);
                    }
                }
                else
                {
                    if (line.Content.StartsWith("//"))
                    {
                        if (line.Content.Length > 2)
                        {
                            Summarys.Add(line.Content.Substring(2).Trim());
                        }
                    }

                    if (line.Content.Contains("{"))
                    {
                        //msg type and begin
                        string[] strings = line.Content.Split(new string[] {" ","{"}, StringSplitOptions.RemoveEmptyEntries);
                        if (strings.Length != 2)
                        {
                            throw new ProtoBufferException(string.Format("message 解析出错：{0}",line.ToString()));
                        }
                        if (strings[0].Equals("message"))
                        {
                            DataType = DataType.Class;
                            Name = strings[1];
                            hasBegin = true;
                        }
                        else if(strings[0].Equals("enum"))
                        {
                            DataType = DataType.Enum;
                            Name = strings[1];
                            hasBegin = true;
                        }
                        else
                        {
                            throw new ProtoBufferException(string.Format("message 解析出错：{0}", line.ToString()));
                        }

                        for (int i = 0; i < Name.Length; i++)
                        {
                            if (i == 0)
                            {
                                if (!Char.IsUpper(Name[i]))
                                {
                                    throw new ProtoBufferException(string.Format("Message命名不正确，第一个字母必须是大写。{0}",line.ToString()));
                                }
                            }
                            else
                            {
                                if (!Char.IsLetter(Name[i]))
                                {

                                    throw new ProtoBufferException(string.Format("Message命名不正确，出现了非字母字符。{0}", line.ToString()));
                                }
                            }
                        }


                    }
                }

            }

            if (hasBegin)
            {
                throw new ProtoBufferException("message 只有开头没有结尾");
            }
            return lines;
        }

        private List<List<Line>> ParseFieldLine(List<Line> lines)
        {
            List<List<Line>> result = new List<List<Line>>();

            Stack<Line> stack = new Stack<Line>();

            foreach (Line line in lines)
            {

                if (string.IsNullOrEmpty(line.Content))
                {
                    continue;
                }
                if (line.Content.Contains("//"))
                {
                    stack.Push(line);
                    continue;
                }
                if (DataType == DataType.Enum || DataType == DataType.Class)
                {
                    List<Line> list = new List<Line>();
                    list.Add(line);
                    while (stack.Count > 0)
                    {
                        Line tmp = stack.Pop();
                        list.Insert(0, tmp);
                    }
                    result.Add(list);         
                }
                else
                {
                    throw new ProtoBufferException("还没有解析出message 的类型。");
                }
            }
            if (!NeedUseLessLineCheck)
            {
                return result;
            }
            if (stack.Count != 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("[");
                Line[] errorLine = stack.ToArray();

                for (int i = 0; i < errorLine.Length; i++)
                {
                    if (i == errorLine.Length - 1)
                    {
                        sb.Append(errorLine[i].ToString());
                    }
                    else
                    {
                        sb.Append(errorLine[i].ToString()).Append(",");
                    }
                }
                sb.Append("]");

                throw new ProtoBufferException(string.Format("文件{0}中无效的行数：{1}", File.FileName, sb.ToString()));

            }


            return result;
        } 
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");

            sb.Append("Dependents:[");

            for (int i = 0; i < Dependents.Count; i++)
            {
                if (i == Dependents.Count -1)
                {
                    sb.Append(Dependents[i].NameSpace);
                }
                else
                {
                    sb.Append(Dependents[i].NameSpace).Append(",");
                }
            }
            sb.Append("]");

            sb.Append(string.Format(",NameSpace:{0},",File.NameSpace));

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
            sb.Append(string.Format(",Name:{0},DataType:{1}", Name,DataType.ToString()));
            sb.Append(",Fields:[");

            for (int i = 0; i < Fields.Count; i++)
            {
                if (i == Fields.Count -1)
                {
                    sb.Append(Fields[i]);
                }
                else
                {
                    sb.Append(Fields[i]).Append(",");
                }
            }
            sb.Append("]");
            sb.Append("}");
            return sb.ToString();
        }

    }
}
