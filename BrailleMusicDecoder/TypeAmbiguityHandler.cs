using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicXmlReaderModel;

namespace BrailleMusicDecoder
{

    /// <summary>
    /// Class for handling various types of type ambiguity found in the Music Braille decinitions.
    /// For instance:
    ///  1) Same symbol used for FullMeasure rest, whole rest, 16th rest etc or
    ///  2) Note grouping using 1/8 duration for 1/8 notes
    /// 
    /// This is implemented by queing (Music Braille) InputInterpretation objects untill a a sequence of such objects representing a full measure of a single part has been received.
    /// When a such sequence of InputInterpretation objects from a single part has been collected, it is analysed as a whole, and all ambiguities are (hopefully) resolved.
    /// Note that only the ambiguous musical type values (musical durations) of notes and rests are modified, no other modification is done, except that any empty measured are ignored.
    /// If the sequence represents more than one voice (Danish: stemme: HovedStemme, Bistemme(r)), represented by InAccordPartMeasure, InAccordFullMeasure and MeasureDivision objects,
    /// the sequence is first split into Voice objects and each voice is analyzed separately.
    /// </summary>
    public class TypeAmbiguityHandler
    {
        // Some simple shorthands for reducing the amount of text:
        private const InputSubSubCategoryEnum aNtWholeOr16nd = InputSubSubCategoryEnum.NoteTypeFullMeasureOrWholeOr16th;
        private const InputSubSubCategoryEnum aNt2ndOr32nd = InputSubSubCategoryEnum.NoteTypeHalfOr32nd;
        private const InputSubSubCategoryEnum aNt4thOr64th = InputSubSubCategoryEnum.NoteTypeQuarterOr64th;
        private const InputSubSubCategoryEnum aNt8thOr128th = InputSubSubCategoryEnum.NoteTypeEighthOr128th;
        private const UnAmbiguousNoteTypeEnum uNt16th = UnAmbiguousNoteTypeEnum.Type16th;
        private const UnAmbiguousNoteTypeEnum uNt32nd = UnAmbiguousNoteTypeEnum.Type32nd;
        private const UnAmbiguousNoteTypeEnum uNt64nd = UnAmbiguousNoteTypeEnum.Type64th;
        private const UnAmbiguousNoteTypeEnum uNt128th = UnAmbiguousNoteTypeEnum.Type128th;

        private bool useExperimentalCode = false;

        private DevelopmentOptionEnum developmentOptions = DevelopmentOptionEnum.None;

        private bool EnableLogging = Logger.DeveloperMode;

        /// <summary>
        /// Simple mechanism for turning all local logging on or off
        /// </summary>
        /// <param name="s"></param>
        private void ConditionalLogCF(string s)
        {
            if (!EnableLogging) return;
            Logger.LogCF1(s);
        }

        /// <summary>
        /// Strictly experimental code for collecting a full measure of Musicstate inputInterpretations before handing them to the MusicXmlBuilder.
        /// In this way it will be possible to deal with Music Braille disambiguties such as usage og the same symbol for several note durations.
        /// </summary>
        /// <param name="inputInterpretation"></param>
        public void ApplyNextInput(InputInterpretation inputInterpretation)
        {
            // This is the trick: By always referencing the InputList via the MusicXmlBuilder we reference the InputList per Part!
            // This handles the situation where the Music Braille input file changes part (for instance RightHand to LeftHand inside a measure.)
            List<InputInterpretation> inputSequence = musicXmlBuilder.InputListForCurrentState;  // All  inputs for the measure within the current part

            switch (inputInterpretation.Category)
            {
                case InputCategoryEnum.Hand: // Just pass it through in order to change the state of the MusicXmlBuilder  (and thus the contents of the lists referenced through it)                
                    ToXml(inputInterpretation);     // From now on input goes to he new state (for instance to the LeftHand part instead of to the RightHand part) 
                    return;

                case InputCategoryEnum.ToMusicBraille:
                    ToXml(inputInterpretation);  
                    return;

                case InputCategoryEnum.Beat:  // Just pass it through in order to change the state of the MusicXmlBuilder  (and thus the value of the expected measure duration)
                    ToXml(inputInterpretation); // From now on type ambiguities are resolved using the new beat/beattype
                    return;

                case InputCategoryEnum.FinalDoubleBar:  // This marks the termination of a part.
                    Flush(inputSequence); // Handle the input of the last measure collected until now.
                    ToXml(inputInterpretation); // Handle the FinalDoubleBar itself.
                    return;

                case InputCategoryEnum.NewMeasure:
                case InputCategoryEnum.SectionalDoubleBar:
                    bool currentMeasureIsNotEmpty  = OnEndOfCurrentMeasure(inputSequence); // Attempt to resolve ambiguities in the current measure for the current part
                    if (currentMeasureIsNotEmpty)
                    {
                        // Only add the new measure if the current measure is not empty
                        ToXml(inputInterpretation); // Current input, causing the flush.
                    }
                    return;

                default: // This is just another input for the current measure. Update the list, which is local for the current part:           
                    inputSequence.Add(inputInterpretation);
                    return;
            }
        }



