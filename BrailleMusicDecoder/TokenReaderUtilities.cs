using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicXmlReaderModel;

namespace BrailleMusicDecoder
{
    class TokenReaderUtilities: TokenReaderBase
    {

        // Just to avoid a lot of stupid problems
        // The "alwaysFalse" parameter is osed to prevent compiler warnings about unreachable code !
        public bool CheckDefinitions(bool alwaysFalse)
        {
            if ((digit0Lowered != 2 * digit0)
            || (digit1Lowered != 2 * digit1)
            || (digit2Lowered != 2 * digit2)
            || (digit3Lowered != 2 * digit3)
            || (digit4Lowered != 2 * digit4)
            || (digit5Lowered != 2 * digit5)
            || (digit6Lowered != 2 * digit6)
            || (digit7Lowered != 2 * digit7)
            || (digit8Lowered != 2 * digit8)
            || (digit9Lowered != 2 * digit9)
            || alwaysFalse) return false;

            // Check that no Input catogories overlap.
            ulong logicalSum = 0;
            ulong arithmeticSum = 0;
            var values = Enum.GetValues(typeof(InputCategoryEnum));
            Logger.LogCF(string.Format(": InputCategoryEnum contains {0} values including 'NONE'", values.Length));
            foreach (InputCategoryEnum value in values)
            {
                if (value != InputCategoryEnum.AllCategories)
                {
                    logicalSum |= (ulong)value;
                    arithmeticSum += (ulong)value;
                }
            }
            if (logicalSum != arithmeticSum)
            {
                string s = string.Format(": InputCategoryEnum contains overlapping flags: ArithmeticSum=0x{0:X} LogicalSum=0x{1:X}", arithmeticSum, logicalSum);
                Logger.LogCF(s);
                return false;
            }
            else
            {
                string s = string.Format(": InputCategoryEnum flags in use 0x{0:X} ", logicalSum);
                Logger.LogCF(s);
            }

            return true;
        }

        public string ToLocalizedString(OtherValuesNameEnum otherValuesName)
        {
            switch (otherValuesName)
            {
                case OtherValuesNameEnum.equality: return ResourcesForBrailleMusicDecoder.OtherValuesNameEnum_Equality;
                case OtherValuesNameEnum.newline: return ResourcesForBrailleMusicDecoder.OtherValuesNameEnum_NewLine;
                case OtherValuesNameEnum.ornament: return ResourcesForBrailleMusicDecoder.OtherValuesNameEnum_Ornament;
                case OtherValuesNameEnum.trill: return ResourcesForBrailleMusicDecoder.OtherValuesNameEnum_Trill;
                default: return "";
            }
        }

        /// <summary>
        /// Add a possible interpretation as a note with step and type.
        /// </summary>
        /// <param name="allInputInterpretations"></param>
        /// <param name="thisValue"></param>
        public void AddStep(InputInterpretationList allInputInterpretations, int thisValue)
        {
            string stepName = null;
            string typeName = null;
            string stepAndType = null;
            // First find all step values
            int stepvalue = thisValue & dot1245;
            InputSubCategoryEnum stepEnum = InputSubCategoryEnum.None; // 
            switch (stepvalue)
            {
                case dot145: stepName = "C"; stepEnum = InputSubCategoryEnum.FullStepC; break;
                case dot15: stepName = "D"; stepEnum = InputSubCategoryEnum.FullStepD; break;
                case dot124: stepName = "E"; stepEnum = InputSubCategoryEnum.FullStepE; break;
                case dot1245: stepName = "F"; stepEnum = InputSubCategoryEnum.FullStepF; break;
                case dot125: stepName = "G"; stepEnum = InputSubCategoryEnum.FullStepG; break;
                case dot24: stepName = "A"; stepEnum = InputSubCategoryEnum.FullStepA; break;
                case dot245: stepName = "B"; stepEnum = InputSubCategoryEnum.FullStepB; break;
                default: break;
            }
            if (!string.IsNullOrEmpty(stepName))
            {
                InputSubSubCategoryEnum typeEnum = InputSubSubCategoryEnum.Unknown;
                // This is a pitched note, find the type
                int typevalue = thisValue & dot36;
                switch (typevalue)
                {
                    case dot36: typeName = "1"; typeEnum = InputSubSubCategoryEnum.NoteTypeFullMeasureOrWholeOr16th; break; // Full or 1/16
                    case dot3: typeName = "2"; typeEnum = InputSubSubCategoryEnum.NoteTypeHalfOr32nd; break; //  Half of 1/32
                    case dot6: typeName = "4"; typeEnum = InputSubSubCategoryEnum.NoteTypeQuarterOr64th; break; // Quarter of 1/64
                    case noDots: typeName = "8"; typeEnum = InputSubSubCategoryEnum.NoteTypeEighthOr128th; break; // Eighth or 1/128
                }
                stepAndType = stepName + "/" + typeName;
                allInputInterpretations.Add(thisValue, InputCategoryEnum.Note, stepAndType, stepEnum, typeEnum);
            }
        }

        /// <summary>
        ///  Add a possible interpretation as an octave number.
        /// </summary>
        /// <param name="allInputInterpretations"></param>
        /// <param name="thisValue"></param>
        /// <param name="nextValue"></param>
        public void AddOctave(InputInterpretationList allInputInterpretations, int thisValue, int nextValue)
        {
            string octave = null;
            switch (thisValue) // Look for octave marks
            {
                case dot4: octave = "1"; break;
                case dot45: octave = "2"; break;
                case dot456: octave = "3"; break;
                case dot5:
                    if (!Dot5IsLilleBistemme(nextValue))
                    {
                        octave = "4";
                    }
                    break;
                case dot46:
                    if (!Dot46IsMeasureDivisionMark(nextValue))
                    {
                        octave = "5";
                    }
                    break;
                case dot56: octave = "6"; break;
                case dot6: octave = "7"; break;
                default: break;
            }
            allInputInterpretations.Add(thisValue, InputCategoryEnum.Octave, octave);
        }

        public void AddChordSymbol(InputInterpretationList allInputInterpretations, int thisValue)
        {
            string chordSymbol = null;
            InputSubCategoryEnum chordSymbolSubCategory = InputSubCategoryEnum.None;
            switch (thisValue) // Look for special chord symbols See BANA 2015 Table 23
            {
                case dot16: chordSymbol = "Natural"; chordSymbolSubCategory = InputSubCategoryEnum.ChordSymbolNatural; break;
                //case dot126: chordSymbol = "Flat"; chordSymbolSubCategory = InputSubCategoryEnum.ChordSymbolFlat; break;
                //case dot146: chordSymbol = "Sharp"; chordSymbolSubCategory = InputSubCategoryEnum.ChordSymbolSharp; break;
                case dot346: chordSymbol = "+"; chordSymbolSubCategory = InputSubCategoryEnum.ChordSymbolAug; break;
                //case dot36: chordSymbol = "-"; chordSymbolSubCategory = InputSubCategoryEnum.ChordSymbolMinus; break;
                case dot256: chordSymbol = "dim"; chordSymbolSubCategory = InputSubCategoryEnum.ChordSymbolDim; break;
                // HAlfDiminished, TriangleBisecedByALine, Parentesis, NC and Tacet need more than one symbol and are added separately below
                case dot356: chordSymbol = "maj"; chordSymbolSubCategory = InputSubCategoryEnum.ChordSymbolMaj; break;
                //case dot34:  chordSymbol = "/"; break; // Removed 20200528 to avoid clash with the normal character "/"
                default: return;
            }
            allInputInterpretations.Add(thisValue, InputCategoryEnum.ChordSymbol, chordSymbol, chordSymbolSubCategory);
        }

        public void AddRests(InputInterpretationList allInputInterpretations, int thisValue)
        {
            string rest = null;
            InputSubSubCategoryEnum restType = InputSubSubCategoryEnum.Unknown;
            switch (thisValue) // Look for rests
            {
                case dot134: rest = "/1"; restType = InputSubSubCategoryEnum.NoteTypeFullMeasureOrWholeOr16th; break;
                case dot136: rest = "/2"; restType = InputSubSubCategoryEnum.NoteTypeHalfOr32nd; break;
                case dot1236: rest = "/4"; restType = InputSubSubCategoryEnum.NoteTypeQuarterOr64th; break;
                case dot1346: rest = "/8"; restType = InputSubSubCategoryEnum.NoteTypeEighthOr128th; break;
                default: return;
            }
            allInputInterpretations.Add(thisValue, InputCategoryEnum.Rest, rest, InputSubCategoryEnum.None, restType); // No FullStep to specify
        }

