using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXmlReaderModel
{
    // Ref.1: "Music Braille C0de 1997" BANA: "Braille Authority of North Americe"
    // https://www.loc.gov/nls/wp-content/uploads/2016/03/music_braille_code.pdf


    public class BrailleBuilderForIntervalNotation : BrailleBuilder
    {
        private bool logWellFormedNess = false; // Loggin option

        public static byte[] intervalPerfectUnison = { (dot2 + dot3 + dot5 + dot6), dot5, (dot3 + dot6), (dot2 + dot3 + dot5 + dot6) };
        public static byte[] intervalMinorSecond = { (dot1 + dot2 + dot6), (dot3 + dot4) }; // {Flat, Second } Check with Lars
        public static byte[] intervalMajorSecond =  { (dot3 + dot4) };
        public static byte[] intervalMinorThird = { (dot1 + dot2 + dot6), (dot3 + dot4 + dot6) }; // {Flat , Third}
        public static byte[] intervalMajorThird =      { (dot3 + dot4 + dot6) };
        public static byte[] intervalPerfectFourth = { (dot3 + dot4 + dot5 + dot6) };
        public static byte[] intervalTritone = { (dot1 + dot4 + dot6), (dot3 + dot4 + dot5 + dot6) }; // (Sharp, Fourth)
        public static byte[] intervalPerfectFifth = { (dot3 + dot5) };
        public static byte[] intervalMinorSixth = { (dot1 + dot2 + dot6), (dot3 +  dot5 +dot6) }; // (Flat, Sixth)
        public static byte[] intervalMajorSixth = { (dot3 + dot5 + dot6) };
        public static byte[] intervalMinorSeventh = { (dot1 + dot2 + dot6), (dot2 + dot4) }; // (Flat, Seventh)
        public static byte[] intervalMajorSeventh = { (dot2 + dot4) };
        public static byte[] intervalPerfectOctave = { (dot3 + dot6) };
        public static byte[] intervalUnknown = { };

        // New implementation
//        public static byte[] intervalUnison = { (dot2 + dot3 + dot5 + dot6), dot5, (dot3 + dot6), (dot2 + dot3 + dot5 + dot6) }; // Error reported by Lars Petersen
        public static byte[] intervalUnison = { (dot3 + dot6)};
        public static byte[] intervalSecond = { (dot3 + dot4) };
        public static byte[] intervalThird = { (dot3 + dot4 + dot6) };
        public static byte[] intervalFourth = { (dot3 + dot4 + dot5 + dot6) };
        public static byte[] intervalFifth = { (dot3 + dot5) };
        public static byte[] intervalSixth = { (dot3 + dot5 + dot6) };
        public static byte[] intervalSeventh = { (dot2 + dot5) };
        public static byte[] intervalOctave = { (dot3 + dot6) }; 



        // New code for handling Interval otation


        private int Ascending(NoteElement x, NoteElement y)
        {
            return x.PitchValue.SemiTonesAboveC0 - y.PitchValue.SemiTonesAboveC0;
        }

        private int Descending(NoteElement x, NoteElement y)
        {
            return y.PitchValue.SemiTonesAboveC0 - x.PitchValue.SemiTonesAboveC0;
        }


        private bool IsWellformedIntervalRepresentation(List<NoteElement> nE, bool strictRules)
        {
            if (nE.Count < 2) return false;
            if (null == nE[0].PitchValue) return false;
            for (int i = 1; (i < nE.Count); i++)
            {
                if (strictRules && (nE[i].PartNumber != nE[0].PartNumber)) return false;
                if (strictRules && (nE[i].Staff != nE[0].Staff)) return false;
                if (nE[i].PitchValue == null) return false;
                if (nE[i].DurationInCommonDivisions != nE[0].DurationInCommonDivisions)
                {
                    if (logWellFormedNess)
                    {
                        string message = "";
#if false
                    string message = string.Format(": Different duration:  nE[{0}].DurationInCommonDivisions={1}  nE[{2}].DurationInCommonDivisions={3} Interval-representation can not be used!",
                        i, nE[i].DurationInCommonDivisions, 0, nE[0].DurationInCommonDivisions);
#else
                        if (verbose)
                        {
                            message = string.Format(": Different duration:  nE[{0}].DurationInCommonDivisions != nE[{1}].DurationInCommonDivisions. Part={2} Staff={3} Measure={4} Interval-representation can not be used!",
                                 i, 0, nE[0].PartId, nE[0].Staff, nE[0].MeasureNumber);
                        }
                        else
                        {
                            message = string.Format(": Different DurationInCommonDivisions:  nE[{0:D2}] != nE[{1}] Interval-representation can not be used!", i, 0);
                        }
#endif
                        Logger.LogCFOnce(message);
                    }
                    return false;
                }
            }
            return true;
        }

        private void Log(List<NoteElement> noteElements)
        {
            StringBuilder message = new StringBuilder();
            message.AppendFormat(string.Format("\r\n{0}", noteElements[0].ToDetailsString()));
            for (int i = 1; i < noteElements.Count; i++)
            {
                message.AppendFormat("\r\n   {0}", noteElements[i].ToDetailsString());
            }
            Logger.LogCF(string.Format(": PartId={0} Staff={1} {2} ", noteElements[0].PartId, noteElements[0].Staff, message.ToString()));
        }

#if false
        /// <summary>
        /// Original implementation
        /// https://en.wikipedia.org/wiki/Interval_(music)
        /// https://www.loc.gov/nls/wp-content/uploads/2016/03/music_braille_code.pdf
        /// Manual of BrailleMusic Notation Chapter 8.4)
        /// "Intervals larger than the octave are expressed by the same series of signs
        /// preceded by an appropriate octave mark, the 9th corresponding to the 2nd, the
        /// 10th to the 3rd, and so on."
        /// </summary>
        /// <param name="noteElement"></param>
        /// <param name="originSemitones"></param>
        private void AddIntervalV0(NoteElement noteElement, int originSemitones)
        {
            int semiTones = Math.Abs( noteElement.PitchValue.SemiTonesAboveC0 - originSemitones);
            // Logger.LogCF(string.Format(": Interval={0} semitones", semiTones));
#warning ToDo: code this

            int semiTonesWithinOctave = semiTones % 12;
            int relativeOctave = semiTones / 12;
            byte[] octaveMark = new byte[] { }; // Empty as default.
            string octaveMarkText = "";
            if (0 != relativeOctave)
            {
                //  Manual of BrailleMusic Notation Chapter 8.4)
                octaveMark = GetOctaveMark(noteElement.Octave); // Table 2: "Octave Signs"
                octaveMarkText = string.Format("{0}", noteElement.Octave); // We might start with som identofyer, for instance "Oct" but this uses same syntax as the normal octave mark !
                // Logger.LogCFOnce(string.Format(": OctaveMarkText={0}", octaveMarkText)); 
            }

            // Find the interval representation within the octave (Ref.1 Chapter 8.4)
            byte[] bytes = new byte[] { };
            string s = "";
            switch (semiTonesWithinOctave)
            {
                // "p" means "Perfect"
                // "m" means "Minor"
                // "M" means "Major"
                // "d" means "Diminished"
                // "a" means "Augmented"
                case 0: bytes = intervalPerfectUnison; s = "p1"; break; // C to C
                case 1: bytes = intervalMinorSecond; s = "m2"; break;// C to D
                case 2: bytes = intervalMajorSecond; s = "M2"; break; // C to D
                case 3: bytes = intervalMinorThird; s = "m3"; break; // C to Eb
                case 4: bytes = intervalMajorThird; s = "M3"; break; // C to E
                case 5: bytes = intervalPerfectFourth; s = "p4"; break; // C to F
                case 6: bytes = intervalTritone; s = "d5"; break; // C to F#
                case 7: bytes = intervalPerfectFifth; s = "p5"; break; // C to G
                case 8: bytes = intervalMinorSixth; s = "m6"; break; // C to Ab
                case 9: bytes = intervalMajorSixth; s = "M6"; break; // C to A
                case 10: bytes = intervalMinorSeventh; s = "m7"; break;
                case 11: bytes = intervalMajorSeventh; s = "M7"; break;// C to H
                case 12: bytes = intervalPerfectOctave; s = "p8"; break; // C to C
                default:
                    bytes = intervalUnknown;
                    s = string.Format("({0})",semiTones);
                    Logger.LogCFOnce(string.Format(": Unknown interval: {0} semitones", semiTones));
                    break;
            }
            // Logger.LogCF(string.Format("Semitones={0} {1}", semiTones, s));
            Append(octaveMark); // The octave mark prececes the interval
            Append(bytes);
            AppendText(string.Format(" {0}",octaveMarkText)); // The octave mark prececes the interval
            AppendText(string.Format("{0}",s));
        }


        /// <summary>
        /// New implementation NOT based on the Pitch value, but solely on octave and 
        /// </summary>
        /// <param name="noteElement"></param>
        /// <param name="note0"></param>
        private void AddIntervalV1(NoteElement noteElement, NoteElement noteElement0)
        {
            // Find the interval in semitones without respect to altering
            int s0 = noteElement0.PitchValue.SemiTonesAboveC0 - noteElement0.PitchValue.Alter;
            int s1 = noteElement.PitchValue.SemiTonesAboveC0 - noteElement.PitchValue.Alter;

            int semiTones = Math.Abs(s1-s0);
            // Logger.LogCF(string.Format(": Interval={0} semitones", semiTones));
#warning ToDo: code this

            int semiTonesWithinOctave = semiTones % 12;
            int relativeOctave = semiTones / 12;
            byte[] octaveMark = new byte[] { }; // Empty as default.
            string octaveMarkText = "";
            if (0 != relativeOctave)
            {
                //  Manual of BrailleMusic Notation Chapter 8.4)
                octaveMark = GetOctaveMark(noteElement.Octave); // Table 2: "Octave Signs"
                octaveMarkText = string.Format("{0}", noteElement.Octave); // We might start with som identofyer, for instance "Oct" but this uses same syntax as the normal octave mark !
                // Logger.LogCFOnce(string.Format(": OctaveMarkText={0}", octaveMarkText)); 
            }

            // Find the interval representation within the octave (Ref.1 Chapter 8.4)
            byte[] bytes = new byte[] { };
            string s = "";
            switch (semiTonesWithinOctave)
            {
                case 0: bytes = intervalUnison; s = "1"; break; // C to C
                case 1: bytes = intervalSecond; s = "2"; break;// C to Db
                case 2: bytes = intervalSecond; s = "2"; break; // C to D
                case 3: bytes = intervalThird; s = "3"; break; // C to Eb
                case 4: bytes = intervalThird; s = "3"; break; // C to E
                case 5: bytes = intervalFourth; s = "4"; break; // C to F
                case 6: bytes = intervalFourth; s = "5"; break; // C to F#
                case 7: bytes = intervalFifth; s = "5"; break; // C to G
                case 8: bytes = intervalSixth; s = "6"; break; // C to Ab
                case 9: bytes = intervalSixth; s = "6"; break; // C to A
                case 10: bytes = intervalSeventh; s = "7"; break;
                case 11: bytes = intervalSeventh; s = "7"; break;// C to H
                case 12: bytes = intervalPerfectOctave; s = "8"; break; // C to C
                default:
                    bytes = intervalUnknown;
                    s = string.Format("({0})", semiTones);
                    Logger.LogCFOnce(string.Format(": Unknown interval: {0} semitones", semiTones));
                    break;
            }

#warning ToDo refactor next code as private methode
            byte[] accidentalMark = new byte[] { };
            string accidentalText = "";
            char AccidentalNatural = (char) 0x266E; // The Unicode representation of the "Natural" accidental
            if (null != noteElement.AccidentalElement)
            {
                switch (noteElement.AccidentalElement.Accidental)
                {
                    case AccidentalTypeEnum.flat: accidentalMark = new byte[] { Flat }; accidentalText = "b"; break;
                    case AccidentalTypeEnum.flatFlat: accidentalMark = new byte[] { Flat, Flat }; accidentalText = "bb"; break;
                    case AccidentalTypeEnum.sharp: accidentalMark = new byte[] { Sharp }; accidentalText = "#"; break;
                    case AccidentalTypeEnum.doubleSharp: accidentalMark = new byte[] { Sharp,Sharp }; accidentalText = "##"; break;
                    case AccidentalTypeEnum.natural: accidentalMark = new byte[] { Natural }; accidentalText = AccidentalNatural.ToString(); break; // Danish "Opløsningstegn"
                    case AccidentalTypeEnum.none: break;
                    default:
                        Logger.LogCF(string.Format("Unsuported accidental= {0}", noteElement.AccidentalElement.Accidental)); break;
                }
            }
            

            // Logger.LogCF(string.Format("Semitones={0} {1}", semiTones, s));
            Append(octaveMark); // The octave mark prececes the interval
            Append(accidentalMark); // Add any accidential
            Append(bytes);
            AppendText(string.Format(" {0}", octaveMarkText)); // The octave mark prececes the interval
            AppendText(accidentalText); // Add any accidential
            AppendText(string.Format("{0}", s));
        }
#endif

        /// <summary>
        /// New implementation NOT based on the Pitch value, but solely on octave and 
        /// </summary>
        /// <param name="noteElement"></param>
        /// <param name="note0"></param>
        private void AddIntervalV2(NoteElement noteElement, NoteElement noteElement0)
        {
            // Find the interval in semitones without respect to altering
            int s0 = noteElement0.PitchValue.FullStepsAboveC0;
            int s1 = noteElement.PitchValue.FullStepsAboveC0;

            int fullSteps = Math.Abs(s1 - s0);
            // Logger.LogCF(string.Format(": Interval={0} semitones", semiTones));
#warning ToDo: code this

            int fullStepsWithinOctave = fullSteps % 7;
            int relativeOctave = fullSteps / 7;
            byte[] octaveMark = new byte[] { }; // Empty as default.
            string octaveMarkText = "";
            if (0 != relativeOctave)
            {
                //  Manual of BrailleMusic Notation Chapter 8.4)
                octaveMark = GetOctaveMark(noteElement.Octave); // Table 2: "Octave Signs"
                octaveMarkText = string.Format("{0}", noteElement.Octave); // We might start with som identifier, for instance "Oct" but this uses same syntax as the normal octave mark !
                // Logger.LogCFOnce(string.Format(": OctaveMarkText={0}", octaveMarkText)); 
            }

            // Find the interval representation within the octave (Ref.1 Chapter 8.4)
            byte[] bytes = new byte[] { };
            string s = "";
            switch (fullStepsWithinOctave)
            {
                case 0: // This is either a prime, an octave or a "romote octave" (2 or more octaves away)
                    switch (relativeOctave)
                    {
                        case 1: // The "neighbour" octave, exactly 8 steps away.
                            octaveMark = new byte[] { }; // This is the "neighbour" octave! No octavemark!
                            octaveMarkText = "";  // This is the "neighbour" octave! No octavemark text!
                            bytes = intervalOctave; 
                            s = "8"; // C to neighbour C
                            break; 
                        case 0:
                        default:
                            // The "prime" interval of 0 steps is handled exactly the same way as the intervals of  16, 24, 32 etc steps
                            octaveMark = GetOctaveMark(noteElement.Octave); // Table 2: "Octave Signs"
                            octaveMarkText = string.Format("{0}", noteElement.Octave);
                            bytes = intervalOctave;
                            s = "0"; // C to C either in the same octace or at least 16 steps away
                            break;
                    }
                    break;                 
                case 1: bytes = intervalSecond; s = "2"; break;// C to D
                case 2: bytes = intervalThird;  s = "3"; break; // C to E
                case 3: bytes = intervalFourth; s = "4"; break; // C to F
                case 4: bytes = intervalFifth;  s = "5"; break; // C to G
                case 5: bytes = intervalSixth;  s = "6"; break; // C to A
                case 6: bytes = intervalSeventh; s = "7"; break; // C to H
//              case 7: bytes = intervalOctave; s = "8"; break; // C to C // Will never occur. Is coded in case 0 with an octave number specified
          
                default:
                    bytes = intervalUnknown;
                    s = string.Format("({0})", fullStepsWithinOctave);
                    Logger.LogCFOnce(string.Format(": Unknown interval: {0} fullStepsWithinOctave", fullStepsWithinOctave));
                    break;
            }

#warning ToDo refactor next code as private methode, returning a BrailleBuilder
#warning Todo Also refactor the generation of bytes and ocdavemark in the same way 
            byte[] accidentalMark = new byte[] { };
            string accidentalText = "";
            char AccidentalNatural = (char)0x266E; // The Unicode representation of the "Natural" accidental
            if (null != noteElement.AccidentalElement)
            {
                switch (noteElement.AccidentalElement.Accidental)
                {
                    case AccidentalTypeEnum.flat: accidentalMark = new byte[] { Flat }; accidentalText = "b"; break;
                    case AccidentalTypeEnum.flatFlat: accidentalMark = new byte[] { Flat, Flat }; accidentalText = "bb"; break;
                    case AccidentalTypeEnum.sharp: accidentalMark = new byte[] { Sharp }; accidentalText = "#"; break;
                    case AccidentalTypeEnum.doubleSharp: accidentalMark = new byte[] { Sharp, Sharp }; accidentalText = "##"; break;
                    case AccidentalTypeEnum.natural: accidentalMark = new byte[] { Natural }; accidentalText = AccidentalNatural.ToString(); break; // Danish "Opløsningstegn"
                    case AccidentalTypeEnum.none: break;
                    default:
                        Logger.LogCF(string.Format("Unsuported accidental= {0}", noteElement.AccidentalElement.Accidental)); break;
                }
            }


            // Add any tie
            byte[] tieMark = new byte[] { };
            string tieText = "";
            if (noteElement.TieStart)
            {
                tieMark = BrailleBuilder.Tie;
                tieText = "Tie";
            }

            byte[] slurMark = new byte[] { };
            string slurText = "";
            // Add any slur
            if (noteElement.SlurStart)
            {
                slurMark = new byte[] {BrailleBuilder.Slur };
                slurText = "Slur";
            }


            // Logger.LogCF(string.Format("Semitones={0} {1}", semiTones, s));
            Append(accidentalMark); // Add any accidential, preceding the octave mark
            Append(octaveMark); // The octave mark prececes the interval
            Append(bytes);
            Append(tieMark); // The tie follows the note
            Append(slurMark); // The slur follows the note
            AppendText(accidentalText); // Add any accidential, preceding the octave mark
            AppendText(string.Format(" {0}", octaveMarkText)); // The octave mark prececes the interval
            AppendText(string.Format("{0}", s));
            AppendText(string.Format("{0}", tieText));
            AppendText(string.Format("{0}", slurText));

        }









        /// <summary>
        /// Adds BrailleMusic Intervalnotation or "Bistemme" for a list of notes.      
        /// </summary>
        /// <param name="noteElements"></param>
        /// <param name="StatusInformation"></param>
        /// <param name="addNotations"></param>
        public bool AdIntervalNotation(List<NoteElement> noteElements, StatusInformation statusInformation, bool addNotations, bool fromTop)
        {
#warning TODO Assert that (count > 1) (All parts ar identical) , (all stafffs are identical), no notes are rests !

            const string epilog = " "; // Add a space after the Interval representation (as after notes and rests) 
            bool strictRules = false;
            if (!IsWellformedIntervalRepresentation(noteElements, strictRules))
            {
                // Logger.LogCFOnce(string.Format(": Not wellformed"));
                // Utilities.Beep();
                return false;
            }

            // From now on we can assume that noteElements are wellformed for generation of Interval-representation.
            if (fromTop)
            {
                noteElements.Sort(Descending); // For staff 1 
            }
            else
            {
                noteElements.Sort(Ascending); // For all other staffs
            }

            // Log(noteElements);

            if (addNotations) AddBrailleNotationsBeforeNotes(noteElements);

            // Now generate the Braille representation for the List<NoteElement>
            AppendText("[");

            NoteElement initialNoteElement = noteElements[0];
            if (addNotations) this.AddBrailleNotationsBeforeNoteOrRest(initialNoteElement.Notations); // Some notations are added Before the note/rest itself 
            this.AddNote(initialNoteElement, statusInformation.CurrentKeyElement); // Add the "Origin" noteelement as usual
            if (addNotations) this.AddBrailleNotationsAfterNoteOrRest(initialNoteElement.Notations); // Some notations are added After the note/rest itself                       

            for (int i = 1; (i < noteElements.Count); i++)
            {
                this.AddIntervalV2(noteElements[i], noteElements[0]); // Based on BANA 2015 chapter 9 and comments from Lars, but using fullsteps.
            }
            AppendText("]" + epilog);


            if (addNotations) AddBrailleNotationsAfterNotes(noteElements);

            return true;
        }


        private void AddBrailleNotationsBeforeNotes(List<NoteElement> noteElements)
        {
            Logger.LogCFOnce(": Not implemented yet!");
        }


        private void AddBrailleNotationsAfterNotes(List<NoteElement> noteElements)
        {
            Logger.LogCFOnce(": Not implemented yet!");
        }

        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private BrailleBuilderForIntervalNotation(Int64 timeStamp) : base(timeStamp)
        {
        }

        public static new BrailleBuilderForIntervalNotation Create(Int64 timeStamp)
        {
            return new BrailleBuilderForIntervalNotation(timeStamp);
        }


    }
}
