using System.Xml;

namespace MusicXmlReaderModel
{
    public class MidiInstrumentElement : Element
    {
        private string className = "MidiInstrumentElement";
        private string id;
        private int midiProgram = 1; // Use Grand Acoustic Piano as default
        private int midiChannel = 1; // Use midi channel 1 as default
        private float midiVolume = 127 ; // Use midi volume 127 as default
        private int midiUnpitched = 1;
        private string pan;

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

        public float MidiUnpitched
        {
            get
            {
                return midiUnpitched;
            }
        }


        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private MidiInstrumentElement()
        {
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
                        pan = n.InnerText;
                        break;
                    case "midi-unpitched":
                        Utilities.Parse(n.InnerText, ref midiUnpitched, 1, 255, "MidiInstrumentElement: Invalid value of midi-instrument", false);
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

        public override string ToString() // No need for localisation. Not used, or only used for debug messages.
        {
            return (string.Format("Midi-Instrument: Id='{0}' Program='{1}' Kanal='{2}' Volumen='{3}' Pan='{4}'", id, midiProgram, midiChannel, midiVolume, pan));
        }
    }
}
