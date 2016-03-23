using System.Collections.Generic;
using NAudio.Midi;

namespace JSJ.MusicSynthesis
{

    public enum ChordType {
        // Triads
        Major, Minor, Aug, Dim, Major6, Minor6,                     // 6 triads
        // Sevenths
        Dom7, Major7, Minor7, Aug7, FullDim7, HalfDim7, MinorMajor, // 7 sevenths
        // Ninths
        Dom9, Major9, Minor9,                                       // 3 ninths
        // Elevenths
        Dom11, Major11, Minor11,                                    // 3 elevenths
        // Thirteenths
        Dom13, Major13, Minor13,                                    // 3 thirteenths
        // Suspended
        Sus2, Sus4,                                                 // 2 sus
        // Unimplemented
        UnImplemented };


    public class MidiChord
    {
        // The following static arrays describe the intervals found in the varions chords as described in
        // http://www.musicxml.com/UserManuals/MusicXML/Content/ST-MusicXML-kind-value.htm 
        // Triads:  
        static readonly Interval[] MajorChordIntervals  = new Interval[] { Interval.Unison, Interval.MajorThird, Interval.PerfectFifth };
        static readonly Interval[] MinorChordIntervals  = new Interval[] { Interval.Unison, Interval.MinorThird, Interval.PerfectFifth };
        static readonly Interval[] DimChordIntervals    = new Interval[] { Interval.Unison, Interval.MinorThird, Interval.Fourth };
        static readonly Interval[] AugChordIntervals    = new Interval[] { Interval.Unison, Interval.MinorThird, Interval.AugmentedFifth };
        static readonly Interval[] Major6ChordIntervals = new Interval[] { Interval.Unison, Interval.MajorThird, Interval.MajorSixth };
        static readonly Interval[] Minor6ChordIntervals = new Interval[] { Interval.Unison, Interval.MinorThird, Interval.MajorSixth };
        // Sevenths:
        static readonly Interval[] Dom7ChordIntervals   = new Interval[] { Interval.Unison, Interval.MajorThird, Interval.PerfectFifth, Interval.MinorSeventh };
        static readonly Interval[] Major7ChordIntervals = new Interval[] { Interval.Unison, Interval.MajorThird, Interval.PerfectFifth, Interval.MajorSeventh };
        static readonly Interval[] Minor7ChordIntervals = new Interval[] { Interval.Unison, Interval.MajorThird, Interval.PerfectFifth, Interval.MinorSeventh };
        static readonly Interval[] MinorMajorChordIntervals = new Interval[] { Interval.Unison, Interval.MinorThird, Interval.PerfectFifth, Interval.MajorSeventh };
        static readonly Interval[] Aug7ChordIntervals    = new Interval[] { Interval.Unison, Interval.MinorThird, Interval.AugmentedFifth, Interval.MinorSeventh };
        static readonly Interval[] HalfDim7ChordIntervals= new Interval[] { Interval.Unison, Interval.MinorThird, Interval.Fourth, Interval.MinorSeventh };
        static readonly Interval[] FullDim7ChordIntervals= new Interval[] { Interval.Unison, Interval.MinorThird, Interval.Fourth, Interval.DiminishedSeventh };
        // Ninths:
        static readonly Interval[] Dom9ChordIntervals    = new Interval[] { Interval.Unison, Interval.MajorThird, Interval.PerfectFifth, Interval.MinorSeventh, Interval.Ninth };
        static readonly Interval[] Major9ChordIntervals  = new Interval[] { Interval.Unison, Interval.MajorThird, Interval.PerfectFifth, Interval.Ninth };
        static readonly Interval[] Minor9ChordIntervals  = new Interval[] { Interval.Unison, Interval.MinorThird, Interval.PerfectFifth, Interval.Ninth };
        // Elevenths:
        static readonly Interval[] Dom11ChordIntervals   = new Interval[] { Interval.Unison, Interval.MajorThird, Interval.PerfectFifth, Interval.MinorSeventh, Interval.Ninth,Interval.Eleventh };
        static readonly Interval[] Major11ChordIntervals = new Interval[] { Interval.Unison, Interval.MajorThird, Interval.PerfectFifth, Interval.Eleventh };
        static readonly Interval[] Minor11ChordIntervals = new Interval[] { Interval.Unison, Interval.MinorThird, Interval.PerfectFifth, Interval.Eleventh };
        // Thirteenths:
        static readonly Interval[] Dom13ChordIntervals   = new Interval[] { Interval.Unison, Interval.MajorThird, Interval.PerfectFifth, Interval.MinorSeventh, Interval.Ninth, Interval.Eleventh, Interval.Thirteenth };
        static readonly Interval[] Major13ChordIntervals = new Interval[] { Interval.Unison, Interval.MajorThird, Interval.PerfectFifth, Interval.Thirteenth };
        static readonly Interval[] Minor13ChordIntervals = new Interval[] { Interval.Unison, Interval.MinorThird, Interval.PerfectFifth, Interval.Thirteenth};
        // Sustained 
        static readonly Interval[] Sus2ChordIntervals    = new Interval[] { Interval.Unison, Interval.MajorSecond, Interval.MajorSeventh };
        static readonly Interval[] Sus4ChordIntervals    = new Interval[] { Interval.Unison, Interval.Fourth, Interval.MinorSeventh };

