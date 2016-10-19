using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicXmlReaderModel;

namespace MusicXmlReaderUI
{


    class BrailleBuilder
    {
        // References:
        // Ref.1: http://usermanuals.musicxml.com/MusicXML/Content/EL-MusicXML-articulations.htm
        // Ref.2: https://en.wikipedia.org/wiki/Braille_music
        // Ref.3: https://www.rnib.org.uk/sites/default/files/New%20International%20Manual.pdf
        // Ref.4: Elementær nodelære i Braille-skrift Enstemmig notation, Revideret udgave 1995, SynsCenter Refsnæs
        // Ref.5: Elementær nodelære i Braille-skrift 2. del Akkordnotation SynsCenter Refsnæs
        // Ref.6: MUSIC BRAILLE CODE 1997 Developed Under the Sponsorship of the BRAILLE AUTHORITY OF NORTH AMERICA
        //        Downloaded to C:\Users\Jens\Dropbox\Root\Dokumenter\music braille code.pdf

        //public enum Constant
        //{
        //    FourMeasureRest,
        //    DoubleBar,
        //    Dot,
        //    MusicHyphen,
        //    Triplet,
        //    RepeatSign,
        //    Slur,
        //    Tie,
        //};

        // Valuse for explicitly defining dot patterns in terms of hex byte-values
        private const byte noDots = 0;
        private const byte dot1 = 0x01;
        private const byte dot2 = 0x02;
        private const byte dot3 = 0x04;
        private const byte dot4 = 0x08;
        private const byte dot5 = 0x10;
        private const byte dot6 = 0x20;
        private const byte dot7 = 0x40;
        private const byte dot8 = 0x80;


        public static readonly byte[] FullMeasureRest = new byte[] { noDots, (dot1+dot3+dot4), noDots }; // According to Lars Petersen: space,m,space
        public static readonly byte[] FourMeasureRest = new byte[] { 60, 25, 13 };
        public static readonly byte[] DoubleBar = new byte[] { 35 };
        public static readonly byte Dot = 4;
        public static readonly byte MusicHyphen = 16; // This measure will be continued on the following line
        public static readonly byte Triplet = 6;
        public static readonly byte RepeatSign = 54; // A beat, a half measure or a full measure must be repeated
        public static readonly byte Slur = 9; // Connects 2 or more notes of different pitch. Danish: Legatobue
        public static readonly byte[] Tie = new byte[] { 8, 9 }; //  Connects 2 notes of the same pitch and makes them souna as one. Danish: Bindebue
        public static readonly byte[] ChordTie = new byte[] { 40, 9 };
        public static readonly byte[] BracketSlurStart = new byte[] { 48, 6 };
        public static readonly byte[] BracketClurEnd = new byte[] { 24, };
        public static readonly byte WordSign = 28; //  Must preceed all musical indications.
        public static readonly byte WordApostrophe = 32; // The word will be continued on the following line
        public static readonly byte[] CrescendoHairpin = new byte[] { 28, 9 };
        public static readonly byte[] Crescendo = new byte[] { 28, 9, 23 };
        public static readonly byte[] DimShape = new byte[] { 28, 25 };
        public static readonly byte[] Forte = new byte[] { 28, 11 };
        public static readonly byte[] Fortissimo = new byte[] { 28, 11, 11 };
        public static readonly byte[] Fff = new byte[] { 28, 11, 11, 11 };
        public static readonly byte[] MezzoForte = new byte[] { 28, 13, 11 };
        public static readonly byte[] Piano = new byte[] { 28, 15 };
        public static readonly byte[] Pianissimo = new byte[] { 28, 15, 15 };
        public static readonly byte[] MezzoPiano = new byte[] { 28, 13, 15 };
        public static readonly byte[] Dim = new byte[] { 28, 25, 10, 9 };
        public static readonly byte[] Rallentando = new byte[] { 28, 23, 1, 7, 7, 4 };
        public static readonly byte[] Ritardando = new byte[] { 28, 23, 10, 30 };
        public static readonly byte[] Ritenuto = new byte[] { 28, 23, 10, 30, 17, 4 };
        public static readonly byte Staccato = 38; // Articulation mark
        public static readonly byte[] Staccatissimo = new byte[] { 32, 38 };    // Articulation mark
        public static readonly byte[] Tenuto = new byte[] { 56, 38 };   // Articulation mark
        public static readonly byte[] TenutoStaccato = new byte[] { 16, 38 };// Articulation mark
        public static readonly byte[] Accent = new byte[] { 24, 38 };// Articulation mark
        public static readonly byte[] Martellato = new byte[] { 48, 38 };// Articulation mark
        public static readonly byte[] CommaHalfBreath = new byte[] { 28, 2 };     //  Articulation mark Ref.3 Page 268
        public static readonly byte[] FullBreakOrBreath = new byte[] { 32, 12 };  //  Articulation mark Ref.3 Page 268
        public static readonly byte[] Swell = new byte[] { 33, 4 };// Articulation mark
        public static readonly byte[] FermatoOnNote = new byte[] { 57 };
        public static readonly byte[] FermataBetweenNotes = new byte[] { 16, 35, 7 };
        public static readonly byte[] FermataOverBarLine = new byte[] { 56, 35, 7 };
        public static readonly byte[] MeasureInAccord = new byte[] { 35, 28 };
        public static readonly byte[] PartMeasureInAccord = new byte[] { 16, 2 };
        public static readonly byte[] MeasureDivisionSign = new byte[] { 40, 5 };
        public static readonly byte Flat = 35;
        public static readonly byte Sharp = 41;
        public static readonly byte Natural = 33;
        public static readonly byte TupletOf3 = dot2 + dot3;
        public static readonly byte[] ArpeggioUp = new byte[] { dot3 + dot4 + dot5, dot1 + dot3 };
        public static readonly byte[] ArpeggioDown = new byte[] { (dot3 + dot4 + dot5), (dot1 + dot3) , (dot1 + dot3) };