        /// <summary>
        /// Generate a comprehensive stringrepresentation to be used for debugging
        /// </summary>
        /// <returns></returns>
        private string ToShortDebugString(List<InputInterpretation> inputSequence)
        {
            StringBuilder sb = new StringBuilder();
            foreach (InputInterpretation i in inputSequence)
            {
                sb.Append(i.ToShortDebugString()+ " ");
            }
            return sb.ToString();
        }




        /// <summary>
        ///  At this point inputInterpretation.Category == InputCategoryEnum.NewMeasure which marks the end of the current measure.
        //  We thus have all information ablut the current measure and can attempt to fix all unambiguities before passing it on.
        /// </summary>
        /// <param name="inputSequence">The inputSequence to build the measure from</param>
        /// <returns>true <==> The current measure is not empty</returns>
        private bool OnEndOfCurrentMeasure(List<InputInterpretation> inputSequence)
        {
            BreakInMeasure(24); // Break here during debugging

            // First check if measure contains notes, rests or puctuations
            // Also find the set of all Categories represented in the inputsequence
            InputCategoryEnum categoriesRepresented = 0;
            int notesAndRestsAndPunctuations = 0;
            foreach (InputInterpretation input in inputSequence)
            {
                categoriesRepresented |= input.Category;
                if (IsNoteOrRestOrPunctuation(input))
                {
                    notesAndRestsAndPunctuations++;
                }
            }

            string measureAtEntry = ToShortDebugString(inputSequence);
            ConditionalLogCF(string.Format("+ {0}", measureAtEntry));

            if (0 == notesAndRestsAndPunctuations)
            {
                string warning = string.Format("{0} {1}",PreAmble," Empty measure is ignored ! ");
                UserWarnings.LogUserWarning(warning,UserInfoFlagsEnum.InterpretationEmptyMeasureIgnored);
                ConditionalLogCF("-" + warning);
                return false;
            }

            // Spilt up in main voice and 0 or more side voices
            VoiceList voiceList = VoiceList.Create(inputSequence, musicXmlBuilder.GetCurrentFullMeasureDivisions());
            if (voiceList.NumberOfVoices > 1)
            {
                //string preAmble = string.Format("'{0}' Measure={1}", this.PartName, this.MeasureNumber);
                voiceList.Log(PreAmble);
            }

            // Handle the voices one at a time, attempting to fix any timing problem caused by ambiguity:
            foreach (Voice voice in voiceList.AllVoices)
            {
                OnEndOFVoice(voice);
            }

            string measureAtExit = ToShortDebugString(inputSequence);
            if (0 != string.Compare(measureAtEntry, measureAtExit))
            {
                // If the lines differ they are logged together to make it easy to vompare them:
                ConditionalLogCF(" :ENTRY:" + measureAtEntry);
                ConditionalLogCF(" :EXIT: " + measureAtExit);
            }

            ConditionalLogCF(string.Format("- {0}", measureAtExit));
    

            // We have now (hopefully) resolved all ambiguities in this measure, one voice at a time and pass the measure on for conversion to MusicXml.
            Flush(inputSequence); // Handle all previous input for this measure

            return true;
        }

        private bool IsNoteOrRestOrPunctuation(InputInterpretation i)
        {
            InputCategoryEnum category = i.Category;
            if (category == InputCategoryEnum.Note) return true;
            if (category == InputCategoryEnum.Rest) return true;
//            if (category == InputCategoryEnum.InsertedRest) return true;
            if (category == InputCategoryEnum.Punctuation) return true;
            return false;
        }

