using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MusicXmlReaderUI
{

    public class EventDescriptionList
    {

        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private EventDescriptionList()
        {
        }

        List<EventDescription> events;

        public List<EventDescription> Events
        {
            get
            {
                return events;
            }
        }

        /// <summary>
        /// Private constructor, used by the Crate() method
        /// </summary>
        /// <param name="node"></param>
        private EventDescriptionList(TimeDescriptionList timeDescriptionList, int numberOfParts)
        {
            events = new List<EventDescription>();
            int currentStartTime = -1;
            EventDescription currentEventDescription = null;
            foreach (NoteElement note in timeDescriptionList.times)
            {
                if (note.StartTime != currentStartTime)
                {
                    currentStartTime = note.StartTime;
                    currentEventDescription = EventDescription.Create(currentStartTime, numberOfParts);       
                    events.Add(currentEventDescription);
                }
                currentEventDescription.AddNote(note);
            }

        }

        public static EventDescriptionList Create(TimeDescriptionList timeDescriptionList, int numberOfParts)
        {
            return new EventDescriptionList(timeDescriptionList, numberOfParts);
        }


        public void LoadListBox(ListBox listBox)
        {
            foreach (EventDescription eventDescription in events)
            {
                listBox.Items.Add(eventDescription);
            }
        }


    }
}
