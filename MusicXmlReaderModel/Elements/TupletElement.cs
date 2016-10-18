using System.Xml;
using System.Globalization;

namespace MusicXmlReaderModel
{
    enum ShowTupletEnum {unknown,actual,both,none};

    class TupletElement : StartStopContinueElement
    {
        private bool bracket;
        private ShowTupletEnum showNumber;
        private ShowTupletEnum showType;

        private ShowTupletEnum GetTypletEnum(string functionName, string name, string value)
        {
            switch (value)
            {
                case "actual": return ShowTupletEnum.actual;
                case "both": return ShowTupletEnum.both;
                case "none": return ShowTupletEnum.none;
                default:
                    Logger.LogOnce(string.Format("{0}: Attribute={1} has unknown value={2}", functionName, name, value)); 
                    return ShowTupletEnum.unknown;
            }
    
        }

        private TupletElement(XmlNode node) : base(node)
        {
            const string functionName = "TupletElement";
            // Dig out attributes that were not handled by the StartStopContinueElement
            foreach (XmlAttribute a in node.Attributes)
            {
                switch (a.Name)
                {
                    case "bracket": Utilities.ParseYesNoAttributeValue(functionName, a.Name, a.Value, ref bracket); break;
                    case "show-number": showNumber = GetTypletEnum(functionName, a.Name, a.Value); break;
                    case "show-type":   showType = GetTypletEnum(functionName, a.Name, a.Value); break;
                    default: break; // Unknown attributes that were handled by the StartStopContinueElement
                }
            }
        }


        public static TupletElement Create(XmlNode node)
        {
            return new TupletElement(node);
        }


        public override string ToString()
        {
            string number = (1 == this.NumberLevel) ? "" : NumberLevel.ToString(); // Ignore the number if it has its default value of 1
            return string.Format("{0} {1} {2}",ResourcesForModel.TupletElement_Name, number, Localize(this.StartStopContinueType));
        }
    }
}