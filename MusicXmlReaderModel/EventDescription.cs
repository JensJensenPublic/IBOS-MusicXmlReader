using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
// using MusicXmlReaderUI;

namespace MusicXmlReaderModel
{

    public class EventDescription
    {
        string className = "EventDescription";
        Int64 startTime;
        int numberOfParts;
        UserSettings userSettings;
        int metronomeBeatsPerMinute = -1;

        string musicBrailleRepresentation;
        string musicBrailleAsTextRepresentation;
        string textRepresentation;

        private bool isLastEvent;
        private bool isFirstEvent;
        private EventDescription next = null;
        public EventDescription Next { get { return next; } set{ next = value; } } 
        private EventDescription prev = null;
        public EventDescription Prev { get { return prev; } set { prev = value; } }


        /// <summary>
        /// Notes to be played at this time
        /// </summary>
//        private NoteElement[] notes; // TO DO: Remove notes. Use noteLists instead!!

        private NoteElementList[] noteLists; // An array of lists of notes

        private HarmonyElement harmonyElement; // The harmony related to this event, if any.

        private MeasureElement measureElement; //The  measure related to this event, if any.
        //private int MeasureNumber = -1; // The measure Number if this event falls on a measure border.

        private RepeatElement repeatElementForward; // The RepeatElement  with the "forward" attribute relateted to this event, if any
        private RepeatElement repeatElementBackward; // The RepeatElement  with the "backward" attribute relateted to this event, if any

        private List<EndEventElement> endEventElements; // Elements (for instance NoteElements) to end at this time

        // A few other elements may be related to a specific event
        private List<SoundElement> soundElements;           // The SoundElements related to this event, if any
        private List<KeyElement> keyElements;             // The KeyElements related to this event, if any
        private List<ClefElement> clefElements;            // The ClefElements related to this event, if any
        private List<TimeElement> timeElements;             // The TimeElements related to this event, if any
        private List<BarlineElement> barlineElements;       // The BarlineElements related to this event, if any. In some rare cases more than one!!
        private List<DirectionElement> directionElements;  
        private List<MeasureStyleElement> measureStyleElements; // The MeasureStyleElements related to this event, if any              
        private List<MeasureElement> measureElements; // The MeasureElements related to this event, if any
        public  List<MeasureElement> MeasureElements { get { return measureElements; } }
        private List<PrintElement> printElements; // The PrintElements related to this event, if any 
        public  List<PrintElement> PrintElements { get { return printElements; } }
        private List<StavesElement> stavesElements;
        public List<StavesElement>  StavesElements { get { return stavesElements; } }

        private StatusInformation statusInformation; // Contains Status information valid for this eventdescription

        private NoteElementList GetAllNotes()
        {
            NoteElementList result = NoteElementList.Create();
            foreach (NoteElementList nel in NoteLists)
            {
                foreach (NoteElement ne in nel.NoteElements)
                {
                    result.NoteElements.Add(ne);
                }
            }
            return result;
        }

        private NoteElementList GetAllSelectedNotes()
        {
            NoteElementList result = NoteElementList.Create();
            foreach (NoteElementList nel in NoteLists)
            {
                foreach (NoteElement ne in nel.NoteElements)
                {
                    if (userSettings.IsSelected(ne.PartId,ne.Staff))
                    {
                        result.NoteElements.Add(ne);
                    }
                }
            }
            return result;
        }

        


        //        public MeasureFractionHistory MeasureFractions;
        private MeasureFraction measureFraction;
        public MeasureFraction MeasureFraction { get { return measureFraction; } set{ measureFraction = value; }  }

        /// <summary>
        /// Marks the event as the first event in the EventList
        /// </summary>
        public bool IsFirstEvent
        {
            get { return isFirstEvent; }
            set { isFirstEvent = value; }
        }

        /// <summary>
        /// Marks the event as the last event in the EventList
        /// </summary>
        public bool IsLastEvent
        {
            get { return isLastEvent; }
            set { isLastEvent = value; }
        }


        /// <summary>
        /// New implementation, used while generating Braille Music files
        /// </summary>
        public int CurrentMeasureNumber // Always holds the Measure number, also for noteElements between bars
        {
#warning: TODO: get rid of the old implementattion "MeasureNumber" which means something slightly different and is only valid when this contains a MeasureElement !
            get
            {
                if (null == statusInformation)
                {
                    Logger.LogCF(": StatusInformation is null.");
                    Utilities.Beep();
                    return -1;
                }
                if (null == statusInformation.CurrentMeasureElement)
                {
                    Logger.LogCF(": StatusInformation.CurrentMeasureElement is null.");
                    Utilities.Beep();
                    return -1;
                }
                return statusInformation.CurrentMeasureElement.Number;
            }
        }


        /// <summary>
        /// The original implementation, not to be replaced without further consideration
        /// </summary>
        public int MeasureNumber
        {
            get
            {
                return (null == measureElement) ? -1 : measureElement.Number;
            }

        }

        public bool IsFirstEventInMeasure { get { return (null != measureElement); } }

        public Int64 StartTime
        {
            get
            {
                return startTime;
            }
        }

        public NoteElementList[] NoteLists
        {
            get
            {
                return noteLists;
            }
        }

