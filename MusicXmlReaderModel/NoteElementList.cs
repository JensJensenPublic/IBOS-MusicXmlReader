using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXmlReaderModel
{
    /// <summary>
    /// Represents a list of MusicXml NoteElements.
    /// The NoteElements may come form many varyous sources: They may belong to
    ///  One specific staff in one specific part, 
    ///  Various staffs in the same part
    ///  Various staffs and parts within the whole score
    ///  Or have any other relation.
    /// </summary>
    public class NoteElementList
    {
        private List<NoteElement> noteElements = new List<NoteElement>();
        public  List<NoteElement> NoteElements { get { return noteElements; } }

        private NoteElementList()
        {            
        }

        private NoteElementList(List<NoteElement> noteElements)
        {
            this.noteElements = noteElements;
        }

        /// <summary>
        /// Returns the duration in the unit of CommonDivivions of the longest note in the notelist
        /// </summary>
        /// <returns></returns>
        public long GetLongestDuration(UserSettings userSettings)
        {
#warning TODO use usersettings for filtering
            long result = 0;
            foreach (NoteElement noteElement in this.NoteElements)
            {
                result = Math.Max(result, noteElement.DurationInCommonDivisions);
            }
            return result;
        }

        /// <summary>
        /// Used during debugging to catch obvious programming errors.
        /// </summary>
        /// <param name="ok"></param>
        private void Assert(bool ok,string message)
        {
            if (ok) return;
            Logger.LogCF(string.Format(": {0}",message));            
        }


        /// <summary>
        /// ONLY for debugging purposes ! May be changed as required!
        /// </summary>
        /// <returns></returns>
        public string ToDebugString(bool assertSameStartTime)
        {
            StringBuilder sb = new StringBuilder();
            foreach (NoteElement noteElement in this.NoteElements)
            {
                if (assertSameStartTime) Assert((this.NoteElements[0].StartTime == noteElement.StartTime),"Unexpected different StartTimes");
                Assert((this.NoteElements[0].MeasureNumber == noteElement.MeasureNumber),"Unexpected different MeasureNumbers");       
                sb.Append(string.Format("({0})  ", noteElement.ToShortDebugString()));
            }
            return sb.ToString();
        }

        public string ToDebugString()
        {
            return ToDebugString(true);
        }


        /// <summary>
        /// The simple version of NoteElementList.ToBraille() without chord notation.
        /// Used from
        ///  DetailsPlayer.PartDetailsPlayer for showing explicit details of a single event
        ///  BrailleMeasureDivision.ToBraille() when chord notation is enabled, but not needed
        /// </summary>
        /// <param name="userSettings"></param>
        /// <param name="owningEventDescription"></param>
        /// <returns></returns>
        public BrailleBuilder ToBraille(UserSettings userSettings, EventDescription owningEventDescription)
        {
            EventDescription dummyArgument = null;
            return ToBraille(userSettings, owningEventDescription, false, out dummyArgument);
        }


        /// <summary>
        /// The general version of NoteElementList.ToBraille(). All other versions of NoteElementList.ToBraille() maps to this one, using default call parameters.
        /// Depending on the call parameters this general version generates either
        ///  A simple list of notes
        ///  Interval representation
        ///  MesaureDivision representation
        /// </summary>
        /// <param name="userSettings"></param>
        /// <param name="owningEventDescription"></param>
        /// <param name="useChordNotation"></param>
        /// <param name="nextEventDescription">
        /// Will be set to the next EventDescription to handle, typically eventDescription.Next.
        /// When MeasureDivision notation is used set to the first EventDescription AFTER the block of EventDescriptions handled by the current MeasureDescription!
        /// </param>
        /// <returns></returns>
        public BrailleBuilder ToBraille(UserSettings userSettings, EventDescription owningEventDescription, bool useChordNotation, out EventDescription nextEventDescription)
        {
            // Extract what we need from owningEventDescription.
            long startTime = owningEventDescription.StartTime;
            StatusInformation statusInformation = owningEventDescription.StatusInformation;
            nextEventDescription = owningEventDescription.Next; // The default. The only exception is BrailleMeasureDivision notation

            BrailleBuilderForIntervalNotation bb = BrailleBuilderForIntervalNotation.Create(startTime);
            if (!userSettings.GetMusicBrailleSettings(UserSettings.MusicBrailleSettingsEnum.Notes)) return bb; // User completely turned off reading of notes
            bool addNotations = userSettings.GetMusicBrailleSettings(UserSettings.MusicBrailleSettingsEnum.Notations);


            if (useChordNotation)
            {
                // First attempt to get away with using Interval Notation. This requires that all Notes are of equal length !
                List<NoteElement> selectedNotes = ChordNotation(userSettings);
                if ((null != selectedNotes) && (selectedNotes.Count >= 2))
                {
                    bool fromTop = (userSettings.SelectedStaffs != null) && (userSettings.SelectedStaffs[0].FromTop); // The first selected staff decides
                    if (bb.AdIntervalNotation(selectedNotes, statusInformation, addNotations, fromTop))
                    {
                        return bb;
                    }
                    else
                    {
                        // We can't Interval Notation, because the notes are not of equal length.
                        // Use MeasureDivision notation instead. Note that the output parameter nextEventDescription is set to the first EventDescription AFTER
                        // the block of EventDescriptions handled by MeasureDivision !
                        BrailleInAccordSegment brailleMeasureDivisionSegment = BrailleInAccordSegment.Create(owningEventDescription, userSettings);
                        nextEventDescription = brailleMeasureDivisionSegment.NextEventDescription;
                        return brailleMeasureDivisionSegment.ToBraille(userSettings,fromTop);
                    }
                }
            }

            // We don't need Interval notation. Just continue as in version <= 3.0
            const string prolog = "";
            const string epilog = " ";
            foreach (NoteElement noteElement in noteElements) // Iterate over the notes within one part! For instance (S1,S2).
            {

                try
                {
                    // Add pitch information
                    // If the note is not marked for printing with the PrintObjectAttributeValue we ignore it
                    if ((userSettings.GetParts(UserSettings.Category.MusicBraille, noteElement.PartNumber))
                    && noteElement.PrintObjectAttributeValue
                    && userSettings.IsSelected(noteElement.PartId, noteElement.Staff) // For generating BrailleMusic for separate staffs
                    )
                    {
                        // bool addNotations = userSettings.GetMusicBrailleSettings(UserSettings.MusicBrailleSettingsEnum.Notations);
                        BrailleBuilder bb1 = BrailleBuilder.Create(startTime); // TO DO: Why not use bb directly ???
                                                                                    // userSettings.ReadNotePitch, userSettings.ReadNoteOctave, userSettings.ReadNoteDuration (Danish: Tone/Oktav/Varighed)
#warning TODO Refactor: Only the calls to bb.AddRest/bb.AddNote seem to be different !
                        if (noteElement.IsPause)
                        {
                            // This is a rest 
                            bb1.AppendText(prolog);
                            if (addNotations) bb1.AddBrailleNotationsBeforeNoteOrRest(noteElement.Notations); // Some notations are added Before the note/rest itself 
                            bb1.AddRest(noteElement.NoteDuration, false); // TO DO: Handle punctured rests
                            if (addNotations) bb1.AddBrailleNotationsAfterNoteOrRest(noteElement.Notations);  // Some notations are added After the note/rest itself
                            bb1.AppendText(epilog);
                        }
                        else
                        {
                            // This is a note
                            bb1.AppendText(prolog);
                            if (addNotations) bb1.AddBrailleNotationsBeforeNoteOrRest(noteElement.Notations); // Some notations are added Before the note/rest itself                       
                            bb1.AddNote(noteElement, statusInformation.CurrentKeyElement);
                            if (addNotations) bb1.AddBrailleNotationsAfterNoteOrRest(noteElement.Notations);  // Some notations are added After the note/rest itself
                            bb1.AppendText(epilog);

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

        private List<NoteElement> ChordNotation(UserSettings userSettings)
        {
            // Start experimental code for generating Chord notation!!
            if (noteElements.Count <= 1) return null; // Performance optimization: Most times we can stop here !
            List<NoteElement> selectedNotes = new List<NoteElement>(); // Pick up and sort the relevant notes 
            foreach (NoteElement noteElement in noteElements) // Iterate over the notes within one part! For instance (S1,S2).
            {
                if ((userSettings.GetParts(UserSettings.Category.MusicBraille, noteElement.PartNumber))
                && noteElement.PrintObjectAttributeValue
                && userSettings.IsSelected(noteElement.PartId, noteElement.Staff) // For generating BrailleMusic for separate staffs
                && (!noteElement.IsPause) // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! THINK !!
#warning TODO Think 
                    )
                {
                    selectedNotes.Add(noteElement);
                }
            }
            if (selectedNotes.Count >= 2)
            {
                return selectedNotes; // 2 or more notes for the same part and staff at the same time. We must use IntervalNotation or "Bistemmer"
            }
            return null;
        }

        ///// <summary>
        ///// Simple mechanism for selecting a specific staff within a spscific part.
        ///// Used for generating BrailleMusic information one staff at a time
        ///// When generating BrailleMusic for the UI UserSettings.SelectedStaff is null, signalling that no spscific staff is salected.
        ///// </summary>
        ///// <param name="partId"></param>
        ///// <param name="staffNumberWithinPart"></param>
        ///// <returns></returns>
        //public bool IsSelected(UserSettings userSettings,string partId, int staffNumberWithinPart)
        //{
        //    if (null == userSettings.SelectedStaff) return true; // No specific staff is selected. Merge information for all parts enabled in UserSettings.
        //    return userSettings.SelectedStaff.Equals(partId, staffNumberWithinPart);  // A specific staff is selected.   Only return information for this staff 
        //}

        //private bool IsSelected(UserSettings userSettings,string partId)
        //{
        //    if (null == userSettings.SelectedStaff) return true; // No part staff is selected. Merge information for all parts enabled in UserSettings.
        //    return (0 == string.Compare(userSettings.SelectedStaff.PartId, partId));  // A specific part is selected.   Only return information for this part 
        //}



        /// <summary>
        /// Generate a string representing the (possibly multiple) notes of a single part
        /// This function is where we can really differentiate ourselves from mainstream products such as MuseScore
        /// </summary>
        /// <param name="noteElementList"></param>
        /// <returns></returns>
        public string ToString(UserSettings userSettings)
        {
            if (!userSettings.GetReaderSettings(UserSettings.ReaderSettingsEnum.Notes)) return ""; // User completely turned off reading of notes
            if (0 == noteElements.Count()) return " "; // Nothing happened in this part 
            StringBuilder sb = new StringBuilder();
            foreach (NoteElement noteElement in noteElements) // Iterate over the notes within one part! For instance (S1,S2).
            {
                //string delimiter = string.IsNullOrEmpty(sb) ? "" : "+"; // Use this string to separate notes within one part
                // Add pitch information
                if (userSettings.GetParts(UserSettings.Category.Speech, noteElement.PartNumber)) // Might later look at subparts S1/S2 ? 
                {

                    string note = "";
                    // userSettings.ReadNotePitch, userSettings.ReadNoteOctave, userSettings.ReadNoteDuration (Danish: Tone/Oktav/Varighed)
                    if (noteElement.IsPause)
                    {
                        // This is a pause
                        // Here the type and the word "pause" are cocatenated such as "punkteret halvnodepause"
                        string type = userSettings.GetReaderSettings(UserSettings.ReaderSettingsEnum.NoteTypes) ? noteElement.LocalizedPauseType : "pause";
                        string notations = (userSettings.GetReaderSettings(UserSettings.ReaderSettingsEnum.Notations) && (null != noteElement.Notations)) ? noteElement.Notations.ToString() : "";
                        note = string.Format(" {0} {1}", type, notations);
                    }
                    else
                    {
                        // This is a note
                        // Here the sequence is pitch,octave,type such af "Cis4 punkteret halvnode"
                        string accidental = (userSettings.GetReaderSettings(UserSettings.ReaderSettingsEnum.NoteAccidentals) && (null != noteElement.AccidentalElement)) ? noteElement.AccidentalElement.ToString() : "";
                        string pitch = noteElement.PitchValue.Name; // Always use the name of the note
                        string octave = userSettings.GetReaderSettings(UserSettings.ReaderSettingsEnum.NoteOctaves) ? noteElement.Octave.ToString() : "";
                        string type = userSettings.GetReaderSettings(UserSettings.ReaderSettingsEnum.NoteTypes) ? noteElement.LocalizedType : "";
                        string pitchAndOctave = noteElement.UnPitched ? noteElement.UnpitchedText : string.Format("{0}{1}", pitch, octave);
                        string cueString = noteElement.CueNoteString;
                        string notations = (userSettings.GetReaderSettings(UserSettings.ReaderSettingsEnum.Notations) && (null != noteElement.Notations)) ? noteElement.Notations.ToString() : "";
                        //                      note = string.Format("{0,-4} {1}", pitchAndOctave, type); // Always use 4 chars for pitch and Octave. Examples: "C   ","Cis4"
                        string printability = noteElement.PrintObjectAttributeValue ? "" : string.Format("({0})", ResourcesForModel.EventDescription_NotPrinted); // TODO USe Resources !
                        note = string.Format("{0} {1} {2} {3} {4} {5}", accidental, pitchAndOctave, type, cueString, notations, printability);    // Do not use extra chars for Pitch and Octave. Examples: "C","Cis4"
                    }

                    // string note = string.IsNullOrEmpty(noteElement.Step) ? "Pause" : noteElement.PitchValue.Name + noteElement.PitchValue.Octave + " " +noteElement.LocalizedType;
                    //s = s + delimiter + note;
                    if (sb.Length > 0) sb.Append(" "); // Separate the notes with ""
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
        public string ToLyrics(UserSettings userSettings)
        {
            if (!userSettings.GetReaderSettings(UserSettings.ReaderSettingsEnum.Lyrics)) return "";
            if (0 == this.NoteElements.Count()) return ""; // Nothing happened in this part 
            StringBuilder sb = new StringBuilder();
            foreach (NoteElement noteElement in this.NoteElements) // Iterate over the notes within one part! For instance (S1,S2).
            {
                if (userSettings.GetParts(UserSettings.Category.Speech, noteElement.PartNumber))
                {
                    if (!string.IsNullOrEmpty(noteElement.Text))
                    {
                        sb.Append((0 == sb.Length) ? "" : " ");
                        sb.Append(noteElement.Text);
                    }
                }
            }
            return sb.ToString();
        }



        public static NoteElementList Create()
        {
            return new NoteElementList();
        }

        public static NoteElementList Create(List<NoteElement> noteElements)
        {
            return new NoteElementList(noteElements);
        }
    }
}