        // The followung symbolr are used as the fixed symbol describing the key
        public static readonly byte KeySharp = dot1 + dot4 + dot6;
        public static readonly byte KeyFlat  = dot1 + dot2 + dot6;
        public static readonly byte KeyNone  = dot1 + dot6;   // "opløsningstegn" 

        public static readonly byte Number = dot3 + dot4 + dot5 + dot6; // Marks the start of numeric coding
        public static readonly byte cipher0 = dot2 + dot4 + dot5; // 
        public static readonly byte cipher1 = dot1; // 
        public static readonly byte cipher2 = dot1 + dot2; // 
        public static readonly byte cipher3 = dot1 + dot4; // 
        public static readonly byte cipher4 = dot1 + dot4 + dot5; // 
        public static readonly byte cipher5 = dot1 + dot5; // 
        public static readonly byte cipher6 = dot1 + dot2 + dot4; // 
        public static readonly byte cipher7 = dot1 + dot2 + dot4 + dot5; // 
        public static readonly byte cipher8 = dot1 + dot2 + dot5; // 
        public static readonly byte cipher9 = dot2 + dot5; // 

        public static readonly byte[] musicBraille = new byte[] { dot6, dot3 }; // Marks the start of Music Braille coding


        // Clefs
        public static readonly byte[] clefG = new byte[] { (dot3 + dot4 + dot5), (dot3 + dot4       ), (dot1 + dot2 + dot3) };
        public static readonly byte[] clefF = new byte[] { (dot3 + dot4 + dot5), (dot3 + dot4 + dot6), (dot1 + dot2 + dot3) };

        // Repeat
        public static readonly byte[] repeatEnd   = new byte[] { dot6, dot3, (dot1 + dot2 + dot6), (dot2 + dot3 + dot5 + dot6) };
        public static readonly byte[] repeatStart = new byte[] { dot6, dot3, (dot1 + dot2 + dot6), (dot2 + dot3) } ;

