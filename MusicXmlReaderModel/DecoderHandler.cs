using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Xml;
using BrailleMusicDecoder;

namespace MusicXmlReaderModel
{
    /// <summary>
    /// Class implementing all methods needed by an UI client for handling the BrailleMusicDecoder.
    /// Also references the MusicPlayer class, which is not a part of the BrailleMusicDecoder.
    /// </summary>
    public class DecoderHandler : IDecoderClient
    {
        public bool MusicXmlGenerationFailed { get { return brailleMusicDecoder.MusicXmlGenerationFailed; } }
        public string MusicXmlGenerationError { get { return brailleMusicDecoder.MusicXmlGenerationError; } }
        Decoder brailleMusicDecoder;
        MusicPlayer musicPlayer;
        IDecoderUiClient decoderUiClient;
        string brailleFileAsUnicode = null;
        public string BrailleFileAsUnicode { get { return brailleFileAsUnicode; } }
        private BrailleMusicDecoder.Options rawOptions;

        /// <summary>
        /// Used from client interpreting the contents of any file as Braille Music.
        /// Returns a list of DecoderItems primarily designed to be used directly by a ListBox in the UI.
        /// Generates a complete MusicXml representation of the input file
        /// </summary>
        /// <param name="fileName">The name of the MusicBrailleFile to decode, including the full path</param>
        /// <param name="fileEncoding">The encoding of the MusicBrailleFile to decode, for instance BRF_ASCII,  PEF,  BRL_OctoBraille_1252,  BRF_Unicode</param>
        /// <param name="musicXmlDocument">The XmlDocument to fill in with the Xml representation generated</param>
        /// <param name="decoderOptions">Detailled options for text formatting of the result returned </param>
        /// <returns></returns>
        public List<DecoderItem> InterpretBrailleMusicFile(string fileName, BrailleFileHandler.FileEncoding fileEncoding, out XmlDocument musicXmlDocument, DecoderOptions decoderOptions)
        {
            UserWarnings.ClearLocalUserWarnings();
            Logger.CurrentMusicBrailleSourceFileName = fileName; // Make filename accessible anywhere from a static variable. For logging purposes only !
            musicXmlDocument = null;
            //BrailleMusicDecoder.Options rawOptions = decoderOptions.RawDecoderOptions; // Make all options avvessible in a structure defined by the BrailleMusicDecoder. 
            rawOptions = decoderOptions.RawDecoderOptions; // Make all options avvessible in a structure defined by the BrailleMusicDecoder. 

            Logger.LogCF(string.Format(": FileName={0} FileEncoding={1} DecoderDeveloperMode={2}", fileName, fileEncoding.ToString(), rawOptions.DeveloperMode.ToString()));
            List<DecoderItem> result = new List<DecoderItem>();

            BrailleFileHandler brailleFileHandler = BrailleFileHandler.Create(fileEncoding, 0, 0); // Just leave the formatting parameters as 0 for interpreting a file
            string logLine = (null != brailleFileHandler) ? string.Format("Created BraillefileHandler {0}", brailleFileHandler) : "Failed to create BrailleFileHandler";
            Logger.LogCF(logLine);
            if (null == brailleFileHandler) return result;

            brailleFileAsUnicode = brailleFileHandler.ReadFromFile(fileName); // Read the Input file and convert to Unicode

            // During debug: modify the contents of the input string before decoding it
            DecoderDebugTools decoderDebugTools = DecoderDebugTools.Create(fileName);
            //DevelopmentOptionEnum developmentOptions = decoderDebugTools.GetDevelopmentOptions(fileName); // Check for modifications of the Unicode input needed during development 
            brailleFileAsUnicode = decoderDebugTools.ModifyDuringDebug(brailleFileAsUnicode); // Apply modifications of the Unicode input needed during development 

            // If required, add contents formatted as raw Lines of MusicBraille
            if (0 != (decoderOptions.FormatOptions & DecoderOptions.FormatOptionsEnum.rawBrailleLines))
            {
                result.AddRange(this.GetRawLines(brailleFileAsUnicode));
            }

            // Create a Decoder using the set of options specified 
            brailleMusicDecoder = BrailleMusicDecoder.Decoder.Create(DecoderStateMachine.StateEnum.Text, brailleFileAsUnicode, rawOptions, this as IDecoderClient, decoderDebugTools);
            // Use the Decoder to create the decoded lines and always add contents formatted as decoded lines of MusicBraille         
            result.AddRange(GetDecodedLines(brailleMusicDecoder, brailleFileAsUnicode, decoderOptions));

            // Save the MusicXml representation of the decoded file 
            brailleMusicDecoder.RemoveEmptyLinesAtEnd();
            musicXmlDocument = brailleMusicDecoder.MusicXmlDocument;
            brailleMusicDecoder.LogStatistics(); // Log a table containing the number of state transitions that occurred during the operation. For debugging purposes only!
            //brailleMusicDecoder.LogWarnings();

            brailleMusicDecoder.LogCategories(); // Log a list of all the number of occurances of each InputCategoryEum during the operation. For debugging purposes only ! 

            Logger.LogCF(": Exit");
            return result;
        }        

