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
            ListBoxRight.SelectedIndex = ListBoxLeft.SelectedIndex;
            // Always speek the contents of the TextBrailleListbox
            screenReaderAPI.Silence(); // Stop the screenreader from speaking the  Braille value!
            screenReaderAPI.Speak(ListBoxRight.Items[ListBoxRight.SelectedIndex].ToString());
            screenReaderAPI.Silence();
        }

        public void TextBrailleListBoxIndexChanged()
        {
            if (!ListBoxRight.Focused) return; // Avoid looping
            ListBoxLeft.SelectedIndex = ListBoxRight.SelectedIndex ;
            if (isNVDA)
            {
                // In the NVDA case we expect that only the text Listbox is visible
                // because we can't prevent the speak of "Braille 1 2 3 4 5 6 7 8" from the MusicBrailleListbox
                // We must give the user an other way ofselecting what he wants to be brailled in that case.
                ListBox listbox = (isNVDA && renderAsMusicBraille) ? ListBoxLeft : ListBoxRight;
                screenReaderAPI.Braille(listbox.Items[ListBoxRight.SelectedIndex].ToString(), true);
            }

        }


        //public void IndexChanged(ListBox thisListBox, ListBox theOtherListbox)
        //{
        //    // Prevent looping !
        //    if(!(thisListBox.Focused))
        //        return;

        //    // Stop the Speak and Braille automatically generated behind the scene
        //    screenReaderAPI.Silence();
        //    screenReaderAPI.Speak("Her siger vi en masse sludder");
        //    screenReaderAPI.Silence();

 
        //    // Synchronize the listboxes
        //    theOtherListbox.SelectedIndex = thisListBox.SelectedIndex;


        //    ListBox listBox = thisListBox;


        //    TraceLine(string.Format("Index={0}", index));
        //    string s = listBox.Items[index].ToString();
        //    if (!string.IsNullOrEmpty(s))
        //    {
        //        if (ISBraille((char)s[0]))
        //        {
        //            // If the contents is Braille, speak from the same item in the other listbox instead
        //            screenReaderAPI.Silence();
        //            string stringToSay = theOtherListbox.Items[listBox.SelectedIndex].ToString();
        //            screenReaderAPI.Speak(stringToSay);
        //        }
        //    }
      

        //    // Say something different in order to check if the original text was heard !
        //    // screenReaderAPI.Speak("HEJ");
        //    // Populate a string with the first char received
        //    string b = new StringBuilder().Append(string.IsNullOrEmpty(s) ? ' ' : s[0], displaySize).ToString();
        //    // Braille it
        //    //System.Threading.Thread.Sleep(1000); // Se what happens when
        //    bool result = screenReaderAPI.Braille(b, true); // Only needed in the NVDA case!
        //}



        private ListBox ListBoxLeft;
        private ListBox ListBoxRight;

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
        public BrailleExperimentsUIModel(ListBox listBoxLeft, ListBox listBoxRight)
        {
            this.ListBoxLeft = listBoxLeft;
            this.ListBoxRight = listBoxRight;
            screenReaderAPI = ScreenReaderAPI.Create(false, this as IScreenReaderAPILogger);
            isNVDA = (screenReaderAPI.GetScreenReaderType() == ScreenReaderAPI.ScreenReaderType.NVDA);
            isJAWS = (screenReaderAPI.GetScreenReaderType() == ScreenReaderAPI.ScreenReaderType.JAWS);
            isDummy = (screenReaderAPI.GetScreenReaderType() == ScreenReaderAPI.ScreenReaderType.Dummy);

            // ListBoxLeft.Enabled= (ScreenReaderAPI.ScreenReaderType.NVDA != screenReaderAPI.GetScreenReaderType());
            // NVDA insists on reading Braille pin combinations, so we do not want to enable the listbox containing the Music-Braille codes

            for (int i = 0; (i < 256); i++)
            {
                add(new StringBuilder().Append(ToBraille(i), 14).ToString(),string.Format("Linie {0}", i));
            }

            //ListBoxLeft.Items.Add("a");   
            //ListBoxLeft.Items.Add("b");
            //ListBoxLeft.Items.Add("c");
            //ListBoxLeft.Items.Add(""); // What is reported here?
            //ListBoxLeft.Items.Add(ToBraille(0xff)); // No pins   
            //ListBoxLeft.Items.Add(ToBraille(0x00)); // No pins
            //ListBoxLeft.Items.Add(ToBraille(0x81)); // Pin 1, pin 8
            //ListBoxLeft.Items.Add(new StringBuilder().Append(ToBraille(0xff), 14).ToString()); // All pins, 4 of them 
            //ListBoxLeft.Items.Add(new StringBuilder().Append(ToBraille(0x00), 4).ToString()); // No pins, 4 of them 
            //ListBoxLeft.Items.Add(new StringBuilder().Append(ToBraille(0x81), 4).ToString()); //  Pin 1, pin 8, 4 of them 

            //ListBoxRight.Items.Add("Linie0"); // pin 1
            //ListBoxRight.Items.Add("Linie1"); // pin 2
            //ListBoxRight.Items.Add("Linie2"); // pin 1
            //ListBoxRight.Items.Add("Linie3"); // pin 2
            //ListBoxRight.Items.Add("Linie4"); // pin 1
            //ListBoxRight.Items.Add("Linie5"); // pin 2
            //ListBoxRight.Items.Add("Linie6"); // pin 1
            //ListBoxRight.Items.Add("Linie7"); // pin 2
            //ListBoxRight.Items.Add("Linie8"); // pin 1
            //ListBoxRight.Items.Add("Linie9"); // pin 2

            ////ListBoxRight.Items.Add(ToBraille(0x04)); // pin 3
            ////ListBoxRight.Items.Add(ToBraille(0x40)); // pin 7
            ////ListBoxRight.Items.Add(ToBraille(0x08)); // pin 4
            ////ListBoxRight.Items.Add(ToBraille(0x10)); // pin 5
            ////ListBoxRight.Items.Add(ToBraille(0x20)); // pin 6
            ////ListBoxRight.Items.Add(ToBraille(0x80)); // pin 8     

        }
    }
}
