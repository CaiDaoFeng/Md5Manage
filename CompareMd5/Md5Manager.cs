using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Json;

namespace CompareMd5
{
    public class Md5Manager
    {
        //程序所处路径
        public static string path = System.AppDomain.CurrentDomain.BaseDirectory;
        //程序上一级路径
        //public static string path = new DirectoryInfo("../").FullName;
        //MD5 JSON文件
        public static string filenamepath = path + "\\" + "md5data.json";
        //MD5 JSON文件 对象
        public static FileInfo md5fileInfo = new FileInfo(filenamepath);
        //日志文件列表
        public static List<string> loglist = new List<string>();
        //是否存在MD5 JSON文件
        public bool InitMd5Data()
        {
            bool isexists = false;
            try
            {
                if (Directory.Exists(path))
                {
                    if (File.Exists(filenamepath))
                    {
                        isexists = true;
                    }
                }
            }
            catch (Exception e)
            {
                loglist.Add("读取json文件异常" + e.Message);
                return isexists;
            }
            return isexists;
        }


        /// <summary>
        /// 获取路径下所有文件以及子文件夹中文件
        /// </summary>
        /// <param name="path">全路径根目录</param>
        /// <param name="FileList">存放所有文件集合</param>
        /// <param name="RelativePath"></param>
        /// <returns></returns>
        public List<FileInfo> GetFile(string path, List<FileInfo> listFiles)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(path);
                FileInfo[] fil = dir.GetFiles();
                //DirectoryInfo[] dii = dir.GetDirectories();
                foreach (FileInfo f in fil)
                {
                    listFiles.Add(f);//添加文件路径到列表中
                }
                //获取子文件夹内的文件列表，递归遍历
                //foreach (DirectoryInfo d in dii)
                //{
                //    GetFile(d.FullName, listFiles);
                //}
            }
            catch (Exception e)
            {
                loglist.Add("获取所有文件信息异常" + e.Message);
                throw;
            }
            return listFiles;
        }
        /// <summary>
        /// 写入实体数据
        /// </summary>
        /// <returns></returns>
        public List<Datamodel> Setfiledata()
        {
            List<FileInfo> filelist = new List<FileInfo>();
            List<Datamodel> datalist = new List<Datamodel>();
            try
            {
                //获得所有文件
                filelist = this.GetFile(path, filelist);
                if (md5fileInfo != null)
                {
                    filelist.Remove(filelist.Where(p => p.FullName == md5fileInfo.FullName).FirstOrDefault());
                }
                if (filelist.Count == 0)
                {
                    return null;
                }
                foreach (var item in filelist)
                {
                    Datamodel datamodel = new Datamodel();
                    //文件名称
                    datamodel.FileName = item.Name;
                    //加入日志列表
                    loglist.Add("比对" + item.Name);
                    //MD5值
                    datamodel.MD5Value = this.GetMD5HashFromFile(item.FullName);
                    //版本号
                    string versionstr = this.GetFileVersion(item.FullName);
                    if (!string.IsNullOrEmpty(versionstr))
                    {
                        datamodel.Version = versionstr;
                    }
                    //文件路径
                    datamodel.Path = item.FullName;
                    //修改时间
                    datamodel.ChangeTime = item.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss");
                    datalist.Add(datamodel);
                }
            }
            catch (Exception e)
            {
                loglist.Add("写入实体数据异常" + e.Message);
                throw;
            }
            return datalist;
        }
        /// <summary>
        /// 获取文件MD5值
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <returns></returns>
        private string GetMD5HashFromFile(string fileName)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                FileStream file = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
            }
            catch (Exception ex)
            {
                loglist.Add("解析文件:" + fileName + "Md5值异常" + ex.Message);
            }
            return sb.ToString();
        }
        /// <summary>
        /// 获取文件版本信息
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        public string GetFileVersion(string filePath)
        {

            string FileVersions = "";
            try
            {
                System.Diagnostics.FileVersionInfo file = System.Diagnostics.FileVersionInfo.GetVersionInfo(filePath);
                if (file.FileVersion != null)
                {
                    FileVersions = file.FileVersion;
                }
            }
            catch (Exception e)
            {
                FileVersions = "";
                loglist.Add("获取:" + filePath + "版本信息异常" + e.Message);
            }
            return FileVersions;
        }
        /// <summary>
        /// 获取DataJson文件内容
        /// </summary>
        public List<Datamodel> Readjson()
        {
            List<Datamodel> textlist = new List<Datamodel>();
            try
            {
                //读取JSON文件内容
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<Datamodel>));
                string str = File.ReadAllText(filenamepath);
                using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(str)))
                {
                    textlist = serializer.ReadObject(ms) as List<Datamodel>;
                }
            }
            catch (Exception e)
            {
                loglist.Add("读取JSON文件内容异常:" + e.Message);
                return textlist;
            }
            return textlist;
        }
        /// <summary>
        /// 比较MD5差异
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> CompareMd5()
        {
            Dictionary<string, string> changemd5list = new Dictionary<string, string>();
            try
            {
                List<Datamodel> datalist = this.Setfiledata();
                List<Datamodel> textlist = this.Readjson();
                if (datalist.Count > 0 && textlist.Count > 0)
                {
                    foreach (var d in datalist)
                    {
                        foreach (var t in textlist)
                        {
                            if (d.Path == t.Path && d.MD5Value != t.MD5Value)
                            {
                                changemd5list.Add(d.FileName, d.Path);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                loglist.Add("比较文件MD5值异常:" + e.Message);
            }
            return changemd5list;
        }
    }
}
