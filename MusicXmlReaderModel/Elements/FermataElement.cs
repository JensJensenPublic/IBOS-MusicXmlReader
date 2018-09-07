using System.Xml;
using System.Globalization;

namespace MusicXmlReaderModel
{

    // http://usermanuals.musicxml.com/MusicXML/Content/EL-MusicXML-fermata.htm

    class FermataElement : Element
    {
        string className = "FermataElement";
        public enum FermataTypeEnum { undefined, upright, inverted, angled, square}; // Angled and Square are found in 'C:\Users\Jens\Dropbox\Root\MusicXml sample file archive\LiliPond.org test samples\32a-Notations.xml
        private FermataTypeEnum fermataType = FermataTypeEnum.undefined;
        public FermataTypeEnum FermataType
        {
            get
            {
                return fermataType;
            }
        }

        private FermataTypeEnum GetFermataType(string s)
        {
            string function = "GetFermataType";
            switch (s)
            {
                case "upright": return FermataTypeEnum.upright;
                case "inverted": return FermataTypeEnum.inverted;
                case "angled": return FermataTypeEnum.angled;
                case "square": return FermataTypeEnum.square;
                default:
                    Logger.LogOnce(string.Format("{0}: Found unexpected fermata value: {1}", function, s));
                    return FermataTypeEnum.undefined;
            }
        }


    /// <summary>
    /// To force the use of the Create() method
    /// </summary>
        private FermataElement(XmlNode node)
        {
            const string functionName = "FermataElement";
        

            // Dig out attributes
            foreach (XmlAttribute a in node.Attributes)
            {
                switch (a.Name)
                {
                    case "type": fermataType = GetFermataType(a.Value); break;
                    case "default-x":
                    case "default-y":
                    case "relative-x":
                    case "relative-y":
                    case "font-family":
                    case "font-style":
                    case "font-size":
                    case "font-weight":
                    case "color":  break; // Explicitly ignore graphic attributes
                    default: Logger.LogOnce(string.Format("{0}: Unexpected fermata attribute {1}", functionName, a.Name)); break;                    
                }
            }

            // Dig out elements
            foreach (XmlNode n in node.ChildNodes)
            {
                switch (n.Name)
                {
                    case "#text":   if ("normal" != n.Value)
                                    {  Logger.LogOnce(string.Format("{0}.{1}: Unexpected text element found: Value={2}",
                                        className, functionName, n.Value)); 
                                    } break;
                    default:         Logger.LogOnce(string.Format("{0}.{1}: Unexpected child found: Name={2} Value={3}",
                                    className, functionName, n.Name, n.Value)); break;
                }
            }



        }


        public static FermataElement Create(XmlNode node)
        {
            return new FermataElement(node);
        }


        public override string ToString() 
        {
            switch (fermataType)
            {
                case FermataTypeEnum.upright:  return ResourcesForModel.FermataElement_Upright;  // "Fermat";
                case FermataTypeEnum.inverted: return ResourcesForModel.FermataElement_Inverted; // "Omvendt fermat";
                default: return "";
            }
        }
    }
}

