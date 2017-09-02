using System.Collections.Generic;
using NAudio.Midi;
using MusicSynthesis;

namespace JSJ.MusicSynthesis
{

    public enum ChordType
    {
        // Triads
        Major, Minor, Aug, Dim, Major6, Minor6,                     // 6 triads
        // Sevenths
        Dom7, Major7, Minor7, Aug7, FullDim7, HalfDim7, MajorMinor, // 7 sevenths
        // Ninths
        Dom9, Major9, Minor9,                                       // 3 ninths
        // Elevenths
        Dom11, Major11, Minor11,                                    // 3 elevenths
        // Thirteenths
        Dom13, Major13, Minor13,                                    // 3 thirteenths
        // Suspended
        Sus2, Sus4,                                                 // 2 sus
        // 
        UnImplemented,                                              // Mentioned in the MusicXml definition, but not implemented here        
        Unknown                                                     // Not mentioned in the MusicXml definition, 
    };


    public class MidiChord
    {
        private string className = "MidiChord";

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
        static readonly Interval[] MajorMinorChordIntervals = new Interval[] { Interval.Unison, Interval.MinorThird, Interval.PerfectFifth, Interval.MajorSeventh };
        static readonly Interval[] Aug7ChordIntervals    = new Interval[] { Interval.Unison, Interval.MinorThird, Interval.AugmentedFifth, Interval.MinorSeventh };
        static readonly Interval[] HalfDim7ChordIntervals= new Interval[] { Interval.Unison, Interval.MinorThird, Interval.Tritone, Interval.MinorSeventh };
        static readonly Interval[] FullDim7ChordIntervals= new Interval[] { Interval.Unison, Interval.MinorThird, Interval.Tritone, Interval.DiminishedSeventh };
        // Ninths:
        static readonly Interval[] Dom9ChordIntervals    = new Interval[] { Interval.Unison, Interval.MajorThird, Interval.PerfectFifth, Interval.MinorSeventh, Interval.Ninth };
        static readonly Interval[] Major9ChordIntervals  = new Interval[] { Interval.Unison, Interval.MajorThird, Interval.PerfectFifth, Interval.MajorSeventh, Interval.Ninth };
        static readonly Interval[] Minor9ChordIntervals  = new Interval[] { Interval.Unison, Interval.MinorThird, Interval.PerfectFifth, Interval.MinorSeventh, Interval.Ninth };
        // Elevenths:
        static readonly Interval[] Dom11ChordIntervals   = new Interval[] { Interval.Unison, Interval.MajorThird, Interval.PerfectFifth, Interval.MinorSeventh, Interval.Ninth, Interval.Eleventh };
        static readonly Interval[] Major11ChordIntervals = new Interval[] { Interval.Unison, Interval.MajorThird, Interval.PerfectFifth, Interval.MajorSeventh, Interval.Ninth, Interval.Eleventh };
        static readonly Interval[] Minor11ChordIntervals = new Interval[] { Interval.Unison, Interval.MinorThird, Interval.PerfectFifth, Interval.MinorSeventh, Interval.Ninth, Interval.Eleventh };
        // Thirteenths:
        static readonly Interval[] Dom13ChordIntervals   = new Interval[] { Interval.Unison, Interval.MajorThird, Interval.PerfectFifth, Interval.MinorSeventh, Interval.Ninth, Interval.Eleventh, Interval.Thirteenth };
        static readonly Interval[] Major13ChordIntervals = new Interval[] { Interval.Unison, Interval.MajorThird, Interval.PerfectFifth, Interval.MajorSeventh, Interval.Ninth, Interval.Eleventh, Interval.Thirteenth };
        static readonly Interval[] Minor13ChordIntervals = new Interval[] { Interval.Unison, Interval.MinorThird, Interval.PerfectFifth, Interval.MinorSeventh, Interval.Ninth, Interval.Eleventh, Interval.Thirteenth};
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
                case "major-minor": return ChordType.MajorMinor;        // (minor triad, major seventh)     https://en.wikipedia.org/wiki/Minor_major_seventh_chord         {0,3,7,11}

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

                // Other chords, mentioned in http://usermanuals.musicxml.com/MusicXML/Content/ST-MusicXML-kind-value.htm but not implemented here

                // Functional sixths
                case "Neapolitan ":     // https://en.wikipedia.org/wiki/Neapolitan_chord
                case "Italian":         // https://en.wikipedia.org/wiki/Augmented_sixth_chord#Italian_sixth
                case "French":          // https://en.wikipedia.org/wiki/Augmented_sixth_chord#Italian_sixth
                case "German":          // https://en.wikipedia.org/wiki/Augmented_sixth_chord#Italian_sixth

                // Other kinds
                case "pedal":
                case "power":
                case "Tristan":
                case "other": // The "other" kind is used when the harmony is entirely composed of add elements.
                case "none":  // The "none"  kind is used to explicitly encode absence of chords or functional 
                    return ChordType.UnImplemented;

