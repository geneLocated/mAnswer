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
        List<string> strQuest = new List<string>();
        List<string> strAns = new List<string>();
        string clipText;
        string lastClipText;
        ClipboardManager clipboard;
        Button switchButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            // Get our button from the layout resource, and attach an event to it
            switchButton = FindViewById<Button>(Resource.Id.switchButton);
            switchButton.Click += SwitchButton_Click;
            GetQA();
            //InitClipboard();
        }

        private void SwitchButton_Click(object sender, System.EventArgs e)
        {
            Intent serviceIntent=new Intent(this, typeof(FloatWindowService));
            StartService(serviceIntent);
            switchButton.Text = Resources.GetString(Resource.String.stop);
        }

        #region TextProcessPart
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
        private void InitClipboard()
        {
            clipboard = (ClipboardManager)GetSystemService(Context.ClipboardService);
            Timer timer = new Timer(new TimerCallback(Timer_Tick), null, 100, 600);
        }

        void ShowAlert(string str)
        {
            AlertDialog.Builder alertDialog = new AlertDialog.Builder(this);
            alertDialog.SetMessage(str);
            alertDialog.Show();
        }

        public void Timer_Tick(object sender)
        {
            RunOnUiThread(() =>
            {
                clipText = clipboard.Text;
                if (clipText != lastClipText)
                {
                    ShowAlert(clipText);
                }
                lastClipText = clipText;
            });
        }
        #endregion
    }

    [Service(Name = "MatchAnswer.MatchAnswer.FloatWindowService")]
    public class FloatWindowService : Service
    {
        private Timer timer;
        LinearLayout floatLayout;   //浮动窗口布局
        private IWindowManager WindowManager
        {
            get
            {   //不是简单的类型转换，一定要用 JavaCast
                return this.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
            }
        }

        public override IBinder OnBind(Intent intent)   //必须重写的方法
        {
            return null;
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            CreateFloatWindow();
            if (timer == null)  //开启定时器
                timer = new Timer(new TimerCallback(Timer_Tick), null, 100, 600);
            return base.OnStartCommand(intent, flags, startId);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        private void CreateFloatWindow()
        {
            this.floatLayout = new LinearLayout(this);
            var shape = new OvalShape();
            var dr = new ShapeDrawable(shape);
            dr.Paint.Color = Color.WhiteSmoke;
            dr.Paint.Alpha = 100;
            floatLayout.Background = dr;

            var param = new WindowManagerLayoutParams
            {
                Type = WindowManagerTypes.Phone,
                Format = Android.Graphics.Format.Transparent,
                Gravity = GravityFlags.Top | GravityFlags.Left, //原点
                Flags = WindowManagerFlags.NotFocusable,    //不可聚焦
                Width = 100,    //宽度
                Height = 100,   //高度
            };
            this.WindowManager.AddView(this.floatLayout, param);

            TextView floatTextView = new TextView(this)
            {
                Text = "ANS",
            };
            floatLayout.AddView(floatTextView);
        }

        private void Timer_Tick(object sender)
        {
            
        }
    }
}

