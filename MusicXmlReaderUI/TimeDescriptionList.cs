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
                        // Create an EndEventElement to mark the end of this NoteElement                            
                        EndEventElement endEventElement = EndEventElement.Create(noteElement, noteElement.StartTime + noteElement.DurationInCommonDivisions);
                        times.Add(endEventElement);

                    }
                    else if (e is ForwardElement)
                    {
                        // Move the MusciXml program counter without playing anything
                        ForwardElement forwardElement = e as ForwardElement;
                        nextStartTime += forwardElement.DurationInCommonDivisions; /////////////////////////////// FIX THIS TO DO
                        //previousStartTime = nextStartTime; // ??????????????????????????????????????????
                    }

                    else if (e is BackupElement)
                    {
                        // Move the MusciXml program counter without playing anything
                        BackupElement backupElement = e as BackupElement;
                        nextStartTime -= backupElement.DurationInCommonDivisions; /////////////////////////////// FIX THIS TO DO
                        //previousStartTime = nextStartTime; // ??????????????????????????????????????????
                    }

                    else if ((e is HarmonyElement)
                         ||  (e is MeasureElement)
                         ||  (e is SoundElement)
                         ||  (e is KeyElement)
                         ||  (e is ClefElement)
                         ||  (e is TimeElement))
                    {
                        // All these elements are EventElements!
                        EventElement eventElement = e as EventElement;
                        eventElement.StartTime = nextStartTime;
                        times.Add(eventElement);
                    }
    
                    else
                    {
                        // Ignore this element.
                        Model.Log(string.Format("TimeDescriptionList: Unexpected element of type {0} String='{1}'", e.GetType(), e.ToString()));
                    }
                }

                if (0 == (times.Count))
                {
                    return;
                }

                times.Sort(Compare);

            }

        }

        public static TimeDescriptionList Create(PartDescriptionList partDescriptionList, int divisions)
        {
            return new TimeDescriptionList(partDescriptionList, divisions);
        }

        public int Compare(Element x, Element y)
        {
            if ((x is EventElement) && (y is EventElement))
            {
                return ((x as EventElement).StartTime - (y as EventElement).StartTime);
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

