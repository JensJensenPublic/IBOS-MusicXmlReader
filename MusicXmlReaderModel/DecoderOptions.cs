using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BrailleMusicDecoder;

namespace MusicXmlReaderModel
{

    /// <summary>
    /// Class for isolating the client application from the BrailleMusicDecoder and letting all control go through the Model
    /// All public members except RawDecoderOptions use enumerations visible to the client application
    /// RawDecoderOptions uses enumerations defined by the Decoder and not all visible to the client applications
    /// </summary>
    public class DecoderOptions
    {

        private RegionalOptionsEnum regionalOptions = RegionalOptionsEnum.Unknown;
        public RegionalOptionsEnum RegionalOptions { get { return regionalOptions; } }
        private BrailleMusicDecoder.RegionalOptionsEnum GetRegionalOptions() { return (BrailleMusicDecoder.RegionalOptionsEnum) regionalOptions; }

        private FormatOptionsEnum formatOptions = FormatOptionsEnum.defaultOptions; // Find the right defsults
        public FormatOptionsEnum FormatOptions { get { return formatOptions; } } 
        public void IncludeSubStrings(FormatOptionsEnum options) { formatOptions |= options; } // For manipulation from client side
        public void ExcludeSubStrings(FormatOptionsEnum options) { formatOptions &= ~options; } // For manipulation from client side
        private BrailleMusicDecoder.StringFormatOptions GetFormatOptions() { return (BrailleMusicDecoder.StringFormatOptions)formatOptions;}

        private CategoryEnum visibleCategories = CategoryEnum.AllCategories; // Find right defaults 
        public CategoryEnum VisibleCategories { get { return visibleCategories; } }
        public void IncludeCategories(CategoryEnum options) { visibleCategories |= options; } // For manipulation from client side
        public void ExcludeCategories(CategoryEnum options) { visibleCategories &= ~options; } // For manipulation from client side
        private BrailleMusicDecoder.InputCategoryEnum GetVisibleCategories() { return (BrailleMusicDecoder.InputCategoryEnum)visibleCategories; }

        private CategoryEnum visibleCategoryNames= CategoryEnum.AllCategories; // Find right defaults 
        public CategoryEnum VisibleCategoryNames { get { return visibleCategoryNames; } }
        public void IncludeCategoryNames(CategoryEnum options) { visibleCategoryNames |= options; } // For manipulation from client side
        public void ExcludeCategoryNames(CategoryEnum options) { visibleCategoryNames &= ~options; } // For manipulation from client side
        private BrailleMusicDecoder.InputCategoryEnum GetVisibleCategoryNames() { return (BrailleMusicDecoder.InputCategoryEnum)visibleCategoryNames; }

        //private Decoder.DevelopmentOptionEnum developmentOptions;
        //public Decoder.DevelopmentOptionEnum DevelopmentOptions { get { return developmentOptions; } set { developmentOptions = value; } }

        private bool developerMode;
        public bool DeveloperMode { get { return developerMode; } }

        /// <summary>
        /// Presents the Decoder options in a sorm suitable for the Decoder, i.e using enumerations only visible to the Decoder
        /// </summary>
        public BrailleMusicDecoder.Options RawDecoderOptions { get { return BrailleMusicDecoder.Options.Create(GetRegionalOptions(), GetFormatOptions(), GetVisibleCategories(),GetVisibleCategoryNames(), developerMode); } }
        
        public enum RegionalOptionsEnum {
            Unknown = BrailleMusicDecoder.RegionalOptionsEnum.UnKnown,
            AutoSelect = BrailleMusicDecoder.RegionalOptionsEnum.AutoSelect,
            English = BrailleMusicDecoder.RegionalOptionsEnum.English,
            Danish  = BrailleMusicDecoder.RegionalOptionsEnum.Danish
        }

        private DecoderOptions(RegionalOptionsEnum regionalOptions, bool developerMode)
        {
            this.regionalOptions = regionalOptions;
            this.developerMode = developerMode;
            CheckDefinitions();

        }

