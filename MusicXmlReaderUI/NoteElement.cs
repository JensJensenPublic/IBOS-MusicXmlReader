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
        string alter = ""; // Represents the number of semitones the note is altered.
        string octave = "";
        int duration = 0;
        bool chord = false; // Means that this note starts at the same time as the previous note, not after the previous note.
        string type = "";
        string voice = "";
        bool dot = false;
        int divisions = 0; 
        TieElement tieElement;
        string tieType = ""; // Is this note tied to another note
        bool tieStop = false;
        string localizedType = "";      //  If this is n note,  not a pause
        string localizedPauseType = ""; //  If this is a pause, not a note
        string localizedTie = "";
        Pitch pitchValue;
        int measureNumber;
        string partId;
        int partNumber;
        //int startTime;
        string syllabic; // Child of lyric
        string text;     // Child of lyric
        string staffString = "";
        int staff = 0;
        string articulations = "";

        MidiNote midiNote = null; // If !null holds a MidiNote curently being played and representing this NoteElement


        public string Step
        {
            get
            {
                return step;
            }
        }

        public string Octave
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
        {            get
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

        public string Alter
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
                return partId;
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
                return partNumber;
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


        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private NoteElement()
        { }


        private string LocalizeType(string s, bool modifier)
        {

            switch (s)
            {
                case "whole":   return (modifier ? punctured : "") + "helnode";
                case "half":    return (modifier ? punctured : "") + "halvnode";
                case "quarter": return (modifier ? punctured : "") + "fjerdedel";
                case "eighth":  return (modifier ? punctured : "") + "ottendedel";
                case "16th":    return (modifier ? punctured : "") + "sekstendedel";
                case "32nd":    return (modifier ? punctured : "") + "toogtredivtedel";
                case "64nd":    return (modifier ? punctured : "") + "fireogtredsindstyvendedel";
            }
            return (modifier ? punctured : "") + s; // 
        }

        private string LocalizePause(string s, bool modifier)
        {
            switch (s)
            {
                case "whole":   return (modifier ? punctured : "") + "helnodepause";
                case "half":    return (modifier ? punctured : "") + "halvnodepause";
                case "quarter": return (modifier ? punctured : "") + "fjerdedelspause";
                case "eighth":  return (modifier ? punctured : "") + "ottendedelspause";
                case "16th":    return (modifier ? punctured : "") + "sekstendedelspause";
                case "32nd":    return (modifier ? punctured : "") + "toogtredivtedelspause";
                case "64nd":    return (modifier ? punctured : "") + "fireogtredsindstyvendedelspause";
            }
            return "pause " + LocalizeType(s, false);
        }

        private string LocalizeTie(string tieType)
        {
            switch (tieType)
            {
                case "start" : return "bue start";
                case "stop"  : return "bue slut";
                default:       return "";     
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
        

        /// <summary>
        /// Private constructor, used by the Crate() method
        /// </summary>
        /// <param name="node"></param>
        private NoteElement(XmlNode xmlNode,int divisions,int measureNumber, string partId,int partNumber)
        {
            this.measureNumber = measureNumber;
            this.partId = partId;
            this.partNumber = partNumber;
            foreach (XmlNode child in xmlNode.ChildNodes)
            {
                switch (child.Name)
                {
                    case "pitch":
                         // The pitch represents the sound, not what is notated, so an alter element must be included even if it represents a flat or sharp
                         // that is part of the key signature. This is why the E-flat contains an alter element, though there is no accidental on the note.
                         step = GetChildValue(child, "step");
                         alter = GetChildValue(child, "alter");
                         octave = GetChildValue(child, "octave");
                         pitchValue = Pitch.Create(step, alter, octave);
                         break;                
                    case "duration": duration = int.Parse(child.InnerText); break;
                    case "chord": chord = true; break;
                    case "type": type = child.InnerText; break;
                    case "voice": voice = child.InnerText; break;
                    case "dot": dot = true; break;
                    case "tie": tieElement = TieElement.Create(child);
                          tieType = tieElement.TieType;
                          tieStop = ("stop" == tieType);
                          break;
                    case "lyric":
                          text = GetChildValue(child, "text");
                          syllabic = GetChildValue(child, "syllabic");
                          break;
                    case "staff": staffString= child.InnerText;
                        staff = int.Parse(staffString);
                        break;
                    case "notations": // TO DO: Find out what to do here                                                
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
                    default:  throw new ArgumentException();
                }
            }
            localizedType = LocalizeType(Type, dot);
            localizedPauseType = (string.IsNullOrEmpty(step)) ? LocalizePause(Type,dot) : ""; 
            localizedTie  = LocalizeTie(tieType); 
            this.divisions = divisions;
            // Model.GetNoteTiming(out this.startTime, out this.endTime, int.Parse(this.duration)); 
        }

        public static NoteElement Create(XmlNode node, int divisions, int measureNumber, string partId, int partNumber)
        {
            return new NoteElement(node, divisions, measureNumber, partId, partNumber);
        }

        public override string ToString()
        {

            string measureString = "";
            if (0 != measureNumber)
            {
                measureString = string.Format("Takt {0}", measureNumber);
            }

            // Primarily for debugging
            string partString = string.Format("{0} ", partId);
            string timeString = string.Format("{0}:", startTime);

            if (!String.IsNullOrEmpty(Step))
            {
                // This is a note.
                return String.Format("{0}{1}{2} {3} {4} {5} {6}", timeString, partString, measureString, pitchValue.Name, pitchValue.Octave, localizedType, localizedTie);
            }
            else
            {      
                // This is a pause,not a note.     
                return(String.Format("{0}{1}{2} {3}", timeString, partString, measureString, LocalizePause(Type,dot)));
            }         
        }



    }    
}
