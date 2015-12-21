using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

        }
        List<string> links = new List<string>();
        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            textBox3.Text = webBrowser1.Url.ToString();
            linkListMaker();
        }
        private void linkListMaker()
        {

            HtmlElementCollection link = webBrowser1.Document.GetElementsByTagName("A");
            foreach (HtmlElement i in link)
            {
                    links.Add(i.GetAttribute("href").ToString());
            }

            links = links.Distinct().ToList<string>(); 
            textBox1.Clear();

            foreach (var i in links)
            {
                if (i.Contains("http://") || i.Contains("https://"))
                {
                    textBox1.AppendText(i + Environment.NewLine);
                }
            }
        }

        private void goButton_Click(object sender, EventArgs e)
        {
            var tempUrl = "";
            if (!textBox3.Text.Contains("http://"))
                tempUrl = "http://" + textBox3.Text.ToString();
            else
                tempUrl = textBox3.Text.ToString();
            webBrowser1.Navigate(new Uri(tempUrl));
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            webBrowser1.GoBack();
        }

        private void forwardButton_Click(object sender, EventArgs e)
        {
            webBrowser1.GoForward();
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                goButton_Click(null, e);
                textBox3.Clear();
            }

        }
    }
}
