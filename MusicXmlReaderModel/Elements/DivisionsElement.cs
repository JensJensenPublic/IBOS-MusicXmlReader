using System.Xml;

namespace MusicXmlReaderModel
{
    class DivisionsElement : Element
    {
        private string className = "DivisionsElement";
        private int divisions = 0;

        // Contains all divisions until now found in sample files. Only for statistic purposes!
        private int[] knownDivisions = new int[] { 1, 2, 3, 4, 6, 8, 12, 16, 24, 30, 32, 36, 38, 48, 60, 64, 84,96, 120, 256, 336,408, 480, 768, 960, 1024 };

        public int Divisions
        {
            get
            {
                return divisions;
            }
        }

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
            Utilities.Parse(node.InnerText, ref divisions, 0, int.MaxValue, className + "." + functionName, false);
            bool found = false;
            foreach (int d in knownDivisions)
            {
                found = found || (d == divisions);
            }
            if (!found)
            {
                Logger.LogOnce(string.Format("{0}.{1} Divisions={2} Please add to DivisionsElement.KnownDivisions!", className, functionName, divisions));
            }
        }

        public static DivisionsElement Create(XmlNode node)
        {
            return new DivisionsElement(node);
        }

        public override string ToString()
        {
            return (string.Format("Divisions: {0} (pr. fjerdedelsnode)", divisions));
        }

        //public int GetDivisions()
        //{
        //    return divisions;
        //}
    }
}

