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

        // https://en.wikipedia.org/wiki/Braille_music
        // https://www.rnib.org.uk/sites/default/files/New%20International%20Manual.pdf

        public enum Constant
        {
            FourMeasureRest,
            DoubleBar,
            Dot,
            MusicHyphen,
            Triplet,
            RepeatSign,
            Slur,
            Tie,
        };





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

        /// <summary>
        /// For looking op in the notes array 
        /// </summary>
        /// <param name="step"></param>
        /// <returns></returns>
        private int GetStepIndex(FullToneStep step)
        {
            switch (step)
            {
                case FullToneStep.C: return 0;
                case FullToneStep.D: return 1;
                case FullToneStep.E: return 2;
                case FullToneStep.F: return 3;
                case FullToneStep.G: return 4;
                case FullToneStep.A: return 5;
                case FullToneStep.B: return 6;
                default:
                    Model.Log(string.Format("GetStepIndex({0}) Unknown step '{0}'", step.ToString()));
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
                    Model.Log(string.Format("GetTypeIndex({0}) Unknown typeString '{0}'", type));
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
            Model.Log(string.Format("Getoctavemark({0}) was called with illegal parametervalue octave={0}", octave));
            return new byte[] { 8, 8 };
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="step">One of the 7 fulltone steps</param>
        /// <param name="alter">(-1 for flat)    (0 for no alteration)  (+1 for sharp)  </param>
        /// <param name="octave"></param>
        /// <param name="type">From "whole" to 64nd</param>
        /// <param name="punctured">A puncture added</param>
        public void AddNote(FullToneStep step, int alter,int octave, string type, bool punctured)
        {
            const byte sharp = (byte)41;
            const byte flat = (byte)35;
            const byte dot = (byte)4;

            int stepIndex = GetStepIndex(step);
            int typeIndex = GetTypeIndex(type);        
            byte note = notes[typeIndex,stepIndex]; // Represents pitch within an octave
            byte[] octaveMark = GetOctaveMark(octave);
            braille.Add(note);
            if (0 != alter) braille.Add((alter > 0) ? sharp : flat);
            braille.AddRange(octaveMark);
            if (punctured) braille.Add(dot); 
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
                    Model.Log(string.Format("AddFinger() was called with illegal parameter hand={0}", hand.ToString())); break;
            }

            switch (finger)
            {
                case 1: braille.Add(finger1); break;
                case 2: braille.Add(finger2); break;
                case 3: braille.Add(finger3); break;
                case 4: braille.Add(finger4); break;
                case 5: braille.Add(finger5); break;
                default:
                    Model.Log(string.Format("AddFinger() was called with illegal parameter finger={0}", finger.ToString())); break;
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
                    Model.Log(string.Format("AddInterval() was called with illegal parameter size={0}", size.ToString())); break;
            }


        }

        // TO DO: ********************************************************

        public void AddConstant(Constant constant)
        { }


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
