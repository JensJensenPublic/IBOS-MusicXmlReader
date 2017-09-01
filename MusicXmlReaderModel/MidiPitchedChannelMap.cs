using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JSJ.MusicSynthesis;

namespace MusicXmlReaderModel
{

    /// <summary>
    /// Maps different instruments numbers (for midi instruments to virtual instruments) to different channels 
    /// </summary>
    public static class MidiPitchedChannelMap
    {
        static private string className = "MidiPitchedChannelMap";
        static private int[] channels;
        static private int nextFreeChannel;
        static private int firstChannel = 1;
        static private int lastChannel = 16;
        static private int unPitchedChannel = 10;

        /// <summary>
        /// Resets the assignment of channels 
        /// </summary>
        public static void Reset()
        {
            channels = new int[lastChannel + 1];
            for (int i = 0; (i < channels.Length); i++)
            { 
                channels[i] = 0;
            }
            nextFreeChannel = firstChannel; // Start numbering at 1
        }

        public static int GetNextChannel(int instrumentNumber)
        {
            string functionName = "GetNextChannel";

            // Attempt to reuse existing channel for same instrument
            for (int i = 0; (i < channels.Length); i++)
            {
                if (instrumentNumber == channels[i])
                {
                    return i;
                }
            }

            // Instrument not in use. Assign a new channel
            int result = nextFreeChannel;
            channels[nextFreeChannel] = instrumentNumber;
            nextFreeChannel++;
            if (10 == nextFreeChannel)
            {
                nextFreeChannel++; // Skip channel 10, which is reserved for unpitched instruments.
            }
            if ( nextFreeChannel > lastChannel )
            {
                nextFreeChannel = firstChannel; // Start reusing channels
            }
            PitchedMidiInstrumentEnum pitchedMidiInstrumentEnum = (PitchedMidiInstrumentEnum)instrumentNumber;
            Logger.LogOnce(string.Format("{0}.{1}: Using Channel={2} for Instrument={3} ({4})", className, functionName, result,  instrumentNumber, pitchedMidiInstrumentEnum.ToString()));
            return result;
        }

    }
}
