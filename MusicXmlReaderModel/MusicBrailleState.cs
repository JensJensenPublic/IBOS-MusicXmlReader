using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXmlReaderUI
{
    static class MusicBrailleState
    {
        private const int initalValue = int.MinValue;
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
                result =  ( distance > 4 );
            }
            else
            {
                result = ( distance > 7 );
            }

            // Update the global state
            lastOctave = newOctave;
            lastSemiTonesAboveC0 = newSemiTonesAboveC0;

            return result;
        }

    }
}
