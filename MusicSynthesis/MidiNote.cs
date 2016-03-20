using NAudio.Midi;

namespace JSJ.MusicSynthesis
{
    // These definitions  in Step must match the MIDI definitions:
    // http://www.midi.org/techspecs/midimessages.php 
    // http://www.midi.org/techspecs/gm1sound.php


    public enum ChromaticStep { C = 0, Cis = 1, D = 2, Dis = 3, E = 4, F = 5, Fis = 6, G = 7, Gis = 8, A = 9, Bb = 10, B = 11, NumberOfSteps = 12 };   

    public enum Interval
    {
        Unison = 0, MinorSecond = 1, MajorSecond = 2, MinorThird = 3, MajorThird = 4, Fourth = 5,
        Tritone = 6, PerfectFifth = 7, AugmentedFifth = 8, MinorSixth = 8, MajorSixth = 9, DiminishedSeventh = 9, MinorSeventh = 10, MajorSeventh = 11, Octave = 12, MinorNinth = 13, Ninth = 14, // ??
        Eleventh = 17,  // ??
        Thirteenth = 21 // ??
    };

    public enum ChordType { Major, Minor, Augmented, Major6, Minor6, Major7, Minor7, Aug7, FullDim7, Major7maj, MinorMajor, Dom9, Major9, Minor9, Dom11, Major11, Minor11, Dom13,Major13, Minor13, Dim, HalfDim7, Sus2, Sus4, UnImplemented };

    public class MidiNote
    {
        private byte[] startCommand;

        /// <summary>
        /// Constructor, primarily used for playing notes from a MusicXml description
        /// </summary>
        /// <param name="step"></param>
        /// <param name="alter"></param>
        /// <param name="octave"></param>
        /// <param name="velocity"></param>
        /// <param name="midiOut"></param>
        public MidiNote(string step, string alter, string octave, int velocity, MidiOut midiOut)
        {
            CommonConstructor(GetChromaticStep(step), GetAlterValue(alter), int.Parse(octave), velocity, Interval.Unison, midiOut);
        }

        // New Code
        public MidiNote(ChromaticStep step, int alter, int octave, int velocity, Interval interval)
        {
            CommonConstructor(step, alter, octave, velocity, interval, null); // Default: MidiOut = null
        }

        // New Code
        public MidiNote(ChromaticStep step, int octave, int velocity, Interval interval)
        {
            CommonConstructor(step, 0, octave, velocity, interval, null); // Default: MidiOut = null
        }

        /// <summary>
        /// Convert from the string representation step/alter to ChromaticStep representation
        /// </summary>
        /// <param name="step"></param>
        /// <param name="alter"></param>
        /// <returns></returns>
        public static ChromaticStep GetChromaticStep(string step, string alter)
        {
            ChromaticStep chromaticStep = GetChromaticStep(step);
            int alterValue = GetAlterValue(alter);
            // Map "C", "-1" to B
            // Map "B", "+1" to C
            int alteredStep = ((int)chromaticStep + alterValue) % (int) ChromaticStep.NumberOfSteps;
            return (ChromaticStep)alteredStep;
        }

        // New code
        public static ChromaticStep GetChromaticStep(string s)
        {
            switch (s)
            {
                case "C": return ChromaticStep.C;
                case "Cis": return ChromaticStep.Cis;
                case "D": return ChromaticStep.D;
                case "Dis": return ChromaticStep.Dis;
                case "E": return ChromaticStep.E;
                case "F": return ChromaticStep.F;
                case "Fis": return ChromaticStep.Fis;
                case "G": return ChromaticStep.G;
                case "Gis": return ChromaticStep.Gis;
                case "A": return ChromaticStep.A;
                case "Bb": return ChromaticStep.Bb;
                case "B": return ChromaticStep.B;
                //case "H": return ChromaticStep.B;
                default: throw new System.ArgumentException(string.Format("Unknown step:{0}", s));
            }
        }

        // New code
        private static int GetAlterValue(string alter)
        {
            if (string.IsNullOrEmpty(alter))
                return 0;
            else
                return int.Parse(alter);
        }

        // New code
        /// <summary>
        /// NOTE: This function handles the adding of step, alter and octave !!!!
        /// </summary>
        /// <param name="step"></param>
        /// <param name="alter"></param>
        /// <param name="octave"></param>
        /// <param name="velocity"></param>
        /// <param name="interval"></param>
        /// <param name="midiOut"></param>
        private void CommonConstructor(ChromaticStep step, int alter, int octave, int velocity, Interval interval, MidiOut midiOut)
        {
            startCommand = new byte[3];
            startCommand[0] = 0x90; // Command "Start"
            startCommand[1] = (byte)(12 * (octave + 1) + (int)step + alter + (int)interval);
            startCommand[2] = (byte)velocity;
            StartPlaying(midiOut);
        }
        
        public void StartPlaying(MidiOut midiOut)
        {
            if (null == midiOut) return;
            midiOut.SendBuffer(this.startCommand);
       }

        public void StopPlaying(MidiOut midiOut)
        {
            if (null == midiOut) return;
            byte[] stopCommand = new byte[3];
            stopCommand[0] = 0x80; // Command STOP
            stopCommand[1] = this.startCommand[1];
            stopCommand[2] = this.startCommand[2];
            midiOut.SendBuffer(stopCommand);
        }
    }    
}
