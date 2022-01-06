using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrailleMusicDecoder
{

    [Flags]
    public enum InputCategoryEnum : ulong // "ulong" enables use of up to 64 bits instead of 31
    {
        // Most commenly used categories are placed first. Yhis may optimize the speed of some calculations.
        None = 0x0,
        Note = 0x0001,
        Character = 0x0002,
        Octave = 0x0004,
        NewMeasure = 0x0008,
        Interval = 0x0010,
        Rest = 0x0020,
        Accidental = 0x0040, 
        Digit = 0x0080,
        TextVersal= 0x0100,
        Hand = 0x0200,
        PartMeasureRepeat = 0x400,
        GeneralSigns = 0x0800,  // BANA2015 Table 1
        OtherValues = 0x1000,
        Denominator = 0x2000,
        Beat = 0x4000,  // Danish "Taktart"
        GuideDots = 0x8000,
        Space = 0x00010000,
        ToNumber= 0x00020000,
        TypeFormIndicator = 0x00040000, // Such as "Bold", "Italic" or "Underlined"
        ControlCharCRLFNumber = 0x00080000, // CR, LF followed by a number (Typically a measure number)
        BeatAsText = 0x00100000,
        SectionHeader = 0x00200000,
        Slur = 0x00400000, // English "slur" "Danish "Legato"  <> "Bindebue"
        UnusualBarLine = 0x00800000,
        //
        FinalDoubleBar = 0x01000000,
        DaCapoAndDalSegno = 0x02000000,
        Punctuation = 0x04000000,
        ToMusicBraille = 0x08000000,
        //
        MeasureDivision = 0x10000000,     // Danish "Skilletegn"   
        InAccordPartMeasure = 0x20000000, // Dansih: "Lille bistemme"
        InAccordFullMeasure = 0x40000000,  // Dansih: "Stor bistemme" 
        KeySignatureText = 0x80000000,
        // 8 zeroes "1 << 32"
        SectionalDoubleBar = 0x100000000,
        ToText = 0x200000000, // Transition from MusicBraille to TextBraille
        Tie = 0x400000000, // Danish "Bindebue" <> "Legato"
        Articulation = 0x800000000,
        // 9 zeroes "1 << 36"
        TimeModification = 0x1000000000,
        Chords = 0x2000000000,
        ChordSymbol = 0x4000000000, // Generic value for all chord symbols that are NEITHER ChordSymbolRoot NOR ChordSymbolBass. May graphically be either a text such as "sus4" or a  graphic symbol such as a circle 
        ChordTiming = 0x8000000000,
        // 10 zeroes "1 << 40"
        ChordNumericExtension = 0x10000000000,
        ChordStemSign = 0x20000000000,
        EmbeddedTextRepresentation = 0x40000000000, // Music Braille represented as text, for instance "p" for "piano" and "ff for "forte"
        DigitSpecialCharacter = 0x80000000000,
        // 11 zeroes "1 << 44"
        PrintPagination = 0x100000000000,
        LineContinuation = 0x200000000000,
        InsertedRest = 0x400000000000,
        InAccordTie = 0x800000000000,
        // 12 zeroes "1 << 48"
        ControlCharCRLF = 0x1000000000000,
        ChordSymbolBass = 0x2000000000000, // The "e" in C/e
        ChordSymbolRoot = 0x4000000000000, // The "C" in C/e
        ToNumberLowered = 0x8000000000000, // For decoding "First MeasureNumber" followed by "Number of measures"
        // 13 zeroes "1 << 52"
        LoweredDigit = 0x10000000000000, // For Number of Measures"
        KeySignature = 0x20000000000000, // Danish "Fast Fortegn"
        RepeatSequence = 0x40000000000000,
        ChordCharacter = 0x80000000000000, 
        // 14 zeroes "1 << 56"
        Finger= 0x100000000000000, 
        NonBrailleCharacter = 0x200000000000000, // CR, LF
        ControlCharFF = 0x400000000000000, // FF, 4 bits remaining !
        MusicalHyphenAndSpace = 0x800000000000000, // 3 bits remaining !
        // 15 zeroes "1 << 60"
        Hyphen = 0x1000000000000000, // 2 bits remaining !
        PageNumber = 0x2000000000000000, // 1 bits remaining !
        Clef = 0x4000000000000000, // 0 bits remaining !  //BANA 2015:28.1.3. Guide Dots   
        ToWord =  0x8000000000000000, // We get this extra value by using ulong instead of long ! And use it for "Note" to be sure it has no strange sideeffects
        AllCategories = 0xffffffffffffffff
    }


    /// <summary>
    /// For further sub-categorizing the inputcategorues. Must be used when the MusicXml representation is more complex than just a string value.
    /// These values can also be used for strings that are NOT to be localized, for instance text strings used for building MusicXml
    /// </summary>
    public enum InputSubCategoryEnum
    {
        None, // Generally used to represent absense of a value
        // Hands
        HandRight,
        HandLeft,
        // Accidentals
        AccidentalSharp,
        AccidentalFlat,
        AccidentalNatural,
        AccidentalCourtesySharp,
        AccidentalCourtesyFlat,
        AccidentalCourtesyNatural,
        ChordSymbolRootNoRoot,
        // Chord kind symbols
        ChordSymbolSus2,
        ChordSymbolSus4,
        ChordSymbolMinor,
        //ChordSymbolMinus,
        //ChordSymbolPlus,
        ChordSymbolDim,
        ChordSymbolHalfDim,
        ChordSymbolAug,
        ChordSymbolMaj, // As i Cmaj7
        // Other Chord symbols
        ChordSymbolNatural,
        ChordSymbolSharp,
        ChordSymbolFlat,
        // Chord numeric extensions
        ChordNumericExtension5,
        ChordNumericExtension5Flat,
        ChordNumericExtension5Sharp,
        ChordNumericExtension6,
        ChordNumericExtension7,
        //ChordNumericExtension7Flat,
        ChordNumericExtension7Sharp,
        ChordNumericExtension9,
        ChordNumericExtension9Flat,
        ChordNumericExtension9Sharp,
        ChordNumericExtension11,
        ChordNumericExtension11Flat,
        ChordNumericExtension11Sharp5,
        ChordNumericExtension13,
        ChordNumericExtension13Flat,
        ChordNumericExtension13Sharp,
        // Clefs
        ClefF,
        ClefG,
        // FullSteps
        FullStepA,
        FullStepB,
        FullStepC,
        FullStepD,
        FullStepE,
        FullStepF,
        FullStepG,
        // Chord stem signs
        ChordStemSign,
        ChordStemSignWithPunctuation,
        ChordStemSignWithDoublePunctuation,
        //Beats
        BeatTypeCommon, // "C"
        BeatTypeCut,      // "C" with a vertical bar inside a.k.a "alla breve"
        BeatFraction,     // A fraction (beats/beattype), for instance (3/4) 
        BeatFractionAndEmptySpace,     // A fraction (beats/beattype), for instance (3/4) and followed by a empty space
        BeatFractionAndCarriageReturn,     // A fraction (beats/beattype), for instance (3/4) and followed by a Carriage Return
        IntervalSecond,
        IntervalThird,
        IntervalFourth,
        IntervalFifth,
        IntervalSixth,
        IntervalSeventh,
        IntervalOctave,
        // More to come
        FirstMeasureIs0, // The first measure is an anacrusis
        FirstMeasureIs1,
        // Punctuations:
        PunctuationSingle,
        PunctuationDouble,
        PunctuationTriple,
        // Texts, embedded in the Music Braille
        TextPiano,
        TextPianoPianissimo,
        TextMezzoPiano,
        TextForte,
        TextForteFortissimo,
        TextMezzoForte,
        TextCrescentoStart,
        TextDiminiuendoStart,
        TextCrescentoEnd,
        TextDiminiuendoEnd,
        TextFreeText,
        UnusualBarLineSpecialBrailleBarline,
        UnusualBarLineSpecialPrintBarline,
        // Square Brackets: BANA 2015 Table 1 "General Signs"
        SquareBracketBelowStaffStart,
        SquareBracketBelowStaffEnd,
        SmallInvertedArchAboveNote,
        PianoPedalDown,
        ControlCharCRLFOneDigit,
        ControlCharCRLFTwoDigits,
        ControlCharCRLFThreeDigits,
        SlurNormal,
        SlurDouble,
        SlurStartBracketSlur,
        SlurEndBracketSlur,
        SlursThatDoNotLeadtoNotes,
        PartMeasureRepeatOnce,
        PartMeasureRepeatTwice,
        OthervaluesFullMeasureRepeat,
        //        OthervaluesFullMeasureRepeatOnce,
        OthervaluesFullMeasureRepeatTwice,
        OthervaluesFullMeasureRepeatNoInitialBlank,
        OtherValuesTrill,
        OtherValuesOrnament,
        OthervaluesShortAppoggiatura, // Danish: "Forslagsnode"
        OthervaluesLongAppoggiatura,  // Danish: "Forslagsnode"
        OthervaluesDoubleBarFollowedByDots,
        OthervaluesDoubleBarPrecededByDots,
        OthervaluesVolta1FirstEnding, // "1" is always first ending
        OthervaluesVolta2SecondEnding, // "2" is always second ending
        OthervaluesVoltaIntervalEnding, // Either first or second ending
        OthervaluesVoltaNumericEnding, // Either first or second ending
        OthervaluesTremoloRepeatedNote,
        OthervaluesTremoloAlternatingNotes,
        ArticulationStaccato,
        ArticulationStaccattissimo,
        ArticulationAccent,
        ArticulationPortato,
        ArticulationPortamento,
        ArticulationTenuto,
        ArticulationStaccatoAccent,
        CharacterExpandedContraction,
        TextVersalSymbol, // The next text symbol is capitalized
        TextVersalWord, // The next text word is capitalized
        TextVersalPassage,  // The next text passage is capitalized
        CharacterTempo2Digits,
        CharacterTempo3Digits,
        CharacterTimeSignature,
        CharacterBlank,
        CharacterBlankSequence,
        CharacterSpecialSequence, // Such as "@" Copyright-sign etc
        KeySignatureDK, // As specified by Refsnæs
        KeySignatureBANA, // As specified in BANA 2015
        TimeModificationFermata,
        TimeModificationTriplet,
        ExtraSpace1, // For general use across categories!
        ExtraSpace2, // For general use across categories!
        ExtraSpace3, // For general use across categories!
        ExtraSpace4, // For general use across categories!
        ExtraSpace5, // For general use across categories!
        TypeFormIndicatorUnderlineStart,
        TypeFormIndicatorUnderlineEnd,
        Bana2015Style,
        NotaStyle,
        DaCapoAndDalSegnoPrintSegno,
        PrintDaCapoOrDC,
        BrailleOnlyDaCapo,
        BrailleOnlySegnoWithLetter,
        BrailleOnlyDalSegnoWithLetter,
        EndOfBrailleOnlySegnoPassage,
        PrintEncircledCrossCodaSign,
        ControlCharCRLFContinued,
        SectionalDoubleBarFollowedByBarline,
        NumberOfSubCategories // The number of values in InputSubCategoryEnum. Must be last !!!
    }


    /// <summary>
    /// A few items need a further subcategirizing, for example for describing the type of a note.
    /// For instance a Braille Music Note is described as
    /// InputCategoryEnum,
    /// InputSubCategoryEnum    for the FullStep ( A,B,C,D,E,F,G )
    /// InputSusSubCategoruEnum for the Type ( 1, 1/2, 1/4, 1/8 ... )  
    /// </summary>
    public enum InputSubSubCategoryEnum
    {
        Unknown = 0,
        // Note types are ambiguous in Music Braille:
        NoteTypeFullMeasureOrWholeOr16th,
        NoteTypeHalfOr32nd,
        NoteTypeQuarterOr64th,
        NoteTypeEighthOr128th,
        NoteTypeFullMeasureOrWholeOr16thDotted,
        NoteTypeHalfOr32ndDotted,
        NoteTypeQuarterOr64thDotted,
        NoteTypeEighthOr128thDotted,

#warning TODo get rid of 16th
        NoteType16th,
        // Maybe more to come
        HandRightIntervalsReadUpward, // BANA 2015 Right hand, intervals read upward (25) 29.2, 30.4
        HandLeftIntervalsReadDownward, // BANA 2015 Right hand, intervals read upward (25) 29.2, 30.4

        // The following values are used across several SubCategories (Intervals, Articulations etc)
        OccursOnce, // Only affects the following note
        OccursTwice,  // Affects several notes: "sticky"
        Tremolo8th,
        Tremolo16th,
        Tremolo32nd,
        Tremolo64nd,
        Tremolo128th,
        KeySignatureCancel,
        NumberOfSubSubCategories // Must be last !!
    }


    /// <summary>
    /// Describe NoteTypes after resolving unambiguity  
    /// </summary>
    public enum UnAmbiguousNoteTypeEnum
    {
        TypeUnknown = 0,
        TypeFullMeasure,
        TypeWhole,
        TypeHalf,
        TypeQuarter,
        TypeEight,
        Type16th,
        Type32nd,
        Type64th,
        Type128th
    }



    enum IntervalNameEnum { None, second, third, fourth, fifth, sixth, seventh, octave }

    enum OtherValuesNameEnum { equality, newline, trill, ornament }


    /// <summary>
    /// Base class for TokenReader class. 
    /// Holds a lot of pulbic definitions and declarations.
    /// </summary>
    class TokenReaderBase
    { 
        // 0 Dots: 1 value
        public const byte noDots = 0;
        // 1 Dot: 6 values
        public const byte dot1 = 0x01;
        public const byte dot2 = 0x02;
        public const byte dot3 = 0x04;
        public const byte dot4 = 0x08;
        public const byte dot5 = 0x10;
        public const byte dot6 = 0x20;
        // public const byte dot7 = 0x40;
        // public const byte dot8 = 0x80;
        // 2 Dots: 15 values (as for the 4 dots missing)
        public const byte dot12 = dot1 | dot2;
        public const byte dot13 = dot1 | dot3;
        public const byte dot14 = dot1 | dot4;
        public const byte dot15 = dot1 | dot5;
        public const byte dot16 = dot1 | dot6;
        public const byte dot23 = dot2 | dot3;
        public const byte dot24 = dot2 | dot4;
        public const byte dot25 = dot2 | dot5;
        public const byte dot26 = dot2 | dot6;
        public const byte dot34 = dot3 | dot4;
        public const byte dot35 = dot3 | dot5;
        public const byte dot36 = dot3 | dot6;
        public const byte dot45 = dot4 | dot5;
        public const byte dot46 = dot4 | dot6;
        public const byte dot56 = dot5 | dot6;
        // 3 Dots:  20 values
        public const byte dot123 = dot1 | dot2 | dot3;
        public const byte dot124 = dot1 | dot2 | dot4;
        public const byte dot125 = dot1 | dot2 | dot5;
        public const byte dot126 = dot1 | dot2 | dot6;
        public const byte dot134 = dot1 | dot3 | dot4;
        public const byte dot135 = dot1 | dot3 | dot5;
        public const byte dot136 = dot1 | dot3 | dot6;
        public const byte dot145 = dot1 | dot4 | dot5;
        public const byte dot146 = dot1 | dot4 | dot6;
        public const byte dot156 = dot1 | dot5 | dot6;
        public const byte dot234 = dot2 | dot3 | dot4;
        public const byte dot235 = dot2 | dot3 | dot5;
        public const byte dot236 = dot2 | dot3 | dot6;
        public const byte dot245 = dot2 | dot4 | dot5;
        public const byte dot246 = dot2 | dot4 | dot6;
        public const byte dot256 = dot2 | dot5 | dot6;
        public const byte dot345 = dot3 | dot4 | dot5;
        public const byte dot346 = dot3 | dot4 | dot6;
        public const byte dot356 = dot3 | dot5 | dot6;
        public const byte dot456 = dot4 | dot5 | dot6;
        // 4 Dots: 15 values (as for the 2 dots missing)
        public const byte dot1234 = dot1 | dot2 | dot3 | dot4;
        public const byte dot1235 = dot1 | dot2 | dot3 | dot5;
        public const byte dot1236 = dot1 | dot2 | dot3 | dot6;
        public const byte dot1245 = dot1 | dot2 | dot4 | dot5;
        public const byte dot1246 = dot1 | dot2 | dot4 | dot6;
        public const byte dot1256 = dot1 | dot2 | dot5 | dot6;
        public const byte dot1345 = dot1 | dot3 | dot4 | dot5;
        public const byte dot1346 = dot1 | dot3 | dot4 | dot6;
        public const byte dot1456 = dot1 | dot4 | dot5 | dot6;
        public const byte dot2345 = dot2 | dot3 | dot4 | dot5;
        public const byte dot2346 = dot2 | dot3 | dot4 | dot6;
        public const byte dot2356 = dot2 | dot3 | dot5 | dot6;
        public const byte dot2456 = dot2 | dot4 | dot5 | dot6;
        public const byte dot3456 = dot3 | dot4 | dot5 | dot6;

        // 5 Dots: 6 values
        public const byte dot12345 = dot1 | dot2 | dot3 | dot4 | dot5;
        public const byte dot12346 = dot1 | dot2 | dot3 | dot4 | dot6;
        public const byte dot12356 = dot1 | dot2 | dot3 | dot5 | dot6;
        public const byte dot12456 = dot1 | dot2 | dot4 | dot5 | dot6;
        public const byte dot13456 = dot1 | dot3 | dot4 | dot5 | dot6;
        public const byte dot23456 = dot2 | dot3 | dot4 | dot5 | dot6;
        // 6 Dots: 1 value (as for the 1 dot missing)
        public const byte dot123456 = dot1 | dot2 | dot3 | dot4 | dot5 | dot6;

        // 2*(1+6+15) + 20 = 64 values ! 

        // Special values, used for encoding control characters. These definitions are local for this application! 
        // Using these definitions the control characters can be handled in the same way as the Braille Characters.
        public const int carriageReturn = 0x100;
        public const int lineFeed = 0x200;
        public const int formFeed = 0x400;

        public const int toVersal = dot6;
        public const int letterA = dot1;
        public const int letterB = dot12;
        public const int letterC = dot14;
        public const int letterD = dot145;
        public const int letterE = dot15;
        public const int letterF = dot124;
        public const int letterG = dot1245;
        public const int AccidentalFlat = dot126;
        public const int AccidentalSharp = dot146;
        public const int AccidentalNatural = dot16;

        public const int slashForward = dot34; // "/" as in "C/e"

        public const int toDigit = dot3456;

        // All digits
        public const int digit0 = dot245;
        public const int digit1 = dot1;
        public const int digit2 = dot12;
        public const int digit3 = dot14;
        public const int digit4 = dot145;
        public const int digit5 = dot15;
        public const int digit6 = dot124;
        public const int digit7 = dot1245;
        public const int digit8 = dot125;
        public const int digit9 = dot24;

        // All digits lowered
        public const int digit0Lowered = dot356;
        public const int digit1Lowered = dot2;
        public const int digit2Lowered = dot23;
        public const int digit3Lowered = dot25;
        public const int digit4Lowered = dot256;
        public const int digit5Lowered = dot26;
        public const int digit6Lowered = dot235;
        public const int digit7Lowered = dot2356;
        public const int digit8Lowered = dot236;
        public const int digit9Lowered = dot35;

        // Octave numbers
#warning todo use these octave symbols for real octaves also !
        public const int octave1 = dot4;
        public const int octave2 = dot45;
        public const int octave3 = dot456;
        public const int octave4 = dot5;
        public const int octave5 = dot46;
        public const int octave6 = dot56;
        public const int octave7 = dot6;

        // Note types for use in text header: Various type values of a "C" note
        public const int noteTypeEightC = dot145;
        public const int noteTypeQuarterC = dot1456;
        public const int noteTypeHalfC = dot1345;
        public const int noteTypeFullC = dot1345;

        // Other values used in text header
        public const int EqualitySymbol = dot2356; // BANA 2015 1.8. "Equals"
        public const int ToDigitSymbol = dot3456;

        // Constants used primarily for handling embedded texts
        public static int musicalHyphen = dot5;
        public static int toWord = dot345;

        // Hands
        public const int Hand = dot345;
        public const int HandRight = dot46;
        public const int HandLeft = dot456;

        // Intervals
        public const int IntervalSecond = dot34;
        public const int IntervalThird = dot346;
        public const int IntervalFourth = dot3456;
        public const int IntervalFifth = dot35;
        public const int IntervalSixth = dot356;
        public const int IntervalSeventh = dot25;
        public const int IntervalOctave = dot36;



        // Some basic, general definitions
        protected const int BrailleBase = 0x2800; // Offset of Braille characters iithin Unicode

        /// <summary>
        /// Default constructor
        /// </summary>
        protected TokenReaderBase()
        { }
    }
}
