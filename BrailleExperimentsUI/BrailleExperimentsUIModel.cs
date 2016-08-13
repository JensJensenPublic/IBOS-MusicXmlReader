using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JSJ.ScreenReaderAPI;

namespace BrailleExperimentsUI
{
    public class BrailleExperimentsUIModel: IScreenReaderAPILogger
    {

        #region  IScreenReaderAPILogger
        public bool LogEvent(string s)
        {
            Console.WriteLine(s);
            return false;
        }

        public bool TraceLine(string s)
        {
            Console.WriteLine(s);
            return true;
        }

        public bool TraceChar(char s)
        {
            Console.Write(s);
            return true;
        }
        #endregion

        ScreenReaderAPI screenReaderAPI;
        int displaySize = 14;
        bool renderAsMusicBraille;


        private char ToBraille(int i)
        {
            int brailleValue = (0x2800 + i);
            return (char)brailleValue;
        }

        private bool ISBraille(char c)
        {
            return ((c >= 0x2800) && (c <= 0x28ff));
        }

        public void RenderAsMusicBrailleChanged(bool newValue)
        {
            TraceLine(string.Format("RenderAsMusicBrailleChanged({0})", newValue));
            renderAsMusicBraille = newValue;
            if (isNVDA && (!newValue)) screenReaderAPI.StopRefreshing();
        }

        public void FocusLost()
        {
            TraceLine("FocusLost");
            if (isNVDA && (!ListBoxLeft.Focused) && (!ListBoxLeft.Focused))
            {
                screenReaderAPI.StopRefreshing();
            }
        }

        public void MusicBrailleListBoxIndexChanged()
        {
            if (!ListBoxLeft.Focused) return; // Avoid looping
            int selectedIndex = ListBoxLeft.SelectedIndex;
            ListBoxRight.SelectedIndex = selectedIndex;
            // Always speek the contents of the TextBrailleListbox
            screenReaderAPI.Silence(); // Stop the screenreader from speaking the  Braille value!
            screenReaderAPI.Speak(ListBoxRight.Items[selectedIndex].ToString());
            screenReaderAPI.Silence();
            // Always show the contents in the textbox
            textBoxBraille.Text = ListBoxLeft.Items[selectedIndex].ToString();
        }

        public void TextBrailleListBoxIndexChanged()
        {
            if (!ListBoxRight.Focused) return; // Avoid looping
            int selectedIndex = ListBoxRight.SelectedIndex;
            ListBoxLeft.SelectedIndex = selectedIndex ;
            string brailleText;
            if (isNVDA || isDummy)
            {
                // In the NVDA case we expect that only the text Listbox is visible
                // because we can't prevent the speak of "Braille 1 2 3 4 5 6 7 8" from the MusicBrailleListbox
                // We must give the user an other way ofselecting what he wants to be brailled in that case.
                ListBox listbox = (renderAsMusicBraille) ? ListBoxLeft : ListBoxRight;
                brailleText = listbox.Items[selectedIndex].ToString();
                screenReaderAPI.Braille(brailleText, true);
                textBoxBraille.Text = renderAsMusicBraille ? brailleText : "";
            }
            else
            {
                // The screanreader has already written to the Braille Display, but we dont know exactly
                // how it was mapped,somwe only flush the textbox:      
                textBoxBraille.Text = "";
            }
          
        }

        private ListBox ListBoxLeft;
        private ListBox ListBoxRight;
        private TextBox textBoxBraille;

        private bool isNVDA;
        private bool isJAWS;
        private bool isDummy;


        private void add(string musicBraille, string textBraille)
        {
            ListBoxLeft.Items.Add(musicBraille);
            ListBoxRight.Items.Add(textBraille);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public BrailleExperimentsUIModel(ListBox listBoxLeft, ListBox listBoxRight,TextBox textBoxBraille)
        {
            this.ListBoxLeft = listBoxLeft;
            this.ListBoxRight = listBoxRight;
            this.textBoxBraille = textBoxBraille;
            screenReaderAPI = ScreenReaderAPI.Create(false, this as IScreenReaderAPILogger);
            isNVDA = (screenReaderAPI.GetScreenReaderType() == ScreenReaderAPI.ScreenReaderType.NVDA);
            isJAWS = (screenReaderAPI.GetScreenReaderType() == ScreenReaderAPI.ScreenReaderType.JAWS);
            isDummy = (screenReaderAPI.GetScreenReaderType() == ScreenReaderAPI.ScreenReaderType.Dummy);

            // ListBoxLeft.Enabled= (ScreenReaderAPI.ScreenReaderType.NVDA != screenReaderAPI.GetScreenReaderType());
            // NVDA insists on reading Braille pin combinations, so we do not want to enable the listbox containing the Music-Braille codes

            for (int i = 0; (i < 256); i++)
            {
                add(new StringBuilder().Append(ToBraille(i), displaySize).ToString(),string.Format("Linie {0}", i));
            }
            
        }
    }
}
