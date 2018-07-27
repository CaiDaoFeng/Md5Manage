using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WriteMd5
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private bool writeresult = false;
        Md5Manager Md5Manager = new Md5Manager();
        private void button1_Click(object sender, EventArgs e)
        {
            //清除日志
            listView1.Items.Clear();
            Md5Manager.loglist.Clear();
            button1.Enabled = false;
            listView1.Items.Add("开始写入...");
            writeresult = Md5Manager.WriteMd5data();
            List<string> loglist = Md5Manager.loglist;
            foreach (var item in loglist)
            {
                listView1.Items.Add(item);
            }
            if (writeresult)
            {
                listView1.Items.Add("写入成功");
                button1.Enabled = true;
                listView1.EnsureVisible(listView1.Items.Count - 1);
            }
            else
            {
                listView1.Items.Add("写入失败");
                button1.Enabled = true;
                listView1.EnsureVisible(listView1.Items.Count - 1);
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            int high = this.Height;
            button1.Height = high / 2;
            listView1.Height = high / 2 - 50;
            listView1.Top = button1.Top + high / 2;

            listView1.Columns[0].Width = this.Width - 50;
        }
    }
}