        private int DurationDifference(Voice voice)
        {
            int actualVoiceDuration = musicXmlBuilder.GetTotalDuration(voice.InputInterpretations);
            int difference = (actualVoiceDuration - voice.ExpectedMeasureDuration);
            if (0 != difference)
            {
                string message = string.Format(": {0}  ( {1} )  ActualVoiceDivision={2} differs from ExpectedVoiceDuration={3}", PreAmble, voice.ToShortDebugString(), actualVoiceDuration, voice.ExpectedMeasureDuration);
                ConditionalLogCF(message);
            }
            return difference;
        }
  

        private void OnEndOFVoice(Voice voice)
        {
            voice.ExpandPartMeasureRepeats(); // Start by expanding all PartMasure repeats. 

            int dif = DurationDifference(voice);
            if (0 == dif) return; // The actual length of the inputsequence when using default values (1/2 1/4 1/8 1/16) for the ambiguities notetypes fits the expectation. Leave it there! 

            ConditionalLogCF(string.Format(": Voice={0} Applying grouping etc",voice.VoiceNumber));

            // First initialize local temporary convenience variable
            List<InputInterpretation> nodesAndRestsAndPunctuations = new List<InputInterpretation>();
            List<InputSubSubCategoryEnum> types = new List<InputSubSubCategoryEnum>();

            foreach (InputInterpretation input in voice.InputInterpretations)
            {
                if (IsNoteOrRestOrPunctuation(input))
                {
                    nodesAndRestsAndPunctuations.Add(input);
                    types.Add(input.SubSubCategory);
                }
            }

            LogMeasure(nodesAndRestsAndPunctuations);
            BreakInMeasure(38); // Break here during debugging. 

            switch (nodesAndRestsAndPunctuations.Count)
            {
                case 0:
                    OnEmptyVoice(voice.VoiceNumber);
                    return; // Obviously nothing to do! 

                case 1: // If the only note or rest is FullMeasure or Whole or 16th it is reported as FullMeasure
                    if (nodesAndRestsAndPunctuations[0].SubSubCategory == InputSubSubCategoryEnum.NoteTypeFullMeasureOrWholeOr16th)
                    {
                        nodesAndRestsAndPunctuations[0].UnAmbiguousNoteType = UnAmbiguousNoteTypeEnum.TypeFullMeasure;
                    }
                    break;

                case 6: // A measure containing 6 notes or rests where the first is a 16th and the rest is eight:
                    bool grouping = (nodesAndRestsAndPunctuations[0].SubSubCategory == InputSubSubCategoryEnum.NoteTypeFullMeasureOrWholeOr16th);
                    for (int i = 1; (i < 6) && grouping; i++)
                    {
                        grouping &= (nodesAndRestsAndPunctuations[i].SubSubCategory == InputSubSubCategoryEnum.NoteTypeEighthOr128th);
                    }
                    if (grouping)
                    {
                        for (int i = 0; (i < 6); i++)
                        {
                            nodesAndRestsAndPunctuations[i].UnAmbiguousNoteType = UnAmbiguousNoteTypeEnum.Type16th;
                        }
                        ConditionalLogCF(string.Format(": Grouping {0} notes or rests as 1/16;", nodesAndRestsAndPunctuations.Count));
                    }
                    break;

                case 12: // A measure containing 12 notes or rests where the first is a 32th and the rest is eight:
                    bool grouping12 = true;
                    // 3 sub-groups of 4
                    for (int i4 = 0; (i4 < 12) && grouping12; i4 += 4)
                    {
                        // First in each group is half
                        grouping12 &= (nodesAndRestsAndPunctuations[i4].SubSubCategory == InputSubSubCategoryEnum.NoteTypeHalfOr32nd);
                        {
                            // Remaining are eight
                            for (int i = 1; (i < 4) && grouping12; i++)
                            {
                                grouping12 &= (nodesAndRestsAndPunctuations[i4 + i].SubSubCategory == InputSubSubCategoryEnum.NoteTypeEighthOr128th);
                            }
                        }
                    }
                    if (grouping12)
                    {
                        for (int i = 0; (i < 12); i++)
                        {
                            nodesAndRestsAndPunctuations[i].UnAmbiguousNoteType = UnAmbiguousNoteTypeEnum.Type32nd;
                        }
                        ConditionalLogCF(string.Format(": Grouping {0} notes or rests as 1/32;", nodesAndRestsAndPunctuations.Count));
                    }
                    break;



                default:
                    // Adding the following code will solve a lot of problems in Fur Elise where even partmeasure repetition of grouping now works!
                    // But at the same time introduceces errore in "4 piano piecec Leo Smith measure 12 and 15 where half-notes followed by eights are misinterpreted
                    // are misinterpreded as groups of 1/32th.
                    // A decent solution requires a mechanism generating a several versions of each measure using different resolving of ambiguities until
                    // one is found whrer the length fits.
                    // As this involves handlign of part-measure repetitions and bistemmer it requires a full generation of each vrsion of each measure.
                    // Suggests a Measure class or a MeasureBuilder class and a class holding a list of these during evaluation.
                    List<InputInterpretation> nrp = nodesAndRestsAndPunctuations; // Shorthand
                    for (int i = 0; (i <= nrp.Count - 4);)
                    {
                        // Check the ambiguous notetypes for grouping of 1/16th
                        InputSubSubCategoryEnum firstNoteInGroup = nrp[i].SubSubCategory;
                        UnAmbiguousNoteTypeEnum groupingNoteType = GetGroupingNoteType(firstNoteInGroup); // Returns "Unknown" if grouping is not supported!"
                        if ((UnAmbiguousNoteTypeEnum.TypeUnknown != groupingNoteType) // Either a 1/32 or a  1/16                    
                        && (nrp[i + 1].SubSubCategory == aNt8thOr128th) // followed by 3 1/8
                        && (nrp[i + 2].SubSubCategory == aNt8thOr128th)
                        && (nrp[i + 3].SubSubCategory == aNt8thOr128th))
                        {
                            // Identified a grouping of 4 notes. Set the Unambiguous values ti 1/16                    
                            nrp[i].UnAmbiguousNoteType = groupingNoteType;
                            nrp[i + 1].UnAmbiguousNoteType = groupingNoteType;
                            nrp[i + 2].UnAmbiguousNoteType = groupingNoteType;
                            nrp[i + 3].UnAmbiguousNoteType = groupingNoteType;
                            i += 4;
                            ConditionalLogCF(string.Format(": Grouping {0} notes or rests as {1};", 4, groupingNoteType));
                        }
                        else
                        {
                            i++;
                        }

                    }
                    break; // Remaining case are to be handled here.
            }

            dif = DurationDifference(voice);
            if (0 == dif) return;

            ConditionalLogCF(string.Format(": Voice={0} Applying other fixes",voice.VoiceNumber));

            // If possible fix any obviously wrong duration information:
            int fullMeasureDivisions = musicXmlBuilder.GetCurrentFullMeasureDivisions();
            FixDurations(voice, fullMeasureDivisions);
            dif = DurationDifference(voice);
            if (0 == dif) return;
             
            FixDurationsLastChange(voice,dif);

            dif = DurationDifference(voice);
            if (0 == dif) return;


            OnFixFailed(voice);
        }


