using System;
using System.Text;
using System.Xml;
using System.Collections.Generic;
using MusicXmlReaderModel; // Use the namespace, but do not reference MusicXmlReaderModel. Reference MusicXmlReaderModelBase instead to avoid circular references

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
    public class Decoder // : IDecoderUserInfo
    {
        private bool Verbose = false;
        DecoderStateMachine decoderStateMachine;
        MusicXmlBuilder musicXmlBuilder;
        Options rawOptions;
        IDecoderClient decoderClient;
        DecoderSpacePositionHandler decoderSpacePositionHandler;
        TypeAmbiguityHandler typeAmbiguityHandler;
        BrailleSubSequenceList brailleSubSequenceList;

        public BrailleSubSequenceList BrailleSubSequenceList { get { return brailleSubSequenceList; } }
        readonly char[] removeStartingBlanks = new char[] { ' ' };

        string musicXmlGenerationError = null;
        public string MusicXmlGenerationError { get { return (null == musicXmlGenerationError) ? "" : musicXmlGenerationError; } }
        public bool MusicXmlGenerationFailed { get { return (null != musicXmlGenerationError); } }

        // Cached Localization values
        public readonly string Text_Page = ResourcesForBrailleMusicDecoder.Text_Page;
        public readonly string Text_Line = ResourcesForBrailleMusicDecoder.Text_Line;
        public readonly string Text_Space = ResourcesForBrailleMusicDecoder.Text_Space;
        public readonly string Text_Dot = ResourcesForBrailleMusicDecoder.Text_Dot;
        public readonly string Text_Warning = ResourcesForBrailleMusicDecoder.Text_Warning;

        string brailleAsUnicode; // The Unicode string to decode
        TokenReader tokenReader; // An instance of the Tokenreader class for doing the lowlevel parsing.

        public int LineNumber { get { return decoderSpacePositionHandler.LineNumber; } }
        public int FormNumber { get { return decoderSpacePositionHandler.FormNumber; } }
        public string FormLineString { get { return string.Format("{0} {1} {2,2} {3,3}",  Text_Page, FormNumber,Text_Line, LineNumber); } }
        private StringBuilder accumulatedCharacters = new StringBuilder();
        private string accumulatedStringStartIndex;
        //private DevelopmentOptionEnum developmentOptions = DevelopmentOptionEnum.None; // For changing behaviour cureong development
        private DecoderDebugTools decoderDebugTools;

        //private List<string> decoderWarnings = new List<string>(); // For internal collection of selected messages

        public string GetStateInformation()
        {
            return this.musicXmlBuilder.GetStateInformation();
        }

        public void LogStatistics()
        {
            this.decoderStateMachine.LogStatistics();
            UserWarnings.DumpLocalUserWarnings();
        }

        public void LogGlobalStatistics()
        {
            this.decoderStateMachine.LogGlobalStatistics();
            UserWarnings.DumpGlobalUserWarnings();
        }


        /// <summary>
        /// Update information possibly later used for Error reporting to user
        /// </summary>
        /// <param name="dsp"></param>
        /// <param name="initialIndex"></param>
        /// <returns></returns>
        private string OnPositionChanged(DecoderSpacePositionHandler dsp, int initialIndex)
        {
            // Save the old position in various representations, primarily for logging purposes and user warnings
            string result = decoderSpacePositionHandler.ToString();
            char brailleValue = brailleAsUnicode[initialIndex];// Just a shorthand
            UserPositionInfo userPositionInfo = UserPositionInfo.Create(initialIndex, dsp.FormNumber, dsp.LineNumber, dsp.SpaceNumber, brailleValue, ToDotNumbers(brailleValue));
            UserWarnings.OnUserPositionChanged(userPositionInfo); // Will be used for user warnings from now in
            return result;
        }
        
        /// <summary>
        ///  The main logic:
        ///  Extracts the next Token from the input string of Unicode Braille characters and updates the pusition within the string.
        /// </summary>
        /// <param name="i">The position within the input strin</param>
        /// <returns>A class representing the extracted token as clear text and (if relevant) as MusicXml</returns>
        public Token GetNextToken(ref int i)
        {
            string accumulatedText = null;
            int initialIndex = i;
            DecoderStateMachine.StateEnum initialDecoderState = decoderStateMachine.State.MyStateEnum; // Needed for editing raw Braille

            if ((i < 0) || (i >= brailleAsUnicode.Length)) return Token.Create(null,accumulatedText,initialDecoderState); // Outside the array of input characters          

            string oldPosition = OnPositionChanged(decoderSpacePositionHandler, initialIndex); // Update information possibly later used for Error reporting to user
            oldPosition = oldPosition.TrimStart(removeStartingBlanks); // Remove starting blanks

            // **********************************************************
            // Get the InputInterpretation. This is where things happen !
            // ********************************************************** 
            InputInterpretationList filteredInputs = null;  // Receives a list of ALL POSSIBLE interpretations of the next token. Can be used for debugging etc.
            InputInterpretation inputInterpretation = ToInputInterpretation(i, out filteredInputs); // Receives THE interpretation  to be used from now on

            // Update the BrailleSubSequenceList. This allows for splitting large files containing MusicBraille for several scores into the separate scores.            
            brailleSubSequenceList.OnNewInput(inputInterpretation, i);

            // Update the position
            decoderSpacePositionHandler.OnNewInput(inputInterpretation);

            // Update the position within the input Unicode string. If we can not determine an interpretation we just continue to the next input character
            int tokenLength = (null == inputInterpretation) ? 1 : inputInterpretation.TokenLength;
            i += tokenLength;

            if (null == inputInterpretation)  // If no interpretation is found return a token containg an error description.
            {
                string message = OnNoResult(initialIndex, oldPosition); // Common message for decoder textfile and selected events
                string userWarning = string.Format("Rum{0}", message);
                UserWarnings.LogUserWarning(userWarning,UserInfoFlagsEnum.InterpretationNotFound);
                return Token.Create(message, accumulatedText,initialDecoderState);
            }


            // Start experimental code for generating MusicXml "on the fly"
            // If ths code for generating MusicXml fails by throwing an exception we attempt not to influence the interpretation of MusicBraille as text
            //******************************************************************************************************************************************
            if (!MusicXmlGenerationFailed)
            {
                try
                {
                    // string temp = null; temp.ToString(); // ONLY for debbuging: Trigger an exception
                    if (decoderStateMachine.IsInAnyMusicState()                                 // The Decoder state machine is in of the 3 Musicxxx states AFTER this state transition.
                    || (inputInterpretation.Category == InputCategoryEnum.FinalDoubleBar))      // The new token is a final double bar, which must trigger a flush of he latest measure.
                    {
                        typeAmbiguityHandler.ApplyNextInput(inputInterpretation); // ** This is where wa call the TypeAmbiguityHandler which calls the MusicXmlBuilder to build MusicXml **
                    }
                    else
                    {
                        musicXmlBuilder.ApplyNextInput(inputInterpretation); // ** This is where wa call the  the MusicXmlBuilder directly to build embedded text **
                    }
                }
                catch (Exception e)
                {
                    // This will allow the decoding to continue even if the generation of MusicXml throws an exception
                    Logger.LogCFE(e); ;
                    musicXmlGenerationError = (null == e.Message) ? "" : e.Message;
                    ModelBaseMessageBox.Show("Generering af MusicXml mislykkedes!" + "\r\n" + musicXmlGenerationError + "\r\n"
                                           + "Fortolkning af punktnoder forsøges gennemført.",
                                              ModelBaseMessageBoxButtons.OK, ModelBaseMessageBoxIcon.Exclamation);
#warning TODO Localize
                }
            }
            // End experimental code
            //*********************************************************************************************************************************************

            //Accumulate all sequences of simple input characters and save the startindex
            Accumulate(inputInterpretation, oldPosition, ref accumulatedText);

            decoderDebugTools.CountCategories(inputInterpretation); // NOTE: Time consuming !!!

            if (0 == (rawOptions.VisibleCategoryies & inputInterpretation.Category))
            {
                // A token of this category (for instance InputCategoryEnum.Character) is not visible, but if an accumulated text exists it must be shown anyway! 
                return  Token.Create("",accumulatedText,initialDecoderState);
            }
            else
            {
                // Even if a MusicXml file can not be generated result contains the decoded information
//                string s = string.Format("{0} {1}", oldPosition, result.ToString(rawOptions));                         // CHECK !!
                string format = (string.IsNullOrWhiteSpace(oldPosition)) ? "{1}" : "{0} {1}";
                string s = string.Format(format, oldPosition, inputInterpretation.ToString(rawOptions));                         // CHECK !!
                return Token.Create(s,accumulatedText, inputInterpretation,initialIndex,initialDecoderState); // Show the token and accumulated text.
            }            
        }


        // Simple mechansim only used for debugging
        public string GetNextRawUnicodeLine(int startIndex)
        {
            int endIndex = brailleAsUnicode.IndexOf('\r',startIndex); // Find index of first CR
            if (-1 == endIndex)
            {
                endIndex = brailleAsUnicode.Length; // Last line. Return the rest of the file
            }
            string result =  brailleAsUnicode.Substring(startIndex, endIndex - startIndex);
            return result;
        }

        public void LogCategories()
        {
            bool sort = true;       
            decoderDebugTools.DumpCategories(sort);
            decoderDebugTools.DumpSubCategories(sort);
            decoderDebugTools.DumpSubSubCategories(sort);
        }

        //*****************************************************************************************
        // Simple convenience methods
        //*****************************************************************************************

        private void Accumulate(InputInterpretation inputInterpretation, string oldPosition, ref string accumulatedText)
        {
            string inputString = null;
            if ((inputInterpretation.Category == InputCategoryEnum.Character) || (inputInterpretation.Category == InputCategoryEnum.Digit))
            {
                if (0 == accumulatedCharacters.Length)
                {
                    accumulatedStringStartIndex = oldPosition;
                }
                string s = ToVersal(decoderStateMachine.ShowAsVersal, inputInterpretation.FriendlyValue);
                accumulatedCharacters.Append(s);
                //return "";  // Hides the line containing the InputCategoryEnum.Character
            }
            else
            {
                if (0 != accumulatedCharacters.Length)
                {
                    inputString = accumulatedCharacters.ToString();
                    string caption = ResourcesForBrailleMusicDecoder.Decoder_AccumulatedText;
                    accumulatedText = string.Format("{0} {1} {2}", accumulatedStringStartIndex, caption, inputString);
                    accumulatedCharacters.Clear();
                }
            }
        }

        /// <summary>
        /// Exclusively for use by analysis of Localization-resources
        /// </summary>
        /// <returns></returns>
        public static object CreateResourcesForBrailleMusicDecoder()
        {
            return new ResourcesForBrailleMusicDecoder();
        }

        private string NonBrailleInterpretation(int c)
        {
            musicXmlBuilder.ApplyNextInput(InputCategoryEnum.NonBrailleCharacter, c, InputSubCategoryEnum.None, null);
            switch (c)
            {
                case 10: return "10 (LF)";
                case 12: return "12 (FF)";
                case 13: return "13 (CR)";
                default: return string.Format("Unexpected character = 0x{0:X04}", c);
            }
        }


        /// <summary>
        /// Simple mechanism used only during debugging for visualizing the tokens extracted.
        /// Returns a formattet string representing the raw values within the halfopen interval [start, end[
        /// </summary>
        /// <param name="startIndex">The index of the first char to include</param>
        /// <param name="endIndex">The index of th first char NOT to include</param>
        /// <returns></returns>
        public string GetRawValues(int startIndex, int endIndex)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = startIndex; i < endIndex; i++)
            {
                char c = brailleAsUnicode[i];
                if ((0x2800 <= c) && (c <= 0x283f))
                {
                    sb.Append(ToDotNumbers(c - 0x2800) + " ");
                }
                else
                {
                    switch (c)
                    {
                        case '\r': sb.Append("CR "); break;
                        case '\n': sb.Append("LF "); break;
                        case '\f': sb.Append("FF "); break;
                        default:   sb.Append("??"); break; // Unknown Unicode char !!
                            //string message = string.Format("Unexpected Unicode value {0} found in inputstring. Hex value={1:0x}", c, c);
                            //throw new Exception(message);
                            // No "break" needed after Exception                        
                    }
                }
            }
            string result = sb.ToString();
            return result;
        }

        private string ToString(char c)
        {
            switch (c)
            {
                case '\r': return "<CR>";
                case '\n': return "<LF>"; 
                default: return c.ToString();
            }
        }


        /// <summary>
        /// Generate a string for reporting that no result was found
        /// </summary>
        /// <param name="initialIndex"></param>
        /// <param name="oldPosition"></param>
        /// <returns></returns>
        private string OnNoResult(int initialIndex, string oldPosition)
        {
            // Use the initial index to access the arrays:
            char charValue = brailleAsUnicode[initialIndex]; // Will be shown as a Braille pattern
            string charValueAsString = ToString(charValue);
            int hexValue = tokenReader.GetBrailleFileAsInteger(initialIndex);
            string dots = "?";
            string textDot = Text_Dot;
            if (hexValue < 64)
            {
                dots = ToDotNumbers(hexValue, ""); // "123456" instead of "1 2 3 4 5 6"
            }
            else
            {
                // This was not a Braille6 character !
                dots = "";
                textDot = "";
            }
            string textNoInterpretationFor = ResourcesForBrailleMusicDecoder.Text_noInterpretationFor; // Localize !
         
                                                                       // Index is the position within the file
                                                                       // OldPosition the position within the current line 
            string s = string.Format("{0:02} {1} {2} {3}{4}", oldPosition, textNoInterpretationFor, charValueAsString, textDot, dots); // For instance "03 No Interpretation for . DOT 3"
            return s;
        }


        private string ToVersal(bool versal, string s)
        {
            if (!versal) return s;
            if (1 == s.Length) return s.ToUpper();
            // This is contraction, so we must only convert the first letter to uppercase !
            string s0 = s.Substring(0,1);
            string theRest = s.Substring(1, s.Length - 1);
            return s0.ToUpper() + theRest;
        }


        /// <summary>
        /// Returns the standard Braille notation for the binary inputvalue
        /// For instance: 
        /// 0 ->  "0"
        /// 1 ->  "1"
        /// 2 ->  "2"
        /// 3 ->  "12"
        /// 63 -> "123456"
        /// </summary>
        /// <param name="binaryInputValue"></param>
        /// <returns></returns>
        private static string ToDotNumbers(int binaryInputValue,string delimiter)
        {
            StringBuilder sb = new StringBuilder();                  
            for (int i = 0; (i <= 5); i++)
            {
                int mask = 1 << i;
                int maskedInput = binaryInputValue & mask;
                if (0 != maskedInput)
                {
                    sb.Append(i + 1);
                    sb.Append(delimiter);
                } 
            }
            string result = (0 == sb.Length) ? "0" : sb.ToString(); // Return "0" instead of the empty string
            // Log(string.Format("ToBraille({0})={1}", binaryInputValue, result));
            return result;
        }

        private static string ToDotNumbers(int binaryInputValue)
        {
            return ToDotNumbers(binaryInputValue,""); // Default: Use no delimiter: "123456" instead of "1 2 3 4 5 6"
        }

        /// <summary>
        /// Converts from Braile Unicode (0x2800-0x28ff) to Dotnumbers such as 1 12 14 to represent the 1 2 3
        /// </summary>
        /// <param name="unicodeBraille"></param>
        /// <returns></returns>
        public static string ToDotNumbers(string unicodeBraille)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < unicodeBraille.Length; i++)
            {
                char c = (char)unicodeBraille[i];
                string dotNumbers = ToDotNumbers(c - 0x2800);
                sb.Append(dotNumbers + " ");
            }
            return sb.ToString();
        }


        /// <summary>
        /// Attempts to extract and return a unique interpretation of MusicBraille sequence starting a startIndex in the current state.
        /// </summary>
        /// <param name="startIndex">The position to start at</param>
        /// <param name="prioritizedInputValues">All possible inputinterpretations (For debugging purposes)</param>
        /// <returns>
        /// If no interpretation exists, null is returned.
        /// If exactly one interpretation this interpretation is returned. This is the normal, desired case! 
        /// If several interpretations exist, the interpretatation consuming the largest number of Music Braille symbols is returned.
        /// If several interpretation share the same max length the one at index 0 is (arbitrarily) returned.
        /// </returns>
        private InputInterpretation ToInputInterpretation(int startIndex, out InputInterpretationList prioritizedInputValues)
        {
            int breakIndex = int.MaxValue; // No break

            prioritizedInputValues = null;


            // First apply ad hoc mechanism for handling wellknown errors in BrailleMusic files received from external source, for instance NOTA
            //DecoderOptionEnum options = (DecoderOptionEnum)logger.GetDecoderOptions();

            DecoderStateMachine.StateEnum forcedNewState = decoderStateMachine.State.MyStateEnum;

            switch (decoderDebugTools.DevelopmentOptions)
            {
                // Here we handle known errors in the files that we decode             
                case DevelopmentOptionEnum.MariaGennemTorneGårFromNOTA:
                    {
                        switch (startIndex)
                        {
                            case 635:
                            case 2139: forcedNewState = DecoderStateMachine.StateEnum.Music; break;
                            case 1020: forcedNewState = DecoderStateMachine.StateEnum.Text; break;
                            default: break;
                        }
                    }
                    break;
                case DevelopmentOptionEnum.NuErJordOgHimmelStille:  break;  // Not needed !
                case DevelopmentOptionEnum.Ulandsvise: break; // Not needed !
                case DevelopmentOptionEnum.DenneMorgensMulighed: // Not needed !
                default: break;
            }
            if (forcedNewState != decoderStateMachine.State.MyStateEnum)
            {      
                // We need to force the statemachine into a new state.
                decoderStateMachine.SetState(forcedNewState,startIndex);
            }


            // Now for the "real" algorithm:
            int count =  50; // Take the next ut to 50 characters . Maybe not always enough!
            IntegerList brailleIntegers = tokenReader.GetBrailleFileAsIntegers(startIndex, count);   // The next value to interpret
 
            if (startIndex == breakIndex)
            {
                Logger.LogCF(string.Format(": DebugBreak at startIndex={0}",breakIndex)); // For setting conditional breakpoint during debugging
            }

            InputInterpretationList originalInputValues = tokenReader.GetInputInterpretations(brailleIntegers); // Get a list of all possible input values independent of the current state.
            InputCategoryEnum allowedInputCategories = decoderStateMachine.AllowedInputCategories; ;

            // Get all inputvalues accepted in the current state.
            InputInterpretationList filteredInputValues = originalInputValues.Filter(allowedInputCategories);   
            
            // Get the inputvalue with the largest length
            prioritizedInputValues = filteredInputValues.Prioritize();

            // Log if we had to reduce the number if items
            int nFiltered = filteredInputValues.Count;
            int nPrioritized = prioritizedInputValues.Count;
            if (nFiltered  != nPrioritized)
            {
                Logger.LogCF(string.Format(": Filtered={0}, Prioritized={1} **********************************************", nFiltered, nPrioritized));
            }

            //************************************************************************************************************
            // At EXACTLY THIS POINT we select the interpretation to use !!
            //************************************************************************************************************
            InputInterpretation inputInterpretation = null;
            switch (nPrioritized)
            {
#warning TODO consider returning an InputInterpreatation object with "Category = None" in case 0 in order to avoid handling the null value
                case 0: break; //ShowWarning(decoderClient, filteredInputValues); // Warn through UI if desired
                case 1: inputInterpretation = prioritizedInputValues.InputInterpretations[0];break; // The normal case
                default: inputInterpretation = prioritizedInputValues.InputInterpretations[0]; break;
#warning: TODO: Let the user select among the possible interpretations at this point.
            }

            // Calculate the new state

            DecoderStateMachine.StateEnum oldState = decoderStateMachine.State.MyStateEnum;
            string oldStateName = oldState.ToString();
            decoderStateMachine.SetNewState(inputInterpretation);
            DecoderStateMachine.StateEnum newState = decoderStateMachine.State.MyStateEnum;
            string newStateName = newState.ToString();
            string inputValueString = prioritizedInputValues.ToString(rawOptions); // Only needed in error situations and for debugging !

            string inputInterpretationString = "No interpretation found";
            if (null != inputInterpretation)
            {
                inputInterpretationString = ShowControlCharacters(inputInterpretation.ToString(rawOptions)); // Describes the selected interpretation as a text string
            }
            
            bool stateChanged = (newState != oldState); // Simply compare the enums !
            int thisValue = brailleIntegers.List[0];

            string inputAsUnicode = TokenReaderUtilities.ToUnicodeChar(thisValue);
            string thisValueAsBraille = ToDotNumbers(thisValue);
            if (1 != prioritizedInputValues.Count)
            {
                // Exclusively for debugging purposes:
                string epilogue = string.Format("State={0,-15} Offset={1} StartsWith: {2} DOT{3,-6}", oldStateName, startIndex, inputAsUnicode, thisValueAsBraille);
                LogInterpretations(originalInputValues,    "OriginalInputValues", epilogue); // All possible inputvalues as received from the TokenReader. Shown with state information.
                LogInterpretations(filteredInputValues,    "FilteredInputValues", null); // All remaining inputvalues after applying the statedependent filter 
                LogInterpretations(prioritizedInputValues, "PrioritizedInputValues", null); // All remaining inputvalues after prioritizing (longest token has highest priority)
                string message = string.Format(": Arbitrarily choose filteredvalues[0] = {0}", inputInterpretationString);
                Logger.LogCF(message); // The arbitrarily chosen inputValæue. May be the wrong choise !!!
                //decoderWarnings.Add(message + "  " + epilogue); // Collect selected log messages locally in the decoder
                UserWarnings.LogUserWarning(message + "  " + epilogue,UserInfoFlagsEnum.InterpretationMoreThanOneFound);
            }

            if (Logger.DeveloperMode && this.Verbose)
            {
                string newStateText = stateChanged ? string.Format("NewState={0} ", newStateName) : "";
                string inputString = ToString(inputInterpretation, brailleIntegers.List);
                Logger.LogCF(string.Format(" {0,7} State={1,-15} Input={2} Result={3,-20} {4} ", startIndex, oldStateName, inputString, inputInterpretationString, newStateText));
            }

            return inputInterpretation;
        }


        /// <summary>
        /// Used during debug only for identifying ambiguioties in the interpretations received from TokenReader after applying statedependent filter an prioritizing with respect to tokenlength.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="listName"></param>
        /// <param name="epilogue"></param>
        private void LogInterpretations(InputInterpretationList list, string listName, string epilogue)
        {
            if (!Logger.DeveloperMode) return;
            Logger.LogCF(string.Format(": Found {0} Interpretations in {1}{2}:", list.Count, listName, (null == epilogue) ? "" : " for " + epilogue ));
            for (int i = 0; (i < list.Count); i++)
            {
                string s = list.InputInterpretations[i].ToString(rawOptions);
                Logger.Log(string.Format("----->{0}[{1}]={2}", listName, i, ShowControlCharacters(s)));
            }
        }


        /// <summary>
        /// Build representation in Unicode and Dotnumbers, solely for logging- for debugging-purposes
        /// </summary>
        /// <param name="input"></param>
        /// <param name="values"></param> 
        private string ToString(InputInterpretation input, List<int> values)
        {
            string unicode = "";
            string dots = "";         
            if (null == input) return "null";
            int length = input.TokenLength;
            StringBuilder sbUnicode = new StringBuilder();
            StringBuilder sbDotNumbers = new StringBuilder();
            bool allSpaces = true;
            for (int i = 0; (i < length); i++)
            {
                int value = values[i];
                allSpaces &= (value == TokenReader.noDots);
                sbUnicode.Append(TokenReaderUtilities.ToUnicodeChar(value));
                string s = (value <= 0x3f) ? ToDotNumbers(value) : TokenReaderUtilities.ToNonBrailleInterpretation(value);
                sbDotNumbers.Append(s + " ");
            }
            unicode = sbUnicode.ToString();
            if (allSpaces)
            {
                unicode = string.Format("{0} SPACE{1}", length, (1 == length) ? "" : "S");
            }
            dots = sbDotNumbers.ToString();
            return string.Format("{0,-8} DOTS=({1,-10})", unicode, dots);
        }

        /// <summary>
        /// Simple mechanism for reporting to UI if the number of interpretations differs from 0.
        /// MAy later be extended, allowing the user to selegt among the interpretations !
        /// </summary>
        /// <param name="decoderClient"></param>
        /// <param name="inputInterpretations"></param>
        private void ShowWarning(IDecoderClient decoderClient, InputInterpretationList inputInterpretations)
        {
            if (null == decoderClient) return;
            int nInterpretations = inputInterpretations.Count;
            if (nInterpretations == 1) return;    // Found exactly one interpretation as desired.   
            {
                List<string> lines = new List<string>();
                foreach (InputInterpretation inputInterpretation in inputInterpretations.InputInterpretations)
                {
                    string s = inputInterpretation.ToString(this.rawOptions);
                    lines.Add(s);
                }
                string caption = string.Format("{0} Interpretation found:", nInterpretations);
                decoderClient.ShowMessageBox(caption, lines);
            }
        }


