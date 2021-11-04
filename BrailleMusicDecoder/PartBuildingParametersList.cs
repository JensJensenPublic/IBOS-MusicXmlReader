using System.Collections.Generic;

using MusicXmlReaderModel; // For Logger. NameSpace only, NO reference !!

namespace BrailleMusicDecoder
{
    /// <summary>
    /// Class needed for handling "Bar over Bar" notation, where the parts are interleaved in the inputstream
    /// </summary>
    class PartBuildingParametersList
    { 
        private List<PartBuildingParameters> partParameters;
        private PartBuildingParameters currentPartParameters;
        public PartBuildingParameters CurrentPartParameters { get { return currentPartParameters; } }

        private PartBuildingParameters Find(string partName)
        {
            foreach (PartBuildingParameters p in partParameters)
            {
                if ( 0 == string.Compare(p.Name, partName))
                {      
                    return p;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns true if the part was added, false if it was already found.
        /// </summary>
        /// <param name="partName"></param>
        /// <returns></returns>
        public bool Add(string partName)
        {
            currentPartParameters = this.Find(partName);
            if (null == currentPartParameters)
            {
                currentPartParameters = PartBuildingParameters.Create(partName);
                Logger.LogCF(string.Format(" PortBuildingParameters for part={0} were not found. Added.", partName));
                return true;
            }
            Logger.LogCF(string.Format(" PortBuildingParameters for part={0} were already found. ", partName));
            return false;
        }
        
        private PartBuildingParametersList()
        {
            partParameters = new List<PartBuildingParameters>();
        }

        public static PartBuildingParametersList Create()
        {
            return new PartBuildingParametersList();
        }
    }
}
