using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using MusicXmlReaderUI;

namespace MusicXmlReaderModel
{

    public class EventDescription
    {
        string className = "EventDescription";
        int startTime;
        int numberOfParts;
        UserSettings userSettings;
        int metronomeBeatsPerMinute = -1;

        string musicBrailleRepresentation;
        string musicBrailleAsTextRepresentation;
        string textRepresentation;

        /// <summary>
        /// Notes to be played at this time
        /// </summary>
//        private NoteElement[] notes; // TO DO: Remove notes. Use noteLists instead!!

        private List<NoteElement>[] noteLists; // An array of lists of notes

        private HarmonyElement harmonyElement; // The harmony related to this event, if any.

        private MeasureElement measureElement; //The  measure related to this event, if any.
        //private int MeasureNumber = -1; // The measure Number if this event falls on a measure border.

        private RepeatElement repeatElementForward; // The RepeatElement  with the "forward" attribute relateted to this event, if any
        private RepeatElement repeatElementBackward; // The RepeatElement  with the "backward" attribute relateted to this event, if any

        private List<EndEventElement> endEventElements; // Elements (for instance NoteElements) to end at this time

        // A few other elements may be related to a specific event
        private List<SoundElement> soundElements;           // The SoundElements related to this event, if any
        private List<KeyElement>   keyElements;             // The KeyElements related to this event, if any
        private List<ClefElement>  clefElements;            // The ClefElements related to this event, if any
        private List<TimeElement> timeElements;             // The TimeElements related to this event, if any
        private List<BarlineElement> barlineElements;       // The BarlineElements related to this event, if any. In some rare cases more than one!!
        private List<DirectionElement> directionElements;   // The DirectionElements related to this event, if any 
        private List<MeasureStyleElement> measureStyleElements; // The MeasureStyleElements related to this event, if any 

        private StatusInformation statusInformation; // Contains Status information valid for this eventdescription

        public int MeasureNumber
        {
            get
            {
                return (null == measureElement) ? -1 : measureElement.Number;
            }

        }

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

        internal StatusInformation StatusInformation
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
                noteLists[noteElement.PartNumber].Add(noteElement);
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
                &&  (null != directionElement.DirectionTypeElement.MetronomeElement) &&
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

        }

        /// <summary>
        /// Generate a representing of the (possibly multiple) notes of a single part
        /// This function is where we can really differentiate ourselves from mainstream products such as MuseScore
        /// </summary>
        /// <param name="noteElementList"></param>
        /// <returns></returns>
        private BrailleBuilder NotesForOnePartAsBraille(List<NoteElement> noteElementList)
        {
            BrailleBuilder bb = BrailleBuilder.Create();
            if (!userSettings.GetMusicBrailleSettings(UserSettings.MusicBrailleSettings.Notes)) return bb; // User completely turned off reading of notes


            foreach (NoteElement noteElement in noteElementList) // Iterate over the notes within one part! For instance (S1,S2).
            {
                try
                {
                    // Add pitch information
                    // If the note is not marked for printing with the PrintObjectAttributeValue we ignore it
                    if ((userSettings.partsToBraille[noteElement.PartNumber]) && noteElement.PrintObjectAttributeValue)// Might later look at subparts S1/S2 ? 
                    {
                        bool addNotations = userSettings.GetMusicBrailleSettings(UserSettings.MusicBrailleSettings.Notations);
                        BrailleBuilder bb1 = BrailleBuilder.Create(); // TO DO: Why not use bb directly ???
                                                                      // userSettings.ReadNotePitch, userSettings.ReadNoteOctave, userSettings.ReadNoteDuration (Danish: Tone/Oktav/Varighed)
                        if (noteElement.IsPause)
                        {
                            // This is a rest 
                            bb1.Append("<");
                            if (addNotations) bb1.AddBrailleNotationsBeforeNoteOrRest(noteElement.Notations); // Some notations are added Before the note/rest itself 
                            bb1.AddRest(noteElement.NoteDuration, false); // TO DO: Handle punctured rests
                            if (addNotations) bb1.AddBrailleNotationsAfterNoteOrRest(noteElement.Notations);  // Some notations are added After the note/rest itself
                            bb1.Append(">");
                        }
                        else
                        {
                            // This is a note
                            bb1.Append("<");
                            if (addNotations) bb1.AddBrailleNotationsBeforeNoteOrRest(noteElement.Notations); // Some notations are added Before the note/rest itself                       
                            bb1.AddNote(noteElement,statusInformation.CurrentKeyElement);
                            if (addNotations) bb1.AddBrailleNotationsAfterNoteOrRest(noteElement.Notations);  // Some notations are added After the note/rest itself
                            bb1.Append(">");

                        }
                        //bb.Append(bb1.Braille, bb1.Text.ToString());
                        bb.Append(bb1);
                    }
                }
                catch (Exception e)
                {
                    Logger.Log(string.Format("EventDescription.NotesForOnePartAsBraille threw an exception. Message='{0}'", e.Message));
                    Logger.Log(string.Format("NoteElement: Step={0} Alter={1} Octave={2} Duration={3} Measure={4} Part={5}",
                        noteElement.Step, noteElement.Alter, noteElement.Octave, noteElement.Duration.ToString(), noteElement.MeasureNumber, noteElement.PartId));
                }
            }

            return bb;
        }
                

