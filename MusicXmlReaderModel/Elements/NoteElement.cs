using System;
using System.Xml;
using System.Globalization;
using JSJ.MusicSynthesis;


namespace MusicXmlReaderModel
{

    /// <summary>
    /// As specified by MusicXML, but modified according to use C# sytax
    /// Also used to describe nurations of rests
    /// The ndtmeasure is not a part of the MusicXml definition but is used to describe a note with the "pullmeasure"= "yes" attribute
    /// </summary>
    public enum NoteTypeEnum 
    {
        unknown,    // We know absolute nothing about the value
        nt1024th,   // Can't start with a number
        nt512th,    // Can't start with a number
        nt256th,    // Can't start with a number
        nt128th,    // Can't start with a number
        nt64th,       // Can't start with a number
        nt32nd,       // Can't start with a number
        nt16th,       // Can't start with a number
        eight,
        quarter,
        half,
        whole,
        breve,
        longus,     // Long is a C# keyword
        maxima,
        measure,
        unspecifiedRest // HACK: We use this value for rests which nave no type and do not contain the "measure" = "yes" attribute
                        // This situation seems to be interpreted as a full measure rest by MuseScore - and for the time being we do the same !
                        // TO DO Find a better solution 
    }

  

    public class NoteElement : EventElement
    {

        // https://usermanuals.musicxml.com/MusicXML/Content/CT-MusicXML-note.htm

        // Allows for representing the following subdivisions of a quarter node:
        // 2,3,4,5,6,7,8,9,10. 
        // 1260 can be divided by 2,3,4,5,6,7,8,9 and 10 !
        // public const int commonDivisions = 1260;
        // While testing version 0.8.0.0 it was found that Sibelius may use divisions=1024
        // So commonDivisions was changed so it can be divided by 1024 as well !
        public const Int64 commonDivisions = 322560; // 2^10 * 3^2 * 5 * 7
        string punctured = ResourcesForModel.NoteElement_dotted + " "; // "punkteret ";


        // Values found in MusicXml file, possibly after a minor type conversion, typically from string to int.
        //FullStepEnum step = FullStepEnum.Unknown ;   // Represents a diatonoc step: A,B,C,D,E,F, G or a pause
        //int alter = 0 ;     // Represents the number of semitones the note is altered. 
        //int octave = 0;
        int duration = 0;   // The value of the child element "duration"
        bool chord = false; // Means that this note starts at the same time as the previous note, not after the previous note.
        //string type = "unspecified";
        NoteTypeEnum noteDuration = NoteTypeEnum.unknown;
        int voice = 0;
        bool dot = false;
        int divisions = 0; 
        TieElement tieElement;             // A NoteElement may contain a nested TieElement  (Danish: "Bindebue")
        RestElement restElement;   // A NoteElement may contain a nested RestElement
        NotationsElement notations; // A NoteElement may contain a nested NotationsElement     
        string tieType = "";        // Is this note tied to another note
        bool tieStart = false;      // This note is tied to a note later   in the score
        bool tieStop = false;       // This note is tied to a note earlier in the score
        string localizedType = "";      //  If this is n note,  not a pause
        string localizedPauseType = ""; //  If this is a pause, not a note
        string localizedTie = "";
        PitchElement pitchElement;
        UnpitchedElement unpitchedElement;
        TransposeElement transposeElement; // Used if the cuttent scorepart describes an "non-C" instrument such as an A-clarinet.
        string syllabic; // Child of lyric
        string text;     // Child of lyric
        LyricElementList lyricElementList = LyricElementList.Create("");
        string staffString = "";
        int staff = 1;
        //string articulations = "";
        InstrumentElement instrumentElement; // The instrument type distinguishes between score-instrument elements in a score-part. The id attribute is an IDREF back to the score-instrument ID.
                                             //If multiple score-instruments are specified on a score-part, there should be an instrument element for each note in the part.
        AccidentalElement accidentalElement;
        NoteHeadElement noteHeadElement; // Special graphical variants of hoathead
        TimeModificationElement timeModificationElement; // Tuplet information 
        public TimeModificationElement TimeModificationElement { get { return timeModificationElement; } } // Tuplet information 

        // MeasureNumber and MeasureNumber are not found inside the XML describing the note, but are derived from the XML surrounding the note.
        int measureNumber;
        ScorePartElement scorePartElement; // Holds a reference to the ScorePartelement describing the score part for this note
        TimeElement currentTimeElement; // Holds a reference to the TimeElement describing this note

        MidiNote midiNote = null; // If !null holds a MidiNote curently being played and representing this NoteElement

        // Simple booleans describing special variants of notes
        bool unpitched; // Set if the note is marked as unpitched
        bool isCueNote; // Set if the note is marked as a cue note
        bool graceNote; // Set if the note is marked as a grace note 

        // Values directly contained as attributes to the NoteElement
        bool measureAttributeValue = false;    // Default: This object is not a full measure pause
        bool printObjectAttributeValue = true; // Default: This object should be printed

        const string className = "NoteElement";
        float dynamicsFloatValue = 100;
        int dynamicsValue = 90;
        bool isFirstNoteInScorePart;
        int midiUnpitchedInstrumentNumber;
        string scoreUnpitchedInstrumentName;

        private BrailleInAccordInfo brailleMeasureDivisionInfo;
        public  BrailleInAccordInfo BrailleMeasureDivisionInfo { get { return brailleMeasureDivisionInfo; } set { brailleMeasureDivisionInfo = value; } }


        public override int GetStaffNumber()
        {
            return this.staff;
        }

        public int MidiUnpitchedInstrumentNumber
        {
            get
            {
                return midiUnpitchedInstrumentNumber;
            }
        }

        public string ScoreUnpitchedInstrumentName
        {
            get
            {
                return scoreUnpitchedInstrumentName;
            }
        }



        public PitchElement.FullStepEnum Step
        {
            get
            {
                return pitchElement.Step;
            }
        }

        public bool Pitched
        {
            get
            {
                return ((null != pitchElement) && (pitchElement.Step != PitchElement.FullStepEnum.Unknown));
            }
        }

        public bool UnPitched
        {
            get
            {
                return unpitched;
            }
        }

        public UnpitchedElement UnpitchedElement
        {
            get
            {
                return unpitchedElement;
            }
        }

        public int Transpose
        {
            get
            {
                return (null == transposeElement) ? 0 : (transposeElement.ChromaticValue + (12 * transposeElement.OctaveChangeValue));
            }
        }


        public int Octave
        {
            get
            {
                {
                    return pitchElement.Octave;
                }
            }
        }

#warning ToDO Use !

