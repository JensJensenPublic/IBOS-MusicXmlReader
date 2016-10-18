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
        // Allows for representing the following subdivisions of a quarter node:
        // 2,3,4,5,6,7,8,9,10. 
        // 1260 can be divided by 2,3,4,5,6,7,8,9 and 10 !
        public const int commonDivisions = 1260;
        string punctured = ResourcesForModel.NoteElement_dotted + " "; // "punkteret ";


        // Values found in MusicXml file, possibly after a minor type conversion, typically from string to int.
        //FullStepEnum step = FullStepEnum.Unknown ;   // Represents a diatonoc step: A,B,C,D,E,F, G or a pause
        //int alter = 0 ;     // Represents the number of semitones the note is altered. 
        //int octave = 0;
        int duration = 0;
        bool chord = false; // Means that this note starts at the same time as the previous note, not after the previous note.
        //string type = "unspecified";
        NoteTypeEnum noteDuration = NoteTypeEnum.unknown;
        string voice = "";
        bool dot = false;
        int divisions = 0; 
        TieElement tieElement;             // A NoteElement may contain a nested TieElement  (Danish: "Bindebue")
        RestElement restElement;   // A NoteElement may contain a nested RestElement
        NotationsElement notations; // A NoteElement may contain a nested NotationsElement     
        string tieType = ""; // Is this note tied to another note
        bool tieStop = false;
        string localizedType = "";      //  If this is n note,  not a pause
        string localizedPauseType = ""; //  If this is a pause, not a note
        string localizedTie = "";
        PitchElement pitchElement;
        string syllabic; // Child of lyric
        string text;     // Child of lyric
        string staffString = "";
        int staff = 0;
        //string articulations = "";
        InstrumentElement instrumentElement; // The instrument type distinguishes between score-instrument elements in a score-part. The id attribute is an IDREF back to the score-instrument ID.
                                             //If multiple score-instruments are specified on a score-part, there should be an instrument element for each note in the part.
        AccidentalElement accidentalElement;
        NoteHeadElement noteHeadElement; // Special graphical variants of hoathead
        TimeModificationElement timeModificationElement; // Tuplet information 

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


        public PitchElement.FullStepEnum Step
        {
            get
            {
                return pitchElement.Step;
            }
        }

        public int Octave
        {
            get
            {
                return pitchElement.Octave;
            }
        }

 
        public int Duration
        {
            get
            {
                return duration;
            }
  
        }

        public string Voice
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

        public int DurationInCommonDivisions
        {
            get
            {
                return duration * commonDivisions / divisions;
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

        /// <summary>
        /// Per definition a note without a pitch is a pause !
        /// </summary>
        public bool IsPause
        {
            get
            {
                return (null == pitchElement);
            }
        }

        internal PitchElement PitchValue
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
                case NoteTypeEnum.whole:     value = ResourcesForModel.NoteElement_whole; break;    // "helnode"; break;
                case NoteTypeEnum.half:      value = ResourcesForModel.NoteElement_half; break;     //"halvnode"; ; break;
                case NoteTypeEnum.quarter:   value = ResourcesForModel.NoteElement_quarter; break;  // "fjerdedel"; break;
                case NoteTypeEnum.eight:     value = ResourcesForModel.NoteElement_eight; break;    // "ottendedel"; break;
                case NoteTypeEnum.nt16th:    value = ResourcesForModel.NoteElement_16th;break;      // "sekstendedel"; break;
                case NoteTypeEnum.nt32nd:    value = ResourcesForModel.NoteElement_32nd; break;     //"toogtredivtedel"; break;
                case NoteTypeEnum.nt64th:    value = ResourcesForModel.NoteElement_64th; break;     //"fireogtredsindstyvendedel"; break;
                case NoteTypeEnum.measure:   value = ResourcesForModel.NoteElement_measure;break;   //  "heltakt"; break;
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
                case "whole": return NoteTypeEnum.whole;
                case "half": return NoteTypeEnum.half;
                case "quarter": return NoteTypeEnum.quarter;
                case "eighth":return NoteTypeEnum.eight;
                case "16th": return NoteTypeEnum.nt16th;
                case "32nd": return NoteTypeEnum.nt32nd;
                case "64nd": return NoteTypeEnum.nt64th;
                case "measure": return NoteTypeEnum.measure;
                case "unspecified": return NoteTypeEnum.unknown;
                default:  Logger.LogOnce(string.Format("{0}.{1}: Unknown Fullstep value={2}", className, "GetDuration", s));
                    return NoteTypeEnum.unknown;
            }
        }



        // http://usermanuals.musicxml.com/MusicXML/Content/EL-MusicXML-note.htm


        private NoteElement(XmlNode xmlNode, int divisions, int measureNumber, ScorePartElement scorePartElement, TimeElement currentTimeElement) // New version
        {
            this.divisions = divisions;
            this.scorePartElement = scorePartElement; 
            this.measureNumber = measureNumber;
            this.currentTimeElement = currentTimeElement;  
            const string functionName = "NoteElement constructor"; // For logging
            //this.partId = scorePartElement.partId;
            //this.partNumber = scorePartElement.partNumber;
            //this.midiChannel = (null == scorePartElement.midiInstrumentElement) ? 1 : scorePartElement.midiInstrumentElement.MidiChannel; // Use channel 1 as a default


            // Dig out attributes
            foreach (XmlAttribute a in xmlNode.Attributes)
            {
                switch (a.Name)
                {
                    case "measure":         Utilities.ParseYesNoAttributeValue(functionName, a.Name, a.Value, ref measureAttributeValue); break;
                    case "print-object":    Utilities.ParseYesNoAttributeValue(functionName, a.Name, a.Value, ref printObjectAttributeValue); break;            
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
                    default:
                        Logger.LogOnce(string.Format("{0} Attribute.Name={0}", functionName, a.Name));
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
                        break;
                    case "duration": duration = int.Parse(child.InnerText); break;
                    case "chord": chord = true; break;
                    case "type":
                        noteDuration = GetDuration(child.InnerText);
                        if (NoteTypeEnum.unknown == noteDuration)
                        {
                            Logger.LogOnce(string.Format("{0}: Unknown typeString '{1}' in Measure={2} Voice={3}",
                                functionName,child.InnerText, measureNumber, voice)); break;
                        }
                        break;
                        // type = child.InnerText;
                    case "voice": voice = child.InnerText; break;
                    case "dot": dot = true; break;
                    case "tie":
                        tieElement = TieElement.Create(child);
                        tieType = tieElement.TieType;
                        tieStop = ("stop" == tieType);
                        break;
                    case "lyric":
                        text = Utilities.GetChildValue(child, "text");
                        syllabic = Utilities.GetChildValue(child, "syllabic");
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
                        timeModificationElement = TimeModificationElement.Create(child,this);
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
                        unpitched = true; break; // Just mark the note as unpitched
                    case "cue": // http://usermanuals.musicxml.com/MusicXML/Content/EL-MusicXML-cue.htm
                        isCueNote = true; break; ; // Just mark the note as a cue note
                    case "notehead": // http://usermanuals.musicxml.com/MusicXML/Content/CT-MusicXML-notehead.htm
                        noteHeadElement = NoteHeadElement.Create(child);
                        break;
                    //default:  throw new ArgumentException();
                    default: Logger.LogOnce(string.Format("{0}: Unknown child.Name '{1}'",functionName, child.Name)); break;
                }
                if (unimplemented)
                {
                    Logger.LogOnce(string.Format("{0}: child.Name '{1}' is not implemented yet", functionName, child.Name)); 
                }
            }

            noteDuration = GetNoteDuration();          

            localizedType = LocalizeType(noteDuration, dot);
            localizedPauseType = (IsPause) ? LocalizePause(noteDuration, dot) : "";
            localizedTie = LocalizeTie(tieType);
          
            // Model.GetNoteTiming(out this.startTime, out this.endTime, int.Parse(this.duration)); 
        }


        /// <summary>
        /// Gracefully handle various cases where noteDuration is not explicitly specified.
        /// </summary>
        /// <returns></returns>
        private NoteTypeEnum GetNoteDuration()
        {
            const string functionName = "NoteElement.GetNoteDuration";
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

            int quotient = duration / divisions;
            int remainder = duration % divisions;
            if ((0 == remainder) && (quotient > 1) && ( quotient == ( 4 * currentTimeElement.Beats) / currentTimeElement.BeatType))
            {
                //Logger.LogOnce(string.Format("{0}: {1} because ( duration={2} divisions={3} beats={4} beatType={5} ). Setting to 'full measure'",
                //                              functionName, // 0
                //                              ignoreText,   // 1
                //                              duration,     // 2
                //                              divisions,    // 3
                //                              currentTimeElement.Beats,     // 4
                //                              currentTimeElement.BeatType   // 5
                //                              ));
                return NoteTypeEnum.measure;
            }
  
            // We can not fix the note type. Generate a line containing appropriate logging information.
            string s = string.Format("{0}: Unknown note type in {1}", functionName, ToDebugString());
            Logger.Log(s);

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
                               scorePartElement.partName.ToString(), // 6
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


        public override string ToString()
        {

            string measureString = "";
            if (0 != measureNumber)
            {
                measureString = string.Format("{0} {1}",ResourcesForModel.NoteElement_measure_text, measureNumber);
            }

            string notationsString = (null != notations) ? notations.ToString() : ""; 

            // Primarily for debugging
            string partString = string.Format("{0} ", PartId);
            string timeString = string.Format("{0}:", startTime);

            if (!IsPause)
            {
                // This is a note.
                return String.Format("{0}{1}{2} {3} {4} {5} {6} {7}",
                    timeString, partString, measureString, pitchElement.Name, pitchElement.Octave, localizedType, localizedTie,notationsString);
            }
            else
            {      
                // This is a pause,not a note.     
                return(String.Format("{0}{1}{2} {3}", timeString, partString, measureString, LocalizePause(noteDuration,dot)));
            }         
        }



    }    
}
