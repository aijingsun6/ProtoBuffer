using System;
using System.Collections.Generic;
using System.Text;

namespace ProtoBuffer
{
    /// <summary>
    /// 每一行ProtoBuffer语言
    /// </summary>
    public class Line
    {
        /// <summary>
        /// 行数
        /// </summary>
        public int LineNumber { get; private  set; }
        /// <summary>
        /// 实际的内容
        /// </summary>
        public string Content { get; private set; }
        /// <summary>
        /// 文件
        /// </summary>
        public ProtoBufferFile File { get; private set; }

        public Line(ProtoBufferFile file,int lineNumber,string content)
        {
            File = file;
            LineNumber = lineNumber;
            Content = content.Trim();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            
            sb.Append("{")
              .Append("FileName:").Append(File.FileName).Append(",")
              .Append("LineNumber:").Append(LineNumber).Append(",")
              .Append("Content:\"").Append(Content).Append("\"")
              .Append("}");

            return sb.ToString();
        }
    }
}