        // Suggested by Corine and Bert 
        public string OctaveName
        {
            get
            {
                switch (pitchElement.Octave)
                {
                    case 1: return ResourcesForModel.OctaveName_Subcontra;
                    case 2: return ResourcesForModel.OctaveName_Contra;
                    case 3: return ResourcesForModel.OctaveName_Large;
                    case 4: return ResourcesForModel.OctaveName_Small;
                    case 5: return ResourcesForModel.OctaveName_First;
                    case 6: return ResourcesForModel.OctaveName_Second;
                    case 7: return ResourcesForModel.OctaveName_Third;
                    case 8: return ResourcesForModel.OctaveName_Fourth;
                    default: return ResourcesForModel.OctaveName_Octave + " " + pitchElement.Octave;
                }
            }
        }


        public int Duration
        {
            get
            {
                return duration;
            }
  
        }

        /// <summary>
        /// https://usermanuals.musicxml.com/MusicXML/Content/EL-MusicXML-voice.htm
        /// </summary>
        public int Voice
        {
            get
            {
                return voice;
            }
        }

        public int Divisions
        {
            get
            {
                return divisions;
            }
        }

        public int Alter
        {
            get
            {
                return pitchElement.Alter;
            }
        }

        public string Tie
        {
            get
            {
                return tieType;
            }
        }

        public bool TieStop
        {
            get
            {
                return tieStop;
            }
        }

        ///// <summary>
        ///// Unit is milliSeconds. Is 0 at start of part.
        ///// </summary>
        //public int StartTime
        //{
        //    get
        //    {
        //        return startTime;
        //    }

        //    set
        //    {
        //        startTime = value;
        //    }
        //}

        public Int64 DurationInCommonDivisions
        {
            get
            {
                return (duration * commonDivisions) / divisions;
            }


        }

        /// <summary>
        /// The Id of the part to which this note belongs
        /// Example: "P1"
        /// </summary>
        public string PartId
        {
            get
            {
                return scorePartElement.partId;
            }
        }

        /// <summary>
        /// The number of the part to which this note belongs
        /// Example 0
        /// </summary>
        public int PartNumber
        {
            get
            {
                return scorePartElement.partNumber;
            }
        }

 
        public ScorePartElement ScorePartElement
        {
            get
            {
                return scorePartElement;
            }
        }


        /// <summary>
        /// The official way of specifying that this NoteElement is a rest
        /// </summary>
        public bool IsRest
        {
#warning TODO Attempt to use IsRest instead of (the self-invnted) IsPause
            get { return null != restElement; }
        }


        /// <summary>
        /// Per definition a note without a pitch is a pause ! Compare to IsRest
        /// </summary>
        public bool IsPause
        {
            get
            {
                return (null == pitchElement);
            }
        }

        /// <summary>
        /// Returns 0 for rests and for unpitched notes!
        /// </summary>
        public int PitchInSemitonesAboveC0
        {
            get
            {
                if (null != restElement) return 0; // A
                if (null == pitchElement) return 0;
                return pitchElement.SemiTonesAboveC0;
            } 
        }


        public PitchElement PitchValue
        {
            get
            {
                return pitchElement;
            }
            
        }

        public bool Chord
        {
            get
            {
                return chord;
            }
        }

        public string Syllabic
        {
            get
            {
                return syllabic;
            }
                    }

        public string Text
        {
            get
            {
                return text;
            }
        }

        public int Staff
        {
            get
            {
                return staff;
            }
        }

        public MidiNote MidiNote
        {
            get
            {
                return midiNote;
            }

            set
            {
                midiNote = value;
            }
        }

        public string LocalizedType
        {
            get
            {
                return localizedType;
            }
        }

        public string LocalizedPauseType
        {
            get
            {
                return localizedPauseType;
            }
        }

        public int MidiChannel
        {
            get
            {
                return scorePartElement.MidiChannel; // Use channel 1 as a default
            }
        }

        public int MidiProgram
        {
            get
            {
                return scorePartElement.MidiProgram;
            }
        }

        public float MidiVolume
        {
            get
            {
                return scorePartElement.MidiVolume;
            }
        }

        public bool GraceNote
        {
            get
            {
                return graceNote;
            }
        }

        public bool CueNote
        {
            get
            {
                return isCueNote;
            }
        }

        public int MeasureNumber
        {
            get
            {
                return measureNumber;
            }
        }

        public NotationsElement Notations
        {
            get
            {
                return notations;
            }
        }

        public NoteTypeEnum NoteDuration
        {
            get
            {
                return noteDuration;
            }
        }

        public bool PrintObjectAttributeValue
        {
            get
            {
                return printObjectAttributeValue;
            }
        }

        public bool Dot
        {
            get
            {
                return dot;
            }
        }

        internal TransposeElement TransposeElement
        {
            get
            {
                return transposeElement;
            }

            set
            {
                transposeElement = value;
            }
        }

        public int DynamicsIntValue
        {
            get
            {
                return dynamicsValue;
            }
        }

        public bool TieStart
        {
            get
            {
                return tieStart;
            }
        }


        public bool SlurStart
        {
            get
            {
                if (null == Notations) return false;
                if (null == Notations.SlurElement) return false;
                return (StartStopContinueElement.StartStopContinueTypeEnum.Start == Notations.SlurElement.StartStopContinueType);
            }
        }

        public AccidentalElement AccidentalElement
        {
            get
            {
                return accidentalElement;
            }
        }

        public bool IsFirstNoteInScorePart
        {
            get
            {
                return isFirstNoteInScorePart;
            }
        }  

        public string CueNoteString
        {
            get
            {
                return isCueNote ? ResourcesForModel.NoteElement_CueNote : "";
            }
        }

        public string UnpitchedText
        {
            get
            {
                // Take the name from the ScoreInstrument. Take the number from the midiInstrument 
                return string.Format("{0} ({1}={2})", this.ScoreUnpitchedInstrumentName, midiUnpitchedInstrumentNumber, (UnpitchedMidiInstrumentEnum)midiUnpitchedInstrumentNumber);
            }


//            get
//            {
//                UnpitchedMidiInstrumentEnum unpitchedMidiInstrument = (UnpitchedMidiInstrumentEnum)midiUnpitchedInstrumentNumber;
//                if (MidiNote.IsKnownUnpitchedMidiInstrument(unpitchedMidiInstrument))
//                {
//                    return string.Format("{0}({1})", unpitchedMidiInstrument.ToString(), ((int)unpitchedMidiInstrument).ToString());
//#warning ToDo  Implement localization of names of unpitched instruments
//                }
//                else
//                {
//                    return string.Format("{0}({1})",ResourcesForModel.NoteElement_unpitched_text, unpitchedMidiInstrument);
//                }
//            }
        }

        public InstrumentElement InstrumentElement
        {
            get
            {
                return instrumentElement;
            }
        }


        
        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private NoteElement()
        { }


