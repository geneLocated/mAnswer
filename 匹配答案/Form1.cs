using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace 匹配答案
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            ReadQA();
        }

        List<string> questLib = new List<string>();
        List<string> ansLib = new List<string>();

        private void startButton_Click(object sender, EventArgs e)
        {
            timer1.Start();
        }

        string lastQuest = null;
        private void timer1_Tick(object sender, EventArgs e)
        {
            string thisQuest = GetQuest();
            if (lastQuest != thisQuest)
            {
                string thisAns = FindInAns(FindInQuest(thisQuest));
                ClickAns(thisAns);
                ClickSend();
                lastQuest = thisQuest;
            }
        }

        #region 网页操作
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

        private string GetQuest()
        {
            //获取最后一个问题
            HtmlElementCollection questDiv = webBrowser1.Document.GetElementsByTagName("div");
            string str = null;
            foreach (HtmlElement item in questDiv)
            {
                if (item.GetAttribute("className") == "answer_text")
                    if (item.FirstChild.InnerText.Contains(".")
                        || item.FirstChild.InnerText.Contains("．"))    //避免识别到“恭喜你答对了”
                    {
                        str = item.FirstChild.InnerText;
                        str = str.Substring(str.IndexOf(".") + 1, str.IndexOf("\r\n") - 2);
                    }
            }
            return str;
        }
        #endregion

        #region 题库操作
        private void ReadQA()
        {
            //将题库文件读入List
            StreamReader sr = new StreamReader("Question.txt");
            string currLine;
            while ((currLine = sr.ReadLine()) != null)
                questLib.Add(currLine);
            sr = new StreamReader("Answer.txt");
            while ((currLine = sr.ReadLine()) != null)
                ansLib.Add(currLine);
        }

        private string FindInQuest(string value)
        {
            //根据内容找题号
            string result = null;
            value = FormatStr(value);
            foreach (string currLine in questLib)
            {
                string formatLine = FormatStr(currLine);
                if (formatLine.Contains(value))   //若包含内容
                {
                    int stopIndex = formatLine.IndexOf(".");
                    result = formatLine.Substring(0, stopIndex);
                }
            }
            return result;
        }

        private string FindInAns(string order)
        {
            //根据题号找答案
            string result = null;
            foreach (string currLine in ansLib)
            {
                if(currLine.Contains(order))
                {
                    int targetIndex = currLine.IndexOf(order);
                    //从序号后开始查找"."
                    int startIndex = currLine.IndexOf(".", targetIndex) + 1;
                    int stopIndex = currLine.IndexOf("  ", startIndex + 1);
                    result = currLine.Substring(startIndex, stopIndex - startIndex);
                }
            }
            return result;
        }

        private string FormatStr(string str)
        {
            str = str.Replace("[多选]", "");
            str = str.Replace(":", ".");
            str = str.Replace("\r", "");
            str = str.Replace("　", "");
            str = str.Replace("(", "");
            str = str.Replace(")", "");
            str = str.Replace("（", "");
            str = str.Replace("）", "");
            str = str.Replace("_", "");
            str = str.Replace("c", "");
            str = str.Replace(" ", "");
            return str;
        }
        #endregion
    }
}
