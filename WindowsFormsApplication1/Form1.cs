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
        bool autoScrape = false;        bool firstTime = true;
        bool firstTimeAuto = true;
        bool manualMode = true;
        object _lock = new object();

        public Form1()
        {
            InitializeComponent();
            button2.Visible = false;
            checkIfRunning();

        }
        StringListEnhanced links = new StringListEnhanced();
        StringListEnhanced linkQueue = new StringListEnhanced();

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            //when each page loads, make sure the address bar has an accurate address
            textBox3.Text = webBrowser1.Url.ToString();
            
            //MANUAL MODE - This hit if you're not in autoscrape mode and you browse. 
            //Every page visited pull all links and adds them to the lists
            if(!autoScrape && !firstTime && manualMode) linkListMaker();
            
            //Autoscape is AND there are less links than requested AND it is the first time around
            //then pull all links from current page, set first time to false, and update the link count
            if(autoScrape && links.Count <= numericUpDown1.Value && firstTimeAuto)
            {
                linkListMaker();
                firstTimeAuto = false;
                updateLinkCount();
            }
            
            //Autoscape is AND there are less links than requested AND it is at least second time around
            //then browse to the link at position zero, pull the links on that page, at remove link at position zero
            if(autoScrape && links.Count <= numericUpDown1.Value && !firstTimeAuto)
            {
                if (linkQueue.Count >= 1 && !webBrowser1.IsBusy)
                {
                    webBrowser1.Navigate(new Uri(linkQueue.ElementAt(0)));
                    linkListMaker();
                    linkQueue.Pop();
                }
                manualMode = false;
                updateLinkCount();
            }
            checkIfRunning();
            firstTime = false;
        }
        private void linkListMaker()
        {
            HtmlElementCollection link = webBrowser1.Document.GetElementsByTagName("A");

            Parallel.ForEach(link.Cast<HtmlElement>(), i =>
            {
                links.Add(i.GetAttribute("href").ToString());
                linkQueue.Add(i.GetAttribute("href").ToString());
            });

            links.RemoveDuplicate();
            linkQueue.RemoveDuplicate();

            textBox1.Clear();

            string[] linkArr = convertLinesAsync(links).Result;

            textBox1.Lines = linkArr;

 
            if (links.Count >= numericUpDown1.Value) autoScrape = false;

        }

        //GO BUTTON on Manual Page    
        private async void goButton_Click(object sender, EventArgs e)
        {
            var tempUrl = "";
            await Task.Factory.StartNew(() => {
                if (!textBox3.Text.Contains("http://"))
                    tempUrl = "http://" + textBox3.Text.ToString();
                else
                    tempUrl = textBox3.Text.ToString();
                manualMode = true;
            }).ConfigureAwait(false);

            webBrowser1.Navigate(new Uri(tempUrl));

        }
        
        //BACK BUTTON on Manual Page
        private void backButton_Click(object sender, EventArgs e)
        {
            webBrowser1.GoBack();
        }

        //FORWARD BUTTON on Manual Page
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

        //GO BUTTON on Auto Page 
        private void button1_Click(object sender, EventArgs e)
        {

            if(links.Count >= numericUpDown1.Value)
            {
                MessageBox.Show("Amount of links exceeds number of links requested, increase request amount or clear and try again");
            }
            button2.Visible = true;
            var tempUrl = "";
            if (!textBox4.Text.Contains("http://"))
                tempUrl = "http://" + textBox4.Text.ToString();
            else
                tempUrl = textBox4.Text.ToString();
            try
            {
                webBrowser1.Navigate(new Uri(tempUrl));
                autoScrape = true;
            }
            catch
            {
                MessageBox.Show("An Error Occured");
                autoScrape = false;
                button2.Visible = false;
            }
        }

        //sets automatic scrape status to false so loop will stop
        private void button2_Click(object sender, EventArgs e)
        {
            autoScrape = false;
        }

        //CLEAR BUTTON - Call clear all function below
        private void button3_Click(object sender, EventArgs e)
        {
            clearAll();
        }

        //checks if running so that it can update the status label
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

        //updates label on UI to show the current amount on links scraped
        private void updateLinkCount()
        {
            linkCount.Text = links.Count.ToString();
        }
        
        //clear all text boxes and lists
        private void clearAll()
        {
            autoScrape = false;
            links.Clear();
            linkQueue.Clear();
            textBox1.Clear();
            textBox4.Clear();
            button2.Visible = false;
            updateLinkCount();
            checkIfRunning();
        }
        
        //if enter is hit in the address bar, click go
        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == (char)Keys.Enter)
            {
                button1_Click(null, e);
            }
        }

        private async Task<string[]> convertLinesAsync(List<string> listIn)
        {
            string[] tempArr = new string[listIn.Count];
            await Task.Factory.StartNew(() => {
                tempArr = listIn.ToArray();
            }).ConfigureAwait(false);

            return tempArr;
        }
    }
}