        /// <summary>
        /// Attempt to localize note types
        /// </summary>
        /// <param name="typeString"></param>
        /// <param name="modifier"></param>
        /// <returns></returns>
        private string LocalizeType(NoteTypeEnum noteDuration, bool modifier) 
        {
            string modif = (modifier ? punctured : "");
            string value = "";
            switch (noteDuration)
            {
                case NoteTypeEnum.breve: value = ResourcesForModel.NoteElement_breve; break;    // "dobbelthelnode"; break;
                case NoteTypeEnum.whole: value = ResourcesForModel.NoteElement_whole; break;    // "helnode"; break;
                case NoteTypeEnum.half: value = ResourcesForModel.NoteElement_half; break;     //"halvnode"; ; break;
                case NoteTypeEnum.quarter: value = ResourcesForModel.NoteElement_quarter; break;  // "fjerdedel"; break;
                case NoteTypeEnum.eight: value = ResourcesForModel.NoteElement_eight; break;    // "ottendedel"; break;
                case NoteTypeEnum.nt16th: value = ResourcesForModel.NoteElement_16th; break;      // "sekstendedel"; break;
                case NoteTypeEnum.nt32nd: value = ResourcesForModel.NoteElement_32nd; break;     //"toogtredivtedel"; break;
                case NoteTypeEnum.nt64th: value = ResourcesForModel.NoteElement_64th; break;     //"fireogtredsindstyvendedel"; break;
                case NoteTypeEnum.nt128th: value = ResourcesForModel.NoteElement_128th; break;     //"hundredeogotteogtyvendedeltyvendedel"; break;
                case NoteTypeEnum.measure: value = ResourcesForModel.NoteElement_measure; break;   //  "heltakt"; break;
                case NoteTypeEnum.unknown: value = "ukendt"; break;
                default:                    
                    Logger.LogOnce(string.Format("LocalizeType ({0},{1}) Unknown duration '{2}' in Measure={3} Voice={4} PartId={5} PartNumber={6}",
                                                 noteDuration.ToString(), modifier, noteDuration.ToString(), measureNumber, voice,PartId,PartNumber)); break;
            }
            return modif + value; // 
        }

        /// <summary>
        /// Attempt to localize names of pauses seperately.
        /// Note that in Danish an "s" is sometimes, sometimes not used as glue!!!
        /// </summary>
        /// <param name="typeString"></param>
        /// <param name="modifier"></param>
        /// <returns></returns>
        private string LocalizePause(NoteTypeEnum noteDuration, bool modifier) 
        {
            string modif = (modifier ? punctured : "");
            string value = "";
            switch (noteDuration)
            {
                case NoteTypeEnum.whole: value =    ResourcesForModel.NoteElement_whole_rest; break;    // "helnodepause"; break;
                case NoteTypeEnum.half: value =     ResourcesForModel.NoteElement_half_rest; break;     // "halvnodepause"; ; break;
                case NoteTypeEnum.quarter: value =  ResourcesForModel.NoteElement_quarter_rest; break;  // "fjerdedelspause"; break;
                case NoteTypeEnum.eight: value =    ResourcesForModel.NoteElement_eight_rest; break;    // "ottendedelspause"; break;
                case NoteTypeEnum.nt16th: value =   ResourcesForModel.NoteElement_16th_rest; break;     // "sekstendedelspause"; break;
                case NoteTypeEnum.nt32nd: value =   ResourcesForModel.NoteElement_32nd_rest; break;     // "toogtredivtedel"; break;
                case NoteTypeEnum.nt64th: value =   ResourcesForModel.NoteElement_64th_rest; break;     // "fireogtredsindstyvendedelspause"; break;
                case NoteTypeEnum.measure: value =  ResourcesForModel.NoteElement_measure_rest; break;  // "heltaktspause"; break;
                case NoteTypeEnum.unknown: value =  ResourcesForModel.NoteElement_unknown_rest; break;  // "ukendt"; break;
                default:
                    Logger.LogOnce(string.Format("LocalizePause({0},{1}) Unknown duration '{2}' in Measure={3} Voice={4}",
                        duration.ToString(), modifier, duration.ToString(), measureNumber, voice)); break;
            }
            return modif + value;
        }

        private string LocalizeTie(string tieType) 
        {
            switch (tieType)
            {
                case "start": return ResourcesForModel.NoteElement_tie_start; // "bue start";
                case "stop" : return ResourcesForModel.NoteElement_tie_stop;  // "bue slut";
                case ""     :  return "";
                default: Logger.Log(string.Format("LocalizeTie({0}) Unknown tieType '{1}'", tieType, tieType)); return "";     
            }
        }


        public NoteTypeEnum GetDuration(string s)
        {
            switch (s)
            {
                case "breve": return NoteTypeEnum.breve; // A double whole note
                case "whole": return NoteTypeEnum.whole;
                case "half": return NoteTypeEnum.half;
                case "quarter": return NoteTypeEnum.quarter;
                case "eighth":return NoteTypeEnum.eight;
                case "16th": return NoteTypeEnum.nt16th;
                case "32nd": return NoteTypeEnum.nt32nd;
                case "64th": return NoteTypeEnum.nt64th;
                case "128th": return NoteTypeEnum.nt128th;
                case "measure": return NoteTypeEnum.measure;
                case "unspecified": return NoteTypeEnum.unknown;
                default:  Logger.LogOnce(string.Format("{0}.{1}: Unknown Fullstep value={2}", className, "GetDuration", s));
                    return NoteTypeEnum.unknown;
            }
        }

        private Int64 NoteTypeEnumToInt64(NoteTypeEnum nte)
        {       
            switch (nte)
            {
                case NoteTypeEnum.nt1024th:  return commonDivisions / 256;
                case NoteTypeEnum.nt512th:   return commonDivisions / 128;
                case NoteTypeEnum.nt256th:   return commonDivisions / 64;
                case NoteTypeEnum.nt128th:   return commonDivisions / 32;
                case NoteTypeEnum.nt64th:    return commonDivisions / 16;
                case NoteTypeEnum.nt32nd:    return commonDivisions / 8;
                case NoteTypeEnum.nt16th:    return commonDivisions / 4;
                case NoteTypeEnum.eight:     return commonDivisions / 2;
                case NoteTypeEnum.quarter:   return commonDivisions * 1;
                case NoteTypeEnum.half:      return commonDivisions * 2;
                case NoteTypeEnum.whole:     return commonDivisions * 4;
                case NoteTypeEnum.breve:
                case NoteTypeEnum.longus: 
                case NoteTypeEnum.maxima: 
                case NoteTypeEnum.measure:
                case NoteTypeEnum.unspecifiedRest:
                default:                    return -1;
            }
        }


