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

        /*private string FindInQuest()
        {
            //foreach(string )
        }*/
    }
}

