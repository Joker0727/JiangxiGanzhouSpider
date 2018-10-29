using JiangxiGanzhouSpider.SpiderProgram;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JiangxiGanzhouSpider
{
    public partial class Form1 : Form
    {
        public StartSpider ss = null;
        public string basePath = AppDomain.CurrentDomain.BaseDirectory;
        public Thread th = null;
        public string workId = "ww-008";
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.MaximizeBox = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string webSiteName = this.comboBox1.Text;
            if(string.IsNullOrEmpty(webSiteName)|| webSiteName == "请选择要下载的网站")
            {
                MessageBox.Show("请选择要下载的网站！");
                return;
            }
            StartWork(1);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            string webSiteName = this.comboBox1.Text;
            if (string.IsNullOrEmpty(webSiteName) || webSiteName == "请选择要下载的网站")
            {
                MessageBox.Show("请选择要下载的网站！");
                return;
            }
            StartWork(2);
        }
        public void StartWork(int option)
        {
            if (!IsAuthorised(workId))
            {
                MessageBox.Show("网络异常！");
                return;
            }
            Icook icook = new Icook(this.listBox1,this.label3);

            th = new Thread(icook.StartSpider);
            th.IsBackground = true;
            th.Start(option);
        }
        public bool IsAuthorised(string workId)
        {
            string conStr = "Server=111.230.149.80;DataBase=MyDB;uid=sa;pwd=1add1&one";
            using (SqlConnection con = new SqlConnection(conStr))
            {
                string sql = string.Format("select count(*) from MyWork Where WorkId ='{0}'", workId);
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    con.Open();
                    int count = int.Parse(cmd.ExecuteScalar().ToString());
                    if (count > 0)
                        return true;
                }
            }
            return false;
        }

    }
}
