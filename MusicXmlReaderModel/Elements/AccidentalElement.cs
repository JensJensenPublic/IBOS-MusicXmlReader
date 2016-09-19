using System.Xml;

namespace MusicXmlReaderUI
{

    //http://usermanuals.musicxml.com/MusicXML/Content/EL-MusicXML-accidental.htm


    public class AccidentalElement
    {
        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private AccidentalElement()
        {
        }
        
        public static AccidentalElement Create(XmlNode node)
        {
            return new AccidentalElement(node);
        }


        private AccidentalElement(XmlNode node)
        {

            const string functionName = "AccidentalElement constructor";
            // Dig out attributes
            foreach (XmlAttribute a in node.Attributes)
            {
                //switch (a.Name)
                //{
                //}
                Logger.LogOnce(string.Format("{0}: Unexpected  attribute. Name ={1} Value={2}",functionName, a.Name,a.Value));
            }

            // Dig out elements
            foreach (XmlNode child in node.ChildNodes)
            {
                string value = child.Value;         
                switch (value)
                {
                    case "flat":
                    case "natural":
                    case "sharp":
                        break; // Ignore until needed !
                    default:
                        Logger.LogOnce(string.Format("{1}: Unexpected  child: Value={1}",functionName, value));
                        break;
                }
            }
        }
    }
}
