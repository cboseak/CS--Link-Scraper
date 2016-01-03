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
        static StringListEnhanced listOfLinks = new StringListEnhanced();
        static StringListEnhanced linkQueue = new StringListEnhanced();

        internal static StringListEnhanced scraper(string url)
        {
            url = urlFormatChecker(url);
            string outputHtml = getHtml(url);
            return checkHtmlForLinks(outputHtml, urlPatternEnhanced.ToString());
        }

        internal static string getHtml(string url)
        {
            WebClient wc = new WebClient();
            return wc.DownloadString(url);
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
            if(!urlToCheck.Contains("http://") && !urlToCheck.Contains("https://"))
                return "http://" + urlToCheck;
            else
                return urlToCheck;
        }

    }
}
