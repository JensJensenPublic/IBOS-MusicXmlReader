using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace MusicXmlReaderUI
{

    /// <summary>
    /// Similar function to MusicPlyuer
    /// </summary>
    class BrailleDisplayer
    {


        private BrailleDisplayer()
        {
        }

        public static BrailleDisplayer Create()
        {
            return new BrailleDisplayer();
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

                // Write these bytes to the Braille display

            }
            return;
        }



    }


}