        public void AddFingers(InputInterpretationList allInputInterpretations, int thisValue)
        {
            string finger = null;
            switch (thisValue) // Look for finger
            {
                case dot1: finger = "1"; break;
                case dot2: finger = "4"; break;
                case dot12: finger = "2"; break;
                case dot13: finger = "5"; break;
                case dot123: finger = "3"; break;
                default: return;
            }
            allInputInterpretations.Add(thisValue, InputCategoryEnum.Finger, finger);
        }


        public void AddIntervals(InputInterpretationList allInputInterpretations, int thisValue)
        {
            InputSubCategoryEnum intervalSubCategory = InputSubCategoryEnum.None;
            switch (thisValue) // Look for interval
            {
                case IntervalSecond: intervalSubCategory = InputSubCategoryEnum.IntervalSecond; break;
                case IntervalThird: intervalSubCategory = InputSubCategoryEnum.IntervalThird; break;
                case IntervalFourth: intervalSubCategory = InputSubCategoryEnum.IntervalFourth; break; // Note: already used for "number"?
                case IntervalFifth: intervalSubCategory = InputSubCategoryEnum.IntervalFifth; break;
                case IntervalSixth: intervalSubCategory = InputSubCategoryEnum.IntervalSixth; break;
                case IntervalSeventh: intervalSubCategory = InputSubCategoryEnum.IntervalSeventh; break;
                case IntervalOctave: intervalSubCategory = InputSubCategoryEnum.IntervalOctave; break;
                default: return;
            }
            allInputInterpretations.Add(thisValue, InputCategoryEnum.Interval, "", intervalSubCategory, InputSubSubCategoryEnum.OccursOnce);
        }

        public void AddPartMeasureRepeat(InputInterpretationList allInputInterpretations, int thisValue)
        {
            InputSubCategoryEnum partMeasureRepeatSubcategory = InputSubCategoryEnum.None;
            switch (thisValue) // Look for remaining codes
            {
                case dot2356: partMeasureRepeatSubcategory = InputSubCategoryEnum.PartMeasureRepeatOnce; break; // BANA 2015 Measure or part-measure repeat (18) 18.1–18.5
                default: return;
            }
            allInputInterpretations.Add(thisValue, InputCategoryEnum.PartMeasureRepeat, "", partMeasureRepeatSubcategory);
        }


        public void AddOtherValues(InputInterpretationList allInputInterpretations, int thisValue)
        {
            string otherValues = null;
            InputSubCategoryEnum otherValuesSubcategory = InputSubCategoryEnum.None;
            switch (thisValue) // Look for remaining codes
            {
                // Maybe we should use repeated ifs instead of switch here ??
                // case dot3: otherValues = "Dotted"; break;
                //                case dot5: otherValues = "Reference"; break; // For the time being we omit this because it clashes with Octave4 !

                //                case dot14: otherValues = "Legato"; break;
//                case dot2356: otherValuesSubcategory = InputSubCategoryEnum.OtherValuesPartMeasureRepeat; otherValues = ""; break; // BANA 2015 Measure or part-measure repeat (18) 18.1–18.5
                //case dot25: otherValues = "Newline"; break; // Same as Character("-") 
                case dot235: otherValuesSubcategory = InputSubCategoryEnum.OtherValuesTrill; otherValues = ""; /*  ToLocalizedString(OtherValuesNameEnum.trill); */ break;
                case dot26: otherValuesSubcategory = InputSubCategoryEnum.OthervaluesShortAppoggiatura; otherValues = ""; break; // BANA 2015: Table 16. Ornaments
                                                                                                                                 //                case dot256: otherValues = "DoublebeatOnNote"; break; // For the time being we omit this because it clashes with 4 lowered in 4/4
                default: return;
            }
            allInputInterpretations.Add(thisValue, InputCategoryEnum.OtherValues, otherValues, otherValuesSubcategory);
        }

        public void AddDigits(InputInterpretationList allInputInterpretations, int thisValue)
        {
            string digit = null;
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
                case 26: digit = "0"; break;
                case 04: digit = "."; break; // dot3 = "." is a valid value inside a sequence of digits
                case 36: digit = "_"; break; // dot3 || dot6 = "_" is a valid value inside a sequence of digits
                default: return;
            }
            allInputInterpretations.Add(thisValue, InputCategoryEnum.Digit, digit);
        }

        public void AddLoweredDigits(InputInterpretationList allInputInterpretations, int thisValue)
        {
            string loweredDigit = null;
            switch (thisValue) // Look for lovered digits
            {
                case dot2: loweredDigit = "1"; break;
                case dot23: loweredDigit = "2"; break;
                case dot25: loweredDigit = "3"; break;
                case dot256: loweredDigit = "4"; break;
                case dot26: loweredDigit = "5"; break;
                case dot235: loweredDigit = "6"; break;
                case dot2356: loweredDigit = "7"; break;
                case dot236: loweredDigit = "8"; break;
                case dot35: loweredDigit = "9"; break;
                case dot356: loweredDigit = "0"; break;
                case dot3: loweredDigit = "."; break; // Accept and ignore a simple dot (".") at this place
                default: return;
            }
            allInputInterpretations.Add(thisValue, InputCategoryEnum.LoweredDigit, loweredDigit);
        }

