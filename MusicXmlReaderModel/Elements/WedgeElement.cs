using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static MusicXmlReaderModel.PitchElementBase;

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

        private int number = 1; // 	When a number-level value is implied, the value is 1 by default.
        public int Number { get { return number; } }

        private int spread;
        public int Spread { get { return spread; } } // Spread values are measured in tenths; those at the start of a crescendo wedge or end of a diminuendo wedge are ignored.

        private WedgeElement(XmlNode node)
        {
            // Attributes. 
            foreach (XmlAttribute a in node.Attributes)
            {
                switch (a.Name)
                {
                    case "type":
                        switch (a.Value)
                        {
                            case "diminuendo": wedge = WedgeEnum.diminuendo; break;
                            case "crescendo": wedge = WedgeEnum.crescendo; break;
                            case "stop": wedge = WedgeEnum.stopWwedge; break;
                            case "continue": wedge = WedgeEnum.continueWedge; break;
                            default: Logger.LogCF(string.Format("Unknown attribute value={0}", a.Value)); break;
                        }
                        break;
                    case "number": number = int.Parse(a.Value); break; // When a number-level value is implied, the value is 1 by default.
                    case "spread": spread = int.Parse(a.Value); break; 
                    default: Logger.LogCF(string.Format("Unknown attribute name={0}", a.Name)); break;
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
            switch (wedge)
            {
                case WedgeEnum.crescendo: return ResourcesForModel.Wedge_Crescendo;
                case WedgeEnum.diminuendo: return ResourcesForModel.Wedge_Diminuendo;
                case WedgeEnum.stopWwedge: return ResourcesForModel.Wedge_Stop_Wedge;
                case WedgeEnum.continueWedge: return ResourcesForModel.Wedge_Continue_Wedge;
                default:  return "";
            }
        }

        public static WedgeElement Create(XmlNode node)
        {
            return new WedgeElement(node);
        }
    }
}
