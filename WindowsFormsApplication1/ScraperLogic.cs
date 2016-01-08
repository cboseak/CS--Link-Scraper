using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    class ScraperLogic
    {
        static Regex urlPattern = new Regex(@"^http(s)?://([\w-]+.)+[\w-]+(/[\w- ./?%&=])?$");
        static Regex urlPatternEnhanced = new Regex("http://([\\w+?\\.\\w+])+([a-zA-Z0-9\\~\\!\\@\\#\\$\\%\\^\\&amp;\\*\\(\\)_\\-\\=\\+\\\\\\/\\?\\.\\:\\;\\'\\,]*)?", RegexOptions.IgnoreCase);
        static string lastUrl = "";
        internal static StringListEnhanced scraper(string url)
        {
            url = urlFormatChecker(url);
            Uri uri;
            string outputHtml = getHtml(url);
            try
            {
                uri = new Uri(url);
                lastUrl = url;
            }
            catch
            {
                uri = new Uri(lastUrl);
            }

            var host = uri.Host;
            url = urlFormatChecker(host.ToString());

            return Find(outputHtml,url);
        }

        internal static string getHtml(string url)
        {
            WebClient wc = new WebClient();
            try
            {
                return wc.DownloadString(url);
            }
            catch
            {
                return "";
            }
        }

        internal static void setTextBox(TextBox tbToChange, string textToSet)
        {
            tbToChange.Text = textToSet;
        }

        internal static StringListEnhanced checkHtmlForLinks(string stringToCheck, string regexToCheck)
        {

            StringListEnhanced temp = new StringListEnhanced();
            var matches = Regex.Matches(stringToCheck, urlPatternEnhanced.ToString());
            foreach (var i in matches)
            {
                temp.Add(i.ToString());
            }
            temp.RemoveDuplicate();
            return temp;
        }

        internal static void setTextBoxFromArray(TextBox tbToChange, String[] stringArrIn)
        {
            tbToChange.Lines = stringArrIn;
        }

        internal static string urlFormatChecker(string urlToCheck)
        {
            if (!urlToCheck.Contains("http://") && !urlToCheck.Contains("https://"))
                return "http://" + urlToCheck;
            else
                return urlToCheck;
        }

        public static StringListEnhanced Find(string file, string baseUrl)
        {
            StringListEnhanced list = new StringListEnhanced();

            // 1.
            // Find all matches in file.
            MatchCollection m1 = Regex.Matches(file, @"(<a.*?>.*?</a>)",
                RegexOptions.Singleline);

            // 2.
            // Loop over each match.
            foreach (Match m in m1)
            {
                string value = m.Groups[1].Value;
                LinkItem i = new LinkItem();

                // 3.
                // Get href attribute.
                Match m2 = Regex.Match(value, @"href=\""(.*?)\""",
                RegexOptions.Singleline);
                if (m2.Success)
                {
                    i.Href = m2.Groups[1].Value;
                }
                string t = Regex.Replace(value, @"\s*<.*?>\s*", "",
                RegexOptions.Singleline);
                i.Text = t;

                if (("" + i.Href).Length < 2)
                {
                    list.Add(baseUrl);
                }
                else if(("" + i.Href).Contains("http"))
                {
                    list.Add(i.Href);
                }
                else
                {
                    list.Add(baseUrl + i.Href);
                }
                //}
                //else
                //{
                //    list.Add(baseUrl);
                //}
            }
            return list;
        }   
        public struct LinkItem
        {
            public string Href;
            public string Text;

            public override string ToString()
            {
                return Href + "\n\t" + Text;
            }
        }
    }
}
