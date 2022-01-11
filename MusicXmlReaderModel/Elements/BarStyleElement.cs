using System.Xml;
using System.Globalization;


namespace MusicXmlReaderModel
{
    public enum BarStyleEnum { unknown, regular, dotted, dashed, heavy, lightLight, lightHeavy, heavyLight, heavyHeavy, tick, shortBarStyle, none }; // "short" is areserved word!


    /// <summary>
    /// http://usermanuals.musicxml.com/MusicXML/Content/EL-MusicXML-bar-style.htm
    /// </summary>
    public class BarStyleElement :Element
    {

        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private BarStyleElement()
        {
        }

        private BarStyleEnum barStyle;
        public  BarStyleEnum BarStyle
        {
            get
            {
                return barStyle;
            }
        }

        /// <summary>
        /// Private constructor, used by the Crate() method
        /// </summary>
        /// <param name="node"></param>
        private BarStyleElement(XmlNode node)
        {
            const string functionName = "BarStyleElement";
            //Logger.LogOnce(string.Format("{0} constructor", functionName));

            // Dig out elements
            foreach (XmlNode n in node.ChildNodes)
            {
                switch (n.Value)
                {
                    case "regular": barStyle = BarStyleEnum.regular; break;
                    case "dotted": barStyle = BarStyleEnum.dotted; break;
                    case "dashed": barStyle = BarStyleEnum.dashed; break;
                    case "heavy": barStyle = BarStyleEnum.heavy; break;
                    case "light-light": barStyle = BarStyleEnum.lightLight; break;
                    case "light-heavy": barStyle = BarStyleEnum.lightHeavy; break;
                    case "heavy-light": barStyle = BarStyleEnum.heavyLight; break;
                    case "heavy-heavy": barStyle = BarStyleEnum.heavyHeavy; break;
                    case "tick": barStyle = BarStyleEnum.tick; break;
                    case "short": barStyle = BarStyleEnum.shortBarStyle; break;
                    case "none": barStyle = BarStyleEnum.none; break; // No barline appears.
                    default: Logger.LogOnce(string.Format("{0}: Unknown element value ={1} ", functionName, n.Value)); break;
                }
            }
            
            // Dig out attributes
            foreach (XmlAttribute a in node.Attributes)
            {
                switch (a.Name)
                {
                    default: Logger.LogOnce(string.Format("{0}: Unexpected attribute '{1}'", functionName, a.Name)); break;
                }
            }
        }

        public static BarStyleElement Create(XmlNode node)
        {
            return new BarStyleElement(node);
        }

    

        public override string ToString()
        {
            return barStyle.ToString(); // Needs localisation !
        }
    }
}

