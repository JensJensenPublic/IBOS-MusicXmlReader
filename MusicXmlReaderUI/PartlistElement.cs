using System.Collections.Generic;
using System.Xml;

namespace MusicXmlReaderUI
{
    class PartlistElement : Element
    {

        List<ScorePartElement> scorePartElements = new List<ScorePartElement>();

        public int NumberOfParts()
        {
            return scorePartElements.Count;
        }

        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private PartlistElement()
        { }
        

        /// <summary>
        /// Private constructor, used by the Crate() method
        /// </summary>
        /// <param name="node"></param>
        private PartlistElement(XmlNode node)
        {


            // Dig out attributes
            foreach (XmlAttribute a in node.Attributes)
            {
                // None expected!
                throw new System.ArgumentException();
            }

            // Dig out elements
            foreach (XmlNode n in node.ChildNodes)
            {
                switch (n.Name)
                {
                    case "score-part":
                        this.scorePartElements.Add(ScorePartElement.Create(n));
                        break;
                    //case "part-name": partName = n.InnerText; break;
                    //case "score-instrument":
                    //    // TO DO: Fill in
                    //    break;
                    //case "midi-instrument":
                    //    // TO DO: Fill in
                    //    break;
                    default:
                        throw new System.ArgumentException();
                }
            }

        }

        public static PartlistElement Create(XmlNode node)
        {
            return new PartlistElement(node);
        }

        public override string ToString()
        {
            return string.Format("Partituret indeholder {0} stemmer:" ,scorePartElements.Count);
        }

        public string[] ToStrings()
        {
            List<string> list = new List<string>();
            foreach (ScorePartElement spe in scorePartElements)
            {
                list.Add(string.Format("Stemme: {0} ({1})",spe.partName,spe.partId));
                list.Add(string.Format("   {0}",spe.scoreInstrumentElement.ToString()));
                list.Add(string.Format("   {0}",spe.midiInstrumentElement.ToString()));
            }
            // Convert from List to Array:
            string[] strings = new string[list.Count];
            for (int i = 0; (i < list.Count); i++)
            {
                strings[i] = list[i];
            }
            return strings;            
        }
        
    }
}
