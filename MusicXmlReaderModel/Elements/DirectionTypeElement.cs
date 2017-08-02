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
        DynamicsElement dynamicsElement;
        MetronomeElement metronomeElement;

        private DirectionTypeElement(XmlNode node)
        {
            const string functionName = "DirectionTypeElement";
            // Dig out elements
            foreach (XmlNode n in node.ChildNodes)
            {
                switch (n.Name)
                {
                    case "dynamics":
                        dynamicsElement = DynamicsElement.Create(n);
                        // This is mentioned in Error 158  so we get rid of the log                 
                        // Logger.LogOnce(string.Format("{0}.{1}: Known but unsupported element. Name={2} ", className, functionName, n.Name));
                        break;
                   
                    case "metronome":
                        metronomeElement = MetronomeElement.Create(n); 
                        break;            

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
                        // This is mentioned in Error 158  so we get rid of the log 
                        // Logger.LogOnce(string.Format("{0}.{1}: Known but unsupported element. Name={2} ", className, functionName, n.Name));
                        //Logger.LogOnce(string.Format("{0}.{1}: Known but unsupported element.", className, functionName)); // Group them all together
                        break;
                    default: Logger.LogOnce(string.Format("{0}.{1}: Unknown element. Name={2} ",className, functionName, n.Name)); break;
                }
            }

        }

        public DynamicsElement DynamicsElement
        {
            get
            {
                return dynamicsElement;
            }
        }

        public MetronomeElement MetronomeElement
        {
            get
            {
                return metronomeElement;
            }
        }

        public static DirectionTypeElement Create(XmlNode node)
        {
            return new DirectionTypeElement(node);
        }
    }
}
