using System;
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
        public override string Caption { get { return ResourcesForModel.NoteElement_measure_text; } }
        private MeasureElement previousMeasureElement; // The previous MeasureElement within this part. Null for the first MeasureElement.
        public MeasureElement PreviousMeasureElement { get { return previousMeasureElement; } set { previousMeasureElement = value; } }

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

        // Used for validation and test only !
        private string partId;
        public string PartId { get { return partId; } set { partId = value; } }

        private Int64 measureDuration;
        public Int64 MeasureDuration
        {
            get { return measureDuration; }
            set { measureDuration = value; }
        }



        public static MeasureElement Create(XmlNode node)
        {
            return new MeasureElement(node);
        }

        public override string ToString()
        {
            string measureString = "";
            if (0 != Number)
            {
                measureString = Number.ToString();
            }
            return measureString;
        }
    }
}
