using System;
using System.Xml;
using JSJ.MusicSynthesis;

namespace MusicXmlReaderUI
{

    /// <summary>
    /// As specified by MusicXML, but modified according to use C# sytax
    /// Also used to describe nurations of rests
    /// The ndtmeasure is not a part of the MusicXml definition but is used to describe a note with the "pullmeasure"= "yes" attribute
    /// </summary>
    public enum NoteDurationType
    {
        ndtunknown, // We know absolute nothing about the value
        ndt1024th,
        ndt512th,
        ndt256th,
        ndt128th,
        ndt64th,
        ndt32nd,
        ndt16,
        ndteight,
        ndtquarter,
        ndthalf,
        ndtwhole,
        ndtbreve,
        ndtlong,
        ndtmaxima,
        ndtmeasure,
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
        string punctured = "punkteret ";


        // Values found in MusicXml file, possibly after a minor type conversion, typically from string to int.
        char step = ' ';   // Represents a diatonoc step: A,B,C,D,E,F, G or a pause
                            // We need a string here because the empty string is used to denote a pause !
        int alter = 0 ;     // Represents the number of semitones the note is altered. 
        int octave = 0;
        int duration = 0;
        bool chord = false; // Means that this note starts at the same time as the previous note, not after the previous note.
        //string type = "unspecified";
        NoteDurationType noteDuration = NoteDurationType.ndtunknown;
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
        Pitch pitchValue;
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


        public char Step
        {
            get
            {
                return step;
            }
        }

        public int Octave
        {
            get
            {
                return octave;
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
                return alter;
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
                return (null == pitchValue);
            }
        }

