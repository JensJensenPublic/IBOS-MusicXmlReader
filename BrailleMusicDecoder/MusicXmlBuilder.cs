using System;
using System.Collections.Generic;
using System.Xml;
using MusicXmlReaderModel; // Logger
using BrailleMusicDecoder.MusicXmlHandlers;
using BrailleMusicDecoder.MusicXmlElements;

namespace BrailleMusicDecoder
{
    public class MusicXmlBuilder
    {
        // The Xml Document we are currently building
        private MusicXmlDocument doc;
        public MusicXmlDocument Doc { get { return doc; } }
        private MusicXmlBuilderState state = MusicXmlBuilderState.Create(MusicXmlBuilderStateEnum.Initial,null);

        private List<MusicXmlBuilderState> allStates = new List<MusicXmlBuilderState>();
        /// <summary>
        /// A list of all currently existing  MusicXmlBuilderState: Typicalle 1 for the title, one for chords and 1 per part.
        /// </summary>
        public List<MusicXmlBuilderState> AllStates { get { return allStates; } }

        public string GetDebugInfo() { return this.state.GetDebugInfo(); }

        // The following nodes are needed as member variable because we need to add children later
        private XmlNode workNode = null;
        public XmlNode WorkNode { get { return workNode; } }
        private XmlNode creditNode = null;
        public XmlNode CreditNode { get { return creditNode; } } 

        private XmlNode scorePartwiseNode = null;
        public XmlNode ScorePartwiseNode { get { return scorePartwiseNode; } }
        private XmlNode partList = null;
        public XmlNode PartList { get { return partList; } }

        private MusicXmlBuilderStateHarmonyPart theHarmonyPart = null;
        internal MusicXmlBuilderStateHarmonyPart TheHarmonyPart { get { return theHarmonyPart; } set { theHarmonyPart = value; } } // The one and only HarmonyPart

        private List<MusicXmlBuilderState> existingStates = new List<MusicXmlBuilderState>();
        public List<MusicXmlBuilderState> ExistingStates  { get { return existingStates; } }

        private string stateNameCaption = "State="; 
        public  string StateNameCaption{ set{ stateNameCaption = value; } } // Allow for localization !
        private string partNameCaption = "PartName=";
        public string PartNameCaption { set { partNameCaption = value; } } // Allow for localization !
        private string measureNumberCaption = "Measure=";
        public string MeasureNumberCaption { set { measureNumberCaption = value; } } // Allow for localization !

        private SectionHeaderHandler sectionHeaderHandler;
        public SectionHeaderHandler SectionHeaderHandler { get { return sectionHeaderHandler; } }


        /// <summary>
        /// All inputs for the current measure within the current part.
        /// Used by TypeAmbiguityHandler for storing input until the complete measure is available end the ambiguities can (hopefully) be resolved.
        /// </summary>
        public List<InputInterpretation> InputListForCurrentState { get { return state.InputList; } }  


        public string GetStateInformation()
        {
            string result;
            MusicXmlBuilderStateEnum stateEnum = state.GetState();
            if (state is MusicXmlBuilderStateMusic)
            {
                // In the music states we have lots of information available (and even more if desired!!
                MusicXmlBuilderStateMusic stateMusic = state as MusicXmlBuilderStateMusic;
                result = string.Format("{0}={1}  {2}'{3}'  {4}{5}",
                    stateNameCaption, stateEnum.ToString(), //                 0 and 1 Example: "State=Part"  PartName='Højre hånd'  Measure=5
                    partNameCaption, stateMusic.FriendlyName, //               2 and 3 Example: "PartName='Højre hånd'" 
                    measureNumberCaption, stateMusic.GetCurrentMeasureNumber()); // 4 and 5 Example: " Measure=5"
                return result;
            }
          
            result = string.Format("{0}={1}", stateNameCaption, stateEnum.ToString()); // Example: "State=Title"
            return result;
        }

        private int defaultBeatType = 4;
        /// <summary>
        /// The "4" in "4/4"
        /// </summary>
        public int DefaultBeatType { get { return defaultBeatType; } }

        private int defaultBeat = 4;
        /// <summary>
        /// The 3 in "3/4"
        /// </summary>
        public int DefaultBeats { get { return defaultBeat; } }


        //public void OnSectionHeader(string s)
        //{
        //    string message = string.Format("({0}) AllStates contains {1} states",s, allStates.Count);
        //    Logger.LogCF(message);
        //    // We need to distribute this call to all existing states:
        //    foreach (MusicXmlBuilderState state in allStates)
        //    {
        //       state.OnSectionHeader(s);
        //    }

        //}


