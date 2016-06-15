using System.Xml;

namespace MusicXmlReaderUI
{
    public class MidiInstrumentElement : Element
    {
        private string id;
        private int midiProgram;
        private int midiChannel;
        private float midiVolume;
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

        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private MidiInstrumentElement()
        { }

        /// <summary>
        /// Private constructor, used by the Crate() method
        /// </summary>
        /// <param name="node"></param>
        private MidiInstrumentElement(XmlNode node)
        {
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
                        Utilities.Parse(n.InnerText, ref midiChannel, 1, 16, "MidiInstrumentElement: Invalid value of midi-channel");
                        //midiChannel = int.Parse(n.InnerText);
                        //if ((midiChannel < 1) || (MidiChannel > 16))
                        //{
                        //    Model.Log(string.Format("MidiInstrumentElement: Invalid value of midi-channel {0} found", midiChannel));
                        //}
                        break;
                    case "midi-program":
                        Utilities.Parse(n.InnerText, ref midiProgram, 1, 255, "MidiInstrumentElement: Invalid value of midi-program");
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
                }
            }

        }

        public static MidiInstrumentElement Create(XmlNode node)
        {
            return new MidiInstrumentElement(node);
        }

        public override string ToString()
        {
            return (string.Format("Midi-Instrument: Id='{0}' Program='{1}' Kanal='{2}' Volumen='{3}' Pan='{4}'",id, midiProgram, midiChannel, midiVolume,pan));
        }
    }
}