        internal Pitch PitchValue
        {
            get
            {
                return pitchValue;
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

        public NoteDurationType NoteDuration
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
        private string LocalizeType(NoteDurationType noteDuration, bool modifier)
        {
            string modif = (modifier ? punctured : "");
            string value = "";
            switch (noteDuration)
            {
                case NoteDurationType.ndtwhole:     value = "helnode"; break;
                case NoteDurationType.ndthalf:      value = "halvnode"; ; break;
                case NoteDurationType.ndtquarter:   value = "fjerdedel"; break;
                case NoteDurationType.ndteight:     value = "ottendedel"; break;
                case NoteDurationType.ndt16:        value = "sekstendedel"; break;
                case NoteDurationType.ndt32nd:      value = "toogtredivtedel"; break;
                case NoteDurationType.ndt64th:      value = "fireogtredsindstyvendedel"; break;
                case NoteDurationType.ndtmeasure:   value = "heltakt"; break;
                case NoteDurationType.ndtunknown:   value = "ukendt"; break;
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
        private string LocalizePause(NoteDurationType noteDuration, bool modifier)
        {
            string modif = (modifier ? punctured : "");
            string value = "";
            switch (noteDuration)
            {
                case NoteDurationType.ndtwhole: value = "helnodepause"; break;
                case NoteDurationType.ndthalf: value = "halvnodepause"; ; break;
                case NoteDurationType.ndtquarter: value = "fjerdedelspause"; break;
                case NoteDurationType.ndteight: value = "ottendedelspause"; break;
                case NoteDurationType.ndt16: value = "sekstendedelspause"; break;
                case NoteDurationType.ndt32nd: value = "toogtredivtedel"; break;
                case NoteDurationType.ndt64th: value = "fireogtredsindstyvendedelspause"; break;
                case NoteDurationType.ndtmeasure: value = "heltaktspause"; break;
                case NoteDurationType.ndtunknown: value = "ukendt"; break;
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
                case "start": return "bue start";
                case "stop" : return "bue slut";
                case ""     :  return "";
                default: Logger.Log(string.Format("LocalizeTie({0}) Unknown tieType '{1}'", tieType, tieType)); return "";     
            }
        }


        private string GetChildValue(XmlNode note, string name)
        {
            foreach (XmlNode n in note.ChildNodes)
            {
                if (name == n.Name) return n.InnerText;
            }
            return "";
        }


        public NoteDurationType GetDuration(string s)
        {
            switch (s)
            {
                case "whole": return NoteDurationType.ndtwhole;
                case "half": return NoteDurationType.ndthalf;
                case "quarter": return NoteDurationType.ndtquarter;
                case "eighth":return NoteDurationType.ndteight;
                case "16th": return NoteDurationType.ndt16;
                case "32nd": return NoteDurationType.ndt32nd;
                case "64nd": return NoteDurationType.ndt64th;
                case "measure": return NoteDurationType.ndtmeasure;
                case "unspecified": return NoteDurationType.ndtunknown;
                default:  return NoteDurationType.ndtunknown;
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
                        //step = GetChildValue(child, "step");
                        string stepString = GetChildValue(child, "step");
                        if (!string.IsNullOrEmpty(stepString))
                        {
                            Utilities.Parse(GetChildValue(child, "step"), ref step, 'A', 'G', "NoteElement: step");
                            Utilities.Parse(GetChildValue(child, "alter"), ref alter, -2, +2, "NoteElement: alter", true);
                            Utilities.Parse(GetChildValue(child, "octave"), ref octave, 0, 9, "NoteElement: octave", false);
                            pitchValue = Pitch.Create(step, alter, octave);
                        }
                        break;
                    case "duration": duration = int.Parse(child.InnerText); break;
                    case "chord": chord = true; break;
                    case "type":
                        noteDuration = GetDuration(child.InnerText);
                        if (NoteDurationType.ndtunknown == noteDuration)
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
                        text = GetChildValue(child, "text");
                        syllabic = GetChildValue(child, "syllabic");
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
                            noteDuration = NoteDurationType.ndtmeasure; // This Rest covers a full measure
                        }                  
                        break;
                    case "accidental": // http://usermanuals.musicxml.com/MusicXML/Content/EL-MusicXML-accidental.htm
                        accidentalElement = AccidentalElement.Create(child);
                        break;
                    case "time-modification":   http://usermanuals.musicxml.com/MusicXML/Content/EL-MusicXML-time-modification.htm  
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
        private NoteDurationType GetNoteDuration()
        {
            const string functionName = "NoteElement.GetNoteDuration";
            // const string ignoreText   = "Ignoring unknown note type because"; 
            if (noteDuration != NoteDurationType.ndtunknown)
            {
                return noteDuration; // Everytning is ok
            }

            if (!this.printObjectAttributeValue) // If this NoteElement is not to be printed we don't need the NoteDurationType
            {
                // Logger.LogOnce(string.Format("{0}: {1} print-object='no'",functionName, ignoreText));
                return noteDuration;
            }
            
            if ((null == pitchValue) && (null != restElement) && (restElement.MeasureAttributeValue))
            {
                // Logger.LogOnce(string.Format("{0}: {1} this is a fullmeasure rest. Setting to 'full measure'", functionName, ignoreText));
                return NoteDurationType.ndtmeasure; // Assume it is a full measure rest even if not specified!
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
                return NoteDurationType.ndtmeasure;
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
                               (null == pitchValue) ? "" : "Pitch=" + pitchValue.ToString(),  // 3
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
                measureString = string.Format("Takt {0}", measureNumber);
            }

            string notationsString = (null != notations) ? notations.ToString() : ""; 

            // Primarily for debugging
            string partString = string.Format("{0} ", PartId);
            string timeString = string.Format("{0}:", startTime);

            if (!IsPause)
            {
                // This is a note.
                return String.Format("{0}{1}{2} {3} {4} {5} {6} {7}",
                    timeString, partString, measureString, pitchValue.Name, pitchValue.Octave, localizedType, localizedTie,notationsString);
            }
            else
            {      
                // This is a pause,not a note.     
                return(String.Format("{0}{1}{2} {3}", timeString, partString, measureString, LocalizePause(noteDuration,dot)));
            }         
        }



    }    
}
