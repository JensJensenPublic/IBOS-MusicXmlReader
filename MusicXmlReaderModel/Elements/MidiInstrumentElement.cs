using System.Xml;
using JSJ.MusicSynthesis;

namespace MusicXmlReaderModel
{
    public class MidiInstrumentElement : Element
    {
        // https://usermanuals.musicxml.com/MusicXML/Content/CT-MusicXML-midi-instrument.htm

        private string className = "MidiInstrumentElement";
        private string id;
        private string midiName = "";
        private int midiProgram = 1; // Use Grand Acoustic Piano as default
        private int midiChannel = 1; // Use midi channel 1 as default
        private int midiBank = 0; // Use midi bank 0 as default
        private float midiVolume = 127 ; // Use midi volume 127 as default
        private int midiUnpitchedInstrumentNumber = 0;
        private int pan;
        private int elevation;

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
                    case "midi-name":
                        // The midi-name element corresponds to a ProgramName meta-event within a Standard MIDI File.
                        midiName = n.InnerText; // Pure text, nothing to check 
                        break;  
                    case "midi-channel":
                        // The midi-channel element specifies a MIDI 1.0 channel number ranging from 1 to 16.
                        Utilities.Parse(n.InnerText, ref midiChannel, 1, 16, "MidiInstrumentElement: Invalid value of midi-channel",false);
                        break;
                    case "midi-program":
                        // The midi-program element specifies a MIDI 1.0 program number ranging from 1 to 128.
                        Utilities.Parse(n.InnerText, ref midiProgram, 1, 128, "MidiInstrumentElement: Invalid value of midi-program",false);
                        break;
                    case "midi-bank":
                        // The midi-bank element specifies a MIDI 1.0 bank number ranging from 1 to 16,384.
                        Utilities.Parse(n.InnerText, ref midiBank,1,16384, "MidiInstrumentElement: Invalid value of midi-bank", false);
                        break;
                    case "volume":
                        // The volume element value is a percentage of the maximum ranging from 0 to 100, with decimal values allowed.
                        // This corresponds to a scaling value for the MIDI 1.0 channel volume controller.
                        Utilities.Parse(n.InnerText, ref midiVolume, 0, 100, "MidiInstrumentElement: Invalid value of volume ");                      
                        break;
                    case "pan":
                        // The pan and elevation elements allow placing of sound in a 3-D space relative to the listener. Both are expressed in degrees ranging from -180 to 180.
                        // For pan, 0 is straight ahead, -90 is hard left, 90 is hard right, and -180 and 180 are directly behind the listener.
                        Utilities.Parse(n.InnerText, ref pan, -180, +180, "MidiInstrumentElement: Invalid value of pan ", false);
                        break;
                    case "elevation":
                        // The elevation and pan elements allow placing of sound in a 3-D space relative to the listener. Both are expressed in degrees ranging from -180 to 180.
                        // For elevation, 0 is level with the listener, 90 is directly above, and -90 is directly below.
                        Utilities.Parse(n.InnerText, ref elevation, -180, +180, "MidiInstrumentElement: Invalid value of elevation ", false);
                        break;
                    case "midi-unpitched":
                        // For unpitched instruments, the midi-unpitched element specifies a MIDI 1.0 note number ranging from 1 to 128. It is usually used with MIDI banks for percussion.
                        // Note that MIDI 1.0 note numbers are generally specified from 0 to 127 rather than the 1 to 128 numbering used in this element.
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
            string bankString = (0 == midiBank) ? "" : string.Format("{0}={1}", "MidiBank", midiBank);
            string panString = (0 == pan) ? "" : string.Format("{0}={1}", "MidiPan", pan);
            string elevationString = (0 == elevation) ? "" : string.Format("{0}={1}", "MidiElevation",elevation);
#warning ToDo Localize UnpitchedMidiInstrumentEnum
            int iMidi = MidiUnpitchedInstrumentNumber - 1; // The representation is 1-based in MusicXml , 0-based in Midi
            string unpitchedString =  Pitched ?  "" :  string.Format("{0}={1}(´{2}')", "Unpitched MidiInstrument",iMidi, ((UnpitchedMidiInstrumentEnum)iMidi)).ToString(); // Only if relevant !
            string result = string.Format("{0} {1} {2} {3} {4} {5} {6}",programString,channelString,bankString,volumeString,panString,elevationString,unpitchedString);  
            return result;
        }

    }
}
