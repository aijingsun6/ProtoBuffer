#define DEBUG
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ProtoBuffer
{
    /// <summary>
    /// 目录，含有很多的文件
    /// 
    /// 
    /// 2013-12-13 添加了
    /// </summary>
    public class ProtoBufferDic
    {

        public string Path { get; private set; }

        public List<ProtoBufferFile> Files { get; private set; }
        
        public Dictionary<string, Dictionary<string, ProtoBufferMessage>> MessagesDic { get; private set; }

        public List<ProtoBufferMessage> Messages { get; private set; }
        /// <summary>
        /// 设置或得到输出的命名空间。
        /// 如果设置了，那么parse出来的所有message的命名空间将会更改为所设置的命名空间
        /// </summary>
        public string OutNameSpace { get; set; }

        /// <summary>
        /// 是否检查无效行
        /// </summary>
        public bool NeedUseLessLineCheck { get; set; }

        /// <summary>
        /// 得到message
        /// </summary>
        /// <param name="nameSpace"></param>
        /// <param name="name"></param>
        /// <returns>
        /// 如果不存在返回null
        /// </returns>
        public ProtoBufferMessage Message(string nameSpace, string name)
        {

            if (MessagesDic.ContainsKey(nameSpace))
            {
                if (MessagesDic[nameSpace].ContainsKey(name))
                {
                    return MessagesDic[nameSpace][name];
                }
            }
            return null;
        }
        public ProtoBufferDic()
        {
            Files = new List<ProtoBufferFile>();

            MessagesDic = new Dictionary<string, Dictionary<string, ProtoBufferMessage>>();

            Messages = new List<ProtoBufferMessage>();

        }

        public ProtoBufferDic(string path):this()
        {
            Path = path;
            if (!Directory.Exists(path))
            {
                throw new ProtoBufferException(string.Format("找不到文件夹：{0}",path));
            }
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            FileInfo[] fileInfos = directoryInfo.GetFiles();
            foreach (FileInfo fileInfo in fileInfos)
            {
                if (fileInfo.Extension.Equals(".proto"))
                {
                    using (FileStream fs = new FileStream(fileInfo.FullName, FileMode.Open))
                    {
                        try
                        {
                            using (StreamReader reader = new StreamReader(fs))
                            {
                                try
                                {
                                    List<string> list = new List<string>();
                                    while (!reader.EndOfStream)
                                    {
                                        list.Add(reader.ReadLine());
                                    }
                                    ProtoBufferFile file = new ProtoBufferFile(fileInfo.FullName,this,list);
                                    AddFile(file);
                                }
                                catch (Exception)
                                {

                                    throw;
                                }
                                finally
                                {
                                    reader.Close();
                                }
                            } 

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
            }


        }



        /// <summary>
        /// 测试使用
        /// </summary>
        /// <param name="file"></param>
        public void AddFile(ProtoBufferFile file)
        {
            Files.Add(file);
        }

        public void Parse()
        {
            foreach (ProtoBufferFile file in Files)
            {
                file.ParseInfos();
            }

            foreach (ProtoBufferFile file in Files)
            {
                file.Parse2Message();
            }

            foreach (ProtoBufferFile file in Files)
            {        
                file.Parse2Field();
            }

            foreach (ProtoBufferFile file in Files)
            {
                string nameSpace = file.NameSpace;

                if (OutNameSpace != null)
                {
                    file.NameSpace = OutNameSpace;
                }

                if (!MessagesDic.ContainsKey(nameSpace))
                {
                    MessagesDic.Add(nameSpace,new Dictionary<string, ProtoBufferMessage>());
                }
                foreach (ProtoBufferMessage msg in file.Messages)
                {
                    if (MessagesDic[nameSpace].ContainsKey(msg.Name))
                    {
                        throw new ProtoBufferException(string.Format("该命名空间{0}已经存在{1}",nameSpace,msg.Name));
                    }
                    else
                    {
                        MessagesDic[nameSpace].Add(msg.Name,msg);
                        Messages.Add(msg);
                    }
                }
            }

        }
        


    }
}
