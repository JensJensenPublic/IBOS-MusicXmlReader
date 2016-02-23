using System.Xml;

namespace MusicXmlReaderUI
{
    public class ScoreInstrumentElement : Element
    {

        string id = "";
        string instrumentSound = "";
        string instrumentName = "";


        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private ScoreInstrumentElement()
        { }



        /// <summary>
        /// Private constructor, used by the Crate() method
        /// </summary>
        /// <param name="node"></param>
        private ScoreInstrumentElement(XmlNode node)
        {
            
            // Dig out attributes
            foreach (XmlAttribute a in node.Attributes)
            {
                switch (a.Name)
                {
                    case "id":
                        id = a.Value;
                        break;
                }
            }

            // Dig out elements
            foreach (XmlNode n in node.ChildNodes)
            {
                switch (n.Name)
                {
                    case "instrument-name":
                        instrumentName = n.InnerText;  
                        break;
                    case "instrument-sound":
                        instrumentSound = n.InnerText;
                        break;

                    default:
                        throw new System.ArgumentException();
                }
            }
        }

        public static ScoreInstrumentElement Create(XmlNode node)
        {
            return new ScoreInstrumentElement(node);
        }
        
        public override string ToString()
        {
            return (string.Format("Node: {0} {1} {2}", id, instrumentSound, instrumentName));    
        }
    }
}
