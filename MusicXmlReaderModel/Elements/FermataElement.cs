using System.Xml;

namespace MusicXmlReaderUI
{
    // The tied type represents the notated tie. The tie element represents the tie sound.
    class FermataElement : Element
    {
        public enum FermataTypeEnum { undefined, upright, inverted};
        private FermataTypeEnum fermataType = FermataTypeEnum.undefined;
        public FermataTypeEnum FermataType
        {
            get
            {
                return fermataType;
            }
        }

        private FermataTypeEnum GetFermataType(string s)
        {
            string function = "GetFermataType";
            switch (s)
            {
                case "upright": return FermataTypeEnum.upright;
                case "inverted": return FermataTypeEnum.inverted;
                default:
                    Logger.LogOnce(string.Format("{0}: Found unexpected fermata value: {1}", function, s));
                    return FermataTypeEnum.undefined;
            }
        }


    /// <summary>
    /// To force the use of the Create() method
    /// </summary>
        private FermataElement(XmlNode node)
        {
            const string function = "FermataElement constructor";
            if (0 != node.ChildNodes.Count)
            {
                Logger.LogOnce(string.Format("{0}: Found unexpected child nodes",function));
            }
                        

            // Dig out attributes
            foreach (XmlAttribute a in node.Attributes)
            {
                switch (a.Name)
                {
                    case "type": fermataType = GetFermataType(a.Value); break;
                    default: Logger.LogOnce(string.Format("{0}: Unexpected fermata attribute {0}", function, a.Name)); break;                    
                }
            }
        }


        public static FermataElement Create(XmlNode node)
        {
            return new FermataElement(node);
        }


        public override string ToString()
        {
            switch (fermataType)
            {
                case FermataTypeEnum.upright: return "Fermat";
                case FermataTypeEnum.inverted: return "Omvendt fermat";
                default: return "";
            }
        }
    }
}

