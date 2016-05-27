using System.Xml;

namespace MusicXmlReaderUI
{
    public class ScorePartElement : Element
    {

        public string partId = ""; // For instance "P1"
        public string partName = ""; // For instance "Soprano"
        public int partNumber; // A unique artificial index  for this part.
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

            //this.partNumber = partNumber;
            // Dig out attributes
            foreach (XmlAttribute a in node.Attributes)
            {
                switch (a.Name)
                {
                    case "id":
                        partId = a.Value;                       
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
                        Model.Log(string.Format(scoreInstrumentElement.ToString())); // Not of interest for the normal user !
                        break;                
                    case "midi-instrument":
                        midiInstrumentElement = MidiInstrumentElement.Create(n);
                        Model.Log(string.Format(midiInstrumentElement.ToString())); // Not of interest for the normal user !
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
            return string.Format("Stemme[{0}] {1} = {2} TODO: Fill in the rest!", partNumber, partId, partName);  
        }
    }
}
