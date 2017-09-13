using System.Xml;

namespace MusicXmlReaderModel
{
    public class ScoreInstrumentElement : Element
    {
        // https://usermanuals.musicxml.com/MusicXML/Content/CT-MusicXML-score-instrument.htm

        const string className = "ScoreInstrumentElement";
        string id = "";
        string instrumentSound = "";
        string instrumentName = "";
        string instrumentAbbreviation = "";
        string solo = "";
        VirtualInstrumentElement virtualInstrumentElement;

        public string Id
        {
            get
            {
                return id;
            }
        }

        public bool IsVirtualInstrument
        {
            get
            {
                return ((null != virtualInstrumentElement) && (!string.IsNullOrEmpty(virtualInstrumentElement.VirtualName)));
            }
        }

        public VirtualInstrumentElement VirtualInstrumentElement
        {

            get
            {
                return virtualInstrumentElement;
            }
        }


        public string InstrumentName
        {
            get
            {
                string functionName = "InstrumentName";
                string result = "";
                int count = 0;
                if (!string.IsNullOrEmpty(instrumentName)) // Lowest priority
                {
                    result = instrumentName;
                    count++;
                }
                if (IsVirtualInstrument) // Highest priority
                {
                    result = virtualInstrumentElement.VirtualName;
                    count++;                  
                }
                if (0 == count)
                {
                    Logger.LogOnce(string.Format("{0}.{1} No Instrument name found.", className, functionName));
                }
                return result;
            }
        }

        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private ScoreInstrumentElement()
        { }



        /// <summary>
        /// Private constructor, used by the Crate() method
        /// </summary>
        /// <param name="node"></param>
        private ScoreInstrumentElement(XmlNode node)
        {
            const string functionName = "ScoreInstrumentElement";

            // Dig out attributes
            foreach (XmlAttribute a in node.Attributes)
            {
                switch (a.Name)
                {
                    case "id":
                        id = a.Value;
                        break;
                }
            }

            // Dig out elements
            foreach (XmlNode n in node.ChildNodes)
            {
                switch (n.Name)
                {
                    case "instrument-name":
                        instrumentName = n.InnerText;  
                        break;
                    case "instrument-sound":
                        instrumentSound = n.InnerText;
                        break;
                    case "instrument-abbreviation":
                        instrumentAbbreviation = n.InnerText;
                        break;
                    case "solo":
                        solo = n.InnerText;
                        break;
                    case "virtual-instrument":
                        virtualInstrumentElement = VirtualInstrumentElement.Create(n);
                        //virtualInstrument = n.InnerText;
                        //Logger.LogOnce(string.Format("{0}.{1}: Found Unsupported element 'virtual-instrument'='{2}'", className, functionName, virtualInstrument));                   
                        break;
                    default:
                        Logger.Log(string.Format("ScoreInstrumentElement: Unsupported element {0}", n.InnerText));
                        break;
                        // throw new System.ArgumentException();
                }
            }
        }

        public static ScoreInstrumentElement Create(XmlNode node)
        {
            return new ScoreInstrumentElement(node);
        }

        public override string ToString() // Only used by Logger and .cmd version. Not localized!!
        {
            return (string.Format("ScoreInstrument: Id='{0}' Sound='{1}' Name='{2}' Abbeviation='{3}' Solo='{4}' VirtualInstrument={5}",
                                   id, instrumentSound, instrumentName, instrumentAbbreviation, solo, virtualInstrumentElement));
        }

        public string ToUserFriendlyString()
        {
            //scorePartElement.ScoreInstrumentString: id, instrumentSound, instrumentName, instrumentAbbreviation, solo, virtualInstrumentElement;
            // Only show the caption for existing values
            string soundString = string.IsNullOrEmpty(instrumentSound) ? "" : string.Format("{0}='{1}'", "Lyd", instrumentSound);
            string soloString = string.IsNullOrEmpty(solo) ? "" : string.Format("{0}:'{1}'", "Solo", solo);
            string viString = (null == virtualInstrumentElement) ? "" : string.Format("{0}:'{1}'", "Virtual Instrument", virtualInstrumentElement.ToString());
            string result = string.Format("{0} {1} {2}", soundString,soloString,viString);
            return result;
        }

    }
}
