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
        int duration;
        int numberOfParts;

        /// <summary>
        /// Notes to be played at this time
        /// </summary>
        private List<NoteElement> notes;

        public List<NoteElement> Notes
        {
            get
            {
                return notes;
            }
        }

        public int Duration
        {
            get
            {
                return duration;
            }
        }

        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private EventDescription()
        {
        }

        private EventDescription(int time, int numberOfParts)
        {
            this.duration = time;
            this.numberOfParts = numberOfParts;
            this.notes = new List<NoteElement>(numberOfParts);
        }

        public static EventDescription Create(int time, int numberOfParts)
        {
            return new EventDescription(time, numberOfParts);
        }

        public void AddNote(NoteElement noteElement)
        {
            notes.Add(noteElement);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(string.Format("{0}: ", duration));
            foreach (NoteElement noteElement in notes)
            {
                sb.Append( string.Format("{0} ", noteElement.PartId));
            }
            return sb.ToString();           
        }
    }
}
