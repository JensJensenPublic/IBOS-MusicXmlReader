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


        static readonly private int[][] notes = 
        new int[][]   // C, D, E, F, G, A, B, REST 
        {   new int[] { 25,17,11,27,19,10,26,45 } , // 8th, 128th
            new int[] { 57,49,43,59,51,42,26,39 } , // quarter,64th
            new int[] { 29,21,15,31,55,14,30,37 } , // half,32th
            new int[] { 61,53,47,63,55,46,62,13 } };// whole,16th


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

            int stepIndex = GetStepIndex(step); 
            // byte byte0 =  

            switch (type)
            {
                case "whole": break;
                case "half":  ; break;
                case "quarter":  break;
                case "eighth":  break;
                case "16th":  break;
                case "32nd":  break;
                case "64nd":  break;
                default:
                    // Model.Log(string.Format("LocalizeType({0},{1}) Unknown typeString '{2}'", typeString, modifier, typeString));
                    break;
            }

        }

        // TO DO: ********************************************************

        public void AddOctaveMark(int value)
        { }
        public void AddFinger()
        { }
        public void AddInterval()
        { }
        public void AddConstant()
        { }


        /// <summary>
        /// Converts the internal contents from Music-Braille to a Litterary-Braille with the samme 
        /// Braille representation.
        /// </summary>
        public override string ToString()
        {
            return "";
        }
    }
}
