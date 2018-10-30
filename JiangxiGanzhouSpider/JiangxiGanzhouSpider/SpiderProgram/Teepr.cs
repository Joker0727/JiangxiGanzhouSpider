using JiangxiGanzhouSpider.Tool;
using MyTool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JiangxiGanzhouSpider.SpiderProgram
{
    public class Teepr
    {
        public HttpHelper hh = null;
        public string mainUrl = "https://www.teepr.com/";
        public MyUtils myUtils = null;
        public CookieContainer cookie = new CookieContainer();
        public string basePath = AppDomain.CurrentDomain.BaseDirectory;
        public string sqlitePath = AppDomain.CurrentDomain.BaseDirectory + "sqlite3.db";
        public SQLiteHelper sh = null;
        public string outPath = AppDomain.CurrentDomain.BaseDirectory + @"Word\Teepr\";
        public ListBox listBox1 = null;
        public Label label3 = null;

        public Teepr(ListBox listBox1, Label label3)
        {
            this.listBox1 = listBox1;
            this.label3 = label3;
            hh = new HttpHelper();
            myUtils = new MyUtils();
            sh = new SQLiteHelper(this.sqlitePath);
            if (!Directory.Exists(outPath))//判断是否存在
                Directory.CreateDirectory(outPath);//创建新路径
        }

        public void StartSpider(object obj)
        {
            int option = int.Parse(obj.ToString());
            switch (option)
            {
                case 1:
                    {
                        DownLoadHtml();
                        break;
                    }
                case 2:
                    {
                        DownLoadCategoriesUrl();
                        DownLoadNewsUrl();
                        break;
                    }
                default:
                    break;
            }
        }
        /// <summary>
        /// 下载目录链接
        /// </summary>
        public void DownLoadCategoriesUrl()
        {
            for (int i = 0; i < 100000; i++)
            {
                myUtils.UpdateLabel(label3, i);
                myUtils.UpdateListBox(listBox1, i.ToString());
                Thread.Sleep(1000);
            }
        }
        /// <summary>
        /// 下载新闻链接
        /// </summary>
        public void DownLoadNewsUrl()
        {
            for (int i = 0; i < 100000; i++)
            {
                myUtils.UpdateLabel(label3, i);
                myUtils.UpdateListBox(listBox1, i.ToString());
                Thread.Sleep(1000);
            }
        }
        /// <summary>
        /// 下载html
        /// </summary>
        public void DownLoadHtml()
        {
            for (int i = 0; i < 100000; i++)
            {
                myUtils.UpdateLabel(label3, i);
                myUtils.UpdateListBox(listBox1, i.ToString());
                Thread.Sleep(1000);
            }
        }

    }
}
