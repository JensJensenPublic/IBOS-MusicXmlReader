using System.Xml;
using System.Collections.Generic;
using System.Text;

namespace MusicXmlReaderModel
{
    // http://usermanuals.musicxml.com/MusicXML/Content/EL-MusicXML-ornaments.htm


    class OrnamentsElement : Element
    {
        public enum OrnamentsTypeEnum 
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


        private string ToLocalizedString(OrnamentsTypeEnum ornament) 
        {
            const string function = "OrnamentsElement.ToLocalizedString";
            string result = null;
            switch (ornament)
            {
                case OrnamentsTypeEnum.undefined: return ResourcesForModel.Ornament_Undefined;
                case OrnamentsTypeEnum.delayedInvertedTurn: return ResourcesForModel.Ornament_DelayedInvertedTurn;
                case OrnamentsTypeEnum.delayeTurn: return ResourcesForModel.Ornament_DelayedTurn;
                case OrnamentsTypeEnum.invertedMordent: return ResourcesForModel.Ornament_InvertedMordent;
                case OrnamentsTypeEnum.invertedTurn: return ResourcesForModel.Ornament_Turn; ;
                case OrnamentsTypeEnum.mordent: return ResourcesForModel.Ornament_Mordent;
                case OrnamentsTypeEnum.otherOrnament: return ResourcesForModel.Ornament_OtherOrnament;
                case OrnamentsTypeEnum.schleifer: return ResourcesForModel.Ornament_Schleifer;
                case OrnamentsTypeEnum.shake: return ResourcesForModel.Ornament_Shake;
                case OrnamentsTypeEnum.tremolo: return ResourcesForModel.Ornament_Tremolo;
                case OrnamentsTypeEnum.trillMark: return ResourcesForModel.Ornament_TrillMark;
                case OrnamentsTypeEnum.turn: return ResourcesForModel.Ornament_Turn;
                case OrnamentsTypeEnum.verticalTurn: return ResourcesForModel.Ornament_VerticalTurn;
                case OrnamentsTypeEnum.wavyLine: return ResourcesForModel.Ornament_WavyLine;
                case OrnamentsTypeEnum.accidentalMark: return ResourcesForModel.Ornament_AccidentalMark;
                default:
                    Logger.LogOnce(string.Format("{0} Unknown ornament type: {1}", function,ornament.ToString())); break;
            }
            if (null == result)
            {
                result = ornament.ToString(); 
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

