using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicXmlReaderModel;

namespace MusicXmlReaderModel
{

    /// <summary>
    /// Used during test for decoding Braille Music files into readable symbols
    /// Intensionally does NOT use exicting definitions of symbols in order to avoid duplication of existing errors.
    /// </summary>
    public class BrailleMusicDecoder
    {

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

        // Internal variables
        int count = 0; // Number of interpretations found. Interesting (if <> 1) !! 
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

        string warning = "";

        private void Count(string s)
        {
            if (string.IsNullOrEmpty(s)) return;
            count++;
        }

        public BrailleMusicDecoder(int i)
        {
            if ((i < 0) || (i > 63)) throw new Exception("Invalid argument");
            // First find all step values
            int stepvalue = i & dot1245;

            switch (stepvalue)
            {
                case dot1 | dot4 | dot5: stepName = "C"; break;
                case dot1 | dot5: stepName = "D"; break;
                case dot1 | dot2 | dot4: stepName = "E"; break;
                case dot1 | dot2 | dot4 | dot5: stepName = "F"; break;
                case dot1 | dot2 | dot5: stepName = "G"; break;
                case dot2 | dot4  :  stepName = "A"; break;
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
                    case dot3:  typeName = "1/2"; break;
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
                case dot5: otherValues = "Reference"; break;
                case dot2 | dot3: otherValues = "Triplet"; break;
                case dot3 | dot4 | dot5 : otherValues = "Word"; break;
                case dot3 | dot4 | dot5 | dot6: otherValues = "Number"; break;
                case dot1 | dot4 : otherValues = "Legato"; break;
                case dot2 | dot3 | dot5 | dot6:  otherValues = "Equality"; break;
                case dot2 | dot5: otherValues = "Newline"; break;
                case dot2 | dot3 | dot5: otherValues = "Trill"; break;
                case dot2 | dot6: otherValues = "Ornament"; break; 
                case dot2 | dot3 | dot6: otherValues = "Staccato"; break; 
                case dot2 | dot5 | dot6: otherValues = "DoublebeatOnNote"; break;
            }
            Count(otherValues);

            if (1 != count)
            {
                warning = string.Format("Warning: {0} interpretations found", count);
            }

        }

        public static BrailleMusicDecoder Create(int x)
        {
            return new BrailleMusicDecoder(x);
        }

        public override string  ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(Format(stepAndType));
            sb.Append(Format("Oct",octave)); // Prefix octave number with "Oct";
            sb.Append(Format(rest));
            sb.Append(Format(accidental));
            sb.Append(Format("Finger",finger));  // Prefix finger number with "Finger";
            sb.Append(Format(interval));
            sb.Append(Format(otherValues));
            sb.Append(Format(warning));

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

    }
}