        /// <summary>
        /// Allows the MusicXmlStateTitle class to set up the default value of beat and beattype.
        /// Needed if the Music Braille file contains an informal  beat specification (as text Braille) in the title in stead of as a formal specification for each part.
        /// </summary>
        /// <param name="beat"></param>
        /// <param name="beatType"></param>
        public void SetDefaultBeatParameters(int beat, int beatType)
        {
            this.defaultBeat = beat;
            this.defaultBeatType = beatType;
        }

        static public MusicXmlBuilder theMusicXmlBuilder;

        /// <summary>
        /// Avoid construction
        /// </summary>
        private MusicXmlBuilder()
        {
            sectionHeaderHandler = SectionHeaderHandler.Create();
            if (null == theMusicXmlBuilder) theMusicXmlBuilder = this;
        }

        //private MusicXmlBuilder()
        //{  
        //}

        public void ApplyNextInput(InputCategoryEnum inputCategory, int inputInteger, InputSubCategoryEnum inputSubCategory, string inputSubCategoryValue)
        {
            // Show CR as CR etc.
            char c = (char)inputInteger;
            string s = "" + c;
            InputInterpretation input = new InputInterpretation(new IntegerList(), inputCategory, s, inputSubCategory);
            ApplyNextInput(input);   
        }


        /// <summary>
        /// This is the main Method for applying new input to the MusicXmlBuilder StateMachine.
        /// </summary>
        /// <param name="input"></param>
        public void ApplyNextInput(InputInterpretation input)
        {
            MusicXmlBuilderState  newState = state.ApplyNextInput(input);
            if (!allStates.Contains(newState))
            {
                allStates.Add(newState);
            }
            state = newState;        
        }


        /// <summary>
        /// Primarily for debugging
        /// </summary>
        /// <returns></returns>
        public int GetCurrentMeasureNumber()
        {
            MusicXmlBuilderStateMusic musicState = this.state as MusicXmlBuilderStateMusic;
            if (null == musicState) return -1;
            return musicState.GetCurrentMeasureNumber();
        }

        /// <summary>
        /// Primarily for debugging
        /// </summary>
        /// <returns></returns>
        public int FirstMeasureNumber { get { return MusicXmlBuilderStateMusic.FirstMeasureNumber; } }

        private MusicXmlBuilderStatePart GetMusicXmlBuilderState()
        {
            MusicXmlBuilderStatePart musicXmlBuilderStatePart = this.state as MusicXmlBuilderStatePart;
            if (null == musicXmlBuilderStatePart)
            {
                string message = string.Format("Unexpected MusicXmlBuilderState = {0}", this.state.GetState().ToString());
                Logger.LogCF(string.Format(": {0}", message));
                //throw new Exception(message);
            }
            return musicXmlBuilderStatePart;
        }

        public int GetCurrentFullMeasureDivisions()
        {
            MusicXmlBuilderStatePart musicXmlBuilderStatePart = GetMusicXmlBuilderState();
            if (null == musicXmlBuilderStatePart) return 0;
            return musicXmlBuilderStatePart.CurrentFullMeasureDivisions;
        }
        
        public string GetCurrentPartName()
        {
            MusicXmlBuilderStatePart musicXmlBuilderStatePart = GetMusicXmlBuilderState();
            if (null == musicXmlBuilderStatePart) return "?";
            return musicXmlBuilderStatePart.FriendlyName;
        }

        public int GetTotalDuration(List<InputInterpretation> inputList)
        {
            List<int> durations; // Dummy parameter
            return FixTotalDuration(inputList, out durations, false);
        }

        public int GetTotalDuration(List<InputInterpretation> inputList, out List<int> durations)
        {
            return FixTotalDuration(inputList, out durations, false);
        }

        /// <summary>
        /// Attempts to fix the duration of a measure if the duration of the measure does not match the expected duration.
        /// This can not be done in the decoder because all sorts of part-measure repetition and grouping has to be applied first. 
        /// </summary>
        /// <param name="inputList">The sequence of InputInterpretations representing the measure</param>
        /// <returns></returns>
        public int FixTotalDuration(List<InputInterpretation> inputList)
        {
            List<int> durations; // Dummy parameter
            return FixTotalDuration(inputList, out durations, true);
        }

