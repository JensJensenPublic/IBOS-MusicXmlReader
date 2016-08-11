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
        public bool LogEvent(string s)
        {
            Console.WriteLine(s);
            return false;
        }

        ScreenReaderAPI screenReaderAPI;
        int displaySize = 14;


        private char ToBraille(int i)
        {
            int brailleValue = (0x2800 + i);
            return (char)brailleValue;
        }

        public void IndexChanged(object sender, EventArgs e)
        {

            // Stop the Speak and Braille automatically generated behind the scene
            // screenReaderAPI.Speak("Nu prøver vi at sige en hel masse og se hvad der sker");
            screenReaderAPI.Silence();
            // Get the contents of the selected item   
            ListBox listBox = (sender as ListBox);
            string s = listBox.Items[listBox.SelectedIndex].ToString();

            // Say something different in order to check if the original text was heard !
            // screenReaderAPI.Speak("HEJ");
            // Populate a string with the first char received
            string b = new StringBuilder().Append(string.IsNullOrEmpty(s) ? ' ' : s[0], displaySize).ToString();
            // Braille it
            //System.Threading.Thread.Sleep(1000); // Se what happens when
            bool result = screenReaderAPI.Braille(b, true);
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
            ListBoxLeft.Items.Add(ToBraille(0xff)); // All pins
            ListBoxLeft.Items.Add(ToBraille(0x00)); // No pins
            ListBoxLeft.Items.Add(ToBraille(0x81)); // Pin 1, pin 8
            ListBoxRight.Items.Add("d");
            ListBoxRight.Items.Add("e");
            ListBoxRight.Items.Add("f");
            ListBoxRight.Items.Add(ToBraille(0x01)); // pin 1
            ListBoxRight.Items.Add(ToBraille(0x02)); // pin 2
            ListBoxRight.Items.Add(ToBraille(0x04)); // pin 3
            ListBoxRight.Items.Add(ToBraille(0x40)); // pin 7
            ListBoxRight.Items.Add(ToBraille(0x08)); // pin 4
            ListBoxRight.Items.Add(ToBraille(0x10)); // pin 5
            ListBoxRight.Items.Add(ToBraille(0x20)); // pin 6
            ListBoxRight.Items.Add(ToBraille(0x80)); // pin 8     

        }
    }
}
