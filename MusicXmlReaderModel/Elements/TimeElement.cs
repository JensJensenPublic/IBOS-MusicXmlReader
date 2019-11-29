using System.Xml;

namespace MusicXmlReaderModel
{

    // http://usermanuals.musicxml.com/MusicXML/MusicXML.htm#EL-MusicXML-time.htm%3FTocPath%3DMusicXML%2520Reference%7CScore%2520Schema%2520(XSD)%7CElements%7Cattributes%7C_____37

    public class TimeElement : EventElement
    {

        public enum TimeSymbolEnum {unknown, common,cut,singleNumber,note,dottedNote,normal}
        string localizedBeats = "";
        string localizedBeatType = "";
        private TimeSymbolEnum timeSymbol = TimeSymbolEnum.normal;
        public TimeSymbolEnum TimeSymbol { get { return timeSymbol; } }
        private string senzaMisura = "";
        public string SenzaMisura { get { return senzaMisura; } }
        private int staffNumber = 0; // Means "Valid for all staffs"
        public bool IsValidForStaffNumber(int staffNumber)
        {
            return (0 == this.staffNumber || (this.staffNumber == staffNumber));
        }
   

        private int beats;
        private int beatType;
        public override string Caption { get { return ResourcesForModel.TimeElement_pulse; } }



        public int Beats
        {
            get
            {
                return beats;
            }
        }

        public int BeatType
        {
            get
            {
                return beatType;
            }
        }


        public System.Int64 GetMeasureDuration()
        {
            if (0 == beatType)
            {
                string s = ": BeatType=0";
                Logger.LogCFOnce(s);
                throw new MusicXmlParserException(string.Format("TimeElement.MeasureDuration: {0}",s));
            }
            return NoteElement.commonDivisions * 4 * beats / beatType;
        } // NoteElement.commonDivisions is per quarter Note

        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private TimeElement()
        { }


        /// <summary>
        /// Convert from position on the circle of fifths to an (audible) node name
        /// </summary>
        /// <param name="beats"></param>
        /// <returns></returns>
        private string LocalizeBeatType(int beatType)
        {
            string functionName = "LocalizeBeatType";
            switch (beatType)
            {
                case  1: return ResourcesForModel.TimeElement_wholes; // "hele";
                case  2: return ResourcesForModel.TimeElement_halves; // "halve";
                case  4: return ResourcesForModel.TimeElement_quarters; // "fjerdedele";
                case  8: return ResourcesForModel.TimeElement_eights; // "ottendele";
                case 16: return ResourcesForModel.TimeElement_sixteenths; // "sekstendedele";
                case 32: return ResourcesForModel.TimeElement_thirtyseconds; // "toogtredivtedele";
                default:
                    Logger.Log(string.Format("{0}: Illegal value for beatTyoe={1}", functionName, beatType.ToString())); break;
            }
            return "";
        }

        


