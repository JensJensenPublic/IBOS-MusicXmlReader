using System.Xml;

namespace MusicXmlReaderModel
{
    class DivisionsElement : Element
    {
        string className = "DivisionsElement";
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
            string functionName = "DivisionsElement";
            divisions = node.InnerText;
            Logger.LogOnce(string.Format("{0}.{1} Divisions={2}", className, functionName, divisions));
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

