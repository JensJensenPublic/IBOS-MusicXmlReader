using System.Xml;

namespace MusicXmlReaderUI
{
    class SlurElement
    {
        public enum SlurTypeEnum { Undefinded, Start, Stop, Continue };
        SlurTypeEnum slurType;
        int numberLevel = 1 ; // MusicXml default value
        
        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private SlurElement()
        {
        }


        /// <summary>
        /// Private constructor, used by the Crate() method
        /// </summary>
        /// <param name="node"></param>
        private SlurElement(XmlNode node)
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
                                case "start": slurType = SlurTypeEnum.Start; break;
                                case "stop": slurType = SlurTypeEnum.Stop; break;
                                case "continue": slurType = SlurTypeEnum.Continue; break;
                                default:
                                    Model.Log(string.Format("SlurElement: Unknown attribute value '{0}'", a.Value));
                                    slurType = SlurTypeEnum.Undefinded; break;
                            }
                        }
                        break;

                    case "number":
                        Utilities.Parse(a.Value, ref numberLevel, 1, 6, "SlurElement:");
                        break;

                    default:
                        Model.Log(string.Format("SlurElement: Unknown attribute name '{0}'", a.Name));
                        break;
                }
            }
        }

        private string LocalizeSlur(SlurTypeEnum slurType)
        {
            switch (slurType)
            {
                case SlurTypeEnum.Undefinded: return "Udefineret";
                case SlurTypeEnum.Start: return "Start";
                case SlurTypeEnum.Stop: return "Slut";
                case SlurTypeEnum.Continue: return "Fortsæt";
                default:    Model.Log(string.Format("SlurElement.Localize: Unexpectee value of slurType: '{0}'", slurType.ToString()));
                            return "";
            }
        }


        public SlurTypeEnum SlurType
        {
            get
            {
                return slurType;
            }
        }

        public int NumberLevel
        {
            get
            {
                return numberLevel;
            }
        }

        public static SlurElement Create(XmlNode node)
        {
            return new SlurElement(node);
        }


        public override string ToString()
        {
            string number = (1 == this.NumberLevel) ? "" :  NumberLevel.ToString(); // Ignore the number if it has its default value of 1
            return string.Format("Legato{0} {1}", number, LocalizeSlur(this.slurType));
        }
    }
}

