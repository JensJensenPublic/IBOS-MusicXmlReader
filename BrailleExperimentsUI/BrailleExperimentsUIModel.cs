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


        private char ToBraille(int i)
        {
            int brailleValue = (0x2800 + i);
            return (char)brailleValue;
        }

        private bool ISBraille(char c)
        {
            return ((c >= 0x2800) && (c <= 0x28ff));
        }

        public void IndexChanged(object sender, EventArgs e)
        {
            // Stop the Speak and Braille automatically generated behind the scene
            screenReaderAPI.Silence();
            screenReaderAPI.Speak("Her siger vi en masse sludder");
            screenReaderAPI.Silence();


            // Get the contents of the selected item   
            ListBox listBox = (sender as ListBox);
            int index = listBox.SelectedIndex;
            TraceLine(string.Format("Index={0}", index));
            string s = listBox.Items[index].ToString();
            if (!string.IsNullOrEmpty(s))
            {
                if (ISBraille((char)s[0]))
                {
                    screenReaderAPI.Silence();
                    string stringToSay = ListBoxRight.Items[listBox.SelectedIndex].ToString();
                    screenReaderAPI.Speak(stringToSay);
                }

            }
      

            // Say something different in order to check if the original text was heard !
            // screenReaderAPI.Speak("HEJ");
            // Populate a string with the first char received
            string b = new StringBuilder().Append(string.IsNullOrEmpty(s) ? ' ' : s[0], displaySize).ToString();
            // Braille it
            //System.Threading.Thread.Sleep(1000); // Se what happens when
            //bool result = screenReaderAPI.Braille(b, true);
        }



        private ListBox ListBoxLeft;
        private ListBox ListBoxRight;


        /// <summary>
        /// Constructor
        /// </summary>
        public BrailleExperimentsUIModel(ListBox listBoxLeft, ListBox listBoxRight)
        {
            this.ListBoxLeft = listBoxLeft;
            this.ListBoxRight = listBoxRight;
            screenReaderAPI = ScreenReaderAPI.Create(false, this as IScreenReaderAPILogger);

            ListBoxLeft.Items.Add("a");
            ListBoxLeft.Items.Add("b");
            ListBoxLeft.Items.Add("c");
            ListBoxLeft.Items.Add(""); // What is reported here?
            ListBoxLeft.Items.Add(ToBraille(0xff)); // No pins   
            ListBoxLeft.Items.Add(ToBraille(0x00)); // No pins
            ListBoxLeft.Items.Add(ToBraille(0x81)); // Pin 1, pin 8
            ListBoxLeft.Items.Add(new StringBuilder().Append(ToBraille(0xff), 14).ToString()); // All pins, 4 of them 
            ListBoxLeft.Items.Add(new StringBuilder().Append(ToBraille(0x00), 4).ToString()); // No pins, 4 of them 
            ListBoxLeft.Items.Add(new StringBuilder().Append(ToBraille(0x81), 4).ToString()); //  Pin 1, pin 8, 4 of them 

            ListBoxRight.Items.Add("Linie0"); // pin 1
            ListBoxRight.Items.Add("Linie1"); // pin 2
            ListBoxRight.Items.Add("Linie2"); // pin 1
            ListBoxRight.Items.Add("Linie3"); // pin 2
            ListBoxRight.Items.Add("Linie4"); // pin 1
            ListBoxRight.Items.Add("Linie5"); // pin 2
            ListBoxRight.Items.Add("Linie6"); // pin 1
            ListBoxRight.Items.Add("Linie7"); // pin 2
            ListBoxRight.Items.Add("Linie8"); // pin 1
            ListBoxRight.Items.Add("Linie9"); // pin 2
            ListBoxRight.Items.Add("Linie10"); // pin 1
            ListBoxRight.Items.Add("Linie11"); // pin 2

            //ListBoxRight.Items.Add(ToBraille(0x04)); // pin 3
            //ListBoxRight.Items.Add(ToBraille(0x40)); // pin 7
            //ListBoxRight.Items.Add(ToBraille(0x08)); // pin 4
            //ListBoxRight.Items.Add(ToBraille(0x10)); // pin 5
            //ListBoxRight.Items.Add(ToBraille(0x20)); // pin 6
            //ListBoxRight.Items.Add(ToBraille(0x80)); // pin 8     

        }
    }
}
