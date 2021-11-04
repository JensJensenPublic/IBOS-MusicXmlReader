using System.Xml;

namespace BrailleMusicDecoder
{
    /// <summary>
    /// Contains all parameters needed for building a part.
    /// This information must be saved on a "per part" basis in the "Bar by Bar" mode, where the parts are interleaved!
    /// </summary>
    class PartBuildingParameters
    {
        private string partName;
        /// <summary>
        /// The name of the part, for instance "Right" or Organ
        /// </summary>
        public string Name { get { return partName; } }

        private XmlNode currentPart = null; // The part currently under construction
        public XmlNode CurrentPart { get { return currentPart; } set { currentPart = value; } }

        protected string CurrentPartName { get { return partName; } }

        protected const int FirstMeasureNumber = 1;
        private int currentMeasureNumber = FirstMeasureNumber;
        protected int CurrentMeasureNumber { get { return currentMeasureNumber; } set { currentMeasureNumber = value; } }

        protected XmlNode currentAttributesElement = null;
        public XmlNode CurrentAttributesElement { get { return currentAttributesElement; } set { currentAttributesElement = value; } }

        private XmlNode currentMeasure = null;
        public XmlNode CurrentMeasure { get { return currentMeasure; } set { currentMeasure = value; } }

        private PartBuildingParameters(string partName)
        {
            this.partName = partName;
        }

        public static PartBuildingParameters Create(string name)
        {
            return new PartBuildingParameters(name);
        }
    }
}
