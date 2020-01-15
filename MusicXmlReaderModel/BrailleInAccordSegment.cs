using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXmlReaderModel
{

    /// <summary>
    /// This class handles "In Accord"  (Danish: "Lille Bistemme" and "Stor Bistemme") representation as described in
    /// Music Braille Code 1997   Chapter 10. IN-ACCORD AND MEASURE-DIVISION SIGNS
    /// This class is used as a last resort when an event contains multiple notes, but cannot be described by Interval Notation because the notes are of different lengths
    /// 
    /// Also see http://brailleauthority.org/music/music.html  (Locally downloaded to "\Dropbox\Musiklæser\Braille Music Official Documentation.Music_Braille_Code_2015.pdf")
    /// </summary>
    public class BrailleInAccordSegment
    {
        // Control of logging-options during development and debugging:
        // For convenience the following variables, only used for logging are made static and public, so they can be accessed from anywhere in the code
        public static readonly bool LogAllMatches = false;          // Option for logging  during development and debugging.
        public static readonly bool LogNotesLastingToEnd = false;   // Option for logging  during development and debugging.
        public static readonly bool LogNotesStartinAtStart = false; // Option for logging  during development and debugging.
        public static readonly bool LogMultiMatch = true; // Option for logging  during development and debugging.

        private List<EventDescription> localEvents = new List<EventDescription>(); // Contains a list of all eventsto be handled.
        private List<BrailleInAccordVoice> voices = new List<BrailleInAccordVoice>();
        private EventDescription firstEventDescription;  // The first eventdescription covered by this segment
        private EventDescription nextEventDescription;  // The first eventdescription NOT covered by this segment


        /// <summary>
        /// Used de decide if we should use ?? (Danish: "Lille Bistemme" or ?? (Danish: "Stor Bistemme"
        /// </summary>
        public bool IsFullMeasure
        {
            get
            {
                bool result = this.firstEventDescription.IsFirstEventInMeasure && this.nextEventDescription.IsFirstEventInMeasure;
                int thisMeasureNumber = this.firstEventDescription.CurrentMeasureNumber;
                int nextMeasureNumber = this.nextEventDescription.CurrentMeasureNumber;
                int dif = nextMeasureNumber - thisMeasureNumber;
                if ((true  == result) && (dif != 1)
                ||  (false == result) && (dif != 1) && (dif != 0))
                {
                    Logger.LogCF(string.Format(": Result={0} Inconsistent measurenumbers : this={1} next={2}", result, thisMeasureNumber, nextMeasureNumber));
                }
                return result;
            }
        }

        /// <summary>
        /// The first EventDescription not handled by this segment
        /// </summary>
        public EventDescription NextEventDescription { get { return nextEventDescription; } }
        int measureNumber; // Primarily for debugging !!

        public BrailleBuilder ToBraille(UserSettings userSettings, bool fromTop)
        {

            //if (!newVersion) return ToBraille(userSettings);

            // Now follows the new implementation:
            BrailleBuilder result = BrailleBuilder.Create(firstEventDescription.StartTime);
            // First create a list of notelists, each notelist representing notes from the same part, staff  and voice
            for (int i = 0; (i < localEvents.Count); i++)
            {
                EventDescription currentEvent = localEvents[i];
                foreach (NoteElementList noteList in currentEvent.NoteLists)
                {
                    foreach (NoteElement note in noteList.NoteElements)
                    {
                        bool found = false;
                        foreach (BrailleInAccordVoice voice in voices)
                        {
                            // NOTE: the int variables are sufficient for the comparition! partId and staffId are primarily for debugging.
                            if ((voice.Part == note.PartNumber) && (voice.Staff == note.Staff) && (voice.Voice == note.Voice))
                            {
                                // We already have a similar item. Add the note
                                voice.Notes.NoteElements.Add(note); 
                                found = true;
                                break;
                            }
                        }
                        if (!found)
                        {
                            // No similar item found. Create a new item and add the note to it.
                            BrailleInAccordVoice newVoice = BrailleInAccordVoice.Create(note); // Also adds the note !
                            voices.Add(newVoice);
                        }
                    }
                }
            }
            int nNonEmptyVoices = 0;
            bool isFullMeasure = this.IsFullMeasure; // Only evaluate once, the value is the same for all voices.
            if (!fromTop)
            {
                // Logger.LogCF(string.Format(": Reversing {0} voices because the rendering order is not fromtop",voices.Count)); // Will slow down some scores considerably !
                voices.Reverse();
            }
            foreach (BrailleInAccordVoice voice in voices)
            {
                // Convert each voice to Braille 
                // Logger.LogCF(string.Format(": Rendering Part={0} Staff={1} Voice={2}", voice.Part, voice.Staff, voice.Voice));              
                result.Append(voice.ToBraille(userSettings,ref nNonEmptyVoices, isFullMeasure));
                // Force an octave mark on the first note in each voice (except the first voice ) within the InAccordSegment and on the first note after the InAccordSegment:
                MusicBrailleState.ResetMusicBrailleState(); 
            }

            //Logger.LogCF(string.Format(": nNonEmptyVoices={0}", nNonEmptyVoices));

            if (!isFullMeasure && (nNonEmptyVoices >= 2))
            {
                // Only after a sequence of at least 1 of PartMeasure voices 
                result.AppendMeasureDivisionMarkAtEnd();
            }
            return result;
        }


        private BrailleInAccordSegment()
        {}

        private BrailleInAccordSegment(EventDescription eventDescription,UserSettings userSettings)
        {
            // Cut out a "segment" in the time dimension, containing a number of eventdescription to be converted to In Accord notation.
            // First simple approach: Create a list "localEvents" of all remaining events we need to consider:
            // We need to keep adding events until all notes, started in the list have stopped.
            // This will happen at or before the start of the next measure.


            //bool done = false;
            this.firstEventDescription = eventDescription;
            this.nextEventDescription = eventDescription.Next;                         
            measureNumber = eventDescription.CurrentMeasureNumber; // Always holds the Measure number, also for noteElements between bars

            long latestEndTime = eventDescription.GetLatestEndTime(userSettings);

            while (null != eventDescription)
            {  
                if (eventDescription.StartTime < latestEndTime)
                {
                    latestEndTime = Math.Max(latestEndTime,eventDescription.GetLatestEndTime(userSettings)); // The latest end time of all notes in localEvents
                    this.localEvents.Add(eventDescription);
                    eventDescription = eventDescription.Next;
                    this.nextEventDescription = eventDescription;
                }
                else
                {
                    eventDescription = null;    
                }
            }

            //if (logLocalEvents) LogLocalEvents(measureNumber, userSettings); // Only while debugging !
        }


        /// <summary>
        /// Builds an EventDescriptionList starting with the eventdescription and ending at the end of the current measure
        /// </summary>
        /// <param name="eventDescription"></param>
        /// <param name="userSettings"></param>
        /// <returns></returns>
        public static BrailleInAccordSegment Create(EventDescription eventDescription,UserSettings userSettings)
        {
            return new BrailleInAccordSegment(eventDescription, userSettings);
        }
    }



}
