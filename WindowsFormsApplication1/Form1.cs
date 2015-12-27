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
        bool autoScrape = false;
        bool firstTime = true;
        bool firstTimeAuto = true;
        bool manualMode = true;
        

        public Form1()
        {
            InitializeComponent();
            button2.Visible = false;
            checkIfRunning();

        }
        List<string> links = new List<string>();
        List<string> linkQueue = new List<string>();
        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            textBox3.Text = webBrowser1.Url.ToString();
            if(!autoScrape && !firstTime && manualMode) linkListMaker();
            if(autoScrape && links.Count <= numericUpDown1.Value && firstTimeAuto)
            {
                linkListMaker();
                firstTimeAuto = false;
            }
            if(autoScrape && links.Count <= numericUpDown1.Value && !firstTimeAuto)
            {
                if (linkQueue.ElementAt(0) != null && !webBrowser1.IsBusy)
                {
                    webBrowser1.Navigate(new Uri(linkQueue.ElementAt(0)));
                    linkListMaker();
                    linkQueue.RemoveAt(0);
                }
                manualMode = false;
                linkCount.Text = links.Count.ToString();
            }
            checkIfRunning();
            firstTime = false;
        }
        private void linkListMaker()
        {

            HtmlElementCollection link = webBrowser1.Document.GetElementsByTagName("A");
            foreach (HtmlElement i in link)
            {
                links.Add(i.GetAttribute("href").ToString());
                linkQueue.Add(i.GetAttribute("href").ToString());
            }

            links = removeDuplicates(links);
            linkQueue = removeDuplicates(linkQueue);
            textBox1.Clear();

            foreach (var i in links)
            {
                if (i.Contains("http://") || i.Contains("https://"))
                {
                    textBox1.AppendText(i + Environment.NewLine);
                }
            }
            if (links.Count >= numericUpDown1.Value) autoScrape = false;

        }

        private void goButton_Click(object sender, EventArgs e)
        {
            var tempUrl = "";
            if (!textBox3.Text.Contains("http://"))
                tempUrl = "http://" + textBox3.Text.ToString();
            else
                tempUrl = textBox3.Text.ToString();
            webBrowser1.Navigate(new Uri(tempUrl));
            manualMode = true;
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
        private List<string> removeDuplicates(List<string> listIn)
        {
            listIn = listIn.Distinct().ToList();
            return listIn;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button2.Visible = true;
            var tempUrl = "";
            if (!textBox4.Text.Contains("http://"))
                tempUrl = "http://" + textBox4.Text.ToString();
            else
                tempUrl = textBox4.Text.ToString();
            webBrowser1.Navigate(new Uri(tempUrl));
            autoScrape = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            autoScrape = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            autoScrape = false;
            links.Clear();
            linkQueue.Clear();
            textBox1.Clear();
            textBox4.Clear();
            button2.Visible = false;
            linkCount.Text = links.Count.ToString();
            checkIfRunning();
        }

        private void checkIfRunning()
        {
            if(autoScrape)
            {
                isRunningLabel.Text = "Running";
                isRunningLabel.ForeColor = Color.DarkGreen;
            } 
            if(!autoScrape)
            {
                isRunningLabel.Text = "Not Running";
                isRunningLabel.ForeColor = Color.DarkRed;
            }

        }
    }
}