        public void AddDenominator(InputInterpretationList allInputInterpretations, int thisValue)
        {
            string denominator = null;
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
                case 52: denominator = "/0"; break; // Probably not needed ? Or needed for ordinals ?
                default: return;
            }
            allInputInterpretations.Add(thisValue, InputCategoryEnum.Denominator, denominator);
        }





        public void AddBasses(InputInterpretationList allInputInterpretations, IntegerList rawValues)
        {
            AddSteps(slashForward, InputCategoryEnum.ChordSymbolBass, allInputInterpretations, rawValues);
        }

        public void AddRoots(InputInterpretationList allInputInterpretations, IntegerList rawValues)
        {
            AddSteps(toVersal, InputCategoryEnum.ChordSymbolRoot, allInputInterpretations, rawValues);
        }

        public void AddSteps(int prefix, InputCategoryEnum inputCategoryEnum, InputInterpretationList allInputInterpretations, IntegerList rawValues)
        {
            // Special consideration:
            // If the root or bass ends by a "flat" and is followed by DOT23 it is interpreted as a flat, but DOT23 has now interpretation!
            // So instead we must interpreted as a normal root followed by DOT126 DOT23 which marks the end  of a repeatsequence: "?????????????????"
            // If the root or bass ends by a "flat" and is followed by DOT13 it is interpreted as a flat, but DOT13 is interpretated as ChordChar "K"
            // So instead we must interpreted as a normal root followed by DOT126 DOT13 which is the start of a double bar
            bool allowFlat = true;
            if (rawValues.Count >= 4)
            {
                int rawValue3 = rawValues.List[3];
                if (rawValue3 == dot23      // Repeat Sequence
                || (rawValue3 == dot13))    // Double Bar
                {
                    allowFlat = false;
                    if (Logger.DeveloperMode)
                    {
                        Logger.LogCF(string.Format(": Setting AllowFlat=false because rawValues.Count={0} and rawValues.List[3] == dot23 or dot13 ", rawValues.Count));
                    }
                }
            }
            if (allowFlat)
            {
                allInputInterpretations.Add(rawValues, new IntegerList(prefix, letterA, AccidentalFlat), inputCategoryEnum, A_Flat, InputSubCategoryEnum.FullStepA, "-1");
                allInputInterpretations.Add(rawValues, new IntegerList(prefix, letterB, AccidentalFlat), inputCategoryEnum, B_Flat, InputSubCategoryEnum.FullStepB, "-1");
                allInputInterpretations.Add(rawValues, new IntegerList(prefix, letterC, AccidentalFlat), inputCategoryEnum, C_Flat, InputSubCategoryEnum.FullStepC, "-1");
                allInputInterpretations.Add(rawValues, new IntegerList(prefix, letterD, AccidentalFlat), inputCategoryEnum, D_Flat, InputSubCategoryEnum.FullStepD, "-1");
                allInputInterpretations.Add(rawValues, new IntegerList(prefix, letterE, AccidentalFlat), inputCategoryEnum, E_Flat, InputSubCategoryEnum.FullStepE, "-1");
                allInputInterpretations.Add(rawValues, new IntegerList(prefix, letterF, AccidentalFlat), inputCategoryEnum, F_Flat, InputSubCategoryEnum.FullStepF, "-1");
                allInputInterpretations.Add(rawValues, new IntegerList(prefix, letterG, AccidentalFlat), inputCategoryEnum, G_Flat, InputSubCategoryEnum.FullStepG, "-1");
            }


            allInputInterpretations.Add(rawValues, new IntegerList(prefix, letterA), inputCategoryEnum, A, InputSubCategoryEnum.FullStepA, "");
            allInputInterpretations.Add(rawValues, new IntegerList(prefix, letterA, AccidentalSharp), inputCategoryEnum, A_Sharp, InputSubCategoryEnum.FullStepA, "+1");

            allInputInterpretations.Add(rawValues, new IntegerList(prefix, letterB), inputCategoryEnum, B, InputSubCategoryEnum.FullStepB, "");
            allInputInterpretations.Add(rawValues, new IntegerList(prefix, letterB, AccidentalSharp), inputCategoryEnum, B_Sharp, InputSubCategoryEnum.FullStepB, "+1");

            allInputInterpretations.Add(rawValues, new IntegerList(prefix, letterC), inputCategoryEnum, C, InputSubCategoryEnum.FullStepC, "");
            allInputInterpretations.Add(rawValues, new IntegerList(prefix, letterC, AccidentalSharp), inputCategoryEnum, C_Sharp, InputSubCategoryEnum.FullStepC, "+1");

            allInputInterpretations.Add(rawValues, new IntegerList(prefix, letterD), inputCategoryEnum, D, InputSubCategoryEnum.FullStepD, "");
            allInputInterpretations.Add(rawValues, new IntegerList(prefix, letterD, AccidentalSharp), inputCategoryEnum, D_Sharp, InputSubCategoryEnum.FullStepD, "+1");

            allInputInterpretations.Add(rawValues, new IntegerList(prefix, letterE), inputCategoryEnum, E, InputSubCategoryEnum.FullStepE, "");
            allInputInterpretations.Add(rawValues, new IntegerList(prefix, letterE, AccidentalSharp), inputCategoryEnum, E_Sharp, InputSubCategoryEnum.FullStepE, "+1");

            allInputInterpretations.Add(rawValues, new IntegerList(prefix, letterF), inputCategoryEnum, F, InputSubCategoryEnum.FullStepF, "");
            allInputInterpretations.Add(rawValues, new IntegerList(prefix, letterF, AccidentalSharp), inputCategoryEnum, F_Sharp, InputSubCategoryEnum.FullStepF, "+1");

            allInputInterpretations.Add(rawValues, new IntegerList(prefix, letterG), inputCategoryEnum, G, InputSubCategoryEnum.FullStepG, "");
            allInputInterpretations.Add(rawValues, new IntegerList(prefix, letterG, AccidentalSharp), inputCategoryEnum, G_Sharp, InputSubCategoryEnum.FullStepG, "+1");
        }


        /// <summary>
        /// Convert to plain text (such as in the Title) using regiondependent contractions and other rules
        /// </summary>
        /// <param name="allInputInterpretations"></param>
        /// <param name="thisValue"></param>
        public void AddTextCharacter(InputInterpretationList allInputInterpretations, int thisValue)
        {
            string character = textBrailleToTextConverter.GetCharacter(thisValue);
            if (null != character)
            {
                // Assign a separate subcategory to the Braille "empty space" (Unicode 0x2800);
                InputSubCategoryEnum characterSubCategory = InputSubCategoryEnum.None;
                if (0 == character.CompareTo(" "))
                {
                    characterSubCategory = InputSubCategoryEnum.CharacterBlank;
                }
                allInputInterpretations.Add(thisValue, InputCategoryEnum.Character, character, characterSubCategory);
            }
            else
            {
                // If the character is not found we assume that it is a contraction
                string expandedValue = textBrailleToTextConverter.ExpandContraction(thisValue);
                if (null != expandedValue)
                {
                    // Same action as for the non-contracted value, but marked as an expanded value.
                    allInputInterpretations.Add(thisValue, InputCategoryEnum.Character, expandedValue, InputSubCategoryEnum.CharacterExpandedContraction);
                }
            }
        }



        public void AddChordCharacter(InputInterpretationList allInputInterpretations, int thisValue)
        {
            string s = GetChordCharacter(thisValue);
            if (null == s) return;
            allInputInterpretations.Add(thisValue, InputCategoryEnum.ChordCharacter, s);
        }


        /// <summary>
        /// Same as GetCharacter except that some non-alphabetic characters have been removed
        /// in order to avoid collisio9ns with for instance the "dim" chord symbol
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        private string  GetChordCharacter(int i)
        {
            switch (i)
            {
                // Primitive mapping. Add more as needed !
                // NOTE: We can probably handle a lot of national differences by localizing parts of this table !!
                //case 00: return " "; // Is categorized as InputCharacterEnum.NewMeasure
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
                //                case 13: return "M"; // Used as a special HrmonySymbol for Minor
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
                // The remaining characters are not expected in chords or have a different meaning
                //                case 32: return "VERSAL";
                //                case 60: return "CIFFER";
                // case 50: return ".";
                //case 02: return ",";
                //case 38: return "?";
                //case 06: return ";";
                //case 22: return "!";
                //case 54: return "/";
                //case 36: return "-";
                case 04: return "."; // Seems to be used as a fill in character in some cases
                //case 18: return ":"; // Or "-" ??
                //case 56: return ResourcesForBrailleMusicDecoder.InputCharacter_Uppercase;
                ////                case 56: return "UPPERCASE";
                case 12: return "/"; // Used in for instance C/f
                default: return null;
            }
        }



        public static string ToNonBrailleInterpretation(int binaryValue)
        {
            switch (binaryValue)
            {
                case carriageReturn: return "CR";
                case lineFeed: return "LF";
                case formFeed: return "FF";
                default: return "??";
            }
        }


        public int Blank { get { return BrailleBase + noDots; } }

