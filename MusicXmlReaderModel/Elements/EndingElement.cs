using System.Xml;

namespace MusicXmlReaderUI
{

    /// <summary>
    /// http://usermanuals.musicxml.com/MusicXML/Content/CT-MusicXML-ending.htm
    /// This class strongly resembles StartStopContinueElement, but not sompletely, so we define it from scratch:
    /// </summary>
    public class EndingElement : Element
    {
        public enum EndingElementTypeEnum { Undefinded, Start, Stop, Discontinue };
        private EndingElementTypeEnum endingElementType;
        private int numberLevel = 1; // MusicXml default value

        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private EndingElement() 
        {
        }


        /// <summary>
        /// Private constructor, used by the Crate() method
        /// </summary>
        /// <param name="node"></param>
        protected EndingElement(XmlNode node)
        {
            const string functionName = "EndingElement";
            Logger.LogOnce(string.Format("{0} constructor",functionName));
            // Dig out attributes
            foreach (XmlAttribute a in node.Attributes)
            {
                switch (a.Name)
                {
                    case "type":
                        {
                            switch (a.Value)
                            {
                                case "start": endingElementType = EndingElementTypeEnum.Start; break;
                                case "stop": endingElementType = EndingElementTypeEnum.Stop; break;
                                case "discontinue": endingElementType = EndingElementTypeEnum.Discontinue; break;
                                default:
                                    Logger.Log(string.Format("{0}: Attribute Name={1} Unknown attribute Value='{2}'",functionName,a.Name, a.Value));
                                    endingElementType = EndingElementTypeEnum.Undefinded; break;
                            }
                        }
                        break;

                    case "number":
                        Utilities.Parse(a.Value, ref numberLevel, 1, 2, "StartStopContinueElement:", false);
                        // TO DO: also accept more complicated structures, such as "1,2"
                        break;

                    // Explicitly ignore the following
                    case "print-object": break;
                    case "default-x": break;
                    case "default-y": break;
                    case "relative-x": break;
                    case "relative-y": break;
                    case "font-family": break;
                    case "font-size": break;
                    case "font-weight": break;
                    case "color": break;
                    case "end-length": break;
                    case "text-x": break;
                    case "text-y": break;
                    default:
                        Logger.LogOnce(string.Format("{0}: Unknown attribute name='{1}'", functionName, a.Name));
                        break;
                }
            }
        }

        protected string Localize(EndingElementTypeEnum endingType)
        {
            const string functionName = "EndingElementType.Localize";
            switch (endingType)
            {
                case EndingElementTypeEnum.Undefinded: return "Udefineret";
                case EndingElementTypeEnum.Start: return "Start";
                case EndingElementTypeEnum.Stop: return "Slut";
                case EndingElementTypeEnum.Discontinue: return "Stop";
                default:
                    Logger.Log(string.Format("{0}: Unexpected value='{1}'", functionName, endingType.ToString()));
                    return "";
            }
        }


        public int NumberLevel
        {
            get
            {
                return numberLevel;
            }
        }

        internal EndingElementTypeEnum EndingElementType
        {
            get
            {
                return endingElementType;
            }
        }



        public static EndingElement Create(XmlNode node)
        {
            return new EndingElement(node);
        }

    }
}


