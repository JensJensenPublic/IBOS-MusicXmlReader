using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicXmlReaderModel;

namespace MusicXmlReaderModel
{



    /// <summary>
    /// 
    /// NOTE!!! This is a primitive initial implementation, only lookin k for a transition from StateEnum.Text to StateEnum.Music  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    /// 
    /// 
    /// Used during test for decoding Braille Music files into readable symbols
    /// Intensionally does NOT use exicting definitions of symbols in order to avoid duplication of existing errors.
    /// </summary>
    public class BrailleMusicDecoder
    {
        public enum StateEnum { Unknown, Text, Digit, Music };
        public enum BrailleMusicSubState { Unchanged, Music, Number }; // More to be added

        const byte noDots = 0;
        const byte dot1 = 0x01;
        const byte dot2 = 0x02;
        const byte dot3 = 0x04;
        const byte dot4 = 0x08;
        const byte dot5 = 0x10;
        const byte dot6 = 0x20;
        const byte dot7 = 0x40;
        const byte dot8 = 0x80;
        const byte none = 0x00;
        const int dot1245 = dot1 | dot2 | dot4 | dot5; // For isolating values representing note steps
        const int dot36 = dot3 | dot6; // For isolating type

        int count;
        StateEnum state = StateEnum.Unknown;
        BrailleMusicSubState brailleMusicSubState = BrailleMusicSubState.Music;
        BrailleMusicSubState nextBrailleMusicSubState = BrailleMusicSubState.Music;


        private void Count(string s)
        {
            if (string.IsNullOrEmpty(s)) return;
            count++;
        }

        public void ResetState()
        {
            state = StateEnum.Unknown;
        }

        bool gotDot6 = false;

        public string ToString(int i)
        {
            if ((i < 0) || (i > 63)) throw new Exception("Invalid argument");         
              

            switch (state)
            {
                case StateEnum.Music: return MusicBrailleToString(i);
                case StateEnum.Text: return TextToString(i);
                case StateEnum.Digit: return DigitToString(i);
                case StateEnum.Unknown: return "UNKNOWN";
                default: throw new Exception(string.Format("Unsupported state {0} ", state.ToString()));
            }
        }

  

        private string TextToString(int i)
        {
            // Look for Dot6 followed by Dot3 which signals a transition to StateEnum.Music
            if (gotDot6 && (i == dot3))
            {
                state = StateEnum.Music;
                return string.Format("State changed to {0}",state.ToString());
            }
            gotDot6 = (i == dot6);


            switch (i)
            {
                // Primitive mapping. Add more as needed !
                case 00: return " ";
                case 01: return "A";
                case 03: return "B";
                case 09: return "C";
                case 25: return "D";
                case 17: return "E";
                case 11: return "F";
                case 27: return "G";
                case 19: return "H";
                case 10: return "I";
                case 26: return "J";
                case 05: return "K";
                case 07: return "L";
                case 13: return "M";
                case 29: return "N";
                case 21: return "O";
                case 15: return "P";
                case 31: return "Q";
                case 23: return "R";
                case 14: return "S";
                case 30: return "T";
                case 37: return "U";
                case 39: return "V";
                case 58: return "W";
                case 45: return "X";
                case 61: return "Y";
                case 53: return "Z";
                case 28: return "Æ";
                case 42: return "Ø";
                case 33: return "Å";
                case 32: return "VERSAL";
                case 60: return "CIFFER";
                case 50: return ".";
                case 02: return ",";
                case 38: return "?";
                case 06: return ";";
                case 22: return "!";
                case 54: return "/";
                case 36: return "-";
                default: return "UKENDT";
            }
 
        }

        private string DigitToString(int i)
        {
            return "DIGIT";
        }


