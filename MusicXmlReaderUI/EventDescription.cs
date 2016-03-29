using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JSJ.MusicSynthesis;
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
//        private NoteElement[] notes; // TO DO: Remove notes. Use noteLists instead!!

        private List<NoteElement>[] noteLists; // An array of lists of notes

        private HarmonyElement harmonyElement; // The harmony related to this event, if any.

        private MeasureElement measureElement; //The  measure related to this event, if any.
        //private int MeasureNumber = -1; // The measure Number if this event falls on a measure border.

        private List<EndEventElement> endEventElements; // Elements (for instance NoteElements) to end at this time

        //public NoteElement[] Notes // TO DO: Remove Notes. Use NoteLists instead!!
        //{
        //    get
        //    {
        //        return notes;
        //    }
        //}

        public int StartTime
        {
            get
            {
                return startTime;
            }
        }

        public List<NoteElement>[] NoteLists
        {
            get
            {
                return noteLists;
            }
        }

        public HarmonyElement HarmonyElement
        {
            get
            {
                return harmonyElement;
            }
        }

        public List<EndEventElement> EndEventElements
        {
            get
            {
                return endEventElements;
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
            //            this.notes = new NoteElement[numberOfParts];
            // For each part a list is needed to handle to handle multiple notes within the same part!
            this.noteLists = new List<NoteElement>[numberOfParts];
            for (int i = 0; (i < numberOfParts); i++)
            {
                noteLists[i] = new List<NoteElement>();
            }
            this.userSettings = userSettings;
        }

        public static EventDescription Create(int time, int numberOfParts, UserSettings userSettings)
        {
            return new EventDescription(time, numberOfParts, userSettings);
        }

        public void AddNote(EventElement eventElement)
        {
            //            notes[noteElement.PartNumber] = noteElement;
            if (eventElement is NoteElement)
            {
                NoteElement noteElement = eventElement as NoteElement;
                noteLists[noteElement.PartNumber].Add(noteElement);
            }
            else if (eventElement is HarmonyElement)
            {
                // Assuming only one harmony starts at one time.
                harmonyElement = eventElement as HarmonyElement; // Assume only one harmony per event!
            }
            else if (eventElement is MeasureElement)
            {
                // Assuming only one harmony starts at one time.
                measureElement = eventElement as MeasureElement; // Assume only one measure per event!
            }
            else if (eventElement is EndEventElement)
            {
                if (null == endEventElements)
                {
                    endEventElements = new List<EndEventElement>();
                }
                endEventElements.Add(eventElement as EndEventElement);
            }
        }


        /// <summary>
        /// Generate a string representing the (possibly multiple) notes of a single part
        /// </summary>
        /// <param name="noteElementList"></param>
        /// <returns></returns>
        private string PartNotes(List<NoteElement> noteElementList)
        {
            if (0 == noteElementList.Count()) return "-"; // Nothing happened in this part 
            string s = "";
            foreach (NoteElement noteElement in noteElementList) // Iterate over the notes within one part! For instance (S1,S2).
            {
                string delimiter = string.IsNullOrEmpty(s) ? "" : "+"; // Use this string te separate notes within one part
                // Add pitch information
                if (userSettings.partsToRead[noteElement.PartNumber]) // Might later look at subparts S1/S2 ? 
                {
                    string note = string.IsNullOrEmpty(noteElement.Step) ? "P" : noteElement.PitchValue.Name + noteElement.PitchValue.Octave;
                    s = s + delimiter + note;
                }
            }
            return s;
        }

        /// <summary>
        /// Generate a string representing the (possibly multiple) texts of a single part
        /// </summary>
        /// <param name="noteElementList"></param>
        /// <returns></returns>
        private string PartLyrics(List<NoteElement> noteElementList)
        {
            if (0 == noteElementList.Count()) return ""; // Nothing happened in this part 
            string s = "";
            foreach (NoteElement noteElement in noteElementList) // Iterate over the notes within one part! For instance (S1,S2).
            {
                string delimiter = string.IsNullOrEmpty(s) ? "" : " "; // Use this string te separate notes within one part
                // Add pitch information
                if (userSettings.partsToReadLyrics[noteElement.PartNumber]) // Might later look at subparts S1/S2 ? 
                {
                    string text = string.IsNullOrEmpty(noteElement.Text) ? "" : noteElement.Text;
                    s = s + delimiter + text;
                }
            }
            return s;
        }


        public override string ToString()
        {
            string divisions = userSettings.readDivisions ? string.Format("{0,6}: ", startTime, "") : "";

            string measure = "";
            if (userSettings.readMeasureNumbers)
            {
                measure = (null != measureElement) ? string.Format("Takt {0,4}", measureElement.Number) : "         "; // Up to 10000 measures
            }
            
            string harmonyCode = "";
            if ((userSettings.readHarmonyCodes) && (null != harmonyElement))
            {
                harmonyCode = string.Format(" {0} {1} {2} : ",harmonyElement.Kind, harmonyElement.RootStep, harmonyElement.RootAlter);
            }


            string harmony = "";
            if ((userSettings.readHarmonies) && (null != harmonyElement))
            {
                //ChromaticStep chromaticStep = MidiNote.GetChromaticStep(harmonyElement.RootStep, harmonyElement.RootAlter);
                //ChordType chordType = MidiChord.GetChordType(harmonyElement.Kind);
                harmony = string.Format("{0}-{1}",harmonyElement.ChromaticStep, harmonyElement.LocalizedChordType);
            }


            string endEventString = "";
            if ((userSettings.readEndEvents) && (null != endEventElements))
            {
                endEventString += "(";
                foreach (EndEventElement endEventElement in endEventElements)
                {
                    // Get the starttime for the Element that this EndEventElement represents
                    endEventString += " " + endEventElement.StartElement.StartTime.ToString();
                }
                endEventString += ")";
            }


            StringBuilder sbNotes = new StringBuilder();
            StringBuilder sbTexts = new StringBuilder();
            // Iterate over the parts and build a complete representation of all notes and of all texts
            foreach (List<NoteElement> noteElementList in noteLists) // Iterate over the fixed number of parts.
            {
                string partNotes = PartNotes(noteElementList); // Represents all notes for all parts
                string partLyrics = PartLyrics(noteElementList); // Represents all texts for all parts
                // Assume that: The step is described with 3 characters. The octave with 1 character and max 2 notes per part !
                sbNotes.Append(string.Format("{0,9} ", partNotes.Replace(" ", "")));  // Remove any blanks and fix width to 9 
                sbTexts.Append(string.Format("{0} ", partLyrics));
            }
            return measure + divisions +  sbNotes.ToString() + " " + sbTexts.ToString() + harmonyCode + harmony + endEventString;
        }
    }
}

