using System.Xml;
using JSJ.MusicSynthesis;

namespace MusicXmlReaderModel
{
    public class MidiInstrumentElement : Element
    {
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
                        //midiChannel = int.Parse(n.InnerText);
                        //if ((midiChannel < 1) || (MidiChannel > 16))
                        //{
                        //    Model.Log(string.Format("MidiInstrumentElement: Invalid value of midi-channel {0} found", midiChannel));
                        //}
                        break;
                    case "midi-program":
                        Utilities.Parse(n.InnerText, ref midiProgram, 1, 255, "MidiInstrumentElement: Invalid value of midi-program",false);
                        //midiProgram = int.Parse (n.InnerText);
                        //if ((midiProgram < 1) || (midiProgram > 255))
                        //{
                        //    Model.Log(string.Format("MidiInstrumentElement: Invalid value of midi-program {0} found", midiProgram));
                        //}
                        break;
                    case "volume":
                        Utilities.Parse(n.InnerText, ref midiVolume, 1, 255, "MidiInstrumentElement: Invalid value of volume ");
                        //midiVolume = float.Parse(n.InnerText);
                        //if ((midiProgram < 1) || (midiProgram > 255))
                        //{
                        //    Model.Log(string.Format("MidiInstrumentElement: Invalid value of midi-volume {0} found", midiVolume));
                        //}                        
                        break;
                    case "pan":
                        int pan = 0;
                        Utilities.Parse(n.InnerText, ref pan, -180, +180, "MidiInstrumentElement: Invalid value of pan ",false);
                        //pan = n.InnerText;
                        break;
                    case "midi-unpitched":
                        Utilities.Parse(n.InnerText, ref midiUnpitchedInstrumentNumber, 1, 255, "MidiInstrumentElement: Invalid value of midi-instrument", false);
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
            string programString = string.Format("{0}={1}","program",midiProgram);
            string channelString = string.Format("{0}={1}","channel",midiChannel);
            string volumeString = string.Format("{0}={1}", "volume", midiVolume);
            string panString = (0 == pan) ? "" : string.Format("{0}={1}", "pan",pan);
            string unpitchedString =  Pitched ?  "" :  string.Format("{0}={1}", "Instrument", midiUnpitchedInstrumentNumber); // Only if relevant !
            string result = string.Format("{0} {1} {2} {3} {4}",programString,channelString,volumeString,panString,unpitchedString);  
            return result;
        }

    }
}
