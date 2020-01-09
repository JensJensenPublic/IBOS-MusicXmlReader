using System;

namespace BrailleMusicDecoder
{
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
        /// <summary>
        /// The current decoding-state.
        /// The state determines which tokens are accepted as input and how to interpret them
        /// </summary>
        public enum StateEnum
        {
            Unknown,    // The current way of decoding has not yet been established
            Text,       // Decoding as lower-case text
            TextNumber, // Decoding as digits while decoding text
            TextVersal, // Decoding as upper-case text
            Music,      // Decoding as Music, but not while decoding a note
            MusicNumber,// Decoding as digits while decoding music
            MusicNote   // Decoding as a note while decoding music 
        };

        StateEnum state = StateEnum.Unknown;

        IBrailleMusicDecoderLogger logger;  // Used for simple logging   
        string brailleAsUnicode; // The Unicode string to decode
        TokenReader tokenReader;

        public enum DecoderOptionEnum { None, MariaGennemTorneGårFromNOTA };

        //
        // Simple convenience methods
        //

        public void ResetState()
        {
            state = StateEnum.Text;
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

        /// <summary>
        /// Pass the call to the object specified during creation!
        /// </summary>
        /// <param name="s"></param>
        private void Log(string s)
        {
            logger.Log(s);
        }


        //
        // The main logic:
        //


        public string GetNextToken(ref int i)
        {
            if ((i<0) || (i >= brailleAsUnicode.Length)) return null; // Outside the array of input characters
            int thisValue = brailleAsUnicode[i];
            int nextValue = (i+1 >= brailleAsUnicode.Length) ? (tokenReader.Blank) :  brailleAsUnicode[i]; // Insert an empty Braille6 character 

            // Immediately get rid of characters outside the Unicode Braille6 interval [0x2800..0x283f] 
            if (!tokenReader.IsBraille6(thisValue))
            {
                switch (thisValue)
                {
                    // But first apply some simple state changes
                    case 10: break; // (LF)";
                    case 12: break; // (FF)";
                    case 13: if (StateEnum.TextNumber == state) //(CR)
                        {
                            StateEnum newState = StateEnum.Text;
                            Log(string.Format("Non-Braille input={0} Changing state from {1} to {2} -------------------------------------", thisValue, state, newState));
                            state = newState;
                        }
                        break;
                    default: break;
                }


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



        private InputInterpretationList ToTokenList(int startIndex)
        {

            // Ad hoc mechanism for handling wellknown errors in BrailleMusic files received from external source, for instance NOTA
            DecoderOptionEnum options = (DecoderOptionEnum)logger.GetDecoderOptions();
            switch (options)
            {
                // Here we handle known errors in the files that we decode
                case DecoderOptionEnum.MariaGennemTorneGårFromNOTA:
                    if (startIndex == 635) // 635 and 636 contain the symbol for "right hand"
                    {
                        state = StateEnum.Music;
                        Log(string.Format("StartIndex={0} Forcing State={1} ***********************************************************************************", startIndex, state));
                    }
                    if (startIndex == 2145) // Start of Tenor
                    {
                        state = StateEnum.Music;
                        Log(string.Format("StartIndex={0} Forcing State={1} ***********************************************************************************", startIndex, state));
                    }

                    if (
//                           (startIndex == 277) // Use CRLF from number
//                         (startIndex == 338) // Use CRLF from number
 //                        (startIndex == 440) // Use CRLF from number
//                        (startIndex == 484) // Use CRLF from number
//                        (startIndex == 503) // Use CRLF from number
//                         (startIndex == 537) // Use CRLF from number
                         (startIndex == 1030)
//                        || (startIndex == 1035) // Use CRLF from number
  //       || (startIndex == 1578)
  //                      || (startIndex == 1583) // Use CRLF from number
  //                      || (startIndex == 1161)
  //                      || (startIndex == 1698)
  //                      || (startIndex == 2236)
  //                      || (startIndex == 2774)
                        )
                    {
                        state = StateEnum.Text;
                        Log(string.Format("StartIndex={0} Forcing State={1} ***********************************************************************************", startIndex, state));
                    }

                    break;
                default: break;
            }

            int endIndex = Math.Min(brailleAsUnicode.Length, startIndex + 10); // Take the next 10 characters 
            IntegerList brailleCharacters = new IntegerList();
            {
                for (int i = startIndex; (i < endIndex); i++)
                {
                    brailleCharacters.Add(tokenReader.ToBraille(brailleAsUnicode[i]));
                }
            }

            InputInterpretationList inputValues = tokenReader.GetInputInterpretations(brailleCharacters); // Get a list of all possible input values independent of the current state.
            InputCategoryEnum allowedInputCategories = 0;

            // Define some shorthand values to be used in the case below
            const InputCategoryEnum AnyEnding = InputCategoryEnum.FullEnd | InputCategoryEnum.HalfEnd;
            InputCategoryEnum allowedInTextStates   = InputCategoryEnum.Character | InputCategoryEnum.TextVersal | InputCategoryEnum.ToNumber | InputCategoryEnum.ToMusicBraille;
            InputCategoryEnum allowedInMusicStates  = InputCategoryEnum.Note | InputCategoryEnum.Octave | InputCategoryEnum.Rest | InputCategoryEnum.NewMeasure | InputCategoryEnum.MeasureDivision | InputCategoryEnum.InAccordPartMeasure | InputCategoryEnum.InAccordFullMeasure | InputCategoryEnum.Clef | InputCategoryEnum.ToMusicBraille | InputCategoryEnum.ToText;
            InputCategoryEnum allowedInNumberStates = InputCategoryEnum.Digit | InputCategoryEnum.Denominator | InputCategoryEnum.Space | InputCategoryEnum.ToMusicBraille | InputCategoryEnum.ToNumber;

            switch (state)
            {
                case StateEnum.Text:        allowedInputCategories = allowedInTextStates; break;
                case StateEnum.TextVersal:  allowedInputCategories = allowedInTextStates; break;
                case StateEnum.TextNumber:  allowedInputCategories = allowedInNumberStates | InputCategoryEnum.TextVersal | InputCategoryEnum.ToMusicBraille | InputCategoryEnum.ToWord; break;
                case StateEnum.MusicNumber: allowedInputCategories = allowedInNumberStates | InputCategoryEnum.Accidental; break;
                case StateEnum.Music:       allowedInputCategories = allowedInMusicStates  | InputCategoryEnum.Accidental | InputCategoryEnum.UnusualBarLine | InputCategoryEnum.ToNumber | InputCategoryEnum.Finger | AnyEnding | InputCategoryEnum.Hand | InputCategoryEnum.Beat; break;
                case StateEnum.MusicNote:   allowedInputCategories = allowedInMusicStates  | InputCategoryEnum.Legato | InputCategoryEnum.UnusualBarLine | AnyEnding | InputCategoryEnum.EndRepeat | InputCategoryEnum.Punctuation | InputCategoryEnum.Interval | InputCategoryEnum.Accidental | InputCategoryEnum.OtherValues; break;
                default: throw new Exception(string.Format("Unsupported state {0} ", state.ToString()));
            }

            // Get all inputvalues accepted in the current state.
            InputInterpretationList filteredInputValues0 = inputValues.Filter(allowedInputCategories);   
            
            // Get the inputvalue with the largest length
            InputInterpretationList filteredInputValues = filteredInputValues0.Prioritize();

            // Log if we had to reduce the number if items
            int nFiltered = filteredInputValues0.Count;
            int nPrioritized = filteredInputValues.Count;
            if (nFiltered  != nPrioritized)
            {
                Log(string.Format(": Filtered={0}, Prioritized={1} **********************************************", nFiltered, nPrioritized));
            }

            // Calculate the new state
            StateEnum newState = state;

            switch (state)
            {
                case StateEnum.Text:
                    if (filteredInputValues.Contains(InputCategoryEnum.ToMusicBraille))
                    {
                        newState = StateEnum.Music; break;
                    }
                    if (filteredInputValues.Contains(InputCategoryEnum.ToNumber))
                    {
                        newState = StateEnum.TextNumber;
                    }
                    break;
                case StateEnum.TextVersal:
                    if (filteredInputValues.Contains(InputCategoryEnum.ToMusicBraille))
                    {
                        newState = StateEnum.Music; break;
                    }
                    if (filteredInputValues.Contains(InputCategoryEnum.ToNumber))
                    {
                        newState = StateEnum.TextNumber;
                    }
                    break;
                case StateEnum.TextNumber:
                    if (filteredInputValues.Contains(InputCategoryEnum.ToMusicBraille))
                    {
                        newState = StateEnum.Music;
                        break;
                    }
                    if (!(filteredInputValues.Contains(InputCategoryEnum.Digit) || filteredInputValues.Contains(InputCategoryEnum.Denominator) || filteredInputValues.Contains(InputCategoryEnum.ToNumber)))
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
                    if (filteredInputValues.Contains(InputCategoryEnum.ToText))
                    {
                        newState = StateEnum.Text; break;
                    }
                    break;

                case StateEnum.MusicNumber:
                    if (filteredInputValues.Contains(InputCategoryEnum.Space))
                    {
                        newState = StateEnum.Music;
                    }
                    break;

                case StateEnum.MusicNote:

                    if (filteredInputValues.Contains(InputCategoryEnum.ToText))
                    {
                        newState = StateEnum.Text; break;
                    }

                    if (filteredInputValues.Contains(InputCategoryEnum.NewMeasure))
                    {
                        newState = StateEnum.Music; 
                    }

                    if (filteredInputValues.Contains(InputCategoryEnum.HalfEnd))
                    {
                        newState = StateEnum.Music;
                    }

                    if (filteredInputValues.Contains(InputCategoryEnum.FullEnd))
                    {
                        newState = StateEnum.Music;
                    }


                    if (filteredInputValues.Contains(InputCategoryEnum.InAccordPartMeasure))
                    {
                        newState = StateEnum.MusicNote; // No change, probably not needed
                    }

                    if (filteredInputValues.Contains(InputCategoryEnum.InAccordFullMeasure))
                    {
                        newState = StateEnum.MusicNote; // No change, probably not needed
                    }
       

                    break;

            }

            string result = filteredInputValues.ToString();

            int thisValue = brailleCharacters.List[0];

            char inputAsUnicode = TokenReader.ToUnicodeChar(thisValue);
            if (1 != filteredInputValues.Count)
            {
                Log(string.Format(" State={0,-15} Input={1}(i={2,02}) OriginalInputValues = {3}", state.ToString(), inputAsUnicode, thisValue ,inputValues.ToString()));
                Log(string.Format(" State={0,-15} Input={1}(i={2,02}) FilteredInputValues = {3}", state.ToString(), inputAsUnicode, thisValue ,filteredInputValues.ToString()));
            }

            string newStateText = (state != newState) ? string.Format("NewState={0} ", newState) : "";
        
            Log(string.Format("{0,5} State={1,-15} Input={1}(i={3,02}) Result='{4}' {5} ", startIndex, state.ToString(), inputAsUnicode, thisValue, result, newStateText));

            if (newState != state)
            {
//                Logger.Log(string.Format(": >>>>>>>>>>>>>Changing state from {0} to {1}<<<<<<<<<", state, newState));
            }


            // Generate the output

            // Update the state to the new state

            state = newState;

            return filteredInputValues;

        }
  
        private string DigitToString(int i)
        {
            return "DIGIT";
        } 

        private string Format(string s)
        {
            return (string.IsNullOrEmpty(s) ? "" : " " + s);
        }

        private string Format(string prefix, string s)
        {
            return (string.IsNullOrEmpty(s) ? "" : " " + prefix + s);
        }

        //
        // Constructors
        //

        private Decoder()
        {
            state = StateEnum.Unknown;
        }

        private Decoder(StateEnum initialState, string brailleAsUnicode,IBrailleMusicDecoderLogger logger)
        {
            state = initialState;
            this.brailleAsUnicode = brailleAsUnicode;
            this.logger = logger;
            this.tokenReader = TokenReader.Create();
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
