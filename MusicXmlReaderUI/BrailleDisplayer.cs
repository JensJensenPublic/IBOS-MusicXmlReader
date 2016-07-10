using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

        private FSBrlDspAPIWrapper fSBrlDspAPIWrapper;
        private TextBox musicBrailleTextBox; // The textbox used for writing MusicBraille bytes, repredsented as UniCode

        private BrailleDisplayer(TextBox tb)
        {
            musicBrailleTextBox = tb;
            fSBrlDspAPIWrapper = FSBrlDspAPIWrapper.Create();
            fSBrlDspAPIWrapper.Open();
        }

  
        public static BrailleDisplayer Create(TextBox tb)
        {
             return new BrailleDisplayer(tb);
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
                string text = eventDescription.ToString();          // The text currently shown on the visual display
                List<byte> bytes = eventDescription.ToBraille();    // The Braille pattern to show on the Braill display

                byte[] byteArray = bytes.ToArray();

                // Write these bytes to the Braille display
                if (byteArray.Length > 0)
                {
                    fSBrlDspAPIWrapper.Write(byteArray);
                }

                // Write these bytes to the MusicBraille textbox, represented as UniCode                               
                StringBuilder musicBrailleStringBuilder = new StringBuilder();
                char UnicodeBrailleBase = (char) 0x2800;
                foreach (byte b in bytes)
                {
                    musicBrailleStringBuilder.Append((char)(UnicodeBrailleBase + (char)b));
                }
                musicBrailleTextBox.Text = musicBrailleStringBuilder.ToString();
               
            }
            return;
        }



    }


}
