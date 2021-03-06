﻿using JiangxiGanzhouSpider.SpiderProgram;
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
        public string workId = "ww-0022";
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
            string btn1Str = this.button1.Text;

            CloseThread();

            if (btn1Str == "下载html")
            {
                th = new Thread(StartWork);
                th.IsBackground = true;
                th.Start(1);

                this.button1.Text = "暂停";
                this.button2.Text = "下载链接";
            }
            else
            {
                this.button1.Text = "下载html";
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            string btn2Str = this.button2.Text;

            CloseThread();

            if (btn2Str == "下载链接")
            {
                th = new Thread(StartWork);
                th.IsBackground = true;
                th.Start(2);
                this.button2.Text = "暂停";
                this.button1.Text = "下载html";
            }
            else
            {
                this.button2.Text = "下载链接";
            }
        }
        /// <summary>
        /// 开始任务
        /// </summary>
        /// <param name="obj"></param>
        public void StartWork(object obj)
        {
            int option = int.Parse(obj.ToString());
            if (!IsAuthorised())
            {
                MessageBox.Show("网络异常！");
                return;
            }
            string webSiteName = string.Empty;
            this.comboBox1.Invoke(new Action(() => { webSiteName = this.comboBox1.Text; }));

            if (string.IsNullOrEmpty(webSiteName) || webSiteName == "请选择要下载的网站")
            {
                MessageBox.Show("请选择要下载的网站！");
                return;
            }
            switch (webSiteName)
            {
                case "icook":
                    {
                        Icook icook = new Icook(this.listBox1, this.label3);
                        icook.StartSpider(option);
                        break;
                    }
                case "teepr":
                    {
                        Teepr teepr = new Teepr(this.listBox1, this.label3);
                        teepr.StartSpider(option);
                        break;
                    }
                default:
                    {
                        MessageBox.Show("该网站不存在！");
                        break;
                    }
            }
        }
        /// <summary>
        /// 授权
        /// </summary>
        /// <param name="workId"></param>
        /// <returns></returns>
        public bool IsAuthorised()
        {
            string conStr = "Server=111.230.149.80;DataBase=MyDB;uid=sa;pwd=1add1&one";
            bool bo = false;
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    string sql = string.Format("select count(*) from MyWork Where IsAuth = 1 and WorkId ='{0}'", workId);
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        con.Open();
                        int count = int.Parse(cmd.ExecuteScalar().ToString());
                        if (count > 0)
                        {
                            bo = true;
                            string currentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            sql = $"update MyWork set LastTime ='{currentTime}' where WorkId = '{workId}'";
                            cmd.CommandText = sql;
                            int res = cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception)
            {
                bo = false;
            }

            return bo;
        }
        /// <summary>
        /// 关闭线程
        /// </summary>
        public void CloseThread()
        {
            if (th != null)
            {
                th.Abort();
                th = null;
            }
        }
    }
}
