using System;
using System.Text;
using MusicXmlReaderModel; // Use the namespace, but do not reference MusicXmlReaderModel. Reference MusicXmlReaderModelBase instead to avoid circular references

namespace BrailleMusicDecoder
{

    /// <summary>
    /// Class for implementing the statemachine of the decoder.
    /// Contains a reference to the previous state, thus allowing for a return-mechanism ! 
    /// </summary>
    public class DecoderStateMachine
    {

        public enum StateEnum
        {
            Unknown,    // The current way of decoding has not yet been established
            Text,       // Decoding as lower-case text
            TextNumber, // Decoding as digits while decoding text
            TextNumberLowered, // Decode as lowered digits, used for describing number of first measure and number of measures
            TextVersal, // Decoding as upper-case text
//            TextAndChords, // Decoding as text, but allow for extra special symbols, describing chords
            Music,      // Decoding as Music, but not while decoding a note
            MusicNumber,// Decoding as digits while decoding music
            MusicNote,   // Decoding as a note while decoding music
            Chords, // Decoding as chords, looking for the next root note
            ChordsRoot // Decoding as chords, while adding details information to a root (Like MusicNote is adding details to a not)
        };


        private DecoderState state;
        public DecoderState State { get{ return state; } }
        private DecoderState previousState;

        public bool IsInAnyMusicState()
        {
            switch (state.MyStateEnum)
            {
                case StateEnum.Music: return true;
                case StateEnum.MusicNumber: return true;
                case StateEnum.MusicNote: return true;
                default: return false;
            }
        }

        #region statistics

        int numberOfStates;
        /// <summary>
        /// Only for statistics during debugging
        /// </summary>
        private int[,] transitionTable;
        private static int[,] theStaticTransitionTable = null;
        private void CountTransition(StateEnum fromState, StateEnum toState)
        {
            if (fromState != toState)
            {
                (transitionTable[(int)fromState, (int)toState])++; // Count within the instance table, that is within this Music Braill file
                (theStaticTransitionTable[(int)fromState, (int)toState])++; // Count within the static table, that is within this program activation.
            }
        }

        public void LogStatistics()
        {
            LogStatistics(transitionTable);
        }

        public void LogGlobalStatistics()
        {
            Logger.LogCF("+");
            LogStatistics(theStaticTransitionTable);
            Logger.LogCF("-");
        }


        private void LogStatistics(int[,] table)
        {
            Logger.LogCF("+");
            StringBuilder sbHeader = new StringBuilder();
            sbHeader.Append(string.Format("{0,-29}","To state:"));
            for (int col = 0; (col < numberOfStates); col++)
            {
                sbHeader.Append(string.Format("{0,5}", col));
            }
            Logger.Log(sbHeader.ToString());
            for (int row = 0; row < numberOfStates; row++)
            {
                StringBuilder sb = new StringBuilder();
                for (int col = 0; (col < numberOfStates); col++)
                {
                    sb.Append(string.Format("{0,5}", table[row, col]));
                }
                string rowText = string.Format("From {0}:{1,-20}: {2}",row, ((StateEnum)row).ToString(), sb.ToString());
                Logger.Log(rowText);
            }
            Logger.LogCF("-");
        }
        #endregion

        private DecoderStateMachine()
        {}

        public DecoderStateMachine(StateEnum state)
        {
            numberOfStates = Enum.GetValues(typeof(StateEnum)).Length;
            transitionTable = new int[numberOfStates, numberOfStates]; //Implicitly all values ar initialized to 0
            if (null == theStaticTransitionTable)
            {
                // New the singleton static version once per program activation.
                theStaticTransitionTable = new int[numberOfStates, numberOfStates]; //Implicitly all values ar initialized to 0
            }
            this.state = new DecoderState(state, null);
        }

