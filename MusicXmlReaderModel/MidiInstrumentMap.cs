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


        /// <summary>
        /// Using this function as is will cause all vitual instruments to be played at Midi Channel 1, which will affect the over all timing
        /// in a very inpleasant way. This problem must be fixed before Pitched virtual instruments can be handled !!!
        /// </summary>
        /// <param name="virtualInstrumentElement"></param>
        /// <returns></returns>
        public static  PitchedMidiInstrumentEnum GetPitchedMidiInstrument(VirtualInstrumentElement virtualInstrumentElement)
        {
            string functionName = "GetPitchedMidiInstrument";

            return PitchedMidiInstrumentEnum.AcousticGrandPiano;

            //if ((null == virtualInstrumentElement) || (null == virtualInstrumentElement.VirtualName))
            //{

            //    Logger.LogOnce(string.Format("{0}.{1}: Unnamed virtual is mapped to {2}",
            //        className, functionName, PitchedMidiInstrumentEnum.AcousticGrandPiano));
            //    return PitchedMidiInstrumentEnum.AcousticGrandPiano;
            //}

            //string instrumentName = virtualInstrumentElement.VirtualName.Replace(" ", "").ToLower(); // Simpler lookup !
            //switch (instrumentName)
            //{
            //    case "altosaxophone": return PitchedMidiInstrumentEnum.AltoSax;
            //    case "tenorsaxophone": return PitchedMidiInstrumentEnum.TenorSax;
            //    case "baritonesaxophone": return PitchedMidiInstrumentEnum.BaritoneSax;
            //    case "trumpet": return PitchedMidiInstrumentEnum.Trumpet;
            //    case "trombone": return PitchedMidiInstrumentEnum.Trombone;
            //    case "classicalguitar": return PitchedMidiInstrumentEnum.AcousticGuitarNylon;
            //    case "'orchestralpercussion": return PitchedMidiInstrumentEnum.Woodblock;
            //    default:
            //        Logger.LogOnce(string.Format("{0}.{1}: Unsupported virtual instrument={2} is mapped to {3}",
            //            className, functionName, virtualInstrumentElement.VirtualName, PitchedMidiInstrumentEnum.AcousticGrandPiano));
            //        return PitchedMidiInstrumentEnum.AcousticGrandPiano;
            //}

        }



    }
}