        private int NoteTypeEnumToDenominator(NoteTypeEnum nte)
        {
            switch (nte)
            {
                case NoteTypeEnum.nt1024th: return 1024;
                case NoteTypeEnum.nt512th: return 512;
                case NoteTypeEnum.nt256th: return 256;
                case NoteTypeEnum.nt128th: return 128;
                case NoteTypeEnum.nt64th: return 64;
                case NoteTypeEnum.nt32nd: return 32;
                case NoteTypeEnum.nt16th: return 16;
                case NoteTypeEnum.eight: return 8;
                case NoteTypeEnum.quarter: return 4;
                case NoteTypeEnum.half: return 2;
                case NoteTypeEnum.whole: return 1;
                case NoteTypeEnum.breve:
                case NoteTypeEnum.longus:
                case NoteTypeEnum.maxima:
                case NoteTypeEnum.measure:
                case NoteTypeEnum.unspecifiedRest:
                default: return -1;
            }
        }




        /// <summary>
        /// Experimental code!
        /// Used for computing the exact note duration without rounding errors.
        /// Needed to compute the exact position for instance in case of swing notes and triplets, where we can not rely on the "duration" element
        /// </summary>
        /// <param name="nte"></param>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        public Int64 ArithmeticDurationInCommonDivisions
        {
            get
            {
                Int64 rawDuration = NoteTypeEnumToInt64(this.noteDuration);
                if (-1 == rawDuration)
                {
                    Logger.LogCF(string.Format(" = {0}: Raw duration can not be evaluated for {1}", rawDuration,this.noteDuration.ToString()));
                    return -1;
                }
                if (null == this.timeModificationElement)
                {
                    // Logger.LogCF(string.Format(" = {0}: No time modification found", rawDuration));
                    // return rawDuration; // This is not a tuplet
                    return -1;// This is not a tuplet
                }
                // This is a tuplet. The exact duration can be represented as an integer. See comments above to "commonDivisions"
                int actual = this.timeModificationElement.ActualNotes;
                int normal = this.timeModificationElement.NormalNotes;
                Int64 exactDuration = rawDuration * normal / actual;
                // Logger.LogCF(string.Format(" = {0}: Duration={1} ActualNotes={2} NormalNotes={3}", exactDuration, this.noteDuration.ToString(),actual, normal));
                return exactDuration;     
            }
        }

        public LyricElementList LyricElementList { get => lyricElementList; }


        //public string TupleDurationString()
        //{
        //    int actual = 1;
        //    int normal = 1;
        //    if (null == this.timeModificationElement)
        //    {
        //        Logger.LogCFOnce(": TimeModificationElement is null. Using Actual=1 Normal=1");
        //    }
        //    else
        //    {
        //        actual = this.timeModificationElement.ActualNotes;
        //        normal = this.timeModificationElement.NormalNotes;
        //    }
        //    Int64 rawDuration = NoteTypeEnumToInt64(this.noteDuration);
        //    int denominator = NoteTypeEnumToDenominator(this.noteDuration);
        //    //Logger.LogCF(string.Format(": {0} Actual={1} Normal={2}", this.noteDuration.ToString(), actual, normal));
        //    // string result =  string.Format("{0}/{1}", rawDuration * normal / commonDivisions, actual);
        //    string result = (-1 == denominator) ? "?" : string.Format("{0}/{1}", normal, actual * denominator);
        //    Logger.LogCF(string.Format(": NoteDuration={0} Actual={1} Normal={2} returns {3}", this.noteDuration.ToString(), actual, normal, result));
        //    return result;
        //}

        public TupletFraction TupleDuration()
        {
            int actual = 1;
            int normal = 1;
            if (null == this.timeModificationElement)
            {
                Logger.LogCFOnce(": TimeModificationElement is null. Using Actual=1 Normal=1");
            }
            else
            {
                actual = this.timeModificationElement.ActualNotes;
                normal = this.timeModificationElement.NormalNotes;
            }
            int duration = NoteTypeEnumToDenominator(this.noteDuration);
            if (this.dot)
            {
#warning ToDo  Find a beter way to represent this !!!!!
                normal = normal * 3;
                actual = actual * 2;
            }
            //IntegerFraction result = new TupletFraction(normal, actual * denominator);
            TupletFraction result = TupletFraction.Create(normal, actual , duration,LocalizeType(noteDuration, false));
            // Logger.LogCF(string.Format(": NoteDuration={0} Actual={1} Normal={2} returns {3}/{4}", this.noteDuration.ToString(), actual, normal, result.Nominator,result.Denominator));
            return result;
        }


        /// <summary>
        /// Simple convenience method
        /// </summary>
        private void CheckScorePartElement()
        {
            if (null == scorePartElement)
            {
                // In this way we explicitly log the error cause, but also prevents further useless attempts to load the file.
                string s = ": scorePartElement is null";
                Logger.LogCFOnce(s);
                throw new MusicXmlParserException(string.Format("NoteElement.ctor {0}", s));
            }
        }

        /// <summary>
        /// Simple convenience method
        /// </summary>
        private void  CheckInStrumentElementId()
        {
            string s = "";
            if (null == this.InstrumentElement)
            {
                s = ": InstrumentElement is null";
            }
            else
            {
                if (null == this.InstrumentElement.Id)
                {
                    s = ": InstrumentElement.Id is null";
                }
            }

            if (string.IsNullOrEmpty(s)) return;

            Logger.LogCFOnce(s);
            throw new MusicXmlParserException(string.Format("NoteElement.ctor {0}", s));
        }

        /// <summary>
        /// Simple convenience method
        /// </summary>
        /// <param name="midiInstrumentElement"></param>
        private void CheckMidiInstrumentElement(MidiInstrumentElement midiInstrumentElement)
        {
            if (null == midiInstrumentElement)
            {
                string s = ": MidiInstrumentElement is null";
                Logger.LogCFOnce(s);
                throw new MusicXmlParserException(string.Format("NoteElement.ctor {0}", s));
            }
        }



        // http://usermanuals.musicxml.com/MusicXML/Content/EL-MusicXML-note.htm


