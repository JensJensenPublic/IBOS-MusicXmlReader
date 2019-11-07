using System;

namespace BrailleMusicDecoder
{
    [Flags]
    enum InputCategoryEnum
    {
        Word = 0x0001,
        Number = 0x0002,
        TextVersal = 0x0004,
        Character = 0x0008,
        Digit = 0x0020,
        Note = 0x0040,
        Rest = 0x0080,
        Octave = 0x0100,
        Interval = 0x0200,
        Accidental = 0x400,
        Finger = 0x0800,
        OtherValues = 0x1000,
        Denominator = 0x2000,
        Space = 0x00010000,
        NewMeasure = 0x00020000,
        Legato = 0x00400000,
        BarLine = 0x00800000,
        FullEnd = 0x01000000,
        EndRepeat = 0x02000000,
        Punctuation = 0x04000000,
        ToMusic = 0x08000000,
        MeasureDivision = 0x10000000,    // Danish "Skilletegn"   
        InAccordPartMeasure = 0x20000000 // Dansih: "Lille bistemme"
    }

    class TokenReader
    {
        // Some basic, general definitions
        public const int BrailleBase = 0x2800;
        public const byte noDots = 0;
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

        /// <summary>
        /// After receiving dot5 it is not possible to determine the next state and ths output without knowig the next value:
        /// If it is Dot2 wh have the sequenec dot5, dot2, which is the signature of Lille Bistemme.
        /// Otherwise we Dot5 just meant "Octave4" and 
        /// </summary>
        /// <param name="nextValue"></param>
        /// <returns></returns>
        private bool Dot5IsLilleBistemme(int nextValue)
        {
            return (nextValue == dot2);
        }
        
        private bool Dot46IsMeasureDivisionMark(int nextValue)
        {
            return (nextValue == (dot1 | dot3));
        }
        
        private string GetCharacter(int i)
        {
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
                //                case 32: return "VERSAL";
                //                case 60: return "CIFFER";
                case 50: return ".";
                case 02: return ",";
                case 38: return "?";
                case 06: return ";";
                case 22: return "!";
                case 54: return "/";
                case 36: return "-";
                default: return null;
            }
        }


        /// <summary>
        /// Returns a list of all POSSIBLE inputvalues, without considering the inputState
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public InputInterpretationList GetInputInterpretations(IntegerList rawValues)
        {

            int thisValue = rawValues.List[0];
            int nextValue = (rawValues.Count > 1) ? rawValues.List[1] : 0x27ff; // An illecgal value

            InputInterpretationList allInputInterpretations = new InputInterpretationList(rawValues);

            // Internal variables

            string stepName = null;
            string typeName = null;

            // Strings for collecting all decoded values
            string stepAndType = null;
            string octave = null;
            string rest = null;
            string accidental = null;
            string finger = null;
            string interval = null;
            string otherValues = null;
            string digit = null;
            string denominator = null;

            // First find all step values
            int stepvalue = thisValue & dot1245;

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
                int typevalue = thisValue & dot36;
                switch (typevalue)
                {
                    case dot3 | dot6: typeName = "1/1"; break;
                    case dot3: typeName = "1/2"; break;
                    case dot6: typeName = "1/4"; break;
                    case none: typeName = "1/8"; break;
                }
                stepAndType = stepName + typeName;
                allInputInterpretations.Add(thisValue, InputCategoryEnum.Note, stepAndType);
            }

            switch (thisValue) // Look for octave marks
            {
                case dot4: octave = "1"; break;
                case dot4 | dot5: octave = "2"; break;
                case dot4 | dot5 | dot6: octave = "3"; break;
                case dot5:
                    if (!Dot5IsLilleBistemme(nextValue))
                    {
                        octave = "4";
                    }
                    break;
                case dot4 | dot6:
                    if (!Dot46IsMeasureDivisionMark(nextValue))
                    {
                        octave = "5";
                    }
                    break;
                case dot5 | dot6: octave = "6"; break;
                case dot6: octave = "7"; break;
                default: break;
            }
            allInputInterpretations.Add(thisValue, InputCategoryEnum.Octave, octave);

            // This was not an octave sign. Continue:

            switch (thisValue) // Look for rests
            {
                case dot1 | dot3 | dot4: rest = "R1/1"; break;
                case dot1 | dot3 | dot6: rest = "R1/2"; break;
                case dot1 | dot2 | dot3 | dot6: rest = "R1/4"; break;
                case dot1 | dot3 | dot4 | dot6: rest = "R1/8"; break;
                default: break;
            }
            allInputInterpretations.Add(thisValue, InputCategoryEnum.Rest, rest);

