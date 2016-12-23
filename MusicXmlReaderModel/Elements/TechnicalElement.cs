using System.Xml;
using System.Collections.Generic;

namespace MusicXmlReaderModel
{

    /// <summary>
    /// https://usermanuals.musicxml.com/MusicXML/Content/EL-MusicXML-technical.htm
    /// </summary>
    public class TechnicalElement : Element
    {
        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private TechnicalElement()
        { }
                
        /// <summary>
        /// Private constructor, used by the Crate() method
        /// </summary>
        /// <param name="node"></param>
        private TechnicalElement(XmlNode node)
        {
            const string functionName = "TechnicalElement";
            List<string> technicals = new List<string>();
            foreach (XmlNode n in node.ChildNodes)
            {
                technicals.Add(n.Name); // For now we just collect the technicals !
                Logger.LogOnce(string.Format("{0}: {1}", functionName, n.Name)); break;               
            }
        }

        public static TechnicalElement Create(XmlNode node)
        {
            return new TechnicalElement(node);
        }

        public override string ToString() // To be localized when implemented
        {
            return string.Format("");
        }
    }
}