        /// <summary>
        /// Force a state transition
        /// </summary>
        /// <param name="newState"></param>
        internal void SetState(StateEnum newState, int startIndex)
        {
            Logger.LogCF( string.Format("StartIndex={0} Forcing Statechange from {1} for {2}***********************************************************************************", startIndex, state.MyStateEnum.ToString(), newState.ToString()));
            state = new DecoderState(newState, this.state);;
        }

        // Define some (private) shorthand values to be used in the case below
        const InputCategoryEnum AnyEnding = InputCategoryEnum.FinalDoubleBar | InputCategoryEnum.SectionalDoubleBar;
        const InputCategoryEnum AnyChordSymbol = InputCategoryEnum.ChordSymbol | InputCategoryEnum.ChordSymbolRoot | InputCategoryEnum.ChordSymbolBass | InputCategoryEnum.ChordCharacter
            | InputCategoryEnum.ChordNumericExtension | InputCategoryEnum.ChordTiming | InputCategoryEnum.ChordStemSign;

        const InputCategoryEnum allowedInTextStates = InputCategoryEnum.Character | InputCategoryEnum.TextVersal | InputCategoryEnum.ToNumber | InputCategoryEnum.ToMusicBraille | InputCategoryEnum.Chords
            | InputCategoryEnum.Hand | InputCategoryEnum.ToNumberLowered | InputCategoryEnum.ControlCharCRLF | InputCategoryEnum.ControlCharFF | InputCategoryEnum.PrintPagination
            | InputCategoryEnum.KeySignatureText | InputCategoryEnum.TypeFormIndicator | InputCategoryEnum.BeatAsText | InputCategoryEnum.SectionHeader; // But not  | InputCategoryEnum.ControlCharCRLFNumber

        const InputCategoryEnum allowedInMusicStates = InputCategoryEnum.Note | InputCategoryEnum.Octave | InputCategoryEnum.Rest | InputCategoryEnum.InsertedRest | InputCategoryEnum.NewMeasure | InputCategoryEnum.SectionHeader
            | InputCategoryEnum.MeasureDivision | InputCategoryEnum.InAccordPartMeasure | InputCategoryEnum.InAccordFullMeasure | InputCategoryEnum.Clef
            | InputCategoryEnum.ToMusicBraille | InputCategoryEnum.ToText | InputCategoryEnum.TimeModification | InputCategoryEnum.GeneralSigns
            | InputCategoryEnum.ControlCharCRLF | InputCategoryEnum.ControlCharCRLFNumber | InputCategoryEnum.ControlCharFF | InputCategoryEnum.MusicalHyphenAndSpace
            | InputCategoryEnum.LineContinuation | InputCategoryEnum.PrintPagination | InputCategoryEnum.GuideDots | InputCategoryEnum.OtherValues | InputCategoryEnum.PartMeasureRepeat;

        const InputCategoryEnum allowedInNumberStates = InputCategoryEnum.Digit | InputCategoryEnum.Denominator | InputCategoryEnum.Space | InputCategoryEnum.ToMusicBraille
            | InputCategoryEnum.ToNumber | InputCategoryEnum.ControlCharCRLF | InputCategoryEnum.ControlCharCRLFNumber | InputCategoryEnum.ControlCharFF;

        const InputCategoryEnum allowedInAllChordStates =  InputCategoryEnum.TextVersal  | InputCategoryEnum.ChordTiming | InputCategoryEnum.ChordStemSign | InputCategoryEnum.ChordSymbolRoot
            | InputCategoryEnum.FinalDoubleBar | InputCategoryEnum.Beat | InputCategoryEnum.Hyphen
            | InputCategoryEnum.NewMeasure | InputCategoryEnum.ControlCharCRLF | InputCategoryEnum.ControlCharCRLFNumber | InputCategoryEnum.ControlCharFF
            | InputCategoryEnum.Hand | InputCategoryEnum.SectionHeader | InputCategoryEnum.PrintPagination | InputCategoryEnum.EmbeddedTextRepresentation
            | InputCategoryEnum.OtherValues  | InputCategoryEnum.PartMeasureRepeat | InputCategoryEnum.SectionalDoubleBar; // SectionHeadser probably also in MusicState !



