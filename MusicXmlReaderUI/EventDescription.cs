using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;


namespace MusicXmlReaderUI
{
    
    public class EventDescription
    {
        int startTime;
        int numberOfParts;
        UserSettings userSettings;

        /// <summary>
        /// Notes to be played at this time
        /// </summary>
        private NoteElement[] notes;

        public NoteElement[] Notes
        {
            get
            {
                return notes;
            }
        }

        public int StartTime
        {
            get
            {
                return startTime;
            }
        }

        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private EventDescription()
        {
        }

        private EventDescription(int time, int numberOfParts, UserSettings userSettings)
        {
            this.startTime = time;
            this.numberOfParts = numberOfParts;
            this.notes = new NoteElement[numberOfParts];
            this.userSettings = userSettings;
        }

        public static EventDescription Create(int time, int numberOfParts,  UserSettings userSettings)
        {
            return new EventDescription(time, numberOfParts, userSettings);
        }

        public void AddNote(NoteElement noteElement)
        {
            notes[noteElement.PartNumber] = noteElement;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(string.Format("{0,6}: ", startTime));
            StringBuilder sbText = new StringBuilder();
            foreach (NoteElement noteElement in notes)
            {
                //              sb.Append(string.Format("{0} ", noteElement.PartId));
                string s = "-";
                if (null != noteElement)
                {
                    // Add pitch information
               
                    if (userSettings.partsToRead[noteElement.PartNumber])
                    {
                        s = string.IsNullOrEmpty(noteElement.Step) ? "P" : noteElement.PitchValue.Name + noteElement.PitchValue.Octave;
                    }
                

                    // Add any lyrics
                    if (!string.IsNullOrEmpty(noteElement.Text))
                    {
                        sbText.Append(noteElement.Text);
                    }
                }
                sb.Append(string.Format("{0,4} ", s.Replace(" ", "")));  // Remove any blanks
            }
            return sb.ToString() + sbText.ToString();           
        }
    }
}
