using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content.Res;
using Android.Content;
using Android.Views;
using Android.Runtime;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;

using System.IO;
using System.Collections.Generic;
using System.Threading;

namespace MatchAnswer
{
    [Activity(Label = "MatchAnswer", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity
    {
        Button startButton;
        Button stopButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Main);
            startButton = FindViewById<Button>(Resource.Id.startButton);
            stopButton = FindViewById<Button>(Resource.Id.stopButton);
            startButton.Click += StartButton_Click;
            stopButton.Click += StopButton_Click;
            GetQA();
        }


        private void StartButton_Click(object sender, System.EventArgs e)
        {
            CreateFloatWindow();
            InitClipboard();
            startButton.Enabled = false;
            stopButton.Enabled = true;
        }

        private void StopButton_Click(object sender, System.EventArgs e)
        {
            MWindowManager.RemoveView(floatLayout);
            floatLayout.Dispose();
            floatLayout = null;
            timer.Dispose();
            timer = null;
            clipboard.Dispose();
            clipboard = null;
            startButton.Enabled = true;
            stopButton.Enabled = false;
        }

        ShapeDrawable dr0;
        ShapeDrawable dr1;

        LinearLayout floatLayout;   //浮动窗口布局
        private IWindowManager MWindowManager
        {
            get
            {   //不是简单的类型转换，一定要用 JavaCast
                return this.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
            }
        }
        private void CreateFloatWindow()
        {
            this.floatLayout = new LinearLayout(this);
            var shape = new OvalShape();
            dr0 = new ShapeDrawable(shape);
            dr0.Paint.Color = Color.DarkGray;
            dr1 = new ShapeDrawable(shape);
            dr1.Paint.Color = Color.DarkBlue;
            floatLayout.Background = dr0;

            var param = new WindowManagerLayoutParams
            {
                Type = WindowManagerTypes.Phone,
                Format = Android.Graphics.Format.Transparent,
                Gravity = GravityFlags.Bottom | GravityFlags.Left, //原点
                Flags = WindowManagerFlags.NotFocusable,    //不可聚焦
                Width = 100,    //宽度
                Height = 100,   //高度
            };
            this.MWindowManager.AddView(this.floatLayout, param);
        }

        #region TextProcessPart
        List<string> strQuest = new List<string>();
        List<string> strAns = new List<string>();
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
        #endregion

        #region ClipPart
        ClipboardManager clipboard;
        Timer timer;
        string clipText;
        string lastClipText;
        private void InitClipboard()
        {
            clipboard = (ClipboardManager)GetSystemService(Context.ClipboardService);
            timer = new Timer(new TimerCallback(Timer_Tick), null, 100, 600);
        }

        bool bgColorStatus = false;
        public void Timer_Tick(object sender)
        {
            RunOnUiThread(() =>
            {
                clipText = clipboard.Text;
                if (clipText != lastClipText)
                {
                    TextView tv = new TextView(this);
                    try
                    {
                        string answer = FindInAns(FindInQuest(clipText));
                        tv.Text = answer;
                    }
                    catch { tv.Text = "null"; }
                    floatLayout.RemoveAllViews();
                    floatLayout.AddView(tv);
                    if (bgColorStatus)
                        floatLayout.Background = dr1;
                    else
                        floatLayout.Background = dr0;
                    bgColorStatus = !bgColorStatus;
                }
                lastClipText = clipText;
            });
        }
        #endregion
    }
}