#warning todo update to new method for decoding multiple bytes 
        /// <summary>
        /// After receiving dot5 it is not possible to determine the next state and ths output without knowig the next value:
        /// If it is Dot2 wh have the sequenec dot5, dot2, which is the signature of Lille Bistemme.
        /// Otherwise we Dot5 just meant "Octave4" and 
        /// </summary>
        /// <param name="nextValue"></param>
        /// <returns></returns>
        public bool Dot5IsLilleBistemme(int nextValue)
        {
            return (nextValue == dot2);
        }

        public bool Dot46IsMeasureDivisionMark(int nextValue)
        {
            return (nextValue == dot13);
        }



        public bool IsBraille6(int c)
        {
            return (((c >= BrailleBase) && (c <= BrailleBase + 63)));
        }

        public int ToBraille(char c)
        {
            return c - BrailleBase;
        }


        /// <summary>
        /// Convert from the internal braille representation, which represents Braille6 as numbers in [0..63] and has special values for CR LF  and FF - and nothing else !
        /// </summary>
        /// <param name="i">The internal code to convert</param>
        /// <returns>The Unicode representation of the input parameter</returns>
        public static string ToUnicodeChar(int i)
        {
            if ((0 <= i) && (i <= 63))
            {
                return ((char)(i + BrailleBase)).ToString();
            }

            switch (i)
            {
                case carriageReturn: return " CR";
                case lineFeed: return " LF";
                case formFeed: return " FF";
                default:
                    Logger.LogCF(string.Format(": Unexpected input: i={0}", i));
                    return " ??"; // Represents an unexpected value !
            }
        }

        public int ToNotBraille(char c)
        {  
            switch (c)
            {
                case '\r': return carriageReturn; 
                case '\n': return  lineFeed;
                case '\f': return formFeed;
                default: return -1;  
            }
        }


    private bool IsDigit(int i, out int value)
        {
            value = -1;
            switch (i)
            {
                case digit0: value = 0; return true;
                case digit1: value = 1; return true;
                case digit2: value = 2; return true;
                case digit3: value = 3; return true;
                case digit4: value = 4; return true;
                case digit5: value = 5; return true;
                case digit6: value = 6; return true;
                case digit7: value = 7; return true;
                case digit8: value = 8; return true;
                case digit9: value = 9; return true;
            }
            return false;
        }

        private bool IsLoweredDigit(int i, out int value)
        {
            value = -1;
            switch (i)
            {
                case digit0Lowered: value = 0; return true;
                case digit1Lowered: value = 1; return true;
                case digit2Lowered: value = 2; return true;
                case digit3Lowered: value = 3; return true;
                case digit4Lowered: value = 4; return true;
                case digit5Lowered: value = 5; return true;
                case digit6Lowered: value = 6; return true;
                case digit7Lowered: value = 7; return true;
                case digit8Lowered: value = 8; return true;
                case digit9Lowered: value = 9; return true;
            }
            return false;
        }


        private bool IsDigit(int i, out int value, bool lowered)
        {
            return lowered ? IsLoweredDigit(i, out value) : IsDigit(i, out value);
        }
        

        /// <summary>
        /// Looks for a sequence of digits or lowered digits in the list, starting at index.
        /// Updates index to the first index which is not a digit / lowered digit.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool GetNumber(List<int> list, ref int index, out int value, bool lowered)
        {
            int i;
            bool result = false; // Returns true if at least one lowered digit is found
            int number = 0;
            for (i = index; (i < list.Count);)
            {
                int digit;
                if (IsDigit(list[i], out digit, lowered))
                {
                    result = true;
                    number = number * 10 + digit;
                    i++;
                }
                else
                {
                    break; // At the first item not representing a digit
                }
            }
            index = i; // Points to first int after the result
            value = result ? number : 0;
            return result;
        }

        /// <summary>
        /// Implicitly NOT lowered number
        /// </summary>
        /// <param name="list"></param>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool GetNumber(List<int> list, ref int index, out int value)
        {
            return GetNumber(list, ref index, out value, false);
        }

        /// <summary>
        /// Explicitly lowered number
        /// </summary>
        /// <param name="list"></param>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool GetLoweredNumber(List<int> list, ref int index, out int value)
        {
            return GetNumber(list, ref index, out value, true);
        }



        /// <summary>
        /// Add a complex ending of the form 1.-3.
        /// </summary>
        /// <param name="rawValues"></param>
        /// <param name="allInputInterpretations"></param>
        public void AddComplexEnding(IntegerList rawValues, InputInterpretationList allInputInterpretations)
        {
            int index = 0;
            int firstNumber;
            int secondNumber;
            if (dot3456 != rawValues.List[index++]) return; //"ending"
            if (!GetLoweredNumber(rawValues.List, ref index, out firstNumber)) return; // "1"
            if ((dot3 != rawValues.List[index++])) return; //"."
            if ((dot36) != rawValues.List[index++]) return; // "-"
            if (dot3456 != rawValues.List[index++]) return; //"ending"
            if (!GetLoweredNumber(rawValues.List, ref index, out secondNumber)) return; // "3"
            if ((dot3 != rawValues.List[index++])) return; //"."
            string message = string.Format(": FirstNumber={0} Secondnumber={1}", firstNumber, secondNumber);
            Logger.LogCF(message);
            string friendlyValue = string.Format("{0}-{1}", firstNumber, secondNumber);
            IntegerList token = new IntegerList(rawValues, index); //
            List<string> parameters = new List<string>() { firstNumber.ToString(), secondNumber.ToString() };
            // The graphic symbols for first and second ending differ, but that is handled by the MusicXmlBuilder based on the parameters !
            InputSubCategoryEnum subcategory = InputSubCategoryEnum.OthervaluesVoltaIntervalEnding;
            allInputInterpretations.Add(rawValues, token, InputCategoryEnum.OtherValues, friendlyValue, subcategory, parameters);
        }



        private InputSubCategoryEnum GetSubCategory(IntegerList token, string textAsUnicode, out string friendlyValue)
        {
            InputSubCategoryEnum result = InputSubCategoryEnum.None;
            friendlyValue = "";
            // Some of the 1-byte valus are coded by dot combinations that do net represent letters and may be different depending on language.
            // Refsnæs II Black page 32
            // To prevent problems we pick these out and handle them separately. In some cases they are followed by dot3.
            if ((token.Count == 2) // Token contains exactly "toWord" and 1 value
            || (((token.Count >= 3) && (dot3 == token.List[2]))))  // Token contains exactly "toWord" and 1 value and a dot3 "Musical comma)
            {
                int c1 = token.List[1]; // The value which is candidate for one of the predefined special values below:
                bool fixedExpression = true;
                switch (c1)
                {
                    // Same as above
                    case CreschendoStart: result = InputSubCategoryEnum.TextCrescentoStart; break; // letterC
                    case DiminuendoStart: result = InputSubCategoryEnum.TextDiminiuendoStart; break; // letterD
                    case CrescendoEnd: result = InputSubCategoryEnum.TextCrescentoEnd; break;// letterC lowered Might be ":" depending on language !
                    case DiminuendoEnd: result = InputSubCategoryEnum.TextDiminiuendoEnd; break; // letterD lowered Might be "." depending on language !
                    default: fixedExpression = false; break;
                }
                if ((fixedExpression) && (token.Count > 3))
                {
                    // This is a fixed expressinon for Crescendo/Diminuendo, start/end followed by a dot3 and should NOT be taken as a  freestyle embedded text!
                    Logger.LogCF(string.Format(": Changing length of token from {0} to 3 to prevent misinterpretation of fixed term '{1}'", token.Count, friendlyValue));
                    token.List.RemoveRange(3, token.Count - 3); // Remove all remaining characters after toWord,x,dot3 
                }
            }

            if (InputSubCategoryEnum.None == result)
            {
                // This was not one of the special combinations. Look for the text-based values and user defined strings such as "Holdback" or "move forward"
                switch (textAsUnicode.ToUpper())
                {
#warning todo Find out what the ".?" mean !!
                    // Dynamics:
                    case "P":
                    case "P.":
                    case "P.?": result = InputSubCategoryEnum.TextPiano; break;
                    case "PP":
                    case "PP.":
                    case "PP.?": result = InputSubCategoryEnum.TextPianoPianissimo; break;
                    case "MP":
                    case "MP.":
                    case "MP.?": result = InputSubCategoryEnum.TextMezzoPiano; break;
                    case "F":
                    case "F.":
                    case "F.?": result = InputSubCategoryEnum.TextForte; break;
                    case "FF":
                    case "FF.":
                    case "FF.?": result = InputSubCategoryEnum.TextForteFortissimo; break;
                    case "MF":
                    case "MF.":
                    case "MF.?": result = InputSubCategoryEnum.TextMezzoForte; break;
                    //
                    //case "C": inputSubCategoryEnum = InputSubCategoryEnum.TextCrescentoStart; friendlyValue = "CrescendoStart"; break;
                    //case "D": inputSubCategoryEnum = InputSubCategoryEnum.TextDiminiuendoStart; friendlyValue = "DiminuendoStart"; break;
                    //case ".": inputSubCategoryEnum = InputSubCategoryEnum.TextCrescentoEnd; friendlyValue = "CrescendoEnd"; break;
                    //case ":": inputSubCategoryEnum = InputSubCategoryEnum.TextDiminiuendoEnd; friendlyValue = "DiminuendoEnd"; break;
                    // If we do not recognize the text we just pass it along. It may contain any directive (except the ones recognized above)  in any language 
                    default: result = InputSubCategoryEnum.TextFreeText; friendlyValue = textAsUnicode; break;
                }
            }
            return result;
        }


        public void AddFullMeasureRepeatTwice(IntegerList rawValues, InputInterpretationList allInputInterpretations, List<int> prolog, List<int> epilog)
        {
            if (!rawValues.Contains(prolog, 0)) return;
            int i;
            List<int> list = rawValues.List; // Shorthand
            for (i = prolog.Count; ((i < list.Count - epilog.Count) && (list[i] == dot3)); i++) { }; // Skip any number of dot3
            if (!rawValues.Contains(epilog, i)) return;
            // The token consists of the first N values from rawValues where N is the lengths of prolog + the length of the dot3-sequence + the length of the  epilog.
            int nDots = i - prolog.Count;
            string dotString = string.Format("{0} DOTS", nDots);
            IntegerList token = new IntegerList(rawValues, i + epilog.Count); // Only used for debugging purposes
            allInputInterpretations.Add(rawValues, token, InputCategoryEnum.OtherValues, dotString, InputSubCategoryEnum.OthervaluesFullMeasureRepeatTwice); // BANA 2015: 18.2. Full-Measure Repeats
        }


        /// <summary>
        /// Report a sequence of N NoDots as a simgle item instead of N items. (For performance and ease of debugging)
        /// </summary>
        /// <param name="rawValues"></param>
        /// <param name="allInputInterpretations"></param>
        public void AddNoDotsSequence(IntegerList rawValues, InputInterpretationList allInputInterpretations)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < rawValues.Count; i++)
            {
                if (0 != rawValues.List[i]) break;
                sb.Append(" ");
            }
            int length = sb.Length;
            if (sb.Length < 2) return;
            IntegerList token = new IntegerList(rawValues, length); //
            allInputInterpretations.Add(rawValues, token, InputCategoryEnum.Character, sb.ToString(), InputSubCategoryEnum.CharacterBlankSequence);
        }





        private const int hack = dot126; // Prevents the embedded text from eating the first part of a 126,345 sequence (Meaning start of FullMeasureInAccord 
        private const int hack1 = dot26; // Prevents the embedded text from eating a 26 sequence meaning Danish "Forslagsnode"
        private const int hack2 = dot236;  // Prevents the embedded text from eating a 236 sequence meaning staccato
        private const int hack3 = dot23;  // Prevents the embedded text from eating a 23 sequence meaning triplet, whish is sometimes found in front of the octace mark.


        const int CreschendoStart = dot14; // letterC
        const int CrescendoEnd = dot25;  // letterC lowered. Might be ":" depending on language !
        const int DiminuendoStart = dot145; // letterD
        const int DiminuendoEnd = dot256; // letterD lowered



        // The set of symbols that can be used for terminating an embedded text are specified as "ToWord" or an octave sign.
        // But in the real world a carriage return is used as well:
#warning: TODO Find a better way to prevent the embedded text from eating a FullMeasureInAccord sequence !!!!
        private static readonly List<int> textTerminatorsNoCR = new List<int>() { toWord, octave1, octave2, octave3, octave4, octave5, octave6, hack, hack1, hack2, hack3 }; // Removed octave7: conflicts with "'"
        private static readonly List<int> textTerminatorsCR = new List<int>() { toWord, octave1, octave2, octave3, octave4, octave5, octave6, carriageReturn, hack, hack1, hack2, hack3 }; // Removed octave7: conflicts with "'"
  
        /// <summary>
        /// Extracts embedded texts, which may contain predefined Braillesequences such as "f" for "forte" or free text sequences such as "with emphasis"
        /// These texts may contain CR LF and may (or may not) start with the "Musical Hyphen" showing that the sequence continues across a cr lf.
        /// </summary>
        /// <param name="rawValues"></param>
        /// <param name="allInputInterpretations"></param>
        public void AddEmbeddedText(IntegerList rawValues, InputInterpretationList allInputInterpretations, TextBrailleToTextConverter textBrailleToTextConverter)
        {
            List<int> textTerminators = null;
            int firstIndex = 0;
            int i0 = rawValues.List[0];
            if (i0 == toWord)
            {
                textTerminators = textTerminatorsCR; // CR will terminate the text
                firstIndex = 1; // The embedded text contents starts  at index 1 (after toWord)
            }
            if ((i0 == musicalHyphen) && (rawValues.List.Count >= 2) && (rawValues.List[1] == noDots) && (rawValues.List[2] == toWord))
            {
                // This is an embedded textsequence following a "musical hyphen" and an "empty space".
                // This means that a CR LF (needed because of the limitid width of a physical embosser) sequence should be ignored!
                textTerminators = textTerminatorsNoCR; // CR will NOT terminate the text
                firstIndex = 3; // The embedded text starts at index 2  (after toWord)
            }
            if (0 == firstIndex)
            {
                return; // This is not an embedded textsequence
            }

            // This is Text Braille embedded in Music Braille. Find the terminator.
            int length = rawValues.Count;
            int terminatorPosition = 0;
            List<int> textAsBraille = new List<int>(); // The embedded text in Braille representation, using special values for CR and LF
#warning todo Find better way to prevent crash here than the hack in the next 2 lines
            //            for (int i = firstIndex; i < length; i++)  // Skip the initial ToWord
            for (int i = firstIndex; i < length - 1; i++)  // Skip the initial ToWord
            {
                int value = rawValues.List[i];
                if (textTerminators.Contains(value))
                {
                    terminatorPosition = i;
                    break;
                }
                else
                {
                    textAsBraille.Add(value);
                }
            }

            if (0 == terminatorPosition)
            {
                Logger.LogCF(string.Format(": No terminating character found. Using full length of rawVlues={0}", rawValues.List.Count));
            }

            int extratextLength = 0;
            if (textAsBraille.Contains(0))
            {
                // BANA 2015: 22.3.8. Expressions That Contain Spaces
                Logger.LogCF("");
                int terminator0 = rawValues.List[terminatorPosition]; // The terminating Braille6 symbol
                int terminator1 = rawValues.List[terminatorPosition + 1]; // The symbol after the terminating Braille6 symbol 
                if ((toWord == terminator0) && (noDots == terminator1))
                {
                    Logger.LogCF("Embedded expression contains spaces and is terminated with (toWord,nodots) BANA 2015: 22.3.8");
                    extratextLength = 2; // Eat the 2 extra characters without reporting them as a part of the result"
                }
            }


            int textLength = textAsBraille.Count;
            IntegerList token = new IntegerList(rawValues, firstIndex + textLength + extratextLength); // The token holding this embedded text

            string textAsUnicode = textBrailleToTextConverter.Convert(textAsBraille);

            // The text has been isolated. Look for some specific values, such as "p" for piano
            String friendlyValue = "";
            InputSubCategoryEnum inputSubCategoryEnum = GetSubCategory(token, textAsUnicode, out friendlyValue);

            allInputInterpretations.Add(rawValues, token, InputCategoryEnum.EmbeddedTextRepresentation, friendlyValue, inputSubCategoryEnum);

        }



        public void AddKeySignature(IntegerList rawValues, InputInterpretationList allInputInterpretations, InputCategoryEnum category)
        {
            const int keySignatureFlat = dot126;
            const int keySignatureSharp = dot146;
            const int keySignatureNatural = dot16;
            // Assume that all Key signatures ("Danish: "faste fortegn") are followed by a space and all accidentals ( Danisd: "Løse fortegn") are followed immediately by the Note.

            // Simple local shorthands:
            const InputSubCategoryEnum DK = InputSubCategoryEnum.KeySignatureDK;
            const InputSubCategoryEnum BANA = InputSubCategoryEnum.KeySignatureBANA;

            allInputInterpretations.Add(rawValues, new IntegerList(keySignatureFlat, 0), category, "-1", InputSubCategoryEnum.None); // 
            allInputInterpretations.Add(rawValues, new IntegerList(keySignatureFlat, keySignatureFlat, 0), category, "-2", InputSubCategoryEnum.None); // 
            allInputInterpretations.Add(rawValues, new IntegerList(keySignatureFlat, keySignatureFlat, keySignatureFlat, 0), category, "-3", InputSubCategoryEnum.None); // 
            allInputInterpretations.Add(rawValues, new IntegerList(digit4Lowered, keySignatureFlat, 0), category, "-4", DK); // Danish, Refsnæs
            allInputInterpretations.Add(rawValues, new IntegerList(digit5Lowered, keySignatureFlat, 0), category, "-5", DK); // Danish, Refsnæs
            allInputInterpretations.Add(rawValues, new IntegerList(digit6Lowered, keySignatureFlat, 0), category, "-6", DK); // Danish, Refsnæs
            allInputInterpretations.Add(rawValues, new IntegerList(toDigit, digit4, keySignatureFlat), category, "-4", BANA); // BANA 2015 Table 6. Accidentals and Key Signatures (Pars. 6.1–6.5.1)
            allInputInterpretations.Add(rawValues, new IntegerList(toDigit, digit5, keySignatureFlat), category, "-5", BANA); // BANA 2015 Table 6. Accidentals and Key Signatures (Pars. 6.1–6.5.1)
            allInputInterpretations.Add(rawValues, new IntegerList(toDigit, digit6, keySignatureFlat), category, "-6", BANA); // BANA 2015 Table 6. Accidentals and Key Signatures (Pars. 6.1–6.5.1)

            allInputInterpretations.Add(rawValues, new IntegerList(keySignatureSharp, 0), category, "+1", InputSubCategoryEnum.None); // 
            allInputInterpretations.Add(rawValues, new IntegerList(keySignatureSharp, keySignatureSharp, 0), category, "+2", InputSubCategoryEnum.None); // 
            allInputInterpretations.Add(rawValues, new IntegerList(keySignatureSharp, keySignatureSharp, keySignatureSharp, 0), category, "+3", InputSubCategoryEnum.None);
            allInputInterpretations.Add(rawValues, new IntegerList(digit4Lowered, keySignatureSharp, 0), category, "+4", DK); // Danish, Refsnæs
            allInputInterpretations.Add(rawValues, new IntegerList(digit5Lowered, keySignatureSharp, 0), category, "+5", DK); // Danish, Refsnæs
            allInputInterpretations.Add(rawValues, new IntegerList(digit6Lowered, keySignatureSharp, 0), category, "+6", DK); //  Danish, Refsnæs
            allInputInterpretations.Add(rawValues, new IntegerList(toDigit, digit4, keySignatureSharp), category, "+4", BANA); // BANA 2015 Table 6. Accidentals and Key Signatures (Pars. 6.1–6.5.1)
            allInputInterpretations.Add(rawValues, new IntegerList(toDigit, digit5, keySignatureSharp), category, "+5", BANA); // BANA 2015 Table 6. Accidentals and Key Signatures (Pars. 6.1–6.5.1)
            allInputInterpretations.Add(rawValues, new IntegerList(toDigit, digit6, keySignatureSharp), category, "+6", BANA); // BANA 2015 Table 6. Accidentals and Key Signatures (Pars. 6.1–6.5.1)

#if true
            // Added 2022.01.07 to avoid "Empty measure ignored" by eating any NoDots in front of a keysignature.
            // Repeating all lines above, but inserting the "noDots" symbol first:

            allInputInterpretations.Add(rawValues, new IntegerList(noDots,keySignatureFlat, 0), category, "-1", InputSubCategoryEnum.None); // 
            allInputInterpretations.Add(rawValues, new IntegerList(noDots, keySignatureFlat, keySignatureFlat, 0), category, "-2", InputSubCategoryEnum.None); // 
            allInputInterpretations.Add(rawValues, new IntegerList(noDots, keySignatureFlat, keySignatureFlat, keySignatureFlat, 0), category, "-3", InputSubCategoryEnum.None); // 
            allInputInterpretations.Add(rawValues, new IntegerList(noDots, digit4Lowered, keySignatureFlat, 0), category, "-4", DK); // Danish, Refsnæs
            allInputInterpretations.Add(rawValues, new IntegerList(noDots, digit5Lowered, keySignatureFlat, 0), category, "-5", DK); // Danish, Refsnæs
            allInputInterpretations.Add(rawValues, new IntegerList(noDots, digit6Lowered, keySignatureFlat, 0), category, "-6", DK); // Danish, Refsnæs
            allInputInterpretations.Add(rawValues, new IntegerList(noDots, toDigit, digit4, keySignatureFlat), category, "-4", BANA); // BANA 2015 Table 6. Accidentals and Key Signatures (Pars. 6.1–6.5.1)
            allInputInterpretations.Add(rawValues, new IntegerList(noDots, toDigit, digit5, keySignatureFlat), category, "-5", BANA); // BANA 2015 Table 6. Accidentals and Key Signatures (Pars. 6.1–6.5.1)
            allInputInterpretations.Add(rawValues, new IntegerList(noDots, toDigit, digit6, keySignatureFlat), category, "-6", BANA); // BANA 2015 Table 6. Accidentals and Key Signatures (Pars. 6.1–6.5.1)

            allInputInterpretations.Add(rawValues, new IntegerList(noDots, keySignatureSharp, 0), category, "+1", InputSubCategoryEnum.None); // 
            allInputInterpretations.Add(rawValues, new IntegerList(noDots, keySignatureSharp, keySignatureSharp, 0), category, "+2", InputSubCategoryEnum.None); // 
            allInputInterpretations.Add(rawValues, new IntegerList(noDots, keySignatureSharp, keySignatureSharp, keySignatureSharp, 0), category, "+3", InputSubCategoryEnum.None);
            allInputInterpretations.Add(rawValues, new IntegerList(noDots, digit4Lowered, keySignatureSharp, 0), category, "+4", DK); // Danish, Refsnæs
            allInputInterpretations.Add(rawValues, new IntegerList(noDots, digit5Lowered, keySignatureSharp, 0), category, "+5", DK); // Danish, Refsnæs
            allInputInterpretations.Add(rawValues, new IntegerList(noDots, digit6Lowered, keySignatureSharp, 0), category, "+6", DK); //  Danish, Refsnæs
            allInputInterpretations.Add(rawValues, new IntegerList(noDots, toDigit, digit4, keySignatureSharp), category, "+4", BANA); // BANA 2015 Table 6. Accidentals and Key Signatures (Pars. 6.1–6.5.1)
            allInputInterpretations.Add(rawValues, new IntegerList(noDots, toDigit, digit5, keySignatureSharp), category, "+5", BANA); // BANA 2015 Table 6. Accidentals and Key Signatures (Pars. 6.1–6.5.1)
            allInputInterpretations.Add(rawValues, new IntegerList(noDots, toDigit, digit6, keySignatureSharp), category, "+6", BANA); // BANA 2015 Table 6. Accidentals and Key Signatures (Pars. 6.1–6.5.1)
#endif


#if true
            InputSubSubCategoryEnum cancel = InputSubSubCategoryEnum.KeySignatureCancel; // Explicitly shows that this input is for cancelling, not adding sharp/flat. (Inplicitly shown by the missing "-"/"+" in FriendlyValue
            allInputInterpretations.Add(rawValues, new IntegerList(keySignatureNatural, 0), category, "1", InputSubCategoryEnum.None, cancel); // 
            allInputInterpretations.Add(rawValues, new IntegerList(keySignatureNatural, keySignatureNatural, 0), category, "2", InputSubCategoryEnum.None, cancel); // 
            allInputInterpretations.Add(rawValues, new IntegerList(keySignatureNatural, keySignatureNatural, keySignatureNatural, 0), category, "3", InputSubCategoryEnum.None, cancel);
            allInputInterpretations.Add(rawValues, new IntegerList(digit4Lowered, keySignatureNatural, 0), category, "4", DK, cancel); // Danish, Refsnæs
            allInputInterpretations.Add(rawValues, new IntegerList(digit5Lowered, keySignatureNatural, 0), category, "5", DK, cancel); // Danish, Refsnæs
            allInputInterpretations.Add(rawValues, new IntegerList(digit6Lowered, keySignatureNatural, 0), category, "6", DK, cancel); //  Danish, Refsnæs
            allInputInterpretations.Add(rawValues, new IntegerList(toDigit, digit4, keySignatureNatural), category, "4", BANA, cancel); // BANA 2015 Table 6. Accidentals and Key Signatures (Pars. 6.1–6.5.1)
            allInputInterpretations.Add(rawValues, new IntegerList(toDigit, digit5, keySignatureNatural), category, "5", BANA, cancel); // BANA 2015 Table 6. Accidentals and Key Signatures (Pars. 6.1–6.5.1)
            allInputInterpretations.Add(rawValues, new IntegerList(toDigit, digit6, keySignatureNatural), category, "6", BANA,cancel); // BANA 2015 Table 6. Accidentals and Key Signatures (Pars. 6.1–6.5.1)
#endif
        }


        /// <summary>
        /// Skips the next member of the list if it has the value specified
        /// </summary>
        /// <param name="list"></param>
        /// <param name="index"></param>
        /// <param name="value"></param>
        private void Skip(List<int> list, ref int index, int value)
        {
            if (list.Count <= index) return; // At end of list
            if (value != list[index]) return; // Not the value specified
            index++; // The list contains the value specified, skip it.
        }


        readonly IntegerList sectionHeaderStart5 = new IntegerList(new List<int>() { noDots, noDots, noDots, noDots, toDigit }); // 4 empty spaces and toDigit.  Refsnæs 1 Chapter 7.3 "Opstilling"
        readonly IntegerList sectionHeaderStart1 = new IntegerList(new List<int>() { toDigit }); //  ToDigit.  Special case,  after a line continuation.


        /// <summary>
        /// Refsnæs 1 Chapter 7.3 "Opstilling"
        /// Basicly looks for a sequence of the form "SectionHeader SectionNumber FirstMeasure-LastMeasure" where
        /// SectionHeader is dot3456  (also known as the "digit" symbol)
        /// SectionNumber, FirstMeasure and LastMeasure are sequences of lowered digits.
        /// FirstMeasure and LastMeasure may or may not be terminated by Dot3
        /// </summary>
        /// <param name="rawValues"></param>
        /// <param name="allInputInterpretations"></param>
        public void AddSectionHeader(IntegerList rawValues, InputInterpretationList allInputInterpretations)
        {
            int index = -1;
            if (rawValues.StartsWith(sectionHeaderStart5)) index = sectionHeaderStart5.Count; // Starts in space 5
            if (rawValues.StartsWith(sectionHeaderStart1)) index = sectionHeaderStart1.Count; // Starts in space 5, but 4 spaces were eaten by the line continuation mechanism !
            if (-1 == index) return;
            Logger.LogCF(string.Format(": Found SectionHeaderStart"));
            int sectionNumber = 0;
            int firstMeasure = 0;
            int lastMeasure = 0;
            if (!GetLoweredNumber(rawValues.List, ref index, out sectionNumber)) return;
            if (0 != rawValues.List[index++]) return;
            if (!GetLoweredNumber(rawValues.List, ref index, out firstMeasure)) return;
            Skip(rawValues.List, ref index, dot3); // A terminating dot3 is allowed and must be added to the token
            if ((dot36) != rawValues.List[index++]) return; // "-"
            if (!GetLoweredNumber(rawValues.List, ref index, out lastMeasure)) return;
            Skip(rawValues.List, ref index, dot3); // A terminating dot3 is allowed and must be added to the token
            IntegerList token = new IntegerList(rawValues, index); //
            string friendlyValue = string.Format(" {0} {1}-{2}", sectionNumber, firstMeasure, lastMeasure);
            List<string> values = new List<string>() { sectionNumber.ToString(), firstMeasure.ToString(), lastMeasure.ToString() };
            allInputInterpretations.Add(rawValues, token, InputCategoryEnum.SectionHeader, friendlyValue, values);
        }


        public void AddPrintPagination(IntegerList rawValues, InputInterpretationList allInputInterpretations)
        {
            // BANA 2015: 1.5. Pagination of Music Pages
            int i0 = rawValues.List[0];
            if ((i0 != noDots) && (i0 != dot5)) return;
            // This might be a printpagination. skip empty spaces 
            int length = rawValues.Count;
            int firstNonEmpty = 0;
            for (int i = 1; i < length; i++)  // Skip the initial noDots
            {
                int value = rawValues.List[i];
                if (noDots != value)
                {
                    firstNonEmpty = i;
                    break;
                }
            }
            if (firstNonEmpty > 10)
            {
                Logger.LogCF(": >10");
            }

            // FirstNonEmpty now contains the index of the first nonEmpty Braille symbol
            if (rawValues.Count < firstNonEmpty + 4) return; // We need a sequence of at least 4Braille symbols
            InputSubCategoryEnum style = InputSubCategoryEnum.None;
            int nextIndex = 0;
            if ((rawValues.List[firstNonEmpty] == dot5) && (rawValues.List[firstNonEmpty + 1] == dot25))
            {
                // The BANA page-pagination symbol is DOT5 DOT25
                style = InputSubCategoryEnum.Bana2015Style;
                nextIndex = firstNonEmpty + 2;
            }
            if (rawValues.List[firstNonEmpty] == dot1234)
            {
                // The NOTA page-pagination symbol is DOT1234 (The lettet 'p'))
                style = InputSubCategoryEnum.NotaStyle;
                nextIndex = firstNonEmpty + 1;
            }
            if (style == InputSubCategoryEnum.None) return;
            if (rawValues.List[nextIndex] != dot3456) return; // ToDigit
            int digit = rawValues.List[nextIndex + 1];
            // Digit now contains the page number
            int index = nextIndex + 1;
            int pageNumber = 0;
            if (!GetNumber(rawValues.List, ref index, out pageNumber)) return; // Normal digits, not lowered
            IntegerList token = new IntegerList(rawValues, index);
            allInputInterpretations.Add(rawValues, token, InputCategoryEnum.PrintPagination, pageNumber.ToString());
        }




        /// <summary>
        /// Simple implementation, handling number up to 9
        /// </summary>
        /// <param name="rawValues"></param>
        /// <param name="allInputInterpretations"></param>
        public void AddRepeatSequence(IntegerList rawValues, InputInterpretationList allInputInterpretations)
        {
            int offset = 0;
            int count = 0;
            if (toDigit != rawValues.List[0]) return;
            if (!IsDigit(rawValues.List[1], out offset)) return;
            if (toDigit != rawValues.List[2]) return;
            if (!IsDigit(rawValues.List[3], out count)) return;
            if (0 != rawValues.List[4]) return;

            IntegerList token = new IntegerList(rawValues, 5); // The token consists of the first 5 values from rawValues

#warning todo Introduce a List<string> parameterlist instead of mis-using friendlyValue and SubCategoryValue
            // allInputInterpretations.Add(rawValues, token, InputCategoryEnum.RepeatSequence, offset.ToString(), InputSubCategoryEnum.None, count.ToString());
            allInputInterpretations.Add(rawValues, token, InputCategoryEnum.RepeatSequence, "", new List<string>() { offset.ToString(), count.ToString() });

        }
        
        public void AddAnyNumericEnding(IntegerList rawValues, InputInterpretationList allInputInterpretations)
        {
            int index = 0;
            int number;
            if (dot3456 != rawValues.List[index++]) return; //"ending"
            if (!GetLoweredNumber(rawValues.List, ref index, out number)) return; // For instance "4"
            if ((dot3 != rawValues.List[index++])) return; //"."        
            string message = string.Format(": number={0}", number);
            Logger.LogCF(message);
            string friendlyValue = string.Format("{0}", number);
            IntegerList token = new IntegerList(rawValues, index); //
            List<string> parameters = new List<string>() { number.ToString() };
            // The graphic symbols for first and second ending differ, but that is handled by the MusicXmlBuilder based on the parameters !
            InputSubCategoryEnum subcategory = InputSubCategoryEnum.OthervaluesVoltaNumericEnding;
            if (1 == number) return; // Avoid warning. Already reported by more specific code: OthervaluesVolta1FirstEnding       
            if (2 == number) return; // Avoid warning. Already reported by more specific code: OthervaluesVolta2SecondEnding, 
            allInputInterpretations.Add(rawValues, token, InputCategoryEnum.OtherValues, friendlyValue, subcategory, parameters);
        }

        //     ii.Add(rawValues, new IntegerList(beatType, digit3, digit4Lowered, noDots), InputCategoryEnum.Beat, "3/4", InputSubCategoryEnum.BeatFractionAndEmptySpace, "3/4");


        /// <summary>
        /// As AddNumericBeatType() below, but also accepts beattype en a text context
        /// </summary>
        /// <param name="rawValues"></param>
        /// <param name="allInputInterpretations"></param>
        public void AddNumericBeatTypeAsText(IntegerList rawValues, InputInterpretationList allInputInterpretations) // Explicit N/N sucn as 3/4
        {
            int index = 0;
            int beats;
            int beatType = dot3456;
            if (beatType != rawValues.List[index++]) return; // "BeatType"
            if (!GetNumber(rawValues.List, ref index, out beats)) return; // For instance "3" in 3/4. (Or even 22 in 22/16)
            if (!GetLoweredNumber(rawValues.List, ref index, out beatType)) return; // For instance "/4" in 3/4. (Or even 16 in 22/16)       
            string stringRepresentation = string.Format("{0}/{1}", beats, beatType);
            IntegerList token = new IntegerList(rawValues, index); //
            allInputInterpretations.Add(rawValues, token, InputCategoryEnum.BeatAsText, stringRepresentation, InputSubCategoryEnum.BeatFraction, stringRepresentation);
        }


        /// <summary>
        /// BAna 2015: Meter or time signatures (7) 7.1–7.1.5
        /// </summary>
        /// <param name="rawValues"></param>
        /// <param name="allInputInterpretations"></param>
        public void AddNumericBeatType(IntegerList rawValues, InputInterpretationList allInputInterpretations)
        {
            int index = 0;
            int beats;
            int beatType = dot3456;
            if (beatType != rawValues.List[index++]) return; // "BeatType"
            if (!GetNumber(rawValues.List, ref index, out beats)) return; // For instance "3" in 3/4. (Or even 22 in 22/16)
            if (!GetLoweredNumber(rawValues.List, ref index, out beatType)) return; // For instance "/4" in 3/4. (Or even 16 in 22/16)       
            string stringRepresentation = string.Format("{0}/{1}", beats, beatType);
            // Finally look for either a noDots (which is included in the token or a CR which is NOT included in the token
            InputSubCategoryEnum subCategory = InputSubCategoryEnum.None;
            int nextValue = rawValues.List[index];
            switch (nextValue)
            {
                case carriageReturn:  subCategory = InputSubCategoryEnum.BeatFractionAndCarriageReturn; break; // The CR is NOT a part of the token
                case noDots:  subCategory = InputSubCategoryEnum.BeatFractionAndEmptySpace; index++; break; // The noDots IS a part of the token
                default: return;
            }
            IntegerList token = new IntegerList(rawValues, index); //
            allInputInterpretations.Add(rawValues, token, InputCategoryEnum.Beat, stringRepresentation, subCategory, stringRepresentation);
        }


        public void AddGuideDots(IntegerList rawValues, InputInterpretationList allInputInterpretations)
        {
            // BANA 2015: 28.1.3. Guide Dots
            if (rawValues.List.Count < 5) return; //  // BANA requires at least 5 dots
            int i0 = rawValues.List[0];
            int i1 = rawValues.List[1];
            if ((i0 != noDots) || (i1 != dot3)) return;
            // This might be a sequence of guide dots
            int length = rawValues.Count;
            int terminatorIndex = 0;
            for (int i = 1; i < length; i++)  // Skip the initial noDots
            {
                int value = rawValues.List[i];
                if (dot3 != value)
                {
                    terminatorIndex = i;
                    break;
                }
            }
            if (terminatorIndex < 6) return; // BANA requires at least 5 dots
            int terminator = rawValues.List[terminatorIndex];
            if (terminator != noDots) return;
            IntegerList token = new IntegerList(rawValues, terminatorIndex); //  
            allInputInterpretations.Add(rawValues, token, InputCategoryEnum.GuideDots, string.Format("{0}", terminatorIndex));
        }



        // Handle the case of invalid characters in the input here, outside the normal execution flow.
        // In order to identify the error, the initial, valid part (if any) of the file is logged.
        public string LogInvalidInputCharacter(string input, UserPositionInfo userPositionInfo, ref int nErrors)
        {
            //string errorMessage = string.Format("Unicode representation contains unexpected Unicode character='{0}' (Hewvalue=0x{1:x}) at Form={2} Line={3} Space={4}", c, (int)c, nFF, nCR, nSpace);
            char c = userPositionInfo.BrailleAsUnicode;
            string logMessage = string.Format("Unicode representation contains unexpected Unicode character (Hewvalue=0x{0:x}) Will be interpreted as Braille NoDots", (int)c); // Avoid outputting the invalid char itself. It may be any control character!
            // Log a warning for the user, specifying the exact values. The normal Logger.LogUserWarning(errorMessage) takes parameters by callback to the Decoder and requires that the Decoder has been created)   
            UserWarnings.LogUserWarning(logMessage, userPositionInfo, "X", "0",UserInfoFlagsEnum.InterpretationUnExpectedInput); // Use "X" for unexpected character to avoid confusion: The Console represents all 0x2900..0x28ff as "?"
            string errorMessage = string.Format(": {0} at Form={1} Line={2} Space={3}", logMessage, userPositionInfo.Form, userPositionInfo.Line, userPositionInfo.Space); // Add extra information
            Logger.LogCF(errorMessage);
            // Log the valid start of the file in order to easier identify the error. But only do this for the first error.
            if (0 == nErrors)
            {
                string validStart = input.Substring(0, userPositionInfo.Index);
                string[] splitString = new string[] { "\r\n" };
                string[] validLines = validStart.Split(splitString, StringSplitOptions.None);
                Logger.LogCF("+");
                int lineNumber = 1; // 
                foreach (string validLine in validLines)
                {
                    Logger.Log(string.Format("{0,3}: {1}", lineNumber++, validLine));
                }
            }
            Logger.LogCF("-");
            nErrors++;
            return errorMessage;
        }

        // Simple names without sharp or flat will probably be constant across all localization. 
        private const string A = " A";
        private const string B = " B";
        private const string C = " C";
        private const string D = " D";
        private const string E = " E";
        private const string F = " F";
        private const string G = " G";

        private readonly string A_Flat = " " + ResourcesForBrailleMusicDecoder.Note_A_Flat;
        private readonly string B_Flat = " " + ResourcesForBrailleMusicDecoder.Note_B_Flat;
        private readonly string C_Flat = " " + ResourcesForBrailleMusicDecoder.Note_C_Flat;
        private readonly string D_Flat = " " + ResourcesForBrailleMusicDecoder.Note_D_Flat;
        private readonly string E_Flat = " " + ResourcesForBrailleMusicDecoder.Note_E_Flat;
        private readonly string F_Flat = " " + ResourcesForBrailleMusicDecoder.Note_F_Flat;
        private readonly string G_Flat = " " + ResourcesForBrailleMusicDecoder.Note_G_Flat;

        private readonly string A_Sharp = " " + ResourcesForBrailleMusicDecoder.Note_A_Sharp;
        private readonly string B_Sharp = " " + ResourcesForBrailleMusicDecoder.Note_B_Sharp;
        private readonly string C_Sharp = " " + ResourcesForBrailleMusicDecoder.Note_C_Sharp;
        private readonly string D_Sharp = " " + ResourcesForBrailleMusicDecoder.Note_D_Sharp;
        private readonly string E_Sharp = " " + ResourcesForBrailleMusicDecoder.Note_E_Sharp;
        private readonly string F_Sharp = " " + ResourcesForBrailleMusicDecoder.Note_F_Sharp;
        private readonly string G_Sharp = " " + ResourcesForBrailleMusicDecoder.Note_G_Sharp;


        private RegionalOptions regionalOptions;
        private TextBrailleToTextConverter textBrailleToTextConverter;

        private TokenReaderUtilities(RegionalOptions regionalOptions)
        {
            this.regionalOptions = regionalOptions;
            textBrailleToTextConverter = TextBrailleToTextConverter.Create(regionalOptions);     
        }

        public static TokenReaderUtilities Create(RegionalOptions regionalOptions)
        {
            return new TokenReaderUtilities(regionalOptions);
        }

    }
}
