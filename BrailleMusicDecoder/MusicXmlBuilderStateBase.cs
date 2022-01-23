using System;

using System.Xml;
using MusicXmlReaderModel; // For Logger. NameSpace only, NO reference !!
using System.Collections.Generic;


namespace BrailleMusicDecoder
{
    public enum MusicXmlBuilderStateEnum
    {
        Unknown = 0,
        Initial,
        Title,
        Part,
        Harmonies
    };


    public abstract class MusicXmlBuilderState
    {
        protected string friendlyName;
        public string FriendlyName { get { return friendlyName; } } // is Null unless for parts
        private XmlNode partList = null;
        protected MusicXmlElementFactory musicXmlElementFactory;


        // Start new code
        private List<InputInterpretation> inputList = new List<InputInterpretation>(); // Used by TypeAmbiguityHandler only! See below:
        /// <summary>
        /// All inputs for the current measure within this current part.
        /// Used by TypeAmbiguityHandler for storing input until the complete measure is available end the ambiguities can (hopefully) be resolved.
        /// </summary>
        public List<InputInterpretation> InputList{  get { return inputList; } }
        // End new code

        public abstract MusicXmlBuilderStateEnum GetState();

        /// <summary>
        /// The main StateMachine method for applying the next input to the current state of the StateMachine
        /// </summary>
        /// <param name="input">An InputInterpretation containing the following fields</param>
        /// <param name="input.Category">The main category of the input, for instance "Note"</param>
        /// <param name="input.Value">A string representation of the input, only used for debugging</param>
        /// <param name="input.SubCategory">The SubCategory of the input, for instance "FullStepA" for the "Note" "A"</param>
        /// <param name="input.SubCAtegoryValue">A string value, further specifying the input</param>
        /// <param name="inputS.ubSubCategory">The SubSubCategory of the input, for instance "NoteTypeQuarter" for the "Note" "A" "Quarter"</param>
        /// <returns>The new state</returns>
        public abstract MusicXmlBuilderState ApplyNextInput(InputInterpretation input);

        public virtual void OnSectionHeader(string s) { }

        private void GetBeatParameters(string inputValue, out int beats, out int beatType)
        {
            char[] splitterChar = new char[] { '/' };
            string[] inputValues = inputValue.Split(splitterChar);
            beats = int.Parse(inputValues[0]);
            beatType = int.Parse(inputValues[1]);
        }

        public abstract void AddPart(InputInterpretation input);
 

        public virtual void SetIntervalDirection(InputSubCategoryEnum subCategory)
        {
            throw new Exception("Not implemented");
        }

        protected void GetBeatParameters(InputInterpretation input, out int beats, out int beatType)
        {
            beats = 0;
            beatType = 0;
            switch (input.SubCategory)
            {
                case InputSubCategoryEnum.BeatTypeCommon: beats = 4; beatType = 4; return;
                case InputSubCategoryEnum.BeatTypeCut: beats = 2; beatType = 2; return; // a.k.a. "alla breve"
                case InputSubCategoryEnum.BeatFraction:
                case InputSubCategoryEnum.BeatFractionAndEmptySpace:
                case InputSubCategoryEnum.BeatFractionAndCarriageReturn: GetBeatParameters(input.SubCategoryValue, out beats, out beatType); return;
            }
            throw new Exception(string.Format("Unexpected input"));
        }

        public static MusicXmlBuilderState Create(MusicXmlBuilderStateEnum newState, MusicXmlBuilder musicXmlBuilder)
        {
            return MusicXmlBuilderState.Create(newState, musicXmlBuilder, null);
        }

        public static MusicXmlBuilderState Create(MusicXmlBuilderStateEnum newState,MusicXmlBuilder musicXmlBuilder, string friendlyPartName)
        {
            if (newState == MusicXmlBuilderStateEnum.Initial)
            {
                //Logger.LogCF(": Explicitly Clearing static list of states!");
                //existingStates.Clear(); // This is a STATIC liat and must be explicitly cleared!
                return null;
            }

            foreach (MusicXmlBuilderState existingState in musicXmlBuilder.ExistingStates)
            {
                if ((existingState is MusicXmlBuilderStatePart)
                &&  (null != existingState.FriendlyName)
                &&  (0 == string.Compare(existingState.FriendlyName, friendlyPartName)))
                {
                    // We already have a state object representing this part (for instance in Bar over Bar layout)
                    // Do not create a new object, but continue using the existing one. 
                    return existingState;
                }
            }

            switch (newState)
            {
                case MusicXmlBuilderStateEnum.Title: return MusicXmlBuilderStateTitle.Create(musicXmlBuilder);
                case MusicXmlBuilderStateEnum.Part:
                    // We need to handle several parts in parallea in Bar over Bar notation:
                    MusicXmlBuilderState state =  MusicXmlBuilderStatePart.Create(musicXmlBuilder,friendlyPartName);
                    musicXmlBuilder.ExistingStates.Add(state);
                    return state;
                case MusicXmlBuilderStateEnum.Harmonies: return MusicXmlBuilderStateHarmonyPart.Create(musicXmlBuilder);
                default: return null;
            }
        }

        protected MusicXmlBuilder musicXmlBuilder;

        protected MusicXmlBuilderState(MusicXmlBuilder musicXmlBuilder)
        {
            // All common initialization goes here !
            this.musicXmlBuilder = musicXmlBuilder;
            this.musicXmlElementFactory = musicXmlBuilder.MusicXmlElementFactory; 
            // Derived classes may add extra initialization as needed.      
        }

        protected void AddSelectedEvent(string selectedEvent)
        {
            UserWarnings.LogUserWarning(selectedEvent,UserInfoFlagsEnum.InterpretationAddedSelectedEvent);
        }

        protected void OnUnsupportedInput(string prefix,InputInterpretation input)
        {
            UserWarnings.LogUserWarning(string.Format("{0} Unsupported InputCategory={1}", prefix, input.Category.ToString()),UserInfoFlagsEnum.InterpretationUnSupportedInput); // For the list of user warnings
        }
 
        
        /// <summary>
        /// Centralized logging to be used by all derived classes. Logs the ClassName and MethodName of the method calling this method.
        /// NOTE: The BrailleMusicDecoder project is referenced by the MusicXmlReaderModel project!
        /// So in order to avoid circular references the Logger class is placed in a separate project MusicXmlReaderModelBase, which is referenced by both.
        /// Everything uses the same MusicXmlReaderModel Namespace! 
        /// </summary>
        /// <param name="s">The string to be logged</param>
        protected void LogCF(string s)
        {
            Logger.LogCF1(s);
        }

        public virtual string GetDebugInfo() { return "";}

    }
}
