
using System.Xml;

namespace MusicXmlReaderUI
{
    // http://usermanuals.musicxml.com/MusicXML/Content/EL-MusicXML-instrument.htm

    class InstrumentElement
    {
        private string id;

        public string Id
        {
            get
            {
                return id;
            }
        }

        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private InstrumentElement()
        {
        }

        public static InstrumentElement Create(XmlNode node)
        {
            return new InstrumentElement(node);
        }
        

        /// <summary>
        /// Private constructor, used by the Crate() method
        /// </summary>
        /// <param name="node"></param>
        private InstrumentElement(XmlNode node)
        {
            // Dig out attributes
            foreach (XmlAttribute a in node.Attributes)
            {
                switch (a.Name)
                {
                    case "id": id = a.Value; break;
                    default: Logger.LogOnce(string.Format("InstrumentElement: Unknown attribute: Name={0} Value={1}",a.Name,a.Value)); break;                  
                }
            }
        }

    }
}
