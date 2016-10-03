using System.Xml;

namespace MusicXmlReaderUI
{

    /// <summary>
    /// http://usermanuals.musicxml.com/MusicXML/Content/CT-MusicXML-barline.htm
    /// </summary>
    public class BarlineElement : EventElement
    {

        RepeatElement repeatElement;
        EndingElement endingElement; 


        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private BarlineElement()
        {
        }


        /// <summary>
        /// Private constructor, used by the Crate() method
        /// </summary>
        /// <param name="node"></param>
        private BarlineElement(XmlNode node)
        {
            const string functionName = "BarlineElement";
            Logger.LogOnce(string.Format("BarlineElement constructor"));

            // Dig out elements
            foreach (XmlNode n in node.ChildNodes)
            {
                switch (n.Name)
                {
                    case "ending":
                        endingElement = EndingElement.Create(n);
                        Logger.LogOnce(string.Format("{0}: Child element='{1}'", functionName, n.Name)); break;
                    case "repeat":
                        repeatElement = RepeatElement.Create(n);
                        Logger.LogOnce(string.Format("{0}: Child element='{1}'", functionName, n.Name)); break;
                    case "bar-style":
                    case "wavy-line":
                    case "segno":
                    case "coda":
                    case "fermata": 
                        //Logger.LogOnce(string.Format("{0}: Explicitly ignoring child element. Name={1} Value={2} ", functionName, n.Name, n.InnerText));
                        return; // Ignore graphical information that can not be represented in Music Braille anyway       

                    default: Logger.LogOnce(string.Format("{0}: Unknown element ={1} ", functionName, n.Name)); break;
                }
            }   


            // Dig out attributes
            foreach (XmlAttribute a in node.Attributes)
            {
                switch (a.Name)
                {
                    case "":
                        switch (a.Value)
                        {
 
                            default: Logger.Log(string.Format("BarlineeElement: Unexpected attributevalue {0} found", a.Value)); break;
                        }
                        break;

                    default: Logger.Log(string.Format("BarlineElement: Unexpected attribute {0} found", a.Name)); break;
                }
            }
        }

        public static BarlineElement Create(XmlNode node)
        {
            return new BarlineElement(node);
        }
        
        public override string ToString()
        {

            return string.Format("Barline");
        }
    }
}

