using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JSJ.MusicSynthesis;

namespace MusicXmlReaderModel
{
    public static class MidiInstrumentMap
    {
        const string className = "MidiInstrumentMap";

        /// <summary>
        /// Attempts to look up a PITCHED Midi Instrument to replace the (unsupported virtual instrument)
        /// Use Acoustic Grand Piano as a default.
        /// </summary>
        /// <param name="virtualInstrumentElement"></param>
        /// <returns></returns>
        public static  PitchedMidiInstrumentEnum GetPitchedMidiInstrument(VirtualInstrumentElement virtualInstrumentElement)
        {
            string functionName = "GetPitchedMidiInstrument";


            if ((null == virtualInstrumentElement) || (null == virtualInstrumentElement.VirtualName))
            {

                Logger.LogOnce(string.Format("{0}.{1}: Unnamed virtual instrument is mapped to {2}",
                    className, functionName, PitchedMidiInstrumentEnum.AcousticGrandPiano));
                return PitchedMidiInstrumentEnum.AcousticGrandPiano;
            }

            string instrumentName = virtualInstrumentElement.VirtualName.Replace(" ", "").ToLower(); // Simpler lookup !
            switch (instrumentName)
            {
                case "altosaxophone": return PitchedMidiInstrumentEnum.AltoSax;
                case "tenorsaxophone": return PitchedMidiInstrumentEnum.TenorSax;
                case "baritonesaxophone": return PitchedMidiInstrumentEnum.BaritoneSax;
                case "trumpet": return PitchedMidiInstrumentEnum.Trumpet;
                case "trombone": return PitchedMidiInstrumentEnum.Trombone;
                case "classicalguitar": return PitchedMidiInstrumentEnum.AcousticGuitarNylon;
                case "orchestralpercussion": return PitchedMidiInstrumentEnum.Woodblock;
                case "acousticbass": return PitchedMidiInstrumentEnum.AcousticBass;
                case "acousticpiano": return PitchedMidiInstrumentEnum.AcousticGrandPiano;
                case "woodblocks": return PitchedMidiInstrumentEnum.Woodblock;
                default:
                    Logger.LogOnce(string.Format("{0}.{1}: Unsupported virtual instrument={2} is mapped to {3}",
                        className, functionName, virtualInstrumentElement.VirtualName, PitchedMidiInstrumentEnum.AcousticGrandPiano));
                    return PitchedMidiInstrumentEnum.AcousticGrandPiano;
            }

        }


        /// <summary>
        /// Attempts to look up an UNPITCHED Midi Instrument to replace the (unsupported virtual instrument)
        /// Use UnpitchedMidiInstrumentEnum.SideStick as a default.
        /// </summary>
        /// <param name="virtualInstrumentElement"></param>
        /// <returns></returns>
        public static UnpitchedMidiInstrumentEnum GetUnpitchedMidiInstrumentNumber(VirtualInstrumentElement virtualInstrumentElement)
        {
            const string functionName = "GetUnpitchedMidiInstrumentNumber";
            UnpitchedMidiInstrumentEnum result = UnpitchedMidiInstrumentEnum.SideStick;
            if (null != virtualInstrumentElement)
            {
                switch (virtualInstrumentElement.VirtualName)
                {
                    case "Woodblocks":
                        result = UnpitchedMidiInstrumentEnum.LowConga; break;
                    case "Orchestral percussion":
                        result = UnpitchedMidiInstrumentEnum.BassDrum1; break;
                    default:
                        Logger.LogOnce(string.Format("{0}.{1}: Unsupported virtual instrument='{2}'.'{3}'", className, functionName, virtualInstrumentElement.VirtualLibrary, virtualInstrumentElement.VirtualName));
                        break;
                }

            }
            // Logger.LogOnce(string.Format("{0}.{1}: Returned '{2}' for '{3}'.'{4}'", className, functionName, result, virtualInstrumentElement.VirtualLibrary, virtualInstrumentElement.VirtualName));
            return result;
        }


        /// <summary>
        /// Attempts to look up an UNPITCHED Midi Instrument to replace an unsupported unpitched instrument, typically a drum-type instrument
        /// Use UnpitchedMidiInstrumentEnum.SideStick as a default.
        /// </summary>
        /// <param name="scoreInstrumentElement"></param>
        /// <returns></returns>
        public static UnpitchedMidiInstrumentEnum GetUnpitchedMidiInstrumentNumber(ScoreInstrumentElement scoreInstrumentElement)
        {
            const string functionName = "GetUnpitchedMidiInstrumentNumber";
            UnpitchedMidiInstrumentEnum result = UnpitchedMidiInstrumentEnum.SideStick;
            if (null != scoreInstrumentElement)
            {
                switch (scoreInstrumentElement.InstrumentName.Replace(" ", "")) // Ignore spaces
                {
                    // Drums:
                    case "Drum1":
                    case "Drum2":
                    case "Drum3":
                    case "Drum4":
                    case "Drum5":
                        result = UnpitchedMidiInstrumentEnum.BassDrum1; ; break;
                    case "BassDrum1":
                    case "BassDrum2":
                    case "BassDrum3":
                    case "BassDrum4":
                    case "BassDrum5":
                        result = UnpitchedMidiInstrumentEnum.AcousticBassDrum; ; break;
                    case "Drum1Rim":
                    case "Drum2Rim":
                    case "Drum3Rim":
                    case "Drum4Rim":
                    case "Drum5Rim":
                        result = UnpitchedMidiInstrumentEnum.LowConga; break;
                    case "Drum1Buzz":
                    case "Drum2Buzz":
                    case "Drum3Buzz":
                    case "Drum4Buzz":
                    case "Drum5Buzz":
                        result = UnpitchedMidiInstrumentEnum.BassDrum1; break;
                    case "BassDrum1RimKnock":
                    case "BassDrum2RimKnock":
                    case "BassDrum3RimKnock":
                    case "BassDrum4RimKnock":
                    case "BassDrum5RimKnock":
                        result = UnpitchedMidiInstrumentEnum.LowConga; break;

                    // Other percussion instruments
                    case "Spock":
                        result = UnpitchedMidiInstrumentEnum.Claves; break;
                    case "SpockRim":
                        result = UnpitchedMidiInstrumentEnum.Sticks; break;
                    case "Smash":
                        result = UnpitchedMidiInstrumentEnum.Slap; break;
                    case "Zing":
                        result = UnpitchedMidiInstrumentEnum.Vibraslap; break;
                    default:
                        Logger.LogOnce(string.Format("{0}.{1}: Unsupported instrument='{2}'", className, functionName, scoreInstrumentElement.InstrumentName));
                        break;
                }
            }
            // Logger.LogOnce(string.Format("{0}.{1}: Returned '{2}' for ScoreInstrumentName='{3}'",className, functionName, result, scoreInstrumentElement.InstrumentName));

            // More detailed logging
            //            Logger.LogOnce(string.Format("{0}.{1}: Creating unpitched MidiNote for unexpected MidiInstrument={2} MidiProgram={3} ScoreInstrumentName='{4}' Id={5} Result={6} in '{7}'",
            //                className, functionName, midiUnpitchedInstrumentNumber, MidiProgram, scoreInstrumentElement.InstrumentName, scoreInstrumentElement.Id,result, Model.TheStaticXmlFileName));
            return result;
        }


    }
}
