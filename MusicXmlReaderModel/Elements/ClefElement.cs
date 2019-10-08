using System.Xml;
using System.Globalization;

namespace MusicXmlReaderModel
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
        int staffNumber = 1;
        ScorePartElement scorePartElement;

        public ClefEnum Clef
        {
            get
            {
                return clef;
            }
        }

        public int StaffNumber { get { return staffNumber; } }

        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private ClefElement()
        { }

        private string LocalizeClef(ClefEnum clef)
        {
            const string functionName = "LocalizeClef";
            switch (clef)
            {
                case ClefEnum.C: return ResourcesForModel.ClefElement_C_Key; // "C-Nøgle";
                case ClefEnum.G: return ResourcesForModel.ClefElement_G_Key; // "G-Nøgle";
                case ClefEnum.F: return ResourcesForModel.ClefElement_F_Key; // "F-nøgle";
                case ClefEnum.percussion: return ResourcesForModel.ClefElement_Percussion; // "slagtøj";
                case ClefEnum.TAB: return ResourcesForModel.ClefElement_TAB; // "TAB";
                case ClefEnum.jianpu: return ResourcesForModel.ClefElement_jianpu; // "jianpu";
                case ClefEnum.none: return ResourcesForModel.ClefElement_None; // "Ingen nøgle";
                default:
                    Logger.LogOnce(string.Format("{0}.{1} Illegal Clef:{2}", className, functionName, clef.ToString()));
                    return "";
            }
        }

        /// <summary>
        /// To be used for Music Braille as text
        /// </summary>
        /// <param name="clef"></param>
        /// <returns></returns>
        public string ToShortString()
        {
            const string functionName = "ToShortString";
            switch (clef)
            {
                case ClefEnum.C: return "C-clef"; 
                case ClefEnum.G: return "G-clef";
                case ClefEnum.F: return "F-clef";
                case ClefEnum.percussion: return "Perc.";
                case ClefEnum.TAB: return "TAB";
                case ClefEnum.jianpu: return "jianpu";
                case ClefEnum.none: return "no-clef";
                default:
                    Logger.LogOnce(string.Format("{0}.{1} Illegal Clef:{2}", className, functionName, clef.ToString()));
                    return "";
            }
        }

        /// <summary>
        /// Private constructor, used by the Crate() method
        /// </summary>
        /// <param name="node"></param>
        private ClefElement(XmlNode node, ScorePartElement scorePartElement)
        {
            const string functionName = "ClefElement()";
            this.scorePartElement = scorePartElement;
            bool signFound = false;


            // Dig out attributes
            foreach (XmlAttribute a in node.Attributes)
            {
                switch (a.Name)
                { 
                    case "number":
                        Utilities.Parse(a.InnerText, ref staffNumber, 1, int.MaxValue, "Invalid staff-number", false); break;
                    case "color":
                        Logger.LogOnce(string.Format("{0}.{1} Unsupported attribute. Name={2} Value={3}", className, functionName, a.Name, a.Value));
                        break;
                    default:
                    Logger.LogOnce(string.Format("{0}.{1} Unexpected attribute. Name={2} Value={3}", className, functionName, a.Name, a.Value));
                    break;
                }
            }
                        
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
#if false
            string message = string.Format("Clef={0} Line={1} Staff={2}", clef.ToString(), line, staffNumber);
            Logger.LogCF(message);
#endif
        }

        /// <summary>
        /// The Id of the part to which this note belongs
        /// Example: "P1"
        /// </summary>
        public string PartId
        {
            get
            {
                return scorePartElement.partId;
            }
        }



        public static ClefElement Create(XmlNode node,ScorePartElement scorePartElement)
        {
            return new ClefElement(node, scorePartElement);
        }

        public override string ToString() 
        {
            return string.Format("{0}", LocalizeClef(clef));
        }
    }
}
