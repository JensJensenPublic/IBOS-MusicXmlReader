using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MusicXmlReaderModel
{

    public enum WedgeEnum{unknown,crescendo, diminuendo,stopWwedge,continueWedge} // Can't use the reserved word "continue"

    /// <summary>
    /// https://usermanuals.musicxml.com/MusicXML/Content/ST-MusicXML-wedge-type.htm
    /// </summary>
    public class WedgeElement : Element
    {
        private WedgeEnum wedge = WedgeEnum.unknown;
        public WedgeEnum Wedge { get { return wedge; } }

        private WedgeElement(XmlNode node)
        {
            // Attributes. 
            foreach (XmlAttribute a in node.Attributes)
            {
                if (a.Name == "type")
                {
                    switch (a.Value)
                    {
                        case "diminuendo": wedge = WedgeEnum.diminuendo;  break;
                        case "crescendo": wedge = WedgeEnum.crescendo; break;
                        case "stop": wedge = WedgeEnum.stopWwedge; break;
                        case "continue": wedge = WedgeEnum.continueWedge; break;
                        default: Logger.LogCF(string.Format("Unknown attribute value={0}", a.Value)); break;
                    }
                }
                else
                {
                    Logger.LogCF(string.Format("Unknown attribute name={0}", a.Name)); break;
                }
            }


            // Dig out elements
            foreach (XmlNode n in node.ChildNodes)
            {
                switch (n.Name)
                {
                    case "":
                    default: Logger.LogOnce(string.Format(": Unknown element. Name={0} ", n.Name)); break;
                }
            }

        }

        public override string ToString()
        {
#warning TODO Localize
            switch (wedge)
            {
                case WedgeEnum.crescendo: return "crescendo";
                case WedgeEnum.diminuendo: return "diminuendo";
                case WedgeEnum.stopWwedge: return "stop wedge";
                case WedgeEnum.continueWedge: return "continue wedge";
                default:  return "";
            }
        }

        public static WedgeElement Create(XmlNode node)
        {
            return new WedgeElement(node);
        }
    }
}
