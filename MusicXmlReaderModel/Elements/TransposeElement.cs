using System;
using System.Xml;

namespace MusicXmlReaderModel
{

    // http://usermanuals.musicxml.com/MusicXML/Content/CT-MusicXML-transpose.htm

    /// <summary>
    /// 
    /// </summary>
    class TransposeElement: EventElement
    {
        const string className = "TransposeElement";
        private int diatonicValue = 0;
        private int chromaticValue = 0;
        private int octaveChangeValue = 0;
        private bool doubleValue = false; 
        private int staffNumberAttribute = 0 ; // 0 "means all staffs for this part"

        public int ChromaticValue
        {
            get
            {
                return chromaticValue;
            }
        }

        public int DiatonicValue
        {
            get
            {
                return diatonicValue;
            }
        }

        public int OctaveChangeValue
        {
            get
            {
                return octaveChangeValue;
            }
        }

        public bool DoubleValue
        {
            get
            {
                return doubleValue;
            }
        }

        /// <summary>
        /// Prevent construction
        /// </summary>
        private TransposeElement()
        {  }

        /// <summary>
        /// Private constructor, used by the Crate() method
        /// </summary>
        /// <param name="node"></param>
        private TransposeElement(XmlNode node)
        {

            const string functionName = "TransposeElement()";

            // Dig out attributes
            foreach (XmlAttribute a in node.Attributes)
            {

                switch (a.Name)
                {
                    case "number": Utilities.Parse(a.Value, ref staffNumberAttribute, 0, 10,"", false); break; // 0 and 10 are just guesses
                    default:
                        Logger.LogOnce(string.Format("{0}.{1} Unknown Attribute.Name={2}", className,functionName,a.Name)); break;
                }
            }

            // Dig out Elements
            foreach (XmlNode n in node.ChildNodes)
            {
                switch (n.Name)
                {
                    case "diatonic": Utilities.Parse(n.InnerText, ref diatonicValue, -7 , +7, "", false); // -7 / +7 is just a guess ! 
                        break;
                    case "chromatic":
                        Utilities.Parse(n.InnerText, ref chromaticValue, -12, +12, "", false); // -12 / +12 is just a guess ! 
                        break;
                    case "octave-change":
                        Utilities.Parse(n.InnerText, ref octaveChangeValue, -2, +2, "", false); // -2 / +2 is just a guess ! 
                        break;
                    case "double":
                        doubleValue = true;
                        break;
                    default:
                        Logger.LogOnce(string.Format("{0}.{1} Unexpected element={2}",className,functionName,n.Name));
                        break;
                }
                Logger.LogOnce(string.Format("{0}.{1} chromatic={2} diatonic={3} octaveChange={4} double={5} ",
                    className, functionName, diatonicValue, chromaticValue, octaveChangeValue, doubleValue));
            }
        }

        public static TransposeElement Create(XmlNode node)
        {
            return new TransposeElement(node);
        }
           

    }
}