        private List<MidiNote> midinotes = new List<MidiNote>();

        /// <summary>
        /// Maps from the text found in the MusicXml file to an enum identifying the chord
        /// The first coloumn in the comment is the MusicXml definition of the chord.
        /// The URL is the corresponding Wikipedia article.
        /// The {a,b,c} notation is the integer notation, if found in Wikipedia.
        /// In case of doubt it is probably best to use the MusicXml definition !!!
        /// </summary>
        /// <param name="chordString"></param>
        /// <returns></returns>
        public static ChordType GetChordType(string chordString)
        {
            switch (chordString)
            {
                // Triads:
                case "major": return ChordType.Major;                   // (major third, perfect fifth)     https://en.wikipedia.org/wiki/Major_chord                       {0,4,7}
                case "minor": return ChordType.Minor;                   // (minor third, perfect fifth)     https://en.wikipedia.org/wiki/Minor_chord                       {0,3,7}
                case "augmented": return ChordType.Aug;                 // (major third, augmented fifth)   https://en.wikipedia.org/wiki/Augmented_triad                   {0,4,8}
                case "diminished":; return ChordType.Dim;               // (minor third, diminished fifth)  https://en.wikipedia.org/wiki/Diminished_triad                  {0,3,6}

                // Sevenths:
                case "dominant": return ChordType.Dom7;                 // (major triad, minor seventh)     https://en.wikipedia.org/wiki/Dominant_seventh_chord            {0,4,7,10}
                case "major-seventh": return ChordType.Major7;          // (major triad, major seventh)     https://en.wikipedia.org/wiki/Major_seventh_chord               {0,4,7,11}
                case "minor-seventh": return ChordType.Minor7;          // (minor triad, minor seventh)     https://en.wikipedia.org/wiki/Minor_seventh_chord               {0,3,7,10}
                case "diminished-seventh": return ChordType.FullDim7;   // (diminished triad, diminished seventh) https://en.wikipedia.org/wiki/Diminished_seventh_chord    {0,3,6,9}
                case "augmented-seventh":  return ChordType.Aug7;       // (augmented triad, minor seventh) JSJ: Do they mean {0,4,8,10} or {0,4,8,11} (see below!!)
                                                                        // (augmentedminor seventh:         https://en.wikipedia.org/wiki/Augmented_seventh_chord           {0,4,8,10}  
                                                                        // (augmentedmajor seventh:         https://en.wikipedia.org/wiki/Augmented_major_seventh_chord     {0,4,8,11}
                case "half-diminished":; return ChordType.HalfDim7;     // (diminished triad, minor seventh) https://en.wikipedia.org/wiki/Half-diminished_seventh_chord    {0,3,6,10}
                case "minor-major": return ChordType.MinorMajor;        // (minor triad, major seventh)     https://en.wikipedia.org/wiki/Minor_major_seventh_chord         {0,3,7,11}

                // Sixths
                case "major-sixth": return ChordType.Major6;            // (major triad, added sixth)       https://en.wikipedia.org/wiki/Major_6th_chord  
                case "minor-sixth": return ChordType.Minor6;            // (minor triad, added sixth)       Not found in Wikipedia

                // Ninths
                case "dominant-ninth": return ChordType.Dom9;           // (dominant-seventh, major ninth)  https://en.wikipedia.org/wiki/Ninth_chord#Dominant_ninth 
                case "major-ninth": return ChordType.Major9;            // (major-seventh, major ninth)     https://en.wikipedia.org/wiki/Ninth_chord#Major_ninth      
                case "minor-ninth": return ChordType.Minor9;            // (minor-seventh, major ninth)     https://en.wikipedia.org/wiki/Ninth_chord#Minor_ninth      

                // 11ths (usually as the basis for alteration)
                case "dominant-11th": return ChordType.Dom11;           // (dominant - ninth, perfect 11th) https://en.wikipedia.org/wiki/Eleventh_chord
                case "major-11th": return ChordType.Major11;            // (major - ninth, perfect 11th)    https://en.wikipedia.org/wiki/Eleventh_chord
                case "minor-11th": return ChordType.Minor11;            // (minor-ninth, perfect 11th)      https://en.wikipedia.org/wiki/Eleventh_chord


                // 13ths(usually as the basis for alteration):
                case "dominant-13th": return ChordType.Dom13;           // (dominant-11th, major 13th)     https://en.wikipedia.org/wiki/Thirteenth
                case "major-13th": return ChordType.Major13;            // (major-11th, major 13th)        https://en.wikipedia.org/wiki/Thirteenth
                case "minor-13th": return ChordType.Minor13;            // (minor-11th, major 13th)        https://en.wikipedia.org/wiki/Thirteenth

                // Suspended
                case "suspended-second": return ChordType.Sus2;         // (major second, perfect fifth)    https://en.wikipedia.org/wiki/Suspended_chord {0,2,7}
                case "suspended-fourth": return ChordType.Sus4;         // (perfect fourth, perfect fifth)  https://en.wikipedia.org/wiki/Suspended_chord {0,5,7}

                default: return ChordType.UnImplemented;
            }
        }

