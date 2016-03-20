using System.Collections.Generic;
using NAudio.Midi;

namespace JSJ.MusicSynthesis
{

    public enum ChordType {
        // Triads
        Major, Minor, Augmented, Dim,
        // Sixths
        Major6, Minor6,
        // Sevenths
        Dom7, Major7, Minor7, Aug7, FullDim7, HalfDim7, Major7maj, MinorMajor,
        // Ninths
        Dom9, Major9, Minor9,
        // Elevenths
        Dom11, Major11, Minor11,
        // Thirteenths
        Dom13, Major13, Minor13,
        // Suspended
        Sus2, Sus4,
        // Unimplemented
        UnImplemented };


    public class MidiChord
    {
        // http://www.musicxml.com/UserManuals/MusicXML/Content/ST-MusicXML-kind-value.htm


        private List<MidiNote> midinotes = new List<MidiNote>();

        /// <summary>
        /// Maps from the text found in the MusicXml file to an enum identifying the chord
        /// </summary>
        /// <param name="chordString"></param>
        /// <returns></returns>
        public static ChordType GetChordType(string chordString)
        {
            switch (chordString)
            {
                // Triads:
                case "major": return ChordType.Major; // (major third, perfect fifth)
                case "minor": return ChordType.Minor; // (minor third, perfect fifth)
                case "augmented": return ChordType.Augmented; // (major third, augmented fifth)
                case "diminished":; return ChordType.Dim; // (minor third, diminished fifth)

                // Sevenths:
                case "dominant": return ChordType.Dom7; // (major triad, minor seventh) (0,4,7,10)
                case "major-seventh": return ChordType.Major7maj; // (major triad, major seventh)
                case "minor-seventh": return ChordType.Minor7; // (minor triad, minor seventh)
                case "diminished-seventh": return ChordType.FullDim7; //(diminished triad, diminished seventh)  (0,3,6,9)
                case "augmented-seventh":  return ChordType.Aug7;//(augmented triad, minor seventh) (0,4,8,10)
                case "half-diminished":; return ChordType.HalfDim7; // (diminished triad, minor seventh) (0,3,6,10)
                case "major-minor": return ChordType.MinorMajor;//(minor triad, major seventh) //  (minor triad, major seventh) (0,3,7,11)

                // Sixths
                case "major-sixth": return ChordType.Major6; // (major triad, added sixth)
                case "minor-sixth": return ChordType.Minor6; // (minor triad, added sixth)

                // Ninths
                case "dominant-ninth": return ChordType.Dom9; //  (dominant-seventh, major ninth)
                case "major-ninth": return ChordType.Major9; // (major-seventh, major ninth)
                case "minor-ninth": return ChordType.Minor9; // (minor-seventh, major ninth)

                // 11ths (usually as the basis for alteration)
                case "dominant-11th": return ChordType.Dom11; // (dominant - ninth, perfect 11th)
                case "major-11th": return ChordType.Major11; // (major - ninth, perfect 11th)
                case "minor-11th": return ChordType.Minor11; // (minor-ninth, perfect 11th)


                // 13ths(usually as the basis for alteration):
                case "dominant-13th": return ChordType.Dom13; //(dominant-11th, major 13th)
                case "major-13th": return ChordType.Major13; // (major-11th, major 13th)
                case "minor-13th": return ChordType.Minor13; // (minor-11th, major 13th)

                // Suspended
                case "suspended-second": return ChordType.Sus2; // (major second, perfect fifth) (0,2,7)
                case "suspended-fourth": return ChordType.Sus4; // (perfect fourth, perfect fifth) (0,4,7)

                default: return ChordType.UnImplemented;
            }
        }

        /// <summary>
        /// Maps from an enum identifying a chord to the intervals representing the chord
        /// </summary>
        /// <param name="chordType"></param>
        /// <returns></returns>
        public static List<Interval> GetChordIntervals(ChordType chordType)
        {
            return null; // TO DO: Implement!
        }


        public static string LocalizeChordType(ChordType chordType)
        {
            switch (chordType)
            {
                // Minor
                case ChordType.Minor: return "m";
                case ChordType.Minor6: return "m6";
                case ChordType.Minor7: return "m7";
                case ChordType.MinorMajor: return "molmaj";
                case ChordType.Minor9: return "m9";
                case ChordType.Minor11: return "m11";
                case ChordType.Minor13: return "m13";


                // Major
                case ChordType.Major: return ""; // Implicitly "dur"
                case ChordType.Major6: return "6";
                case ChordType.Dom7:   return "dom";
                case ChordType.Major7: return "7";
                case ChordType.Major7maj: return "maj7";
                case ChordType.Major9: return "9";
                case ChordType.Major11: return "11";
                case ChordType.Major13: return "13";

                // Diminished
                case ChordType.Dim: return "ø";
                case ChordType.HalfDim7: return "ø7"; // ??
                case ChordType.FullDim7: return "ø7"; // ??

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
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.PerfectFifth));
                    break;
                case ChordType.Minor:
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Unison));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.MinorThird));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.PerfectFifth));
                    break;
                case ChordType.Augmented:
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Unison));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.MinorThird));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.AugmentedFifth));
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
                case ChordType.Dom7:
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Unison));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.MajorThird));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.PerfectFifth));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.MinorSeventh));
                    break;
                case ChordType.Major7:
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Unison));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.MajorThird));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.PerfectFifth));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.MajorSeventh));
                    break;
                case ChordType.Minor7:
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Unison));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.MinorThird));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.PerfectFifth));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.MinorSeventh));
                    break;
                case ChordType.Major7maj:
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Unison));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.MajorThird));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.PerfectFifth));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.MajorSeventh));
                    break;
                case ChordType.MinorMajor:
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Unison));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.MinorThird));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.PerfectFifth));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.MajorSeventh));
                    break;
                case ChordType.Aug7:
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Unison));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.MinorThird));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.AugmentedFifth));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.MinorSeventh));
                    break;
                case ChordType.Dom9:
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Unison));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.MajorThird));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.PerfectFifth));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.MinorSeventh));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Ninth));
                    break;
                case ChordType.Major9:
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Unison));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.MajorThird));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.PerfectFifth));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Ninth));
                    break;
                case ChordType.Minor9:
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Unison));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.MinorThird));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.PerfectFifth));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Ninth));
                    break;
                case ChordType.Dom11:
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Unison));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.MajorThird));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.PerfectFifth));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.MinorSeventh));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Ninth));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Eleventh));
                    break;
                case ChordType.Major11:
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Unison));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.MajorThird));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.PerfectFifth));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Eleventh));
                    break;
                case ChordType.Minor11:
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Unison));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.MinorThird));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.PerfectFifth));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Eleventh));
                    break;
                case ChordType.Dom13:
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Unison));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.MajorThird));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.PerfectFifth));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.MinorSeventh));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Ninth));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Eleventh));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Thirteenth));
                    break;
                case ChordType.Major13:
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Unison));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.MajorThird));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.PerfectFifth));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Thirteenth));
                    break;
                case ChordType.Minor13:
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Unison));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.MinorThird));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.PerfectFifth));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Thirteenth));
                    break;
                case ChordType.Dim:
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Unison));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.MinorThird));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Fourth));
                    break;
                case ChordType.HalfDim7:
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Unison));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.MinorThird));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Fourth));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.MinorSeventh));
                    break;
                case ChordType.FullDim7:
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Unison));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.MinorThird));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Fourth));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.DiminishedSeventh));
                    break;
                case ChordType.Sus2:
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.Unison));
                    midinotes.Add(new MidiNote(step, octave, velocity, Interval.MajorSecond));
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
