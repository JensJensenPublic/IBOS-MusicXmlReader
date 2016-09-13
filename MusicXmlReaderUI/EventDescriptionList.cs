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

        UserSettings userSettings;

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
        private EventDescriptionList(TimeDescriptionList timeDescriptionList, int numberOfParts,UserSettings userSettings)
        {
            this.userSettings = userSettings;
            events = new List<EventDescription>();
            int currentStartTime = -1;
            EventDescription currentEventDescription = null;
            foreach (EventElement eventElement in timeDescriptionList.times)
            {
                if (eventElement.StartTime != currentStartTime)
                {
                    currentStartTime = eventElement.StartTime;
                    currentEventDescription = EventDescription.Create(currentStartTime, numberOfParts, this.userSettings);       
                    events.Add(currentEventDescription);
                }
                currentEventDescription.AddNote(eventElement);
            }

        }

        public static EventDescriptionList Create(TimeDescriptionList timeDescriptionList, int numberOfParts, UserSettings userSettings)
        {
            return new EventDescriptionList(timeDescriptionList, numberOfParts,userSettings);
        }


    }
}
