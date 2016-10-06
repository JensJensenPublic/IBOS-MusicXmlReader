using System.Xml;

namespace MusicXmlReaderUI
{
    class RestElement
    {
        // http://usermanuals.musicxml.com/MusicXML/Content/EL-MusicXML-rest.htm

        private bool measureAttributeValue = false;

        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private RestElement()
        {
        }


        /// <summary>
        /// Private constructor, used by the Crate() method
        /// </summary>
        /// <param name="node"></param>
        private RestElement(XmlNode node)
        {
            const string functionName = "RestElement";
            if (0 == node.Attributes.Count)
            {
                //Model.Log(string.Format("RestElement: No attributes found"));
            }
            // Dig out attributes
            foreach (XmlAttribute a in node.Attributes)
            {
  
                switch (a.Name)
                {
                    case "measure": Utilities.ParseYesNoAttributeValue(functionName, a.Name, a.Value, ref measureAttributeValue); break;
                    case "default-x":
                    case "default-y":
                        break; // Explicitly ignore some graphical attributes 

                    default:
                        Logger.LogOnce(string.Format("RestElement: Attribute.Name={0}", a.Name)); break;
                }
            }
        }

        public bool MeasureAttributeValue
        {
            get
            {
                return measureAttributeValue;
            }
        }

        public static RestElement Create(XmlNode node)
        {
            return new RestElement(node);
        }


        public override string ToString() // LOCALIZE
        {
            return string.Format("Pause");
        }
    }
}
