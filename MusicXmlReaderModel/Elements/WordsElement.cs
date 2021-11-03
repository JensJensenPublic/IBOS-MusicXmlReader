using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MusicXmlReaderModel
{




    public class WordsElement : Element
    {
        private string words;
        public string Words { get { return words; } }

        private WordsElement(XmlNode node)
        {
            // Attributes. 
            foreach (XmlAttribute a in node.Attributes)
            { }


            // Dig out elements
            foreach (XmlNode n in node.ChildNodes)
            { }

            words = node.InnerText;

        }

        public override string ToString()
        {
            return (null == words) ? "" :words;
        }

        public static WordsElement Create(XmlNode node)
        {
            return new WordsElement(node);
        }
    }
}
