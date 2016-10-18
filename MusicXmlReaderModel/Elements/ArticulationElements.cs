using System.Xml;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace MusicXmlReaderModel
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

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("Articulations:");
            foreach (Articulation a in articulationList)
            {
                sb.Append(" " + LocalizeArticulation(a) );
            }
            return sb.ToString();
        }

        private string LocalizeArticulation(Articulation a)
        {
            string functionName = "LocalizeArticulation";
            switch (a)
            {
                case Articulation.accent: return ResourcesForModel.ArticulationsElement_Accent;
                case Articulation.breathmark: return ResourcesForModel.ArticulationsElement_BreathMark;
                case Articulation.caesura: return ResourcesForModel.ArticulationsElement_Caesura;
                case Articulation.detachedlegato: return ResourcesForModel.ArticulationsElement_DetatchedLegato;
                case Articulation.doit: return ResourcesForModel.ArticulationsElement_Doit;
                case Articulation.falloff: return ResourcesForModel.ArticulationsElement_Falloff;
                case Articulation.otherarticulation:return ResourcesForModel.ArticulationsElement_OtherArticulation;
                case Articulation.plop:return ResourcesForModel.ArticulationsElement_Plop;
                case Articulation.scoop:return ResourcesForModel.ArticulationsElement_Scoop;
                case Articulation.spiccato: return ResourcesForModel.ArticulationsElement_Spiccato;
                case Articulation.staccatissimo: return ResourcesForModel.ArticulationsElement_Staccatissimo;
                case Articulation.staccato: return ResourcesForModel.ArticulationsElement_Staccato;
                case Articulation.stress: return ResourcesForModel.ArticulationsElement_Stress;
                case Articulation.strongaccent: return ResourcesForModel.ArticulationsElement_StrongAccent;
                case Articulation.tenuto: return ResourcesForModel.ArticulationsElement_Tenuto;
                case Articulation.unstress: return ResourcesForModel.ArticulationsElement_Unstress;
                default:
                    Logger.LogOnce(string.Format("{0}: Unexpected Articulation={1} ", functionName, a.ToString()));
                    return "";
            }

        }

    }
}
