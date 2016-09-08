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


        //static readonly private int[][] notes =
        //new int[][]   // C, D, E, F, G, A, B, REST 
        //{   new int[] { 25,17,11,27,19,10,26,45 } , // 8th, 128th
        //    new int[] { 57,49,43,59,51,42,26,39 } , // quarter,64th
        //    new int[] { 29,21,15,31,55,14,30,37 } , // half,32th
        //    new int[] { 61,53,47,63,55,46,62,13 } };// whole,16th

        static readonly private byte[,] notes = new byte[,] {
            { 25, 17, 11, 27, 19, 10, 26, 45 },     // 8th, 128th
            { 57, 49, 43, 59, 51, 42, 26, 39 },     // quarter,64th
            { 29, 21, 15, 31, 55, 14, 30, 37 },     // half,32th
            { 61, 53, 47, 63, 55, 46, 62, 13 } };   // whole,16th

        static readonly private byte[] rests = new byte[]
        {
            45, // 8th, 128th
            39, // quarter,64th
            37, // half,32th
            13, // whole,16th
        };

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

        /// <summary>
        /// For looking op in the notes array 
        /// </summary>
        /// <param name="step"></param>
        /// <returns></returns>
        private int GetStepIndex(string step)
        {
            switch (step)
            {
                case "C": return 0;
                case "D": return 1;
                case "E": return 2;
                case "F": return 3;
                case "G": return 4;
                case "A": return 5;
                case "B": return 6;
                default:
                    Logger.Log(string.Format("GetStepIndex({0}) Unknown step '{0}'", step));
                    return -1;            
            }
        }

        private int GetTypeIndex(string type)
        {
            switch (type)
            {
                case "whole": return 3;
                case "half": return 2;
                case "quarter": return 1;
                case "eighth": return 0;
                case "16th": return 3;
                case "32nd": return 2;
                case "64nd": return 1;
                case "128nd": return 0;
                default:
                    Logger.Log(string.Format("GetTypeIndex({0}) Unknown typeString '{0}'", type));
                    return -1;
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
            Logger.Log(string.Format("Getoctavemark({0}) was called with illegal parametervalue octave={0}", octave));
            return new byte[] { 8, 8 };
        }

        
        public void AddNotationsBeforeNote(NotationsElement notations) // Some notations are added Before the note itself
        {
            if (null == notations) return;
            if (null == notations.Articulations) return;
            const string function = "BrailleBuilder.AddNotationsBeforeNote";
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
            const string function = "BrailleBuilder.AddNotationsAfterNote";
            Logger.LogOnce(string.Format("{0}: Unimplemented NotationElement:{1}", function, elementName));
        }

        public void AddNotationsAfterNote(NotationsElement notations)  // Some notations are added After the note itself
        {
            // Actually we dont know what is added before and what is added after.
            // This method is primarily used for logging unimplemented notations !
            if (null == notations) return;
            LogUninplementedNotationElement(notations.SlurElement, "SlurElement"); // Only log first occurrance
            LogUninplementedNotationElement(notations.TiedElement, "TiedElement"); // Only log first occurrance
            LogUninplementedNotationElement(notations.SlideElement, "SlideElement"); // Only log first occurrance
            LogUninplementedNotationElement(notations.GlissandoElement, "GlissandoElement"); // Only log first occurrance
            LogUninplementedNotationElement(notations.TupletElement, "TupletElement"); // Only log first occurrance
            LogUninplementedNotationElement(notations.ArpeggiateElement, "ArpeggiateElement");

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
        public void AddNote(string step, int alter, int octave, string type, bool punctured) // ********************* FIX ! Temp signature
        {
            int stepIndex = GetStepIndex(step);
            int typeIndex = GetTypeIndex(type);        
            byte note = notes[typeIndex,stepIndex]; // Represents pitch within an octave
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
        public void AddRest(string type, bool punctured) // ********************* FIX ! Temp signature
        {
            int typeIndex = GetTypeIndex(type);
            if ((typeIndex < 0) || (typeIndex >= rests.Length))
            {
                Logger.Log(string.Format("BrailleBuilder.AddRest: Skipping invalid type: {0}", type));
            }
            else
            {
                byte rest = rests[typeIndex];
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
                    Logger.Log(string.Format("AddFinger() was called with illegal parameter hand={0}", hand.ToString())); break;
            }

            switch (finger)
            {
                case 1: braille.Add(finger1); break;
                case 2: braille.Add(finger2); break;
                case 3: braille.Add(finger3); break;
                case 4: braille.Add(finger4); break;
                case 5: braille.Add(finger5); break;
                default:
                    Logger.Log(string.Format("AddFinger() was called with illegal parameter finger={0}", finger.ToString())); break;
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
                    Logger.Log(string.Format("AddInterval() was called with illegal parameter size={0}", size.ToString())); break;
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
