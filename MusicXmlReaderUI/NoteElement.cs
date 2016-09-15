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
    public enum NoteDurationType { ndtunknown, ndt1024th, ndt512th, ndt256th, ndt128th, ndt64th, ndt32nd, ndt16, ndteight, ndtquarter, ndthalf, ndtwhole, ndtbreve, ndtlong, ndtmaxima, ndtmeasure }


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
        bool graceNote;  
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
        string measureAttributeValue; 

        // MeasureNumber and MeasureNumber are not found inside the XML describing the note, but are derived from the XML surrounding the note.
        int measureNumber;
        ScorePartElement scorePartElement; // Holds a reference to the ScorePartelement describing the score part for this note

        MidiNote midiNote = null; // If !null holds a MidiNote curently being played and representing this NoteElement


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


        private NoteDurationType GetDuration(string s)
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



        private NoteElement(XmlNode xmlNode, int divisions, int measureNumber, ScorePartElement scorePartElement) // New version
        {
            this.scorePartElement = scorePartElement; 
            this.measureNumber = measureNumber;
            //this.partId = scorePartElement.partId;
            //this.partNumber = scorePartElement.partNumber;
            //this.midiChannel = (null == scorePartElement.midiInstrumentElement) ? 1 : scorePartElement.midiInstrumentElement.MidiChannel; // Use channel 1 as a default


            // Dig out attributes
            foreach (XmlAttribute a in xmlNode.Attributes)
            {
                switch (a.Name)
                {
                    case "measure":
                        measureAttributeValue = a.Value;
                        break;
                    case "default-x":
                    case "default-y":
                    case "print-object":
                    case "print-spacing":
                        break; // Explicitly ignore some graphical attributes 
                    default:
                        Logger.LogOnce(string.Format("NoteElement: Attribute.Name={0}", a.Name));
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
                            Logger.LogOnce(string.Format("NoteElement constructor: Unknown typeString '{0}' in Measure={1} Voice={2}",
                                child.InnerText, measureNumber, voice)); break;
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
                    case "beam": // TO DO: Find out what to do here 
                        unimplemented = true;
                        break;
                    case "rest":
                        //The RestElement is just a cleaner way of specifying a rest/pause instead of using a noteElement with no pitch! 
                        restElement = RestElement.Create(child);
                        if (restElement.MeasureAttributeValue == "yes")
                        {
                            noteDuration = NoteDurationType.ndtmeasure; // This Rest covers a full measure
                        }                  
                        break;
                    case "accidental": // TO DO: Find out what to do here 
                        unimplemented = true;
                        break;
                    case "time-modification": // TO DO: Find out what to do here  
                        unimplemented = true;
                        break;
                    case "instrument": // TO DO: Find out what to do here 
                        unimplemented = true;
                        break;
                    case "stem": // TO DO: Find out what to do here 
                        unimplemented = true;
                        break;
                    case "grace": // TO DO: Find out what to do here
                        // Mark this note as a grace note, i.e a note not taking part of the normal timing mechanisms.
                        // Grace notes may be implemented later, for now they are just ignored while building eventlists. 
                        graceNote = true;                                                 
                        break;
                    case "unpitched":
                    case "cue":
                    case "notehead":
                        unimplemented = true;
                        break;
                    //default:  throw new ArgumentException();
                    default: Logger.LogOnce(string.Format("NoteElement() Unknown child.Name '{0}'", child.Name)); break;
                }
                if (unimplemented)
                {
                    Logger.LogOnce(string.Format("NoteElement() child.Name '{0}' is not implemented yet", child.Name)); 
                }
            }
            if (NoteDurationType.ndtunknown ==  noteDuration)
            {
                Logger.LogOnce("Unknown note duration");
            }

            localizedType = LocalizeType(noteDuration, dot);
            localizedPauseType = (IsPause) ? LocalizePause(noteDuration, dot) : "";
            localizedTie = LocalizeTie(tieType);
            this.divisions = divisions;
            // Model.GetNoteTiming(out this.startTime, out this.endTime, int.Parse(this.duration)); 
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

        public static NoteElement Create(XmlNode node, int divisions,int tempMeasureNumber, ScorePartElement scorePartElement) // New version
        {
            return new NoteElement(node, divisions, tempMeasureNumber, scorePartElement);
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
