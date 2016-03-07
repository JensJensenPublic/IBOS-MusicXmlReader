using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MusicXmlReaderUI
{

    public class TimeDescriptionList:  IComparer<Element>
    {

        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private TimeDescriptionList()
        {
        }

        // The list of times ,each containing a list of elements        
        public List<Element> times;

        /// <summary>
        /// Private constructor, used by the Crate() method
        /// </summary>
        /// <param name="node"></param>
        private TimeDescriptionList(PartDescriptionList partDescriptionList, int divisions)
        {
            int numberOfParts = partDescriptionList.NumberOfParts;
            times = new List<Element>();

            foreach (List<Element> elementList in partDescriptionList.parts)
            {
                int nextStartTime = 0; // Each part starts at time = 0 MilliSeconds
                int previousStartTime = 0; 
                foreach (Element e in elementList)
                {
                    if (e is NoteElement)
                    {
                        NoteElement noteElement = e as NoteElement;
                        times.Add(noteElement);
                        if (noteElement.Chord)
                        {   // Start at the beginning of the previous note
                            noteElement.StartTime = previousStartTime; 
                        }
                        else
                        {
                            // Default: Start after the previous note
                            noteElement.StartTime = nextStartTime;
                            previousStartTime = nextStartTime; // Needed if the following note has the "chord" elemenn
                            nextStartTime += noteElement.DurationInCommonDivisions;
                        }
                    }
                }
            }

            if (0 == (times.Count))
            {
                return;
            }

            times.Sort(Compare);

            //List<EventDescription> events = new List<EventDescription>();
            //int currentStartTime = -1;
            //EventDescription currentEventDescription = null;
            //foreach (NoteElement note in times)
            //{
            //    if (note.StartTime != currentStartTime)
            //    {
            //        currentEventDescription = EventDescription.Create(currentStartTime, numberOfParts);
            //        events.Add(currentEventDescription);
            //    }
            //    currentEventDescription.AddNote(note);
            }

        

        public static TimeDescriptionList Create(PartDescriptionList partDescriptionList, int divisions)
        {
            return new TimeDescriptionList(partDescriptionList, divisions);
        }

        public int Compare(Element x, Element y)
        {
            if ((x is NoteElement) && (y is NoteElement))
            {
                return ((x as NoteElement).StartTime - (y as NoteElement).StartTime);
            }
            return 0;
        }

        public void LoadListBox(ListBox listBox)
        {
            foreach (Element e in times)
            {
                listBox.Items.Add(e);
            }
        }

    }
}

