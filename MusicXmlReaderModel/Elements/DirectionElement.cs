using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MusicXmlReaderModel
{
    class DirectionElement : EventElement
    {
        string className = "DirectionElement";
        private DynamicsElement dynamicsElement;
        // Prevent construction
        private DirectionElement()
        {
        }

        private DirectionElement(XmlNode node)
        {
            string functionName = "DirectionElement";
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
                    case "direction-type":
                        // Logger.LogOnce(string.Format("{0}.{1} DirectionType Value={2}", className, functionName, n.InnerText));
                        foreach (XmlNode child in n.ChildNodes)
                        {
                            switch (child.Name)
                            {
                                case "dynamics": dynamicsElement = DynamicsElement.Create(child); break;
                                default: break;
                            }
                        }
                        break;
                    default:
                        // Logger.LogOnce(string.Format("{0}.{1} DirectionType Value={2}", className, functionName, n.Name));
                        break;
                }
            }

        }

        internal DynamicsElement DynamicsElement
        {
            get
            {
                return dynamicsElement;
            }

        }

        public static DirectionElement Create(XmlNode xmlNode)
        {
            return new DirectionElement(xmlNode);
        }

    }
}
