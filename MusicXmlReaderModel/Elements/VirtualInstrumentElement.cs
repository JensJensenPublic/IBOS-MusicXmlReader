using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;


namespace MusicXmlReaderModel
{


    /// <summary>
    /// https://usermanuals.musicxml.com/MusicXML/Content/EL-MusicXML-virtual-instrument.htm
    /// </summary>
    class VirtualInstrumentElement : Element
    {
        const string className = "VirtualInstrumentElement";

        private string virtualLibrary;
        private string virtualName;

        public string VirtualLibrary
        {
            get
            {
                return virtualLibrary;
            }
        }

        public string VirtualName
        {
            get
            {
                return virtualName;
            }
        }


        /// <summary>
        /// Prevent construction
        /// </summary>
        private VirtualInstrumentElement()
        {

        }

        private VirtualInstrumentElement(XmlNode node)
        {
            string functionName = "VirtualInstrumentElement";

            // Dig out elements
            foreach (XmlNode n in node.ChildNodes)
            {
                switch (n.Name)
                {
                    case "virtual-library": virtualLibrary = n.InnerText; break;
                    case "virtual-name":    virtualName = n.InnerText; break;
                    default:
                        Logger.LogOnce(string.Format("{0}.{1}: Unexpected Element. Name={2} InnerText={3} ", className, functionName, n.Name, n.InnerText));
                        break;
                }
                if ((null != virtualLibrary) && (null != virtualName))
                {
                    Logger.LogOnce(string.Format("{0}.{1}: VirtualLibrary='{2}' VirtualName='{3}'", className, functionName, virtualLibrary, virtualName));
                }
            }
        }


        static public VirtualInstrumentElement Create(XmlNode node)
        {
            return new VirtualInstrumentElement(node);
        }

    }
}