        public string GetNumberOfSeparateScores(out int numberOfScores)
        {
            Logger.LogCF("+");
            numberOfScores = 0;
            string message;
            if ((null == brailleMusicDecoder) || (null == brailleMusicDecoder.BrailleSubSequenceList))
            {
#warning todo Localize message
                message = "No MusicBraille file is loaded.";
                Logger.LogCF(string.Format(": Error: {0}", message));
                // ModelBaseMessageBox.Show(message,ModelBaseMessageBoxButtons.OK,ModelBaseMessageBoxIcon.Exclamation);
                return message;
            }
            numberOfScores = brailleMusicDecoder.BrailleSubSequenceList.List.Count;
            return string.Empty;
        }

        const string tempExportPath = @"C:\Temp\TempExport";

        /// <summary>
        /// Generates an informatice suffix for the filename to indicate the encoding
        /// This is NOT a file extension, but is intended to be used as a part of the filename
        /// </summary>
        /// <param name="encoding"></param>
        /// <returns></returns>
        private string GetSuffix(BrailleFileHandler.FileEncoding encoding)
        {
            switch (encoding)
            {
                case BrailleFileHandler.FileEncoding.BRF_Unicode_utf8: return "_Utf-8";
                case BrailleFileHandler.FileEncoding.BRF_Unicode_utf16: return "_Utf-16";
                case BrailleFileHandler.FileEncoding.BRF_Unicode_utf32: return "_Utf-32";
                case BrailleFileHandler.FileEncoding.BRF_ASCII: return "_ASCII";
                case BrailleFileHandler.FileEncoding.BRL_OctoBraille_1252: return "_OctoBraille";
                default:return "";
            }
        }


        public string ExportToSingleScore(BrailleFileHandler.FileEncoding encoding, DecoderOptions.RegionalOptionsEnum regionalOptions,string fullInputFileName)
        {
            Logger.LogCF(string.Format("({0},{1},{2})+", encoding, regionalOptions,fullInputFileName));
            try   // And now for the real action:
            {
                // throw new Exception("For test only!");
                BrailleFileHandler brailleFileHandler = BrailleFileHandler.Create(encoding, 0, 0);
                string suffix = GetSuffix(encoding);
                string extension = brailleFileHandler.GetExtension();
                string shortFileName = Path.GetFileNameWithoutExtension(fullInputFileName) + suffix + extension;
                string fullFileName = Path.Combine(tempExportPath, shortFileName);
                brailleFileHandler.WriteToFile(brailleFileAsUnicode, fullFileName, true); // Write the original contents of the file in the format specified
            }
            catch (Exception ex)
            {
#warning ToDo Localize
                return string.Format("Exception.Message='{0}'", ex.Message);
            }
            Logger.LogCF(string.Format("({0},{1})-", encoding, regionalOptions));
            return ""; // Signals success 
        }     


            public string ExportToSeparateScores(BrailleFileHandler.FileEncoding encoding, DecoderOptions.RegionalOptionsEnum regionalOptions, string fullInputFileName)
        {
            Logger.LogCF(string.Format("({0},{1},{2}) ", encoding, regionalOptions,fullInputFileName));
            int numberOfScores;
            string message = GetNumberOfSeparateScores(out numberOfScores);
            if (!string.IsNullOrEmpty(message)) return message;
            if (numberOfScores <= 1)
            {
#warning todo Localize message
                message = "The MusicBraille file currently loaded does not contain multiple scores";
                Logger.LogCF(string.Format(": Error: {0}", message));
                return message;
            }

            try   // And now for the real action:
            {
                // throw new Exception("For test only!");
                BrailleFileHandler brailleFileHandler = BrailleFileHandler.Create(encoding, 0, 0);
                string extension = brailleFileHandler.GetExtension();
                foreach (BrailleSubSequence bss in brailleMusicDecoder.BrailleSubSequenceList.List)
                {
                    string contents = bss.Contents;
                    string fileName = bss.Name;           
                    string shortFileName = bss.Name + extension;
                    string fullFileName = Path.Combine(tempExportPath, shortFileName);
                    brailleFileHandler.WriteToFile(contents, fullFileName, true);
                }
            }
            catch (Exception ex)
            {
#warning ToDo Localize
                return string.Format("Exception.Message='{0}'",ex.Message);
            }
            return ""; // Signals success   
        }

