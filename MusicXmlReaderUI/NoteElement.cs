using System;
using System.Xml;
using JSJ.MusicSynthesis;

namespace MusicXmlReaderUI
{


    public class NoteElement : EventElement
    {
        // Allows for representing the following subdivisions of a quarter node:
        // 2,3,4,5,6,7,8,9,10. 
        // 1260 can be divided by 2,3,4,5,6,7,8,9 and 10 !
        public const int commonDivisions = 1260;
        string punctured = "punkteret ";

        // Values found in MusicXml file, possibly after a minor type conversion, typically fromstring to int.
        string step = "";
        int alter = 0 ; // Represents the number of semitones the note is altered.
        int octave = 0;
        int duration = 0;
        bool chord = false; // Means that this note starts at the same time as the previous note, not after the previous note.
        string type = "";
        string voice = "";
        bool dot = false;
        int divisions = 0; 
        TieElement tieElement;             // A NoteElement may contain a nested TieElement  (Danish: "Bindebue")
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

        // MeasureNumber and MeasureNumber are not found inside the XML describing the note, but are derived from the XML surrounding the note.
        int measureNumber;
        ScorePartElement scorePartElement; // Holds a reference to the ScorePartelement describing the score part for this note

        MidiNote midiNote = null; // If !null holds a MidiNote curently being played and representing this NoteElement


        public string Step
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

        public string Type
        {
            get
            {
                return type;
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

        public bool IsPause
        {
            get
            {
                return string.IsNullOrEmpty(step);
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
        private string LocalizeType(string typeString, bool modifier)
        {
            string modif = (modifier ? punctured : "");
            string value = "";
            switch (typeString)
            {
                case "whole":   value = "helnode"; break;
                case "half":    value = "halvnode"; ; break;
                case "quarter": value = "fjerdedel"; break;
                case "eighth":  value = "ottendedel"; break;
                case "16th":    value = "sekstendedel"; break;
                case "32nd":    value = "toogtredivtedel"; break;
                case "64nd":    value = "fireogtredsindstyvendedel"; break;
                default:
                    Model.Log(string.Format("LocalizeType({0},{1}) Unknown typeString '{2}'", typeString, modifier, typeString)); break;
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
        private string LocalizePause(string typeString, bool modifier)
        {
            string modif = (modifier ? punctured : "");
            string value = "";
            switch (typeString)
            {
                case "whole":   value = "helnodepause"; break;
                case "half":    value = "halvnodepause"; ; break;
                case "quarter": value = "fjerdedelspause"; break;
                case "eighth":  value = "ottendedelspause"; break;
                case "16th":    value = "sekstendedelspause"; break;
                case "32nd":    value = "toogtredivtedelspause"; break;
                case "64nd":    value = "fireogtredsindstyvendedelspause"; break;
                default:
                    Model.Log(string.Format("LocalizePause({0},{1}) Unknown typeString '{2}'", typeString, modifier, typeString)); break;
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
                default: Model.Log(string.Format("LocalizeTie({0}) Unknown tieType '{1}'", tieType, tieType)); return "";     
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

        private NoteElement(XmlNode xmlNode, int divisions, int measureNumber, ScorePartElement scorePartElement) // New version
        {
            this.scorePartElement = scorePartElement; 
            this.measureNumber = measureNumber;            
            //this.partId = scorePartElement.partId;
            //this.partNumber = scorePartElement.partNumber;
            //this.midiChannel = (null == scorePartElement.midiInstrumentElement) ? 1 : scorePartElement.midiInstrumentElement.MidiChannel; // Use channel 1 as a default
            foreach (XmlNode child in xmlNode.ChildNodes)
            {
                switch (child.Name)
                {
                    case "pitch":
                        // The pitch represents the sound, not what is notated, so an alter element must be included even if it represents a flat or sharp
                        // that is part of the key signature. This is why the E-flat contains an alter element, though there is no accidental on the note.
                        step = GetChildValue(child, "step");
                        //alter = GetChildValue(child, "alter");
                        Utilities.Parse(GetChildValue(child, "alter"), ref alter, -2, +2, "NoteElement: alter");
                        //string octave = GetChildValue(child, "octave");
                        Utilities.Parse(GetChildValue(child, "octave"), ref this.octave, 0, 9, "NoteElement: octave");
                        pitchValue = Pitch.Create(step, alter, octave);
                        break;
                    case "duration": duration = int.Parse(child.InnerText); break;
                    case "chord": chord = true; break;
                    case "type": type = child.InnerText; break;
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
                        break;
                    case "rest": // TO DO: Find out what to do here                                                
                        break;
                    case "accidental": // TO DO: Find out what to do here                                                
                        break;
                    case "time-modification": // TO DO: Find out what to do here                                                
                        break;
                    case "instrument": // TO DO: Find out what to do here                                                
                        break;
                    case "stem": // TO DO: Find out what to do here                                                
                        break;
                    case "grace": // TO DO: Find out what to do here
                        // Mark this note as a grace note, i.e a note not taking part of the normal timing mechanisms.
                        // Grace notes may be implemented later, for now they are just ignored while building eventlists. 
                        graceNote = true;                                                 
                        break;
                    //default:  throw new ArgumentException();
                    default: Model.Log(string.Format("NoteElement() Unknown child.Name '{0}'", child.Name)); break;
                }
            }
            localizedType = LocalizeType(Type, dot);
            localizedPauseType = (string.IsNullOrEmpty(step)) ? LocalizePause(Type, dot) : "";
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

            if (!String.IsNullOrEmpty(Step))
            {
                // This is a note.
                return String.Format("{0}{1}{2} {3} {4} {5} {6} {7}",
                    timeString, partString, measureString, pitchValue.Name, pitchValue.Octave, localizedType, localizedTie,notationsString);
            }
            else
            {      
                // This is a pause,not a note.     
                return(String.Format("{0}{1}{2} {3}", timeString, partString, measureString, LocalizePause(Type,dot)));
            }         
        }



    }    
}
