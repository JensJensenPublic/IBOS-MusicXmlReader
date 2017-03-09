using System.Xml;

namespace MusicXmlReaderModel
{
    // https://usermanuals.musicxml.com/MusicXML/Content/CT-MusicXML-slash.htm

    class SlashElement    {

            const string className = "SlashElement";
            bool start = false;
            bool useStems = false;
            bool useDots = false;

            private SlashElement(XmlNode node)
            {

                const string functionName = "SlashElement";

            // Dig out attributes
            foreach (XmlAttribute a in node.Attributes)
            {
                switch (a.Name)
                {
                    case "type": Utilities.ParseStartStopAttributeValue(functionName, a.Name, a.Value, ref start); break;
                    case "use-stems": Utilities.ParseYesNoAttributeValue(functionName, a.Name, a.Value, ref useStems); break;
                    case "use-dots": Utilities.ParseYesNoAttributeValue(functionName, a.Name, a.Value, ref useDots); break;
                    default:
                        Logger.LogOnce(string.Format("{0}.{1} Unexpected attribute. Name={2} Value={3}", className, functionName, a.Name, a.Value));
                        break;
                }
            }

        }

            public bool Start
            {
                get
                {
                    return start;
                }
            }

        public bool UseDots
        {
            get
            {
                return useDots;
            }
        }

        public bool UseStems
        {
            get
            {
                return useStems;
            }
        }

        public override string ToString()
        {
            return ""; // This is pure graphical notation !
        }

        public static SlashElement Create(XmlNode node)
        {
            return new SlashElement(node);
        }
    }
}
