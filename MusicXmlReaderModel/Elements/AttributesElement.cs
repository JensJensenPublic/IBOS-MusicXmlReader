using System.Xml;

namespace MusicXmlReaderModel
{

    // http://usermanuals.musicxml.com/MusicXML/Content/EL-MusicXML-attributes.htm

    /// <summary>
    /// 
    /// </summary>
    class AttributesElement : EventElement
    {
        private string className = "AttributesElement";
        // Prevent construction
        private AttributesElement()
        {
        }

        private AttributesElement(XmlNode node)
        {
            string functionName = "AttributesElement";
            // Dig out elements
            foreach (XmlNode child in node.ChildNodes)
            {
                string childName = child.Name;
                switch (childName)
                {
                    case "footnote":
                    case "level":
                    case "divisions":
                    case "key":
                    case "time":
                    case "staves":
                    case "part-symbol":
                    case "instruments":
                    case "":
                    case "clef":
                    case "staff-details":
                    case "transpose":
                    case "directive":
                    case "measure-style":
                        break;  // For the time being we just check the syntax here.
                                // The elements are handled individually by Model.WriteElement
                                // For this reacon Model.WriteElement CONTINUES RECURSION below the attributes element
                    default:
                        Logger.LogOnce(string.Format("{0}.{1}: Unexpected child: Name='{2}' Value='{3}'",
                                                className,functionName, childName, child.Value));
                        break;
                }
            }


        }

        public static AttributesElement Create(XmlNode node)
        {
            return new AttributesElement(node);
        }
    }
}