        // Note: Articulation marks must be inserted BEFORE the note



        public enum Hand { Undefined, Left, Right };


        private List<byte> braille;

        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private BrailleBuilder()
        {
            braille = new List<byte>();
        }

        public static BrailleBuilder Create()
        {
            return new BrailleBuilder();
        }

        private const string className = "BrailleBuilder"; // Only used for logging ! 

        public List<byte> Braille
        {
            get
            {
                return braille;
            }
        }

        public void Append(List<byte> bytes)
        {
            this.braille.AddRange(bytes);
        }

        public void Append(byte[] bytes)
        {
            this.braille.AddRange(new List<byte>(bytes));
        }

        public void Append(byte b)
        {
            this.braille.Add(b);
        }
        
        private byte GetStepValue(PitchElement.FullStepEnum step) //  Returns the values for dot 1,2,4,5
        {
            switch (step)
            {
                case PitchElement.FullStepEnum.C: return dot1+dot4+dot5; // Pin 1,4,5
                case PitchElement.FullStepEnum.D: return dot1+dot5; // Pin 1,5
                case PitchElement.FullStepEnum.E: return dot1+dot2+dot4; // Pin 1,2,4,
                case PitchElement.FullStepEnum.F: return dot1+dot2+dot4+dot5; // Pin 1,2,4,5
                case PitchElement.FullStepEnum.G: return dot1+dot2+dot5;// Pin 1,2,5
                case PitchElement.FullStepEnum.A: return dot2+dot4; // Pin 2,4
                case PitchElement.FullStepEnum.B: return dot2+dot4+dot5; // Pin 2,4,5
                default:
                    Logger.Log(string.Format("{0}.GetStepValue({1}) Unknown step '{1}'", className, step));
                    return noDots;
            }
        }


        private byte GetTypeValue(NoteTypeEnum noteDuration) //  Returns the values for dot 3,6
        {
            switch (noteDuration)
            {
                case NoteTypeEnum.whole:
                case NoteTypeEnum.nt16th:  return dot3+dot6;

                case NoteTypeEnum.half:
                case NoteTypeEnum.nt32nd: return dot3;

                case NoteTypeEnum.quarter:
                case NoteTypeEnum.nt64th: return dot6;

                case NoteTypeEnum.eight:  
                case NoteTypeEnum.nt128th: return 0;

                default:
                    Logger.LogOnce(string.Format("{0}.GetTypeValue({1}) Unknown noteDuration '{1}'", className, noteDuration.ToString()));
                    return noDots;
            }
        }


        private byte GetRestValue(NoteTypeEnum noteDuration)
        {
            switch (noteDuration)
            {
                case NoteTypeEnum.whole:
                case NoteTypeEnum.nt16th: return dot1 + dot3 + dot4;

                case NoteTypeEnum.half:
                case NoteTypeEnum.nt32nd: return dot1 + dot3 + dot6;

                case NoteTypeEnum.quarter:
                case NoteTypeEnum.nt64th: return dot2 + dot3 + dot6;

                case NoteTypeEnum.eight:
                case NoteTypeEnum.nt128th: return dot1 + dot3 + dot4 + dot6;

                default:
                    Logger.LogOnce(string.Format("{0}.GetRestValue({1}) Unknown noteDuration '{1}'", className, noteDuration.ToString()));
                    return noDots;
            }
        }

