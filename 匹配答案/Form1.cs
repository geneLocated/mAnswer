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
            ClickAns("A");
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

        private void ClickAns(string ans)
        {
            //点击答案
            HtmlElementCollection answers = webBrowser1.Document.GetElementsByTagName("span");
            foreach (HtmlElement item in answers)
            {
                if(item.InnerText != null)
                    if (item.InnerText.Contains(ans+"."))
                    {
                        item.InvokeMember("onclick");
                        break;  //有很多答案按钮，这里点击第一个就可以了
                    }
            }
        }
    }
}
