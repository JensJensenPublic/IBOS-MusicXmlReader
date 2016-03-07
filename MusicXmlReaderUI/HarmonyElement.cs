using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;


namespace MusicXmlReaderUI
{

    public class HarmonyElement : Element
    {

        string kind;
        string rootStep;
        string rootAlter;

        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private HarmonyElement()
        {
        }
        

        /// <summary>
        /// Private constructor, used by the Crate() method
        /// </summary>
        /// <param name="node"></param>
        private HarmonyElement(XmlNode node)
        {
            // Dig out elements
            foreach (XmlNode n in node.ChildNodes)
            {
                switch (n.Name)
                {
                    case "root":
                        // Dig out elements from the childNode
                        foreach (XmlNode nn in n.ChildNodes)
                        {
                            switch (nn.Name)
                            {
                                case "root-step": rootStep = nn.InnerText; break;
                                case "root-alter": rootAlter = nn.InnerText; break;
                            }
                        }                    
                          break;
                    case "kind": kind = n.InnerText; break;
                }
            }
        }

        public static HarmonyElement Create(XmlNode node)
        {
            return new HarmonyElement(node);
        }

        public override string ToString()
        {
            return string.Format("Akkord: {0} {1} {2}",
                string.IsNullOrEmpty(kind) ? "" : kind,
                string.IsNullOrEmpty(rootStep) ? "" : rootStep,
                string.IsNullOrEmpty(rootAlter) ? "" : rootAlter);
        }
    }
}

