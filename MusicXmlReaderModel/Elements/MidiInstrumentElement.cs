using System.Xml;
using JSJ.MusicSynthesis;

namespace MusicXmlReaderModel
{
    public class MidiInstrumentElement : Element
    {
        // https://usermanuals.musicxml.com/MusicXML/Content/CT-MusicXML-midi-instrument.htm

        private string className = "MidiInstrumentElement";
        private string id;
        private int midiProgram = 1; // Use Grand Acoustic Piano as default
        private int midiChannel = 1; // Use midi channel 1 as default
        private float midiVolume = 127 ; // Use midi volume 127 as default
        private int midiUnpitchedInstrumentNumber = 0;
        private int pan;

        public bool Pitched
        {
            get
            {
                return (midiUnpitchedInstrumentNumber == (int)UnpitchedMidiInstrumentEnum.Pitched);
            }
        }

        public string Id
        {
            get
            {
                return id
;            }
        }

        public int MidiChannel
        {
            get
            {
                return midiChannel;
            }
        }

        public int MidiProgram
        {
            get
            {
                return midiProgram;
            }
        }

        public float MidiVolume
        {
            get
            {
                return midiVolume;
            }
        }

        public int MidiUnpitchedInstrumentNumber
        {
            get
            {
                return midiUnpitchedInstrumentNumber;
            }
        }


        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private MidiInstrumentElement()
        {
            Logger.Log(string.Format("Creating default MidiInstrumentElement: midiProgram={0} midiChannel={1} midiVolume={2}", midiProgram, midiChannel, midiVolume));
        }

        private MidiInstrumentElement(int midiProgram)
        {
            this.midiProgram = midiProgram;
            this.midiChannel = MidiPitchedChannelMap.GetNextChannel(midiProgram);
            Logger.Log(string.Format("Creating default MidiInstrumentElement: midiProgram={0} midiChannel={1} midiVolume={2}", midiProgram, midiChannel, midiVolume));
        }

        /// <summary>
        /// Private constructor, used by the Crate() method
        /// </summary>
        /// <param name="node"></param>
        private MidiInstrumentElement(XmlNode node)
        {
            string functionName = "MidiInstrumentElement";
            // Dig out attributes
            foreach (XmlAttribute a in node.Attributes)
            {
                switch (a.Name)
                {
                    case "id":
                        id = a.Value;
                        break;
                    default:
                        throw new System.ArgumentException();
                }
            }

            // Dig out elements
            foreach (XmlNode n in node.ChildNodes)
            {
                switch (n.Name)
                {
                    case "midi-channel":
                        Utilities.Parse(n.InnerText, ref midiChannel, 1, 16, "MidiInstrumentElement: Invalid value of midi-channel",false);
                        break;
                    case "midi-program":
                        Utilities.Parse(n.InnerText, ref midiProgram, 1, 128, "MidiInstrumentElement: Invalid value of midi-program",false);
                        break;
                    case "volume":
                        Utilities.Parse(n.InnerText, ref midiVolume, 0, 100, "MidiInstrumentElement: Invalid value of volume ");                      
                        break;
                    case "pan":
                        Utilities.Parse(n.InnerText, ref pan, -180, +180, "MidiInstrumentElement: Invalid value of pan ",false);
                        break;
                    case "midi-unpitched":
                        Utilities.Parse(n.InnerText, ref midiUnpitchedInstrumentNumber, 1, 128, "MidiInstrumentElement: Invalid value of midi-instrument", false);
                        break;
                    default:
                        Logger.Log(string.Format("{0}.{1}: Unimplemented element: InnerText={2} Value={3}", className, functionName, n.Name,n.InnerText));
                        break;
                }
            }

        }

        public static MidiInstrumentElement Create(XmlNode node)
        {
            return new MidiInstrumentElement(node);
        }

        public static MidiInstrumentElement CreateDefault()
        {
            return new MidiInstrumentElement();
        }

        public static MidiInstrumentElement CreateDefault(int midiProgram)
        {
            return new MidiInstrumentElement(midiProgram);
        }

        public override string ToString() // No need for localisation. Not used, or only used for debug messages.
        {
            return (string.Format("Midi-Instrument: Id='{0}' Program={1} Kanal={2} Volumen={3} Pan='{4}' Midi-Unpitched={5}", id, midiProgram, midiChannel, midiVolume, pan, midiUnpitchedInstrumentNumber));
        }

        public string ToUserFriendlyString()
        {
#warning ToDo Localize PitchedMidiInstrumentEnum
            int iProgram = MidiProgram - 0; // The representation is 1-based in MusicXml , 0-based in Midi ???? WHY NOT ???
            string programString = (!Pitched)? "" : string.Format("{0}={1}('{2}')","MidiProgram", iProgram, (PitchedMidiInstrumentEnum)iProgram);
            string channelString = string.Format("{0}={1}","MidiChannel",midiChannel);
            string volumeString = string.Format("{0}={1}", "MidiVolume", (int)(midiVolume + 0.5));
            string panString = (0 == pan) ? "" : string.Format("{0}={1}", "MidiPan",pan);
#warning ToDo Localize UnpitchedMidiInstrumentEnum
            int iMidi = MidiUnpitchedInstrumentNumber - 1; // The representation is 1-based in MusicXml , 0-based in Midi
            string unpitchedString =  Pitched ?  "" :  string.Format("{0}={1}(´{2}')", "Unpitched MidiInstrument",iMidi, ((UnpitchedMidiInstrumentEnum)iMidi)).ToString(); // Only if relevant !
            string result = string.Format("{0} {1} {2} {3} {4}",programString,channelString,volumeString,panString,unpitchedString);  
            return result;
        }

    }
}
