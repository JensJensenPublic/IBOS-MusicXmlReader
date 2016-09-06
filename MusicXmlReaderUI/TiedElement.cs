using System.Xml;

namespace MusicXmlReaderUI
{

    // The tied type represents the notated tie. The tie element represents the tie sound.
    // Most code stolen from SlurElement. Might use a common class instead ???????????? TODO
    class TiedElement
    {
        public enum TiedTypeEnum { Undefinded, Start, Stop, Continue };
        TiedTypeEnum tiedType;
        int numberLevel = 1; // MusicXml default value

        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private TiedElement()
        {
        }


        /// <summary>
        /// Private constructor, used by the Crate() method
        /// </summary>
        /// <param name="node"></param>
        private TiedElement(XmlNode node)
        {
            // Dig out attributes
            foreach (XmlAttribute a in node.Attributes)
            {
                switch (a.Name)
                {
                    case "type":
                        {
                            switch (a.Value)
                            {
                                case "start": tiedType = TiedTypeEnum.Start; break;
                                case "stop": tiedType = TiedTypeEnum.Stop; break;
                                case "continue": tiedType = TiedTypeEnum.Continue; break;
                                default:
                                    Model.Log(string.Format("TiedElement: Unknown attribute value '{0}'", a.Value));
                                    tiedType = TiedTypeEnum.Undefinded; break;
                            }
                        }
                        break;

                    case "number":
                        Utilities.Parse(a.Value, ref numberLevel, 1, 6, "TiedElement:", false);
                        break;

                    // Explicitly ignire the following
                    case "relative-x": break;
                    case "relative-y": break;

                    default:
                        Model.Log(string.Format("TiedElement: Unknown attribute name '{0}'", a.Name));
                        break;
                }
            }
        }

        private string LocalizeTied(TiedTypeEnum tiedType)
        {
            switch (tiedType)
            {
                case TiedTypeEnum.Undefinded: return "Udefineret";
                case TiedTypeEnum.Start: return "Start";
                case TiedTypeEnum.Stop: return "Slut";
                case TiedTypeEnum.Continue: return "Fortsæt";
                default:
                    Model.Log(string.Format("TiedElement.Localize: Unexpectee value of slurType: '{0}'", tiedType.ToString()));
                    return "";
            }
        }


        public TiedTypeEnum TiedType
        {
            get
            {
                return tiedType;
            }
        }

        public int NumberLevel
        {
            get
            {
                return numberLevel;
            }
        }

        public static TiedElement Create(XmlNode node)
        {
            return new TiedElement(node);
        }


        public override string ToString()
        {
            string number = (1 == this.NumberLevel) ? "" : NumberLevel.ToString(); // Ignore the number if it has its default value of 1
            return string.Format("Bindebue {0} {1}", number, LocalizeTied(this.tiedType));
        }
    }
}

