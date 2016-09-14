using System.Xml;

namespace MusicXmlReaderUI
{
    class RestElement
    {

        private string measureAttributeValue = "";

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
            if (0 == node.Attributes.Count)
            {
                //Model.Log(string.Format("RestElement: No attributes found"));
            }
            // Dig out attributes
            foreach (XmlAttribute a in node.Attributes)
            {
                Logger.LogOnce(string.Format("RestElement: Attribute.Name={0} Attribute.Value={1}", a.Name, a.Value));
                switch (a.Name)
                {
                    case "measure":
                        measureAttributeValue = a.Value;
                        break;
                }
            }
        }

        public string MeasureAttributeValue
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


        public override string ToString()
        {
            return string.Format("Pause");
        }
    }
}