        /// <summary>
        /// Get all local userwarnings issued since latest call to ClearLocalUserWarnings()
        /// The messages are formatted to fit into a standard MessageBox.
        /// </summary>
        /// <returns></returns>
        public List<string> GetLocalUserWarnings(UserInfoFlagsEnum mask)
        {
            List<string> result = new List<string>();
            List<UserInfoBase> userInfoList = UserWarnings.GetlocalUSerWarnings();
            if (0 == userInfoList.Count) return result;
            // Take the filename from the first item
            foreach (UserInfoBase userInfoBase in userInfoList)
            {
                UserInfoFlagsEnum userInfoEnum = userInfoBase.GetInfoEnum();
                Logger.LogCF(string.Format(": UserInfoEnum={0}", userInfoEnum.ToString()));

                if (0 == (( mask) & userInfoEnum))
                {
                    continue; // Continue with next userInfoBase, skipping this one 
                }

                //result.Add(string.Format("{0} {1} Warning:\r\n{2}", userInfo.PageLineSpace, userInfo.BrailleDotNumberString, userInfo.Message)); // Insert a newline between position and Message. MessageBox has limited width !
                // Build texts one by one
                // As this line is meant to be shown in a (narrow) standardmessagebox during handling of a single file the filename is not included.
                string line = "";
                string warningText = brailleMusicDecoder.Text_Warning; // Warning

                // We need localized values so we can't just relay on an abstract (unlocalized) userInfoBase.ToString()
                // Instead we must build the lines here, where localization information is available 
                switch (userInfoBase.GetInfoEnum())
                {
                    case UserInfoFlagsEnum.InterpretationWarning:
                        // This is a complicated warning contanin exact information about page, line and space etc
                        DecoderUserInfo userInfo = userInfoBase as DecoderUserInfo;
                        string pageText = string.Format("{0}={1,-4}", brailleMusicDecoder.Text_Page, userInfo.Page); // Page=9999
                        string lineText = string.Format("{0}={1,-3}", brailleMusicDecoder.Text_Line, userInfo.Line); // Line=999
                        string spaceText = string.Format("{0}={1,-2}", brailleMusicDecoder.Text_Space, userInfo.Space); // Space=99
                        string dotsText = string.Format("{0}{1,-6}", "", userInfo.BrailleDotNumberString); // DOT123456  No "DOT=" in front of dotnumbers
                        line = string.Format("{0} {1} {2} {3} {4}\r\n{5}", pageText, lineText, spaceText, dotsText, warningText, userInfo.Message); // Limited width in MessageBox. Force controlled linebreak!
                        break;                                                                                                                           // Build line from strings

                    case UserInfoFlagsEnum.DevelopmentInsertion:
                        DecoderUserInsertionInfo insertionInfo = userInfoBase as DecoderUserInsertionInfo;
                        // This is a warning about insertion of MusicBraille symbols into the original sourceFile
                        line = string.Format("Index={0}: {1} DOT {2}",  insertionInfo.Index,  insertionInfo.Message, insertionInfo.DotsToInsert);
                        break;

                    // This is a warning about replacement of MusicBraille symbols with others in the original sourceFile
                    case UserInfoFlagsEnum.DevelopmentReplacement:
                        DecoderUserReplacementInfo info = userInfoBase as DecoderUserReplacementInfo;
                        line = string.Format("Index={0} Replaced Braille={1} (DOTS={2}) by Braille={3} (DOTS={4})", info.Index, info.OldContents, info.OldContentsAsDots , info.NewContents, info.NewContentsAsDots);
                        break;

                    default:
                        // This is a simple warning containing only a caption and a message.                  
                        line = string.Format("{0} {1}", warningText, userInfoBase.Message);
                        break;
                }
                result.Add(line);      
            }
            return result;
        }


        public void OnExit()
        {
            if (null == brailleMusicDecoder)
            {
                Logger.LogCF(": No BrailleMusicDecoder found at exit");
                return;
            }
            brailleMusicDecoder.LogGlobalStatistics(); // The sum of all transitions during program lifetime.
        }

        public void ShowMessageBox(string caption, List<string> messageLines)
        {       
            if (null == decoderUiClient) return;
            // Pass up to the UI. In this way the UI ownly knows the Model, not the Decoder!
            decoderUiClient.ShowMessageBox(caption, messageLines);
        }

