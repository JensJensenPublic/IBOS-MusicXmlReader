using System;
using System.Xml;

namespace MusicXmlReaderUI
{
    class DivisionsElement : Element
    {

        string divisions = "";

        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private DivisionsElement()
        { }

        /// <summary>
        /// Private constructor, used by the Crate() method
        /// </summary>
        /// <param name="node"></param>
        private DivisionsElement(XmlNode node)
        {
            divisions = node.InnerText;
        }

        public static DivisionsElement Create(XmlNode node)
        {
            return new DivisionsElement(node);
        }

        public override string ToString()
        {
            return (string.Format("Divisions: {0} (pr. fjerdedelsnode)", divisions));
        }

        public int GetDivisions()
        {
            return int.Parse(divisions);
        }
    }
}