        /// <summary>
        /// Private constructor, used by the Crate() method
        /// </summary>
        /// <param name="node"></param>
        private TimeElement(XmlNode node)
        {
            string beatsString = "";
            string beatTypeString = "";
            string staffString = "";

            // Dig out attributes
            foreach (XmlAttribute a in node.Attributes)
            {
                switch (a.Name)
                {
                    case "number":
                        Logger.LogCFOnce(string.Format(": StaffNumber found. Value= {0}", a.Value));
                        staffString = a.Value;
                        break;

                    case "symbol":
                        switch (a.Value)
                        {
                            case "common": timeSymbol = TimeSymbolEnum.common; break;
                            case "cut": timeSymbol = TimeSymbolEnum.cut; break;
                            case "single-number": timeSymbol = TimeSymbolEnum.singleNumber; break;
                            case "note": timeSymbol = TimeSymbolEnum.note; break;
                            case "dotted-note": timeSymbol = TimeSymbolEnum.dottedNote; break;
                            case "normal": timeSymbol= TimeSymbolEnum.normal; break;
                            default:
                                Logger.LogCFOnce(string.Format(": Unexpected value of 'symbol'='{0}'", a.Value));
                                break; 
                        }
                        break;

                    case "separator":
                    case "color":
                        Logger.LogCFOnce(string.Format(": Unsupported attribule. Name={0} Value= {1}", a.Name, a.Value));
                        break;

                    default:
                        Logger.LogCFOnce(string.Format(": Unexpected attribule. Name={0} Value= {1}", a.Name, a.Value));
                        break;
                }
            }


            // Dig out elements
            foreach (XmlNode n in node.ChildNodes)
            {
                switch (n.Name)
                {
                    case "beats": beatsString = n.InnerText; break;
                    case "beat-type": beatTypeString = n.InnerText; break;
                    case "senza-misura": senzaMisura = n.InnerText; break;
                    default:
                        Logger.LogCFOnce(string.Format(": Unexpected Childelement. Name={0} InnerText={1}", n.Name, n.InnerText));
                        break;
                }
            }

#warning ToDo Special handling of Beats when SenzaMisura is present!!


            // NOTE The syntax connot be evaluated until all attributes and elements have been dug out !
            bool beatsOk = int.TryParse(beatsString, out this.beats);
            bool beatTypeOk = int.TryParse(beatTypeString, out this.beatType);
            bool staffNumberOK = int.TryParse(staffString, out this.staffNumber);

            if (this.timeSymbol != TimeSymbolEnum.normal)
            {
                // An explicit TimeSymbol has been specified. Note that we log the local variables containing the original strings for beats and beatType !
                // Logger.LogCFOnce(string.Format(": Timesymbol specified as '{0}' Beats='{1}' BeatType='{2}'", this.timeSymbol.ToString(),beatsString,beatTypeString));
            }

            if (!string.IsNullOrEmpty(senzaMisura))
            {
                Logger.LogCFOnce(string.Format(": SenzaMisura  specified as {0}", senzaMisura));
            }
        
            if (!(beatsOk && beatTypeOk && (this.beatType > 0)))
            {
                string s = string.Format(": Invalid TimeElement: Beats='{0}' BeatType='{1}'", beatsString, beatTypeString);
                Logger.LogCFOnce(s);
                Logger.LogCF(s);
                throw new MusicXmlParserException(string.Format("TimeElement.ctor: {0}", s));
            }

            localizedBeats = this.beats.ToString();
            localizedBeatType = LocalizeBeatType(this.beatType);
        }

        public static TimeElement Create(XmlNode node)
        {
            return new TimeElement(node);
        }

        // Represent a text to be used for describing that the BeatType has changed.
        public string NewTimeSignature  { get { return ResourcesForModel.TimeElement_NewTimeSignature; } }

        public override string ToString()
        {
            string symbol = "";

#warning ToDo Discuss this withe experts: "a la breve"  ? etc. Localization ... At
// At least we must avid confusion of this "C" followed by a beattype
// With the note C followed by an octave number aan a duration !

            //            switch (this.timeSymbol)
            //            {
            //
            //                case TimeSymbolEnum.common: symbol = "C "; break;
            //                case TimeSymbolEnum.cut: symbol = "C| "; break;
            //                default: break;
            //            }
            return string.Format("{0}{1} {2}", symbol, localizedBeats, localizedBeatType);
        }

        /// <summary>
        /// For use when debugging MusicBraille
        /// </summary>
        /// <returns></returns>
        public string ToShortString()
        {
            string symbol = "";
            switch (this.timeSymbol)
            {
#warning ToDo Discuss this withe experts: "a la breve"  ? etc. Localization ... Here we might use the UNICODE symbol for C and cut C !!
                case TimeSymbolEnum.common: symbol = "C "; break;
                case TimeSymbolEnum.cut: symbol = "C| "; break;
                default: break;
            }
            return string.Format("{0}{1}/{2}",symbol, beats, beatType);
        }
    }
}
