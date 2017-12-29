using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 匹配答案
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            ClickSend();
        }

        private void ClickSend()
        {
            //实现点击发送按钮
            HtmlElementCollection html = webBrowser1.Document.GetElementsByTagName("button");
            foreach (HtmlElement item in html)
            {
                if (item.InnerText == "发送")
                    item.InvokeMember("onclick");
            }
        }
    }
}
