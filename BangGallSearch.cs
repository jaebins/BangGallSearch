using HtmlAgilityPack;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace most_searched_keyword_Program
{
    public partial class BangGallSearch : Form
    {
        bool isRecommendPost;
        bool repeat_Click;

        public BangGallSearch()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (repeat_Click)
            {
                richTextBox1.Text = "";
            }

            repeat_Click = true;

            /*다른 갤 주소도됨*/ 
            GetGallery("https://gall.dcinside.com/mgallery/board/view/?id=bang_dream");
        }

        private void richTextBox1_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            Process.Start("chrome.exe", e.LinkText);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!isRecommendPost)
            {
                isRecommendPost = true;
                button2.BackColor = Color.White;
                button2.Text = "일반글";
            }
            else
            {
                isRecommendPost = false;
                button2.BackColor = Color.Yellow;
                button2.Text = "개념글";
            }
        }

        public void GetGallery(string targetUrl)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlAgilityPack.HtmlDocument doc;

            string number = string.Empty;
            string title = string.Empty;
            string url = string.Empty;

            int urlLength = targetUrl.IndexOf('=');
            targetUrl = targetUrl.Substring(urlLength, targetUrl.Length - urlLength);

            string postUrl = "https://gall.dcinside.com/mgallery/board/view/?id" + targetUrl;

            if (!isRecommendPost)
            {
                string parseUrl = "https://gall.dcinside.com/mgallery/board/lists?id" + targetUrl;
                doc = web.Load(parseUrl);
            }
            else
            {
                string parseUrl = String.Format("https://gall.dcinside.com/mgallery/board/lists?id{0}&exception_mode=recommend", targetUrl);
                doc = web.Load(parseUrl);
            }

            for (int i = 13; i < 52; i++)
            {
                number = doc.DocumentNode.SelectSingleNode("//*[@id='container']/section[1]/article[2]/div[2]/table/tbody/tr[" + i + "]/td[1]").InnerText;
                title = doc.DocumentNode.SelectSingleNode("//*[@id='container']/section[1]/article[2]/div[2]/table/tbody/tr[" + i + "]/td[3]/a").InnerText;
                url = postUrl + "&no=" + number;
                richTextBox1.AppendText(string.Format("{0}, {1}{2}{3}", title, url, Environment.NewLine, Environment.NewLine));
            }
        }

        public Image GetImage(string url, int index)
        {
            Image image = null;

            if (index == 13)
            {
                string downUrl = "https://search.naver.com/search.naver?where=image&query=%EA%B3%A0%EC%96%91%EC%9D%B4";
                HttpWebRequest request = WebRequest.CreateHttp(downUrl);
                request.CookieContainer = new CookieContainer();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());

                string allhtml = reader.ReadToEnd();
                Console.WriteLine(allhtml);
                int htmlSplit = allhtml.IndexOf("tile_item _item");
                string imgHtml = allhtml.Substring(htmlSplit, allhtml.Length - htmlSplit);

                Match match = Regex.Match(imgHtml, "<img[^>]+>");
                Console.WriteLine(match.Value);
                string[] imgLinkSplit = match.Value.Split('=');
                string imgLink = imgLinkSplit[2];
                Console.WriteLine(imgLink);

                WebClient wc = new WebClient();
                Stream imgStream = wc.OpenRead(imgLink);
                image = Bitmap.FromStream(imgStream);
            }

            return image;
        }
    }
}