        private NoteElement(XmlNode xmlNode, int divisions, int measureNumber, ScorePartElement scorePartElement, TimeElement currentTimeElement) // New version
        {
            this.divisions = divisions;
            this.scorePartElement = scorePartElement;
            this.measureNumber = measureNumber;
            this.currentTimeElement = currentTimeElement;
            const string functionName = "NoteElement"; // For logging            
            //this.partId = scorePartElement.partId;
            //this.partNumber = scorePartElement.partNumber;
            //this.midiChannel = (null == scorePartElement.midiInstrumentElement) ? 1 : scorePartElement.midiInstrumentElement.MidiChannel; // Use channel 1 as a default

            CheckScorePartElement();

            // Dig out attributes
            foreach (XmlAttribute a in xmlNode.Attributes)
            {
                switch (a.Name)
                {
                    case "measure": Utilities.ParseYesNoAttributeValue(functionName, a.Name, a.Value, ref measureAttributeValue); break;
                    case "print-object": Utilities.ParseYesNoAttributeValue(functionName, a.Name, a.Value, ref printObjectAttributeValue); break;
                    case "default-x":
                    case "default-y":
                    case "relative-x":
                    case "relative-y":
                    case "font-family":
                    case "font-style":
                    case "font-size":
                    case "font-weight":
                    case "color":
                    case "print-dot":
                    case "print-spacing":
                    case "print-lyric":
                        break; // Explicitly ignore some graphical attributes 

                    case "dynamics":
                        if (Utilities.Parse(a.Value, ref dynamicsFloatValue, (float)0, (float)1000, "NoteElement: Invalid value of dynamics"))
                        {
                            dynamicsValue = (int)((float)(0.90) * dynamicsFloatValue);
                        }
                        // http://usermanuals.musicxml.com/MusicXML/Content/EL-MusicXML-note.htm
                        // Logger.LogOnce(string.Format("{0}.{1} Unimplemented attribute. Name='{2}' Value is parsed but not used", className, functionName, a.Name));
                        break;
                    case "end-dynamics":
                    case "attack":
                    case "release":
                    case "time-only":
                    case "pizzicato":
                        Logger.LogOnce(string.Format("{0}.{1} Unimplemented attribute. Name={2}", className, functionName, a.Name));
                        break; // Not implemented yet

                    default:
                        Logger.LogOnce(string.Format("{0}.{1} Unexpected attribute. Name={2} Value={3}", className, functionName, a.Name, a.Value));
                        break;

                }
            }


            foreach (XmlNode child in xmlNode.ChildNodes)
            {
                bool unimplemented = false; 
                switch (child.Name)
                {
                    case "pitch":
                        // The pitch represents the sound, not what is notated, so an alter element must be included even if it represents a flat or sharp
                        // that is part of the key signature. This is why the E-flat contains an alter element, though there is no accidental on the note.
                        pitchElement = PitchElement.Create(child); // NOTE! Returns null if no step is specified for the pitch
                        transposeElement = scorePartElement.TransposeElement;
                        break;
                    case "duration": duration = int.Parse(child.InnerText); break;
                    // The duration element is an integer that represents a note’s duration in terms of divisions per quarter note
                    // JSJ: "Represent the sound, not what is notated, for example in the case of swing notes
                    case "chord": chord = true; break;
                    case "type":
                        noteDuration = GetDuration(child.InnerText);
                        if (NoteTypeEnum.unknown == noteDuration)
                        {
                            Logger.LogOnce(string.Format("{0}: Unknown typeString '{1}' in Measure={2} Voice={3}",
                                functionName, child.InnerText, measureNumber, voice)); break;
                        }
                        break;
                    // type = child.InnerText;
                    case "voice":
                        voice = int.Parse(child.InnerText);
                        //Logger.LogCFOnce(string.Format(": {0}: Voice={1}",System.IO.Path.GetFileName(Model.TheStaticXmlFileName),voice)); // Temporarily for debugging !
                        break;
                    case "dot": dot = true; break;
                    case "tie":
                        tieElement = TieElement.Create(child);
                        tieType = tieElement.TieType;
                        if ("start" == tieType)
                        {
                            tieStart = true;
                        }
                        if ("stop" == tieType)
                        {
                            tieStop = true;
                        }
                        break;
                    case "lyric":
                        text = Utilities.GetChildValue(child, "text");
                        syllabic = Utilities.GetChildValue(child, "syllabic");
                        #region New code November 2024
                        LyricElement lyricElement = LyricElement.Create(child);
                        lyricElementList.AddElement(lyricElement);                    
                        #endregion
                        break;
                    case "staff":
                        staffString = child.InnerText;
                        staff = int.Parse(staffString);
                        break;
                    case "notations": // TO DO: Find out what to do here 
                        notations = NotationsElement.Create(child);
                        break;
                    case "beam": // http://usermanuals.musicxml.com/MusicXML/Content/EL-MusicXML-beam.htm
                        // This is pure graphical information. Explicitly ignored!
                        break;
                    case "rest":
                        //The RestElement is just a cleaner way of specifying a rest/pause instead of using a noteElement with no pitch! 
                        restElement = RestElement.Create(child);
                        if (restElement.MeasureAttributeValue)
                        {
                            noteDuration = NoteTypeEnum.measure; // This Rest covers a full measure
                        }
                        break;
                    case "accidental": // http://usermanuals.musicxml.com/MusicXML/Content/EL-MusicXML-accidental.htm
                        accidentalElement = AccidentalElement.Create(child);
                        break;
                    case "time-modification":   // http://usermanuals.musicxml.com/MusicXML/Content/EL-MusicXML-time-modification.htm  
                        timeModificationElement = TimeModificationElement.Create(child, this);
                        break;
                    case "instrument":
                        instrumentElement = InstrumentElement.Create(child);
                        break;
                    case "stem": // http://usermanuals.musicxml.com/MusicXML/Content/EL-MusicXML-stem.htm 
                        // This is pure graphical information. Explicitly ignored!
                        break;
                    case "grace": // TO DO: Find out what to do here
                        // Mark this note as a grace note, i.e a note not taking part of the normal timing mechanisms.
                        // Grace notes may be implemented later, for now they are just ignored while building eventlists. 
                        graceNote = true;
                        break;
                    case "unpitched":
                        unpitched = true;
                        // Logger.LogOnce(string.Format("{0}: Unpitched note is not completely implemented yet", functionName));
                        unpitchedElement = UnpitchedElement.Create(child);     
                        break; // Just mark the note as unpitched
                    case "cue": // http://usermanuals.musicxml.com/MusicXML/Content/EL-MusicXML-cue.htm
                        isCueNote = true; break; ; // Just mark the note as a cue note
                    case "notehead": // http://usermanuals.musicxml.com/MusicXML/Content/CT-MusicXML-notehead.htm
                        noteHeadElement = NoteHeadElement.Create(child);
                        // Logger.LogOnce(string.Format("{0}: NoteHeadElement is decoded to '{1}' but not used yet.", functionName, noteHeadElement.ToString()));
                        break;
                    //default:  throw new ArgumentException();
                    default: Logger.LogOnce(string.Format("{0}.{1}: Unknown child element. Name='{2}'", className, functionName, child.Name)); break;
                }
                if (unimplemented)
                {
                    Logger.LogOnce(string.Format("{0}: child.Name '{1}' is not implemented yet", functionName, child.Name));
                }

                if ((null == pitchElement) && unpitched && (null != unpitchedElement)) // Experiment for handling unpitched notes
                {
                    // Logger.LogOnce(string.Format("{0}: Replacing unpitched note displayed as {1}{2} by pitched equivalent",
                    //    functionName, unpitchedElement.DisplayStep, unpitchedElement.DisplayOctave));
                    pitchElement = PitchElement.Create(unpitchedElement.DisplayStep, unpitchedElement.DisplayOctave);
                }

                CheckDuration(); // Primarily for debugging. Checks that the value of commonDivisions is large enough! The call can be omitted!

            }

            noteDuration = GetNoteDuration();

 

            if (noteDuration == NoteTypeEnum.unknown)
            {
#warning ToDo Localize
                // In some rate cases the NoteElement does not contain a Type value
                // For instance Tremolo can be implemented by a lot of audible, but invisible noteElements with no NoteType value,
                // but with the the physical duration specified by the "Duration" element.
                // In that case we report the duration relative to a full measure: 
                float durationValue = duration / (float)(4 *divisions); // 4 because we show the value relative to a measure, not a quarter note
                localizedType = string.Format("{0}={1} {2}={3:0.000} {4}",
                    ResourcesForModel.NoteElement_NoteType, // 0
                    ResourcesForModel.NoteElement_unknown,  // 1
                    ResourcesForModel.NoteElement_Duration, // 2
                    durationValue, // 3
                    ResourcesForModel.NoteElement_MeasureString); // 4
            }
            else
            {
                localizedType = LocalizeType(noteDuration, dot);
            }

            localizedPauseType = (IsPause) ? LocalizePause(noteDuration, dot) : "";
            localizedTie = LocalizeTie(tieType);


            // Model.GetNoteTiming(out this.startTime, out this.endTime, int.Parse(this.duration));


            if (!scorePartElement.HasNotes)
            {
                // Mark this note as the first note in the score
                isFirstNoteInScorePart = true;
                scorePartElement.HasNotes = true;
            }

            // Allow the TupletElement to access the timeModification Element in order to describe tuplets in details
            if ((null != notations) && (null != notations.TupletElement))
            {
                notations.TupletElement.TimeModificationElement = this.timeModificationElement;
            }

            // We need to be sure that all elements have been interpreted before we can handle unpitched notes.
            if (unpitched)
            {

                CheckInStrumentElementId();

                ScoreInstrumentElement scoreInstrumentElement = scorePartElement.GetScoreInstrument(this.InstrumentElement.Id);
                // int nnn = midiInstrumentElement.MidiUnpitchedInstrumentNumber;
                if (scoreInstrumentElement.IsVirtualInstrument)
                {
                    // This is a virtual instrument, not a Midi instrument.   
                    this.midiUnpitchedInstrumentNumber = (int)MidiInstrumentMap.GetUnpitchedMidiInstrumentNumber(scoreInstrumentElement.VirtualInstrumentElement);
                }
                else
                {
                    // This is a Midi instrument, not a virtual instrument.
                    MidiInstrumentElement midiInstrumentElement = scorePartElement.GetMidiInstrument(this.InstrumentElement.Id);
                    CheckMidiInstrumentElement(midiInstrumentElement);
                    this.midiUnpitchedInstrumentNumber = midiInstrumentElement.MidiUnpitchedInstrumentNumber - 1; // https://musescore.org/en/node/89756
                    if (MidiNote.MidiChannelForUnpitchedInstruments != this.MidiChannel) // Non-virtual unpiched notes must be assigned to channel 10
                    {
                        const string logFormatString = "{0}.{1}: Creating unpitched MidiNote for unexpected {2}={3} in '{4}'";
                        Logger.LogOnce(string.Format(logFormatString, className, functionName, "MidiChannel", MidiChannel, Model.TheStaticXmlFileName));
                    }


                    if (!MidiNote.IsKnownUnpitchedMidiInstrument((UnpitchedMidiInstrumentEnum)midiUnpitchedInstrumentNumber))
                    {
                        // We do not cupport this unpitched instrument !. Replace it by a known instrument, based on the instrument name.
                        this.midiUnpitchedInstrumentNumber = (int)MidiInstrumentMap.GetUnpitchedMidiInstrumentNumber(scoreInstrumentElement);
                    }
                }
                this.scoreUnpitchedInstrumentName = scoreInstrumentElement.InstrumentName;
            }
        }


        

