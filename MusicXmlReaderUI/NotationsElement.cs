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
        private TupletElement tupletElement;
        private SlideElement slideElement;
        private GlissandoElement glissandoElement;
        private ArpeggiateElement arpeggiateElement;
        private AccidentalMarkElement accidentalMarkElement;
        private FermataElement fermataElement;
        private OrnamentsElement ornamentsElement;
        private TechnicalElement technicalElement;

        private ArticulationsElement articulations;

        internal ArticulationsElement Articulations
        {
            get
            {
                return articulations;
            }
        }

        internal SlurElement SlurElement
        {
            get
            {
                return slurElement;
            }
        }

        internal TiedElement TiedElement
        {
            get
            {
                return tiedElement;
            }
        }

        internal TupletElement TupletElement
        {
            get
            {
                return tupletElement;
            }
        }

        internal SlideElement SlideElement
        {
            get
            {
                return slideElement;
            }
        }

        internal GlissandoElement GlissandoElement
        {
            get
            {
                return glissandoElement;
            }            
        }

        internal ArpeggiateElement ArpeggiateElement
        {
            get
            {
                return arpeggiateElement;
            }
        }

        public AccidentalMarkElement AccidentalMarkElement
        {
            get
            {
                return accidentalMarkElement;
            }
        }

        internal FermataElement FermataElement
        {
            get
            {
                return fermataElement;
            }
        }

        internal OrnamentsElement OrnamentsElement
        {
            get
            {
                return ornamentsElement;
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
            // http://usermanuals.musicxml.com/MusicXML/MusicXML.htm#EL-MusicXML-slur.htm?Highlight=slur
            // http://usermanuals.musicxml.com/MusicXML/MusicXML.htm#EL-MusicXML-articulations.htm
            // http://usermanuals.musicxml.com/MusicXML/MusicXML.htm#EL-MusicXML-footnote.htm
            // http://usermanuals.musicxml.com/MusicXML/MusicXML.htm#EL-MusicXML-level.htm
            // http://usermanuals.musicxml.com/MusicXML/MusicXML.htm#EL-MusicXML-accidental-mark.htm
            // http://usermanuals.musicxml.com/MusicXML/MusicXML.htm#EL-MusicXML-arpeggiate.htm             
            // http://usermanuals.musicxml.com/MusicXML/MusicXML.htm#EL-MusicXML-dynamics_1.htm
            // http://usermanuals.musicxml.com/MusicXML/MusicXML.htm#EL-MusicXML-fermata_1.htm
            // http://usermanuals.musicxml.com/MusicXML/MusicXML.htm#EL-MusicXML-glissando.htm
            // http://usermanuals.musicxml.com/MusicXML/MusicXML.htm#EL-MusicXML-non-arpeggiate.htm
            // http://usermanuals.musicxml.com/MusicXML/MusicXML.htm#EL-MusicXML-ornaments.htm
            // http://usermanuals.musicxml.com/MusicXML/MusicXML.htm#EL-MusicXML-other-notation.htm
            // http://usermanuals.musicxml.com/MusicXML/MusicXML.htm#EL-MusicXML-slide.htm
            // http://usermanuals.musicxml.com/MusicXML/MusicXML.htm#EL-MusicXML-technical.htm
            // http://usermanuals.musicxml.com/MusicXML/MusicXML.htm#EL-MusicXML-tied.htm
            // http://usermanuals.musicxml.com/MusicXML/MusicXML.htm#EL-MusicXML-tuplet.htm

            // Dig out elements
            // Some of these elements are graphical representations of another element representing the sound! Example: tied/tie
            foreach (XmlNode child in node.ChildNodes)
            {
                bool ok = true;
                switch (child.Name)
                {
                    case "slur": slurElement = SlurElement.Create(child); break;
                    case "articulations": articulations = ArticulationsElement.Create(child); break;
                    case "footnote": ok = false; break;
                    case "level": ok = false; break;
                    case "accidental-mark": accidentalMarkElement = AccidentalMarkElement.Create(child); break; // Løst fortegn
                    case "arpeggiate": arpeggiateElement = ArpeggiateElement.Create(child); break; // Tilhører brudt akkord
                    case "dynamics": ok = false; break;
                    case "fermata": fermataElement = FermataElement.Create(child); break; // Har fermat tilknyttet
                    case "glissando": glissandoElement = GlissandoElement.Create(child); break;
                    case "non-arpeggiate": ok = false; break;
                    case "ornaments": ornamentsElement = OrnamentsElement.Create(child); break;
                    case "other-notation ": ok = false; break;
                    case "slide": slideElement = SlideElement.Create(child); break;
                    case "technical":  technicalElement = TechnicalElement.Create(child); break;
                    case "tied": tiedElement = TiedElement.Create(child); break;
                    case "tuplet": tupletElement = TupletElement.Create(child); break; 
                    default:
                        Logger.Log(string.Format("NotationsElement: Unknown element '{0}'", child.Name));break;
                }
                if (!ok)
                {                    
                    Logger.LogOnce(string.Format("NotationsElement: Element '{0}' is not implemented yet", child.Name)); break;
                }
            }
        }

        public static NotationsElement Create(XmlNode node)
        {
            return new NotationsElement(node);
        }

        public override string ToString() // LOCALIZE
        {
            string s = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}",
            (null == slurElement) ? "" : slurElement.ToString() + " ", // 0
            (null == tiedElement) ? "" : tiedElement.ToString() + " ", // 1
            (null == glissandoElement) ? "" : glissandoElement.ToString() + " ", // 2
            (null == slideElement) ? "" : slideElement.ToString() + " ",         // 3
            (null == tupletElement) ? "" : tupletElement.ToString() + " ",       // 4
            (null == arpeggiateElement) ? "" : arpeggiateElement.ToString() + " ",// 5
            (null == fermataElement) ? "" : fermataElement.ToString() + " ", //6
            (null == ornamentsElement) ? "" : ornamentsElement.ToString() + " "); //7

            // ...
            // Add other elements as they are implemented!

            return s;
        }
    }
}

