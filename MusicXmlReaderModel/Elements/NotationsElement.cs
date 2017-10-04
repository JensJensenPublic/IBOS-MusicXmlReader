using System.Xml;

namespace MusicXmlReaderModel
{

    public class NotationsElement : Element
    {
        // http://usermanuals.musicxml.com/MusicXML/Content/CT-MusicXML-notations.htm

        string  className = "NotationsElement";
        private SlurElement slurElement;
        private TiedElement tiedElement;
        private TupletElement tupletElement;
        private SlideElement slideElement;
        private GlissandoElement glissandoElement;
        private ArpeggiateElement arpeggiateElement;
        private DynamicsElement dynamicsElement;
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
            string functionName = "NotationsElement";
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

            // http://www.musikipedia.dk/musikordbog-engelsk


            // Dig out elements
            // Some of these elements are graphical representations of another element representing the sound! Example: tied/tie
            foreach (XmlNode child in node.ChildNodes)
            {
                bool ok = true;
                switch (child.Name)
                {
                    case "slur": slurElement = SlurElement.Create(child); break; // Legatobue                                                   
                    case "articulations": articulations = ArticulationsElement.Create(child); break; // Artikulation
                    case "footnote": ok = false; break; // (Fodnote)
                    case "level": ok = false; break; // (Niveau)
                    case "accidental-mark": accidentalMarkElement = AccidentalMarkElement.Create(child); break; // (Løst) fortegn (faste fortegn betegnes: key signature)
                    case "arpeggiate": arpeggiateElement = ArpeggiateElement.Create(child); break; // Brudt akkord
                    case "dynamics": dynamicsElement = DynamicsElement.Create(child); break; // Dynamik
                    case "fermata": fermataElement = FermataElement.Create(child);
                        // Logger.LogOnce(string.Format("{0}.{1}: Child element='{2}'", className, functionName, child.Name));
                        break; // Fermat
                    case "glissando": glissandoElement = GlissandoElement.Create(child); break; // Glissando
                    case "non-arpeggiate": ok = false; break; // Ikke brudt
                    case "ornaments": ornamentsElement = OrnamentsElement.Create(child); break; // Ornamenter
                    case "other-notation ": ok = false; break; // Anden notation
                    case "slide": slideElement = SlideElement.Create(child); break; // ????????????????????????????
                    case "technical":  technicalElement = TechnicalElement.Create(child); break; // Teknisk
                    case "tied": tiedElement = TiedElement.Create(child); break; // Bindebue
                    case "tuplet": tupletElement = TupletElement.Create(child); break; // "Uregelmæssig nodeværdi"
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

        public override string ToString() // Localization is implemented in the various elements mentioned below, not here
        {
            string s = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}",
            (null == slurElement) ? "" : slurElement.ToString() + " ", // 0
            (null == tiedElement) ? "" : tiedElement.ToString() + " ", // 1
            (null == glissandoElement) ? "" : glissandoElement.ToString() + " ", // 2
            (null == slideElement) ? "" : slideElement.ToString() + " ",         // 3
            (null == tupletElement) ? "" : tupletElement.ToString() + " ",       // 4
            (null == arpeggiateElement) ? "" : arpeggiateElement.ToString() + " ",// 5
            (null == fermataElement) ? "" : fermataElement.ToString() + " ", //6
            (null == ornamentsElement) ? "" : ornamentsElement.ToString() + " ", //7
            (null == accidentalMarkElement) ? "" : accidentalMarkElement.ToString() + " ", //8
            (null == technicalElement) ? "" : technicalElement.ToString() + " "); //9

            // ...
            // Add other elements as they are implemented!

            return s;
        }
    }
}