        /// <summary>
        /// This could also be implemented in each derived class, but the switch below is much easier to maintain !!
        /// </summary>
        internal InputCategoryEnum AllowedInputCategories
        {
            get
            {
                switch (state.MyStateEnum)
                {
                    case StateEnum.Text: return allowedInTextStates ;
                    case StateEnum.Chords:     return allowedInAllChordStates  | InputCategoryEnum.Rest | InputCategoryEnum.DaCapoAndDalSegno | InputCategoryEnum.Beat; // Neither Bass, Symbol nor  NumericExtension  nor chordcharacter is allowed  until we have a Root !
                    case StateEnum.ChordsRoot: return allowedInAllChordStates |  InputCategoryEnum.ChordSymbol | InputCategoryEnum.ChordSymbolBass | InputCategoryEnum.ChordCharacter | InputCategoryEnum.ChordNumericExtension | InputCategoryEnum.Beat; // Allow all chord flavoring ! But not Segno
                    case StateEnum.TextVersal: return allowedInTextStates;
                    case StateEnum.TextNumber: return allowedInNumberStates | InputCategoryEnum.TextVersal | InputCategoryEnum.ToMusicBraille | InputCategoryEnum.ToWord | InputCategoryEnum.DigitSpecialCharacter;
                    case StateEnum.MusicNumber: return allowedInNumberStates | InputCategoryEnum.Accidental;
                    case StateEnum.Music: return allowedInMusicStates | InputCategoryEnum.Accidental | InputCategoryEnum.UnusualBarLine | InputCategoryEnum.ToNumber
                            | AnyEnding | InputCategoryEnum.Hand | InputCategoryEnum.Beat | InputCategoryEnum.Articulation | InputCategoryEnum.Chords
                            | InputCategoryEnum.PageNumber | InputCategoryEnum.KeySignature | InputCategoryEnum.RepeatSequence
                            | InputCategoryEnum.EmbeddedTextRepresentation | InputCategoryEnum.Slur | InputCategoryEnum.InAccordTie | InputCategoryEnum.DaCapoAndDalSegno;
                    case StateEnum.MusicNote: return allowedInMusicStates | InputCategoryEnum.Slur | InputCategoryEnum.Tie | InputCategoryEnum.UnusualBarLine | AnyEnding 
                            | InputCategoryEnum.Punctuation | InputCategoryEnum.Interval | InputCategoryEnum.Accidental | InputCategoryEnum.OtherValues | InputCategoryEnum.PartMeasureRepeat | InputCategoryEnum.Finger
                            | InputCategoryEnum.Articulation  | InputCategoryEnum.InAccordTie | InputCategoryEnum.EmbeddedTextRepresentation | InputCategoryEnum.Hand;
                    case StateEnum.TextNumberLowered: return InputCategoryEnum.LoweredDigit | InputCategoryEnum.ToMusicBraille | InputCategoryEnum.Hand
                            | InputCategoryEnum.ControlCharCRLF | InputCategoryEnum.ControlCharCRLFNumber;
                    default: throw new Exception(string.Format("Unsupported state {0} ", state.MyStateEnum.ToString()));
                        return (InputCategoryEnum)0; // To please compiler
                }
            }
        }




        // Simple convenience method
        private bool NewState(StateEnum newState)
        {          
            StateEnum oldStateEnum = state.MyStateEnum;
            state = new DecoderState(newState, this.state);
            this.CountTransition(oldStateEnum, state.MyStateEnum);
            return (state.MyStateEnum != oldStateEnum); 
        }


