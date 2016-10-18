using System.Xml;
using System.Collections.Generic;
using System.Text;
using MusicXmlReaderUI;

namespace MusicXmlReaderModel
{ 

    class OrnamentsElement : Element
    {
        public enum OrnamentsTypeEnum // LOCALIZE
        {
            undefined,
            delayedInvertedTurn,
            delayeTurn,
            invertedMordent,
            invertedTurn,
            mordent,
            otherOrnament,
            schleifer,
            shake,
            tremolo,
            trillMark,
            turn,
            verticalTurn,
            wavyLine,
            accidentalMark
        }

        private List<OrnamentsTypeEnum> ornaments = new List<OrnamentsTypeEnum>();  

        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        /// 
        private OrnamentsElement()
        { }
 

        private OrnamentsElement(XmlNode node)
        {
            const string function = "OrnamentsElement constructor";
            foreach (XmlNode child in node.ChildNodes)
            {
                switch (child.Name)
                {
                    case "delayed-inverted-turn": ornaments.Add(OrnamentsTypeEnum.delayedInvertedTurn); break;
                    case "delayed-turn": ornaments.Add(OrnamentsTypeEnum.delayeTurn); break;
                    case "inverted-mordent": ornaments.Add(OrnamentsTypeEnum.invertedMordent); break;
                    case "inverted-turn": ornaments.Add(OrnamentsTypeEnum.invertedTurn); break;
                    case "mordent": ornaments.Add(OrnamentsTypeEnum.mordent); break;
                    case "other-ornament": ornaments.Add(OrnamentsTypeEnum.otherOrnament); break;
                    case "schleifer": ornaments.Add(OrnamentsTypeEnum.schleifer); break;
                    case "shake": ornaments.Add(OrnamentsTypeEnum.shake); break;
                    case "tremolo": ornaments.Add(OrnamentsTypeEnum.tremolo); break;
                    case "trill-mark": ornaments.Add(OrnamentsTypeEnum.trillMark); break;
                    case "turn": ornaments.Add(OrnamentsTypeEnum.turn); break;
                    case "vertical-turn": ornaments.Add(OrnamentsTypeEnum.verticalTurn); break;
                    case "wavy-line": ornaments.Add(OrnamentsTypeEnum.wavyLine); break;
                    case "accidental-mark": ornaments.Add(OrnamentsTypeEnum.accidentalMark); break;
                    default: Logger.LogOnce(string.Format("{0} Unknown ornament:{1}", function, child.Name));break;
                }
            }



            // Dig out attributes
            foreach (XmlAttribute a in node.Attributes)
            {
                Logger.LogOnce(string.Format("{0} Unknown attribute:{1}", function, a)); break;
            }
        }


        public static OrnamentsElement Create(XmlNode node)
        {
            return new OrnamentsElement(node);
        }


        private string ToLocalizedString(OrnamentsTypeEnum ornament) // LOCALIZE
        {
            const string function = "OrnamentsElement.ToLocalizedString";
            string result = null;
            switch (ornament)
            {
                case OrnamentsTypeEnum.undefined: break;
                case OrnamentsTypeEnum.delayedInvertedTurn: break;
                case OrnamentsTypeEnum.delayeTurn: break;
                case OrnamentsTypeEnum.invertedMordent: break;
                case OrnamentsTypeEnum.invertedTurn: break;
                case OrnamentsTypeEnum.mordent: break;
                case OrnamentsTypeEnum.otherOrnament: break;
                case OrnamentsTypeEnum.schleifer: break;
                case OrnamentsTypeEnum.shake: break;
                case OrnamentsTypeEnum.tremolo: break;
                case OrnamentsTypeEnum.trillMark: return "trille";
                case OrnamentsTypeEnum.turn: break;
                case OrnamentsTypeEnum.verticalTurn: break;
                case OrnamentsTypeEnum.wavyLine: break;
                case OrnamentsTypeEnum.accidentalMark: break;
                default:
                    Logger.LogOnce(string.Format("{0} Unknown ornament type", function)); break;
            }
            if (null == result)
            {
                result = ornament.ToString(); // Until we get the localisation done use this default
            }
            return result;

        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();            
            foreach (OrnamentsTypeEnum ornament in ornaments)
            {
                sb.Append(ToLocalizedString(ornament));
                sb.Append(" ");

            }
            return sb.ToString();
        }
    }
}

