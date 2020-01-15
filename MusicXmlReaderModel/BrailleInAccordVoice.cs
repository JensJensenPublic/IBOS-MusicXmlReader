using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXmlReaderModel
{
    class BrailleInAccordVoice
    {
        // Ref 1: Music Braille Code, 2015 - Braille Authority of North America https://www.google.dk/search?q=music+braille+code+2015&ie=&oe= 

        bool logInput = false;
        bool logMatrix = false;
        bool logLines = false;

        int part; // The part to which this item belongs
        public int Part { get { return part; } }
        int staff; // The staff to which thei item belongs
        public int Staff { get { return staff; } }
        int voice; // The voice to which thei item belongs
        public int Voice { get { return voice; } }
        NoteElementList notes = NoteElementList.Create();
        bool containsChords = false;
        public NoteElementList Notes { get { return notes; } }

        //private BrailleBuilder Format(BrailleBuilder line, ref int nVoices)
        //{
        //    return Format(line, ref nVoices, false); // Pasws on to the general signature.
        //}

        /// <summary>
        /// General method for formatting a line representing a voice in InAccord representation, independently of wether the voice contains chords or not
        /// The formatting consists of the adding of InAccord marks and MeasureSeparators at the right places.
        /// </summary>
        /// <param name="line">The Braille representation of the voice, but without InAccord marks and separators</param>
        /// <param name="nNonEmptyVoices">The number of non-eppty voices handled until now in this BrailleInAccordSegment </param>
        /// <param name="isFullMeasure">True iff the current segment is represents a full measure</param>
        /// <returns></returns>
        private BrailleBuilder Format(BrailleBuilder line, ref int nNonEmptyVoices, bool isFullMeasure)
        {
            if (string.IsNullOrEmpty(line.ToBrailleString()))
            {
                Logger.LogCF(string.Format(": Returned empty line"));
                return line;
            }
            else
            {
                BrailleBuilder result = BrailleBuilder.Create(line.TimeStamp);
                if (0 == nNonEmptyVoices)
                {
                    Logger.LogCF(string.Format(": nVoices=0 Returned line"));
                    nNonEmptyVoices++;              
                    return line; // This is the first line. no mark added in front of id
                }
                else
                {
                    // This is not the first line. Add an InAccordMArk in front of it.
                    // Use different symbols for 
                    // "Full-Measure In-Accords" (Danish: "Stor Bistemme") and
                    // "Part-Measure In-Accords" (Danish: "Lille Bistemme")
                    // as described in Ref1: Chapter 11.1.1 and 11.1.2
                    Logger.LogCF(string.Format(": nVoices={0} Returned InAccordMark and line",nNonEmptyVoices));
                    result.AddInAccordMark(isFullMeasure);
                    result.Append(line);
                    nNonEmptyVoices++;
                    return result;
                }
            }
        }


        /// <summary>
        /// Converts a voice of a BrailleInAccordSegment to the eqvivalent Music Braille representation.
        /// If the voice doesn not contain chords, this is equvivalent to converting a simple piece of monophonic music.
        /// If the voice contains chords we muat handle the situation where a single note (or rest) in the graphical notation 
        /// is used as a stand in for several similar notes or rests, one for each of the remaining notes in the MusicXml chord.
        /// This is handled by the noteMatrix and the surrounding code.
        /// </summary>
        /// <param name="userSettings"></param>
        /// <param name="isMainVoice"></param>
        /// <returns></returns>
        public BrailleBuilder ToBraille(UserSettings userSettings,ref int nVoices,bool isFullMeasure)
        {
            if (!this.containsChords)
            {
                EventDescription eventDescription = notes.NoteElements[0].OwningEventDescription;
                BrailleBuilder line = notes.ToBraille(userSettings, eventDescription);
                return this.Format(line, ref nVoices, isFullMeasure);
            }

            Utilities.Beep();
            Logger.LogCF(": Contains chords! ******************************************************************");

            // If the voice contains chords we cannot use the simple version of ToBraille(). Instead:
            int measure = notes.NoteElements[0].OwningEventDescription.CurrentMeasureNumber;
            if (logInput) Logger.LogCF(string.Format(": Measure={0} P{1} S{2} V{3} {4} ", measure, part, staff, voice, notes.ToDebugString()));


            // Create a 2-dimensional array of NoteElements.
            // Each coloumn represents a starttime
            // Each row represents an index within the notes in a MusicXml chord-representation.
            List<long> startTimes = new List<long>(); 
            List<int> chordIndexes = new List<int>();
            foreach (NoteElement note in Notes.NoteElements)
            {
                if (!startTimes.Contains(note.StartTime))
                {
                    startTimes.Add(note.StartTime);
                }
                if ((!chordIndexes.Contains(note.BrailleMeasureDivisionInfo.ChordIndex)))
                {
                    chordIndexes.Add(note.BrailleMeasureDivisionInfo.ChordIndex);
                }
            }

            int numberOfRows = chordIndexes.Count;
            int numberOfCols = startTimes.Count;

            // Fill in the (sparse) array with all available notes        
            NoteElement[,] noteMatrix = new NoteElement[numberOfRows, numberOfCols];
            foreach (NoteElement note in Notes.NoteElements)
            {
                int col = startTimes.IndexOf(note.StartTime);
                int row = chordIndexes.IndexOf(note.BrailleMeasureDivisionInfo.ChordIndex);        

                noteMatrix[row,col] = note;
                if (logMatrix) Logger.LogCF(string.Format(": Measure={0} NoteMatrix:  Row={1} Col={2} {3}", measure, row, col, note.ToShortDebugString(false, false)));
            }

            // Fill in the remaining cells with the contents of the first row != null in the same coloumn
            for (int col = 0; col < numberOfCols; col++)
            {
                for (int row = 0; (row < numberOfRows); row++)
                {
                    NoteElement note = noteMatrix[row, col];
                    if (null == note)
                    {
                        // Find a replacement in the same coloumn
                        NoteElement replacementNote = null;
                        for (int r = 0; ((r < numberOfRows) && (null == replacementNote)); r++)
                        {
                           replacementNote = noteMatrix[r, col];
                        }
                        if (null == replacementNote)
                        {
                            Logger.LogCF(string.Format(": Measure={0}  ReplacementNote for Row={1} Col={2} is null", measure, row, col));
                        }
#warning ToDo Replace by a similar rest !!
                        noteMatrix[row, col] = replacementNote;
                    }
                }
            }

            BrailleBuilder result = BrailleBuilder.Create(0);
            // result.Append(0, "V");
            for (int row = 0; (row < numberOfRows); row++)
            {
                NoteElementList notes = NoteElementList.Create();
                for (int col = 0; (col < numberOfCols); col++)
                {
                    notes.NoteElements.Add(noteMatrix[row, col]);
                }
#warning ToDo Find out what to do about the eventDescription parameter !!    
                BrailleBuilder line = notes.ToBraille(userSettings, notes.NoteElements[0].OwningEventDescription); // Convert the line to Braille
                result.Append(this.Format(line,ref nVoices, isFullMeasure)); // 
                if (logLines) Logger.LogCF(string.Format(": Measure={0} Row={1} {2}", measure, row, notes.ToDebugString(false)));
            }


            return result;
        }


        private BrailleInAccordVoice() { }

        private BrailleInAccordVoice(NoteElement note)
        {
            this.part = note.PartNumber;
            this.staff = note.Staff;
            this.voice = note.Voice;
            this.containsChords = this.containsChords || note.Chord;
            this.notes.NoteElements.Add(note);
        }


        public static BrailleInAccordVoice Create(NoteElement note)
        {
            return new BrailleInAccordVoice(note);
        }
    }
}
