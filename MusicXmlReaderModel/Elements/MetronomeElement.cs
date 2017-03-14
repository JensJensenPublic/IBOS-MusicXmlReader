using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MusicXmlReaderModel
{
    public class MetronomeElement
    {
        private const string className = "MetronomeElement";
        private string beatsPerMinuteText = "";
        private int beatsPerMinuteInt = -1;
        private bool beatsPerMinuteBool = false;

        /// <summary>
        /// The text value for "per-minute"
        /// </summary>
        public string BeatsPerMinuteText
        {
            get
            {
                return beatsPerMinuteText;
            }
        }

        /// <summary>
        /// The text value for "per-minute" if it could converted to a non-negative integer
        /// -1 otherwise
        /// </summary>
        public int BeatsPerMinuteInt
        {
            get
            {
                return beatsPerMinuteInt;
            }
        }

        /// <summary>
        /// True iff the text value for "per-minute" could be converted to a non-negative integer
        /// </summary>
        public bool BeatsPerMinuteBool
        {
            get
            {
                return beatsPerMinuteBool;
            }
        }

        private MetronomeElement(XmlNode node)
        {
            const string functionName = "MetronomeElement";


            // Ignore attributes. They are all graphic !

            // Dig out elements
            foreach (XmlNode n in node.ChildNodes)
            {
                switch (n.Name)
                {

                    case "per-minute":
                        // In the MusicXml specification "per-minute" is specified as a string, but experiments show that MuseScore
                        // uses it as a Tempo specification. So we decode the value for possible later use.
                        beatsPerMinuteText = n.InnerText;
                        // beatsPerMinuteText = "?"; // For test only !!
                        string errorString = string.Format("{0}.{1}: '{2}'='{3}'", className, functionName, n.Name, (null == beatsPerMinuteText) ? "" : beatsPerMinuteText); // Only for error reporting !
                        beatsPerMinuteBool = Utilities.Parse(beatsPerMinuteText, ref beatsPerMinuteInt, 0, int.MaxValue, errorString, false);
                        //Logger.LogOnce(string.Format("{0}.{1}:  Name={2} Value={3}", className, functionName, n.Name, n.InnerText));
                        Logger.LogOnce(string.Format("{0}.{1}:  Name={2}", className, functionName, n.Name)); // Only count the total number
                        break;  // Number of beats per minute 

                    case "beat-unit": break; // Pure graphical information
                    case "beat-unit-dot": break; // Pure graphical information

                    case "metronome-note":
                    case "metronome-relation":
                        // No current plans for supporting these:
                        Logger.LogOnce(string.Format("{0}.{1}: Known but unsupported element. Name={2} ", className, functionName, n.Name));
                        break;
             
                    case "":
                        Logger.LogOnce(string.Format("{0}.{1}: Known but unsupported element. Name={2} ", className, functionName, n.Name)); break;
                    default: Logger.LogOnce(string.Format("{0}.{1}: Unknown element. Name={2} ", className, functionName, n.Name)); break;
                }
            }

        }

        public override string ToString()
        {
            if (BeatsPerMinuteBool)
            {
                return string.Format("{0}={1}", ResourcesForModel.MetronomeElement_Tempo, beatsPerMinuteInt.ToString()); 
            }
            return "";
        }

        public static MetronomeElement Create(XmlNode node)
        {
            return new MetronomeElement(node);
        }
    }
}
