using System.Xml;


namespace MusicXmlReaderModel
{
    class MidiDeviceElement
    {

        // https://usermanuals.musicxml.com/MusicXML/Content/EL-MusicXML-midi-device.htm

        private string className = "MidiDeviceElement";
        private int port;
        public int Port
        {
            get { return port; }
        }

        private string id;
        public string Id
        {
            get { return id; }
        }


        private MidiDeviceElement()
        { }

        private MidiDeviceElement(XmlNode node)
        {
            string functionName = "MidiDeviceElement";
            // dig out attributes
            foreach (XmlAttribute a in node.Attributes)
            {
                switch (a.Name)
                {
                    case "port":
                        // The optional port attribute is a number from 1 to 16 that can be used with the unofficial MIDI port (or cable) meta event. 
                        Utilities.Parse(a.Value, ref port, 1, 16, "",true);
                        // Logger.LogOnce(string.Format("{0}.{1}: Attribute: Name={2} value={3}", className, functionName, a.Name, a.Value));
                        break;

                    case "id":
                        // The optional id attribute refers to the score-instrument assigned to this device.
                        // If missing, the device assignment affects all score-instrument elements in the score-part.
                        id = a.Name;
                        // Logger.LogOnce(string.Format("{0}.{1}: Attribute: Name={2} value={3}", className, functionName, a.Name, a.Value));
                        break;

                    default:
                        Logger.LogOnce(string.Format("{0}.{1}: Unexpected attribute: Name={2} value={3}", className, functionName, a.Name, a.Value));
                        break;
                }


                string s = a.Name;
            }

            // Dig out elements
            foreach (XmlNode n in node.ChildNodes)
            {
                string e = n.Name;
                Logger.LogOnce(string.Format("{0}.{1}: Unexpected element: Name={2} value={3}", className, functionName, n.Name, n.Value));
            }

        }


        public static MidiDeviceElement Create(XmlNode node)
        {
            return new MidiDeviceElement(node);
        }
    }
}
