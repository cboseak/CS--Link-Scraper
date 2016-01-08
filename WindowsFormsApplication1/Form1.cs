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
using System.Diagnostics;
using System.Collections.Concurrent;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        private SynchronizationContext _syncContext;
        bool autoScrape = false;
        bool firstTime = true;
        bool running = false;
        bool manualMode = true;
        object _lock = new object();
        static HtmlElementCollection linkCollection;
        CancellationTokenSource ct = new CancellationTokenSource();

        public Form1()
        {
            InitializeComponent();
            _syncContext = SynchronizationContext.Current;
            button2.Visible = false;
            checkIfRunning();
            webBrowser1.ScriptErrorsSuppressed = true;
        }

        ConcurrentBag<string> links = new ConcurrentBag<string>();
        ConcurrentQueue<string> linkQueue = new ConcurrentQueue<string>();

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

            textBox3.Text = webBrowser1.Url.ToString();                                   //when each page loads, make sure the address bar has an accurate address

            if (!autoScrape && !firstTime && manualMode) linkListMaker();                   //MANUAL MODE - This hit if you're not in autoscrape mode and you browse. 
            //Every page visited pull all links and adds them to the lists

            checkIfRunning();
            firstTime = false;
        }
        private  void linkListMaker()
        {
            HtmlElementCollection temp = webBrowser1.Document.GetElementsByTagName("A");
            Form1.receivesHtmlCollection(temp);

            foreach (HtmlElement i in linkCollection)
            {
                lock (_lock)
                {
                    links.Add(i.GetAttribute("href").ToString());
                }
            }

            Thread t1 = new Thread(new ThreadStart(() =>
            {
                links = new ConcurrentBag<string>(links.Distinct().ToList());
                UiUpdateHelper.clearTextbox(textBox1, _syncContext);
                string[] linkArr = links.ToArray();
                UiUpdateHelper.updateTextboxLines(textBox1, linkArr, _syncContext);
                if (links.Count >= numericUpDown1.Value) autoScrape = false;
            }));
            t1.Start();
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
        private async void button1_Click(object sender, EventArgs e)
        {
            if (textBox4.Text.Length > 4)
            {
                UiUpdateHelper.updateButtonVisibility(button2, true, _syncContext);
                ct = new CancellationTokenSource();
                StringListEnhanced tempList = new StringListEnhanced();
                string url = textBox4.Text;
                autoScrape = true;
                checkIfRunning();
                await Task.Run(() =>
                {
                    do
                    {
                        tempList = ScraperLogic.scraper(url);

                        foreach (var i in tempList)
                        {
                            linkQueue.Enqueue(i);
                            links.Add(i);
                        }
                        links = new ConcurrentBag<string>(links.Distinct().ToList());
                        BeginInvoke(new Action(() => { ScraperLogic.setTextBoxFromArray(textBox1, links.ToArray()); }));
                        linkQueue.TryDequeue(out url);
                        BeginInvoke(new Action(() => { updateLinkCount(); }));
                    } while (links.Count <= numericUpDown1.Value && autoScrape == true);
                }, ct.Token);
                ct.Cancel();
                autoScrape = false;
                UiUpdateHelper.updateButtonVisibility(button2, false, _syncContext);
                checkIfRunning();
                ScraperLogic.setTextBoxFromArray(textBox1, links.ToArray());
            }
        }

        //sets automatic scrape status to false so loop will stop
        private void button2_Click(object sender, EventArgs e)
        {
            autoScrape = false;
            checkIfRunning();
            ct.Cancel();
            ct = new CancellationTokenSource();
            UiUpdateHelper.updateButtonVisibility(button2, false, _syncContext);
        }

        //CLEAR BUTTON - Call clear all function below
        private async void button3_Click(object sender, EventArgs e)
        {
            await Task.Factory.StartNew(() => { clearAll(); });
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
            links = new ConcurrentBag<string>();
            linkQueue = new ConcurrentQueue<string>();
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
