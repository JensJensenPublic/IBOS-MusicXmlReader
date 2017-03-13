using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MusicXmlReaderModel
{

    // https://usermanuals.musicxml.com/MusicXML/Content/CT-MusicXML-direction-type.htm

    public class DirectionTypeElement
    {
        const string className = "DirectionTypeElement";

        private DirectionTypeElement(XmlNode node)
        {
            const string functionName = "DirectionTypeElement";
            // Dig out elements
            foreach (XmlNode n in node.ChildNodes)
            {
                switch (n.Name)
                {
                    case "dynamics":
                        // TO DO Create a Dynamics element here !
                        Logger.LogOnce(string.Format("{0}.{1}: Known but unsupported element. Name={2} ", className, functionName, n.Name)); break;
                   
                    case "metronome":
                        // TODO Implement Metronome element and create one here
                        Logger.LogOnce(string.Format("{0}.{1}: Known but unsupported element. Name={2} ", className, functionName, n.Name)); break;            

                    // No current plans for supporting these:
                    case "accordion-registration":
                    case "bracket":
                    case "coda":
                    case "damp":
                    case "damp-all":
                    case "dashes":              
                    case "eyeglasses":
                    case "harp-pedals":
                    case "image":
                    case "octave-shift":
                    case "other-direction":
                    case "pedal":
                    case "percussion":
                    case "principal-voice":
                    case "rehearsal":
                    case "secundatura":
                    case "segno":
                    case "string-mute":
                    case "wedge":
                    case "words":
                        Logger.LogOnce(string.Format("{0}.{1}: Known but unsupported element. Name={2} ",className,functionName, n.Name)); break;
                    default: Logger.LogOnce(string.Format("{0}.{1}: Unknown element. Name={2} ",className, functionName, n.Name)); break;
                }
            }

        }

        public static DirectionTypeElement Create(XmlNode node)
        {
            return new DirectionTypeElement(node);
        }
    }
}
