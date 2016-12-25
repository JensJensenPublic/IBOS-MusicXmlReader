using System.Xml;
using System.Collections.Generic;
using System.Text;

namespace MusicXmlReaderModel
{

    /// <summary>
    /// https://usermanuals.musicxml.com/MusicXML/Content/EL-MusicXML-technical.htm
    /// </summary>
    public class TechnicalElement : Element
    {
        private string className = "TechnicalElement";

        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private TechnicalElement()
        { }

        private List<string> technicals;

        /// <summary>
        /// Private constructor, used by the Crate() method
        /// </summary>
        /// <param name="node"></param>
        private TechnicalElement(XmlNode node)
        {
            const string functionName = "TechnicalElement";
            technicals = new List<string>();
            foreach (XmlNode n in node.ChildNodes)
            {
                technicals.Add(n.Name); // For now we just collect the technicals !
                // Logger.LogOnce(string.Format("{0}: {1}", functionName, n.Name)); break;               
            }
        }

        public static TechnicalElement Create(XmlNode node)
        {
            return new TechnicalElement(node);
        }

        public override string ToString() // To be localized when implemented
        {
            string functionName = "ToString";
            StringBuilder allTecnnicals = new StringBuilder();
            foreach (string s in technicals)
            {
                allTecnnicals.Append(s + " ");
            }
            string result = allTecnnicals.ToString();
            if (result.Length > 0)
            {
                Logger.LogOnce(string.Format("{0}.{1}: Localization is missing for '{2}'",className,functionName,result));
            }
            return result; // TODO: Implement localization !
            // return string.Format("{0}", technicals.ToString()); // TODO: Implement localization !
        }
    }
}

