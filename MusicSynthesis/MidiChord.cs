using System.Collections.Generic;
using NAudio.Midi;

namespace JSJ.MusicSynthesis
{
    public class MidiChord
    {
        // http://www.musicxml.com/UserManuals/MusicXML/Content/ST-MusicXML-kind-value.htm

        private List<MidiNote> midinotes = new List<MidiNote>();

        public static ChordType GetChordType(string chordString)
        {
            switch (chordString)
            {
                // Minor
                case "minor": return ChordType.Minor;
                case "minor-sixth": return ChordType.Minor6;
                case "minor-seventh": return ChordType.Minor7;
                case "minor-ninth": return ChordType.Minor9;
                case "minor-11th": return ChordType.Minor11;
                case "minor-13th": return ChordType.Minor13;

                // Major
                case "major": return ChordType.Major;
                case "major-sixth": return ChordType.Major6;
                case "major-seventh": return ChordType.Major7maj;
                case "dominant": return ChordType.Major7;
                case "major-ninth": return ChordType.Major9;
                case "major-11th": return ChordType.Major11;
                case "major-13th": return ChordType.Major13;

                // Diminished
                case "diminished":; return ChordType.Dim;
                case "half-diminished":; return ChordType.Dim7;
                // case "diminished-seventh":

                // Augmented
                // case "augmented":
                // case "augmenteded-seventh":

                // Dominant
                // case "dominant-ninth":
                // case "dominant-11th":
                // case "dominant-13th": 

                // Suspended
                //case "suspended-second": return ChordType.Sus2;
                case "suspended-fourth": return ChordType.Sus4;
                //case "" ; return ChordType.;
                //case "" ; return ChordType.;
                //case "" ; return ChordType.;

                // Other
                // case "major-minor":

                default: return ChordType.UnImplemented;
            }
        }


        public static string LocalizeChordType(ChordType chordType)
        {
            switch (chordType)
            {
                // Minor
                case ChordType.Minor: return "m";
                case ChordType.Minor6: return "m6";
                case ChordType.Minor7: return "m7";
                case ChordType.Minor7maj: return "molmaj7";
                case ChordType.Minor9: return "m9";
                case ChordType.Minor11: return "m11";
                case ChordType.Minor13: return "m13";

                // Major
                case ChordType.Major: return ""; // Implicitly "dur"
                case ChordType.Major6: return "6";
                case ChordType.Major7: return "7";
                case ChordType.Major7maj: return "maj7";
                case ChordType.Major9: return "9";
                case ChordType.Major11: return "11";
                case ChordType.Major13: return "13";

                // Diminished
                case ChordType.Dim: return "ø";
                case ChordType.Dim7: return "ø7";

                // Suspended
                case ChordType.Sus4: return "sus4";

                default: return "Ikke implementeret";
            }
        }
        

        /// <summary>
        /// Constructor
        /// Does NOT start playing the chord.
        /// </summary>
        /// <param name="step"></param>
        /// <param name="octave"></param>
        /// <param name="velocity"></param>
        /// <param name="chordType"></param>
        public MidiChord(ChromaticStep step, int octave,  int velocity, ChordType chordType)
        {     
            byte midicode = (byte)(12 * (octave + 1) + (byte)step);
            switch (chordType)
            {
                case ChordType.Major: 
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Unison));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.MajorThird));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Fifth));
                    break;
                case ChordType.Minor:
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Unison));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.MinorThird));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Fifth));
                    break;
                case ChordType.Major6:
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Unison));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.MajorThird));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.MajorSixth));
                    break;
                case ChordType.Minor6:
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Unison));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.MinorThird));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.MajorSixth));
                    break;
                case ChordType.Major7:
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Unison));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.MajorThird));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Fifth));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.MinorSeventh));
                    break;
                case ChordType.Minor7:
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Unison));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.MinorThird));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Fifth));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.MinorSeventh));
                    break;
                case ChordType.Major7maj:
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Unison));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.MajorThird));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Fifth));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.MajorSeventh));
                    break;
                case ChordType.Minor7maj:
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Unison));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.MinorThird));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Fifth));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.MajorSeventh));
                    break;

                case ChordType.Major9:
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Unison));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.MajorThird));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Fifth));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Ninth));
                    break;
                case ChordType.Minor9:
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Unison));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.MinorThird));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Fifth));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Ninth));
                    break;

                //
                case ChordType.Major11:
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Unison));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.MajorThird));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Fifth));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Eleventh));
                    break;
                case ChordType.Minor11:
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Unison));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.MinorThird));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Fifth));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Eleventh));
                    break;
                //
                case ChordType.Major13:
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Unison));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.MajorThird));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Fifth));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Thirteenth));
                    break;
                case ChordType.Minor13:
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Unison));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.MinorThird));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Fifth));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Thirteenth));
                    break;



                case ChordType.Dim:
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Unison));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.MinorThird));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Fourth));
                    break;
                case ChordType.Dim7:
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Unison));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.MinorThird));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Fourth));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.MinorSeventh));
                    break;
                case ChordType.Sus4:
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Unison));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Fourth));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.MinorSeventh));
                    break;
                default:
                    throw new System.ArgumentException(string.Format("Chordtype {0} is not supported", chordType.ToString()));  
            }
        }

        /// <summary>
        /// Starts playing the chord on the device specified
        /// </summary>
        /// <param name="midiOut"></param>
        public void StartPlaying(MidiOut midiOut)
        {
            foreach (MidiNote midiNote in midinotes)
            {
                midiNote.StartPlaying(midiOut);
            }
        }

        /// <summary>
        /// Starts playing the chord on the device specified
        /// </summary>
        /// <param name="midiOut"></param>
        public void StopPlaying(MidiOut midiOut)
        {
            foreach (MidiNote midiNote in midinotes)
            {
                midiNote.StopPlaying(midiOut);
            }
        }
    }
}