        private static bool definitionsOk = false;
        /// <summary>
        /// If CategoryEnumValues differs from InputCategoryEnumValues the application looses control through the DecoderOptions mechanism and strange errors may occur
        /// So better safe than sorry!
        /// </summary>
        /// <returns></returns>
        private bool CheckDefinitions()
        {
            if (definitionsOk) return true; // Only check once
            // Verify that the enum CategoryEnumValues which is visible to the client contains the same values as InputCategoryEnumValues (Which is by design not visible)
            var CategoryEnumValues = Enum.GetValues(typeof(DecoderOptions.CategoryEnum)); // The definition in this class, visible to the UI client
            var InputCategoryEnumValues = Enum.GetValues(typeof(BrailleMusicDecoder.InputCategoryEnum)); // The definition used in BrailleMusicDecoder.TokenReader.cs. Invisible to the UI client

            if (CategoryEnumValues.Length == InputCategoryEnumValues.Length)
            {
                definitionsOk = true;
                return true;
            }
            string message = string.Format(": ERROR: CategoryEnumValues.Length={0} <> InputCategoryEnumValues.Length={1}", CategoryEnumValues.Length, InputCategoryEnumValues.Length);
            Logger.LogCF(": " + message);

            // In case of an eror we log all the values:
            ulong mask = 0;
            for (int i = 0; (i <= 63); i++)
            {
                string decoderOptionsValue = ((DecoderOptions.CategoryEnum)mask).ToString();
                string tokenReaderEnumValue = ((BrailleMusicDecoder.InputCategoryEnum)(mask)).ToString();
                if (0 == string.Compare(decoderOptionsValue, tokenReaderEnumValue))
                {
                    Logger.LogCF(string.Format(": mask=0x{0:X016} Same value={1} ", mask, decoderOptionsValue));
                }
                else
                {
                    Logger.LogCF(string.Format(": mask=0x{0:X016} Different values: TokenReaderEnumValue={1} DecoderOptionsValue={2} ", mask, tokenReaderEnumValue, decoderOptionsValue));
                }
                mask = ((ulong)1) << i;
            }

            throw (new Exception("MusicXmlReaderModel.DecoderOptions.CheckDefinitions(): " + message));
        }

        /// <summary>
        /// Used by Decoder to control the amount of information reported 
        /// Sample:                     // [134: 1] [1. 6.23] ⠪     Node  'A/(4 eller 64)'  246 
        /// none = 0x01,                // Just a dummy value
        /// token = 0x02,               // ⠪ (Same as rawvalues, but represented as Unicode Braille6 characters 0x2800-0x283F)
        /// categories = 0x04,          //  Node
        /// value = 0x08,               // (Currently not used for generating text representation, only for generating MusicXml
        /// friendlyValue = 0x10,       // 'A/(4 eller 64)'
        /// extraString = 0x20,         // (Not shown in this sample)
        /// pageLinePos = 0x40,         // [1. 6.23] 
        /// // Used by Model
        /// indexAndLength = 0x80,      // [134: 1]
        /// rawValues = 0x100,          // 246 (Same as token, but represented as dot numbers) 
        /// </summary>
        [Flags]
        public enum FormatOptionsEnum
        {
            // The low word reflects the stringformatOptions defined by the BraillemUiscDecoder
            // Used by Decoder to control the amount of information reported 
            // Sample:                  // [134: 1] [1. 6.23] ⠪     Node  'A/(4 eller 64)'  246 
            none = StringFormatOptions.none,    // Just a dummy value
            token = StringFormatOptions.token,  // ⠪ (Same as rawvalues, but represented as Unicode Braille6 characters 0x2800-0x283F)
            categories = StringFormatOptions.categories,  //  Node
            value = StringFormatOptions.value,          // (Currently not used for generating text representation, only for generating MusicXml
            friendlyValue = StringFormatOptions.friendlyValue,       // 'A/(4 eller 64)'
            extraString = StringFormatOptions.extraString,         // (Not shown in this sample)
            apostrophesInValue = StringFormatOptions.apostrophesInValue,
            pageNumber = StringFormatOptions.pageNumber,
            lineNumber = StringFormatOptions.lineNumber,
            spaceNumber = StringFormatOptions.spaceNumber,
            xmlRepresentation = StringFormatOptions.xmlRepresentation,
            // The high 16 bits reflect the formatting options  used by Model
            indexAndLength = 0x40000,      // [134: 1]
            rawValues = 0x80000,          // 246 (Same as token, but represented as dot numbers)
            newBrailleLineNumber = 0x100000,
            rawBrailleLines = 0x200000,
            accumulatedText = 0x400000,
            defaultOptions =
                  token
                | categories
                | value
                | friendlyValue
                | extraString
                | apostrophesInValue
                | pageNumber
                | lineNumber
                | spaceNumber
                | indexAndLength
                | rawValues
                | newBrailleLineNumber
                | rawBrailleLines
                | accumulatedText 
                | xmlRepresentation               
        }