        /// <summary>
        /// Generate a log entry if the value for CommonDivisions is not large enough!
        /// </summary>
        private void CheckDuration()
        {
            const string functionName = "CheckDuration";
            if (0 != duration)
            {
                Int64 remainder = DurationInCommonDivisions % duration;
                if (0 != remainder)
                {
                    Int64 quotient = DurationInCommonDivisions / duration;
                    Logger.LogOnce(string.Format("{0}.{1} DurationInCommonDivisions={2} Duration={3} Quotient={4} Remainder={5} Divisions={6}",
                                  className, functionName, DurationInCommonDivisions, duration, quotient, remainder, divisions));
                }
            }
        }


        /// <summary>
        /// Gracefully handle various cases where noteDuration is not explicitly specified.
        /// </summary>
        /// <returns></returns>
        private NoteTypeEnum GetNoteDuration()
        {
            const string functionName = "GetNoteDuration";
            // const string ignoreText   = "Ignoring unknown note type because"; 
            if (noteDuration != NoteTypeEnum.unknown)
            {
                return noteDuration; // Everytning is ok
            }

            if (!this.printObjectAttributeValue) // If this NoteElement is not to be printed we don't need the NoteDurationType
            {
                // Logger.LogOnce(string.Format("{0}: {1} print-object='no'",functionName, ignoreText));
                return noteDuration;
            }
            
            if ((null == pitchElement) && (null != restElement) && (restElement.MeasureAttributeValue))
            {
                // Logger.LogOnce(string.Format("{0}: {1} this is a fullmeasure rest. Setting to 'full measure'", functionName, ignoreText));
                return NoteTypeEnum.measure; // Assume it is a full measure rest even if not specified!
            }

            // The duration element is an integer that represents a note’s duration in terms of divisions per quarter note.
            // The divisions element indicates how many divisions per quarter note are used to indicate a note's duration
            // Check if the duration of the node is exactly a full measure, taking in acount the beattype:
            if (null == currentTimeElement)
            {
                string message = string.Format("{0}.{1}: CurrentTimeElement is null",className,functionName);
                Logger.Log(message);
                throw new Exception(message);
            }

            int nominator = duration * currentTimeElement.BeatType;
            int denominator = divisions * 4;
            int quotient = nominator / denominator;
            int remainder = nominator % denominator;
            //int quotient = duration / divisions;
            //int remainder = duration % divisions;
            if ((0 == remainder) && (quotient > 1) && ( quotient == currentTimeElement.Beats) )
            {
#if false
                Logger.LogOnce(string.Format("{0}:{1}( duration={2} divisions={3} beats={4} beatType={5} ). Setting to 'full measure'",
                                              className,    // 0
                                              functionName, // 1
                                              duration,     // 2
                                              divisions,    // 3
                                              currentTimeElement.Beats,     // 4
                                              currentTimeElement.BeatType   // 5
                                              ));
#endif
                return NoteTypeEnum.measure;
            }
  
            // We can not fix the note type. Generate a line containing appropriate logging information.
            // string s = string.Format("{0}: Unknown note type in {1}", functionName, ToDebugString());
            //Logger.Log(s);
            Logger.LogOnce(string.Format("{0}:{1}( duration={2} divisions={3} beats={4} beatType={5} ). Failed to determine noteDuration",
                                          className,    // 0
                                          functionName, // 1
                                          duration,     // 2
                                          divisions,    // 3
                                          currentTimeElement.Beats,     // 4
                                          currentTimeElement.BeatType   // 5
                                          ));
            return noteDuration; // No change
        }


