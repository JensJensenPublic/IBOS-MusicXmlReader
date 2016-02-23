using System.Xml;

namespace MusicXmlReaderUI
{
    /// <summary>
    /// For Element simple Element-nodes, only containing a single InnerText
    /// </summary>
    public class SimpleTextElement : Element
    {
        string name = "";
        string text = "";

        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private SimpleTextElement()
        { }
        

        /// <summary>
        /// Private constructor, used by the Crate() method
        /// </summary>
        /// <param name="node"></param>
        private SimpleTextElement(XmlNode node)
        {
            text = node.InnerText;
            name = node.Name;
        }

        public static SimpleTextElement Create(XmlNode node)
        {
            return new SimpleTextElement(node);
        }

        public override string ToString()
        {
            return string.Format("{0}={1}", name, text);
        }
    }
}
