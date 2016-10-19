using System.Xml;
using System.Collections.Generic;
using System.Text;

namespace MusicXmlReaderModel
{
    // http://usermanuals.musicxml.com/MusicXML/Content/EL-MusicXML-ornaments.htm


    class OrnamentsElement : Element
    {
        string className = "OrnamentsElement";

        public enum OrnamentsTypeEnum 
        {
            undefined,
            delayedInvertedTurn,
            delayedTurn,
            invertedMordent,
            invertedTurn,
            mordent,
            otherOrnament,
            schleifer,
            shake, 
            trillMark,
            turn,
            verticalTurn,
            wavyLine,
            accidentalMarkUnknown, 
            accidentalMarkFlat,
            accidentalMarkNatural,
            accidentalMarkSharp,
            tremolo0,
            tremolo1,
            tremolo2,
            tremolo3,
            tremolo4,
            tremolo5,
            tremolo6,
            tremolo7,
            tremolo8
        }

        private List<OrnamentsTypeEnum> ornaments = new List<OrnamentsTypeEnum>();

        internal List<OrnamentsTypeEnum> Ornaments
        {
            get
            {
                return ornaments;
            }
        }

        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        /// 
        private OrnamentsElement()
        { }


        private OrnamentsTypeEnum GetAccidentalType(XmlNode node)
        {
            switch (node.InnerText)
            {

                case "natural":  return OrnamentsTypeEnum.accidentalMarkNatural;
                case "flat":  return OrnamentsTypeEnum.accidentalMarkFlat;
                case "sharp":  return OrnamentsTypeEnum.accidentalMarkSharp;
                default:
                    Logger.LogOnce(string.Format("{0}.{1}: Unknown Accidental: {2}", node.InnerText));
                    return OrnamentsTypeEnum.accidentalMarkUnknown;
            }  
        }

        private OrnamentsTypeEnum GetTremoloType(XmlNode node)
        {
            string functionName = "GetTremoloType";
            string s = node.InnerText;
            switch (s)
            {
                case "0": return OrnamentsTypeEnum.tremolo0;
                case "1": return OrnamentsTypeEnum.tremolo1;
                case "2": return OrnamentsTypeEnum.tremolo2;
                case "3": return OrnamentsTypeEnum.tremolo3;
                case "4": return OrnamentsTypeEnum.tremolo4;
                case "5": return OrnamentsTypeEnum.tremolo5;
                case "6": return OrnamentsTypeEnum.tremolo6;
                case "7": return OrnamentsTypeEnum.tremolo7;
                case "8": return OrnamentsTypeEnum.tremolo8;

            }
            Logger.LogOnce(string.Format("{0}.{1}: Invalid text={2} ",className, functionName, s));
            return OrnamentsTypeEnum.tremolo0;
        }


        private OrnamentsElement(XmlNode node)
        {
            const string function = "OrnamentsElement constructor";
            foreach (XmlNode child in node.ChildNodes)
            {
                switch (child.Name)
                {
                    case "delayed-inverted-turn": ornaments.Add(OrnamentsTypeEnum.delayedInvertedTurn); break;
                    case "delayed-turn": ornaments.Add(OrnamentsTypeEnum.delayedTurn); break;
                    case "inverted-mordent": ornaments.Add(OrnamentsTypeEnum.invertedMordent); break;
                    case "inverted-turn": ornaments.Add(OrnamentsTypeEnum.invertedTurn); break;
                    case "mordent": ornaments.Add(OrnamentsTypeEnum.mordent); break;
                    case "other-ornament": ornaments.Add(OrnamentsTypeEnum.otherOrnament); break;
                    case "schleifer": ornaments.Add(OrnamentsTypeEnum.schleifer); break;
                    case "shake": ornaments.Add(OrnamentsTypeEnum.shake); break;
                    case "tremolo": ornaments.Add(GetTremoloType(child)); break; // Several subtypes exist
                    case "trill-mark": ornaments.Add(OrnamentsTypeEnum.trillMark); break;
                    case "turn": ornaments.Add(OrnamentsTypeEnum.turn); break;
                    case "vertical-turn": ornaments.Add(OrnamentsTypeEnum.verticalTurn); break;
                    case "wavy-line": ornaments.Add(OrnamentsTypeEnum.wavyLine); break;
                    case "accidental-mark": ornaments.Add(GetAccidentalType(child)); break; // Several subtypes exist
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
                case OrnamentsTypeEnum.delayedTurn: return ResourcesForModel.Ornament_DelayedTurn;
                case OrnamentsTypeEnum.invertedMordent: return ResourcesForModel.Ornament_InvertedMordent;
                case OrnamentsTypeEnum.invertedTurn: return ResourcesForModel.Ornament_Turn; ;
                case OrnamentsTypeEnum.mordent: return ResourcesForModel.Ornament_Mordent;
                case OrnamentsTypeEnum.otherOrnament: return ResourcesForModel.Ornament_OtherOrnament;
                case OrnamentsTypeEnum.schleifer: return ResourcesForModel.Ornament_Schleifer;
                case OrnamentsTypeEnum.shake: return ResourcesForModel.Ornament_Shake;
                case OrnamentsTypeEnum.tremolo0: return ResourcesForModel.Ornament_Tremolo;
                case OrnamentsTypeEnum.tremolo1: return ResourcesForModel.Ornament_Tremolo;
                case OrnamentsTypeEnum.tremolo2: return ResourcesForModel.Ornament_Tremolo;
                case OrnamentsTypeEnum.tremolo3: return ResourcesForModel.Ornament_Tremolo;
                case OrnamentsTypeEnum.tremolo4: return ResourcesForModel.Ornament_Tremolo;
                case OrnamentsTypeEnum.tremolo5: return ResourcesForModel.Ornament_Tremolo;
                case OrnamentsTypeEnum.tremolo6: return ResourcesForModel.Ornament_Tremolo;
                case OrnamentsTypeEnum.tremolo7: return ResourcesForModel.Ornament_Tremolo;
                case OrnamentsTypeEnum.tremolo8: return ResourcesForModel.Ornament_Tremolo;
                case OrnamentsTypeEnum.trillMark: return ResourcesForModel.Ornament_TrillMark;
                case OrnamentsTypeEnum.turn: return ResourcesForModel.Ornament_Turn;
                case OrnamentsTypeEnum.verticalTurn: return ResourcesForModel.Ornament_VerticalTurn;
                case OrnamentsTypeEnum.wavyLine: return ResourcesForModel.Ornament_WavyLine;
                case OrnamentsTypeEnum.accidentalMarkUnknown: return ResourcesForModel.Ornament_AccidentalMarkUnknown;
                case OrnamentsTypeEnum.accidentalMarkFlat: return ResourcesForModel.Ornament_AccidentalMarkFlat;
                case OrnamentsTypeEnum.accidentalMarkNatural: return ResourcesForModel.Ornament_AccidentalMarkNatural;
                case OrnamentsTypeEnum.accidentalMarkSharp: return ResourcesForModel.Ornament_AccidentalMarkSharp;
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

        public string UnlocalizedString()
        {
            string delimiter = "";
            StringBuilder sb = new StringBuilder();
            foreach (OrnamentsTypeEnum ornament in ornaments)
            {
                sb.Append(delimiter+ornament.ToString());
                delimiter = ",";
            }
            return sb.ToString();
        }
        
    }
}

