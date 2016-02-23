using System.Xml;

namespace MusicXmlReaderUI
{
    class ScorePartwiseElement : Element
    {
        private string version="";

        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private ScorePartwiseElement()
        { }


        /// <summary>
        /// Private constructor, used by the Crate() method
        /// </summary>
        /// <param name="node"></param>
        private ScorePartwiseElement(XmlNode node)
        {

            foreach (XmlAttribute a in node.Attributes)
            {
                switch (a.Name)
                {
                    case "version":
                        version = a.Value;
                        break;
                }
            }
        }

        public static ScorePartwiseElement Create(XmlNode node)
        {
            return new ScorePartwiseElement(node);
        }

        public override string ToString()
        {
            return string.Format("Score-partwise. Version={0}", version);
        }
    }
}

