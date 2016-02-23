using System.Xml;

namespace MusicXmlReaderUI
{

    public class ClefElement : Element
    {
        string sign = "";
  
        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private ClefElement()
        { }


        /// <summary>
        /// Private constructor, used by the Crate() method
        /// </summary>
        /// <param name="node"></param>
        private ClefElement(XmlNode node)
        {

            // Dig out elements
            foreach (XmlNode n in node.ChildNodes)
            {
                switch (n.Name)
                {
                    case "sign": sign = n.InnerText; break;
                }
            }
        }

        public static ClefElement Create(XmlNode node)
        {
            return new ClefElement(node);
        }

        public override string ToString()
        {
            return string.Format("{0}-Nøgle", sign);
        }
    }
}
