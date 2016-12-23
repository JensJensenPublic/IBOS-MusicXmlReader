using System.Xml;
using System.Globalization;


namespace MusicXmlReaderModel
{
    public enum BarlineLocationEnum {unknown, right, left, middle}; 

    /// <summary>
    /// http://usermanuals.musicxml.com/MusicXML/Content/CT-MusicXML-barline.htm
    /// </summary>
    public class BarlineElement : EventElement
    {
        // If location is left, it should be the first element in the measure, aside from the print, bookmark, and link elements.
        // If location is right, it should be the last element, again with the possible exception of the print, bookmark, and link elements. 
        BarlineLocationEnum location = BarlineLocationEnum.right; // Explicitly mentioned in .htm above !
        RepeatElement repeatElement;
        EndingElement endingElement;
        BarStyleElement barStyleElement;

        public BarlineLocationEnum Location
        {
            get
            {
                return location;
            }
        }

        public RepeatElement RepeatElement
        {
            get
            {
                return repeatElement;
            }
        }

        public EndingElement EndingElement
        {
            get
            {
                return endingElement;
            }
        }

        public BarStyleElement BarStyleElement
        {
            get
            {
                return barStyleElement;
            }
        }


        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private BarlineElement()
        {
        }


        /// <summary>
        /// Private constructor, used by the Crate() method
        /// </summary>
        /// <param name="node"></param>
        private BarlineElement(XmlNode node)
        {
            const string functionName = "BarlineElement";
            Logger.LogOnce(string.Format("{0} constructor",functionName));

            // Dig out elements
            foreach (XmlNode n in node.ChildNodes)
            {
                switch (n.Name)
                {
                    case "ending":
                        endingElement = EndingElement.Create(n);
                        Logger.LogOnce(string.Format("{0}: Child element='{1}'", functionName, n.Name)); break;
                    case "repeat":
                        repeatElement = RepeatElement.Create(n);
                        Logger.LogOnce(string.Format("{0}: Child element='{1}'", functionName, n.Name)); break;
                    case "bar-style":
                        barStyleElement = BarStyleElement.Create(n);
                        Logger.LogOnce(string.Format("{0}: Child element='{1}'", functionName, n.Name)); break;
                    case "wavy-line":
                    case "segno":
                    case "coda":
                    case "fermata": 
                        //Logger.LogOnce(string.Format("{0}: Explicitly ignoring child element. Name={1} Value={2} ", functionName, n.Name, n.InnerText));
                        break; // Ignore graphical information that can not be represented in Music Braille anyway       

                    default: Logger.LogOnce(string.Format("{0}: Unknown element ={1} ", functionName, n.Name)); break;
                }
            }   


            // Dig out attributes
            foreach (XmlAttribute a in node.Attributes)
            {
                switch (a.Name)
                {
                    case "location":
                        switch (a.Value)
                        {
                            case "right": location = BarlineLocationEnum.right; break;
                            case "left": location = BarlineLocationEnum.left; break;
                            case "middle": location = BarlineLocationEnum.middle; break;
                            default: Logger.LogOnce(string.Format("{0}: Attribute Name='{1}' Unexpected attribute Value='{2}'", functionName,a.Name, a.Value)); break;
                        }
                        break;
                    case "segno":
                    case "coda":
                    case "divisions": break; // These 3 attributes may later be used
                    default: Logger.LogOnce(string.Format("{0}: Unexpected attribute '{1}'", functionName,a.Name)); break;
                }
            }
        }

        public static BarlineElement Create(XmlNode node)
        {
            return new BarlineElement(node);
        }


        /// <summary>
        /// This function is currently not used, but localization has been performed anyway.
        /// </summary>
        /// <param name="barlineLocation"></param>
        /// <returns></returns>
        private string LocalizeBarlineLocation(BarlineLocationEnum barlineLocation)
        {
            string functionName = "LocalizeBarlineLocation";
            switch (barlineLocation)
            {
                case BarlineLocationEnum.left: return ResourcesForModel.BarlineElement_Position_left;
                case BarlineLocationEnum.middle: return ResourcesForModel.BarlineElement_Position_middle;
                case BarlineLocationEnum.right: return ResourcesForModel.BarlineElement_Position_right;
                case BarlineLocationEnum.unknown:
                    Logger.LogOnce(string.Format("{0}: Found unknown value {1}", functionName, BarlineLocationEnum.unknown));
                    return ResourcesForModel.BarlineElement_Position_unknown;
                default:
                    Logger.LogOnce(string.Format("{0}: Found undefined value {1}", functionName, barlineLocation.ToString()));
                    return "";
            }
        }


        public override string ToString()
        {

            return string.Format("{0}",ResourcesForModel.BarlineElement);
        }
    }
}

