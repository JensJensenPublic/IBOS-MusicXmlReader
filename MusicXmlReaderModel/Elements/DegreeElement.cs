using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using MusicSynthesis;

namespace MusicXmlReaderModel
{

    // https://usermanuals.musicxml.com/MusicXML/Content/EL-MusicXML-degree.htm

    public enum DegreeAlterEnum { unknown, flat, sharp, none};


   

    /// <summary>
    /// For extended HarmomyElement functionality
    /// </summary>
    public class DegreeElement
    {
        private string className = "DegreeElement";
        private int degreeValue;
        private DegreeAlterEnum degreeAlter;
        private int degreeAlterInteger;
        private DegreeTypeEnum  degreeType;

        public int DegreeValue
        {
            get
            {
                return degreeValue;
            }  
        }

        public int DegreeAlterInteger
        {
            get
            {
                return degreeAlterInteger;
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
                    case "degree-value":
                        // The content of the degree - value type is a number indicating the degree of the chord(1 for the root, 3 for third, etc).
                        // The text attribute specifies how the type of the degree should be displayed in a score. The degree-value symbol attribute indicates that a symbol
                        // should be used in specifying the degree. If the symbol attribute is present, the value of the text attribute follows the symbol.
                        if (!Utilities.Parse(n.InnerText, ref degreeValue, 0, 19, className + functionName, false)) // "false" => Do not accept an empty string
                        {
                            Logger.LogOnce(string.Format("{0}.{1} found unexpected value for degree-value:{2}", className, functionName, n.InnerText));
                        }
                        break;
                    case "degree-alter":
                        // The degree-alter type represents the chromatic alteration for the current degree. If the degree - type value is alter or subtract,
                        // the degree - alter value is relative to the degree already in the chord based on its kind element.
                        // If the degree - type value is add, the degree - alter is relative to a dominant chord(major and perfect intervals except for a minor seventh).
                        // The plus - minus attribute is used to indicate if plus and minus symbols should be used instead of sharp and flat symbols to display
                        // the degree alteration; it is no by default
                        switch (n.InnerText)
                        {
                            case "-1":
                                degreeAlter = DegreeAlterEnum.flat;
                                degreeAlterInteger = -1;    
                                break;
                            case "0":
                                degreeAlter = DegreeAlterEnum.none;
                                degreeAlterInteger = 0;
                                break;
                            case "":
                                degreeAlter = DegreeAlterEnum.none;
                                degreeAlterInteger = 0;
                                break;
                            case "1":
                                degreeAlter = DegreeAlterEnum.sharp;
                                degreeAlterInteger = 1;
                                break;
                            default:   degreeAlter = DegreeAlterEnum.unknown;
                                       Logger.LogOnce(string.Format("{0}.{1} found unexpected value for degree-alter:{2}", className, functionName, n.InnerText));
                                       break;
                        } break;

                    case "degree-type":
                        // The degree-type type indicates if this degree is an addition, alteration, or subtraction relative to the kind of the current chord.
                        // The value of the degree - type element affects the interpretation of the value of the degree-alter element.
                        // The text attribute specifies how the type of the degree should be displayed in a score.
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
            //Logger.LogOnce(string.Format("{0}.{1} found DegreeElement: Value={2} Alter={3} Type= {4} But the value is not used yet", className, functionName, degreeValue, degreeAlter, degreeType));
            Logger.LogOnce(string.Format("{0}.{1} found DegreeElement: {2}  But the value is not used yet", className, functionName, this.ToString()));
            //Logger.LogOnce(string.Format("{0}.{1} found DegreeElement. But the value is not used yet.", className, functionName));
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
