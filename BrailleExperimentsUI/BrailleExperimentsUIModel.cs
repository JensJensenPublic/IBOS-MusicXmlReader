using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JSJ.ScreenReaderAPI;
using System.Threading;

namespace BrailleExperimentsUI
{
    public class BrailleExperimentsUIModel: IScreenReaderAPILogger
    {
        bool traceNeedsNewLine;
        int selectedIndex;

        #region  IScreenReaderAPILogger
        public bool LogEvent(string s)
        {
            Console.WriteLine(s);
            return false;
        }

        public bool TraceLine(string s)
        {
            if (traceNeedsNewLine)
            {
                Console.WriteLine();
            }
            traceNeedsNewLine = false;
            Console.WriteLine(s);
            return true;
        }

        public bool TraceChar(char s)
        {
            traceNeedsNewLine = true;
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
            selectedIndex = ListBoxLeft.SelectedIndex;
            //ListBoxRight.SelectedIndex = selectedIndex;      


            // Always show the contents in the textbox
            // This variant will first show the automatic line for a moment
            // Then it will show the braille pattern exactly as wanted !
            // If the time for flash messages is set to "forever" this version works perfect! But we probably cannot allow this as
            // add global setting. Is it possible to control this settint dynamically from the application ?
            //screenReaderAPI.Braille(ListBoxLeft.Items[selectedIndex].ToString(), true); 


            // This variant will first show the automatic line for a moment
            // It will show the prefix "lbX"
            // It will turn on dots 7 and 8 for all the characters

            int append = 0; // Replace
            int time = 1000; // Keep 
            // screenReaderAPI.Braille(ListBoxLeft.Items[selectedIndex].ToString(), true,append, time ); // This variant will also first show the automatic line for a moment


            // The automatic rendering will show the "lbX" prefix and will turn on dots 7 and 8 but will NOT show another line for a moment, and thus looks better


            textBoxBraille.Text = ListBoxLeft.Items[selectedIndex].ToString();

            // Always speek the contents of the TextBrailleListbox
            screenReaderAPI.Silence(); // Stop the screenreader from speaking the  Braille value!
            screenReaderAPI.Speak(ListBoxRight.Items[selectedIndex].ToString());
            screenReaderAPI.Silence();

        }

        public void TextBrailleListBoxIndexChanged()
        {
            if (!ListBoxRight.Focused) return; // Avoid looping
            selectedIndex = ListBoxRight.SelectedIndex;
            //ListBoxLeft.SelectedIndex = selectedIndex ;
            string brailleText;
            if (isNVDA || isDummy )
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
                ListBox listbox = ListBoxLeft;
                brailleText = listbox.Items[selectedIndex].ToString();
                //screenReaderAPI.Braille(brailleText, true); // The preferred solution !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                //screenReaderAPI.Braille(brailleText,true, 0, 5000);
                textBoxBraille.Text = brailleText;
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
            //ListBoxRight.Items.Add(textBraille);
            ListBoxRight.Items.Add(string.Format("{0} {1}",musicBraille,textBraille));
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public BrailleExperimentsUIModel(ListBox listBoxLeft, ListBox listBoxRight,TextBox textBoxBraille,CheckBox checkBoxbrailleSelector)
        {
            this.ListBoxLeft = listBoxLeft;
            this.ListBoxRight = listBoxRight;
            this.textBoxBraille = textBoxBraille;
            screenReaderAPI = ScreenReaderAPI.Create(false, this as IScreenReaderAPILogger);
            isNVDA = (screenReaderAPI.GetScreenReaderType() == ScreenReaderAPI.ScreenReaderType.NVDA);
            isJAWS = (screenReaderAPI.GetScreenReaderType() == ScreenReaderAPI.ScreenReaderType.JAWS);
            isDummy = (screenReaderAPI.GetScreenReaderType() == ScreenReaderAPI.ScreenReaderType.Dummy);

            this.ListBoxLeft.Visible = !isNVDA;
            checkBoxbrailleSelector.Visible = isNVDA;

            // ListBoxLeft.Enabled= (ScreenReaderAPI.ScreenReaderType.NVDA != screenReaderAPI.GetScreenReaderType());
            // NVDA insists on reading Braille pin combinations, so we do not want to enable the listbox containing the Music-Braille codes

            for (int i = 0; (i < 256); i++)
            {
                add(new StringBuilder().Append(ToBraille(i), 4).ToString(),string.Format("L{0}", i));
            }
            
        }
    }
}