            switch (thisValue) // Look for accidentals
            {
                case dot1 | dot4 | dot6: accidental = "Sharp"; break;
                case dot1 | dot2 | dot6:
                    if ((nextValue != (dot1 | dot3))  // Avoid clash with Fullend
                    && (nextValue != (dot2 | dot3))) // Avoid clash with EndRepeat
                    {
                        accidental = "Flat";
                    }
                    break;
                case dot1 | dot6: accidental = "Natural"; break;
                default: break;
            }
            allInputInterpretations.Add(thisValue, InputCategoryEnum.Accidental, accidental);

            switch (thisValue) // Look for finger
            {
                case dot1: finger = "1"; break;
                case dot2: finger = "4"; break;
                case dot1 | dot2: finger = "2"; break;
                case dot1 | dot3: finger = "5"; break;
                case dot1 | dot2 | dot3: finger = "3"; break;
            }
            allInputInterpretations.Add(thisValue, InputCategoryEnum.Finger, finger);


            switch (thisValue) // Look for interval
            {
                case dot3 | dot4: interval = "Second"; break;
                case dot3 | dot4 | dot6: interval = "Third"; break;
                case dot3 | dot4 | dot5 | dot6: interval = "Fourth"; break; // Note: already used for "number"?
                case dot3 | dot5: interval = "Fifth"; break;
                case dot3 | dot5 | dot6: interval = "Sixth"; break;
                case dot2 | dot5: interval = "Seventh"; break;
                case dot3 | dot6: interval = "Octave"; break;
            }
            allInputInterpretations.Add(thisValue, InputCategoryEnum.Interval, interval);



            switch (thisValue) // Look for remaining codes
            {
                // Maybe we should use repeated ifs instead of switch here ??
                case dot3: otherValues = "Dotted"; break;
                //                case dot5: otherValues = "Reference"; break; // For the time being we omit this because it clashes with Octave4 !
                case dot2 | dot3: otherValues = "Triplet"; break;
                //                case dot1 | dot4: otherValues = "Legato"; break;
                case dot2 | dot3 | dot5 | dot6: otherValues = "Equality"; break;
                case dot2 | dot5: otherValues = "Newline"; break;
                case dot2 | dot3 | dot5: otherValues = "Trill"; break;
                case dot2 | dot6: otherValues = "Ornament"; break;
                case dot2 | dot3 | dot6: otherValues = "Staccato"; break;
                    //                case dot2 | dot5 | dot6: otherValues = "DoublebeatOnNote"; break; // For the time being we omit this because it clashes with 4 lowered in 4/4
            }
            allInputInterpretations.Add(thisValue, InputCategoryEnum.OtherValues, otherValues);

            // Add some simple obe-character tokens if present
            allInputInterpretations.Add(thisValue, (dot3 | dot4 | dot5 | dot6),  InputCategoryEnum.Number);
            allInputInterpretations.Add(thisValue, (dot3 | dot4 | dot5), InputCategoryEnum.Word);
            allInputInterpretations.Add(thisValue, (dot1 | dot4), InputCategoryEnum.Legato); 
            allInputInterpretations.Add(thisValue, noDots, InputCategoryEnum.Space);
            allInputInterpretations.Add(thisValue, noDots, InputCategoryEnum.NewMeasure);            
            allInputInterpretations.Add(thisValue, (dot1 | dot2 | dot3), InputCategoryEnum.BarLine); 
            allInputInterpretations.Add(thisValue, dot3, InputCategoryEnum.Punctuation);
 
            if ((thisValue == (dot6)) && (nextValue != dot3)) // Avoid clash with ToMusic
            {
                allInputInterpretations.Add(thisValue, InputCategoryEnum.TextVersal);
            }
            
            switch (thisValue) // Look for digits
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
            allInputInterpretations.Add(thisValue, InputCategoryEnum.Digit, digit);

            switch (thisValue) // Look for denominators, i.e numbers lowered one position
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
            allInputInterpretations.Add(thisValue, InputCategoryEnum.Denominator, denominator);


            string character = GetCharacter(thisValue);
            if (null != character)
            {
                allInputInterpretations.Add(thisValue, InputCategoryEnum.Character, character);
            }

            // Now follows interpretations based on more than a single Braille character

            allInputInterpretations.Add(rawValues, new IntegerList((dot1 | dot2 | dot6), (dot1 | dot3)), InputCategoryEnum.FullEnd);
            allInputInterpretations.Add(rawValues, new IntegerList((dot1 | dot2 | dot6), (dot2 | dot3)), InputCategoryEnum.EndRepeat);
            allInputInterpretations.Add(rawValues, new IntegerList((dot6), (dot3)), InputCategoryEnum.ToMusic);
            allInputInterpretations.Add(rawValues, new IntegerList((dot4 | dot6), (dot1 | dot3)), InputCategoryEnum.MeasureDivision);
            allInputInterpretations.Add(rawValues, new IntegerList((dot5), (dot2)), InputCategoryEnum.InAccordPartMeasure);   

            return allInputInterpretations;
        }
        
        private TokenReader()
        {}

        public static TokenReader Create()
        {
            return new TokenReader();
        }
    }
}
