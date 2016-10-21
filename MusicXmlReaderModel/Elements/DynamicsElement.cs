using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MusicXmlReaderModel
{
    class DynamicsElement: EventElement
    {
        string className = "DynamicsElement";
        string value = "";
        // Prevent construction
        private DynamicsElement()
        {
        }

        private DynamicsElement(XmlNode node)
        {
            string functionName = "DynamicsElement";
            // Dig out attributes
            foreach (XmlAttribute a in node.Attributes)
            {

                switch (a.Name)
                {
                    case "default-x":  break; // Explicitly ignore some graphical attributes 
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
                    case "f":    value = n.Name; break;
                    // TODO Line up all other expected values here
                    default: value = n.Name; break;
                }
                // Logger.LogOnce(string.Format("{0}.{1} Dynamics={2}", className, functionName, value));                
            }

        }

        public string Value
        {
            get
            {
                return value;
            }
 
        }

        public static DynamicsElement Create(XmlNode xmlNode)
        {
            return new DynamicsElement(xmlNode);
        }

    }
}
