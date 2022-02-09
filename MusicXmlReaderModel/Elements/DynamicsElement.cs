using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MusicXmlReaderModel
{
    // https://usermanuals.musicxml.com/MusicXML/Content/EL-MusicXML-dynamics.htm

    public enum DynamicsEnum {
        unknown,
        Forte,Fortissimo,Fortississimo,ffff,fffff,Fortepiano,
        sforzando,mezzoforte,mezzopiano,otherDynamics,
        Piano,Pianissimi,Pianississimo,pppp,ppppp,pppppp,
        Rinforzando,rfz,Sforzando,sffz,Szorzandopiano,Sforzandopianissimo,Sforzado}


    public class DynamicsElement: EventElement
    {
        string className = "DynamicsElement";
        //string value = "";
        DynamicsEnum value;
        internal DynamicsEnum Value
        {
            get
            {
                return value;
            }
        }

        // Prevent construction
        private DynamicsElement()
        {
        }

        private DynamicsElement(XmlNode node)
        {
            string functionName = "DynamicsElement";
            //string functionName = "DynamicsElement";
            // Dig out attributes
            foreach (XmlAttribute a in node.Attributes)
            {

                switch (a.Name)
                {
                    case "default-x": break; // Explicitly ignore some graphical attributes 
                    //    TODO list other attributes to be ignored
                    default: break;
                        //    Logger.LogOnce(string.Format("{0}.{1}:", className,functionName)); break;
                }
            }



            // Dig out elements
            foreach (XmlNode n in node.ChildNodes)
            {
                switch (n.Name)
                {
                    case "mf": value = DynamicsEnum.mezzoforte; break;
                    case "f": value = DynamicsEnum.Forte; break;
                    case "ff": value = DynamicsEnum.Fortissimo; break;
                    case "fff": value = DynamicsEnum.Fortississimo; break;
                    case "ffff": value = DynamicsEnum.fffff; break;
                    case "fffff": value = DynamicsEnum.fffff; break;
                    case "fp": value = DynamicsEnum.Fortepiano; break;
                    case "fz": value = DynamicsEnum.sforzando; break;
                    case "mp": value = DynamicsEnum.mezzopiano; break;
                    case "other-dynamics": value = DynamicsEnum.otherDynamics; break;
                    case "p": value = DynamicsEnum.Piano; break;
                    case "pp": value = DynamicsEnum.Pianissimi; break;
                    case "ppp": value = DynamicsEnum.Pianississimo; break;
                    case "pppp": value = DynamicsEnum.pppp; break;
                    case "ppppp": value = DynamicsEnum.ppppp; break;
                    case "pppppp": value = DynamicsEnum.pppppp; break;
                    case "rf": value = DynamicsEnum.Rinforzando; break;
                    case "rfz": value = DynamicsEnum.Rinforzando; break;
                    case "sf": value = DynamicsEnum.Sforzando; break;
                    case "sffz": value = DynamicsEnum.Sforzado; break;
                    case "sfp": value = DynamicsEnum.Szorzandopiano; break;
                    case "sfpp": value = DynamicsEnum.Sforzandopianissimo; break;
                    case "sfz": value = DynamicsEnum.Sforzado; break;
                    default:
                        value = DynamicsEnum.unknown;
                        Logger.LogOnce(string.Format("{0}.{1}: Unknown dynamics={2}", className, functionName, n.Name)); break;
                }
            }
        }

        //internal DynamicsEnum Value
        //{
        //    get
        //    {
        //        return value;
        //    }
        //}
    


  

        public static DynamicsElement Create(XmlNode xmlNode)
        {
            return new DynamicsElement(xmlNode);
        }

        public override string ToString()
        {
            return value.ToString(); // No need to localize as  long as we only support danish and english: Both use italian terms anyway!
        }

    }
}
