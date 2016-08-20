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
        //int displaySize = 14;

        private char ToBraille(int i)
        {
            int brailleValue = (0x2800 + i);
            return (char)brailleValue;
        }

        private bool ISBraille(char c)
        {
            return ((c >= 0x2800) && (c <= 0x28ff));
        }

        public void FocusLost()
        {
            TraceLine("FocusLost");
            if (isNVDA)
            {
                screenReaderAPI.StopRefreshing();
            }
        }

        //public void MusicBrailleListBoxIndexChanged()
        //{   
        //}

        public void TextBrailleListBoxIndexChanged()
        {
            // At this time the screenreader has already written the selected line to the physical Braille display and started speaking!
            BrailleExperimentsLineDescription item = ListBoxRight.Items[ListBoxRight.SelectedIndex] as BrailleExperimentsLineDescription;
            textBoxBraille.Text = item.ToMusicBrailleString(); // Always show the MusicBraille in the textbox.
            if (isNVDA)
            {
                // NVDA will read the MusicBraille characters as "Braille 1,2,3,4,5,6,7,8"
                // So in the NVDA case the MusicBraille characters must NOT shown in the listbox.
                // The MusicBraille characteres are thus not automatically shown on the Braille display.
                // Instead we must explicitly write them to the Braille display:
                screenReaderAPI.Braille(item.ToMusicBrailleAndTextBrailleString(),true);
            }
                     
        }
   
        private ListBox ListBoxRight;
        private TextBox textBoxBraille;

        private bool isNVDA;
        private bool isJAWS;
        private bool isDummy;  

        private void add(string musicBraille, string textBraille, string format)
        {
              ListBoxRight.Items.Add(BrailleExperimentsLineDescription.Create(musicBraille, textBraille, format));
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public BrailleExperimentsUIModel(ListBox listBoxRight,TextBox textBoxBraille)
        {
            this.ListBoxRight = listBoxRight;
            this.textBoxBraille = textBoxBraille;
            screenReaderAPI = ScreenReaderAPI.Create(false, this as IScreenReaderAPILogger);
            isNVDA = (screenReaderAPI.GetScreenReaderType() == ScreenReaderAPI.ScreenReaderType.NVDA);
            isJAWS = (screenReaderAPI.GetScreenReaderType() == ScreenReaderAPI.ScreenReaderType.JAWS);
            isDummy = (screenReaderAPI.GetScreenReaderType() == ScreenReaderAPI.ScreenReaderType.Dummy);
            string format = isNVDA ? "{1}" : "{0} {1}"; // Might later depend on screenReaderAPI.GetScreenReaderType()

            for (int i = 0; (i < 256); i++)
            {
                add(new StringBuilder().Append(ToBraille(i), 4).ToString(),string.Format("L{0}", i),format);
            }
            
        }
    }
}