        private byte[] GetOctaveMark(int octave)
        {
            switch (octave)
            {
                case 1: return new byte[] {  8 };
                case 2: return new byte[] { 24 };
                case 3: return new byte[] { 56 };
                case 4: return new byte[] { 16 };
                case 5: return new byte[] { 40 };
                case 6: return new byte[] { 48 };
                case 7: return new byte[] { 32 };
            }
            if (octave < 1) return new byte[] { 8, 8 };
            if (octave > 7) return new byte[] { 32, 32 };
            // This is an error. Log it an return something hopefully harmles
            Logger.Log(string.Format("{0}.Getoctavemark({1}) was called with illegal parametervalue octave={1}",className, octave));
            return new byte[] { 8, 8 };
        }

        
        public void AddNotationsBeforeNote(NotationsElement notations) // Some notations are added Before the note itself
        {
            if (null == notations) return;
            if (null == notations.Articulations) return;
            const string functionName = "AddNotationsBeforeNote";

            if (null != notations.TupletElement)
            {
                if (notations.TupletElement.StartStopContinueType == StartStopContinueElement.StartStopContinueTypeEnum.Start)
                {
                    Braille.AddRange(new List<byte>(TupletOf3));
                    Logger.LogOnce(string.Format("{0}.{1} Added tuplet", className, functionName));
                }
            }
                        

            foreach (ArticulationsElement.Articulation articulation in notations.Articulations.ArticulationList)
            {
                bool implemented = true; 
                switch (articulation)
                {
                    // Cases are shown in the same saquence as in the MusicXml definition:
                    case ArticulationsElement.Articulation.accent:          Braille.AddRange(Accent); break;
                    case ArticulationsElement.Articulation.breathmark:      Braille.AddRange(CommaHalfBreath);  break;
                    case ArticulationsElement.Articulation.caesura:         implemented = false; break;
                    case ArticulationsElement.Articulation.detachedlegato:  implemented = false; break;
                    case ArticulationsElement.Articulation.doit:            implemented = false; break;
                    case ArticulationsElement.Articulation.falloff:         implemented = false; break;
                    case ArticulationsElement.Articulation.otherarticulation: implemented = false; break;
                    case ArticulationsElement.Articulation.plop:            implemented = false; break;
                    case ArticulationsElement.Articulation.scoop:           implemented = false; break;
                    case ArticulationsElement.Articulation.spiccato:        implemented = false; break;
                    case ArticulationsElement.Articulation.staccatissimo:   Braille.AddRange(Staccatissimo); break;
                    case ArticulationsElement.Articulation.staccato:        Braille.Add(Staccato); break;
                    case ArticulationsElement.Articulation.stress:          implemented = false; break;
                    case ArticulationsElement.Articulation.strongaccent:    implemented = false; break;
                    case ArticulationsElement.Articulation.tenuto:          Braille.AddRange(Tenuto); break;
                    case ArticulationsElement.Articulation.unstress:        implemented = false; break;    
                    default:
                        Logger.Log(string.Format("{0}.{1}: Unknown articulation: '{2}'",className,functionName, articulation)); break;
                }
                if (!implemented)
                {
                    Logger.Log(string.Format("{0}.{1}: Unimplemented articulation: '{2}'",className,functionName, articulation));
                }
            }
        }

        private void LogUninplementedNotationElement(Element notationsElement, string elementName)
        {
            string functionName = "LogUninplementedNotationElement";
            if (null == notationsElement) return;
            Logger.LogOnce(string.Format("{0}.{1}:: Unimplemented NotationElement:{2}", className, functionName, elementName));
        }

        private void LogUninplementedOrnamentsElement(OrnamentsElement ornamentsElement, string elementName)
        {
            string functionName = "LogUninplementedOrnamentsElement";
            if (null == ornamentsElement) return;
            string ornaments = ornamentsElement.UnlocalizedString();
            Logger.LogOnce(string.Format("{0}.{1}: Unimplemented NotationElement:{2} Value={3}", className, functionName, elementName,ornaments));
        }

