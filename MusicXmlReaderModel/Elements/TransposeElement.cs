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
                    case "diatonic":
                        // The diatonic element specifies the number of pitch steps needed to go from written to sounding pitch.This allows for correct spelling of enharmonic transpositions.
                        Utilities.Parse(n.InnerText, ref diatonicValue, -7 , +7, "", false); // -7 / +7 is just a guess ! 
                        Logger.LogOnce(string.Format("{0}.{1} diatonic={2} is decoded, but not used yet. ",className, functionName, diatonicValue));
                        break;
                    case "chromatic":
                        // The chromatic element represents the number of semitones needed to get from written to sounding pitch. This value does not include octave-change values
                        // The values for both elements need to be added to the written pitch to get the correct sounding pitch
                        Utilities.Parse(n.InnerText, ref chromaticValue, -12, +12, "", false); // -12 / +12 is just a guess ! 
                        break;
                    case "octave-change":
                        // The octave-change element indicates how many octaves to add to get from written pitch to sounding pitch.
                        Utilities.Parse(n.InnerText, ref octaveChangeValue, -2, +2, "", false); // -2 / +2 is just a guess ! 
                        break;
                    case "double":
                        // If the double element is present, it indicates that the music is doubled one octave down from what is currently written 
                        // (As is the case for mixed cello / bass parts in orchestral literature).
                        doubleValue = true;
                        Logger.LogOnce(string.Format("{0}.{1} doubleValue={2} is decoded, but not used yet. ", className, functionName, doubleValue));
                        break;
                    default:
                        Logger.LogOnce(string.Format("{0}.{1} Unexpected element={2}",className,functionName,n.Name));
                        break;
                }

                // Logger.LogOnce(string.Format("{0}.{1} chromatic={2} diatonic={3} octaveChange={4} double={5} ", className, functionName, diatonicValue, chromaticValue, octaveChangeValue, doubleValue));

            }
        }

        public static TransposeElement Create(XmlNode node)
        {
            return new TransposeElement(node);
        }
           

    }
}