        /// <summary>
        /// Returns the unambiguous notetype to be used for grouping of notes of the input type specified
        /// Retuerns Unknown if grouping is not implemented for the input type specified.
        /// </summary>
        /// <param name="subsubCategory"></param>
        /// <returns></returns>
        private UnAmbiguousNoteTypeEnum  GetGroupingNoteType(InputSubSubCategoryEnum subsubCategory)
        {
            switch (subsubCategory)
            {
                case aNtWholeOr16nd: return uNt16th;
                case aNt2ndOr32nd: return uNt32nd;
                //case aNt4thOr64th: return uNt64nd; // Maybe later
                //case aNt8thOr128th: return uNt128th; // Maybe later
                default: return UnAmbiguousNoteTypeEnum.TypeUnknown;                       
            } 
        }

        private const int DurationOfQuarterNote = MusicXmlBuilderStateMusic.DivisionsPerQuarterNote;
        private const int DurationOfFullNote = DurationOfQuarterNote * 4;
        private const int DurationOfHalfNote = DurationOfQuarterNote * 2;
        private const int DurationOf8thNote  = DurationOfQuarterNote / 2;
        private const int DurationOf16thNote = DurationOfQuarterNote / 4;
        private const int DurationOf32thNote = DurationOfQuarterNote / 8;

        /// <summary>
        ///  Experimental code !! A last change ad hoc attempt to fix ambiguities:
        /// </summary>
        /// <param name="voice"></param>
        /// <param name="durationDifference"></param>
        private void FixDurationsLastChange(Voice voice, int durationDifference)
        {                
            switch (durationDifference)
            {
                case DurationOf16thNote - DurationOfFullNote: // A single note or rest interpreted as 1/16 should really be a 1/1
                    foreach (InputInterpretation ii in voice.InputInterpretations)
                    {
                        if ((ii.UnAmbiguousNoteType == UnAmbiguousNoteTypeEnum.TypeUnknown) && (ii.SubSubCategory == InputSubSubCategoryEnum.NoteTypeFullMeasureOrWholeOr16th))
                        {
                            // nr 266A 267 Det er i dag et vejr.musicxml.DIFFERS.musicxml measure 9
                            // BrailleOrch Copland - Four Piano Blues.1.For Leo Smith.musicxml measure 42

                            ii.UnAmbiguousNoteType = UnAmbiguousNoteTypeEnum.TypeWhole;
                        }
                    }
                    break;
                default: break;
            }
        }