        public void AddNotationsAfterNote(NotationsElement notations)  // Some notations are added After the note itself
        {
            // Actually we dont know what is added before and what is added after.
            // This method is primarily used for logging unimplemented notations !
            const string functionName = "AddNotationsAfterNote";

            if (null == notations) return;
            BrailleBuilder bb = new BrailleBuilder();

            if ((null != notations.SlurElement) && (notations.SlurElement.StartStopContinueType == StartStopContinueElement.StartStopContinueTypeEnum.Start))
            {
                bb.Append(Slur);
                //Logger.LogOnce(string.Format("{0}.{1} Added slur", className, functionName));
            }

            if (null != notations.TiedElement)
            {
                if
                (  (notations.TiedElement.StartStopContinueType == StartStopContinueElement.StartStopContinueTypeEnum.Start)
                || (notations.TiedElement.StartStopContinueType == StartStopContinueElement.StartStopContinueTypeEnum.Continue)
                || (notations.TiedElement.StartStopContinueType == StartStopContinueElement.StartStopContinueTypeEnum.Stop)
                )
                {
                    bb.Append(Tie);
                    //Logger.LogOnce(string.Format("{0}.{1} Added tie", className, functionName));
                    // TODO: implement algorithm handling ties conencting more than 4 notes.
                }
            }

            if (null != notations.FermataElement)
            {
                if ((notations.FermataElement.FermataType == FermataElement.FermataTypeEnum.inverted)
                    || (notations.FermataElement.FermataType == FermataElement.FermataTypeEnum.upright))
                {
                    bb.Append(FermatoOnNote);
                }
            }

            if (null != notations.ArpeggiateElement)
            {
                switch (notations.ArpeggiateElement.ArpeggiateDirection)
                {
                    case ArpeggiateDirectionEnum.down: bb.Append(ArpeggioDown); return;
                    case ArpeggiateDirectionEnum.up: bb.Append(ArpeggioUp);return;
                    default:   break;                
                
                }
                Logger.LogOnce(string.Format("{0}.{1} Unknown Arpeggio direction: {2}", className, functionName, notations.ArpeggiateElement.ArpeggiateDirection.ToString()));            
            }
                  
            LogUninplementedNotationElement(notations.SlideElement, "SlideElement"); 
            LogUninplementedNotationElement(notations.GlissandoElement, "GlissandoElement"); 
            LogUninplementedOrnamentsElement(notations.OrnamentsElement, "OrnamentsElement");

            // Add other notation elements here asthey are added in the parser.
        }


        /// <summary>
        /// Add a NoteElement
        /// </summary>
        /// <param name="noteElement">The NoteElement to add</param>
        public void AddNote(NoteElement noteElement)
        {
            // Add alteration
            if (0 != noteElement.Alter) braille.Add((noteElement.Alter > 0) ? Sharp : Flat);
      
            // Add Octavemark
            if (MusicBrailleState.NeedOctaveMark(noteElement.Octave, noteElement.PitchValue.SemiTonesAboveC0))
            {
                byte[] octaveMark = GetOctaveMark(noteElement.Octave);
                braille.AddRange(octaveMark);
            }

            // Add the combined step value and type value
            byte stepPart = GetStepValue(noteElement.Step);          //  Returns the values for pin 1,2,4,5
            byte typePart = GetTypeValue(noteElement.NoteDuration);  //  Returns the values for pin 3,6
            byte note = (byte)((int)stepPart | (int)typePart);       //  Logical OR to get all 6 pin values 
            braille.Add(note);

            // Add punctuation value
            if (noteElement.Dot) braille.Add(Dot);

            // Start for debugging only:
            StringBuilder sb = new StringBuilder();
            foreach (byte b in braille)
            {
                sb.Append(string.Format("0x{0:x} ", b)); 
            }
            //string s =  string.Format("step={0} alter={1} octave={2} semitone={3} ==>{4}", step, alter, octave, semitonesAboveC0, sb.ToString());
            //Logger.Log(s);
            // End for debugging only:
        }

        public void AddRepeatForward(RepeatElement repeatElement)
        {
            const string functionName = "AddRepeatForward";
            Braille.AddRange(repeatStart);
            Logger.LogOnce(string.Format("{0}.{1}", className, functionName));
        }

