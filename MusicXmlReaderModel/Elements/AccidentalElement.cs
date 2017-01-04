using System.Xml;

namespace MusicXmlReaderModel
{

    //http://usermanuals.musicxml.com/MusicXML/Content/EL-MusicXML-accidental.htm


    public class AccidentalElement
    {
        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private AccidentalElement()
        {
        }
        
        public static AccidentalElement Create(XmlNode node)
        {
            return new AccidentalElement(node);
        }

        private bool cautionary = false;

        public bool Cautionary
        {
            get
            {
                return cautionary;
            }
        }

        private AccidentalElement(XmlNode node)
        {

            const string functionName = "AccidentalElement.AccidentalElement";
            // Dig out attributes
            foreach (XmlAttribute a in node.Attributes)
            {
                switch (a.Name)
                {
                    case "parentheses": Logger.LogOnce(string.Format("{0}: Attribute Name={1} Value={2}", functionName, a.Name, a.Value));
                                        break;

                    case "cautionary":  Utilities.ParseYesNoAttributeValue(functionName, a.Name, a.Value, ref cautionary);
                                        Logger.LogOnce(string.Format("{0}: Attribute Name={1} Value={2}", functionName, a.Name, a.Value));
                                        break;

                    default: Logger.LogOnce(string.Format("{0}: Unexpected  attribute. Name={1} Value={2}", functionName, a.Name, a.Value)); break;
                }
            }

            // Dig out elements
            foreach (XmlNode child in node.ChildNodes)
            {
                string value = child.Value;         
                switch (value)
                {
                    case "flat":
                    case "natural":
                    case "sharp":
                    case "double-flat":
                    case "double-sharp":
                    case "quarter-flat":
                    case "quarter-sharp":
                        // Logger.LogOnce(string.Format("{0}: Element {1}", functionName,  child.Value));
                        Logger.LogOnce(string.Format("{0}: Accidentalelements are ignored until needed", functionName));
                        break; // Ignore until needed !
                    default:
                        Logger.LogOnce(string.Format("{0}: Unexpected child: Value={1}",functionName, value));
                        break;
                }
            }
        }
    }
}
