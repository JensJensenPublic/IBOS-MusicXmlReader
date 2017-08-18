using System.Xml;


namespace MusicXmlReaderModel
{
    class MidiDeviceElement
    {
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
                        Utilities.Parse(a.Value, ref port, 1, 16, "",true);
                        Logger.Log(string.Format("{0}.{1}: Attribute: Name={2} value={3}", className, functionName, a.Name, a.Value));
                        break;

                    default:
                        Logger.Log(string.Format("{0}.{1}: Unexpected attribute: Name={2} value={3}", className, functionName, a.Name, a.Value));
                        break;
                }


                string s = a.Name;
            }

            // Dig out elements
            foreach (XmlNode n in node.ChildNodes)
            {
                string e = node.Name;
            }

        }


        public static MidiDeviceElement Create(XmlNode node)
        {
            return new MidiDeviceElement(node);
        }
    }
}
