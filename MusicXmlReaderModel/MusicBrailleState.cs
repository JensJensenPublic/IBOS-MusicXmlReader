using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXmlReaderUI
{
    static class MusicBrailleState
    {
        private const int initalValue = 0;
        private static int lastSemiTonesAboveC0 = initalValue;
        private static int lastOctave = initalValue;


        public static void ResetMusicBrailleState()
        {
            NeedOctaveMark(initalValue, initalValue);
        }

        /// <summary>
        /// Implements the algorithm described in the Music Braille standard:
        /// If same octave a new octave mark is needed if the distance  > a fifth ( > 7 semitones)
        /// If different octaves a new octave mark is needed if the distance  > a triad ( > 4 semitones)
        /// </summary>
        /// <param name="newOctave"></param>
        /// <param name="newSemiTonesAboveC0"></param>
        /// <returns></returns>
        public static bool NeedOctaveMark(int newOctave, int newSemiTonesAboveC0)
        {
            bool result = false;
            int distance = Math.Abs(newSemiTonesAboveC0 - lastSemiTonesAboveC0);
            if (lastOctave == newOctave)
            {
                result =  ( distance > 7 ); // 7 semitones is a fifth
            }
            else
            {
                result = ( distance > 4 ); // 4 semitones is a (major) third
            }

            // Update the global state
            lastOctave = newOctave;
            lastSemiTonesAboveC0 = newSemiTonesAboveC0;
            //Logger.Log(string.Format("NeedsOctaveMark({0},{1} with old values {2},{3} returned {4}",
            //    newOctave, newSemiTonesAboveC0, lastOctave, lastSemiTonesAboveC0, result));

            Logger.LogOnce(string.Format("NeedOctaveMark returned {0}", result)); // To get an idea of the number of octave marks

            return result;
        }

    }
}
