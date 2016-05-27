using System.Xml;

namespace MusicXmlReaderUI
{
    public class MidiInstrumentElement : Element
    {
        string id;
        string midiProgram;
        string midiChannel;
        string volume;
        string pan;

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
                        midiChannel = n.InnerText;
                        break;
                    case "midi-program":
                        midiProgram = n.InnerText;
                        break;
                    case "volume":
                        volume = n.InnerText;
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
            return (string.Format("Midi-Instrument: Id='{0}' Program='{1}' Kanal='{2}' Volumen='{3}' Pan='{4}'",id, midiProgram, midiChannel, volume,pan));
        }
    }
}
