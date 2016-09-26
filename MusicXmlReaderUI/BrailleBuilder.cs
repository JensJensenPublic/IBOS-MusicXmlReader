using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JSJ.MusicSynthesis;



namespace MusicXmlReaderUI
{
    class BrailleBuilder
    {
        // References:
        // Ref.1: http://usermanuals.musicxml.com/MusicXML/Content/EL-MusicXML-articulations.htm
        // Ref.2: https://en.wikipedia.org/wiki/Braille_music
        // Ref.3: https://www.rnib.org.uk/sites/default/files/New%20International%20Manual.pdf

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

        public static readonly byte[] FourMeasureRest = new byte[] { 60, 25, 13 };
        public static readonly byte[] DoubleBar = new byte[] { 35 };
        public static readonly byte Dot = 4;
        public static readonly byte MusicHyphen = 16; // This measure will be continued on the following line
        public static readonly byte Triplet = 6;
        public static readonly byte RepeatSign = 54; // A beat, a half measure or a full measure must be repeated
        public static readonly byte Slur = 9;
        public static readonly byte[] Tie = new byte[] { 8, 9 };
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


        private byte GetStepValue(string step) //  Returns the values for dot 1,2,4,5
        {
            switch (step)
            {
                case "C": return dot1+dot4+dot5; // Pin 1,4,5
                case "D": return dot1+dot5; // Pin 1,5
                case "E": return dot1+dot2+dot4; // Pin 1,2,4,
                case "F": return dot1+dot2+dot4+dot5; // Pin 1,2,4,5
                case "G": return dot1+dot2+dot5;// Pin 1,2,5
                case "A": return dot2+dot4; // Pin 2,4
                case "B": return dot2+dot4+dot5; // Pin 2,4,5
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
            const string function = className + ".AddNotationsBeforeNote";
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
                        Logger.Log(string.Format("{0}: Unknown articulation: '{1}'",function, articulation)); break;
                }
                if (!implemented)
                {
                    Logger.Log(string.Format("{0}: Unimplemented articulation: '{1}'",function, articulation));
                }
            }
        }

        private void LogUninplementedNotationElement(Element element, string elementName)
        {
            if (null == element) return;
            const string function = className + ".AddNotationsAfterNote";
            Logger.LogOnce(string.Format("{0}: Unimplemented NotationElement:{1}", function, elementName));
        }

        public void AddNotationsAfterNote(NotationsElement notations)  // Some notations are added After the note itself
        {
            // Actually we dont know what is added before and what is added after.
            // This method is primarily used for logging unimplemented notations !
            if (null == notations) return;
            LogUninplementedNotationElement(notations.SlurElement, "SlurElement"); 
            LogUninplementedNotationElement(notations.TiedElement, "TiedElement"); 
            LogUninplementedNotationElement(notations.SlideElement, "SlideElement"); 
            LogUninplementedNotationElement(notations.GlissandoElement, "GlissandoElement"); 
            LogUninplementedNotationElement(notations.TupletElement, "TupletElement"); 
            LogUninplementedNotationElement(notations.ArpeggiateElement, "ArpeggiateElement");
            LogUninplementedNotationElement(notations.FermataElement, "FermataElement");
            LogUninplementedNotationElement(notations.OrnamentsElement, "OrnamentsElement");

            // Add other notation elements here asthey are added in the parser.
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="step">One of the 7 fulltone steps</param>
        /// <param name="alter">(-1 for flat)    (0 for no alteration)  (+1 for sharp)  </param>
        /// <param name="octave"></param>
        /// <param name="type">From "whole" to 128nd</param>
        /// <param name="punctured">A puncture added</param>
        //public void AddNote(FullToneStep step, int alter, int octave, string type, bool punctured) // The right signature
        public void AddNote(string step, int alter, int octave, NoteTypeEnum noteDuration, bool punctured) // ********************* FIX ! Temp signature
        {
            byte stepPart = GetStepValue(step);                 //  Returns the values for pin 1,2,4,5
            byte typePart = GetTypeValue(noteDuration);         //  Returns the values for pin 3,6
            byte note = (byte)((int)stepPart | (int)typePart);  //  Logical OR to get all 6 pin values
            byte[] octaveMark = GetOctaveMark(octave);
            braille.Add(note);
            if (0 != alter) braille.Add((alter > 0) ? Sharp : Flat);
            braille.AddRange(octaveMark);
            if (punctured) braille.Add(Dot);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">From "whole" to 128nd</param>
        /// <param name="punctured">A puncture added</param>
        public void AddRest(NoteTypeEnum noteDuration, bool punctured) // ********************* FIX ! Temp signature
        {
            if (NoteTypeEnum.measure == noteDuration)
            {
                Logger.LogOnce(string.Format("{0}.{1}: Unhandled noteduration: {2}", className, "AddRest", noteDuration));
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
