using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

//Unicode for Braille
//https://en.wikipedia.org/wiki/Braille_Patterns
//http://www.unicode.org/charts/PDF/U2800.pdf

namespace MusicXmlReaderUI
{

    /// <summary>
    /// Similar function to MusicPlyuer
    /// </summary>
    class BrailleDisplayer
    {
        public static char UnicodeBrailleBase = (char)0x2800;

        private FSBrlDspAPIWrapper fSBrlDspAPIWrapper;
        private TextBox musicBrailleTextBox; // The textbox used for writing MusicBraille bytes, repredsented as UniCode
        private string emptyBrailleString;
        private NvdaControllerClientWrapper nvda;
        private int displaySize;
        private string latestMusicBrailleString = string.Empty;

         private BrailleDisplayer(TextBox tb, int displaySize)
        {
            musicBrailleTextBox = tb;
            this.displaySize = displaySize;
            fSBrlDspAPIWrapper = FSBrlDspAPIWrapper.Create(); // For direct access to physical Braille Display
            fSBrlDspAPIWrapper.Open();
            nvda = NvdaControllerClientWrapper.Create(); // For access to physical Braille Display through NVDA            

            StringBuilder sb = new StringBuilder();
            for (int i = 0; (i < this.displaySize); i++) { sb.Append(UnicodeBrailleBase);};
            emptyBrailleString = sb.ToString();
        }

  
        public static BrailleDisplayer Create(TextBox tb,int displaySize)
        {
             return new BrailleDisplayer(tb,displaySize);
        }



        private string BytesToString(List<byte> bytes, int displaySize)
        {
            StringBuilder musicBrailleStringBuilder = new StringBuilder();
            foreach (byte b in bytes)
            {
                musicBrailleStringBuilder.Append((char)(UnicodeBrailleBase + (char)b));
            }
            int size = musicBrailleStringBuilder.Length;
            if ( size < displaySize)
            {
                // First the typial case
                return musicBrailleStringBuilder.Append(UnicodeBrailleBase, (displaySize - size)).ToString();
            }
            else
            {
                return musicBrailleStringBuilder.ToString(0, displaySize);
            }
        }



        /// <summary>
        /// Used when the playing manually.
        /// The User selects a note at a time. 
        /// or
        /// The user selects an EventDescription at a time. This may contain several notes to be played simultaneously
        /// </summary>
        /// <param name="selectedIndex"></param>
        /// <param name="selectedObject"></param>
        internal void SelectedIndexChanged(int selectedIndex, object selectedObject)
        {
            StopRefreshing(); // Stop refreshing the Braille Display; Also happens when controllooses focus      

            //if (playing) return;
            if (null == selectedObject) return;
            if ((selectedObject is NoteElement))
            {
                //NoteElement noteElement = selectedObject as NoteElement;
                //if (noteElement.IsPause) return; // This is a pause
                //// new MidiNote(noteElement.Step, noteElement.Alter, noteElement.Octave, 127, midiOut);
            }
            else if ((selectedObject is EventDescription))
            {
  
                EventDescription eventDescription = selectedObject as EventDescription;
                nvda.CancelSpeech(); // Prevent overloading the internal queue in NVDA when rapidly changing between different events

                // Experimental code, used when displaying Braille code in the listbox: 
                //int speekTextResult = NvdaControllerClientWrapper.nvdaController_speakText(eventDescription.ToNormalTextString());
 
                string text = eventDescription.ToString();          // The text currently shown on the visual display
                List<byte> bytes = eventDescription.ToBraille();    // The Braille pattern to show on the Braill display


                //// Experimental code for accessing a Freedom Scientific display directly, bypassing NVDA. Works.

                //// byte[] byteArray = bytes.ToArray();
                //// Write these bytes to the Braille display
                //// if (byteArray.Length > 0)
                //// {
                ////     fSBrlDspAPIWrapper.Write(byteArray);
                //// }

                string musicBrailleString = BytesToString(bytes, displaySize);

                musicBrailleTextBox.Text = musicBrailleString;  // Write to the Windows Forms control for visualizing Braille on the PC screen      
                nvda.BrailleMessage(musicBrailleString);        // Write to the physical Braille Display device through nvda
            }
            return;
        }

        public void StopRefreshing()
        {
            nvda.StopRefreshing();
            musicBrailleTextBox.Text = emptyBrailleString;
        }

    }


}