        public void AddRepeatBackward(RepeatElement repeatElement)
        {
            const string functionName = "AddRepeatBackward";
            Braille.AddRange(repeatEnd);
            Logger.LogOnce(string.Format("{0}.{1}", className, functionName));
        }        

        public void AddTime(TimeElement timeElement)
        {
            const string functionName = "BrailleBuilder.AddTime";
            byte[] bytes = new byte[] { };
            byte beatType;
            switch (timeElement.BeatType)
            {
                case 2:
                    beatType = (dot2 + dot3);
                    switch (timeElement.Beats)
                    {                       
                        case 2: bytes = new byte[] { Number, cipher2, beatType  }; break;       // 2/2  Not specified by REFSNÆS, suggested by JSJ
                        default: break;
                    }
                    break;

                case 4:
                    beatType = (dot2 + dot5 + dot6);
                    switch (timeElement.Beats)
                    {
                        case 2: bytes = new byte[] { Number, cipher2, beatType }; break; // 2/4
                        case 3: bytes = new byte[] { Number, cipher3, beatType }; break; // 3/4
                        case 4: bytes = new byte[] { Number, cipher4, beatType }; break; // 4/4
                        case 5: bytes = new byte[] { Number, cipher5, beatType }; break; // 5/4 Not specified by REFSNÆS, suggested by JSJ
                        case 6: bytes = new byte[] { Number, cipher6, beatType }; break; // 6/4 Not specified by REFSNÆS, suggested by JSJ
                        default:  break;
                    }
                    break;

                case 8:
                    beatType = (dot2 + dot3 + dot6);
                    switch (timeElement.Beats)
                    {       
                        case 3: bytes = new byte[] { Number, cipher3, beatType }; break; // 3/8
                        case 4: bytes = new byte[] { Number, cipher4, beatType }; break; // 4/8
                        case 6: bytes = new byte[] { Number, cipher6, beatType }; break; // 6/8 
                        case 7: bytes = new byte[] { Number, cipher7, beatType }; break; // 7/8  Not specified by REFSNÆS, suggested by JSJ
                        default:  break;
                    }
                    break;
                default:break;
            }
            if (0 == bytes.Count())
            {
                Logger.LogOnce(string.Format("{0}: Unsupported time specification: Beats={1} BeatsType={2}", functionName, timeElement.Beats, timeElement.BeatType));
            }
            else
            {
                Braille.AddRange(new List<byte>(bytes));
                //Logger.LogOnce(string.Format("{0}: Added time specification: {1}/{2}", functionName, timeElement.Beats, timeElement.BeatType));
            }
        }


        public void AddClef(ClefElement clefElement)
        {
            const string functionName = "AddClef";
            byte[] bytes = new byte[] { };
            switch (clefElement.Clef)
            {
                case ClefEnum.G: bytes = clefG; break;
                case ClefEnum.F: bytes = clefF; break;
                case ClefEnum.C:
                case ClefEnum.percussion:
                case ClefEnum.TAB :
                case ClefEnum.jianpu:
                case ClefEnum.none:
                    Logger.LogOnce(string.Format("{0}.{1} Clef={2} is not supported in Music Braille", className, functionName, clefElement.Clef.ToString())); break;
                default: Logger.LogOnce(string.Format("{0}.{1} Unknown clef={2}", className,functionName, clefElement.Clef.ToString())); break;
            }
            braille.AddRange(bytes);          
        }


