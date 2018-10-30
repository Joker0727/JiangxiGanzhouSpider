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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JiangxiGanzhouSpider.SpiderProgram
{
    public class Icook
    {
        public HttpHelper hh = null;
        public string mainUrl = "https://icook.tw";
        public string categoriesUrl = "https://icook.tw/categories";
        public MyUtils myUtils = null;
        public CookieContainer cookie = new CookieContainer();
        public string basePath = AppDomain.CurrentDomain.BaseDirectory;
        public string sqlitePath = AppDomain.CurrentDomain.BaseDirectory + "sqlite3.db";
        public SQLiteHelper sh = null;
        public string outPath = AppDomain.CurrentDomain.BaseDirectory + @"Word\Icook\";
        public ListBox listBox1 = null;
        public Label label3 = null;

        public Icook(ListBox listBox1, Label label3)
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
                        DownLoadMenusUrl();
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
            string deleteSqlStr = "delete from IcookCategory";
            try
            {
                // sh.RunSql(deleteSqlStr); //先删除
                ArrayList mainAList = hh.GetHtmlData(mainUrl, cookie);
                ArrayList categoryAList = hh.GetHtmlData(categoriesUrl, cookie);
                if (categoryAList.Count == 3)
                {
                    HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                    doc.LoadHtml(categoryAList[1].ToString());

                    HtmlNodeCollection aNodeList = doc.DocumentNode.SelectNodes("//li[@class='list-group-item']/a");

                    string categoryUrl, sqlStr = string.Empty;
                    int urlCount = 0;
                    foreach (var aNode in aNodeList)
                    {
                        try
                        {
                            categoryUrl = aNode.GetAttributeValue("href", "");
                            if (categoryUrl.Contains("categories"))
                            {
                                if (!categoryUrl.Contains(mainUrl))
                                    categoryUrl = mainUrl + categoryUrl;

                                sqlStr = $"insert into IcookCategory (Url,IsDownLoad)values('{categoryUrl}',0)";
                                sh.ExeSqlOut(sqlStr);
                                urlCount++;
                                myUtils.UpdateLabel(label3, urlCount);
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
        /// <summary>
        /// 下载菜单链接
        /// </summary>
        public void DownLoadMenusUrl()
        {
            string sqlStr = "select Url from IcookCategory";
            int totalPages = 0;
            string pageUrl, menuUrl = string.Empty;
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            try
            {
                object[] categoriesUrlObj = sh.GetField(sqlStr);
                ArrayList mainAList = hh.GetHtmlData(mainUrl, cookie);
                int urlCount = 0;
                foreach (var categoryUrl in categoriesUrlObj)
                {
                    try
                    {
                        ArrayList MenuAList = hh.GetHtmlData(categoryUrl.ToString(), cookie);
                        totalPages = GetTotalPages(MenuAList[1].ToString());
                        for (int i = 1; i < totalPages + 1; i++)
                        {
                            try
                            {
                                pageUrl = categoryUrl + "?page=" + i;
                                ArrayList PageAList = hh.GetHtmlData(pageUrl, cookie);
                                doc.LoadHtml(PageAList[1].ToString());
                                HtmlNodeCollection aNodeList = doc.DocumentNode.SelectNodes("//div[@class='categories-browse-recipe']/a[@class='browse-recipe-touch-link']");
                                foreach (var aNode in aNodeList)
                                {
                                    try
                                    {
                                        menuUrl = aNode.GetAttributeValue("href", "");
                                        if (!menuUrl.Contains(mainUrl))
                                            menuUrl = mainUrl + menuUrl;

                                        sqlStr = $"insert into IcookMenu (Url,IsValid)values('{menuUrl}',1)";
                                        sh.ExeSqlOut(sqlStr);
                                        urlCount++;
                                        myUtils.UpdateLabel(label3, urlCount);
                                    }
                                    catch (Exception e)
                                    {
                                    }
                                }
                            }
                            catch (Exception x)
                            {
                            }
                        }
                    }
                    catch (Exception y)
                    {
                    }
                }
            }
            catch (Exception ey)
            {
            }
        }
        /// <summary>
        /// 获取总页数
        /// </summary>
        /// <param name="htmlStr"></param>
        /// <returns></returns>
        public int GetTotalPages(string htmlStr)
        {
            int totalPage = 0, totalMenus = 0;
            try
            {
                //  htmlStr = File.ReadAllText(@"C:\Users\Joker\Desktop\1.txt", Encoding.Default);
                string[] htmlArr = myUtils.SplitByStr(htmlStr, @"</title>");
                htmlArr = myUtils.SplitByStr(htmlArr[0], @"<title>");
                string result = System.Text.RegularExpressions.Regex.Replace(htmlArr[1], @"[^0-9]+", "");
                if (myUtils.IsNumeric(result))
                    totalMenus = int.Parse(result);

                totalPage = totalMenus % 12 == 0 ? totalMenus / 12 : totalMenus / 12 + 1;
            }
            catch (Exception ex)
            {
            }
            return totalPage;
        }

        /// <summary>
        /// 下载html
        /// </summary>
        public void DownLoadHtml()
        {
            string sqlStr = "select Url from IcookMenu where IsDownload = 0";
            object[] menuUrlObj = sh.GetField(sqlStr);
            string title, fullFoldPath = string.Empty;
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();

            ArrayList mainAList = hh.GetHtmlData(mainUrl, cookie);
            int htmlCount = 0;
            foreach (var menuUrl in menuUrlObj)
            {
                ArrayList menuResList = hh.GetHtmlData(menuUrl.ToString(), cookie);
                doc.LoadHtml(menuResList[1].ToString());

                HtmlNode h1Node = doc.DocumentNode.SelectSingleNode("//div[@class='recipe-details-header-title']/h1[@class='title']");
                title = myUtils.StringConvert(h1Node.InnerText).Trim();//标题
                fullFoldPath = outPath + title + @"\";
                if (!Directory.Exists(fullFoldPath))//判断是否存在
                    Directory.CreateDirectory(fullFoldPath);//创建新路径
                HtmlNode headerNode = doc.DocumentNode.SelectSingleNode("//div[@class='recipe-details-header recipe-details-block']");
                HtmlNode headerChild = doc.DocumentNode.SelectSingleNode("//div[@class='recipe-details-header recipe-details-block']/div[@class='header-row center-row']");
                HtmlNode rightChild = doc.DocumentNode.SelectSingleNode("//div[@class='recipe-details-header recipe-details-block']/div[@class='header-row center-row']/div[@class='header-col right-col']");
                headerChild.RemoveChild(rightChild);//删除右边

                HtmlNode headerImgParentNode = doc.DocumentNode.SelectSingleNode("//div[@class='recipe-details-header recipe-details-block']/div[@class='header-row center-row']/div[@class='header-col left-col']/div[@class='recipe-cover']");
                HtmlNode headerImgChild = doc.DocumentNode.SelectSingleNode("//img[@class='main-pic']");
                string headerImgSrc = headerImgChild.GetAttributeValue("src", "");
                myUtils.DownLoadImage(headerImgSrc, fullFoldPath + @"图片1.jpg", cookie);
                headerImgParentNode.RemoveAllChildren();

                HtmlNode newheaderImgNode = doc.CreateElement("div");
                newheaderImgNode.InnerHtml = $"图片--------------------------{1}-------------------------";
                headerImgParentNode.AppendChild(newheaderImgNode);

                string headerHtml = headerNode.InnerHtml;//头部内容  

                HtmlNode mainNode = doc.DocumentNode.SelectSingleNode("//div[@class='recipe-details-main']");
                HtmlNode mainChild = doc.DocumentNode.SelectSingleNode("//div[@class='recipe-details-main']/div[@class='recipe-ad-placeholder']");
                mainNode.RemoveChild(mainChild);

                HtmlNodeCollection imgParentNodeList = doc.DocumentNode.SelectNodes("//div[@class='step-cover']");
                if (imgParentNodeList != null)
                {
                    for (int i = 1; i < imgParentNodeList.Count + 1; i++)
                    {
                        try
                        {
                            HtmlNode imgChildNode = imgParentNodeList[i].SelectSingleNode("//a[@class='strip']");
                            string imgUrl = imgChildNode.GetAttributeValue("href", "").Replace("medium_", "large_");
                            myUtils.DownLoadImage(imgUrl, fullFoldPath + $"图片{i + 1}.jpg", cookie);
                            imgParentNodeList[i].RemoveAllChildren();
                            HtmlNode newImgNode = doc.CreateElement("div");
                            newImgNode.InnerHtml = $"图片--------------------------{i + 1}-------------------------";
                            imgParentNodeList[i].AppendChild(newImgNode);
                        }
                        catch (Exception e)
                        {
                        }
                    }
                }

                string mainStr = mainNode.InnerHtml;//主题内容
                string allStr = headerHtml + mainStr;

                // sqlStr = $"UPDATE IcookMenu SET Title = '{title}', Html = '{allStr}' WHERE Url = '{menuUrl}'";
                sqlStr = $"UPDATE IcookMenu SET Title = '{title}' WHERE Url = '{menuUrl}'";
                sh.RunSql(sqlStr);

                if (myUtils.TransToWord(allStr, title, fullFoldPath))
                {
                    sqlStr = $"UPDATE IcookMenu SET IsDownload = 1 WHERE Url = '{menuUrl}'";
                    sh.RunSql(sqlStr);
                    htmlCount++;
                    myUtils.UpdateLabel(label3, htmlCount);
                }
                myUtils.UpdateListBox(listBox1, title);
            }
        }
    }
}
