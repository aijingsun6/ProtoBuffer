using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ProtoBuffer
{
    /// <summary>
    /// <para>使用方法</para>
    /// <ol>
    /// <li>ParseInfos()</li>
    /// <li>Parse2Message()</li>
    /// <li>Parse2Field()</li>
    /// </ol>
    /// </summary>
    /// <seealso cref="ParseInfos()"/>
    /// <seealso cref="Parse2Message()"/>
    /// <seealso cref="Parse2Field()"/>
    /// 
    public class ProtoBufferFile
    {
        /// <summary>
        /// 文件名必须是以.proto结尾
        /// </summary>
        public string FileName { get; private set; }
        /// <summary>
        /// 命名空间
        /// </summary>
        public string NameSpace { get; set; }

        
        public bool NeedUseLessLineCheck
        {
            get { return Dic.NeedUseLessLineCheck; }
        }

        /// <summary>
        /// 依赖的文件
        /// </summary>
        public List<ProtoBufferFile> Import { get; private set; }
        /// <summary>
        /// 附加的属性，尚未使用
        /// </summary>
        public List<KeyValuePair<string,string>> Option { get; private set; } 

        public List<Line> Lines { get; private set; }

        public List<ProtoBufferMessage> Messages { get; private set; }

        public ProtoBufferDic Dic { get; private set; }

        public ProtoBufferFile(string fileName,ProtoBufferDic dic)
        {
            Messages = new List<ProtoBufferMessage>();
            Lines = new List<Line>();
            Import = new List<ProtoBufferFile>();
            Option = new List<KeyValuePair<string, string>>();
            if (!fileName.EndsWith(".proto"))
            {
                throw new ProtoBufferException(string.Format("文件名称错误,没有以.proto结尾。{0},",fileName));
            }
            FileName = fileName;
            Dic = dic;
        }

        public ProtoBufferFile(string fileName,ProtoBufferDic dic,IEnumerable<string> content) : this(fileName,dic)
        {
            int lineNumber = 1;            
            foreach (string str in content)
            {
                string trimed = str.Trim();
                Line line = new Line(this, lineNumber, trimed);
                Lines.Add(line);
                lineNumber++;
            }
            
        }
        #region 解析命名空间，外部依赖条件，额外的参数
        /// <summary>
        /// 解析命名空间(NameSpace)
        /// </summary>
        private void ParseNameSpace(Line line)
        {
            string[] strings = line.Content.Split(new string[] { " ", ";" }, StringSplitOptions.RemoveEmptyEntries);
            if (strings.Length > 1 && strings[0].Equals("package"))
            {
                if (NameSpace != null)
                {
                    throw new ProtoBufferException(string.Format("重复命名空间：{0}",line.ToString()));
                }

                //命名空间包含非法字符

                string nameSpace = strings[1].Trim();

                foreach (char c in nameSpace)
                {
                    if (Char.IsLower(c) || c.Equals('.'))
                    {
                        
                    }
                    else
                    {
                        throw new ProtoBufferException(string.Format("命名空间只能含有小写字母或者.{0}",line.ToString()));
                    }
                }

                NameSpace = nameSpace;
            }
            else
            {
                throw new ProtoBufferException(string.Format("不能解析命名空间：{0}", line.ToString()));
            }
                  
        }
        /// <summary>
        /// 解析外部依赖条件(Import)
        /// </summary>
        private void ParseImport(Line line)
        {

            string[] strs = line.Content.Split(new string[] { " ", "\"", ";" }, StringSplitOptions.RemoveEmptyEntries);
            if (strs.Length > 1 && strs[0].Equals("import"))
            {
                string import = strs[1];
                if (import.Equals(FileName))
                {
                    throw new ProtoBufferException(string.Format("不能添加自己作为依赖条件。{0}",line.ToString()));
                }

                bool find = false;
                foreach (ProtoBufferFile file in Dic.Files)
                {
                    if (file.FileName.EndsWith(import))
                    {
                        Import.Add(file);
                        find = true;
                        break;
                    }
                }

                if (!find)
                {
                    throw new ProtoBufferException(string.Format("添加外部依赖条件错误：{0}",line.ToString()));
                }
            }
            else
            {
                throw new ProtoBufferException(string.Format("不能解析依赖条件：{0}", line.ToString()));
            }

               
        }
        /// <summary>
        /// 解析额外的参数(Option)
        /// </summary>
        private void ParseOption(Line line)
        {

            string[] strs = line.Content.Split(new string[] { " ", "=", "\"", ";" },
                                               StringSplitOptions.RemoveEmptyEntries);

            if (strs.Length > 2 && strs[0].Equals("option"))
            {
                foreach (KeyValuePair<string, string> keyValuePair in Option)
                {
                    if (keyValuePair.Key.Equals(strs[1]))
                    {
                        throw new ProtoBufferException(string.Format("不能重复添加额外参数：{0}",line));
                    }
                }

                Option.Add(new KeyValuePair<string, string>(strs[1].Trim(), strs[2].Trim()));
            }
            else
            {
                throw new ProtoBufferException(string.Format("不能解析额外条件：{0}", line.ToString()));
            }
               
        }
        #endregion
        
        /// <summary>
        /// 得到每个message 的实际内容。
        /// </summary>
        private List<List<Line>> ParseMessageLine()
        {
            List<List<Line>> result = new List<List<Line>>();
            Stack<Line> stack = new Stack<Line>();

            foreach (Line line in Lines)
            {
                if (string.IsNullOrEmpty(line.Content))
                {
                    continue;
                }
                if (line.Content.StartsWith("package"))
                {
                    
                    continue;
                }
                if (line.Content.StartsWith("import"))
                { 
                    continue;
                }

                if (line.Content.StartsWith("option") && !line.Content.StartsWith("optional"))
                {
                    continue;
                }
                if (line.Content.Contains("}"))
                {
                    if (stack.Count == 0)
                    {
                        throw new ProtoBufferException(string.Format("解析出错：{0}",line.ToString()));
                    }
                    if (line.Content.Length > 1)
                    {
                        throw new ProtoBufferException(string.Format("解析出错：{0}",line.ToString()));
                    }
                    //message end
                    List<Line> lines = new List<Line>();
                    lines.Add(line);

                    while (stack.Count > 0)
                    {
                        Line tmp = stack.Peek();
                        if (tmp.Content.Contains("{"))
                        {

                            if (tmp.Content.Contains("message") || tmp.Content.Contains("enum"))
                            {
                            }
                            else
                            {
                             
                                throw new ProtoBufferException(string.Format("语法不严格：{0}",tmp.ToString()));
                            }
                            //message start
                            lines.Insert(0,tmp);
                            stack.Pop();
                            //find summary

                            while (stack.Count > 0)
                            {
                                Line summary = stack.Peek();
                                if (summary.Content.StartsWith("//"))
                                {
                                    lines.Insert(0,summary);
                                    stack.Pop();
                                }
                                else
                                {
                                    break;
                                }
                            }
                            result.Add(lines);
                            break;


                        }
                        else
                        {
                            lines.Insert(0,tmp);
                            stack.Pop();
                        }
                        
                    }


                }
                else
                {
                    stack.Push(line);
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
                Line[] lines = stack.ToArray();

                for (int i = 0; i < lines.Length; i++)
                {
                    if (i == lines.Length -1)
                    {
                        sb.Append(lines[i].ToString());
                    }
                    else
                    {
                        sb.Append(lines[i].ToString()).Append(",");
                    }
                }
                sb.Append("]");

                throw new ProtoBufferException(string.Format("文件{0}中无效的行数：{1}",FileName,sb.ToString()));

            }
            return result;
        }

        /// <summary>
        /// 解析命名空间(namespace) 外部依赖条件(import)，额外属性(option)
        /// </summary>
        internal void ParseInfos()
        {
            foreach (Line line in Lines)
            {
                if (line.Content.StartsWith("package"))
                {
                    ParseNameSpace(line);
                    
                }
                if (line.Content.StartsWith("import"))
                {
                    ParseImport(line);
                    
                }

                if (line.Content.StartsWith("option") && !line.Content.StartsWith("optional"))
                {
                    ParseOption(line);
                    
                }
            }
        }

        /// <summary>
        /// 一直解析到 message 层。
        /// <para>
        /// 主要是建立message 的索引
        /// </para>
        /// </summary>
        internal void Parse2Message()
        {
            List<List<Line>> msgLines = ParseMessageLine();

            if (NameSpace == null)
            {
                throw new ProtoBufferException(string.Format("文件名：{0}没有命名空间。",FileName));
            }
            foreach (List<Line> msgLine in msgLines)
            {
                ProtoBufferMessage msg = new ProtoBufferMessage(this,msgLine);
                msg.Parse();

                foreach (ProtoBufferMessage v in Messages)
                {
                    if (msg.Name.Equals(v.Name))
                    {
                        throw new ProtoBufferException(string.Format("同一个文件出现相同名称的message:{0}",msg.Name));
                        
                    }
                }
                Messages.Add(msg);
            }

        }

        /// <summary>
        /// 对每个field进行解析。
        /// 
        /// </summary>
        internal void Parse2Field()
        {
            foreach (ProtoBufferMessage message in Messages)
            {
                foreach (ProtoBufferField field in message.Fields)
                {
                    field.Parse();
                }

            }
        }


        

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{")
              .Append("Path:").Append(Dic == null ? "" : Dic.Path).Append(",")
              .Append("FileName:").Append(FileName).Append(",")
              .Append("NameSpace:").Append(NameSpace).Append(",")
              .Append("Import:[");
            for (int i = 0; i < Import.Count; i++)
            {
                if (i == Import.Count -1)
                {
                    sb.Append(Import[i].FileName);
                }
                else
                {
                    sb.Append(Import[i].FileName).Append(",");
                }
            }
            sb.Append("],");

            sb.Append("Option:[");
            for (int i = 0; i < Option.Count; i++)
            {
                if (i == Option.Count -1)
                {
                    sb.Append(string.Format("({0}:{1})", Option[i].Key, Option[i].Value));
                }
                else
                {
                    sb.Append(string.Format("({0}:{1}),", Option[i].Key, Option[i].Value));
                }    

            }
            sb.Append("],");

            sb.Append("Message:[");
            for (int i = 0; i < Messages.Count; i++)
            {
                if (i == Messages.Count -1)
                {
                    sb.Append(Messages[i].ToString());
                }
                else
                {
                    sb.Append(Messages[i].ToString()).Append(",");
                }
            }
            sb.Append("]");
            sb.Append("}");
            return sb.ToString();
        }
    }
}
