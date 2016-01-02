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
    class UiUpdateHelper
    {
        //update any label text on ui thread by passing the label, the string for text, and the SyncrhonizationContext of the UI thread
        internal static void updateLabel(Label labelToChange, string labelText, SynchronizationContext _syncContext)
        {
            _syncContext.Post(new SendOrPostCallback(o =>
            {
                labelToChange.Text = (string)o;
            }), labelText);
        }

        //update any label color on ui thread by passing the label, the color, and the SyncrhonizationContext of the UI thread
        internal static void updateLabel(Label labelToChange, Color labelColor, SynchronizationContext _syncContext)
        {
            _syncContext.Post(new SendOrPostCallback(o =>
            {
                labelToChange.ForeColor = (Color)o;
            }), labelColor);
        }

        //appends any textbox on ui thread by passing the textbox, the string, and the SynchronizationContext of the ui thread. By default, this will insert a new line after each
        internal static void appendTextbox(TextBox tbToChange, string stringToAppend, SynchronizationContext _syncContext)
        {
            //if newline is not specified as a parameter, this defaults to true.
            appendTextbox(tbToChange, stringToAppend, _syncContext, true);
        }

        //appends any textbox on ui thread by passing the textbox, the string, the SynchronizationContext of the ui thread, and whether to insert a new line
        internal static void appendTextbox(TextBox tbToChange, string stringToAppend, SynchronizationContext _syncContext, bool newLineAfterEach)
        {
            _syncContext.Post(new SendOrPostCallback(o =>
            {
                tbToChange.AppendText(stringToAppend);
                if (newLineAfterEach)
                {
                    tbToChange.AppendText(Environment.NewLine);
                }
            }), stringToAppend);
        }

        //updates any textbox on ui thread by passing the textbox, a string array, and the SynchronizationContext of the ui thread.
        internal static void updateTextboxLines(TextBox tbToChange, string[] stringArrToUpdate, SynchronizationContext _syncContext)
        {
            _syncContext.Post(new SendOrPostCallback(o =>
            {
                tbToChange.Lines = (string[])o;
            }), stringArrToUpdate);
        }

        //sets button visibility on ui thread by passing the button, whether visible, and the SynchronizationContext of the ui thread.
        internal static void updateButtonVisibility(Button buttonToUpdate, bool isVisible, SynchronizationContext _syncContext)
        {
            _syncContext.Post(new SendOrPostCallback(o =>
            {
                if ((bool)o)
                    buttonToUpdate.Visible = true;
                else if (!(bool)o)
                    buttonToUpdate.Visible = false;
            }),isVisible);
        }

        public static void clearTextbox(TextBox textboxToClear, SynchronizationContext _syncContext)
        {
            _syncContext.Post(new SendOrPostCallback(o =>
            {
                textboxToClear.Clear();

            }),textboxToClear);
        }

        public static void getAllLinksFromBrowser(WebBrowser wbIn, SynchronizationContext _syncContext)
        {
       
            _syncContext.Send(new SendOrPostCallback(o =>
            {
                HtmlElementCollection temp = wbIn.Document.GetElementsByTagName("A");
                Form1.receivesHtmlCollection(temp);
            }),wbIn);
        }


    }
}
