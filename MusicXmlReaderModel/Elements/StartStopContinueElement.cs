using System.Xml;
using System.Globalization;
using MusicXmlReaderUI;

namespace MusicXmlReaderModel
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
                        Utilities.Parse(a.Value, ref numberLevel, 1, 6, "StartStopContinueElement:", false);
                        break;

                    // Explicitly ignore the following
                    case "relative-x": break;
                    case "relative-y": break;
                    case "bezier-x": break;
                    case "bezier-y": break;
                    case "bezier-x1": break;
                    case "bezier-y1": break;
                    case "bezier-x2": break;
                    case "bezier-y2": break;
                    case "default-x": break;
                    case "default-y": break;
                    case "placement": break;
                    case "orientation": break;
                    case "show-number": break;
                    case "bracket": break;
                    case "color": break;

                    default:
                        Logger.LogOnce(string.Format("StartStopContinueElement: Unknown attribute name '{0}'", a.Name));
                        break;
                }
            }
        }

        protected string Localize(StartStopContinueTypeEnum slurType)
        {
            switch (slurType)
            {
                case StartStopContinueTypeEnum.Undefinded: return ResourcesForModel.StartStopContinueElement_Undefined; // "Udefineret";
                case StartStopContinueTypeEnum.Start: return ResourcesForModel.StartStopContinueElement_Start; // "Start";
                case StartStopContinueTypeEnum.Stop: return ResourcesForModel.StartStopContinueElement_Stop; // "Slut";
                case StartStopContinueTypeEnum.Continue: return ResourcesForModel.StartStopContinueElement_Continue; // "Fortsæt";
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