        /// <summary>
        /// Sets up the new state.
        /// Returns tru4 if the state is changed
        /// </summary>
        /// <param name="inputValue"></param>
        /// <returns></returns>
        internal bool SetNewState(InputInterpretation inputValue)
        {
            previousState = state;
            if (null == inputValue) return false;
            InputCategoryEnum inputCategory = inputValue.Category;

            switch (this.state.MyStateEnum)
            {
                case StateEnum.Text:
                    switch (inputCategory)
                    {
                        case InputCategoryEnum.ToMusicBraille: return NewState(StateEnum.Music); // The formally correct way to enter state Music
                        case InputCategoryEnum.Hand: return NewState(StateEnum.Music); // A shortcut based on the "Hand" symbol, even without the "ToMusicBraille" symbol
                        case InputCategoryEnum.ToNumber: return NewState(StateEnum.TextNumber);
                        case InputCategoryEnum.ToNumberLowered: return NewState(StateEnum.TextNumberLowered);
                        case InputCategoryEnum.TextVersal: return NewState(StateEnum.TextVersal);
                        case InputCategoryEnum.BeatAsText: return NewState(StateEnum.TextNumber); // Leave in same state as if not interpretinr BeatAsText as a separate tokes but as a numeric fraction.
                        case InputCategoryEnum.Chords: return NewState(StateEnum.Chords);
                    }
                    break;

                case StateEnum.Chords: // similar to StateEnum.Text
                    switch (inputCategory)
                    {
                        // The first transitions are identical in Chords and ChordRoot:
                        case InputCategoryEnum.ToMusicBraille: return NewState(StateEnum.Music);
                        case InputCategoryEnum.ToNumber: return NewState(StateEnum.TextNumber);
                        case InputCategoryEnum.Hand: return NewState(StateEnum.Music); // A shortcut based on the "Hand" symbol, even without the "ToMusicBraille" symbol
                        //  The remaining transitions are different in Chords and ChordRoot:
                        case InputCategoryEnum.ChordSymbolRoot: return NewState(StateEnum.ChordsRoot);
                        case InputCategoryEnum.FinalDoubleBar: return NewState(StateEnum.Text);
                    }
                    break;

                case StateEnum.ChordsRoot: 
                    switch (inputCategory)
                    {
                        // The first transitions are identical in Chords and ChordRoot:
                        case InputCategoryEnum.ToMusicBraille: return NewState(StateEnum.Music);
                        case InputCategoryEnum.ToNumber: return NewState(StateEnum.TextNumber);
                        case InputCategoryEnum.Hand: return NewState(StateEnum.Music); // A shortcut based on the "Hand" symbol, even without the "ToMusicBraille" symbol
                        //  The remaining transitions are different in Chords and ChordRoot:
                        case InputCategoryEnum.ChordSymbolBass:
                        case InputCategoryEnum.ChordSymbol:
                        case InputCategoryEnum.ChordNumericExtension:
                        case InputCategoryEnum.ChordCharacter: break; // Keep looking for further details about the current Root
                        case InputCategoryEnum.ChordSymbolRoot:  return NewState(StateEnum.ChordsRoot); // Immediately start looking for further details about this new Root
                        case InputCategoryEnum.FinalDoubleBar: return NewState(StateEnum.Text);
                        default: return NewState(StateEnum.Chords); // We are finished with this root and have not found the next yet.
                    }
                    break;

                case StateEnum.TextVersal:
                    switch (inputCategory)
                    {
                        // Exactly as i  StateEnum.Text except for the default clause
                        case InputCategoryEnum.ToMusicBraille: return NewState(StateEnum.Music);
                        case InputCategoryEnum.Hand: return NewState(StateEnum.Music); // A shortcut based on the "Hand" symbol, even without the "ToMusicBraille" symbol
                        case InputCategoryEnum.ToNumber: return NewState(StateEnum.TextNumber);
                        case InputCategoryEnum.ToNumberLowered: return NewState(StateEnum.TextNumberLowered);
                        case InputCategoryEnum.TextVersal: return NewState(StateEnum.TextVersal);
                        case InputCategoryEnum.Chords: return NewState(StateEnum.Chords);
                        default: state = state.PreviousDecoderState ; return true; // Pop the state active when entering TextVersal. Return true Because the state was changed
                    }
                    break;

                case StateEnum.TextNumber:
                    switch (inputCategory)
                    {
                        case InputCategoryEnum.ToMusicBraille: return NewState(StateEnum.Music);
                        case InputCategoryEnum.Digit:
                        case InputCategoryEnum.Denominator:
                        case InputCategoryEnum.ToNumber:
                        case InputCategoryEnum.DigitSpecialCharacter: break;
                        case InputCategoryEnum.ToNumberLowered: return NewState(StateEnum.TextNumberLowered);
                        case InputCategoryEnum.ControlCharCRLF:
                        case InputCategoryEnum.ControlCharCRLFNumber: return NewState(StateEnum.Text);
                        case InputCategoryEnum.Chords: return NewState(StateEnum.Chords);
                        default: // In this single case the default causes a new state!
                            state = state.PreviousDecoderState; // Pop the state active when entering TextNumber
                            //logger.Log(string.Format("'Returning' to {0}", previousState.ToString()));
                            return true; // true Because the state was changed
                    }
                    break;

                case StateEnum.Music:
                    switch (inputCategory)
                    {
                        case InputCategoryEnum.ToNumber: return NewState(StateEnum.MusicNumber);
                        case InputCategoryEnum.Note: return NewState(StateEnum.MusicNote);
                        case InputCategoryEnum.InsertedRest: return NewState(StateEnum.MusicNote); // MAybe we need a separate state MusicRest accepting punctuations ??
                        case InputCategoryEnum.Rest: return NewState(StateEnum.MusicNote); // MAybe we need a separate state MusicRest accepting punctuations ??
                        case InputCategoryEnum.ToText: return NewState(StateEnum.Text);
                        case InputCategoryEnum.Chords: return NewState(StateEnum.Chords);
                        case InputCategoryEnum.PageNumber: return NewState(StateEnum.MusicNumber);
                        case InputCategoryEnum.FinalDoubleBar: return NewState(StateEnum.Text);
                    }
                    break;

                case StateEnum.MusicNumber:
                    switch (inputCategory)
                    {
                        case InputCategoryEnum.Space: return NewState(StateEnum.Music);
                        case InputCategoryEnum.ControlCharCRLF:
                        case InputCategoryEnum.ControlCharCRLFNumber:
                        case InputCategoryEnum.SectionalDoubleBar:
                            return NewState(StateEnum.Music);
                    }
                    break;

                case StateEnum.MusicNote:
                    switch (inputCategory)
                    {
                        case InputCategoryEnum.ToText: return NewState(StateEnum.Text);
                        case InputCategoryEnum.NewMeasure: return NewState(StateEnum.Music);
                        //case InputCategoryEnum.SectionalDoubleBar: return NewState(StateEnum.Text);
                        case InputCategoryEnum.FinalDoubleBar: return NewState(StateEnum.Text);
                        case InputCategoryEnum.InAccordPartMeasure: return NewState(StateEnum.MusicNote); // No change, probably not needed
                        case InputCategoryEnum.InAccordFullMeasure: return NewState(StateEnum.MusicNote); // No change, probably not needed
                        case InputCategoryEnum.Hand: return NewState(StateEnum.Music);
                        case InputCategoryEnum.ControlCharCRLF: return NewState(StateEnum.Music);
                        case InputCategoryEnum.SectionalDoubleBar: return NewState(StateEnum.Music);
                    }
                    break;

                case StateEnum.TextNumberLowered:
                    switch (inputCategory)
                    {
                        case InputCategoryEnum.ToMusicBraille:
                        case InputCategoryEnum.Hand:
                            return NewState(StateEnum.Music);
                    }
                    break;

            }
            return false; // If we get here the state was not changed.
        }

        /// <summary>
        /// Returns true iff the current input should be shown with the first letter in uppercase ("versal")
        /// </summary>
        public bool ShowAsVersal
        {
            get
            {
                if (this.state.MyStateEnum != StateEnum.Text) return false;
                if (this.previousState.MyStateEnum != StateEnum.TextVersal) return false;
                return true;
            }
        }

    }
  
}
