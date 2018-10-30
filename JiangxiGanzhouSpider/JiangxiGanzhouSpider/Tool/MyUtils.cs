using Aspose.Words;
using MyTool;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JiangxiGanzhouSpider.Tool
{
    public class MyUtils
    {
        /// <summary>
        /// 过滤字符串
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string RegexFilterString(string text)
        {
            string resultText = string.Empty;
            try
            {
                text = Regex.Replace(text, "[&nbsp;]", "");//去 &nbsp;
                text = Regex.Replace(text, "[\r\n\t]", "");//去 \r\n\t
                text = Regex.Replace(text, "\\s{2,}", "");//去空格
                resultText = text;
            }
            catch (Exception)
            {
                throw;
            }
            return resultText;
        }
        /// <summary>
        /// 读取word内容
        /// </summary>
        /// <param name="docpath">word文档路径</param>
        /// <returns></returns>
        public void DealWord(string docpath)
        {
            string moreThanCharacter = "Evaluation Only. Created with Aspose.Words. Copyright 2003-2014 Aspose Pty Ltd.";
            //实例化COM
            Microsoft.Office.Interop.Word.ApplicationClass wordApp = new Microsoft.Office.Interop.Word.ApplicationClass();
            object fileobj = docpath;
            object nullobj = System.Reflection.Missing.Value;
            //打开指定文件（不同版本的COM参数个数有差异，一般而言除第一个外都用nullobj就行了）
            Microsoft.Office.Interop.Word.Document doc = wordApp.Documents.Open(ref fileobj, ref nullobj, ref nullobj,
            ref nullobj, ref nullobj, ref nullobj,
            ref nullobj, ref nullobj, ref nullobj,
            ref nullobj, ref nullobj, ref nullobj, ref nullobj, ref nullobj, ref nullobj, ref nullobj);
            foreach (Microsoft.Office.Interop.Word.Shape shape in doc.Shapes)
            {
                if (shape.Name.Equals(moreThanCharacter))
                {
                    shape.Delete();
                    break;
                }
            }

            //取得doc文件中的文本
            // string outText = doc.Content.Text;
            //关闭文件
            doc.Close(ref nullobj, ref nullobj, ref nullobj);
            //关闭COM
            wordApp.Quit(ref nullobj, ref nullobj, ref nullobj);
            //返回
            // return outText;
        }
        /// <summary>
        /// 删除非法字符
        /// </summary>
        /// <param name="fileName"></param>
        public string FilterPath(string filePath)
        {
            try
            {
                filePath = Regex.Replace(filePath, "[&nbsp;]", "");//去 &nbsp;
                filePath = Regex.Replace(filePath, "[|「」，<>]", "");//去 \r\n\t
                filePath = Regex.Replace(filePath, "\\s{2,}", "");//去空格
            }
            catch (Exception)
            {
                throw;
            }

            return filePath;
        }
        /// <summary>
        /// 汉字繁简互转
        /// </summary>
        /// <param name="x">内容</param>
        /// <param name="type">类型2是简体</param>
        /// <returns></returns>
        public string StringConvert(string x, int type = 2)
        {
            String value = String.Empty;
            switch (type)
            {
                case 1://转繁体
                    value = Microsoft.VisualBasic.Strings.StrConv(x, Microsoft.VisualBasic.VbStrConv.TraditionalChinese, 0);
                    break;
                case 2:
                    value = Microsoft.VisualBasic.Strings.StrConv(x, Microsoft.VisualBasic.VbStrConv.SimplifiedChinese, 0);
                    break;
                default:
                    break;
            }
            return value;
        }
        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <param name="oldStr">原来的字符串</param>
        /// <param name="splitKey">分割字符</param>
        /// <returns></returns>
        public string[] SplitByStr(string oldStr, string splitKey)
        {
            return Regex.Split(oldStr, splitKey, RegexOptions.IgnoreCase);
        }
        /// <summary>
        /// 判断是否是数字
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool IsNumeric(string value)
        {
            return Regex.IsMatch(value, @"^[+-]?\d*[.]?\d*$");
        }
        /// <summary>
        /// html转换成Word
        /// </summary>
        /// <param name="htmlLabel"></param>
        public bool TransToWord(string htmlLabel, string title, string outPath)
        {

            try
            {
                title = FilterPath(title);
                string fullPath = outPath + title + ".doc";//word文件保存路径   

                htmlLabel = StringConvert(htmlLabel);
                htmlLabel = $"<!DOCTYPE html><html><head></head><body><div>{htmlLabel}</div></body></html>";
                File.WriteAllText(fullPath, htmlLabel);

                //Document doc = new Document();
                //DocumentBuilder builder = new DocumentBuilder(doc);
                //builder.InsertHtml(htmlLabel);

                //doc.Save(fullPath);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// 更新ListBox
        /// </summary>
        /// <param name="listBox"></param>
        /// <param name="item"></param>
        public void UpdateListBox(ListBox listBox, string item)
        {
            if (listBox.InvokeRequired)
            {
                // 当一个控件的InvokeRequired属性值为真时，说明有一个创建它以外的线程想访问它
                Action<string> actionDelegate = (x) =>
                {
                    listBox.Items.Add(item);
                    listBox.TopIndex = listBox.Items.Count - 1;
                };
                // 或者
                // Action<string> actionDelegate = delegate(string txt) { this.label2.Text = txt; };
                listBox.Invoke(actionDelegate, item);
            }
            else
            {
                listBox.Items.Add(item);
                listBox.TopIndex = listBox.Items.Count - 1;
            }
        }
        /// <summary>
        /// 更新更新面板数据
        /// </summary>
        /// <param name="label"></param>
        /// <param name="count"></param>
        public void UpdateLabel(Label label, int count)
        {
            if (label.InvokeRequired)
            {
                // 当一个控件的InvokeRequired属性值为真时，说明有一个创建它以外的线程想访问它
                Action<int> actionDelegate = (x) =>
                {
                    label.Text = x.ToString();
                };
                // 或者
                // Action<string> actionDelegate = delegate(string txt) { this.label2.Text = txt; };
                label.Invoke(actionDelegate, count);
            }
            else
            {
                label.Text = count.ToString();
            }
        }
        ///
        /// <summary>
        /// 下载网页图片
        /// </summary>
        /// <param name="url">下载路径</param>
        /// <param name="desPath">目标路径</param>
        /// <returns></returns>
        public void DownLoadImage(string url, string path ,CookieContainer cookie)
        {
            HttpHelper hh = new HttpHelper();
            byte[] byteArr = hh.DowloadCheckImg(url, cookie);
            Image image = GetImageByBytes(byteArr);
            image.Save(path, ImageFormat.Jpeg);
        }
        /// <summary>
        /// 读取byte[]并转化为图片
        /// </summary>
        /// <param name="bytes">byte[]</param>
        /// <returns>Image</returns>
        public Image GetImageByBytes(byte[] bytes)
        {
            MemoryStream ms = new MemoryStream(bytes);
            Image image = System.Drawing.Image.FromStream(ms);
            return image;
        }

    }
}