        /// <summary>
        /// Generate a string representing the (possibly multiple) notes of a single part
        /// This function is where we can really differentiate ourselves from mainstream products such as MuseScore
        /// </summary>
        /// <param name="noteElementList"></param>
        /// <returns></returns>
        public string NotesForOnePart(List<NoteElement> noteElementList)
        {
            if (!userSettings.GetReaderSettings(UserSettings.ReaderSettings.Notes)) return ""; // User completely turned off reading of notes
            if (0 == noteElementList.Count()) return " "; // Nothing happened in this part 
            StringBuilder sb = new StringBuilder();
            foreach (NoteElement noteElement in noteElementList) // Iterate over the notes within one part! For instance (S1,S2).
            {
                //string delimiter = string.IsNullOrEmpty(sb) ? "" : "+"; // Use this string to separate notes within one part
                // Add pitch information
                if (userSettings.partsToRead[noteElement.PartNumber]) // Might later look at subparts S1/S2 ? 
                {

                    string note = "";
                    // userSettings.ReadNotePitch, userSettings.ReadNoteOctave, userSettings.ReadNoteDuration (Danish: Tone/Oktav/Varighed)
                    if (noteElement.IsPause)
                    {
                        // This is a pause
                        // Here the type and the word "pause" are cocatenated such as "punkteret halvnodepause"
                        string type = userSettings.GetReaderSettings(UserSettings.ReaderSettings.NoteTypes) ? noteElement.LocalizedPauseType : "pause"; 
                        note = string.Format("{0}",type);
                    }
                    else
                    {
                        // This is a note
                        // Here the sequence is pitch,octave,type such af "Cis4 punkteret halvnode"
                        string accidental = (userSettings.GetReaderSettings(UserSettings.ReaderSettings.NoteAccidentals) && (null != noteElement.AccidentalElement)) ?  noteElement.AccidentalElement.ToString() : "";
                        string pitch    = noteElement.PitchValue.Name; // Always use the name of the note
                        string octave   = userSettings.GetReaderSettings(UserSettings.ReaderSettings.NoteOctaves) ? noteElement.Octave.ToString() : "";
                        string type     = userSettings.GetReaderSettings(UserSettings.ReaderSettings.NoteTypes) ? noteElement.LocalizedType : "";
                        string pitchAndOctave = string.Format("{0}{1}", pitch, octave);
                        string cueString = noteElement.CueNoteString;                        
                        string notations = (userSettings.GetReaderSettings(UserSettings.ReaderSettings.Notations) && (null != noteElement.Notations)) ? noteElement.Notations.ToString() : "";
                        //                      note = string.Format("{0,-4} {1}", pitchAndOctave, type); // Always use 4 chars for pitch and Octave. Examples: "C   ","Cis4"
                        note = string.Format("{0} {1} {2} {3} {4}", accidental, pitchAndOctave, type, cueString, notations);    // Do not use extra chars for Pitch and Octave. Examples: "C","Cis4"
                    }

                    // string note = string.IsNullOrEmpty(noteElement.Step) ? "Pause" : noteElement.PitchValue.Name + noteElement.PitchValue.Octave + " " +noteElement.LocalizedType;
                    //s = s + delimiter + note;
                    if (sb.Length > 0)  sb.Append(" "); // Separate the notes with ""
                    sb.Append(note);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Generate a string representing the (possibly multiple) texts of a single part
        /// </summary>
        /// <param name="noteElementList"></param>
        /// <returns></returns>
        public string LyricsForOnePart(List<NoteElement> noteElementList)
        {
            if (!userSettings.GetReaderSettings(UserSettings.ReaderSettings.Lyrics)) return ""; 
            if (0 == noteElementList.Count()) return ""; // Nothing happened in this part 
            StringBuilder sb = new StringBuilder();
            foreach (NoteElement noteElement in noteElementList) // Iterate over the notes within one part! For instance (S1,S2).
            {
                if (!string.IsNullOrEmpty(noteElement.Text))
                {
                    sb.Append((0 == sb.Length) ? "" : " ");
                    sb.Append(noteElement.Text);
                }
            }
            return sb.ToString();
        }

        public BrailleBuilder ToMusicBrailleString()
        {
            if (!userSettings.MusicAsMusicBraille) return BrailleBuilder.Create();
            return ToBraille();
        }

        /// <summary>
        /// Generates the Braille representation, where ToNormalTextString() generates the text representation 
        /// Same structure as ToNormalTextString()
        /// Depending on settings ToString  will generate a mix of the results of ToBraille and ToNormalTextString()
        /// </summary>
        /// <returns></returns>
        public BrailleBuilder ToBraille()
        {
            string functionName = "ToBraille";
            bool repeatForward  = false; // Max one Repeat forward per eventdescription
            bool repeatBackward = false; // Max one Repeat backward per eventdescription

            // const string functionName = "EventDescription.ToBraille";
            BrailleBuilder bbBeforeNotes = BrailleBuilder.Create(); // For information not contained in notes


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


            //string measure = "";
            //if (userSettings.GetReaderSettings(UserSettings.ReaderSettings.MeasureNumbers))
            //{
            //    measure = (null != measureElement) ? string.Format("Takt {0,3} ", measureElement.Number) : "         "; // Up to 1000 measures
            //}

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
                if (1 == clefElements.Count)
                {
                    ClefElement c = clefElements[0];
                    bbBeforeNotes.Append("(");
                    bbBeforeNotes.AddClef(c, c.ToShortString());  // Get the unlocalized version
                    bbBeforeNotes.Append(")");
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
                bbBeforeNotes.Append("(");
                bbBeforeNotes.AddKey(k0, k0.ToShortString());
                bbBeforeNotes.Append(")");                
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
                bbBeforeNotes.Append("(");
                bbBeforeNotes.AddTime(t0, t0.ToShortString());
                bbBeforeNotes.Append(")");
            }

            

            BrailleBuilder bbNotes = BrailleBuilder.Create();
            // Iterate over the parts and build a complete representation of all notes and of all texts
            foreach (List<NoteElement> noteElementList in noteLists) // Iterate over the fixed number of parts.
            {
                BrailleBuilder notes  = NotesForOnePartAsBraille(noteElementList); // Represents all notes for all parts
                bbNotes.Append(notes);  
            }

            // Extract information to be shown after the notes
            BrailleBuilder bbAfterNotes = BrailleBuilder.Create();
            // Look for repeat forward/backward and insert, but only one of each per event description !!
            // Look for a termination (Danish "Helslutning") and insert the appropriate sequence
            bool foundTermination = false;
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
                    else
                    {
                        if ((barlineElement.Location == BarlineLocationEnum.right)
                        && (barlineElement.EndingElement == null)
                        && (barlineElement.RepeatElement == null)
                        && (barlineElement.BarStyleElement != null)
                        && (barlineElement.BarStyleElement.BarStyle == BarStyleEnum.lightHeavy))
                        {
                            foundTermination = true;
                        }
                    }
                }
                if (foundTermination)
                {
                    bbAfterNotes.Append(BrailleBuilder.fullEnd, "FullEnd");
                    Logger.LogOnce(string.Format("{0}.{1} Found a termination", className, functionName));
                }
            }

            // Finnally compose the result by concatenating all the substrings in the sequence wanted
            //return measure + repeatForward + divisions + sbNotes.ToString() + " " + sbTexts.ToString() + harmonyCode + harmony + endEventString + soundString + keyString + clefString + timeString + repeatBackward;

            // Ad the various components:
            BrailleBuilder total = BrailleBuilder.Create();
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
            BrailleBuilder bb = ToMusicBrailleString();
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


        public string ToMusicBrailleAndTextBrailleString()
        {
            string mb = ToMusicBrailleString().ToBrailleString(); 
            string nt = ToNormalTextString();            
            return string.Format("{0} {1}", mb, nt);
        }

        /// Generates a normal text representation.
        /// Same structure as ToBraille()
        /// Depending on settings ToString  will generate a mix of the results of ToBraille and ToNormalTextString() 
        public string ToNormalTextString()
        {
            if (!userSettings.MusicAsSpeech) return "";

            string divisions = userSettings.GetReaderSettings(UserSettings.ReaderSettings.Divisions) ? string.Format("{0,6}: ", startTime, "") : "";

            // A BarlineElement can contain a RepeatElements containing repetition information.
            string repeatBackward = "";
            string repeatForward = "";
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
                } 
            }

            string measure = "";
            if (userSettings.GetReaderSettings(UserSettings.ReaderSettings.MeasureNumbers))
            {
                measure = ((null != measureElement) && (!measureElement.ImplicitMeasure ))? string.Format("{0} {1,3} ",ResourcesForModel.NoteElement_measure_text, measureElement.Number) : "         "; // Up to 1000 measures
            }
            
            string harmonyCode = "";
            if ((userSettings.GetReaderSettings(UserSettings.ReaderSettings.HarmonyCodes)) && (null != harmonyElement))
            {
                harmonyCode = string.Format(" {0} {1} {2} : ",harmonyElement.Kind, harmonyElement.RootStep, harmonyElement.RootAlter);
            }


            string harmony = "";
            if ((userSettings.GetReaderSettings(UserSettings.ReaderSettings.Harmonies)) && (null != harmonyElement))
            {
                harmony = string.Format("{0}{1}  ",harmonyElement.ChromaticStep, harmonyElement.LocalizedChordType); // Use same formatting as used in the status line !!
            }


            string endEventString = "";
            if ((userSettings.GetReaderSettings(UserSettings.ReaderSettings.EndEvents)) && (null != endEventElements))
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
                        soundString = string.Format("{0}:{1} ", ResourcesForModel.EventDescription_tempo, soundElement.TempoValue); 
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
                        timeString = string.Format("{0} ", timeElement.ToString());
                    }
                }
            }

            string dynamicsString = "";
            if (userSettings.GetReaderSettings(UserSettings.ReaderSettings.Notations) && (null != directionElements))
            {
                foreach (DirectionElement directionElement in directionElements)
                {
                    if (null != directionElement.DynamicsElement)   
                    {
                        dynamicsString += directionElement.DynamicsElement.ToString() + " ";
                    }
                }
            }
            
            string measureStyleString = "";
            if (userSettings.GetReaderSettings(UserSettings.ReaderSettings.Notations) && (null != measureStyleElements))
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
            foreach (List<NoteElement> noteElementList in noteLists) // Iterate over the fixed number of parts.
            {
                string notes  = NotesForOnePart(noteElementList); // Represents all notes for all parts
                string lyrics = LyricsForOnePart(noteElementList); // Represents all texts for all parts
                // Assume that: The step is described with 3 characters. The octave with 1 character and max 2 notes per part !
                //sbNotes.Append(string.Format("{0,9} ", partNotes.Replace(" ", "")));  // Remove any blanks and fix width to 9 
                sbNotes.Append(string.Format("{0,9} ", notes));  //  fix width to 9 
                sbTexts.Append(string.Format("{0} ", lyrics));
            }

            // Finnally compose the result by concatenating all the substrings in the sequence wanted
            // The first event description (to a certain extent) reflects the sequence in which information is aquired by the eye when scanning a music sheet for prima vista use.
            return measure + soundString + timeString + keyString + clefString + repeatBackward + repeatForward + divisions + dynamicsString + measureStyleString + sbNotes.ToString() + " " + sbTexts.ToString() + harmonyCode + harmony + endEventString  ;
        }
    }
}

