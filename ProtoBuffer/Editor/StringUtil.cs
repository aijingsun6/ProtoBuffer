using System;
using System.Collections.Generic;
using System.Text;

namespace ProtoBuffer
{
    public class StringUtil
    {
        /// <summary>
        /// 将字符串的第一个字符变成大写
        /// 注意：
        /// <ol>
        /// <li>字符串不能为空或者空字符串</li>
        /// <li>字符串中只能含有字母和数字</li>
        /// <li>第一个字符不能是数字</li>
        /// </ol>
        /// </summary>
        /// <param name="content"></param>
        /// <exception cref="System.Exception">转化的时候出现异常</exception>
        /// 
        /// <returns></returns>
        public static string ToFirstUpper(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                throw new Exception("字符串不能为空或者空字符串");
            }
            foreach (char c in content)
            {
                if (!Char.IsLetterOrDigit(c))
                {
                    throw new Exception("字符串中存在非字母或者是数字");
                }
                
            }

            if (Char.IsDigit(content[0]))
            {
                throw new Exception("字符串中第一个字符是数字");
            }
            
            int length = content.Length;
            if (length > 1)
            {
                return content.Substring(0, 1).ToUpper() + content.Substring(1);
            }
            else
            {
                return content.ToUpper();
            }
        }

    }
}