        private void OnFixFailed(Voice voice)
        {
            // All attempts to fix duration failed. Log and Warn.
            // First check if the measure containing the Voice contains any of the yet unsupported categories:

            InputCategoryEnum categoriesRepresentedInMeasure = voice.CategoriesRepresentedInMeasure();

            StringBuilder partMeasureInfo = new StringBuilder();
            if (0 != (categoriesRepresentedInMeasure & (InputCategoryEnum.MeasureDivision | InputCategoryEnum.InAccordPartMeasure)))
            {
                // The "Fix" code does not yet handle InAccordPartMeasure, but at least we show it in the warning if the measure contains it !
                partMeasureInfo.Append("InAccordPartMeasure ");
            }
            if (0 != (categoriesRepresentedInMeasure & (InputCategoryEnum.TimeModification)))
            {
                // The "Fix" code does not yet handle TimeModificatins, but at least we show it in the warning if the measure contains it !
                partMeasureInfo.Append("TimeModification");
            }

            if (0 != (categoriesRepresentedInMeasure & (InputCategoryEnum.PartMeasureRepeat)))
            {
                // The "Fix" code does not yet handle PartMeasureRepeats, but at least we show it in the warning if the measure contains it !
                partMeasureInfo.Append("PartMeasureRepeat ");
            }

            if (voice.OwningMeasureContainsEndings())
            {
                // The "Fix" code does not yet handle Repeats and endings, but at least we show it in the warning if the measure contains it !
                partMeasureInfo.Append("GraphicRepeatsAndEndings ");
            }


            bool isFirstMeasure = (MeasureNumber <= musicXmlBuilder.FirstMeasureNumber);
            if (isFirstMeasure)
            {
                // The Fix code can not fix problems in the first measure because it need not contain the normal number of divisions.
                partMeasureInfo.Append("IsFirstMeasure ");
            }

#warning todo implement isLastMeasure.
            bool isLastMeasure = false;
            if (isLastMeasure)
            {
                // The Fix code can not fix problems in the first measure because it need not contain the normal number of divisions.
                partMeasureInfo.Append("IsLastMeasure ");
            }

            // If we can't fix the duration and it is caused by one or more of the known problems we report the set of known problems found
            // If it is not caused by any of the known problems we just report "Unknown Fix-problem""

            string diagnosticsInfo = "Unknown Fix-problem!"; // This is the default if no other problems are found
            if (0 != partMeasureInfo.Length)
            {
                diagnosticsInfo = string.Format(" Problem: {0} ", partMeasureInfo.ToString());
            }  

            string message = Message(voice.VoiceNumber, " Failed to fix duration." + diagnosticsInfo);
   
            ConditionalLogCF(string.Format(": {0} ( {1} )", message, voice.ToShortDebugString())); // Add the actual contents to the logmessage.
            if (isFirstMeasure) return; // For the time being we cant fix ambuguities in the first measure because it needs not be complete. We log them but dont report them!
            UserWarnings.LogUserWarning(message,UserInfoFlagsEnum.GenerationFailedToFixDuration);
        }

        private void OnEmptyVoice(int voiceNumber)
        {
            // Empty voice encountered. Log and warn
            string message = Message(voiceNumber, "Voice contains no notes or rests");
            ConditionalLogCF(":" + message);
            ///UserWarnings.LogUserWarning(message); // Seems to be normal practice to terminate with an empty InAccordFullMeasure token
        }

        int MeasureNumber { get { return musicXmlBuilder.GetCurrentMeasureNumber() - 1; } } // This message relates to the measure just before CurrentMessage !} }
        string PartName { get { return musicXmlBuilder.GetCurrentPartName(); } } // Such as "Right Hand"
        string QuotedPartName { get { return string.Format("'{0}'", PartName); } }

        /// <summary>
        /// Common preamble to be used for most UserWarnings and logentries.
        /// </summary>
        string PreAmble { get { return  string.Format("{0,-15} Measure={1}", this.QuotedPartName, this.MeasureNumber); } } //  Vertically align quoted partnames up to 12 characters : "Venstre hånd"

