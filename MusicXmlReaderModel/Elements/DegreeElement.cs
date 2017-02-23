using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MusicXmlReaderModel
{

   

    /// <summary>
    /// For extended HarmomyElement functionality
    /// </summary>
    public class DegreeElement
    {
        private string className = "DegreeElement";
        private string degreeValue = "";
        private string degreeAlter = "";
        private string degreeType  = "";

        public string DegreeValue
        {
            get
            {
                return degreeValue;
            }  
        }

        public string DegreeAlter
        {
            get
            {
                return degreeAlter;
            } 
        }

        public string DegreeType
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
                    case "degree-value": degreeValue = n.InnerText; break; // TODO : Convert to int!
                    case "degree-alter":
                        switch (n.InnerText)
                        {
                            case "-1": degreeAlter = "b"; break;
                            case "0":  degreeAlter = ""; break;
                            case "":   degreeAlter = ""; break;
                            case "1":  degreeAlter = "#"; break;
                            default:
                                Logger.LogOnce(string.Format("{0}.{1} found unexpected value for degree-alter:{2}", className, functionName, n.InnerText)); break;

                        } break;
                    case "degree-type":  degreeType  = n.InnerText;  break;
                    default:
                        Logger.LogOnce(string.Format("{0}.{1} found unknown degree element. Name={2} InnerText={3}", className, functionName, n.Name, n.InnerText)); break;
                }
            }
            // Log this until we implement a visualization of the DegreeElement !
            //Logger.LogOnce(string.Format("{0}.{1} found DegreeElement: Value={2} Alter={3} Type= {4}", className, functionName, degreeValue, degreeAlter, degreeType));
            Logger.LogOnce(string.Format("{0}.{1} found DegreeElement: {2}", className, functionName, this.ToString()));
        }


        public override string ToString()
        {
            return string.Format("({0}{1}{2})", degreeType, degreeAlter, degreeValue);
        }

        public static DegreeElement Create(XmlNode node)
        {
            return new DegreeElement(node);
        }

    }
}
