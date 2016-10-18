using System.Xml;

namespace MusicXmlReaderModel
{

    /// <summary>
    /// http://usermanuals.musicxml.com/MusicXML/Content/EL-MusicXML-measure.htm
    /// </summary>
    public class MeasureElement : EventElement
    {

        private int number = 0;
        private bool implicitMeasure = false; // 	Measures with an implicit attribute set to "yes" never display a measure number, regardless of the measure-numbering setting

        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private MeasureElement()
        { }

        

        /// <summary>
        /// Private constructor, used by the Crate() method
        /// </summary>
        /// <param name="node"></param>
        private MeasureElement(XmlNode node)
        {
            const string functionName = "MeasureElement";
            foreach (XmlAttribute a in node.Attributes)
            {
                switch (a.Name)
                {
                    case "number": Utilities.Parse(a.Value, ref number, 0, int.MaxValue, "MeasureElement.Number", false);  break;
                    case "implicit": Utilities.ParseYesNoAttributeValue(functionName, "implicit", a.Value, ref implicitMeasure);  break;
                    case "non-controlling": break; // Explicitly ignore
                    case "width": break; // Explicitly ignore
                    default:  Logger.LogOnce(string.Format("{0}: Unknown attribute. Name='{1}' Value='{2}'", functionName, a.Name, a.Value));break;
                }
            }            
        }

        public int Number
        {
            get
            {
                return number;
            }
        }

        public bool ImplicitMeasure
        {
            get
            {
                return implicitMeasure;
            }
        }

        public static MeasureElement Create(XmlNode node)
        {
            return new MeasureElement(node);
        }

        public override string ToString()
        {
            return null;
            // string s = string.Format("Takt {0}", number);
            // return s;
        }
    }
}
