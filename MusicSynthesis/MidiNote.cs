using NAudio.Midi;

namespace JSJ.MusicSynthesis
{
    // These definitions  in Step must match the MIDI definitions:
    // http://www.midi.org/techspecs/midimessages.php 
    // http://www.midi.org/techspecs/gm1sound.php


    /// <summary>
    /// Represents a language-neutral representation of the 12 chromativ steps in an octave
    /// </summary>
    public enum ChromaticStep { C = 0, Cis = 1, Des = 1, D = 2, Dis = 3, Es = 3, E = 4, F = 5, Fis = 6, Ges = 6, G = 7, Gis = 8, As = 8, A = 9, Bb = 10, B = 11, NumberOfSteps = 12 };

    /// <summary>
    /// Represents a language-neutral representation of the 7 tones in an octave
    /// </summary>
    public enum FullToneStep  { C = 0,                   D = 2,                  E = 4, F = 5,                   G = 7,                  A = 9,          B = 11 };


    public enum Interval
    {
        Unison = 0, MinorSecond = 1, MajorSecond = 2, MinorThird = 3, MajorThird = 4, Fourth = 5,
        Tritone = 6, PerfectFifth = 7, AugmentedFifth = 8, MinorSixth = 8, MajorSixth = 9, DiminishedSeventh = 9, MinorSeventh = 10, MajorSeventh = 11, Octave = 12, MinorNinth = 13, Ninth = 14, // ??
        Eleventh = 17,  // ??
        Thirteenth = 21 // ??
    };

    //public enum ChordType { Major, Minor, Augmented, Major6, Minor6, Major7, Minor7, Aug7, FullDim7, Major7maj, MinorMajor, Dom9, Major9, Minor9, Dom11, Major11, Minor11, Dom13,Major13, Minor13, Dim, HalfDim7, Sus2, Sus4, UnImplemented };

    public class MidiNote
    {
        int channelCode;
        private byte[] startCommand;



        /// <summary>
        ///  Constructor, primarily used for playing notes from a MusicXml description
        /// </summary>
        /// <param name="step"></param>
        /// <param name="alter"></param>
        /// <param name="octave"></param>
        /// <param name="velocity"></param>
        /// <param name="midiChannel"></param>
        /// <param name="midiOut"></param>
        public MidiNote(string step, int alter, int octave, int velocity, int midiChannel, MidiOut midiOut)
        {
            CommonConstructor(GetChromaticStep(step), alter, octave, velocity, Interval.Unison, midiChannel, midiOut);
        }


        public MidiNote(string step, int alter, int octave, int velocity, MidiOut midiOut)
        {
            CommonConstructor(GetChromaticStep(step), alter, octave, velocity, Interval.Unison, 1, midiOut);
        }

        // New Code
        public MidiNote(ChromaticStep step, int alter, int octave, int velocity, Interval interval)
        {
            CommonConstructor(step, alter, octave, velocity, interval, 1, null); // Default: MidiOut=null  midiChannel=1
        }

        // New Code
        public MidiNote(ChromaticStep step, int octave, int velocity, Interval interval)
        {
            CommonConstructor(step, 0, octave, velocity, interval, 1,null); // Default: MidiOut=null  midiChannel=1
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
            int alteredStep = ((int)chromaticStep + alterValue) % (int)ChromaticStep.NumberOfSteps;
            return (ChromaticStep)alteredStep;
        }

        private const string invalidArgumentFormat = "{0}={1} is an invalid argument";

        static private string GetFlatString(string step)
        {
            switch (step)
            {
                case "C": return "Ces";
                case "D": return "Des";
                case "E": return "Es";
                case "F": return "Fes";
                case "G": return "Ges";
                case "A": return "As";
                case "B": return "Bb";
                default:
                    throw new System.ArgumentException(string.Format(invalidArgumentFormat,"step",step));
            }
        }

        static private string GetSharpString(string step)
        {
            switch (step)
            {
                case "C": return "Cis";
                case "D": return "Dis";
                case "E": return "Eis";
                case "F": return "Fis";
                case "G": return "Gis";
                case "A": return "Ais";
                case "B": return "Bis"; // TO DO Check!!
                default:
                    throw new System.ArgumentException(string.Format(invalidArgumentFormat, "step", step));
            }
        }


        /// <summary>
        /// Returns the musically corrrect altered value for alter values in {-1,0, +1}
        /// </summary>
        /// <param name="step"></param>
        /// <param name="alter"></param>
        /// <returns></returns>
        public static string GetChromaticString(string step, string alter)
        {
            int alterValue = GetAlterValue(alter);
            switch (alterValue)
            {  
                case -1: return GetFlatString(step);
                case  0: return step;
                case +1: return GetSharpString(step);
                default:
                    throw new System.ArgumentException(string.Format(string.Format(invalidArgumentFormat, "alter", alter)));
            }     
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
        private void CommonConstructor(ChromaticStep step, int alter, int octave, int velocity, Interval interval, int midiChannel,MidiOut midiOut)
        {
            this.channelCode = (midiChannel - 1) % 16; 
            startCommand = new byte[3];
            startCommand[0] = (byte) (0x90 + channelCode); // Command "Start"
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
            stopCommand[0] = (byte) (0x80 + channelCode); // Command STOP
            stopCommand[1] = this.startCommand[1];
            stopCommand[2] = this.startCommand[2];
            midiOut.SendBuffer(stopCommand);
        }
    }    
}
