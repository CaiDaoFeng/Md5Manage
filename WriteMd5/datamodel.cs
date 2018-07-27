using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WriteMd5
{
    public class Datamodel
    {
        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName
        {
            get;set;
        }
        /// <summary>
        /// 修改时间
        /// </summary>
        public string ChangeTime
        {
            get;set;
        }
        /// <summary>
        /// MD5值
        /// </summary>
        public string MD5Value
        {
            get;set;
        }
        /// <summary>
        /// 文件版本
        /// </summary>
        public string Version
        {
            get;set;
        }
        /// <summary>
        /// 文件路径
        /// </summary>
        public string Path
        {
            get;set;
        }
    }
}
