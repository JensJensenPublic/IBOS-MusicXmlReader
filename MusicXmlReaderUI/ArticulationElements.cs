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

        private List<Articulation> articulations = new List<Articulation>();
        public  List<Articulation> Articulations
        {
            get
            {
                return articulations;
            }
        }

        public enum Articulation
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
                    case "accent": articulations.Add(Articulation.accent); break;
                    case "breath-mark": articulations.Add(Articulation.breathmark); break;
                    case "caesura": articulations.Add(Articulation.caesura); break;
                    case "detached - legato": articulations.Add(Articulation.detachedlegato); break;
                    case "doit": articulations.Add(Articulation.doit); break;
                    case "falloff": articulations.Add(Articulation.falloff); break;
                    case "other-articulation": articulations.Add(Articulation.otherarticulation); break;
                    case "plop": articulations.Add(Articulation.plop); break;
                    case "scoop": articulations.Add(Articulation.scoop); break;
                    case "spiccato": articulations.Add(Articulation.spiccato); break;
                    case "staccatissimo": articulations.Add(Articulation.staccatissimo); break;
                    case "staccato": articulations.Add(Articulation.staccato); break;
                    case "stress": articulations.Add(Articulation.stress); break;
                    case "strong-accent": articulations.Add(Articulation.strongaccent); break;
                    case "tenuto": articulations.Add(Articulation.tenuto); break;
                    case "unstress": articulations.Add(Articulation.unstress); break; 
                    default:
                        Model.Log(string.Format("ArticulationsElement: Unknown articulation {0}", node.Name));

                        break;
                }
            }
        }

        public static ArticulationsElement Create(XmlNode node)
        {
            return new ArticulationsElement(node);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("Articulations:");
            foreach (Articulation a in articulations)
            {
                sb.Append(" " + a );
            }
            return sb.ToString();
        }


    }
}
