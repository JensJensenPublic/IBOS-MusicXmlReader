using System.Xml;

namespace MusicXmlReaderUI
{

    public enum ClefEnum {unknown, F,G,C,percussion,TAB,jianpu,none }

    // http://usermanuals.musicxml.com/MusicXML/Content/EL-MusicXML-clef.htm
    // https://en.wikipedia.org/wiki/Numbered_musical_notation


    public class ClefElement : EventElement
    {
        ClefEnum clef;
        int line;
        int clefOctaveChange;
        string className = "ClefElement";

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

        private string Localize(ClefEnum clef) // LOCALIZE
        {
            const string functionName = "Localize"; 
            switch (clef)
            {
                case ClefEnum.C: return "C-Nøgle";
                case ClefEnum.G: return "G-Nøgle";
                case ClefEnum.F: return "F-nøgle";
                case ClefEnum.percussion: return "slagtøj";
                case ClefEnum.TAB: return "TAB";
                case ClefEnum.jianpu: return "jianpu";
                case ClefEnum.none: return "Ingen nøgle";

                default: Logger.LogOnce(string.Format("{0}.{1} Illegal Clef:{2}",className,functionName,clef.ToString()));
                    return "";
            }
        }
        
        /// <summary>
        /// Private constructor, used by the Crate() method
        /// </summary>
        /// <param name="node"></param>
        private ClefElement(XmlNode node)
        {
            const string functionName = "ClefElement()";
            bool signFound = false;

            // Dig out elements
            foreach (XmlNode n in node.ChildNodes)
            {
                switch (n.Name)
                {
                    case "sign":
                        switch (n.InnerText)
                        {
                            case "F": clef = ClefEnum.F; signFound = true; break;
                            case "G": clef = ClefEnum.G; signFound = true; break;
                            case "C": clef = ClefEnum.C; signFound = true; break;
                            case "percussion": clef = ClefEnum.percussion; signFound = true; break;
                            case "TAB": clef = ClefEnum.TAB; signFound = true; break;
                            case "jianpu": clef = ClefEnum.jianpu; signFound = true; break;
                            case "none": clef = ClefEnum.none; signFound = true; break;
                            default: Logger.LogOnce(string.Format("{0}: Element={1} has illegal value={2}", functionName, n.Name, n.InnerText));break;
                        }
                        break;
                    case "line": Utilities.Parse(n.InnerText, ref line, 1, 5, functionName, true); break;
                    case "clef-octave-change": Utilities.Parse(n.InnerText, ref clefOctaveChange, -1, 1, functionName, true); break;
                    default: Logger.LogOnce(string.Format("{0}: Unknown element ={1} ", functionName, n.Name)); break;
                }
            }
            if (!signFound)
            {
                Logger.LogOnce(string.Format("{0}.{1}: No 'sign' child element found", className, functionName)); 
            }

        }

        public static ClefElement Create(XmlNode node)
        {
            return new ClefElement(node);
        }

        public override string ToString() // LOCALIZE
        {
            return string.Format("{0}", Localize(clef));
        }
    }
}
