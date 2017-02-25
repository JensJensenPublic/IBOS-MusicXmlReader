using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using MusicSynthesis;

namespace MusicXmlReaderModel
{


    public enum DegreeAlterEnum { unknown, flat, sharp, none};


   

    /// <summary>
    /// For extended HarmomyElement functionality
    /// </summary>
    public class DegreeElement
    {
        private string className = "DegreeElement";
        private int degreeValue;
        private DegreeAlterEnum degreeAlter;
        private DegreeTypeEnum  degreeType;

        public int DegreeValue
        {
            get
            {
                return degreeValue;
            }  
        }

        public DegreeAlterEnum DegreeAlter
        {
            get
            {
                return degreeAlter;
            }
        }

        public DegreeTypeEnum DegreeType
        {
            get
            {
                return degreeType;
            }
        }


        /// <summary>
        /// Prevent construction
        /// </summary>
        private DegreeElement()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        private DegreeElement(XmlNode node)
        {
            string functionName = "DegreeElement";
            foreach (XmlNode n in node.ChildNodes)
            {
  
                switch (n.Name)
                {
                    case "degree-value": if (!Utilities.Parse(n.InnerText, ref degreeValue, 0, 19, className + functionName, false)) // "false" => Do not accept an empty string
                        {
                            Logger.LogOnce(string.Format("{0}.{1} found unexpected value for degree-value:{2}", className, functionName, n.InnerText));
                        }
                        break;
                    case "degree-alter":
                        switch (n.InnerText)
                        {
                            case "-1": degreeAlter = DegreeAlterEnum.flat; break;
                            case "0":  degreeAlter = DegreeAlterEnum.none; break;
                            case "":   degreeAlter = DegreeAlterEnum.none; break;
                            case "1":  degreeAlter = DegreeAlterEnum.sharp; break;
                            default:   degreeAlter = DegreeAlterEnum.unknown;
                                       Logger.LogOnce(string.Format("{0}.{1} found unexpected value for degree-alter:{2}", className, functionName, n.InnerText));
                                       break;
                        } break;

                    case "degree-type":
                        switch (n.InnerText)
                        {
                            case "add":     degreeType = DegreeTypeEnum.add; break;
                            case "alter":   degreeType = DegreeTypeEnum.alter; break;    
                            case "subtract":degreeType = DegreeTypeEnum.subtract; break;
                            default:        degreeType = DegreeTypeEnum.unknown;
                                            Logger.LogOnce(string.Format("{0}.{1} found unexpected value for degree-type:{2}", className, functionName, n.InnerText));
                                            break;
                        }
                        break; 
                                       
                    default:
                        Logger.LogOnce(string.Format("{0}.{1} found unknown degree element. Name={2} InnerText={3}", className, functionName, n.Name, n.InnerText)); break;
                }
            }
            // Log this until we implement a visualization of the DegreeElement !
            //Logger.LogOnce(string.Format("{0}.{1} found DegreeElement: Value={2} Alter={3} Type= {4}", className, functionName, degreeValue, degreeAlter, degreeType));
            Logger.LogOnce(string.Format("{0}.{1} found DegreeElement: {2}", className, functionName, this.ToString()));
        }

        private string DegreeTypeToString()
        {
            switch (degreeType)
            {
                case DegreeTypeEnum.add:        return "add";       // TODO: Localize
                case DegreeTypeEnum.alter:      return "alter";     // TODO: Localize
                case DegreeTypeEnum.subtract:   return "sub";       // TODO: Localize
                case DegreeTypeEnum.unknown:    return "";
                default: return "";
            }
        }


        private string DegreeAlterToString()        
        {

            switch (degreeAlter)
            {
                case DegreeAlterEnum.flat:      return "b";     // TODO: Localize
                case DegreeAlterEnum.none:      return "";      // TODO: Localize
                case DegreeAlterEnum.sharp:     return "#";     // TODO: Localize
                case DegreeAlterEnum.unknown:   return "";      // TODO: Localize
                default:                        return "";
            }

        }

    public override string ToString()
        {
            return string.Format("({0}{1}{2})", DegreeTypeToString(), DegreeAlterToString(), degreeValue.ToString());
        }

        public static DegreeElement Create(XmlNode node)
        {
            return new DegreeElement(node);
        }

    }
}