        // A simple private common implementation of the two public methods: FixTotalDuration() and GetTotalDuration()
        private int FixTotalDuration(List<InputInterpretation> inputList, out List<int> durations, bool fix)
        {
            int result = 0;
            int timeModificationMultiplier = 1;
            int timemodificationDivider = 1;
            int timeModificationCounter = 0;
            durations = new List<int>();
            MusicXmlBuilderStatePart musicXmlBuilderStatePart = GetMusicXmlBuilderState();
            if (null == musicXmlBuilderStatePart) return 0;
            int latestDivisions = 0; // For adding punctuations
            int appoggiaturaFactor = 1; // Used to ignore specific notes, such as appogiatura
            foreach (InputInterpretation inputInterpretation in inputList)
            {
                switch (inputInterpretation.Category)
                {
                    case InputCategoryEnum.Note:
                    case InputCategoryEnum.Rest:
                    case InputCategoryEnum.InsertedRest:
                        int divisions = musicXmlBuilderStatePart.GetDivisions(inputInterpretation.GetNoteType());
                        // Check if a note  with this number of divisions can be placed at this poundary
                        if (fix)
                        {
                            int remainder = result % divisions;
                            if (0 != remainder)
                            {
                                Logger.LogCF(string.Format(": Remainder={0}", remainder));
                                UnAmbiguousNoteTypeEnum unAmbiguousNoteTypeEnum = UnAmbiguousNoteTypeEnum.TypeUnknown;
                                switch (inputInterpretation.SubSubCategory)
                                {
                                    case InputSubSubCategoryEnum.NoteTypeFullMeasureOrWholeOr16th: unAmbiguousNoteTypeEnum = UnAmbiguousNoteTypeEnum.Type16th; break;
                                    case InputSubSubCategoryEnum.NoteTypeHalfOr32nd: unAmbiguousNoteTypeEnum = UnAmbiguousNoteTypeEnum.Type32nd; break;
                                    case InputSubSubCategoryEnum.NoteTypeQuarterOr64th: unAmbiguousNoteTypeEnum = UnAmbiguousNoteTypeEnum.Type64th; break;
 //                                   case InputSubSubCategoryEnum.NoteTypeEighthOr128th: unAmbiguousNoteTypeEnum = UnAmbiguousNoteTypeEnum.Type128th; break;
                                    case InputSubSubCategoryEnum.NoteTypeEighthOr128th: unAmbiguousNoteTypeEnum = UnAmbiguousNoteTypeEnum.Type16th; break; // Asume Grouping, not 1/128 !
                                    default: throw new Exception("");
                                }
                                if (UnAmbiguousNoteTypeEnum.TypeUnknown != unAmbiguousNoteTypeEnum)
                                {
                                    inputInterpretation.UnAmbiguousNoteType = unAmbiguousNoteTypeEnum; // Modify the unambigutius noteType
                                    Logger.LogCF(string.Format(": Set Unambigoius notetype to {0}", unAmbiguousNoteTypeEnum));
                                    divisions = divisions / 16;
                                }
                            }
                        }
                        divisions = divisions * appoggiaturaFactor * timeModificationMultiplier / timemodificationDivider;
                        switch (timeModificationCounter)
                        {
                            case 0: break;
                            case 1: timeModificationCounter = 0; timeModificationMultiplier = 1; timemodificationDivider = 1; break;
                            default: timeModificationCounter--; break;
                        }    
                        latestDivisions = divisions;
                        result += divisions;
                        durations.Add(divisions);
                        appoggiaturaFactor = 1; 
                        break;
                    case InputCategoryEnum.Punctuation:
                        divisions = latestDivisions / 2; // Normal single punctuation
                        switch (inputInterpretation.SubCategory)
                        {
                            // Handle double and triple punctuations:
                            case InputSubCategoryEnum.PunctuationDouble: divisions = latestDivisions  * 3 / 4; break; // Add 1/2 + 1/4
                            case InputSubCategoryEnum.PunctuationTriple: divisions = latestDivisions  * 7 / 8; break; // Add 1/2 + 1/4 + 1/8
                            default: break;
                        }
                        latestDivisions = divisions;
                        result += divisions;
                        durations.Add(divisions);
                        break;
                    case InputCategoryEnum.OtherValues:
                        switch (inputInterpretation.SubCategory)
                        {
                            case InputSubCategoryEnum.OthervaluesShortAppoggiatura:
                            case InputSubCategoryEnum.OthervaluesLongAppoggiatura:
                                appoggiaturaFactor = 0;
                                break;
                            default: break;
                        }
                        break;

                    case InputCategoryEnum.PartMeasureRepeat:
                        // Repeat the latest durations until the measure is filled up.
                        // If we repeat from the start we will end up with a total durationof 2 * result, so we may have to skip some of the first durations:
                        int expectedDuration = this.GetCurrentFullMeasureDivisions();
                        int durationToSkip =  2* result - expectedDuration;
                        int durationAdded = 0;

                        int numberOfRepetitions = 0;
                        switch (inputInterpretation.SubCategory)
                        {
                            case InputSubCategoryEnum.PartMeasureRepeatOnce: numberOfRepetitions = 1; break;
                            case InputSubCategoryEnum.PartMeasureRepeatTwice: numberOfRepetitions = 2; break;
                            case InputSubCategoryEnum.PartMeasureRepeatThreeTimes: numberOfRepetitions = 3; break;
                            default:
                                Logger.LogCF("");
                                throw new Exception("");
                        }


                        List<int> repeatedDurations = new List<int>();
                        foreach (int duration in durations)
                        {
                            if (durationToSkip > 0)
                            {
                                durationToSkip -= (duration * numberOfRepetitions);
                            }
                            else
                            {
                                repeatedDurations.Add(duration);
                                durationAdded += duration;
                            }
                        }
                        // Add the collected durations once or twice:
                        for (int i = 0; (i < numberOfRepetitions); i++)
                        {
                            durations.AddRange(repeatedDurations);
                            result += durationAdded;
                        }
                        

#if false
                        // Old implementation: Repeat all durations. Assumes the repetition starts at the start of the voice!
                        result += result;
                        List<int> repeatedDurations = new List<int>();
                        repeatedDurations.AddRange(durations);
                        durations.AddRange(repeatedDurations);
#endif
                        break;

                    case InputCategoryEnum.TimeModification:
                        if (InputSubCategoryEnum.TimeModificationTriplet != inputInterpretation.SubCategory) break;
                        // Multiply the duration of the next 3  notes by a factor of 2/3
                        timeModificationMultiplier = 2;
                        timemodificationDivider = 3;
                        timeModificationCounter = 3; 
                        break;

                    default: break; // throw new Exception("");
                }
            }
            return result;
        }