        /// <summary>
        /// Simpel utility for formatting as raw MusicBraille lines
        /// </summary>
        /// <param name="brailleFileAsUnicode"></param>
        /// <returns></returns>
        private List<DecoderItem> GetRawLines(string brailleFileAsUnicode)
        {
            List<DecoderItem> result = new List<DecoderItem>();

            {
                // First add the rawlines to the result. In this version ignore any formfeeds !
                string withoutLF = brailleFileAsUnicode.Replace("\r\n", "\r");
                char[] lineSplitChars = new[] { (char)'\r' };
                string[] lines = withoutLF.Split('\r');
                int lineNumber = 0;
                foreach (string line in lines)
                {
                    lineNumber++;
                    string s = string.Format("{0} {1}", lineNumber, line);
                    result.Add(DecoderItem.Create(s)); // Add a DecoderItem containing only the stringRepresentation
                }
            }
            return result;
        }


        /// <summary>
        /// Get an interpretation of a Braille6 ssequence as a single Token using a specified initial state
        /// </summary>
        /// <param name="text">The new MusicBraille sequence to interpret</param>
        /// <param name="decoderItem">An item describing the original state of the DecoderStatemachine when decoring the original MusicBraille sequence</param>
        /// <returns></returns>
        public string GetInterpretationString(string text, DecoderItem decoderItem)
        {
            if (decoderItem.InitialDecoderState == DecoderStateMachine.StateEnum.Unknown) return "";
            // Create a local Decoder using the original rawOptions, but a dummy version of DecoderDebugTools 
            DecoderDebugTools decoderDebugTools = DecoderDebugTools.Create("");
            Decoder localBrailleMusicDecoder = BrailleMusicDecoder.Decoder.Create(decoderItem.InitialDecoderState, text, rawOptions, this as IDecoderClient, decoderDebugTools);
            // Call the local Decoder to create a single token from the Braille6 textvalue specified
            int index = 0;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            string s;
            do
            {
                Token token = localBrailleMusicDecoder.GetNextToken(ref index);
                s = token.StringRepresentation;
                sb.Append((null == s) ? "" : s);
            } while (null != s);            
            return sb.ToString();  
        }