        /// <summary>
        /// Returns a string which is ONLY used for debugging and may be changed without warning!
        /// </summary>
        /// <returns></returns>
        private string ToDebugString()
        {
            string result = string.Format("Measure={0} Voice={1} {2} {3} {4} PartId={5} PartName={6} Divisions={7} Duration={8} PrintObject={9}",
                               measureNumber,  // 0
                               voice,          // 1
                               unpitched ? "Unpitched" : "", // 2
                               (null == pitchElement) ? "" : "Pitch=" + pitchElement.ToString(),  // 3
                               (null == restElement) ? "" : restElement.ToString(), // 4
                               scorePartElement.partId.ToString(), // 5
                               scorePartElement.PartName.ToString(), // 6
                               divisions.ToString(), // 7
                               duration.ToString(), // 8
                               this.printObjectAttributeValue.ToString()// 9
                               );
            return result;
        } 



        ///// <summary>
        ///// Private constructor, used by the Crate() method
        ///// </summary>
        ///// <param name="node"></param>
        //private NoteElement(XmlNode xmlNode,int divisions,int measureNumber, string partId,int partNumber,int midiChannel)
        //{
        //    this.measureNumber = measureNumber;
        //    this.partId = partId;
        //    this.partNumber = partNumber;
        //    this.midiChannel = midiChannel;
        //    foreach (XmlNode child in xmlNode.ChildNodes)
        //    {
        //        switch (child.Name)
        //        {
        //            case "pitch":
        //                 // The pitch represents the sound, not what is notated, so an alter element must be included even if it represents a flat or sharp
        //                 // that is part of the key signature. This is why the E-flat contains an alter element, though there is no accidental on the note.
        //                 step = GetChildValue(child, "step");
        //                 alter = GetChildValue(child, "alter");
        //                 octave = GetChildValue(child, "octave");
        //                 pitchValue = Pitch.Create(step, alter, octave);
        //                 break;                
        //            case "duration": duration = int.Parse(child.InnerText); break;
        //            case "chord": chord = true; break;
        //            case "type": type = child.InnerText; break;
        //            case "voice": voice = child.InnerText; break;
        //            case "dot": dot = true; break;
        //            case "tie": tieElement = TieElement.Create(child);
        //                  tieType = tieElement.TieType;
        //                  tieStop = ("stop" == tieType);
        //                  break;
        //            case "lyric":
        //                  text = GetChildValue(child, "text");
        //                  syllabic = GetChildValue(child, "syllabic");
        //                  break;
        //            case "staff": staffString= child.InnerText;
        //                staff = int.Parse(staffString);
        //                break;
        //            case "notations": // TO DO: Find out what to do here                                                
        //                break;
        //            case "beam": // TO DO: Find out what to do here                                                
        //                break;
        //            case "rest": // TO DO: Find out what to do here                                                
        //                break;
        //            case "accidental": // TO DO: Find out what to do here                                                
        //                break;
        //            case "time-modification": // TO DO: Find out what to do here                                                
        //                break;
        //            case "instrument": // TO DO: Find out what to do here                                                
        //                break;
        //            case "stem": // TO DO: Find out what to do here                                                
        //                break;
        //            //default:  throw new ArgumentException();
        //            default: Model.Log(string.Format("NoteElement() Unknown child.Name '{0}'", child.Name)); break;
        //        }
        //    }
        //    localizedType = LocalizeType(Type, dot);
        //    localizedPauseType = (string.IsNullOrEmpty(step)) ? LocalizePause(Type,dot) : ""; 
        //    localizedTie  = LocalizeTie(tieType); 
        //    this.divisions = divisions;
        //    // Model.GetNoteTiming(out this.startTime, out this.endTime, int.Parse(this.duration)); 
        //}

        //public static NoteElement Create(XmlNode node, int divisions, int measureNumber, string partId, int partNumber,int midiChannel) // Old version
        //{
        //    return new NoteElement(node, divisions, measureNumber, partId, partNumber, midiChannel);
        //}

        public static NoteElement Create(XmlNode node, int divisions,int tempMeasureNumber, ScorePartElement scorePartElement,TimeElement currentTimeElement) // New version
        {
            return new NoteElement(node, divisions, tempMeasureNumber, scorePartElement,currentTimeElement);
        }

        public string ToShortDebugString()
        {
            return ToShortDebugString(false, true);
        }


        /// <summary>
        /// ONLY for debugging. May bechanged at any time. Currently just returns ToString()
        /// </summary>
        /// <returns></returns>
        public string ToShortDebugString(bool showStartTime, bool showEndTime)
        {

            // Primarily for debugging. We only need to be able to identify the note in the graphics.
            string startTimeString = showStartTime ? string.Format(" Start={0} ", this.startTime) : "";
            string endTimeString   = showEndTime ? string.Format(" End={0} ", this.startTime + this.DurationInCommonDivisions) : "";
            string partString = string.Format("{0} ", PartId); 
            if (!IsPause)
            {                
                // This is a note.
                return String.Format("{0}{1}S{2} V{3} {4,3}{5} {6} {7}{8}",
                    startTimeString,partString, staff,voice,pitchElement.Name, pitchElement.Octave, localizedType, localizedTie, endTimeString);
            }
            else
            {
                // This is a pause,not a note.     
                return (String.Format("{0}{1}S{2} V{3} {4} {5}",
                    startTimeString,partString,staff,voice,  LocalizePause(noteDuration, dot), endTimeString));
            }
        }


