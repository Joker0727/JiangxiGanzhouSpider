using Aspose.Words;
using HtmlAgilityPack;
using JiangxiGanzhouSpider.Tool;
using MyTool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JiangxiGanzhouSpider
{
    public class Travelfans
    {
        public HttpHelper hh = null;
        public string websiteUrl = "https://travelfans.talk.tw/Article.aspx?Article_ID=125";
        public MyUtils myUtils = null;
        public string basePath = AppDomain.CurrentDomain.BaseDirectory;
        public string outPath = AppDomain.CurrentDomain.BaseDirectory + @"Word\Travelfans\";

        public Travelfans()
        {
            hh = new HttpHelper();
            myUtils = new MyUtils();
            if (!Directory.Exists(outPath))//判断是否存在
                Directory.CreateDirectory(outPath);//创建新路径
        }

        public void StartSpider()
        {
            DownLoadHtml();
        }
        /// <summary>
        /// 下载html
        /// </summary>
        public void DownLoadHtml()
        {
            string title, releaseTime = string.Empty;

            string html = hh.GetHtml(websiteUrl);
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

            HtmlNode titleNode = doc.DocumentNode.SelectSingleNode("//h2[@class='post-heading']");
            title = titleNode.InnerText;
            title = myUtils.RegexFilterString(title);

            HtmlNode releaseTimeNode = doc.DocumentNode.SelectNodes("//ul[@class='entry-meta widget']/li")[1];
            releaseTime = releaseTimeNode.InnerText;
            releaseTime = myUtils.RegexFilterString(releaseTime);

            HtmlNode contentNode = doc.DocumentNode.SelectSingleNode("//div[@class='article-content']");
            string htmlStr = contentNode.InnerHtml;
            myUtils.TransToWord(htmlStr, title, outPath);
        }




    }
}
