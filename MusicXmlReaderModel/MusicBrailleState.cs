using System;

namespace MusicXmlReaderModel
{
    static class MusicBrailleState
    {
        private const int initalValue = 0;
        private static int lastSemiTonesAboveC0 = initalValue;
        private static int lastOctave = initalValue;
        private static BrailleBuilderForMusic bbDynamics = BrailleBuilderForMusic.Create(0); // For holding dynamics until immediately before next octavemark to help the interpreter

        /// <summary>
        /// Used for saving Wedge information and dynamics information until the appearance of the next Octave Mark
        /// This makes  it possible for the interpretator to use the Octave mark as a terminator (As well as using the ToText symbol)
        /// </summary>
        public static BrailleBuilderForMusic BbDynamics { get { return bbDynamics; } }

        public static void ClearDynamics()
        {
            bbDynamics = BrailleBuilderForMusic.Create(0);
        }

        /// <summary>
        /// Resets the state variables used deciding if Octave marks are needed to force generation of an octave mark for the next note.
        /// Must be called every time the normal sequence of note generation is broken, for instance
        /// 1) At the start of each part
        /// 2) After use of interval notation
        /// 3) After use of InAccord notation
        /// </summary>
        public static void ResetMusicBrailleState()
        {
            // Logger.LogCF("");
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

            //Logger.LogOnce(string.Format("NeedOctaveMark returned {0}", result)); // To get an idea of the number of octave marks

            return result;
        }

    }
}