        private string  MusicBrailleToString(int i)
        {
            nextBrailleMusicSubState = BrailleMusicSubState.Music;

            // Internal variables
            count = 0; // Number of interpretations found. Interesting (if <> 1) !! 
            string stepName = "";
            string typeName = "";

            // Strings for collecting all decoded values
            string stepAndType = "";
            string octave = "";
            string rest = "";
            string accidental = "";
            string finger = "";
            string interval = "";
            string otherValues = "";
            string digit = "";
            string denominator = "";

            string warning = "";


            // First find all step values
            int stepvalue = i & dot1245;

            switch (stepvalue)
            {
                case dot1 | dot4 | dot5: stepName = "C"; break;
                case dot1 | dot5: stepName = "D"; break;
                case dot1 | dot2 | dot4: stepName = "E"; break;
                case dot1 | dot2 | dot4 | dot5: stepName = "F"; break;
                case dot1 | dot2 | dot5: stepName = "G"; break;
                case dot2 | dot4: stepName = "A"; break;
                case dot2 | dot4 | dot5: stepName = "H"; break;
                default: break;
            }
            if (!string.IsNullOrEmpty(stepName))
            {
                // This is a pitched note, find the type
                int typevalue = i & dot36;
                switch (typevalue)
                {
                    case dot3 | dot6: typeName = "1/1"; break;
                    case dot3: typeName = "1/2"; break;
                    case dot6: typeName = "1/4"; break;
                    case none: typeName = "1/8"; break;
                }
                stepAndType = stepName + typeName;
                Count(stepAndType);
            }

            switch (i) // Look for octave marks
            {
                case dot4: octave = "1"; break;
                case dot4 | dot5: octave = "2"; break;
                case dot4 | dot5 | dot6: octave = "3"; break;
                case dot5: octave = "4"; break;
                case dot4 | dot6: octave = "5"; break;
                case dot5 | dot6: octave = "6"; break;
                case dot6: octave = "7"; break;
                default: break;
            }
            Count(octave);

            // This was not an octave sign. Continue:

            switch (i) // Look for rests
            {
                case dot1 | dot3 | dot4: rest = "R1/1"; break;
                case dot1 | dot3 | dot6: rest = "R1/2"; break;
                case dot1 | dot2 | dot3 | dot6: rest = "R1/4"; break;
                case dot1 | dot3 | dot4 | dot6: rest = "R1/8"; break;
                default: break;
            }
            Count(rest);

            switch (i) // Look for accidentals
            {
                case dot1 | dot4 | dot6: accidental = "Sharp"; break;
                case dot1 | dot2 | dot6: accidental = "Flat"; break;
                case dot1 | dot6: accidental = "Natural"; break;
                default: break;
            }
            Count(accidental);

            switch (i) // Look for finger
            {
                case dot1: finger = "1"; break;
                case dot2: finger = "4"; break;
                case dot1 | dot2: finger = "2"; break;
                case dot1 | dot3: finger = "5"; break;
                case dot1 | dot2 | dot3: finger = "3"; break;
            }
            Count(finger);

            switch (i) // Look for interval
            {
                case dot3 | dot4: interval = "Second"; break;
                case dot3 | dot4 | dot6: interval = "Third"; break;
                case dot3 | dot4 | dot5 | dot6: interval = "Fourth"; break; // Note: already used for "number"?
                case dot3 | dot5: interval = "Fifth"; break;
                case dot3 | dot5 | dot6: interval = "Sixth"; break;
                case dot2 | dot5: interval = "Seventh"; break;
                case dot3 | dot6: interval = "Octave"; break;
            }
            Count(interval);


            switch (i) // Look for remaining codes
            {
                // Maybe we should use repeated ifs instead of switch here ??
                case none: otherValues = "NewMeasure"; break;
                case dot3: otherValues = "Dotted"; break;
//                case dot5: otherValues = "Reference"; break; // For the time being we omit this because it clashes with Octave4 !
                case dot2 | dot3: otherValues = "Triplet"; break;
                case dot3 | dot4 | dot5: otherValues = "Word"; break;
                case dot3 | dot4 | dot5 | dot6: otherValues = "Number"; nextBrailleMusicSubState = BrailleMusicSubState.Number; break;
                case dot1 | dot4: otherValues = "Legato"; break;
                case dot2 | dot3 | dot5 | dot6: otherValues = "Equality"; break;
                case dot2 | dot5: otherValues = "Newline"; break;
                case dot2 | dot3 | dot5: otherValues = "Trill"; break;
                case dot2 | dot6: otherValues = "Ornament"; break;
                case dot2 | dot3 | dot6: otherValues = "Staccato"; break;
//                case dot2 | dot5 | dot6: otherValues = "DoublebeatOnNote"; break; // For the time being we omit this because it clashes with 4 lowered in 4/4
            }
            Count(otherValues);


            switch (i) // Look for digits
            {
                case 01: digit = "1"; break;
                case 03: digit = "2"; break;
                case 09: digit = "3"; break;
                case 25: digit = "4"; break;
                case 17: digit = "5"; break;
                case 11: digit = "6"; break;
                case 27: digit = "7"; break;
                case 19: digit = "8"; break;
                case 10: digit = "9"; break;
            }
            if (!string.IsNullOrEmpty(digit)) nextBrailleMusicSubState = BrailleMusicSubState.Number; // Stay in this state !
            Count(digit);

            switch (i) // Look for denominators, i.e numbers lowered one position
            {
                case 02: denominator = "/1"; break;
                case 06: denominator = "/2"; break;
                case 18: denominator = "/3"; break;
                case 50: denominator = "/4"; break;
                case 34: denominator = "/5"; break;
                case 22: denominator = "/6"; break;
                case 54: denominator = "/7"; break;
                case 38: denominator = "/8"; break;
                case 20: denominator = "/9"; break;
            }
            if (!string.IsNullOrEmpty(denominator)) nextBrailleMusicSubState = BrailleMusicSubState.Number; // Stay in this state !
            Count(denominator);



            if (1 != count)
            {
                warning = string.Format("          Warning: {0} interpretations found", count);
            }


            StringBuilder sb = new StringBuilder();


            sb.Append(Format(stepAndType));
            sb.Append(Format("Oct", octave)); // Prefix octave number with "Oct";
            sb.Append(Format(rest));
            sb.Append(Format(accidental));
            sb.Append(Format("Finger", finger));  // Prefix finger number with "Finger";
            sb.Append(Format(interval));
            sb.Append(Format(otherValues));
            sb.Append(Format(digit));
            sb.Append(Format(denominator));
            sb.Append(Format(warning));



            // Change the state AFTER handling the output!
            if (nextBrailleMusicSubState != BrailleMusicSubState.Unchanged)
            {
                brailleMusicSubState = nextBrailleMusicSubState;
            }

            return sb.ToString();

            //return stepName + " " + typeName + " " + (string.IsNullOrEmpty(octave) ? "" : "Oct" + octave) + " " + rest + " " + accidental + finger + interval + otherValues;
        }

        private string Format(string s)
        {
            return (string.IsNullOrEmpty(s) ? "" : " " + s);
        }

        private string Format(string prefix, string s)
        {
            return (string.IsNullOrEmpty(s) ? "" : " " + prefix + s);
        }


        private BrailleMusicDecoder()
        {
            state = StateEnum.Unknown;
        }

        private BrailleMusicDecoder(StateEnum initialState)
        {
            state = initialState;
        }


        public static BrailleMusicDecoder Create()
        {
            return new BrailleMusicDecoder();
        }

        public static BrailleMusicDecoder Create(StateEnum initialState)
        {
            return new BrailleMusicDecoder(initialState);
        }


    }
}
