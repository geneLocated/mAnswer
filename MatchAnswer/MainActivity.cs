using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content.Res;

using System.IO;
using System.Collections.Generic;

namespace MatchAnswer
{
    [Activity(Label = "MatchAnswer", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity
    {
        int count = 1;
        List<string> strQuest = new List<string>();
        List<string> strAns = new List<string>();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.myButton);

            button.Click += delegate { button.Text = string.Format("{0} clicks!", count++); };

            GetQA();
             string answer = FindInAns(FindInQuest("实施乡村振兴战略。____问题是关"));
            AlertDialog.Builder alertDialog = new AlertDialog.Builder(this)
                .SetMessage(answer);
            alertDialog.Show();
        }

        private List<string> GetFromAssets(string fileName)
        {
            List<string> result = new List<string>();
            AssetManager assetManager = this.Assets;
            StreamReader streamReader = new StreamReader(Assets.Open(fileName));
            string currLine;
            while((currLine=streamReader.ReadLine()) != null)
                result.Add(currLine);
            return result;
        }

        private void GetQA()
        {
            strQuest = GetFromAssets("Question.txt");
            strAns = GetFromAssets("Answer.txt");
        }

        private string FindInQuest(string target)   //返回包含该字符串的题目序号
        {
            string result = null;
            foreach (string currLine in strQuest)
                if (currLine.Contains(target))  //若该行包含target
                {
                    int stopIndex;
                    if (currLine.Contains("．")) //若存在"．"
                        stopIndex = currLine.IndexOf("．");
                    else
                        stopIndex = currLine.IndexOf(".");
                    result = currLine.Substring(0, stopIndex);
                }
            return result;
        }

        private string FindInAns(string target) //返回包含该字符串（序号）的答案内容
        {
            string result = null;
            foreach (string currLine in strAns)
                if (currLine.Contains(target))  //若该行包含target
                {
                    int targetIndex = currLine.IndexOf(target);
                    int startIndex = currLine.IndexOf(".", targetIndex) + 1; ////从targetIndex后开始查找"."
                    //加一，因为点后面才是答案内容
                    int stopIndex = currLine.IndexOf(" ", startIndex + 1);  //从答案内容开始后查找空格
                    result = currLine.Substring(startIndex, stopIndex - startIndex);
                }
            return result;
        }
    }
}

