using System;
using System.Xml;

namespace MusicXmlReaderUI
{

    public class NoteElement : Element
    {
        string step = "";
        string alter = ""; // Represents the number of semitones the note is altered.
        string octave = "";
        string duration = "";
        string type = "";
        string voice = "";
        bool dot = false;
        string divisions = ""; 
        TieElement tieElement;
        string tieType = ""; // Is this note tied to another note
        bool tieStop = false;
        string localizedType = "";
        string localizedTie = "";
        Pitch pitch;
        int measureNumber;
        
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

        public string Duration
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

        public string Divisions
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

        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        private NoteElement()
        { }


        private string LocalizeType(string s, bool modifier)
        {
            string punctured = "punkteret ";
            switch (s)
            {
                case "whole": return (modifier ? punctured : "") + "helnode";
                case "half": return (modifier ? punctured : "") + "halvnode";
                case "quarter": return (modifier ? punctured : "") + "fjerdedel";
                case "eighth": return (modifier ? punctured : "") + "ottendedel";
            }
            return s; // 
        }

        private string LocalizePause(string s)
        {
            switch (s)
            {
                case "whole": return "helnodepause";
                case "half": return "halvnodepause";
                case "quarter": return "fjerdedelspause";
                case "eighth": return "ottendedelspause";
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
        private NoteElement(XmlNode xmlNode,int divisions,int measureNumber)
        {
            this.measureNumber = measureNumber;
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
                         pitch = Pitch.Create(step, alter, octave);
                         break;                
                    case "duration": duration = child.InnerText; break;
                    case "type": type = child.InnerText; break;
                    case "voice": voice = child.InnerText; break;
                    case "dot": dot = true; break;
                    case "tie": tieElement = TieElement.Create(child);
                          tieType = tieElement.TieType;
                          tieStop = ("stop" == tieType);
                          break; 
                }
            }
            localizedType = LocalizeType(Type, dot);
            localizedTie  = LocalizeTie(tieType); 
            this.divisions = divisions.ToString();
            // Model.GetNoteTiming(out this.startTime, out this.endTime, int.Parse(this.duration)); 
        }

        public static NoteElement Create(XmlNode node, int divisions, int measureNumber)
        {
            return new NoteElement(node, divisions, measureNumber);
        }

        public override string ToString()
        {

            string measureString = "";
            if (0 != measureNumber)
            {
                measureString = string.Format("Takt {0}", measureNumber);
            }

            if (!String.IsNullOrEmpty(Step))
            {
                // This is a note.
                return String.Format("{0} {1} {2} {3} {4}", measureString, pitch.Name, pitch.Octave, localizedType, localizedTie);
            }
            else
            {      
                // This is a pause,not a note.     
                return(String.Format("{0} {1}",measureString, LocalizePause(Type)));
            }         
        }

        public int GetDuration()
        {
            return int.Parse(duration);
        }
    }    
}
