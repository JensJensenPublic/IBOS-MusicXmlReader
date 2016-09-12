using System.Xml;

namespace MusicXmlReaderUI
{

    /// <summary>
    /// Base class for GlissandoElement, SlurElement, TiedElement, SlideElement and TupletElement 
    /// </summary>
    abstract class StartStopContinueElement : Element
    {
        public enum StartStopContinueTypeEnum { Undefinded, Start, Stop, Continue };
        StartStopContinueTypeEnum startStopContinueType;
        int numberLevel = 1; // MusicXml default value

        /// <summary>
        /// Private constructor, used by the Crate() method
        /// </summary>
        /// <param name="node"></param>
        protected StartStopContinueElement(XmlNode node)
        {
            // Dig out attributes
            foreach (XmlAttribute a in node.Attributes)
            {
                switch (a.Name)
                {
                    case "type":
                        {
                            switch (a.Value)
                            {
                                case "start": startStopContinueType = StartStopContinueTypeEnum.Start; break;
                                case "stop": startStopContinueType = StartStopContinueTypeEnum.Stop; break;
                                case "continue": startStopContinueType = StartStopContinueTypeEnum.Continue; break;
                                default:
                                    Logger.Log(string.Format("StartStopContinueTypeEnum: Unknown attribute value '{0}'", a.Value));
                                    startStopContinueType = StartStopContinueTypeEnum.Undefinded; break;
                            }
                        }
                        break;

                    case "number":
                        Utilities.Parse(a.Value, ref numberLevel, 1, 6, "SlurElement:", false);
                        break;

                    // Explicitly ignire the following
                    case "relative-x": break;
                    case "relative-y": break;

                    default:
                        Logger.Log(string.Format("SlurElement: Unknown attribute name '{0}'", a.Name));
                        break;
                }
            }
        }

        protected string Localize(StartStopContinueTypeEnum slurType)
        {
            switch (slurType)
            {
                case StartStopContinueTypeEnum.Undefinded: return "Udefineret";
                case StartStopContinueTypeEnum.Start: return "Start";
                case StartStopContinueTypeEnum.Stop: return "Slut";
                case StartStopContinueTypeEnum.Continue: return "Fortsæt";
                default:
                    Logger.Log(string.Format("StartStopContinueElement.Localize: Unexpected value of StartStopContinueType: '{0}'", slurType.ToString()));
                    return "";
            }
        }


        public StartStopContinueTypeEnum StartStopContinueType
        {
            get
            {
                return startStopContinueType;
            }
        }

        public int NumberLevel
        {
            get
            {
                return numberLevel;
            }
        }        
    }
}