private string ShowControlCharacters(string s)
        {
            string s1 = s.Replace("\r", "<CR>");
            string s2 = s1.Replace("\n", "<LF>");
            return s2;
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

        public XmlDocument MusicXmlDocument
        {
            get { return musicXmlBuilder.Doc; }
        }


        /// <summary>
        /// First primitive mechanism for removing empty measures et the end.
        /// Will probably not work in all situations.
        /// </summary>
        public void RemoveEmptyLinesAtEnd()
        {
            if (null == musicXmlBuilder.Doc) return;
            XmlNode score = MusicXmlDocument.SelectSingleNode("score-partwise");
            XmlNodeList parts = score.SelectNodes("part");        
            foreach (XmlNode part in parts)
            {
                XmlNode partid = part.Attributes.GetNamedItem("id");
                string partIdString = partid.Value;

                if (0 == (string.Compare("P3", partIdString)))
                {
#warning remove hack for identifing chords
                    break;
                }

                XmlNodeList measures = part.SelectNodes("measure");
                int nMeasures = measures.Count;
                for (int i = nMeasures - 1; (i >= 0); i--)
                {
                    XmlNode measure = measures[i];
                    XmlNodeList notes = measure.SelectNodes("note");
                    if (0 == notes.Count)
                    {
                        part.RemoveChild(measure);
                    }
                    else
                    {
                        break; // Stop on the first non-empty measure !
                    }
                }
                XmlNodeList measuresAfterRemoval = part.SelectNodes("measure");
                int nMeasuresAfterRemoval = measuresAfterRemoval.Count;
                if (nMeasures != nMeasuresAfterRemoval)
                {
                    string s = string.Format(": Part={0} reduced from {1} to {2} measures by removing empty measures at end.", partIdString, nMeasures, nMeasuresAfterRemoval);
                    Logger.LogCF(s);
                }
            }
        }

        #region RemoveEmptyparts
        /// <summary>
        /// The current implementation sometimes generates a MusicXml file with an empty part (intended to represent the Chord-representation)
        /// This will be reported by MuseScore as a serious error.
        /// This method removes such empty 
        /// Fixes error 1022
        /// </summary>
        /// <param name="musicXmlDocument"></param>
        public void RemoveEmptyParts()
        {
            if (null == musicXmlBuilder.Doc) return;

            XmlNode scorePartwise = MusicXmlDocument.SelectSingleNode("score-partwise");
            XmlNode partList = scorePartwise.SelectSingleNode("part-list");

            // Find all empty parts
            XmlNodeList parts = scorePartwise.SelectNodes("part");
            List<XmlNode> emptyParts = new List<XmlNode>();
            foreach (XmlNode part in parts)
            {
                // Avoid manipulting inside foreach!
                if (part.ChildNodes.Count == 0)
                {
                    emptyParts.Add(part);
                }
            }


            // Remove all empty parts
            foreach (XmlNode emptyPart in emptyParts)
            {
                string id = GetId(emptyPart);
                Logger.LogCF(string.Format(": Removing empty part '{0}'", id));
                // Remove the empty part itself
                scorePartwise.RemoveChild(emptyPart);
                // Remove parts with the same id from the partlist           
                RemoveNamedChildNodes(partList, id);
            }
        }

        private string GetId(XmlNode node)
        {
            return node.Attributes.GetNamedItem("id").Value;
        }

        private void RemoveNamedChildNodes(XmlNode node, string name)
        {
            // Remove empty parts from the partlist
            List<XmlNode> nodesToRemove = new List<XmlNode>();
            foreach (XmlNode child in node)
            {
                string id = GetId(child);
                if (id == name)
                {
                    nodesToRemove.Add(child);
                }
            }
            foreach (XmlNode child in nodesToRemove)
            {
                node.RemoveChild(child);
            }
        }

        #endregion // RemoveEmptyParts






        //
        // Constructors
        //

        private Decoder(DecoderStateMachine.StateEnum initialState, string brailleAsUnicode,Options rawOptions, IDecoderClient decoderClient, DecoderDebugTools decoderDebugTools)
        {
            // Logger.ClearLocalUserWarnings();
            this.decoderStateMachine =  new DecoderStateMachine(initialState);
            this.brailleAsUnicode = brailleAsUnicode;
            this.decoderSpacePositionHandler = DecoderSpacePositionHandler.Create(rawOptions.StringFormatOptions);
          
            this.rawOptions = rawOptions;
            this.decoderClient = decoderClient;
            this.tokenReader = TokenReader.Create(this.brailleAsUnicode,RegionalOptions.Create(rawOptions.RegionalOptions));
            this.musicXmlBuilder = MusicXmlBuilder.Create(MusicXmlBuilderStateEnum.Title); // We exprct the Braille Music file to start with the title information in Text Braille form.
#warning TODO fetch 3 strings below from  localization!
            // The following setings yield for instance: "Part  'Højre hånd'  Takt 1" 
            this.musicXmlBuilder.StateNameCaption = ""; // Use no caption
            this.musicXmlBuilder.PartNameCaption = ""; // Use no caption
            this.musicXmlBuilder.MeasureNumberCaption = "Takt ";
            this.decoderDebugTools = decoderDebugTools;
            //this.developmentOptions = developmentOptions; 
            this.typeAmbiguityHandler = TypeAmbiguityHandler.Create(musicXmlBuilder,decoderDebugTools.DevelopmentOptions);
            this.brailleSubSequenceList = BrailleSubSequenceList.Create(brailleAsUnicode);
            //Logger.ClearLocalUserWarnings();
            //UserWarnings.LocationInfo = this as IDecoderUserInfo;
        }

        public static Decoder Create(DecoderStateMachine.StateEnum initialState, string brailleAsUnicode, Options rawOptions,IDecoderClient decoderClient, DecoderDebugTools decoderDebugTools)
        {
            //string logString = string.Format(": InitialState='{0}' Length={1} Options='{2}'", initialState.ToString(), brailleAsUnicode.Length, options.ToString());
            //Logger.LogCF(logString);
            Decoder result = new Decoder(initialState, brailleAsUnicode, rawOptions, decoderClient, decoderDebugTools);
            //UserWarnings.LocationInfo = result as IDecoderUserInfo;
            return result;
        }

    }
}
