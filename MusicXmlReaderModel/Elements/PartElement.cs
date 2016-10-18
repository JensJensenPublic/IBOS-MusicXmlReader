using System.Xml;
using System.Globalization;

namespace MusicXmlReaderModel
{

    public class PartElement : Element
    {
        string partId = ""; 

        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private PartElement()
        { }



        /// <summary>
        /// Private constructor, used by the Crate() method
        /// </summary>
        /// <param name="node"></param>
        private PartElement(XmlNode node)
        {
            // Dig out attributes
            foreach (XmlAttribute a in node.Attributes)
            {
                switch (a.Name)
                {
                    case "id":
                        partId = a.Value;
                        break;
                }
            }            
        }

        public string PartId
        {
            get
            {
                return partId;
            }
        }

        public static PartElement Create(XmlNode node)
        {
            return new PartElement(node);
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", ResourcesForModel.PartElement_Name, partId);
        }
    }
}
