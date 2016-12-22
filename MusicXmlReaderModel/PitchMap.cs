using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXmlReaderModel
{
    static class PitchMap
    {
        // value = 0 : No change
        // value = +1 : Alter = 0
        // value = +2 : Add an opløsningstegn

        static int[,] values =
       {
           //-6,-5,-4,-3,-2,-1, 0,+1,+2,+3,+4,+5,+6 // Fifths value
           //Gb,Db,Ab,Eb,Bb, F, C, G, D, A, E, H,F# // Major key for this fifths value
           //Eb,Bb, F, C, G, D, A, E, H,F#,C#,G#,D# // Minor key for this fifths value

            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}, // C,-1
            { 2, 0, 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 2}, // C
            { 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1,+1, 1}, // C,+1

            { 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}, // D,-1
            { 2, 2, 2, 0, 0, 0, 0, 0, 0, 0, 2, 2, 2}, // D
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1}, // D,+1

            { 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0}, // E,-1
            { 2, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0, 0, 2}, // E
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1}, // E,+1

            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}, // F,-1
            { 0, 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 2, 2}, // F
            { 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1}, // F,+1

            { 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}, // G,-1
            { 2, 2, 0, 0, 0, 0, 0, 0, 0, 2, 2, 2, 2}, // G
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1}, // G,+1

            { 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0}, // A,-1
            { 2, 2, 2, 2, 0, 0, 0, 0, 0, 0, 0, 2, 2}, // A
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1}, // A,+1

            { 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0}, // H,-1
            { 2, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0, 0}, // H
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}  // H,+1

        };



        public static bool Map(PitchElement pitchElement, int fifths)
        {
            if (null == pitchElement) return false;
            if ((pitchElement.Alter < -1) || (pitchElement.Alter > 1)) return false;
            if ((pitchElement.Step == PitchElement.FullStepEnum.Unknown) || (pitchElement.Step == PitchElement.FullStepEnum.Rest)) return false;
            if ((fifths < -6) || (fifths > 6)) return false;





        }





    }
}
