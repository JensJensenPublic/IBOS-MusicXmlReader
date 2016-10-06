using System.Xml;
using System.Collections.Generic;
using System.Text;


namespace MusicXmlReaderUI
{
    class ArticulationsElement
    {
        // http://usermanuals.musicxml.com/MusicXML/Content/EL-MusicXML-articulations.htm   

        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private ArticulationsElement()
        { }

        private List<Articulation> articulationList = new List<Articulation>();
        public List<Articulation> ArticulationList
        {
            get
            {
                return articulationList;
            }
        }

        public enum Articulation // LOCALIZE
        {
            accent,
            breathmark,
            caesura,
            detachedlegato,
            doit,
            falloff,
            otherarticulation,
            plop,
            scoop,
            spiccato,
            staccatissimo,
            staccato,
            stress,
            strongaccent,
            tenuto,
            unstress,
        };

        /// <summary>
        /// Private constructor, used by the Crate() method
        /// </summary>
        /// <param name="node"></param>
        private ArticulationsElement(XmlNode node)
        {
            foreach (XmlNode child in node.ChildNodes)
            {
                switch (child.Name)
                {
                    case "accent": articulationList.Add(Articulation.accent); break;
                    case "breath-mark": articulationList.Add(Articulation.breathmark); break;
                    case "caesura": articulationList.Add(Articulation.caesura); break;
                    case "detached - legato": articulationList.Add(Articulation.detachedlegato); break;
                    case "doit": articulationList.Add(Articulation.doit); break;
                    case "falloff": articulationList.Add(Articulation.falloff); break;
                    case "other-articulation": articulationList.Add(Articulation.otherarticulation); break;
                    case "plop": articulationList.Add(Articulation.plop); break;
                    case "scoop": articulationList.Add(Articulation.scoop); break;
                    case "spiccato": articulationList.Add(Articulation.spiccato); break;
                    case "staccatissimo": articulationList.Add(Articulation.staccatissimo); break;
                    case "staccato": articulationList.Add(Articulation.staccato); break;
                    case "stress": articulationList.Add(Articulation.stress); break;
                    case "strong-accent": articulationList.Add(Articulation.strongaccent); break;
                    case "tenuto": articulationList.Add(Articulation.tenuto); break;
                    case "unstress": articulationList.Add(Articulation.unstress); break; 
                    default:
                        Logger.Log(string.Format("ArticulationsElement: Unknown articulation {0}", node.Name));

                        break;
                }
            }
        }

        public static ArticulationsElement Create(XmlNode node)
        {
            return new ArticulationsElement(node);
        }

        public override string ToString() // LOCALIZE
        {
            StringBuilder sb = new StringBuilder("Articulations:");
            foreach (Articulation a in articulationList)
            {
                sb.Append(" " + a );
            }
            return sb.ToString();
        }


    }
}
