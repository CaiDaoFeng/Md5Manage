using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CompareMd5
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Md5Manager Md5manager = new Md5Manager();
        private void button1_Click(object sender, EventArgs e)
        {
            //清除日志
            listView1.Items.Clear();
            Md5Manager.loglist.Clear();
            button1.Enabled = false;
            listView1.Items.Add("开始比对...");
            if (!Md5manager.InitMd5Data())
            {
                listView1.Items.Add("不存在JSON文件");
            }
            else
            {
                Dictionary<string, string> changelist = Md5manager.CompareMd5();
                List<string> loglist = Md5Manager.loglist;
                foreach (var item in loglist)
                {
                    listView1.Items.Add(item);
                }
                if (changelist.Count > 0)
                {
                    button1.Enabled = true;
                    this.AddViewItem(changelist);
                }
                else
                {
                    listView1.Items.Add("未发现MD5值不匹配");
                    button1.Enabled = true;
                    listView1.EnsureVisible(listView1.Items.Count - 1);
                }
            }
        }
        private void AddViewItem(Dictionary<string, string> itemlist)
        {
            foreach (var item in itemlist)
            {
                ListViewItem listViewItem = new ListViewItem();
                listViewItem.Text = item.Key + "   Md5值不匹配," + "文件路径:" + item.Value;
                listViewItem.ForeColor = Color.Red;
                listView1.Items.Add(listViewItem);
            }
            listView1.EnsureVisible(listView1.Items.Count - 1);
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            int high = this.Height;
            button1.Height = high / 2;
            listView1.Height = high / 2-50;
            listView1.Top = button1.Top + high / 2;

            listView1.Columns[0].Width = this.Width-50;
        }
    }
}
