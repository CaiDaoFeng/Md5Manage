using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace WriteMd5
{
    public class Md5Manager
    {
        //程序所处路径
        public static string path = System.AppDomain.CurrentDomain.BaseDirectory;

        //程序所处的上一级目录
        //public static string path = new DirectoryInfo("../").FullName;
        //MD5 JSON文件
        public static string filenamepath = path + "\\" + "md5data.json";
        //日志文件列表
        public static List<string> loglist = new List<string>();
        //是否存在MD5 JSON文件
        public void InitMd5Data()
        {
            try
            {
                if (Directory.Exists(path))
                {
                    if (File.Exists(filenamepath))
                    {
                        //文件存在，删除重新创建
                        File.Delete(filenamepath);
                        var file = File.Create(filenamepath);
                        file.Close();
                    }
                    else
                    {
                        //文件不存在,创建文件
                        var file = File.Create(filenamepath);
                        file.Close();
                    }
                }
            }
            catch (Exception e)
            {
                loglist.Add("读取json文件异常" + e.Message);
                throw e;
            }
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
                    loglist.Add("写入" + item.Name);
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
        /// 写入文件的MD5数据
        /// </summary>
        public bool WriteMd5data()
        {
            List<Datamodel> datalist = this.Setfiledata();
            int count = 0;
            string space = "    ";
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append("[");
                foreach (var item in datalist)
                {
                    sb.Append("{" + "\r\n");
                    sb.Append(space + "\"FileName\": ");
                    sb.Append("\"");
                    sb.Append(item.FileName);
                    sb.Append("\"," + "\r\n");
                    sb.Append(space + "\"ChangeTime\": ");
                    sb.Append("\"");
                    sb.Append(item.ChangeTime);
                    sb.Append("\"," + "\r\n");
                    sb.Append(space + "\"MD5Value\": ");
                    sb.Append("\"");
                    sb.Append(item.MD5Value);
                    sb.Append("\"," + "\r\n");
                    sb.Append(space + "\"Version\": ");
                    sb.Append("\"");
                    sb.Append(item.Version);
                    sb.Append("\"," + "\r\n");
                    sb.Append(space + "\"Path\": ");
                    sb.Append("\"");
                    sb.Append(item.Path.Replace("\\", "\\\\"));
                    sb.Append("\"" + "\r\n");
                    count++;
                    if (count == datalist.Count)
                    {
                        sb.Append("}");
                    }
                    else
                    {
                        sb.Append("}," + " ");
                    }
                }
                sb.Append("]");
                //把数据写到文件 
                using (StreamWriter sw = new StreamWriter(filenamepath))
                {
                    sw.Write(sb);
                    sw.Close();
                }
            }
            catch (Exception e)
            {
                loglist.Add("写入JSON文件数据异常" + e.Message);
                return false;
            }
            return true;
        }
    }
}