        public override string ToString()
        {

            string measureString = "";
            if (0 != measureNumber)
            {
                measureString = string.Format("{0} {1}", ResourcesForModel.NoteElement_measure_text, measureNumber);
            }

            string notationsString = (null != notations) ? notations.ToString() : "";

            // Primarily for debugging
            string partString = string.Format("{0} ", PartId);
            string timeString = string.Format("{0}:", startTime);

            if (!IsPause)
            {
                // This is a note.
                return String.Format("{0}{1}{2} {3} {4} {5} {6} {7}",
                    timeString, partString, measureString, pitchElement.Name, pitchElement.Octave, localizedType, localizedTie, notationsString);
            }
            else
            {
                // This is a pause,not a note.     
                return (String.Format("{0}{1}{2} {3}", timeString, partString, measureString, LocalizePause(noteDuration, dot)));
            }
        }

#if false
        /// <summary>
        /// Returns a string to be used in the Details window.
        /// </summary>
        /// <returns></returns>
        public string ToDetailsString()
        {
            //string partString = string.Format("{0} ", PartId);
            string notationsString = (null != notations) ? notations.ToString() : "";
            if (!IsPause)
            {
                // This is a note.
                return String.Format("{0} {1} {2} {3} {4}",
                  pitchElement.Name, pitchElement.Octave, localizedType, localizedTie, notationsString);
            }
            else
            {
                // This is a pause,not a note.     
                return (String.Format("{0} {1}",
                   LocalizePause(noteDuration, dot), notationsString));
            }

        }
#else
        /// <summary>
        /// Returns a string to be used in the Details window. 
        /// Modelled over EventDescription.NotesForOnePart() but (by design!) does not filter by UserSettings
        /// This method is called from 2 different places:
        /// 1) When showing details directry from the NoteList
        /// 2) When showing details from the Part details 
        /// 
        /// </summary>
        /// <returns></returns>
        public string ToDetailsString()
        {
            string note = "";
            if (this.IsPause)
            {
                // This is a pause
                // Here the type and the word "pause" are cocatenated such as "punkteret halvnodepause"
                string type =  this.LocalizedPauseType;
                string notations = (null != this.Notations) ? this.Notations.ToString() : "";
                note = string.Format(" {0} {1}", type, notations);
            }
            else
            {
                // This is a note
                // Here the sequence is pitch,octave,type such af "Cis4 punkteret halvnode" 
                string accidental =  (null != this.AccidentalElement) ? this.AccidentalElement.ToString() : "";
                string pitch = this.PitchValue.Name; // Always use the name of the note
                string octave = this.Octave.ToString();
                string type =  this.LocalizedType;
                string pitchAndOctave = this.UnPitched ? this.UnpitchedText : string.Format("{0}{1}", pitch, octave); // Special handling of unpitched notes !
                string cueString = this.CueNoteString;
                string notations =  (null != this.Notations) ? this.Notations.ToString() : "";
                string printability = this.PrintObjectAttributeValue ? "" : string.Format("({0})", ResourcesForModel.EventDescription_NotPrinted);
                note = string.Format("{0} {1} {2} {3} {4} {5}", accidental, pitchAndOctave, type, cueString, notations, printability);    // Do not use extra chars for Pitch and Octave. Examples: "C","Cis4"
            }
            return note;
        }

#endif








        /// <summary>
        /// Maps the contents of the staff member variable to a localized teststring, assuming that
        ///  Staff number 1 is played by the right hand
        ///  Staff number 2 is played by the left hand
        /// Staff number 0 returns an empty string
        /// All other staff nunbers return a localized version of "staff" followed by the staff number.
        /// </summary>
        /// <param name="staff"></param>
        /// <returns></returns>
        public string LocalizedHand()
        {
            switch (this.staff)
           {
                case 0: return "";
                case 1: return ResourcesForModel.NoteElement_RightHand;
                case 2: return ResourcesForModel.NoteElement_LeftHand;
                default:
                    Logger.LogCFOnce(String.Format("Unexpected value of Staff={0}", staff));
                    return string.Format("{0} {1}",ResourcesForModel.NoteElement_Staff, staff);
            }
        }


        /// <summary>
        /// Convenience method for reporting which hand should be used for playing the note, if relevant.
        /// </summary>
        /// <returns></returns>
        public string GetLeftRightString()
        {
            try
            {
                string partName = scorePartElement.PartName;
#warning ToDo add other instruments if needed.
                bool showLeftRightHand = ((partName == "Piano") || (partName == "Organ"));
                return showLeftRightHand ? this.LocalizedHand() : "";
            }
            catch (Exception e)
            {
                Logger.LogCFE(e);
                return "";
            }
        }

    

        /// <summary>
        /// Convenience method for reporting the name of the part, but defaulting to the Id of the part if either
        /// 1) No name is specified or
        /// 2) A name is specified with "print-object" = "no" attribute
        /// </summary>
        /// <returns></returns>
        public string GetPartString()
        {
            try
            {
                string partId = scorePartElement.partId;
                string partName = scorePartElement.PartNamePrintObject ?   scorePartElement.PartName : "";
                return string.IsNullOrEmpty(partName) ? partId : partName; // Prefere PartName for PartId, i.i "Violin" for "P1"
            }
            catch (Exception e)
            {
                Logger.LogCFE(e);
                return "";
            }
        }

        // NOTE: Use the NoteElementComparer class for sorting


        //        /// <summary>
        //        /// Special implementation used for showing details
        //        /// </summary>
        //        /// <returns></returns>
        //        public string ToDetailsString(bool showHand)
        //        {
        //#warning ToDo: Replace by ToDetailsString(), which should be common for showing details directly from the NoteList and from  Part Detail
        //            string notationsString = (null != notations) ? notations.ToString() : "";
        //            if (!IsPause)
        //            {
        //                // This is a note.
        //                // By using pitchElement.Step instead of pitchElement.Name we might also report how unpitched notes are notated - if we wanted to!
        //                //return String.Format("{0} {1} {2} {3} {4}", pitchElement.Name, pitchElement.Octave, localizedType, localizedTie, notationsString);
        //                string name = pitchElement.Name;
        //                string octave = pitchElement.Octave.ToString();
        //                string hand = showHand ? LocalizedHand() : "";
        //                if (this.UnPitched)
        //                {
        //                    name = (null != UnpitchedText) ? UnpitchedText : "";    // For unpitched instruments we report the instrument here instead of the pitch!
        //                    octave = "";                 
        //                }
        //                return String.Format("{0} {1} {2} {3} {4} {5}",hand,name, octave, localizedType, localizedTie, notationsString);
        //            }
        //            else
        //            {
        //                // This is a pause,not a note.
        //                string pause = LocalizePause(noteDuration, dot);
        //                //if (null == this.pitchElement)
        //                //{
        //                //    // In the unpitched case the instrument NOT is implicitly given by the part, so we need to extract it: 
        //                //    string unpitchedInstrument = "";
        //                //    try
        //                //    {
        //                //        // throw new Exception("test");
        //                //        // New functionality. Better save than sorry !!
        //                //        unpitchedInstrument = this.ScorePartElement.ScoreInstrumentElement.InstrumentName;
        //                //    }
        //                //    catch (Exception e)
        //                //    {
        //                //        Logger.Log(string.Format("{0}.{1}: Exception. Message={2}", className,functionName,e.Message));
        //                //    }
        //                //    return string.Format("{0} {1}", unpitchedInstrument, pause);
        //                //}
        //                //else
        //                {
        //                    // In the pitched case the instrument is implicitly given by the part
        //                    string hand = showHand ? LocalizedHand() : "";
        //                    return (String.Format("{0} {1}", hand, pause));
        //                }
        //            }
        //        }

    }
}
