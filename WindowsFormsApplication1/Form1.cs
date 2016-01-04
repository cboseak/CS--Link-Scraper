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
        private SynchronizationContext _syncContext;
        bool autoScrape = false;
        bool firstTime = true;
        bool firstTimeAuto = true;
        bool manualMode = true;
        object _lock = new object();
        static HtmlElementCollection linkCollection;

        public Form1()
        {
            InitializeComponent();
            _syncContext = SynchronizationContext.Current;
            button2.Visible = false;
            checkIfRunning();
            webBrowser1.ScriptErrorsSuppressed = true;
        }
        StringListEnhanced links = new StringListEnhanced();
        StringListEnhanced linkQueue = new StringListEnhanced();

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            textBox3.Text = webBrowser1.Url.ToString();                                     //when each page loads, make sure the address bar has an accurate address

            if (!autoScrape && !firstTime && manualMode) linkListMaker();                   //MANUAL MODE - This hit if you're not in autoscrape mode and you browse. 
                                                                                            //Every page visited pull all links and adds them to the lists

            if (autoScrape && links.Count <= numericUpDown1.Value && firstTimeAuto)         //Autoscape is AND there are less links than requested AND it is the first time around
            {                                                                               //then pull all links from current page, set first time to false, and update the link count
                linkListMaker();
                firstTimeAuto = false;
                updateLinkCount();
            }

            if (autoScrape && links.Count <= numericUpDown1.Value && !firstTimeAuto)        //Autoscape is AND there are less links than requested AND it is at least second time around
            {
                if (linkQueue.Count > 0 && !webBrowser1.IsBusy)
                {
                    webBrowser1.Navigate(new Uri(linkQueue.ElementAt(0)));
                    linkListMaker();
                    linkQueue.Pop();
                }
                manualMode = false;
                updateLinkCount();
            }                                                              //then browse to the link at position zero, pull the links on that page, at remove link at position zero

            checkIfRunning();
            firstTime = false;
        }
        private void linkListMaker()
        {

            UiUpdateHelper.getAllLinksFromBrowser(webBrowser1, _syncContext);

            foreach (HtmlElement i in linkCollection)
            {
                lock (_lock)
                {
                    links.Add(i.GetAttribute("href").ToString());
                }
            }
            foreach (HtmlElement i in linkCollection)
            {
                lock (_lock)
                {
                    linkQueue.Add(i.GetAttribute("href").ToString());
                }
            }

            links.RemoveDuplicate();
            linkQueue.RemoveDuplicate();

            UiUpdateHelper.clearTextbox(textBox1, _syncContext);
            string[] linkArr = convertLines(links);
            UiUpdateHelper.updateTextboxLines(textBox1, linkArr, _syncContext);

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
            });
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
            var url = textBox4.Text;
            do
            {
                var temp = ScraperLogic.scraper(url);
                linkQueue.AddRange(temp);
                links.AddRange(temp);
                links.RemoveDuplicate();
                linkQueue.RemoveDuplicate();
                ScraperLogic.setTextBoxFromArray(textBox1, links.ToArray());
                url = linkQueue.ElementAt(0);
                linkQueue.Pop();
            } while (linkQueue.Count > 0 && links.Count <= numericUpDown1.Value);
         }

        //sets automatic scrape status to false so loop will stop
        private void button2_Click(object sender, EventArgs e)
        {
            autoScrape = false;
        }

        //CLEAR BUTTON - Call clear all function below
        private async void button3_Click(object sender, EventArgs e)
        {
            await Task.Factory.StartNew(() => { clearAll(); }).ConfigureAwait(false);
        }

        //checks if running so that it can update the status label
        private async void checkIfRunning()
        {
            await Task.Factory.StartNew(() =>
            {
                if (autoScrape)
                {
                    UiUpdateHelper.updateLabel(isRunningLabel, "Running", _syncContext);
                    UiUpdateHelper.updateLabel(isRunningLabel, Color.DarkGreen, _syncContext);
                }
                if (!autoScrape)
                {
                    UiUpdateHelper.updateLabel(isRunningLabel, "Not Running", _syncContext);
                    UiUpdateHelper.updateLabel(isRunningLabel, Color.DarkRed, _syncContext);
                }
            });
        }

        //updates label on UI to show the current amount on links scraped
        private void updateLinkCount()
        {
            UiUpdateHelper.updateLabel(linkCount,links.Count.ToString(),_syncContext);
        }
        
        //clear all text boxes and lists
        private void clearAll()
        {
            autoScrape = false;
            links.Clear();
            linkQueue.Clear();
            UiUpdateHelper.clearTextbox(textBox1, _syncContext);
            UiUpdateHelper.clearTextbox(textBox4, _syncContext);
            UiUpdateHelper.updateButtonVisibility(button2, false, _syncContext);
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

        private string[] convertLines(List<string> listIn)
        {
            string[] tempArr = new string[listIn.Count];
            tempArr = listIn.ToArray();
            return tempArr;
        }
        public static void receivesHtmlCollection(HtmlElementCollection collectionIn)
        {
            linkCollection = collectionIn;
        }




    }
}