        private string GetDotNumbers(ref string rawLine, bool developerMode)
        {
            if (0 == rawLine.CompareTo("\f"))
            {
                rawLine = "FormFeed";
                return "";               
            }
            else
            {
                return developerMode ? " =" + Decoder.ToDotNumbers(rawLine) : ""; // Converts from Unicode (0x2800..0x28ff) to DotNumbers
            }
        }


private List<DecoderItem> GetDecodedLines(Decoder brailleMusicDecoder, string brailleFileAsUnicode, DecoderOptions decoderOptions)
        {
            int i = 0;
            DecoderItem decodedLine = null;
            DecoderOptions.FormatOptionsEnum stringFormatOptions = decoderOptions.FormatOptions;
            int latestBrailleLineNumber = 1; // For logging form, line when changing
            bool isNewBrailleLineNumber = true; // Set to true to force an initial line for form 1 line 1
            List<DecoderItem> decodedLines = new List<DecoderItem>();
            do
            {
                int originalIndex = i; // Will be changed during the call to GetNextToken()
                Token token = brailleMusicDecoder.GetNextToken(ref i);
                decodedLine = DecoderItem.Create(token.StringRepresentation, token.XmlRepresentation,token.TokenString,token.StartIndex,token.InitialDecoderState);
                if ((null != decodedLine) && (null != decodedLine.ToString()))
                {
                    if ((null != token.AccumulatedText) && (0 != (stringFormatOptions & DecoderOptions.FormatOptionsEnum.accumulatedText)))
                    {
                        decodedLines.Add(DecoderItem.Create(token.AccumulatedText));
                    }

                    if (isNewBrailleLineNumber && (0 != (stringFormatOptions & DecoderOptions.FormatOptionsEnum.newBrailleLineNumber)))
                    {
                        // Insert an extra line informing about the new linenumber and formnumber.
                        string s = brailleMusicDecoder.FormLineString;
                        string xmlBuilderString = decoderOptions.DeveloperMode ? "State" + brailleMusicDecoder.GetStateInformation() : ""; 
                        string offsetString = decoderOptions.DeveloperMode ?  string.Format("Offset {0} ", originalIndex) : ""; // Allow easy reference to LogFile. No Localization needed: Developermode only
                        string rawLine = brailleMusicDecoder.GetNextRawUnicodeLine(originalIndex); // The raw, undecoded contents of the line of Braille found at this point. Primarily for debugging.
                        string rawLineAsdotNumbers = GetDotNumbers(ref rawLine,decoderOptions.DeveloperMode);
                        string totalString = offsetString + s + " " + xmlBuilderString + " " + rawLine + rawLineAsdotNumbers;
                        decodedLines.Add(DecoderItem.Create(totalString));
                        if (Logger.DeveloperMode)
                        {
                            Logger.LogCF(string.Format(":>>> {0} <<<", totalString)); // Easy to find in the log !
                        }
                    }

                    if (!string.IsNullOrEmpty(decodedLine.ToString()))
                    {
                        string s = string.Format("{0}", decodedLine);
                        if (decoderOptions.DeveloperMode)
                        {
                            string rawValues = brailleMusicDecoder.GetRawValues(originalIndex, i); // Show each Unicode char as either a string representing Braille dots  or as a controlcharacter.
                            bool b = (0 != (stringFormatOptions & DecoderOptions.FormatOptionsEnum.indexAndLength));
                            int length = (i - originalIndex);
                            string originalIndexString = b ? string.Format("[{0,3}:{1,2}] ", originalIndex.ToString(), length.ToString()) : ""; // 3 positions for index, 2 for length
                            string rawValueString = (0 != (stringFormatOptions & DecoderOptions.FormatOptionsEnum.rawValues)) ? rawValues : "";
                            s = string.Format("{0}{1} {2}",
                            originalIndexString,
                            decodedLine, // Further subdevided by Decoder
                            rawValueString);
                        }
                        decodedLines.Add(DecoderItem.Create(s, token.XmlRepresentation,token.TokenString,token.StartIndex,token.InitialDecoderState));
                    }
                    isNewBrailleLineNumber = (latestBrailleLineNumber != brailleMusicDecoder.LineNumber);
                    latestBrailleLineNumber = brailleMusicDecoder.LineNumber;
                }
            } while ((null != decodedLine) && (null != decodedLine.ToString()));
         
#if true
            // Start new code 2024.03.05
            brailleMusicDecoder.BrailleSubSequenceList.AfterLastInput(); // Fix the latest subesquence if needed
            Logger.LogCF(string.Format(": {0}", brailleMusicDecoder.BrailleSubSequenceList.ToString())); // Log the whole list
            // End new code  2024.03.05
#endif

            return decodedLines;
        }

        /// <summary>
        /// Simple mechanism to be used from the UI for playing MusicXml NoteElements generated by the MusicBraille Decoder.
        /// Uses the MusicPlayer object used as parameter to Create().
        /// </summary>
        /// <param name="xmlNode"></param>
        /// <returns></returns>
        public bool Play(List<XmlNode> xmlRepresentation)
        {
            if (null == xmlRepresentation) return false;
            if (0 == xmlRepresentation.Count) return false;
            XmlNode xmlNode = xmlRepresentation[0];
            if (null == xmlNode) return false;
            if (null == this.musicPlayer) return false;
            if (0 != string.Compare(xmlNode.Name, "note")) return false;
            try
            {
                // throw new Exception("For test");
                XmlNode pitchNode = xmlNode.SelectSingleNode("pitch");
                if (null == pitchNode) return false;
                XmlNode step = pitchNode.SelectSingleNode("step");
                string stepString = step.InnerXml;
                XmlNode octaveNode = pitchNode.SelectSingleNode("octave");
                string octaveString = octaveNode.InnerXml;
                XmlNode alterNode = pitchNode.SelectSingleNode("alter");
                string alterString = "";
                if (null != alterNode)
                {
                    alterString = alterNode.InnerXml;
                }
                int octaveInt = int.Parse(octaveString);
                musicPlayer.Play(stepString, alterString, octaveInt);
            }
            catch (Exception e)
            {
                Logger.LogCFE(e);
                return false;
            }
            return true;
        }

        private DecoderHandler(MusicPlayer musicPlayer, IDecoderUiClient decoderUiClient)
        {
            this.musicPlayer = musicPlayer;
            this.decoderUiClient = decoderUiClient;
        }

        /// <summary>
        /// Simple static Create() method
        /// </summary>
        /// <param name="musicPlayer">The MusiPLayer object to use for playing the decoded file as music. If null no music will be played.</param>
        /// <returns></returns>
        public static DecoderHandler Create(MusicPlayer musicPlayer,IDecoderUiClient decoderUiClient)
        {
            return new DecoderHandler(musicPlayer,decoderUiClient);
        }

    }
}
