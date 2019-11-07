using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrailleMusicDecoder
{


    [Flags]
    enum InputCategoryEnum {
        ToWord = 0x0001,
        ToNumber = 0x0002,
        ToTextVersal = 0x0004,
        Character = 0x0008,
        Digit = 0x0020,
        Note = 0x0040,
        Rest = 0x0080,
        Octave = 0x0100,
        Interval = 0x0200,
        Accidental = 0x400,
        Finger = 0x0800,
        OtherValues = 0x1000,
        Denominator= 0x2000,
        Dot3 = 0x4000,
        //LoweredDigit = 0x8000,
        Space = 0x00010000,
        NewMeasure = 0x00020000,
        Dot5 = 0x00040000,               // Firat part of the transition to Lille Bistemme    
        LilleBistemmeDot2 = 0x00080000,  // Second part of the transition to Lillle Bistemme
        Dot46 = 0x00100000,           // First part of MEasureDivisionMark 
        MeasureDivisionMarkDot13 = 0x00200000, // Second part of MeasureDivisionMArk
        Legato = 0x00400000,
        BarLine = 0x00800000,
        FullEnd = 0x01000000,
        EndRepeat = 0x02000000,  
        Punctuation= 0x04000000,
        ToMusic = 0x08000000
    }


 



    /// <summary>
    /// 
    /// NOTE!!! This is a primitive initial implementation, only lookin k for a transition from StateEnum.Text to StateEnum.Music  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    /// 
    /// 
    /// Used during test for decoding Braille Music files into readable symbols
    /// Intensionally does NOT use exicting definitions of symbols in order to avoid duplication of existing errors.
    /// </summary>
    public class Decoder
    {
        public const int BrailleBase = 0x2800; 
        public enum StateEnum { Unknown, Text, TextNumber, TextVersal, Music, /* ToMusicOrVersal,*/  MusicNumber, MusicNote, ToLilleBistemme, ToMeasureDivisionMark };
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

        IBrailleMusicDecoderLogger logger;
        StateEnum state = StateEnum.Unknown;
        string brailleAsUnicode;

        public void ResetState()
        {
            state = StateEnum.Text;
        }

        private bool IsBraille6(int c)
        {
            return (((c >= BrailleBase) && (c <= BrailleBase + 63))); 
        }

        private string NonBrailleInterpretation(int c)
        {
            switch (c)
            {
                case 10: return "10 (LF)";
                case 12: return "12 (FF)";
                case 13: return "13 (CR)";
                default: return string.Format("Unexpected character = 0x{0:X04}", c);
            }
        }


        public string GetNextToken(ref int i)
        {
            if ((i<0) || (i >= brailleAsUnicode.Length)) return null; // Outside the array of input characters
            int thisValue = brailleAsUnicode[i];
            int nextValue = (i+1 >= brailleAsUnicode.Length) ? (BrailleBase + noDots) :  brailleAsUnicode[i]; // Insert an empty Braille6 character 

            // Immediately get rid of characters outside the Unicode Braille6 interval [0x2800..0x283f]
            if (!IsBraille6(thisValue))
            {
                i += 1;
                return NonBrailleInterpretation(thisValue);
            }

            // Get a list of ALL POSSIBLE interpretations of the next token
            InputInterpretationList result = ToTokenList(i);

            int tokenLength = 1; // Default, if we can not determine an interpretation we just continue to the next input character
            if (1 == result.Count)
            {
                tokenLength = result.InputInterpretations[0].TokenLength;
                if (1 != tokenLength)
                {
                    Log(string.Format("TokenLength={0}. Changing index from {1} to  {2}", tokenLength, i, i + tokenLength));
                }
            }
            i += tokenLength;
    
            return result.ToString();
        }

        /// <summary>
        /// Pass the call to the object specified during creation!
        /// </summary>
        /// <param name="s"></param>
        private void Log(string s)
        {
            logger.Log(s);
        }

        private InputInterpretationList ToTokenList(int startIndex)
        {
            int endIndex = Math.Min(brailleAsUnicode.Length, startIndex + 10); // Take the next 10 characters 
            IntegerList brailleCharacters = new IntegerList();
            {
                for (int i = startIndex; (i < endIndex); i++)
                {
                    brailleCharacters.Add(brailleAsUnicode[i] - BrailleBase);
                }

            }

            InputInterpretationList inputValues = GetInputInterpretations(brailleCharacters); // Get a list of all possible input values independent of the current state.
            InputCategoryEnum allowedInputCategories = 0;

            // Define some shorthand values to be used in the case below
            InputCategoryEnum allowedInTextStates = InputCategoryEnum.Character | InputCategoryEnum.ToTextVersal | InputCategoryEnum.ToNumber | InputCategoryEnum.ToMusic;

            switch (state)
            {
                case StateEnum.Text:        allowedInputCategories = allowedInTextStates; break;
                case StateEnum.TextVersal:  allowedInputCategories = allowedInTextStates; break;
                case StateEnum.TextNumber:  allowedInputCategories = InputCategoryEnum.Digit | InputCategoryEnum.Denominator | InputCategoryEnum.Space | InputCategoryEnum.ToTextVersal | InputCategoryEnum.ToMusic; break;
//                case StateEnum.ToMusicOrVersal: allowedInputCategories = InputCategoryEnum.Dot3 | InputCategoryEnum.Character; break;
                case StateEnum.Music: allowedInputCategories = InputCategoryEnum.Note | InputCategoryEnum.Octave | InputCategoryEnum.ToNumber | InputCategoryEnum.Finger | InputCategoryEnum.Rest | InputCategoryEnum.NewMeasure | InputCategoryEnum.Dot46; break;
                case StateEnum.MusicNumber: allowedInputCategories = InputCategoryEnum.Digit | InputCategoryEnum.Denominator | InputCategoryEnum.Space | InputCategoryEnum.Accidental; break;
                case StateEnum.MusicNote: allowedInputCategories = InputCategoryEnum.Interval | InputCategoryEnum.Note | InputCategoryEnum.Octave | InputCategoryEnum.Accidental | InputCategoryEnum.NewMeasure | InputCategoryEnum.Rest | InputCategoryEnum.Dot5 | InputCategoryEnum.Dot46 | InputCategoryEnum.Legato | InputCategoryEnum.BarLine | InputCategoryEnum.FullEnd | InputCategoryEnum.EndRepeat | InputCategoryEnum.Punctuation; break; // TODO
                case StateEnum.ToLilleBistemme: allowedInputCategories = InputCategoryEnum.LilleBistemmeDot2; break;
                case StateEnum.ToMeasureDivisionMark: allowedInputCategories = InputCategoryEnum.MeasureDivisionMarkDot13; break;
                default: throw new Exception(string.Format("Unsupported state {0} ", state.ToString()));
            }


            InputInterpretationList filteredInputValues = inputValues.Filter(allowedInputCategories);


            // Calculate the new state
            StateEnum newState = state;

            switch (state)
            {
                case StateEnum.Text:
                    if (filteredInputValues.Contains(InputCategoryEnum.ToMusic))
                    {
                        newState = StateEnum.Music; break;
                    }
                    if (filteredInputValues.Contains(InputCategoryEnum.ToNumber))
                    {
                        newState = StateEnum.TextNumber;
                    }
                    break;
                case StateEnum.TextVersal:
                    if (filteredInputValues.Contains(InputCategoryEnum.ToMusic))
                    {
                        newState = StateEnum.Music; break;
                    }
                    if (filteredInputValues.Contains(InputCategoryEnum.ToNumber))
                    {
                        newState = StateEnum.TextNumber;
                    }
                    break;
                case StateEnum.TextNumber:
                    if (filteredInputValues.Contains(InputCategoryEnum.ToMusic))
                    {
                        newState = StateEnum.Music;
                        break;
                    }
                    if (!filteredInputValues.Contains(InputCategoryEnum.Digit))
                    {
                        newState = StateEnum.Text;
                        break;
                    }
                    // Remain in StateEnum.TextNumber
                    break;   
                case StateEnum.Music:
                    if (filteredInputValues.Contains(InputCategoryEnum.ToNumber))
                    {
                        newState = StateEnum.MusicNumber; break;
                    }
                    if (filteredInputValues.Contains(InputCategoryEnum.Note))
                    {
                        newState = StateEnum.MusicNote; break;
                    }
                    break;

                case StateEnum.MusicNumber:
                    if (filteredInputValues.Contains(InputCategoryEnum.Space))
                    {
                        newState = StateEnum.Music;
                    }
                    break;

                case StateEnum.MusicNote:
                    if (filteredInputValues.Contains(InputCategoryEnum.NewMeasure))
                    {
                        newState = StateEnum.Music;
                    }
                    if (filteredInputValues.Contains(InputCategoryEnum.Dot5))
                    {
                        newState = StateEnum.ToLilleBistemme;
                    }

                    if (filteredInputValues.Contains(InputCategoryEnum.Dot46))
                    {
                        newState = StateEnum.ToMeasureDivisionMark;
                    }
                    break;

                case StateEnum.ToLilleBistemme:
                    if (filteredInputValues.Contains(InputCategoryEnum.LilleBistemmeDot2))
                    {
                        newState = StateEnum.MusicNote; // We are now ready to interpret notes within the Lille Bistemme (Second sequence or later)
                    }

                    break;

                case StateEnum.ToMeasureDivisionMark:
                    if (filteredInputValues.Contains(InputCategoryEnum.MeasureDivisionMarkDot13))
                    {
                        newState = StateEnum.MusicNote; // We are now ready to interpret notes within the Lille Bistemme (First sequence)
                    }
                    break;

            }

            string result = filteredInputValues.ToString();

            int thisValue = brailleCharacters.List[0];

            char inputAsUnicode = (char)(thisValue + BrailleBase);
            if (1 != filteredInputValues.Count)
            {
                Log(string.Format(" State={0,-15} Input={1}(i={2,02}) OriginalInputValues = {3}", state.ToString(), inputAsUnicode, thisValue ,inputValues.ToString()));
                Log(string.Format(" State={0,-15} Input={1}(i={2,02}) FilteredInputValues = {3}", state.ToString(), inputAsUnicode, thisValue ,filteredInputValues.ToString()));
            }

            string newStateText = (state != newState) ? string.Format("NewState={0} ", newState) : "";
        
            Log(string.Format(" State={0,-15} Input={1}(i={2,02}) Result='{3}' {4} ", state.ToString(), inputAsUnicode, thisValue, result, newStateText));

            if (newState != state)
            {
//                Logger.Log(string.Format(": >>>>>>>>>>>>>Changing state from {0} to {1}<<<<<<<<<", state, newState));
            }


            // Generate the output

            // Update the state to the new state

            state = newState;

            return filteredInputValues;

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

        private string DigitToString(int i)
        {
            return "DIGIT";
        }



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

        /// <summary>
        /// Returns a list of all POSSIBLE inputvalues, without considering the inputState
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        private InputInterpretationList GetInputInterpretations(IntegerList rawValues)
        {
            
            int thisValue = rawValues.List[0];
            int nextValue = (rawValues.Count > 1) ? rawValues.List[1] : 0x27ff; // An illecgal value

            InputInterpretationList allInputInterpretations = new InputInterpretationList(rawValues);

            // Internal variables

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
                allInputInterpretations.Add(thisValue,InputCategoryEnum.Note, stepAndType);                
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
                case dot6:  octave = "7";  break;
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
                    &&  (nextValue != (dot2 | dot3))) // Avoid clash with EndRepeat
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

            if (thisValue == (dot3 | dot4 | dot5 | dot6))
            {
                allInputInterpretations.Add(thisValue, InputCategoryEnum.ToNumber, "Number");
            }

            if (thisValue == (dot3 | dot4 | dot5 ))
            {
                allInputInterpretations.Add(thisValue, InputCategoryEnum.ToWord, "Word");
            }

            if ((thisValue == (dot6)) && (nextValue != dot3)) // Avoid clash with ToMusic
            {
                allInputInterpretations.Add(thisValue, InputCategoryEnum.ToTextVersal, "ToTextVersal");
            }

            if (thisValue == (dot3))
            {
                allInputInterpretations.Add(thisValue, InputCategoryEnum.Dot3, "Dot3");
            }

            if (thisValue == (dot5))
            {
                if (Dot5IsLilleBistemme(nextValue))
                {
                    allInputInterpretations.Add(thisValue, InputCategoryEnum.Dot5, "ToLilleBistemme"); // First part of mark for "Lille Bistemme"
                }
            }


            if (thisValue == (dot2))
            {
                allInputInterpretations.Add(thisValue, InputCategoryEnum.LilleBistemmeDot2, "LilleBistemmeDot2"); // Second part of mark for "Lille Bistemme"
            }


            if (thisValue == (dot4 | dot6))
            {

                if (Dot46IsMeasureDivisionMark(nextValue))
                {
                    allInputInterpretations.Add(thisValue, InputCategoryEnum.Dot46, "MeasureDivisionMark"); // First part of mark for "MeasureDivisionMark" (Danish "SkilleTEgn")
                }
            }

            if (thisValue == (dot1 | dot3))
            {
                allInputInterpretations.Add(thisValue, InputCategoryEnum.MeasureDivisionMarkDot13, "MeasureDivisionMarkDot13"); // Second part of mark for "MeasureDivisionMark" (Danish "SkilleTEgn")
            }

            if (thisValue == (dot1 | dot4))
            {
                allInputInterpretations.Add(thisValue, InputCategoryEnum.Legato, "Legato");
            }


            if (thisValue == noDots)
            {
                allInputInterpretations.Add(thisValue, InputCategoryEnum.Space, "SPACE");
            }

            if (thisValue == noDots)
            {
                allInputInterpretations.Add(thisValue, InputCategoryEnum.NewMeasure, "NewMeasure");
            }

            if (thisValue == (dot1 | dot2 | dot3))
            {
                allInputInterpretations.Add(thisValue, InputCategoryEnum.BarLine, "Unusual Barline");
            }


            if (thisValue == dot3)
            {
                allInputInterpretations.Add(thisValue, InputCategoryEnum.Punctuation, "Punctuation");
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

            IntegerList fullEndSequence = new IntegerList((dot1 | dot2 | dot6), (dot1 | dot3));
            if (rawValues.StartsWith(fullEndSequence))
            {
                allInputInterpretations.Add(fullEndSequence, InputCategoryEnum.FullEnd, "FullEnd");
            }

            IntegerList endRepeatSequence = new IntegerList((dot1 | dot2 | dot6), (dot2 | dot3));
            if (rawValues.StartsWith(endRepeatSequence))
            {
                allInputInterpretations.Add(endRepeatSequence, InputCategoryEnum.EndRepeat, "RepeatEnd");
            }

            IntegerList musicBrailleSequence = new IntegerList((dot6), (dot3));
            if (rawValues.StartsWith(musicBrailleSequence))
            {
                allInputInterpretations.Add(musicBrailleSequence, InputCategoryEnum.ToMusic, "ToMusicBraille");
            }



            return allInputInterpretations;

        }

        private string Format(string s)
        {
            return (string.IsNullOrEmpty(s) ? "" : " " + s);
        }

        private string Format(string prefix, string s)
        {
            return (string.IsNullOrEmpty(s) ? "" : " " + prefix + s);
        }


        private Decoder()
        {
            state = StateEnum.Unknown;
        }

        private Decoder(StateEnum initialState, string brailleAsUnicode,IBrailleMusicDecoderLogger logger)
        {
            state = initialState;
            this.brailleAsUnicode = brailleAsUnicode;
            this.logger = logger;
        }


        public static Decoder Create()
        {
            return new Decoder();
        }

        public static Decoder Create(StateEnum initialState, string brailleAsUnicode, IBrailleMusicDecoderLogger logger)
        {
            return new Decoder(initialState, brailleAsUnicode,logger);
        }


    }
}