        /// <summary>
        /// Initialize to valid MusicXml
        /// </summary>
        public void Init(MusicXmlBuilderStateEnum initialState)
        {
            string testResult = MusicXmlTest.Test();
            Logger.LogCF(string.Format(": MusicXml.Test() returned {0}", testResult));
            //doc = new XmlDocument();    // Use this for creating a normal XmlDocument 
            doc = new MusicXmlDocument(); // Use this for creating a MusicXmlDocument with derived classes for MusicXmlNoteElement MusicXmlMeasureElement etc!!
            musicXmlElementFactory = MusicXmlElementFactory.Create(doc);
            string encodingName = "UTF-8";
            //string encodingName = "UTF-16";
            XmlNode docNode = doc.CreateXmlDeclaration("1.0", encodingName, null);
            doc.AppendChild(docNode);

            string docType = "DOCTYPE score-partwise PUBLIC \" -//Recordare//DTD MusicXML 3.1 Partwise//EN\" \"http://www.musicxml.org/dtds/partwise.dtd\"";
            XmlNode docTypeNode = doc.CreateComment(docType);
            doc.AppendChild(docTypeNode);

            scorePartwiseNode = doc.CreateElement("score-partwise");
            XmlAttribute versionAttribute = doc.CreateAttribute("version");
            versionAttribute.Value = "3.1";
            scorePartwiseNode.Attributes.Append(versionAttribute);
            doc.AppendChild(scorePartwiseNode);

            workNode = doc.CreateElement("work");
            scorePartwiseNode.AppendChild(workNode);
  
            XmlNode identificationNode = musicXmlElementFactory.IdentificationElement();
            scorePartwiseNode.AppendChild(identificationNode);

            XmlNode defaultsElement = musicXmlElementFactory.DefaultsElement();
            scorePartwiseNode.AppendChild(defaultsElement);

            creditNode = musicXmlElementFactory.Element("credit"); // Empty placeholder! To be filled in later 
            scorePartwiseNode.AppendChild(creditNode); 
//            creditNode.Attributes.
//@"1 Hej
//2 Hej
//3 Hej
//4 Hej"
//);
//            scorePartwiseNode.AppendChild(creditNode);


           partList = doc.CreateElement("part-list");
            scorePartwiseNode.AppendChild(partList);
  
            state = MusicXmlBuilderState.Create(initialState, this);
        }

        private MusicXmlElementFactory musicXmlElementFactory;
        public MusicXmlElementFactory MusicXmlElementFactory { get { return musicXmlElementFactory; } }

        static public MusicXmlBuilder Create()
        {
            MusicXmlBuilder result =  new MusicXmlBuilder();
            result.Init(MusicXmlBuilderStateEnum.Unknown);
            return result;
        }

        static public MusicXmlBuilder Create(MusicXmlBuilderStateEnum initialState)
        {
            MusicXmlBuilder result = new MusicXmlBuilder();
            result.Init(initialState);
            return result;
        }
    }

}
