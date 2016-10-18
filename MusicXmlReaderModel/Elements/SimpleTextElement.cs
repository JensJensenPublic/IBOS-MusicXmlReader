using System.Xml;
using MusicXmlReaderUI;

namespace MusicXmlReaderModel
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
        private SimpleTextElement(XmlNode node,string name)
        {
            this.name = (null == name) ? node.InnerText : name;
            text = node.InnerText;
        }

        public static SimpleTextElement Create(XmlNode node)
        {
            return new SimpleTextElement(node,null);
        }

        public static SimpleTextElement Create(XmlNode node,string text)
        {
            return new SimpleTextElement(node,text);
        }

        public override string ToString() // No need to localize
        {
            return string.Format("{0}: {1}", name, text);
        }
    }
}
