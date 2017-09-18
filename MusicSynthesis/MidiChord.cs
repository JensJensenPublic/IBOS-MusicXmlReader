using System.Collections.Generic;
using NAudio.Midi;
using MusicSynthesis;
using System.Text;

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
        static readonly Interval[] AugChordIntervals    = new Interval[] { Interval.Unison, Interval.MinorThird, Interval.MinorSixth };
        static readonly Interval[] Major6ChordIntervals = new Interval[] { Interval.Unison, Interval.MajorThird, Interval.MajorSixth };
        static readonly Interval[] Minor6ChordIntervals = new Interval[] { Interval.Unison, Interval.MinorThird, Interval.MajorSixth };
        // Sevenths:
        static readonly Interval[] Dom7ChordIntervals   = new Interval[] { Interval.Unison, Interval.MajorThird, Interval.PerfectFifth, Interval.MinorSeventh };
        static readonly Interval[] Major7ChordIntervals = new Interval[] { Interval.Unison, Interval.MajorThird, Interval.PerfectFifth, Interval.MajorSeventh };
        static readonly Interval[] Minor7ChordIntervals = new Interval[] { Interval.Unison, Interval.MajorThird, Interval.PerfectFifth, Interval.MinorSeventh };
        static readonly Interval[] MajorMinorChordIntervals = new Interval[] { Interval.Unison, Interval.MinorThird, Interval.PerfectFifth, Interval.MajorSeventh };
        static readonly Interval[] Aug7ChordIntervals    = new Interval[] { Interval.Unison, Interval.MinorThird, Interval.MinorSixth, Interval.MinorSeventh };
        static readonly Interval[] HalfDim7ChordIntervals= new Interval[] { Interval.Unison, Interval.MinorThird, Interval.AugmentedFourth, Interval.MinorSeventh };
        static readonly Interval[] FullDim7ChordIntervals= new Interval[] { Interval.Unison, Interval.MinorThird, Interval.AugmentedFourth, Interval.MajorSixth };
        // Ninths:
        static readonly Interval[] Dom9ChordIntervals    = new Interval[] { Interval.Unison, Interval.MajorThird, Interval.PerfectFifth, Interval.MinorSeventh, Interval.MajorNinth };
        static readonly Interval[] Major9ChordIntervals  = new Interval[] { Interval.Unison, Interval.MajorThird, Interval.PerfectFifth, Interval.MajorSeventh, Interval.MajorNinth };
        static readonly Interval[] Minor9ChordIntervals  = new Interval[] { Interval.Unison, Interval.MinorThird, Interval.PerfectFifth, Interval.MinorSeventh, Interval.MajorNinth };
        // Elevenths:
        static readonly Interval[] Dom11ChordIntervals   = new Interval[] { Interval.Unison, Interval.MajorThird, Interval.PerfectFifth, Interval.MinorSeventh, Interval.MajorNinth, Interval.Eleventh };
        static readonly Interval[] Major11ChordIntervals = new Interval[] { Interval.Unison, Interval.MajorThird, Interval.PerfectFifth, Interval.MajorSeventh, Interval.MajorNinth, Interval.Eleventh };
        static readonly Interval[] Minor11ChordIntervals = new Interval[] { Interval.Unison, Interval.MinorThird, Interval.PerfectFifth, Interval.MinorSeventh, Interval.MajorNinth, Interval.Eleventh };
        // Thirteenths:
        static readonly Interval[] Dom13ChordIntervals   = new Interval[] { Interval.Unison, Interval.MajorThird, Interval.PerfectFifth, Interval.MinorSeventh, Interval.MajorNinth, Interval.Eleventh, Interval.MajorThirteenth };
        static readonly Interval[] Major13ChordIntervals = new Interval[] { Interval.Unison, Interval.MajorThird, Interval.PerfectFifth, Interval.MajorSeventh, Interval.MajorNinth, Interval.Eleventh, Interval.MajorThirteenth };
        static readonly Interval[] Minor13ChordIntervals = new Interval[] { Interval.Unison, Interval.MinorThird, Interval.PerfectFifth, Interval.MinorSeventh, Interval.MajorNinth, Interval.Eleventh, Interval.MajorThirteenth};
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

        public static string ToLocalizedChordFunction(Interval interval)
        {
            switch (interval)
            {
#warning To Do fix and add more localization !!
                case Interval.Unison: return ResourcesForMusicSynthesis.Chord_Function_Tonic; // Tonic / Grundtone
                case Interval.MinorSecond: return ResourcesForMusicSynthesis.Chord_Function_Minor_Second;
                case Interval.MajorSecond: return ResourcesForMusicSynthesis.Chord_Function_Major_Second;
                case Interval.MinorThird: return ResourcesForMusicSynthesis.Chord_Function_Minor_Third;
                case Interval.MajorThird: return ResourcesForMusicSynthesis.Chord_Function_Major_Third;
                case Interval.Fourth: return ResourcesForMusicSynthesis.Chord_Function_Fourth;
                case Interval.AugmentedFourth: return ResourcesForMusicSynthesis.Chord_Function_Tritone; //  Dimished Fifth / Formindsket kvint
                case Interval.PerfectFifth: return ResourcesForMusicSynthesis.Chord_Function_Perfect_Fifth;
                case Interval.MinorSixth: return ResourcesForMusicSynthesis.Chord_Function_Augmented_Fifth;
                // case Interval.MinorSixth: return ""; == Interval.AugmentedFifth
                case Interval.MajorSixth: return ResourcesForMusicSynthesis.Chord_Function_Major_Sixth;
                // case Interval.DiminishedSeventh: return ""; == Interval.MajorSixth
                case Interval.MinorSeventh: return ResourcesForMusicSynthesis.Chord_Function_Minor_Seventh;
                case Interval.MajorSeventh: return ResourcesForMusicSynthesis.Chord_Function_Major_Seventh;
                case Interval.Octave: return ResourcesForMusicSynthesis.Chord_Function_Octave;
                case Interval.MinorNinth: return ResourcesForMusicSynthesis.Chord_Function_Minor_Ninth;
                case Interval.MajorNinth: return ResourcesForMusicSynthesis.Chord_Function_Ninth;
                case Interval.Eleventh: return ResourcesForMusicSynthesis.Chord_Function_Eleventh;
                case Interval.MajorThirteenth: return ResourcesForMusicSynthesis.Chord_Function_Thirteenth;
                default: 
                    // Log ?                   
                    return "";
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
        /// Map from the diatonic domain to the cromatic domain
        /// </summary>
        /// <param name="degree"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        private bool GetChordInterval(int degree, out Interval interval)
        {
            Interval result = Interval.Unison; 
            bool implemented = true;
            switch (degree)
            {
                case 1:
                    result = Interval.Unison; break;
                case 2:
                    result = Interval.MajorSecond; break;
                case 3:
                    result = Interval.MajorThird; break;
                case 4:
                    result = Interval.Fourth; break;
                case 5:
                    result = Interval.PerfectFifth; break;
                case 6:
                    result = Interval.MajorSixth; break;  
                case 7:
                    result = Interval.MinorSeventh; break;
                case 8:
                    result = Interval.Octave; break;
                case 9:
                    result = Interval.MajorNinth; break;
                case 10: 
                    result = Interval.MajorTenth; break;
                case 11:
                    result = Interval.Eleventh; break;
                case 12:
                    result = Interval.Twelfth; break;
                case 13:
                    result = Interval.MajorThirteenth; break;
                case 14:
                    result = Interval.MajorFourteenth; break;
                case 15:
                    result = Interval.Fifteenth; break;   
                default:
                    implemented = false;
                    result = Interval.Unison; break; //  What else could we do here ??
            }

            interval = result;

            if (!implemented)
            {
#warning Find out what to do !
            }

            return implemented;
        }


        private string ToString(List<Interval> intervals)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Interval interval in intervals)
            {
                sb.Append(interval.ToString() + " ");
            }
            return sb.ToString();
        }

        private string ToString(List<MidiChordDegreeDescription> descriptions)
        {
            StringBuilder sb = new StringBuilder();
            foreach (MidiChordDegreeDescription description in descriptions)
            {
                sb.Append(description.ToString() + " ");
            }
            return sb.ToString();
        }


        private void ModifyIntervals(ref Interval[] intervals, List<MidiChordDegreeDescription> degreeDescriptions)
        {
            string functionName = "ModifyIntervals";
            // Convert to List<> while manipulating!
            List<Interval> intervalList = new List<Interval>(intervals);
            MusicSynthesisLogger.Log(string.Format("{0}.{1}.Entry: {2} {3}", className, functionName, ToString(intervalList),ToString(degreeDescriptions))); // Log at entry
            foreach (MidiChordDegreeDescription degreeDescription in degreeDescriptions)
            {
                Interval interval;
                bool implementedDegreeValue = (GetChordInterval(degreeDescription.Degree, out interval));
                if (!implementedDegreeValue)
                {
                    MusicSynthesisLogger.Log(string.Format("{0}.{1} Unimplemented Degree ={2}", className, functionName, degreeDescription.Degree.ToString()));
                }
                else          
                {
                    switch (degreeDescription.DegreeType)
                    {
                        case DegreeTypeEnum.none: break;
                        case DegreeTypeEnum.add:
                            intervalList.Add((Interval) ((int)interval + degreeDescription.Alter));
                            break;
                        case DegreeTypeEnum.alter:
                            intervalList.Remove(interval);
                            intervalList.Add((Interval)((int)interval) + degreeDescription.Alter);
                            break;
                        case DegreeTypeEnum.subtract:
                            intervalList.Remove(interval);
                            break;
                        case DegreeTypeEnum.unknown:
                            MusicSynthesisLogger.Log(string.Format("{0}.{1} Type={2} Value={3} Degree not implemented yet", className, functionName, degreeDescription.DegreeType.ToString(), degreeDescription.Degree.ToString())); 
                            break;
                        default:
                            MusicSynthesisLogger.Log(string.Format("{0}.{1} Unimplemented DegreeType={2}", className, functionName, degreeDescription.DegreeType.ToString()));
                            break;
                    }
                }
            }
            intervalList.Sort();
            MusicSynthesisLogger.Log(string.Format("{0}.{1}.Exit:  {2}", className, functionName, ToString(intervalList))); // Log at exit
            intervals = intervalList.ToArray();          
        }

        

        /// <summary>
        /// Constructor
        /// Does NOT start playing the chord.
        /// </summary>
        /// <param name="step"></param>
        /// <param name="octave"></param>
        /// <param name="velocity"></param>
        /// <param name="chordType"></param>
        public MidiChord(ChromaticStep step, int octave, int velocity, ChordType chordType, ChromaticStep bassStep, List<MidiChordDegreeDescription> degreeDescriptions)
        {
            // string functionName = "MidiChord";
            Interval[] intervals = GetChordIntervals(chordType);
            if ((null != degreeDescriptions) && ( 0 != degreeDescriptions.Count))
            {
                ModifyIntervals(ref intervals, degreeDescriptions);
            }
            foreach (Interval interval in intervals)
            {
                midinotes.Add(new MidiNote(step, octave, velocity, interval));
            }
            if (step != bassStep)
            {
                midinotes.Add(new MidiNote(bassStep, (octave - 1), velocity, Interval.Unison));
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
