using System.Xml;

namespace MusicXmlReaderUI
{
    class CreatorElement : Element
    {

        string typeValue = "";
        string value = "";

        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private CreatorElement()
        { }


        private string LocalizeCreatorType(string s)
        {
            switch (s)
            {
                case "composer": return "Komponist";
                case "lyricist": return "Tekstforfatter";
                default: return s;
            }
        }


        /// <summary>
        /// Private constructor, used by the Crate() method
        /// </summary>
        /// <param name="node"></param>
        private CreatorElement(XmlNode node)
        {
           
            // Dig out attributes
            foreach (XmlAttribute a in node.Attributes)
            {
                switch (a.Name)
                {
                    case "type":
                        this.typeValue = LocalizeCreatorType(a.Value);                  
                        break;
                }
            }

            this.value = node.InnerText;
        }

        public static CreatorElement Create(XmlNode node)
        {
            return new CreatorElement(node);
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}",typeValue,value );
        }



    }
}