        public bool ContainsVisibleNotes
        {
            get
            {

                if (null == noteLists) return false;
                foreach (NoteElementList noteList in noteLists)
                {
                    foreach (NoteElement noteElement in noteList.NoteElements)
                    {
                        if (noteElement.PrintObjectAttributeValue) return true; ;
                    }
                }
                return false;
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

        internal List<SoundElement> SoundElements
        {
            get
            {
                return soundElements;
            }

            set
            {
                soundElements = value;
            }
        }

        /// <summary>
        /// Contains the current Music Braille representation of this event,
        /// respecting relevant parts of UserSettings
        /// </summary>
        public string MusicBrailleRepresentation
        {
            get
            {
                return musicBrailleRepresentation;
            }
        }

        /// <summary>
        /// Contains the current Text representation of this event,
        /// respecting relevant parts of UserSettings
        /// </summary>
        public string TextRepresentation
        {
            get
            {
                return textRepresentation;
            }
        }

        /// <summary>
        /// Contains a text representation of the current Music Braille representation.
        /// This allows a developer to debug the Music Braille output without knowing Music Braille. 
        /// Only used for debugging
        /// </summary>
        public string MusicBrailleAsTextRepresentation
        {
            get
            {
                return musicBrailleAsTextRepresentation;
            }
        }

        public StatusInformation StatusInformation
        {
            get
            {
                return statusInformation;
            }

            set
            {
                statusInformation = value;
            }
        }

        public int MetronomeBeatsPerMinute
        {
            get
            {
                return metronomeBeatsPerMinute;
            }
        }

        public int NoteCount
        {
            get
            {
                if (null == noteLists) return 0;
                int result = 0;
                foreach (NoteElementList noteList in noteLists)
                {
                    result += noteList.NoteElements.Count;
                }
                return result;
            }
        }



        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private EventDescription()
        {
        }

        private EventDescription(Int64 time, int numberOfParts, UserSettings userSettings)
        {
            this.startTime = time;
            this.numberOfParts = numberOfParts;
            //            this.notes = new NoteElement[numberOfParts];
            // For each part a list is needed to handle to handle multiple notes within the same part!
            this.noteLists = new NoteElementList[numberOfParts];
            for (int i = 0; (i < numberOfParts); i++)
            {
                noteLists[i] = NoteElementList.Create();
            }
            this.userSettings = userSettings;
        }

        public static EventDescription Create(Int64 time, int numberOfParts, UserSettings userSettings)
        {
            return new EventDescription(time, numberOfParts, userSettings);
        }

        /// <summary>
        /// Adds an XML node, which may or may not be a musical note (a NoteElement)
        /// </summary>
        /// <param name="eventElement"></param>
        public void AddNode(EventElement eventElement, StatusInformation currentStatusInformation)
        {
            string functionName = "EventDescription.AddNode";
            //            notes[noteElement.PartNumber] = noteElement;
            if (eventElement is NoteElement)
            {
                NoteElement noteElement = eventElement as NoteElement;
                noteLists[noteElement.PartNumber].NoteElements.Add(noteElement);
            }
            else if (eventElement is HarmonyElement)
            {
                // Assuming only one harmony starts at one time.
                harmonyElement = eventElement as HarmonyElement; // Assume only one harmony per event!
                currentStatusInformation.CurrentHarmonyElement = harmonyElement;
            }
            else if (eventElement is MeasureElement)
            {
                // Assuming only one measure starts at one time.
                measureElement = eventElement as MeasureElement; // Assume only one measure per event!
                if (null == measureElements)
                {
                    measureElements = new List<MeasureElement>();
                }
                measureElements.Add(measureElement);
                //currentStatusInformation.MeasureNumber = measureElement.Number;
                currentStatusInformation.CurrentMeasureElement = measureElement;
            }
            else if (eventElement is EndEventElement)
            {
                if (null == endEventElements)
                {
                    endEventElements = new List<EndEventElement>();
                }
                endEventElements.Add(eventElement as EndEventElement);
            }
            else if (eventElement is SoundElement)
            {
                if (null == soundElements)
                {
                    soundElements = new List<SoundElement>();
                }
                SoundElement soundElement = eventElement as SoundElement;
                soundElements.Add(soundElement);
                //currentStatusInformation.Tempo = soundElement.GetTempo();
                currentStatusInformation.CurrentSoundElement = soundElement;
            }

            else if (eventElement is KeyElement)
            {
                if (null == keyElements)
                {
                    keyElements = new List<KeyElement>();
                }
                KeyElement keyElement = eventElement as KeyElement;
                keyElements.Add(keyElement);
                //currentStatusInformation.Fifths = keyElement.Fifths;
                //currentStatusInformation.Mode = keyElement.Mode;
                currentStatusInformation.CurrentKeyElement = keyElement;
            }

            else if (eventElement is ClefElement)
            {
                if (null == clefElements)
                {
                    clefElements = new List<ClefElement>();
                }
                clefElements.Add(eventElement as ClefElement);
            }

            else if (eventElement is TimeElement)
            {
                if (null == timeElements)
                {
                    timeElements = new List<TimeElement>();
                }
                TimeElement timeElement = eventElement as TimeElement;
                timeElements.Add(timeElement);
                //currentStatusInformation.Beats = timeElement.Beats;
                //currentStatusInformation.BeatType = timeElement.BeatType;
                currentStatusInformation.CurrentTimeElement = timeElement;
            }

            else if (eventElement is RepeatElement)
            {
                RepeatElement repeatElement = eventElement as RepeatElement;
                switch (repeatElement.RepeatDirection)
                {
                    case RepeatElement.RepeatDirectionEnum.Forward: this.repeatElementForward = repeatElement; break;
                    case RepeatElement.RepeatDirectionEnum.Backward: this.repeatElementBackward = repeatElement; break;
                    default: break;
                }
                Logger.LogOnce(string.Format("{0}: Added RepeatElement Direction={1})", functionName, repeatElement.RepeatDirection.ToString()));
            }

            else if (eventElement is BarlineElement)
            {
                // We must add the BarlineElements because they may contain repeatElements and EndingElements and a location attribute
                if (null == barlineElements)
                {
                    barlineElements = new List<BarlineElement>();
                }
                BarlineElement barlineElement = eventElement as BarlineElement;
                barlineElements.Add(barlineElement);
                // For debugging:
#if false
                {
                    string ending = (null == barlineElement.EndingElement) ? "null" : barlineElement.EndingElement.EndingElementType.ToString();
                    string repeat = (null == barlineElement.RepeatElement) ? "null" : barlineElement.RepeatElement.RepeatDirection.ToString();
                    string style  = (null == barlineElement.BarStyleElement) ? "null" : barlineElement.BarStyleElement.ToString();
                    Logger.LogOnce(string.Format("{0}: Added BarlineElement({1},Ending={2},Repeat={3},Style={4})",functionName, barlineElement.Location.ToString(), ending, repeat,style));
                }
#endif
            }
            else if (eventElement is DirectionElement)
            {
                if (null == directionElements)
                {
                    directionElements = new List<DirectionElement>();
                }
                DirectionElement directionElement = eventElement as DirectionElement;
                directionElements.Add(directionElement);

                if ((null != directionElement.DirectionTypeElement)
                && (null != directionElement.DirectionTypeElement.MetronomeElement) &&
                    directionElement.DirectionTypeElement.MetronomeElement.BeatsPerMinuteBool
                )
                {
                    this.metronomeBeatsPerMinute = directionElement.DirectionTypeElement.MetronomeElement.BeatsPerMinuteInt;
                    // The MetronomeElement contains Tempo information to be used by the MusicPlayer and the StatusInformation !
                    currentStatusInformation.CurrentMetronomeElement = directionElement.DirectionTypeElement.MetronomeElement;
                }

            }

            else if (eventElement is MeasureStyleElement)
            {
                if (null == measureStyleElements)
                {
                    measureStyleElements = new List<MeasureStyleElement>();
                }
                measureStyleElements.Add(eventElement as MeasureStyleElement);
            }
            else if (eventElement is AttributesElement)
            {
                // Explicitly do nothing
            }
            else if (eventElement is PrintElement)
            {
                if (null == printElements)
                {
                    printElements = new List<PrintElement>();
                }
                printElements.Add(eventElement as PrintElement);
            }
            else if (eventElement is StavesElement)
            {
                if (null == stavesElements)
                {
                    stavesElements = new List<StavesElement>();
                }
                stavesElements.Add(eventElement as StavesElement);
            }


            else
            {
                Logger.Log(string.Format("{0}.{1} Unsupported eventElement Type={2}", className, functionName, eventElement.GetType()));
            }


        }

        //// Start experimental code ********************************************************************************

        ///// <summary>
        ///// Simple mechanism for selecting a specific staff within a spscific part.
        ///// Used for generating BrailleMusic information one staff at a time
        ///// When generating BrailleMusic for the UI UserSettings.SelectedStaff is null, signalling that no spscific staff is salected.
        ///// </summary>
        ///// <param name="partId"></param>
        ///// <param name="staffNumberWithinPart"></param>
        ///// <returns></returns>
        //public bool IsSelected(string partId, int staffNumberWithinPart)
        //{
        //    if (null == userSettings.SelectedStaff) return true; // No specific staff is selected. Merge information for all parts enabled in UserSettings.
        //    return userSettings.SelectedStaff.Equals(partId, staffNumberWithinPart);  // A specific staff is selected.   Only return information for this staff 
        //}

        //private bool IsSelected(string partId)
        //{
        //    if (null == userSettings.SelectedStaff) return true; // No part staff is selected. Merge information for all parts enabled in UserSettings.
        //    return (0 == string.Compare(userSettings.SelectedStaff.PartId, partId));  // A specific part is selected.   Only return information for this part 
        //}


        //        private List<NoteElement> ChordNotation(List<NoteElement> noteElementList)
        //        {
        //            // Start experimental code for generating Chord notation!!
        //            if (noteElementList.Count <= 1) return null; // Performance optimization: Most times we can stop here !
        //            List<NoteElement> selectedNotes = new List<NoteElement>(); // Pick up and sort the relevant notes 
        //            foreach (NoteElement noteElement in noteElementList) // Iterate over the notes within one part! For instance (S1,S2).
        //            {
        //                if ((userSettings.GetParts(UserSettings.Category.MusicBraille, noteElement.PartNumber))
        //                && noteElement.PrintObjectAttributeValue
        //                && IsSelected(noteElement.PartId, noteElement.Staff) // For generating BrailleMusic for separate staffs
        //                && (!noteElement.IsPause) // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! THINK !!
        //#warning TODO Think 
        //                    )
        //                {
        //                    selectedNotes.Add(noteElement);
        //                }
        //            }
        //            if (selectedNotes.Count >= 2)
        //            {
        //                return selectedNotes; // 2 or more notes for the same part and staff at the same time. We must use IntervalNotation or "Bistemmer"
        //            }
        //            return null;
        //        }

        // End experimental code  ********************************************************************************


        //        /// <summary>
        //        /// Generate a representing of the (possibly multiple) notes of a single part
        //        /// This function is where we can really differentiate ourselves from mainstream products such as MuseScore
        //        /// </summary>
        //        /// <param name="noteElementList"></param>
        //        /// <returns></returns>
        //        public BrailleBuilder NotesForOnePartAsBraille(List<NoteElement> noteElementList)
        //        {
        //            BrailleBuilderEx bb = BrailleBuilderEx.Create(this.startTime);
        //            if (!userSettings.GetMusicBrailleSettings(UserSettings.MusicBrailleSettingsEnum.Notes)) return bb; // User completely turned off reading of notes
        //            bool addNotations = userSettings.GetMusicBrailleSettings(UserSettings.MusicBrailleSettingsEnum.Notations);
        //#if true
        //#warning ToDo fix experimental code !!
        //            List<NoteElement> selectedNotes = ChordNotation(noteElementList);
        //            if ((null != selectedNotes) && (selectedNotes.Count >= 2))
        //            {
        //                bool fromTop = (userSettings.SelectedStaff != null) && (1 == userSettings.SelectedStaff.Staff);
        //                if (bb.AdIntervalNotation(selectedNotes, statusInformation, addNotations, fromTop))
        //                {
        //                    return bb;
        //                }
        //                else
        //                {
        //#warning Early test-implementation !!
        //                    BrailleMeasureDivision bmd = BrailleMeasureDivision.Create(this);
        //                    bmd.ToBraille(); 
        //                }
        //            }
        //#endif
        ////            const string prolog = "<";
        ////            const string epilog = ">";
        //            const string prolog = "";
        //            const string epilog = " ";
        //            // We don't need Interval notation. Just continue as in version <= 3.0
        //            foreach (NoteElement noteElement in noteElementList) // Iterate over the notes within one part! For instance (S1,S2).
        //            {

        //                try
        //                {
        //                    // Add pitch information
        //                    // If the note is not marked for printing with the PrintObjectAttributeValue we ignore it
        //                    if ((userSettings.GetParts(UserSettings.Category.MusicBraille,noteElement.PartNumber))
        //                    && noteElement.PrintObjectAttributeValue
        //                    && IsSelected(noteElement.PartId,noteElement.Staff) // For generating BrailleMusic for separate staffs
        //                    ) 
        //                    {
        //                        // bool addNotations = userSettings.GetMusicBrailleSettings(UserSettings.MusicBrailleSettingsEnum.Notations);
        //                        BrailleBuilder bb1 = BrailleBuilder.Create(this.startTime); // TO DO: Why not use bb directly ???
        //                                                                      // userSettings.ReadNotePitch, userSettings.ReadNoteOctave, userSettings.ReadNoteDuration (Danish: Tone/Oktav/Varighed)
        //#warning TODO Refactor: Only the calls to bb.AddRest/bb.AddNote seem to be different !
        //                        if (noteElement.IsPause)
        //                        {
        //                            // This is a rest 
        //                            bb1.AppendText(prolog);
        //                            if (addNotations) bb1.AddBrailleNotationsBeforeNoteOrRest(noteElement.Notations); // Some notations are added Before the note/rest itself 
        //                            bb1.AddRest(noteElement.NoteDuration, false); // TO DO: Handle punctured rests
        //                            if (addNotations) bb1.AddBrailleNotationsAfterNoteOrRest(noteElement.Notations);  // Some notations are added After the note/rest itself
        //                            bb1.AppendText(epilog);
        //                        }
        //                        else
        //                        {
        //                            // This is a note
        //                            bb1.AppendText(prolog);
        //                            if (addNotations) bb1.AddBrailleNotationsBeforeNoteOrRest(noteElement.Notations); // Some notations are added Before the note/rest itself                       
        //                            bb1.AddNote(noteElement,statusInformation.CurrentKeyElement);
        //                            if (addNotations) bb1.AddBrailleNotationsAfterNoteOrRest(noteElement.Notations);  // Some notations are added After the note/rest itself
        //                            bb1.AppendText(epilog);

        //                        }
        //                        //bb.Append(bb1.Braille, bb1.Text.ToString());
        //                        bb.Append(bb1);
        //                    }
        //                }
        //                catch (Exception e)
        //                {
        //                    Logger.Log(string.Format("EventDescription.NotesForOnePartAsBraille threw an exception. Message='{0}'", e.Message));
        //                    Logger.Log(string.Format("NoteElement: Step={0} Alter={1} Octave={2} Duration={3} Measure={4} Part={5}",
        //                        noteElement.Step, noteElement.Alter, noteElement.Octave, noteElement.Duration.ToString(), noteElement.MeasureNumber, noteElement.PartId));
        //                }
        //            }

        //            return bb;
        //        }




        ///// <summary>
        ///// Generate a string representing the (possibly multiple) notes of a single part
        ///// This function is where we can really differentiate ourselves from mainstream products such as MuseScore
        ///// </summary>
        ///// <param name="noteElementList"></param>
        ///// <returns></returns>
        //public string NotesForOnePart(List<NoteElement> noteElementList)
        //{
        //    if (!userSettings.GetReaderSettings(UserSettings.ReaderSettingsEnum.Notes)) return ""; // User completely turned off reading of notes
        //    if (0 == noteElementList.Count()) return " "; // Nothing happened in this part 
        //    StringBuilder sb = new StringBuilder();
        //    foreach (NoteElement noteElement in noteElementList) // Iterate over the notes within one part! For instance (S1,S2).
        //    {
        //        //string delimiter = string.IsNullOrEmpty(sb) ? "" : "+"; // Use this string to separate notes within one part
        //        // Add pitch information
        //        if (userSettings.GetParts(UserSettings.Category.Speech,noteElement.PartNumber)) // Might later look at subparts S1/S2 ? 
        //        {

        //            string note = "";
        //            // userSettings.ReadNotePitch, userSettings.ReadNoteOctave, userSettings.ReadNoteDuration (Danish: Tone/Oktav/Varighed)
        //            if (noteElement.IsPause)
        //            {
        //                // This is a pause
        //                // Here the type and the word "pause" are cocatenated such as "punkteret halvnodepause"
        //                string type = userSettings.GetReaderSettings(UserSettings.ReaderSettingsEnum.NoteTypes) ? noteElement.LocalizedPauseType : "pause";
        //                string notations = (userSettings.GetReaderSettings(UserSettings.ReaderSettingsEnum.Notations) && (null != noteElement.Notations)) ? noteElement.Notations.ToString() : "";
        //                note = string.Format(" {0} {1}",type, notations);
        //            }
        //            else
        //            {
        //                // This is a note
        //                // Here the sequence is pitch,octave,type such af "Cis4 punkteret halvnode"
        //                string accidental = (userSettings.GetReaderSettings(UserSettings.ReaderSettingsEnum.NoteAccidentals) && (null != noteElement.AccidentalElement)) ?  noteElement.AccidentalElement.ToString() : "";
        //                string pitch    = noteElement.PitchValue.Name; // Always use the name of the note
        //                string octave   = userSettings.GetReaderSettings(UserSettings.ReaderSettingsEnum.NoteOctaves) ? noteElement.Octave.ToString() : "";
        //                string type     = userSettings.GetReaderSettings(UserSettings.ReaderSettingsEnum.NoteTypes) ? noteElement.LocalizedType : "";
        //                string pitchAndOctave = noteElement.UnPitched ? noteElement.UnpitchedText : string.Format("{0}{1}", pitch, octave);
        //                string cueString = noteElement.CueNoteString;                        
        //                string notations = (userSettings.GetReaderSettings(UserSettings.ReaderSettingsEnum.Notations) && (null != noteElement.Notations)) ? noteElement.Notations.ToString() : "";
        //                //                      note = string.Format("{0,-4} {1}", pitchAndOctave, type); // Always use 4 chars for pitch and Octave. Examples: "C   ","Cis4"
        //                string printability = noteElement.PrintObjectAttributeValue ? "" : string.Format("({0})", ResourcesForModel.EventDescription_NotPrinted); // TODO USe Resources !
        //                note = string.Format("{0} {1} {2} {3} {4} {5}", accidental, pitchAndOctave, type, cueString, notations, printability);    // Do not use extra chars for Pitch and Octave. Examples: "C","Cis4"
        //            }

        //            // string note = string.IsNullOrEmpty(noteElement.Step) ? "Pause" : noteElement.PitchValue.Name + noteElement.PitchValue.Octave + " " +noteElement.LocalizedType;
        //            //s = s + delimiter + note;
        //            if (sb.Length > 0)  sb.Append(" "); // Separate the notes with ""
        //            sb.Append(note);
        //        }
        //    }
        //    return sb.ToString();
        //}

        ///// <summary>
        ///// Generate a string representing the (possibly multiple) texts of a single part
        ///// </summary>
        ///// <param name="noteElementList"></param>
        ///// <returns></returns>
        //public string LyricsForOnePart(List<NoteElement> noteElementList)
        //{
        //    if (!userSettings.GetReaderSettings(UserSettings.ReaderSettingsEnum.Lyrics)) return ""; 
        //    if (0 == noteElementList.Count()) return ""; // Nothing happened in this part 
        //    StringBuilder sb = new StringBuilder();
        //    foreach (NoteElement noteElement in noteElementList) // Iterate over the notes within one part! For instance (S1,S2).
        //    {
        //        if (userSettings.GetParts(UserSettings.Category.Speech,noteElement.PartNumber))
        //        {
        //            if (!string.IsNullOrEmpty(noteElement.Text))
        //            {
        //                sb.Append((0 == sb.Length) ? "" : " ");
        //                sb.Append(noteElement.Text);
        //            }
        //        }
        //    }
        //    return sb.ToString();
        //}

        /// <summary>
        /// This is the original implementation, without parameters, used for populating a listbox.
        /// Converts each part separately, thus not allowing for Interval notation across parts
        /// </summary>
        /// <returns></returns>
        public BrailleBuilder ToBraille()
        {
            EventDescription dummyArgument = null;
            return ToBraille(false,false, out dummyArgument); // Convert each part separately, thus NOT allowing for Interval notation across parts. Do not use Chord Notation at all.
        }

        /// <summary>
        /// Generates the Braille representation, where ToNormalTextString() generates the text representation 
        /// Same structure as ToNormalTextString()
        /// Depending on settings ToString  will generate a mix of the results of ToBraille and ToNormalTextString()
        /// If mergeAllParts is true,  all parts are merged before converting, thus allowing for Interval Notation across parts
        /// </summary>
        /// <returns></returns>
        public BrailleBuilder ToBraille(bool mergeAllParts, bool useChordNotation, out EventDescription nextEventDescription)
        {
            nextEventDescription = this.Next; // The default
            if (!userSettings.MusicAsMusicBraille) return BrailleBuilder.Create(this.startTime);

            string functionName = "ToBraille";
            bool repeatForward  = false; // Max one Repeat forward per eventdescription
            bool repeatBackward = false; // Max one Repeat backward per eventdescription

            // const string functionName = "EventDescription.ToBraille";
            BrailleBuilder bbBeforeNotes = BrailleBuilder.Create(this.startTime); // For information not contained in notes


            //string divisions = userSettings.GetReaderSettings(UserSettings.ReaderSettings.Divisions) ? string.Format("{0,6}: ", startTime, "") : "";

            // TO DO: Brug barlineElements i steet for repeatElementForward og repeatElementForward. BarlineElements har en left/right attribute
            // og indeholder selv et RepeatElement!

            if (null != repeatElementBackward)
            {
                bbBeforeNotes.AddRepeatBackward(repeatElementBackward);
            }


            if (null != repeatElementForward)
            {
                bbBeforeNotes.AddRepeatForward(repeatElementForward);
            }


      
            if (userSettings.GetMusicBrailleSettings(UserSettings.MusicBrailleSettingsEnum.MeasureNumbers))
            {
                // Create a list of all selected barstyles for this EventDescription
                List<BarStyleEnum> selectedBarStyles = new List<BarStyleEnum>();
                if (null != barlineElements)
                {
                    foreach (BarlineElement barlineElement in barlineElements)
                    {
                        if (this.userSettings.IsSelected(barlineElement.PartId) && (null != barlineElement.BarStyleElement))
                        {
                            BarStyleEnum barStyle = barlineElement.BarStyleElement.BarStyle;
                            if (!selectedBarStyles.Contains(barStyle))
                            {
                                selectedBarStyles.Add(barStyle);
                            }
                        }
                    }
                }

                // Check if a selected part contains a measureElement
                bool implicitBar = false;
                if (null != measureElements)
                {
                    foreach (MeasureElement measureElement in measureElements)
                    {
                        if (this.userSettings.IsSelected(measureElement.PartId))
                        {
                            implicitBar = true;
                        }
                    }
                }

                bbBeforeNotes.AddBarline(selectedBarStyles,implicitBar,this.MeasureNumber);
            }

            //string harmonyCode = "";
            //if ((userSettings.GetReaderSettings(UserSettings.ReaderSettings.HarmonyCodes)) && (null != harmonyElement))
            //{
            //    harmonyCode = string.Format(" {0} {1} {2} : ", harmonyElement.Kind, harmonyElement.RootStep, harmonyElement.RootAlter);
            //}


            //string harmony = "";
            //if ((userSettings.GetReaderSettings(UserSettings.ReaderSettings.Harmonies)) && (null != harmonyElement))
            //{
            //    //ChromaticStep chromaticStep = MidiNote.GetChromaticStep(harmonyElement.RootStep, harmonyElement.RootAlter);
            //    //ChordType chordType = MidiChord.GetChordType(harmonyElement.Kind);
            //    harmony = string.Format("{0}-{1}", harmonyElement.ChromaticStep, harmonyElement.LocalizedChordType);
            //}


            //string endEventString = "";
            //if ((userSettings.GetReaderSettings(UserSettings.ReaderSettings.EndEvents)) && (null != endEventElements))
            //{
            //    endEventString += "(";
            //    foreach (EndEventElement endEventElement in endEventElements)
            //    {
            //        // Get the starttime for the Element that this EndEventElement represents
            //        endEventString += " " + endEventElement.StartElement.StartTime.ToString();
            //    }
            //    endEventString += ")";
            //}


            //string soundString = "";
            //if (null != soundElements)
            //{
            //    foreach (SoundElement soundElement in soundElements)
            //    {
            //        // For the time being we only handle Tempo here. Later we may handle other velues!
            //        if (0 != soundElement.GetTempo())
            //        {
            //            soundString = string.Format("Tempo={0}", soundElement.GetTempo());
            //        }
            //    }
            //}


            if (null != clefElements) // First the ClefElement
            {
                List<ClefElement> selectedClefElements = new List<ClefElement>();
                foreach (ClefElement clefElement in clefElements)
                {
                    if (this.userSettings.IsSelected(clefElement.PartId,clefElement.StaffNumber)) // For generating BrailleMusic for separate staffs
                    {
                        selectedClefElements.Add(clefElement);
                    }
                }    
                                          
                if (1 == selectedClefElements.Count)
                {
                    ClefElement c = selectedClefElements[0];
                    bbBeforeNotes.AppendText("(");
                    bbBeforeNotes.AddClef(c, c.ToShortString());  // Get the unlocalized version
                    bbBeforeNotes.AppendText(")");
                }
                else
                {
                    Logger.LogOnce(string.Format("{0}.{1}: Music Braille for more than 1 ClefElement is not implemented yet ", className, functionName));
                }
            }


            if (null != keyElements) // Then the KeyElement
            {
                // First check that all keyElements are identical
                KeyElement k0 = keyElements[0];
                bool diff = false;
                foreach (KeyElement k in keyElements)
                {
                    if (k.Fifths != k0.Fifths)
                    {
                        diff = true;
                    }
                }
                if (diff)
                {
                    Logger.LogOnce(string.Format("{0}.{1} Different keyElements for same event", className, functionName));
                }
                bbBeforeNotes.AppendText("(");
                bbBeforeNotes.AddKey(k0, k0.ToShortString());
                bbBeforeNotes.AppendText(")");                
            }
            
  
            if (null != timeElements) // Finally the TimeElement
            {
                // First check that all timeElements are identical
                TimeElement t0 = timeElements[0];
                bool diff = false;

                foreach (TimeElement t in timeElements)
                {
                    if ((t.Beats != t0.Beats) || (t.BeatType != t0.BeatType))
                    {
                        diff = true;
                    }
                }
                if (diff)
                {
                    Logger.LogOnce(string.Format("{0}.{1} Different timeElements for same event", className, functionName));
                }
                bbBeforeNotes.AppendText("(");
                bbBeforeNotes.AddTime(t0, t0.ToShortString());
                bbBeforeNotes.AppendText(")");
            }

      
            BrailleBuilder bbNotes = BrailleBuilder.Create(this.startTime);
            if (mergeAllParts)
            {
                NoteElementList allNotes = NoteElementList.Create();
                // Iterate over the parts and build a complete list of all notes 
                foreach (NoteElementList noteElementList in noteLists) // Iterate over the fixed number of parts.
                {
                    allNotes.NoteElements.AddRange(noteElementList.NoteElements);                  
                }
                BrailleBuilder notes = allNotes.ToBraille(userSettings,this,useChordNotation,out nextEventDescription); // One single BrailleBuilder represents all notes for all parts
                bbNotes.Append(notes);
            }
            else
            {
                // Iterate over the parts and build a complete representation of all notes and of all texts
                foreach (NoteElementList noteElementList in noteLists) // Iterate over the fixed number of parts.
                {
                    BrailleBuilder notes = noteElementList.ToBraille(userSettings,this,useChordNotation,out nextEventDescription); // One BrailleBuilder per part
                    bbNotes.Append(notes);
                }
            }

            // Extract information to be shown after the notes
            BrailleBuilder bbAfterNotes = BrailleBuilder.Create(this.startTime);
            // Look for repeat forward/backward and insert, but only one of each per event description !!
            // Look for a termination (Danish "Helslutning") and insert the appropriate sequence
            bool lastBar = false;
            if (null != barlineElements)
            {  
                foreach (BarlineElement barlineElement in barlineElements)
                {
                    if (null != barlineElement.RepeatElement)
                    {
                        switch (barlineElement.RepeatElement.RepeatDirection)
                        {
                            case RepeatElement.RepeatDirectionEnum.Forward:
                                if (!repeatForward)
                                {
                                    bbBeforeNotes.AddRepeatForward(barlineElement.RepeatElement);
                                    repeatForward = true;
                                }
                                break;
                            case RepeatElement.RepeatDirectionEnum.Backward:
                                if (!repeatBackward)
                                {
                                    bbAfterNotes.AddRepeatBackward(barlineElement.RepeatElement);
                                    repeatBackward = true;
                                }
                                break;
                            default: Logger.Log(string.Format("{0}.{1} Unknown RepeatDirection = {2}",
                                                       className, functionName, barlineElement.RepeatElement.RepeatDirection.ToString())); break;
                        } 
                    }

                    if (barlineElement.IsLastBar)
                    {
                        RepeatElement repeatElement = barlineElement.RepeatElement;
                        if (null == repeatElement)
                        {
                            lastBar = true;
                        }
                    }

                } // foreach

            }

            if ((lastBar) || (this.isLastEvent))
            {
                // By inspecting this.isLastEvent we even handle the case where the terminating Barline is missing !
                bbAfterNotes.Append(BrailleBuilder.fullEnd, "FullEnd");
                // Logger.LogOnce(string.Format("{0}.{1}: Found last bar", className, functionName));
                if (!lastBar)
                {
                    Logger.LogOnce(string.Format("{0}.{1}: Missing terminating bar!", className, functionName));
                }
            }


            // Finnally compose the result by concatenating all the substrings in the sequence wanted
            //return measure + repeatForward + divisions + sbNotes.ToString() + " " + sbTexts.ToString() + harmonyCode + harmony + endEventString + soundString + keyString + clefString + timeString + repeatBackward;

            // Ad the various components:
            BrailleBuilder total = BrailleBuilder.Create(this.startTime);
            total.Append(bbBeforeNotes);
            total.Append(bbNotes);
            total.Append(bbAfterNotes);
            return total;

            //return bbNotes.Braille; // Later define the "+" operator for BrailleBuilder 

        }

        /// <summary>
        /// This method is used by the Listbox when fetching text for a line.
        /// As a side effect the values of the current
        /// Music Braille representation and Text representation are cached inside the event
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            BrailleBuilder bb = ToBraille(); // The original, parameterless version, designed to be used by the Listbox !!
            musicBrailleRepresentation = bb.ToBrailleString();
            musicBrailleAsTextRepresentation = bb.ToEquvivalentTextRepresentation(); // Primarily for dedugging
            textRepresentation = ToNormalTextString();
            // The string shown in the listbox also depends on the top level UserSettings: 
            string s = string.Format(userSettings.defaultStringFormat,
                                    userSettings.MusicAsMusicBraille ?  musicBrailleRepresentation : "",
                                    userSettings.MusicAsSpeech ? textRepresentation : "");
            // HACK: In order to left justify the Braille output on the 14 char Focus Display we assure, that the total length is always >= 14 TO DO: Find better solution
            return s.PadRight(14, ' ');          
        }


        /// <summary>
        /// ONLY for debugging purposes. Can be changed as required.
        /// </summary>
        /// <returns></returns>
        public string ToDebugString()
        {
            return ToDebugString(null);
        }

        /// <summary>
        /// ONLY for debugging purposes. Can be changed as required.
        /// </summary>
        /// <returns></returns>
        public string ToDebugString(UserSettings userSettings)
        {
            NoteElementList notes = (null == userSettings) ? this.GetAllNotes() : this.GetAllSelectedNotes();
            if (0 == notes.NoteElements.Count)
            {
                return ""; // Skip events with no selected notes !
            }
            string result = string.Format("Start={0} Notes={1}:  {2}", this.StartTime,notes.NoteElements.Count, notes.ToDebugString());
            return result;
        }




        public string ToMusicBrailleAndTextBrailleString()
        {
            string mb = ToBraille().ToBrailleString(); 
            string nt = ToNormalTextString();            
            return string.Format("{0} {1}", mb, nt);
        }

        /// Generates a normal text representation.
        /// Same structure as ToBraille()
        /// Depending on settings ToString  will generate a mix of the results of ToBraille and ToNormalTextString() 
        public string ToNormalTextString()
        {
            string functionName = "ToNormalTextString";
            if (!userSettings.MusicAsSpeech) return "";

            string divisions = userSettings.GetReaderSettings(UserSettings.ReaderSettingsEnum.Divisions) ? string.Format("{0,6}: ", startTime) : "";

            // A BarlineElement can contain a RepeatElements containing repetition information.
            string repeatBackward = "";
            string repeatForward = "";
            string endOfScore = "";
            bool lastBar = false;
            if (null != barlineElements)
            {
                foreach (BarlineElement barlineElement in barlineElements)
                {
                    if (barlineElement.RepeatElement != null)
                    {
                        if (barlineElement.RepeatElement.RepeatDirection == RepeatElement.RepeatDirectionEnum.Forward)
                        {
                            repeatForward = barlineElement.RepeatElement.ToString() + " ";
                        }
                        if (barlineElement.RepeatElement.RepeatDirection == RepeatElement.RepeatDirectionEnum.Backward)
                        {
                            repeatBackward = barlineElement.RepeatElement.ToString() + " ";
                        }
                    }
                    if (barlineElement.IsLastBar)
                    {
                        lastBar = true;
                    }
                }
            }

            if ((lastBar || this.IsLastEvent))
            {
                // By inspecting this.isLastEvent we even handle the case where the terminating Barline is missing !
                endOfScore = ResourcesForModel.BarlineElement_EndOfScore + " ";
                if (!lastBar)
                {
                    Logger.LogOnce(string.Format("{0}.{1}: Missing terminating bar!", className, functionName));
                }
            }

            string measure = "";

            if (userSettings.GetReaderSettings(UserSettings.ReaderSettingsEnum.FullMeasureInformation)) 
            {
                // Show Measure Number followed by submeasure information
                // By using the information found in statusInformation we assure same valuse as in textBox Status Status and ListBoxDetails
                string measureNumber = "???????????";
                if (null != statusInformation.CurrentMeasureElement)
                {
                    measureNumber = statusInformation.CurrentMeasureElement.Number.ToString();
                }
                else
                {
                    Logger.LogCFOnce(string.Format(": StatusInformation.CurrentMeasureElement is null"));
                }
                string measureFraction = "???????????";
                if (null != statusInformation.CurrentMeasureFraction)
                {
                    measureFraction = statusInformation.CurrentMeasureFraction.ToString();
                }
                else
                {
                    Logger.LogCFOnce(string.Format(": StatusInformation.CurrentMeasureFraction is null"));
                }
                measure = string.Format("{0} {1,3} {2}", ResourcesForModel.NoteElement_measure_text, measureNumber, measureFraction);
            }
            else
            {
                // Measure number only
                if (userSettings.GetReaderSettings(UserSettings.ReaderSettingsEnum.MeasureNumbers))
                {
                    measure = ((null != measureElement) && (!measureElement.ImplicitMeasure)) ? string.Format("{0} {1,3} ", ResourcesForModel.NoteElement_measure_text, measureElement.Number) : "         "; // Up to 1000 measures
                }
            }
            
            string harmonyCode = "";
            if ((userSettings.GetReaderSettings(UserSettings.ReaderSettingsEnum.HarmonyCodes)) && (null != harmonyElement))
            {
                harmonyCode = string.Format(" {0} {1} {2} : ",harmonyElement.Kind, harmonyElement.RootStep, harmonyElement.RootAlter);
            }


            string harmony = "";
            if ((userSettings.GetReaderSettings(UserSettings.ReaderSettingsEnum.Harmonies)) && (null != harmonyElement))
            {
                harmony = string.Format("{0}", harmonyElement.ToString()); // Use same formatting as used in the status line and details list!!
            }


            string endEventString = "";
            if ((userSettings.GetReaderSettings(UserSettings.ReaderSettingsEnum.EndEvents)) && (null != endEventElements))
            {
                endEventString += "(";
                foreach (EndEventElement endEventElement in endEventElements)
                {
                    // Get the starttime for the Element that this EndEventElement represents
                    endEventString += " " + endEventElement.StartElement.StartTime.ToString();
                }
                endEventString += ")";
            }


            string soundString = "";
            if (null != soundElements)
            {
                foreach (SoundElement soundElement in soundElements)
                {
                    // For the time being we only handle Tempo here. Later we may handle other velues!
                    if (soundElement.TempoValid)
                    {
                        soundString = string.Format("{0}:{1} ", ResourcesForModel.EventDescription_tempo, soundElement.TempoValueAsInt.ToString()); 
                    }
                }
            }

            string keyString = "";
            if (null != keyElements)
            {
                foreach (KeyElement keyElement in keyElements)
                {
                    if (!string.IsNullOrEmpty(keyElement.ToString()))
                    {
                        keyString = string.Format("{0} ", keyElement.ToString());
                    }
                }
            }

            string clefString = "";
            if (null != clefElements)
            {
                foreach (ClefElement clefElement in clefElements)
                {
                    if (!string.IsNullOrEmpty(clefElement.ToString()))
                    {
                        clefString = string.Format("{0} ", clefElement.ToString());
                    }
                }
            }

            string timeString = "";
            if (null != timeElements)
            {
                foreach (TimeElement timeElement in timeElements)
                {
                    if (!string.IsNullOrEmpty(timeElement.ToString()))
                    {
#warning Todo Localize
                        timeString = string.Format("{0}: {1} ", timeElement.NewTimeSignature, timeElement.ToString());
                    }
                }
            }

            string dynamicsString = "";
            string wordsString = "";
            if (userSettings.GetReaderSettings(UserSettings.ReaderSettingsEnum.Notations) && (null != directionElements))
            {
                foreach (DirectionElement directionElement in directionElements)
                {
                    if (null != directionElement.DynamicsElement)   
                    {
                        dynamicsString += directionElement.DynamicsElement.ToString() + " ";
                    }
                    
                    if (null != directionElement.DirectionTypeElement)
                    {
                        DirectionTypeElement directionType = directionElement.DirectionTypeElement;
                        if (null != directionType.WedgeElement)
                        {
                            dynamicsString += directionElement.DirectionTypeElement.WedgeElement.ToString() + " ";
                        }

                        if (null != directionType.WordsElement)
                        {
                            wordsString += directionElement.DirectionTypeElement.WordsElement.ToString() + " ";
                        }
                    }
                }
            }
            
            string measureStyleString = "";
            if (userSettings.GetReaderSettings(UserSettings.ReaderSettingsEnum.Notations) && (null != measureStyleElements))
            {  
                foreach (MeasureStyleElement measureStyleElement in measureStyleElements)
                {
                    if (null != measureStyleElement)
                    {
                        // According to the MusicXML spec The MeasureStyleElement is always a child of an attribute element
                        measureStyleString += measureStyleElement.ToString() + " ";
                    }
                }
            }                     


            StringBuilder sbNotes = new StringBuilder();
            StringBuilder sbTexts = new StringBuilder();
            // Iterate over the parts and build a complete representation of all notes and of all texts
            foreach (NoteElementList noteElementList in noteLists) // Iterate over the fixed number of parts.
            {
                string notes  = noteElementList.ToString(userSettings); // Represents all notes for all parts
                string lyrics = noteElementList.ToLyrics(userSettings); // Represents all lyrics for all parts
                // Assume that: The step is described with 3 characters. The octave with 1 character and max 2 notes per part !
                //sbNotes.Append(string.Format("{0,9} ", partNotes.Replace(" ", "")));  // Remove any blanks and fix width to 9 
                if (!string.IsNullOrWhiteSpace(notes)) // Avoid adding an extra blank if no text is available 
                {
                    sbNotes.Append(string.Format("{0,9} ", notes));  //  fix width to 9 
                }
                if (!string.IsNullOrWhiteSpace(lyrics))  // Avoid adding an extra blank if no text is available
                {
                    sbTexts.Append(string.Format("{0} ", lyrics));
                }
            }

            int newPages = 0;
            int newSystems = 0;
            string printFormatString = "";
            //if (UserSettings.??)
#warning TODO
            {
                if ((null != printElements))
                {
                    foreach (PrintElement printElement in printElements)
                    {
                        if (null != printElement.NewPage) newPages++;
                        if (null != printElement.NewSystem) newSystems++;

#if false  // Only for initial debugging !!
                        string s = string.Format("Measure={0}: PrintElement({1})", this.MeasureNumber,  printElement.ToDebugString());
                        LogFormatter.Log(LogFormatter.LogOptions.Always, s); 
#endif

                    }
                    // We assume that each part contains identical information. Is that right?
                    // Check against NumberOfParts !! 
                    Warn(this.numberOfParts, newPages, "NewPage");
                    Warn(this.numberOfParts, newSystems, "NewSystems");
                    //Logger.LogCF(string.Format(": NewPages={0} NewSystems={1}", newPages, newSystems));
                }
                if (0 != newPages) printFormatString += "New Page";
                if (0 != newSystems) printFormatString += "New System";
            }


            // Finnally compose the result by concatenating all the substrings in the sequence wanted
            // The first event description (to a certain extent) reflects the sequence in which information is aquired by the eye when scanning a music sheet for prima vista use.
            if (0 == this.startTime)
            {
                // Users suggest that we leave out the following information from the first eventDescription to avoid reading a lot of text just after loading the score.
                // This information can still be accessed as status details.
                // And they are still shown whenever they change AFTER the first eventDescription.
#warning ToDo Let the Ststus information reflect the key (Typically C or G)
                soundString = "";
                timeString = "";
                keyString = "";
                clefString = "";
            }

            return printFormatString + measure + soundString + timeString + keyString + clefString + repeatBackward + repeatForward + divisions + dynamicsString + wordsString + measureStyleString + sbNotes.ToString() + " " + sbTexts.ToString() + harmonyCode + harmony + endOfScore + endEventString  ;
        }


        /// <summary>
        /// Returns the duration in commonDivisions of the longest note starting at this EventDescription
        /// </summary>
        /// <param name="userSettings"></param>
        /// <returns></returns>
        public long GetLongestDuration(UserSettings userSettings)
        {
            long result = 0;
            foreach (NoteElementList noteElementList in this.noteLists)
            {
                result = Math.Max(result, noteElementList.GetLongestDuration(userSettings));
            }
            return result;
        }


        /// <summary>
        /// Returns the Endtime of the longset lasting note starting at this eventdescription, using the give UserSettings
        /// </summary>
        /// <param name="userSetings"></param>
        /// <returns></returns>
        public long GetLatestEndTime(UserSettings userSetings)
        {
            long longestDuration = GetLongestDuration(userSettings);
            return this.startTime + longestDuration;
        }


        /// <summary>
        /// Simple convenience method
        /// </summary>
        /// <param name="numberOfParts"></param>
        /// <param name="paramValue"></param>
        /// <param name="paramName"></param>
        private void Warn(int numberOfParts, int paramValue, string paramName)
        {
            if (paramValue == numberOfParts) return;
            if (paramValue == 0) return;
            {
                Logger.LogCF(string.Format(": Unexpected number of PrintElements with '{0}=Yes' = {1} ", paramName, paramValue));
            }
        }


        public int Sort(NoteElementComperator noteElementComperator)
        {
           int result = 0;
           foreach (NoteElementList noteElementList in this.noteLists)
           {             
                result += noteElementList.Sort(noteElementComperator);
           }
            return result;   
        }

    }
}

