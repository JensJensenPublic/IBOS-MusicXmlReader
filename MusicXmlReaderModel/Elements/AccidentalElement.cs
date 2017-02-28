using System.Xml;

namespace MusicXmlReaderModel
{

    //http://usermanuals.musicxml.com/MusicXML/Content/EL-MusicXML-accidental.htm

    public enum AccidentalTypeEnum { unknown,none,natural,flat,sharp,flatFlat,doubleSharp};

    public class AccidentalElement
    {
        private string className = "AccidentalElement";
        private bool cautionary = false;
        private bool editorial = false;
        private bool parentesis = false;
        private AccidentalTypeEnum accidentalType = AccidentalTypeEnum.none;



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

  

        public bool Cautionary
        {
            get
            {
                return cautionary;
            }
        }

        public bool Editorial
        {
            get
            {
                return editorial;
            }
        }

        public AccidentalTypeEnum Accidental
        {
            get
            {
                return accidentalType;
            }
        }

        private string ToLocalizedString()
        {
            string functionName = "ToString";
            switch (accidentalType)
            {
                case AccidentalTypeEnum.doubleSharp:    return "";
                case AccidentalTypeEnum.flat:           return "";
                case AccidentalTypeEnum.flatFlat:       return "";
                case AccidentalTypeEnum.natural:        return "";
                case AccidentalTypeEnum.none:           return "";
                case AccidentalTypeEnum.sharp:          return "";
                case AccidentalTypeEnum.unknown:        return "";
                default:
                    Logger.LogOnce(string.Format("{0}.{1} Unsupported value for accidentaltype={2}", className, functionName, accidentalType.ToString()));
                    return "";        
            }
        }

        public override string ToString()
        {
            // Handle parentesis etc !
            return ToLocalizedString();
        }


        private AccidentalElement(XmlNode node)
        {

            const string functionName = "AccidentalElement";
            // Dig out attributes
            foreach (XmlAttribute a in node.Attributes)
            {
                switch (a.Name)
                {
                    case "parentheses": Utilities.ParseYesNoAttributeValue(functionName,a.Name, a.Value, ref parentesis);
                                        Logger.LogOnce(string.Format("{0}.{1}: Attribute Name={2} Value={3}", className,functionName, a.Name, a.Value));

                                        break;

                    case "cautionary":  Utilities.ParseYesNoAttributeValue(functionName, a.Name, a.Value, ref cautionary);
                                        Logger.LogOnce(string.Format("{0}.{1}: Attribute Name={2} Value={3}", className, functionName, a.Name, a.Value));
                                        break;

                    case "editorial":   Utilities.ParseYesNoAttributeValue(functionName, a.Name, a.Value, ref editorial);
                                        Logger.LogOnce(string.Format("{0}.{1}: Attribute Name={2} Value={3}", className, functionName, a.Name, a.Value));
                                        break;

                    default: Logger.LogOnce(string.Format("{0}.{1}: Unexpected  attribute. Name={2} Value={3}", className, functionName, a.Name, a.Value)); break;
                }
            }

            // Dig out elements
            foreach (XmlNode child in node.ChildNodes)
            {
                string value = child.Value;         
                switch (value)
                {
                    case "flat":            accidentalType = AccidentalTypeEnum.flat;       break;
                    case "natural":         accidentalType = AccidentalTypeEnum.natural;    break;
                    case "sharp":           accidentalType = AccidentalTypeEnum.sharp;      break;
                    case "flat-flat":       accidentalType = AccidentalTypeEnum.flatFlat;   break;
                    case "double-sharp":    accidentalType = AccidentalTypeEnum.doubleSharp; break;
                    default:
                        Logger.LogOnce(string.Format("{0}.{1}: Unexpected child: Value={1}",className,functionName, value));
                        break;
                }

                if (accidentalType != AccidentalTypeEnum.none)
                {
                    Logger.LogOnce(string.Format("{0}.{1}: Accidentalelement {2} ignored until needed", className,functionName, this.ToString()));
                }

            }
        }
    }
}
