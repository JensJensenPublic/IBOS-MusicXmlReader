using System.Xml;

namespace MusicXmlReaderModel
{

    //http://usermanuals.musicxml.com/MusicXML/Content/EL-MusicXML-accidental.htm

    public enum AccidentalTypeEnum { unknown,none,natural,flat,sharp,flatFlat,doubleSharp,quarterSharp,quarterFlat,unsupported};

    public class AccidentalElement
    {
        private string className = "AccidentalElement";
        private bool cautionary = false;
        private bool editorial = false;
        private bool parentesis = false;
        private AccidentalTypeEnum accidentalType = AccidentalTypeEnum.none;
        private string rawAccidentalString; // The raw value from the MusicXml file. Only used for error reporting.



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
                case AccidentalTypeEnum.quarterSharp:   return ResourcesForModel.Accidental_quarterSharp;
                case AccidentalTypeEnum.quarterFlat:    return ResourcesForModel.Accidental_quarterFlat;
                case AccidentalTypeEnum.doubleSharp:    return ResourcesForModel.Accidental_doubleSharp;
                case AccidentalTypeEnum.flat:           return ResourcesForModel.Accidental_flat;
                case AccidentalTypeEnum.flatFlat:       return ResourcesForModel.Accidental_flatFlat;
                case AccidentalTypeEnum.natural:        return ResourcesForModel.Accidental_natural;
                case AccidentalTypeEnum.none:           return ResourcesForModel.Accidental_none;
                case AccidentalTypeEnum.sharp:          return ResourcesForModel.Accidental_sharp;
                case AccidentalTypeEnum.unknown:        return ResourcesForModel.Accidental_unknown;
                case AccidentalTypeEnum.unsupported:    return ResourcesForModel.Accidental_unsupported;
                default:
                    Logger.LogOnce(string.Format("{0}.{1} Unsupported value for accidentaltype={2}", className, functionName, rawAccidentalString));
                    return "";        
            }
        }

        public override string ToString()
        {
            // Handle parentesis etc !
            string pre  = ((parentesis) || cautionary) ? "(" : "";
            string post = ((parentesis) || cautionary) ? ")" : "";

            return pre + ToLocalizedString() + post;
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
                                        //Logger.LogOnce(string.Format("{0}.{1}: Attribute Name={2} Value={3}", className,functionName, a.Name, a.Value));
                                        break;

                    case "cautionary":  Utilities.ParseYesNoAttributeValue(functionName, a.Name, a.Value, ref cautionary);
                                        // Logger.LogOnce(string.Format("{0}.{1}: Attribute Name={2} Value={3}", className, functionName, a.Name, a.Value));
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
                rawAccidentalString = value;  // Used for logging of unsupported values        
                switch (value)
                {
                    case "flat":            accidentalType = AccidentalTypeEnum.flat;       break;
                    case "natural":         accidentalType = AccidentalTypeEnum.natural;    break;
                    case "sharp":           accidentalType = AccidentalTypeEnum.sharp;      break;
                    case "doubleflat":
                    case "flat-flat":       accidentalType = AccidentalTypeEnum.flatFlat;   break;
                    case "sharp-sharp":
                    case "double-sharp":    accidentalType = AccidentalTypeEnum.doubleSharp; break;
                    case "quarter-flat":    accidentalType = AccidentalTypeEnum.quarterFlat; break;
                    case "quarter-sharp":   accidentalType = AccidentalTypeEnum.quarterSharp;break;
                    // The following values are mentioned at    https://usermanuals.musicxml.com/MusicXML/Content/ST-MusicXML-accidental-value.htm
                    // but are not implemented yet !
                    case "natural-sharp":
                    case "natural-flat":

                    case "three-quarters-flat":
                    case "three-quarters-sharp":
                    case "sharp-down":
                    case "sharp-up":
                    case "natural-down":
                    case "natural-up":
                    case "flat-down":
                    case "flat-up":
                    case "triple-sharp":
                    case "triple-flat":
                    case "slash-quarter-sharp":
                    case "slash-sharp":
                    case "slash-flat":
                    case "double-slash-flat":
                    case "sharp-1":
                    case "sharp-2":
                    case "sharp-3":
                    case "sharp-4":
                    case "sharp-5":
                    case "flat-1":
                    case "flat-2":
                    case "flat-3":
                    case "flat-4":
                    case "flat-15":
                    case "sori":
                    case "koron":
                        accidentalType = AccidentalTypeEnum.unsupported;
                        Logger.LogOnce(string.Format("{0}.{1}: Unimplemented child: Value={2}", className, functionName, value));
                        break;
                    default:
                        accidentalType = AccidentalTypeEnum.unknown;
                        Logger.LogOnce(string.Format("{0}.{1}: Unexpected child: Value={2}",className,functionName, value));
                        break;
                }

                if (accidentalType != AccidentalTypeEnum.none)
                {
                    // Logger.LogOnce(string.Format("{0}.{1}: Accidentalelement {2} ignored until needed", className,functionName, rawAccidentalString));
                }

            }
        }
    }
}
