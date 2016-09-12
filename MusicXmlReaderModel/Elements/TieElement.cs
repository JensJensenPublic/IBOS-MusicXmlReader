using System.Xml;

namespace MusicXmlReaderUI
{
    class TieElement
    {
        // The tie element affects the sound, and the time-modification affects placement,
        // but the tied and tuplet elements indicate that there is something to see on the score indicating the tie or tuplet.
        
        string tieType = "";

        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private TieElement()
        {
        }


        /// <summary>
        /// Private constructor, used by the Crate() method
        /// </summary>
        /// <param name="node"></param>
        private TieElement(XmlNode node)
        {
            // Dig out attributes
            foreach (XmlAttribute a in node.Attributes)
            {
                switch (a.Name)
                {
                    case "type":
                        tieType = a.Value;
                        break;
                }
            }
        }

        public string TieType
        {
            get
            {
                return tieType;
            }
        }

        public static TieElement Create(XmlNode node)
        {
            return new TieElement(node);
        }


        public override string ToString()
        {
            return string.Format("Overbinding {0}",tieType);        
        }
    }
}

