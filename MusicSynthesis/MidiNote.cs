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
    public enum FullToneStep { C = 0, D = 2, E = 4, F = 5, G = 7, A = 9, B = 11 };


    /// <summary>
    /// Maps the identifier to an integer, representing 1/100 of the number of cents that the interval spans
    /// https://en.wikipedia.org/wiki/Interval_(music) lists all the intervals and compound intervals:
    /// </summary>
    public enum Interval
    {
        Unison = 0,          // https://en.wikipedia.org/wiki/Eleventh
        MinorSecond = 1,     // https://en.wikipedia.org/wiki/Eleventh
        MajorSecond = 2,     // https://en.wikipedia.org/wiki/Eleventh
        MinorThird = 3,      // https://en.wikipedia.org/wiki/Eleventh
        MajorThird = 4,      // https://en.wikipedia.org/wiki/Eleventh
        Fourth = 5,          // https://en.wikipedia.org/wiki/Eleventh
        AugmentedFourth = 6, // https://en.wikipedia.org/wiki/Eleventh ( or DiminishedFifth or Tritone)
        PerfectFifth = 7,    // https://en.wikipedia.org/wiki/Eleventh  
        MinorSixth = 8,      // https://en.wikipedia.org/wiki/Eleventh
        MajorSixth = 9,      // https://en.wikipedia.org/wiki/Eleventh
        MinorSeventh = 10,   // https://en.wikipedia.org/wiki/Eleventh
        MajorSeventh = 11,   // https://en.wikipedia.org/wiki/Eleventh
        Octave = 12,         // https://en.wikipedia.org/wiki/Eleventh (or DiminishedNinth)
        MinorNinth = 13,     // https://en.wikipedia.org/wiki/Eleventh (or AugmentedOctave)
        MajorNinth = 14,     // https://en.wikipedia.org/wiki/Eleventh (or DiminishedTenth)
        AugmentedNinth = 15, // https://en.wikipedia.org/wiki/Eleventh (or MinorTenth)
        MajorTenth = 16,     // https://en.wikipedia.org/wiki/Eleventh (or DiminishedEleventh)
        Eleventh = 17,       // https://en.wikipedia.org/wiki/Eleventh (or AugmentedTenth) 
        AugmentedEleventh=18,// https://en.wikipedia.org/wiki/Eleventh (or DiminishedTwelvth )
        Twelfth = 19,        // https://en.wikipedia.org/wiki/Eleventh (or DiminishedThirteenth)
        MinorThirteenth = 20,// https://en.wikipedia.org/wiki/Eleventh (or AugmentedTwelwth)
        MajorThirteenth = 21,// https://en.wikipedia.org/wiki/Eleventh (or DiminishedFourteenth)
        MinorFourteenth = 22,// https://en.wikipedia.org/wiki/Eleventh (or AugmentedThirteenth)
        MajorFourteenth = 23,// https://en.wikipedia.org/wiki/Eleventh (or DiminishedFifteenth
        Fifteenth = 24,      // https://en.wikipedia.org/wiki/Eleventh (or AugmentedFourteenth)
        AugmentedFifteenth=25// https://en.wikipedia.org/wiki/Eleventh
    };



    //public enum ChordType { Major, Minor, Augmented, Major6, Minor6, Major7, Minor7, Aug7, FullDim7, Major7maj, MinorMajor, Dom9, Major9, Minor9, Dom11, Major11, Minor11, Dom13,Major13, Minor13, Dim, HalfDim7, Sus2, Sus4, UnImplemented };

    public class MidiNote
    {
        public static int MidiChannelForUnpitchedInstruments = 10;
        int channelCode;
        private byte[] startCommand;
        object hSynthesizedTone; // Handle to the to currently playing tone representing this MidiNote

        static public bool IsKnownUnpitchedMidiInstrument(UnpitchedMidiInstrumentEnum instrument)
        {
            return ((UnpitchedMidiInstrumentEnum.HighQ <= instrument) && (instrument <= UnpitchedMidiInstrumentEnum.OpenSurdo));
        }


        /// <summary>
        ///  Constructor, primarily used for playing notes from a MusicXml description
        /// </summary>
        /// <param name="step"></param>
        /// <param name="alter"></param>
        /// <param name="octave"></param>
        /// <param name="transpose"></param>
        /// <param name="velocity"></param>
        /// <param name="midiChannel"></param>
        /// <param name="midiOut"></param>
        public MidiNote(ChromaticStep step, int alter, int octave, int transpose, int velocity, int midiChannel, MidiOut midiOut)
        {
            CommonConstructor(step, alter, octave, transpose, velocity, Interval.Unison, midiChannel, midiOut);
        }


        public MidiNote(ChromaticStep step, int alter, int octave, int velocity, MidiOut midiOut)
        {
            CommonConstructor(step , alter, octave, 0, velocity, Interval.Unison, 1, midiOut); // Default transpose=0
        }

        // New Code
        public MidiNote(ChromaticStep step, int alter, int octave, int velocity, Interval interval)
        {
            CommonConstructor(step, alter, octave, 0, velocity, interval, 1, null); // Default: transpose=0 MidiOut=null  midiChannel=1
        }

        // New Code
        public MidiNote(ChromaticStep step, int octave, int velocity, Interval interval)
        {
            CommonConstructor(step, 0, octave,0, velocity, interval, 1,null); // Default: alter=0, transpose=0 MidiOut=null  midiChannel=1
        }

        public MidiNote(UnpitchedMidiInstrumentEnum unpitchedMidiInstrument, int velocity, int midiChannel, MidiOut midiOut)
        {
            UnpitchedConstructor(unpitchedMidiInstrument, velocity, midiChannel, midiOut);
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


        private static ChromaticStep GetChromaticStep(string s)
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
        private void CommonConstructor(ChromaticStep step, int alter, int octave, int transpose, int velocity, Interval interval, int midiChannel,MidiOut midiOut)
        {
            this.channelCode = (midiChannel - 1) % 16;       
            startCommand = new byte[3];
            startCommand[0] = (byte) (0x90 + channelCode); // Command "Start"
            startCommand[1] = (byte)(12 * (octave + 1) + ((int)step + alter) % 12 + (int)interval + transpose) ;
            startCommand[2] = (byte)velocity;
            StartPlaying(midiOut);
        }

        /// <summary>
        /// For unpitched instruments neither step, alter, octave nor transpose are needed !
        /// </summary>
        /// <param name="unpitchedMidiInstrument"></param>
        /// <param name="velocity"></param>
        /// <param name="midiChannel"></param>
        /// <param name="midiOut"></param>
        private void UnpitchedConstructor(UnpitchedMidiInstrumentEnum unpitchedMidiInstrument, int velocity, int midiChannel, MidiOut midiOut)
        {

            // Check for channel=10
            this.channelCode = (midiChannel - 1) % 16;
            startCommand = new byte[3];
            startCommand[0] = (byte)(0x90 + channelCode); // Command "Start"
            startCommand[1] = (byte)((int)unpitchedMidiInstrument % MidiCommand.MumberOfMidiInstruments); // 128 Midi Instruments
            startCommand[2] = (byte)velocity;
            StartPlaying(midiOut);
        }

        public void StartPlaying(MidiOut midiOut)
        {
            if (null == midiOut) return;
#if Windows
            midiOut.SendBuffer(this.startCommand); // The original NAudio 3.rd party component
#elif Android
            hSynthesizedTone =  midiOut.SendBuffer(this.startCommand,hSynthesizedTone); // JSJ local Android implementation
#else
#error "Compiling for unknown platform"
#endif
        }

        public void StopPlaying(MidiOut midiOut)
        {
            if (null == midiOut) return;
            byte[] stopCommand = new byte[3];
            stopCommand[0] = (byte) (0x80 + channelCode); // Command STOP
            stopCommand[1] = this.startCommand[1];
            stopCommand[2] = this.startCommand[2];
#if Windows
            midiOut.SendBuffer(stopCommand);
#elif Android
            midiOut.SendBuffer(stopCommand,hSynthesizedTone);
#else
#error "Compiling for unknown platform"
#endif
        }
    }    
}