        /// <summary>
        /// For implementing control of formatting of each separate InputCategory WITHOUT exposing the InputCategoryEnum type to the outside
        /// In this way the clients have no need for referencing BrailleMusicDecoder.dll. All control can go through the Model
        /// NOTE: The program will intensionally throw an exception if ihe number of members here does not match the number of members inInputCategoryEnum !!
        /// </summary>
        [Flags]
        public enum CategoryEnum : ulong // "ulong" enables use of up to 64 bits instead of 31
        {
            None = InputCategoryEnum.None,
            ToWord = InputCategoryEnum.ToWord,
            ToNumber = InputCategoryEnum.ToNumber,
            TextVersal = InputCategoryEnum.TextVersal,
            Character = InputCategoryEnum.Character,
            Hand = InputCategoryEnum.Hand,
            PartMeasureRepeat = InputCategoryEnum.PartMeasureRepeat,
            Digit = InputCategoryEnum.Digit,
            Note = InputCategoryEnum.Note,
            Rest = InputCategoryEnum.Rest,
            Octave = InputCategoryEnum.Octave,
            Interval = InputCategoryEnum.Interval,
            Accidental = InputCategoryEnum.Accidental,
            Finger = InputCategoryEnum.Finger,
            OtherValues = InputCategoryEnum.OtherValues,
            Denominator = InputCategoryEnum.Denominator,
            Beat = InputCategoryEnum.Beat,  // Danish "Taktart"
            BeatAsText = InputCategoryEnum.BeatAsText,
            SectionHeader = InputCategoryEnum.SectionHeader,
            Clef = InputCategoryEnum.Clef,
            Space = InputCategoryEnum.Space,
            NewMeasure = InputCategoryEnum.NewMeasure,
            Slur = InputCategoryEnum.Slur, // English "slur" "Danish "Legato"  <> "Bindebue"
            UnusualBarLine = InputCategoryEnum.UnusualBarLine,
            FinalDoubleBar = InputCategoryEnum.FinalDoubleBar,
            DaCapoAndDalSegno = InputCategoryEnum.DaCapoAndDalSegno,
            Punctuation = InputCategoryEnum.Punctuation,
            ToMusicBraille = InputCategoryEnum.ToMusicBraille,
            MeasureDivision = InputCategoryEnum.MeasureDivision,     // Danish "Skilletegn"   
            InAccordPartMeasure = InputCategoryEnum.InAccordPartMeasure, // Dansih: "Lille bistemme"
            InAccordFullMeasure = InputCategoryEnum.InAccordFullMeasure,  // Dansih: "Stor bistemme" 
            SectionalDoubleBar = InputCategoryEnum.SectionalDoubleBar,
            ToText = InputCategoryEnum.ToText, // Transition from MusicBraille to TextBraille
            Tie = InputCategoryEnum.Tie, // Danish "Bindebue" <> "Legato"
            Articulation = InputCategoryEnum.Articulation,
            TimeModification = InputCategoryEnum.TimeModification,
            Chords = InputCategoryEnum.Chords,
            ChordSymbol = InputCategoryEnum.ChordSymbol, // Generic value for all chord symbols that are NEITHER ChordSymbolRoot NOR ChordSymbolBass
            ChordTiming = InputCategoryEnum.ChordTiming,
            ChordNumericExtension = InputCategoryEnum.ChordNumericExtension,
            ChordStemSign = InputCategoryEnum.ChordStemSign,
            ChordCharacter = InputCategoryEnum.ChordCharacter,
            DigitSpecialCharacter = InputCategoryEnum.DigitSpecialCharacter,
            PageNumber = InputCategoryEnum.PageNumber,
            Hyphen = InputCategoryEnum.Hyphen,
            InsertedRest = InputCategoryEnum.InsertedRest,
            InAccordTie = InputCategoryEnum.InAccordTie,
            NonBrailleCharacter = InputCategoryEnum.NonBrailleCharacter,
            ChordSymbolBass = InputCategoryEnum.ChordSymbolBass, // The "E" in C/e
            ChordSymbolRoot = InputCategoryEnum.ChordSymbolRoot, // The "C" in C/e
            ToNumberLowered = InputCategoryEnum.ToNumberLowered, // For decoding "First MeasureNumber" followed by "Number of measures"
            LoweredDigit = InputCategoryEnum.LoweredDigit, // For Number of Measures"
            KeySignature = InputCategoryEnum.KeySignature, // Danish "Fast Fortegn" (When reported within music context)
            KeySignatureText = InputCategoryEnum.KeySignatureText, // Danish "Fast Fortegn" (When reported within text context)
            RepeatSequence = InputCategoryEnum.RepeatSequence,
            EmbeddedTextRepresentation = InputCategoryEnum.EmbeddedTextRepresentation, // Music Braille represented as text, for instance "p" for "piano" and "ff for "forte"
            GeneralSigns = InputCategoryEnum.GeneralSigns,  // BANA2015 Table 1
            ControlCharCRLF = InputCategoryEnum.ControlCharCRLF, // CR, LF
            ControlCharCRLFNumber = InputCategoryEnum.ControlCharCRLFNumber, // CR, LF followed by a number
            ControlCharFF = InputCategoryEnum.ControlCharFF, // FF, 4 bits remaining !
            MusicalHyphenAndSpace = InputCategoryEnum.MusicalHyphenAndSpace, // 3 bits remaining !
            LineContinuation = InputCategoryEnum.LineContinuation, // 2 bits remaining !
            PrintPagination = InputCategoryEnum.PrintPagination, // 1 bits remaining !
            GuideDots = InputCategoryEnum.GuideDots, // 0 bits remaining !  //BANA 2015:28.1.3. Guide Dots
            TypeFormIndicator = InputCategoryEnum.TypeFormIndicator,
            AllCategories = InputCategoryEnum.AllCategories,
        }

        public static DecoderOptions Create(DecoderOptions.RegionalOptionsEnum regionalOptions,bool developerMode)
        {
            return new DecoderOptions(regionalOptions,developerMode);
        }
    }
}