        private string Message(int voiceNumber, string message)
        {
            return  string.Format("{0} Voice={1} {2}", PreAmble, voiceNumber,message);
        }


        /// <summary>
        /// Computes the total duration os the inputlist
        /// Logs if the total duration differs from the expected value
        /// Attempts to fix the difference.
        /// </summary>
        /// <param name="inputList"></param>
        private void FixDurations(Voice voice, int expectedDuration)
        {
            List<int> allDurations;
            int totalDuration = musicXmlBuilder.GetTotalDuration(voice.InputInterpretations, out allDurations);
            if (totalDuration != expectedDuration)
            {
                LogDurationDiffernce(allDurations, totalDuration, expectedDuration);
                string voiceString = voice.ToShortDebugString();
                ConditionalLogCF(string.Format(": {0}", voiceString));
                //#if false// Disable or enable experimental code
                if (useExperimentalCode)
                {
                    if (totalDuration > expectedDuration)
                    {
                        musicXmlBuilder.FixTotalDuration(voice.InputInterpretations); // Fix too long duration.

                    }
                    else
                    {
                        FixTooShort();
                    }
                }
                //#endif
            }

        }

        private void FixTooShort() { }

        private void LogDurationDiffernce(List<int> allDurations, int totalDuration, int expectedDuration)
        {
            StringBuilder sbAllDurations = new StringBuilder();
            string conc = "";
            foreach (int i in allDurations)
            {
                sbAllDurations.Append(conc + i.ToString());
                conc = "+";
            }
            string allDuratinsString = sbAllDurations.ToString();
            int measureNumber = musicXmlBuilder.GetCurrentMeasureNumber();
            ConditionalLogCF(string.Format(": MeasureNumber={0} TotalDuartion={1}=({2}) differs from ExpectedDuration={3}", measureNumber, totalDuration, allDuratinsString, expectedDuration));
            if (27 == measureNumber)
            {
                ConditionalLogCF("");
            }
        }

        private void Flush(List<InputInterpretation> inputSequence)
        {
            foreach (InputInterpretation input in inputSequence)
            {
                ToXml(input);
            }
            inputSequence.Clear();
        }

        private void ToXml(InputInterpretation inputInterpretation)
        {
                                                                 //  ********************************************************************************************************************        
            musicXmlBuilder.ApplyNextInput(inputInterpretation); //  ****************************** This is where wa call the MusicXmlBuilder to build MusicXml *************************
                                                                 //  ******************************************************************************************************************** 
            // throw new Exception("For debugging purposes only !"); 
     
        }

        /// <summary>
        /// Simple tool for setting a breakpoint in a specific measure
        /// </summary>
        /// <param name="measureNumber"></param>
        /// <returns></returns>
        private bool BreakInMeasure(int measureNumber)
        {
            bool result = false;
            if (measureNumber == musicXmlBuilder.GetCurrentMeasureNumber())
            {
                Logger.LogCF1(string.Format("BreakPoint in measure={0}", measureNumber));
                result = true;
            }
            return result;
        }

        /// <summary>
        /// Build a simple text representation of the current measure for debugging purposes
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        private string LogMeasure(List<InputInterpretation> inputInterpretations)
        {
            StringBuilder sb = new StringBuilder();
            foreach (InputInterpretation inputInterpretation in inputInterpretations)
            {
                sb.Append("("+ inputInterpretation.FriendlyValue + ") ");
            }
            string result = sb.ToString();
            //Logger.LogCF1(string.Format(": {0}", result));
            return result;
        }
        

        MusicXmlBuilder musicXmlBuilder;
        private TypeAmbiguityHandler(MusicXmlBuilder musicXmlBuilder, DevelopmentOptionEnum developmentOptions)
        {
            this.musicXmlBuilder = musicXmlBuilder;
            this.developmentOptions = developmentOptions;
            if (DevelopmentOptionEnum.FurElize == developmentOptions)
            {
                this.useExperimentalCode = true;
                ConditionalLogCF(string.Format(": Using experimental code because DevelopmentOptions = {0}", developmentOptions.ToString()));
            }
        }

        public static TypeAmbiguityHandler Create(MusicXmlBuilder musicXmlBuilder,DevelopmentOptionEnum developmentOptions )
        {
            return new TypeAmbiguityHandler(musicXmlBuilder, developmentOptions);
        }
    }
}