                //  Chords, NOT mentioned in http://usermanuals.musicxml.com/MusicXML/Content/ST-MusicXML-kind-value.htm
                default: return ChordType.Unknown;
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
                case ChordType.MajorMinor: return MajorMinorChordIntervals;
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


        public static string LocalizeChordType(ChordType chordType) // TODO Localize !!
        {
            switch (chordType)
            {

                // Triads

                case ChordType.Major:   return ResourcesForMusicSynthesis.ChordKind_Major;  // ""
                case ChordType.Minor:   return ResourcesForMusicSynthesis.ChordKind_Minor;  // "m"
                case ChordType.Dim:     return ResourcesForMusicSynthesis.ChordKind_Dim;    // "dim"
                case ChordType.Aug:     return ResourcesForMusicSynthesis.ChordKind_Aug;    // "aug"
                case ChordType.Major6:  return ResourcesForMusicSynthesis.ChordKind_Major6; // "6"
                case ChordType.Minor6:  return ResourcesForMusicSynthesis.ChordKind_Minor6; // "m6"

                // Sevenths:
                case ChordType.Dom7:    return ResourcesForMusicSynthesis.ChordKind_Dom7;   // "7"      // A  major with an added 7
                case ChordType.Major7:  return ResourcesForMusicSynthesis.ChordKind_Major7; // "maj7"   // A major with an added major 7
                case ChordType.Minor7:  return ResourcesForMusicSynthesis.ChordKind_Minor7; // "m7"     // A minor with an added 7
                case ChordType.MajorMinor: return ResourcesForMusicSynthesis.ChordKind_MajorMinor; // "molmaj"; // Aminor with an added major7
                case ChordType.Aug7:    return ResourcesForMusicSynthesis.ChordKind_Aug7;   // "aug7"
                case ChordType.HalfDim7:return ResourcesForMusicSynthesis.ChordKind_HalfDim7;// "m7b5"
                case ChordType.FullDim7:return ResourcesForMusicSynthesis.ChordKind_FullDim7;// "dim7"; // ??

                // Ninths
                case ChordType.Dom9:    return ResourcesForMusicSynthesis.ChordKind_Dom9;   // "9";     // Dom7 with a 9
                case ChordType.Major9:  return ResourcesForMusicSynthesis.ChordKind_Major9; // "maj9";  //  maj7 with a 9
                case ChordType.Minor9:  return ResourcesForMusicSynthesis.ChordKind_Minor9; // "m9";    //  m7 with a 9

                // Elevenths
                case ChordType.Dom11:   return ResourcesForMusicSynthesis.ChordKind_Dom11;   //  "11";
                case ChordType.Major11: return ResourcesForMusicSynthesis.ChordKind_Major11; // "maj11"; 
                case ChordType.Minor11: return ResourcesForMusicSynthesis.ChordKind_Minor11; //  "m11";

                // Thirteenths
                case ChordType.Dom13:   return ResourcesForMusicSynthesis.ChordKind_Dom13;   // "13";
                case ChordType.Major13: return ResourcesForMusicSynthesis.ChordKind_Major13; // "maj13"; 
                case ChordType.Minor13: return ResourcesForMusicSynthesis.ChordKind_Minor13; // "m13";

                // Suspended
                case ChordType.Sus2:    return ResourcesForMusicSynthesis.ChordKind_Sus2;   // "sus2";
                case ChordType.Sus4:    return ResourcesForMusicSynthesis.ChordKind_Sus4;   // "sus4";

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
        public MidiChord(ChromaticStep step, int octave,  int velocity, ChordType chordType, ChromaticStep bassStep, List<MidiChordDegreeDescription> degreeDescriptions, List<string> logLines)
        {
            string functionName = "MidiChord";
            Interval[] intervals = GetChordIntervals(chordType);
            foreach (Interval interval in intervals)
            {
                midinotes.Add(new MidiNote(step, octave, velocity, interval));
            }
            if (step != bassStep)
            {
                if (null != logLines)
                {
                    logLines.Add(string.Format("{0}.{1} Root={2} Bass={3} BassStep not implemented yet", className, functionName, step, bassStep));
                }
            }
            if (null != degreeDescriptions)
            {
                 foreach (MidiChordDegreeDescription degreeDescription in degreeDescriptions)
                {
                    switch(degreeDescription.DegreeType)
                    {
                        case DegreeTypeEnum.none: break;

                        case DegreeTypeEnum.add:
                        case DegreeTypeEnum.alter:
                        case DegreeTypeEnum.subtract:
                        case DegreeTypeEnum.unknown:
                            if (null != logLines)
                            {
                                logLines.Add(string.Format("{0}.{1} Type={2} Value={3} Degree not implemented yet", className, functionName, degreeDescription.DegreeType.ToString(), degreeDescription.Degree.ToString()));
                            }
                            break;
                        default: 
                            if (null != logLines)
                            {
                                logLines.Add(string.Format("{0}.{1} Unimplemented DegreeType={2}", className, functionName, degreeDescription.DegreeType.ToString()));
                            }
                            break;

                    }
                }

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
