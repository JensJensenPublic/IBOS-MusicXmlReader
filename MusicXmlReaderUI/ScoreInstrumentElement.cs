using System.Xml;

namespace MusicXmlReaderUI
{
    public class ScoreInstrumentElement : Element
    {

        string id = "";
        string instrumentSound = "";
        string instrumentName = "";
        string instrumentAbbreviation = "";
        string solo = "";
        string virtualInstrument = "";


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
                    case "instrument-abbreviation":
                        instrumentAbbreviation = n.InnerText;
                        break;
                    case "solo":
                        solo = n.InnerText;
                        break;
                    case "virtual-instrument":
                        virtualInstrument = n.InnerText;                   
                        break;
                    default:
                        Logger.Log(string.Format("ScoreInstrumentElement: Unsupported element {0}", n.InnerText));
                        break;
                        // throw new System.ArgumentException();
                }
            }
        }

        public static ScoreInstrumentElement Create(XmlNode node)
        {
            return new ScoreInstrumentElement(node);
        }

        public override string ToString() // Only used by .cmd version. Not localized
        {
            return (string.Format("ScoreInstrument: Id='{0}' Sound='{1}' Navn='{2}' Forkortelse='{3}' Solo='{4}' VirtualInstrument='{5}'",
                                   id, instrumentSound, instrumentName, instrumentAbbreviation, solo, virtualInstrument));
        }
    }
}
