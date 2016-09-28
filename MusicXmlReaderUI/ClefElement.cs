using System.Xml;

namespace MusicXmlReaderUI
{

    public enum ClefEnum {unknown, F,G,C }

    // http://usermanuals.musicxml.com/MusicXML/Content/EL-MusicXML-clef.htm


    public class ClefElement : EventElement
    {
        ClefEnum clef;
        int line;
        int clefOctaveChange;

        public ClefEnum Clef
        {
            get
            {
                return clef;
            }
        }

        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private ClefElement()
        { }

        private string Localize(ClefEnum clef)
        {
            switch (clef)
            {
                case ClefEnum.C: return "C";
                case ClefEnum.G: return "G";
                case ClefEnum.F: return "F";
                default: return "";
            }
        }
        
        /// <summary>
        /// Private constructor, used by the Crate() method
        /// </summary>
        /// <param name="node"></param>
        private ClefElement(XmlNode node)
        {
            const string functionName = "ClefElement()";

            // Dig out elements
            foreach (XmlNode n in node.ChildNodes)
            {
                switch (n.Name)
                {
                    case "sign":
                        switch (n.InnerText)
                        {
                            case "F": clef = ClefEnum.F; break;
                            case "G": clef = ClefEnum.G; break;
                            case "C": clef = ClefEnum.C; break;
                            default: Logger.Log(string.Format("{0}: Element={1} has illegal value={2}", functionName, n.Name, n.InnerText));break;
                        }
                        break;
                    case "line": Utilities.Parse(n.InnerText, ref line, 1, 5, functionName, true); break;
                    case "clef-octace-change": Utilities.Parse(n.InnerText, ref clefOctaveChange, -1, 1, functionName, true); break;
                    default: Logger.Log(string.Format("{0}: Unknown element ={1} ", functionName, n.Name)); break;
                }
            }
        }

        public static ClefElement Create(XmlNode node)
        {
            return new ClefElement(node);
        }

        public override string ToString()
        {
            return string.Format("{0}-Nøgle", Localize(clef));
        }
    }
}