        public void AddKey(KeyElement keyElement)
        {
            const string functionName = "AddKey";
            byte[] bytes = new byte[] { };
            switch (keyElement.Fifths)
            {
                case 0: break; //  Ask Lars Petersen !!! How do we return to C major or A minor from another key ??
                case 1: bytes = new byte[] { KeySharp }; break;
                case 2: bytes = new byte[] { KeySharp, KeySharp }; break;
                case 3: bytes = new byte[] { KeySharp, KeySharp, KeySharp }; break;
                case 4: bytes = new byte[] { Number, cipher4, KeySharp }; break;
                case 5: bytes = new byte[] { Number, cipher5, KeySharp }; break;
                case 6: bytes = new byte[] { Number, cipher6, KeySharp }; break;
                case 7: bytes = new byte[] { Number, cipher7, KeySharp }; break;
                case -1: bytes = new byte[] { KeyFlat }; break;
                case -2: bytes = new byte[] { KeyFlat, KeyFlat }; break;
                case -3: bytes = new byte[] { KeyFlat, KeyFlat, KeyFlat }; break;
                case -4: bytes = new byte[] { Number, cipher4, KeyFlat }; break;
                case -5: bytes = new byte[] { Number, cipher5, KeyFlat }; break;
                case -6: bytes = new byte[] { Number, cipher6, KeyFlat }; break;
                case -7: bytes = new byte[] { Number, cipher7, KeyFlat }; break;
                default: Logger.LogOnce(string.Format("{0}.{1}: Illegal number of fifths={2}", className,functionName, keyElement.Fifths)); break;
            }
            braille.AddRange(bytes);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">From "whole" to 128nd</param>
        /// <param name="punctured">A puncture added</param>
        public void AddRest(NoteTypeEnum noteDuration, bool punctured) // ********************* FIX ! Temp signature
        {
            // const string functionName = "AddRest";
            if (NoteTypeEnum.measure == noteDuration)
            {
                braille.AddRange(FullMeasureRest);
                //Logger.LogOnce(string.Format("{0}.{1}: Added FullMeasureRest", className, functionName));
            }
            else
            {
                byte rest = GetRestValue(noteDuration);
                braille.Add(rest);
                if (punctured) braille.Add(Dot);
            }
        }


        public void AddFinger(Hand hand, int finger)
        {
            const byte left = 56;
            const byte right = 40;
            const byte handConst = 28;

            const byte finger1 = 1;
            const byte finger2 = 3;
            const byte finger3 = 7;
            const byte finger4 = 2;
            const byte finger5 = 5;          

            switch (hand)
            {
                case Hand.Left: braille.Add(left); braille.Add(handConst); break;
                case Hand.Right: braille.Add(right); braille.Add(handConst); break;
                case Hand.Undefined:
                    Logger.Log(string.Format("{0}.AddFinger() was called with illegal parameter hand={1}",className, hand.ToString())); break;
            }

            switch (finger)
            {
                case 1: braille.Add(finger1); break;
                case 2: braille.Add(finger2); break;
                case 3: braille.Add(finger3); break;
                case 4: braille.Add(finger4); break;
                case 5: braille.Add(finger5); break;
                default:
                    Logger.Log(string.Format("{0}.AddFinger() was called with illegal parameter finger={1}",className, finger.ToString())); break;
            }

        }


        public void AddInterval(int size)
        {
            const byte second   = 12;
            const byte third    = 44;
            const byte fourth   = 60;
            const byte fifth    = 20;
            const byte sixth    = 52;
            const byte seventh  = 18;
            const byte eight    = 36;

            switch (size)
            {
                case 2: braille.Add(second); break;
                case 3: braille.Add(third); break;
                case 4: braille.Add(fourth); break;
                case 5: braille.Add(fifth); break;
                case 6: braille.Add(sixth); break;
                case 7: braille.Add(seventh); break;
                case 8: braille.Add(eight); break;
                default:
                    Logger.Log(string.Format("{0}.AddInterval() was called with illegal parameter size={1}", className,size.ToString())); break;
            }


        }

        // TO DO: ********************************************************

        //public void AddConstant(Constant constant)
        //{ }


        /// <summary>
        /// Converts the internal contents from Music-Braille to a Litterary-Braille with the samme 
        /// Braille representation.
        /// This is needed to cheat the Braille ReaderList to represent MusicBraille values
        /// iven if it does not know of their existance.
        /// </summary>
        public override string ToString()
        {
            return "";
        }
    }
}
