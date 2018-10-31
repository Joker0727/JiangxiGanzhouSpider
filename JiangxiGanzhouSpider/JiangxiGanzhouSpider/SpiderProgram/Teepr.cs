using HtmlAgilityPack;
using JiangxiGanzhouSpider.Tool;
using MyTool;
using System;
using System.Collections;
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
        public string[] firstLevelArr = { "生活", "动物", "惊奇", "艺术", "表演", "旅游", "女性", "运动" };

        public string[] lifeArr = { "DIV", "食物", "心理", "健康", "时尚", "爱情", "美容", "亲子", "感动", "可爱", "救援", "星座", "LGBT", "恐怖", "可恶", "医学", "师哥正妹" };
        public string[] surprisedArr = { "地球", "社会", "历史", "灵异" };
        public string[] artArr = { "建筑", "摄影", "设计" };
        public string[] playArr = { "音乐", "舞蹈" };

        public string DirectoryPrefixuRL = "https://www.teepr.com/category/";

        public string animalUrl = "https://www.teepr.com/category/动物/";
        public string travellUrl = "https://www.teepr.com/category/旅游/";
        public string womanUrl = "https://www.teepr.com/category/女性/";
        public string sportUrl = "https://www.teepr.com/category/运动/";

        public string lifeUrl = "https://www.teepr.com/category/生活/";
        public string surprisedUrl = "https://www.teepr.com/category/惊奇/";
        public string actUrl = "https://www.teepr.com/category/艺术/";
        public string playUrl = "https://www.teepr.com/category/表演/";
        public List<string> categoryList = new List<string>();

        public Teepr(ListBox listBox1, Label label3)
        {
            GetTotalPage("https://www.teepr.com/category/%e7%94%9f%e6%b4%bb/%e9%a3%9f%e7%89%a9/page/224/");
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
            CreateCategoryUrl();
        }
        /// <summary>
        /// 下载新闻链接
        /// </summary>
        public void DownLoadNewsUrl()
        {
            string sqlStr = "select Url from TeeprCategory where IsDownload = 0";
            object[] UrlObj = sh.GetField(sqlStr);
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            ArrayList httpList = hh.GetHtmlData(mainUrl, cookie);
            string lastPageurl, pageUrl, newsUrl = string.Empty;
            int totalPages, newUrlCount = 0;
            foreach (var urlObj in UrlObj)
            {
                pageUrl = urlObj.ToString() + @"page/";
                httpList = hh.GetHtmlData(urlObj.ToString(), cookie);
                doc.LoadHtml(httpList[1].ToString());
                HtmlNode lastANode = doc.DocumentNode.SelectSingleNode("//div[@id='simple-pagination']/div[@class='pagination']/a[@class='last']");
                lastPageurl = lastANode.GetAttributeValue("href", "");
                totalPages = GetTotalPage(lastPageurl);
                for (int i = 1; i < totalPages + 1; i++)
                {
                    pageUrl = pageUrl + i + @"/";
                    httpList = hh.GetHtmlData(pageUrl.ToString(), cookie);
                    doc.LoadHtml(httpList[1].ToString());
                    HtmlNodeCollection newsNodeList = doc.DocumentNode.SelectNodes("//div[@class='content_box']/article/a[@class='clearfix']");
                    foreach (var newsNode in newsNodeList)
                    {
                        newsUrl = newsNode.GetAttributeValue("href", "");

                        sqlStr = $"insert into TeeprNewsUrl (Url,IsDownLoad)values('{newsUrl}',0)";
                        sh.ExeSqlOut(sqlStr);
                        newUrlCount++;
                        myUtils.UpdateLabel(label3, newUrlCount);
                    }
                }
            }
        }
        /// <summary>
        /// 下载html
        /// </summary>
        public void DownLoadHtml()
        {
            string sqlStr = "select Url from TeeprNewsUrl where IsDownload = 0";
            object[] newsUrlObj = sh.GetField(sqlStr);

            foreach (var newsUrl in newsUrlObj)
            {

            }
        }
        /// <summary>
        /// 获取总页数
        /// </summary>
        /// <param name="lastPageurl"></param>
        /// <returns></returns>
        public int GetTotalPage(string lastPageurl)
        {
            int totalPages = 0;
            string[] urlArr = lastPageurl.Split('/');
            if (myUtils.IsNumeric(urlArr[urlArr.Length - 1]))
                totalPages = int.Parse(urlArr[urlArr.Length - 1]);
            return totalPages;
        }

        #region 目录连接拼接
        /// <summary>
        /// 生成链接
        /// </summary>
        public void CreateCategoryUrl()
        {
            categoryList.Add(animalUrl);
            categoryList.Add(travellUrl);
            categoryList.Add(womanUrl);
            categoryList.Add(sportUrl);
            foreach (var item in lifeArr)
            {
                categoryList.Add(lifeUrl + item + @"/");
            }
            foreach (var item in surprisedArr)
            {
                categoryList.Add(surprisedUrl + item + @"/");
            }
            foreach (var item in artArr)
            {
                categoryList.Add(actUrl + item + @"/");
            }
            foreach (var item in playArr)
            {
                categoryList.Add(playUrl + item + @"/");
            }
            string sqlStr, simpleWordUrl = string.Empty;
            int categoryCount = 0;
            foreach (var item in categoryList)
            {
                try
                {
                    simpleWordUrl = myUtils.StringConvert(item, 1);
                    sqlStr = $"insert into TeeprCategory (Url,IsDownLoad)values('{simpleWordUrl}',0)";
                    sh.ExeSqlOut(sqlStr);
                    categoryCount++;
                    myUtils.UpdateLabel(label3, categoryCount);
                }
                catch (Exception ex)
                {

                }
            }
        }
        #endregion

    }
}
