using System.Xml;
using System.Globalization;


namespace MusicXmlReaderModel
{
    class NoteHeadElement
    {

        bool filled = false;
        bool parentheses = false;

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
                    default: Logger.LogOnce(string.Format("{0}: Unknown attribute name={1} with value={2]", functionName, a.Name, a.Value)); break;
                }
            }
        }


        public static NoteHeadElement Create(XmlNode node)
        {
            return new NoteHeadElement(node);
        }


        public override string ToString()
        {
            return string.Format("{0} {1}",
                                filled? ResourcesForModel.NoteHeadElement_Filled : "", // 0
                                parentheses? ResourcesForModel.NoteHeadElement_Parentheses : "" // 1
                                );
        }
    }

}

