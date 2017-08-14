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

    // https://www.midi.org/specifications/item/gm-level-1-sound-set
    // On MIDI Channel 10, each MIDI Note number("Key#") corresponds to a different drum sound, as shown below.
    // GM-compatible instruments must have the sounds on the keys shown here.
    // While many current instruments also have additional sounds above or below the range show here,
    // and may even have additional "kits" with variations of these sounds, only these sounds are supported by General MIDI Level 1 devices.
    public enum UnpitchedMidiInstrument
    {
        Pitched = 0,
        AcousticBassDrum = 35,
        BassDrum1 = 36,
        SideStick = 37,
        AcousticSnare = 38,
        HandClap = 39,
        ElectricSnare = 40,
        LowFloorTom = 41,
        ClosedHiHat = 42,
        HighFloorTom = 43,
        PedalHiHat = 44,
        LowTom = 45,
        OpenHiHat = 46,
        LowMidTom = 47,
        HiMidTom = 48,
        CrashCymbal1 = 49,
        HighTom = 50,
        RideCymbal1 = 51,
        ChineseCymbal = 52,
        RideBell = 53,
        Tambourine = 54,
        SplashCymbal = 55,
        Cowbell = 56,
        CrashCymbal2 = 57,
        Vibraslap = 58,
        RideCymbal2 = 59,
        HiBongo = 60,
        LowBongo = 61,
        MuteHiConga = 62,
        OpenHiConga = 63,
        LowConga = 64,
        HighTimbale = 65,
        LowTimbale = 66,
        HighAgogo = 67,
        LowAgogo = 68,
        Cabasa = 69,
        Maracas = 70,
        ShortWhistle = 71,
        LongWhistle = 72,
        ShortGuiro = 73,
        LongGuiro = 74,
        Claves = 75,
        HiWoodBlock = 76,
        LowWoodBlock = 77,
        MuteCuica = 78,
        OpenCuica = 79,
        MuteTriangle = 80,
        OpenTriangle = 81
    };



    //public enum ChordType { Major, Minor, Augmented, Major6, Minor6, Major7, Minor7, Aug7, FullDim7, Major7maj, MinorMajor, Dom9, Major9, Minor9, Dom11, Major11, Minor11, Dom13,Major13, Minor13, Dim, HalfDim7, Sus2, Sus4, UnImplemented };

    public class MidiNote
    {
        int channelCode;
        private byte[] startCommand;
        object hSynthesizedTone; // Handle to the to currently playing tone representing this MidiNote

        static public bool IsKnownUnpitchedMidiInstrument(UnpitchedMidiInstrument instrument)
        {
            return ((UnpitchedMidiInstrument.AcousticBassDrum <= instrument) && (instrument <= UnpitchedMidiInstrument.OpenTriangle));
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

        public MidiNote(UnpitchedMidiInstrument unpitchedMidiInstrument, int velocity, int midiChannel, MidiOut midiOut)
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
        private void UnpitchedConstructor(UnpitchedMidiInstrument unpitchedMidiInstrument, int velocity, int midiChannel, MidiOut midiOut)
        {

            // Check for channel=10
            this.channelCode = (midiChannel - 1) % 16;
            startCommand = new byte[3];
            startCommand[0] = (byte)(0x90 + channelCode); // Command "Start"
            startCommand[1] = (byte)(unpitchedMidiInstrument);
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
