using System.Xml;

namespace MusicXmlReaderUI
{
    public class ScorePartElement : Element
    {

        public string partNumber = "";
        public string partName = "";
        public ScoreInstrumentElement scoreInstrumentElement;
        public MidiInstrumentElement midiInstrumentElement;


        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private ScorePartElement()
        { }



        /// <summary>
        /// Private constructor, used by the Crate() method
        /// </summary>
        /// <param name="node"></param>
        private ScorePartElement(XmlNode node)
        {

            // Dig out attributes
            foreach (XmlAttribute a in node.Attributes)
            {
                switch (a.Name)
                {
                    case "id":
                        partNumber = a.Value;
                        break;
                }
            }

            // Dig out elements
            foreach (XmlNode n in node.ChildNodes)
            {
                switch (n.Name)
                {
                    case "part-name": partName = n.InnerText; break;
                    case "score-instrument":
                        scoreInstrumentElement = ScoreInstrumentElement.Create(n);
                        break;                
                    case "midi-instrument":
                        midiInstrumentElement = MidiInstrumentElement.Create(n);
                        break;        
                }
            } 
            
        }

        public static ScorePartElement Create(XmlNode node)
        {
            return new ScorePartElement(node);
        }

        public override string ToString()
        {
            return string.Format("Stemme {0} = {1} TODO: Fill in the rest!", partNumber, partName);  
        }
    }
}
