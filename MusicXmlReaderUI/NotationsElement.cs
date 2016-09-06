using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml;

namespace MusicXmlReaderUI
{

    public class NotationsElement : Element
    {
        // http://usermanuals.musicxml.com/MusicXML/Content/CT-MusicXML-notations.htm

        private SlurElement slurElement;
        private TiedElement tiedElement;

        private ArticulationsElement articulations;

        internal ArticulationsElement Articulations
        {
            get
            {
                return articulations;
            }
        }


        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private NotationsElement()
        {
        }


        /// <summary>
        /// Convert from position on the circle of fifths to an (audible) node name
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        private string Localize(string k)
        {
            return "";
        }


        /// <summary>
        /// Private constructor, used by the Crate() method
        /// </summary>
        /// <param name="node"></param>
        private NotationsElement(XmlNode node)
        {
            // Dig out elements
            // Some of these elements are graphical representations of another element representing the sound! Example: tied/tie
            foreach (XmlNode child in node.ChildNodes)
            {
                switch (child.Name)
                {
                    case "slur": slurElement = SlurElement.Create(child); break;
                    case "articulations": articulations = ArticulationsElement.Create(child); break;
                    case "footnote":
                    case "level":
                    case "accidental-mark":
                    case "arpeggiate":              
                    case "dynamics":
                    case "fermata":
                    case "glissando":
                    case "non-arpeggiate":
                    case "ornaments":
                    case "other-notation ":
                    case "slide":
                    case "technical":
                        Model.Log(string.Format("NotationsElement: Element '{0}' is not implemented yet", child.Name)); break;
                    case "tied": tiedElement = TiedElement.Create(child); break;
                    case "tuplet":
                        Model.Log(string.Format("NotationsElement: Element '{0}' is not implemented yet", child.Name)); break;
                    default:
                        Model.Log(string.Format("NotationsElement: Unknown element '{0}'", child.Name));break;
                }
            }
        }

        public static NotationsElement Create(XmlNode node)
        {
            return new NotationsElement(node);
        }

        public override string ToString()
        {
            return string.Format("{0} {1}",
                (null == slurElement) ? "" : slurElement.ToString(),
                (null == tiedElement) ? "" : tiedElement.ToString()); // Add other elements as they are implemented!
        }
    }
}