        /// <summary>
        /// Maps from an enum identifying a chord to the intervals representing the chord
        /// </summary>
        /// <param name="chordType"></param>
        /// <returns></returns>
        public static Interval[] GetChordIntervals(ChordType chordType)
        {
            switch (chordType)
            {
                // Triads:
                case ChordType.Major: return MajorChordIntervals;
                case ChordType.Minor: return MinorChordIntervals;
                case ChordType.Dim: return DimChordIntervals;
                case ChordType.Aug: return AugChordIntervals;
                case ChordType.Major6: return Major6ChordIntervals;
                case ChordType.Minor6: return Minor6ChordIntervals;
                // Sevenths:
                case ChordType.Dom7: return Dom7ChordIntervals;
                case ChordType.Major7: return Major7ChordIntervals;
                case ChordType.Minor7: return Minor7ChordIntervals;
                case ChordType.MinorMajor: return MinorMajorChordIntervals;
                case ChordType.Aug7: return Aug7ChordIntervals;
                case ChordType.HalfDim7: return HalfDim7ChordIntervals;
                case ChordType.FullDim7: return FullDim7ChordIntervals;
                // Ninths
                case ChordType.Dom9: return Dom9ChordIntervals;
                case ChordType.Major9: return Major9ChordIntervals;
                case ChordType.Minor9: return Minor9ChordIntervals;
                // Elevenths
                case ChordType.Dom11: return Dom11ChordIntervals;
                case ChordType.Major11: return Major11ChordIntervals;
                case ChordType.Minor11: return Minor11ChordIntervals;
                // Thirteenths:
                case ChordType.Dom13: return Dom13ChordIntervals;
                case ChordType.Major13: return Major13ChordIntervals;
                case ChordType.Minor13: return Minor13ChordIntervals;
                // Sustained:
                case ChordType.Sus2: return Sus2ChordIntervals;
                case ChordType.Sus4: return Sus4ChordIntervals;
                // Undefined
                default:
                    throw new System.ArgumentException(string.Format("Chordtype {0} is not supported", chordType.ToString()));
            }
    
        }


        public static string LocalizeChordType(ChordType chordType)
        {
            switch (chordType)
            {

                // Triads

                case ChordType.Major:   return "dur"; // Or just ""
                case ChordType.Minor:   return "mol";
                case ChordType.Dim:     return "dim";
                case ChordType.Aug:     return "aug";
                case ChordType.Major6:  return "dur6";
                case ChordType.Minor6:  return "mol6";

                // Sevenths:
                case ChordType.Dom7:    return "dom7";
                case ChordType.Major7:  return "dur7";
                case ChordType.Minor7:  return "mol7";
                case ChordType.MinorMajor: return "molmaj";
                case ChordType.Aug7:    return "aug7";
                case ChordType.HalfDim7:return "ø7"; // ??
                case ChordType.FullDim7:return "dim7"; // ??

                // Ninths
                case ChordType.Dom9:    return "dom9";
                case ChordType.Major9:  return "dur9"; // Or just "9"
                case ChordType.Minor9:  return "mol9";

                // Elevenths
                case ChordType.Dom11:   return "dom11";
                case ChordType.Major11: return "dur11"; // Or just "11"
                case ChordType.Minor11: return "mol11";

                // Thirteenths
                case ChordType.Dom13:   return "dom13";
                case ChordType.Major13: return "dur13"; // Or just "13"
                case ChordType.Minor13: return "mol13";

                // Suspended
                case ChordType.Sus2:    return "sus2";
                case ChordType.Sus4:    return "sus4";

                default: return "Ikke implementeret";
            }
        }


        //private void AddChord(ChromaticStep step, int octave, int velocity, Interval[] intervals)
        //{

        //    foreach (Interval interval in intervals)
        //    {
        //        midinotes.Add(new MidiNote(step,octave,velocity,interval));
        //    }
        //}


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
            Interval[] intervals = GetChordIntervals(chordType);
            foreach (Interval interval in intervals)
            {
                midinotes.Add(new MidiNote(step, octave, velocity, interval));
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
