using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXmlReaderModel
{

    /// <summary>
    /// Simple lookup table used for removing Alternations already implicitly given by the current key.
    /// Is implemented (and required) for generation correct Music Braille representation,
    /// but could also be used in a similar way for generating a graphical representation!
    /// Implementation restrictions:
    /// Alter must be -1,0 or +1
    /// Fifths must be in the interval [-7 .. +7]
    /// </summary>
    static class PitchMap
    {

        static string className = "PitchMap";

        // value = 0 : No change
        // value = +1 : Alter = 0
        // value = +2 : Add an opløsningstegn

        static readonly int[,] values =
       {
           //-7,-6,-5,-4,-3,-2,-1, 0,+1,+2,+3,+4,+5,+6,+7 // Fifths value
           //Cb,Gb,Db,Ab,Eb,Bb, F, C, G, D, A, E, H,F#,C# // Major key for this fifths value
           //Ab,Eb,Bb, F, C, G, D, A, E, H,F#,C#,G#,D#,A# // Minor key for this fifths value

            { 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}, // C,-1
            { 2, 2, 0, 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 2, 2}, // C
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1,+1, 1, 1}, // C,+1

            { 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}, // D,-1
            { 2, 2, 2, 2, 0, 0, 0, 0, 0, 0, 0, 2, 2, 2, 2}, // D
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1}, // D,+1

            { 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0}, // E,-1
            { 2, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0, 0, 2, 2}, // E
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1}, // E,+1

            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}, // F,-1
            { 2, 0, 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2}, // F
            { 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1}, // F,+1

            { 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}, // G,-1
            { 2, 2, 2, 0, 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 2}, // G
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1}, // G,+1

            { 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}, // A,-1
            { 2, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0, 0, 2, 2, 2}, // A
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1}, // A,+1

            { 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0}, // B,-1
            { 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0, 0, 2}, // B
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1}  // B,+1

        };


        private static int GetIndex(PitchElement.FullStepEnum step)
        {
            switch (step)
            {
                case PitchElement.FullStepEnum.C: return 0;
                case PitchElement.FullStepEnum.D: return 1;
                case PitchElement.FullStepEnum.E: return 2;
                case PitchElement.FullStepEnum.F: return 3;
                case PitchElement.FullStepEnum.G: return 4;
                case PitchElement.FullStepEnum.A: return 5;
                case PitchElement.FullStepEnum.B: return 6;
                default: return -1;
            }
        }


        /// <summary>
        /// Computes the "alter" value and "natural" sign  (Danish: "opløsningstegn") to be shown taking in account the 
        /// current key (represented by the "fifths" parameter)
        /// </summary>
        /// <param name="pitchElement">The pitchElement to work on</param>
        /// <param name="fifths">The current key, represented as a position on the circle of fifths</param>
        /// <param name="alter">The resulting Alter value</param>
        /// <param name="natural">The resulting natural sign</param>
        /// <returns></returns>
        public static bool Map(PitchElement pitchElement, int fifths, ref int alter, ref bool natural)
        {
            const string functionName = "Map";
            if (null == pitchElement)
            {
                Logger.Log(String.Format("{0}.{1} PitchElement is null", className, functionName));
                return false;
            }
            if ((pitchElement.Alter < -1) || (pitchElement.Alter > 1))
            {
                Logger.LogOnce(String.Format("{0}.{1} Unsupported value of PitchElement.Alter = {2}", className, functionName, pitchElement.Alter));
                return false;
            }
            int stepIndex = GetIndex(pitchElement.Step);
            if (-1 == stepIndex)
            {
                Logger.Log(String.Format("{0}.{1} Unsupported value of PitchElement.Step = {2}", className, functionName, pitchElement.Step.ToString()));
                return false;
            }
            if ((fifths < -7) || (fifths > 7))
            {
                Logger.Log(String.Format("{0}.{1} Unsupported value of fifths = {2}", className, functionName, fifths));
                return false;
            }  
            
            // Convert to indices for a simple table lookup          
            int rowIndex = (stepIndex * 3) + 1 + pitchElement.Alter;
            int colIndex = fifths + 7;

            // Get a code representing what to do 
            int change = values[rowIndex, colIndex];

            switch (change)
            {
                case 0: alter = pitchElement.Alter; natural = false; break; // Use the original step and alter.
                case 1: alter = 0; natural = false; break;                  // Use the original step, but no alteration
                case 2: alter = 0; natural = true; break;                   // Use the original step, but no alteration and add a natural-sign
                default:
                    Logger.Log(String.Format("{0}.{1} Unsupported value found at values[{2},{3}] = {4}", className, functionName, rowIndex,colIndex,change));
                    return false;
            }
            return true;
        }

    }
}
