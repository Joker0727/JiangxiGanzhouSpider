using JiangxiGanzhouSpider.SpiderProgram;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JiangxiGanzhouSpider
{
    public class StartSpider
    {
        public string outPath = string.Empty;
        public ListBox listBox = null;
        public Thread th = null;
        public StartSpider(ListBox listBox)
        {
            this.listBox = listBox;
        }
        public void StartWork()
        {
           
        }
    }
}
