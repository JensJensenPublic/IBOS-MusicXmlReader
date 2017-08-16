using System.Xml;
using System.Globalization;


namespace MusicXmlReaderModel
{

    // https://usermanuals.musicxml.com/MusicXML/Content/CT-MusicXML-notehead.htm

    class NoteHeadElement
    {
        const string className = "NoteHeadElement";
        bool filled = false;
        bool parentheses = false;
        string innerText = "";

        public string InnerText
        {
            get
            {
                return innerText;
            }
        }

        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private NoteHeadElement()
        {
        }


        /// <summary>
        /// Private constructor, used by the Crate() method
        /// </summary>
        /// <param name="node"></param>
        private NoteHeadElement(XmlNode node)
        {
            const string functionName = "NoteHeadElement";
            // Dig out attributes

            foreach (XmlAttribute a in node.Attributes)
            {
                switch (a.Name)
                {
                    case "filled":
                        Utilities.ParseYesNoAttributeValue(functionName, a.Name, a.Value, ref filled); break;
                    case "parentheses":
                        Utilities.ParseYesNoAttributeValue(functionName, a.Name, a.Value, ref parentheses); break;
                    case "font-family":
                    case "font-style":
                    case "font-size":
                    case "font-weight":
                    case "color": break; // Explicitly ignore graphical attributes.
                    default: Logger.LogOnce(string.Format("{0}: Unknown attribute name={1} with value={2}", functionName, a.Name, a.Value)); break;
                }
            }
            innerText = node.InnerText;

            switch (innerText)
            {
                case "slash":
                case "triangle":
                case "diamond":
                case "square":
                case "cross":
                case "x":
                case "circle-x":
                case "inverted triangle":
                case "arrow down":
                case "arrow up":
                case "slashed":
                case "back slashed":
                case "normal":
                case "cluster":
                case "circle dot":
                case "left triangle":
                case "rectangle":
                case "none":
                case "do":
                case "re":
                case "mi":
                case "fa":
                case "fa up":
                case "so":
                case "la":
                case "ti":
                    Logger.LogOnce(string.Format("{0}.{1}: Value='{2}'", className, functionName, innerText));
                    break;
                default:
                    Logger.LogOnce(string.Format("{0}.{1}: Unexpected value='{2}'", className, functionName, innerText));
                    break;

            }
        }


        public static NoteHeadElement Create(XmlNode node)
        {
            return new NoteHeadElement(node);
        }


        public override string ToString()
        {
            return string.Format("{0}{1}{2}",
                                filled? ResourcesForModel.NoteHeadElement_Filled + " " : "", // 0
                                parentheses? ResourcesForModel.NoteHeadElement_Parentheses + " ": "", // 1
                                innerText // 2
                                );
        }
    }

}

