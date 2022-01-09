using System;
using System.Collections.Generic;
using MusicXmlReaderModel; // Use the namespace, but do not reference MusicXmlReaderModel. Reference MusicXmlReaderModelBase instead to avoid circular references

namespace BrailleMusicDecoder
{
    class TokenReader : TokenReaderBase
    {
        /// <summary>
        /// Returns a list of all POSSIBLE inputinterpretations, (without considering the inputState) of the IntgerList rawValues supplied.
        /// </summary>
        /// <param name="rawValues">The value to get all interpretations of</param>
        /// <returns>Alist of all interpretations found</returns>
        public InputInterpretationList GetInputInterpretations(IntegerList rawValues)
        {
            int thisValue = rawValues.List[0];
            int nextValue = (rawValues.Count > 1) ? rawValues.List[1] : 0x27ff; // An illegal value used near end of the input file

            InputInterpretationList ii = new InputInterpretationList(rawValues); // Becomes the result  

            utils.AddStep(ii, thisValue);
            utils.AddOctave(ii, thisValue, nextValue);
            utils.AddChordSymbol(ii, thisValue);

            // The "-" symbol represents the absanse of a root:
            ii.Add(rawValues, new IntegerList(dot36), InputCategoryEnum.ChordSymbolRoot, "",InputSubCategoryEnum.ChordSymbolRootNoRoot); // The special Root symbol, representing the absense of a root

            // Halfdiminished also has its own symbol, but requires 2 Braille characters
            ii.Add(rawValues, new IntegerList(dot256, dot3), InputCategoryEnum.ChordSymbol, "halfdiminished", InputSubCategoryEnum.ChordSymbolHalfDim);

            // Some chord symbols are not represented by special Music Braille symbols but are spelled out using normel letters and digits:
            const int letterS = (dot234);
            const int letterU = dot136;

            // Sus2 and sus4 are also chordsymbols, but they don't have their own Music Braille  symbol, but are spelled using characters "sus2" and "sus4"
            ii.Add(rawValues, new IntegerList(letterS, letterU, letterS, toDigit, digit2), InputCategoryEnum.ChordSymbol, "sus2", InputSubCategoryEnum.ChordSymbolSus2);
            ii.Add(rawValues, new IntegerList(letterS, letterU, letterS, toDigit, digit4), InputCategoryEnum.ChordSymbol, "sus4", InputSubCategoryEnum.ChordSymbolSus4);
            ii.Add(rawValues, new IntegerList(letterS, letterU, letterS), InputCategoryEnum.ChordSymbol, "sus", InputSubCategoryEnum.ChordSymbolSus4); // Use Sus4 as default when no number is specified !
            const int letterM = dot134;
            ii.Add(rawValues, new IntegerList(letterM), InputCategoryEnum.ChordSymbol, "minor", InputSubCategoryEnum.ChordSymbolMinor);

            utils.AddRoots(ii, rawValues);
            utils.AddBasses(ii, rawValues);
            utils.AddRests(ii, thisValue);

            // Add 3 accidentals
            ii.Add(rawValues, new IntegerList(AccidentalSharp), InputCategoryEnum.Accidental, "", InputSubCategoryEnum.AccidentalSharp); // Danish "Løst kryds"
            ii.Add(rawValues, new IntegerList(AccidentalFlat), InputCategoryEnum.Accidental, "", InputSubCategoryEnum.AccidentalFlat); // Danish "Løst b"
            ii.Add(rawValues, new IntegerList(AccidentalNatural), InputCategoryEnum.Accidental, "", InputSubCategoryEnum.AccidentalNatural); // Danish "Opløsningstegn"
            // Add 3 courtesy accidentals. Each consists of the accidental symbol, bracketed with (dot6 , dot3) . Note: The sequence dot6 , dot3 is identical to "Start Music Braille"
            ii.Add(rawValues, new IntegerList(dot6, dot3, AccidentalSharp, dot6, dot3), InputCategoryEnum.Accidental, "", InputSubCategoryEnum.AccidentalCourtesySharp); // Danish "HJælpefortegn"
            ii.Add(rawValues, new IntegerList(dot6, dot3, AccidentalFlat, dot6, dot3), InputCategoryEnum.Accidental, "", InputSubCategoryEnum.AccidentalCourtesyFlat); // Danish "HJælpefortegn"
            ii.Add(rawValues, new IntegerList(dot6, dot3, AccidentalNatural, dot6, dot3), InputCategoryEnum.Accidental, "", InputSubCategoryEnum.AccidentalCourtesyNatural); // Danish "Hælpefortegn"


            utils.AddFingers(ii, thisValue);
            utils.AddIntervals(ii, thisValue);

            const InputSubSubCategoryEnum twice = InputSubSubCategoryEnum.OccursTwice; // Simple shorthand
            ii.Add(rawValues, new IntegerList(IntervalFifth, IntervalFifth), InputCategoryEnum.Interval, "", InputSubCategoryEnum.IntervalFifth, twice);
            ii.Add(rawValues, new IntegerList(IntervalFourth, IntervalFourth), InputCategoryEnum.Interval, "", InputSubCategoryEnum.IntervalFourth, twice);
            ii.Add(rawValues, new IntegerList(IntervalSecond, IntervalSecond), InputCategoryEnum.Interval, "", InputSubCategoryEnum.IntervalSecond, twice);
            ii.Add(rawValues, new IntegerList(IntervalSeventh, IntervalSeventh), InputCategoryEnum.Interval, "", InputSubCategoryEnum.IntervalSeventh, twice);
            ii.Add(rawValues, new IntegerList(IntervalSixth, IntervalSixth), InputCategoryEnum.Interval, "", InputSubCategoryEnum.IntervalSixth, twice);
            ii.Add(rawValues, new IntegerList(IntervalThird, IntervalThird), InputCategoryEnum.Interval, "", InputSubCategoryEnum.IntervalThird, twice);
            ii.Add(rawValues, new IntegerList(IntervalOctave, IntervalOctave), InputCategoryEnum.Interval, "", InputSubCategoryEnum.IntervalOctave, twice);

            // A lot of tokens used for MeasureBivision, InAccordPartMeasure, InAccordFullMeasure etc..
            // IMPORTANT HACK: Placed before OthervaluesTremoloAlternatingNotes to prfere Measuredivision. Both are DOT46 DOT13
#warning TODO Find a real solution to avoid ambuguity between MeasureDivision and OthervaluesTremoloAlternatingNotes
            ii.Add(rawValues, new IntegerList(dot46, dot13), InputCategoryEnum.MeasureDivision);
            ii.Add(rawValues, new IntegerList(dot5, dot2), InputCategoryEnum.InAccordPartMeasure);
            // InAccordFullMeasure may be preceeded by one or two empty spaces, which should NOT be misinterpreted as new Measures
            ii.Add(rawValues, new IntegerList(dot126, dot345), InputCategoryEnum.InAccordFullMeasure, "");
            ii.Add(rawValues, new IntegerList(noDots, dot126, dot345), InputCategoryEnum.InAccordFullMeasure, "BLANK+StorBistemme"); // "Eat" a single space in front of InAccordFullMeasure
            ii.Add(rawValues, new IntegerList(noDots, noDots, dot126, dot345), InputCategoryEnum.InAccordFullMeasure, "BLANK*2+StorBistemme"); // "Eat" a double space in front of InAccordFullMeasure
#warning: Find a real solution for crossing hands !   The solution below is a HACK for "Four piano blues"
            // In "Four piano blues" a single left handnote is shown in the graphics note for the right hand and marked with "l.h."
            // In the MusicBraille file this is converted to InAccordPartMeasure followed by Hand.Left. We simply ignogr Hand.Left:
            ii.Add(rawValues, new IntegerList((dot5), dot2, (dot456), dot345), InputCategoryEnum.InAccordPartMeasure, "InAccordPartMeasure ignoring Lefthand");


            utils.AddPartMeasureRepeat(ii, thisValue);

            utils.AddOtherValues(ii, thisValue);

            InputCategoryEnum OtherValues = InputCategoryEnum.OtherValues;
            // InputSubCategoryEnum.OthervaluesLongAppoggiatura requires 2 values:
            ii.Add(rawValues, new IntegerList(dot5, (dot26)), OtherValues, "", InputSubCategoryEnum.OthervaluesLongAppoggiatura); // BANA 2015: Table 16. Ornaments
            // Print-repeats  
            ii.Add(rawValues, new IntegerList(dot126, dot2356), OtherValues, "", InputSubCategoryEnum.OthervaluesDoubleBarFollowedByDots); // BANA 2015: Table 17. Print repeats  
            ii.Add(rawValues, new IntegerList(dot126, dot23), OtherValues, "", InputSubCategoryEnum.OthervaluesDoubleBarPrecededByDots); // BANA 2015: Table 17. Print repeats  
            ii.Add(rawValues, new IntegerList(dot3456, dot2), OtherValues, "", InputSubCategoryEnum.OthervaluesVolta1FirstEnding); // BANA 2015: Table 17. Print repeats  
            ii.Add(rawValues, new IntegerList(dot3456, dot2, dot3), OtherValues, "", InputSubCategoryEnum.OthervaluesVolta1FirstEnding); // BANA 2015: Table 17. Print repeats  
            ii.Add(rawValues, new IntegerList(dot3456, dot23), OtherValues, "", InputSubCategoryEnum.OthervaluesVolta2SecondEnding); // BANA 2015: Table 17. Print repeats  
            ii.Add(rawValues, new IntegerList(dot3456, dot23, dot3),OtherValues, "", InputSubCategoryEnum.OthervaluesVolta2SecondEnding); // BANA 2015: Table 17. Print repeats  
                                                                                                                                                             // TremoloRepeatedNote
            InputSubCategoryEnum TremoloRepeatedNote = InputSubCategoryEnum.OthervaluesTremoloRepeatedNote;
            ii.Add(rawValues, new IntegerList(dot45, dot123), OtherValues, "", TremoloRepeatedNote,InputSubSubCategoryEnum.Tremolo16th); // BANA 2015: 14.2, Table 14
            ii.Add(rawValues, new IntegerList(dot45, dot12), OtherValues, "", TremoloRepeatedNote, InputSubSubCategoryEnum.Tremolo8th); // BANA 2015: 14.2, Table 14
            ii.Add(rawValues, new IntegerList(dot45, dot2), OtherValues, "", TremoloRepeatedNote, InputSubSubCategoryEnum.Tremolo32nd); // BANA 2015: 14.2, Table 14
            ii.Add(rawValues, new IntegerList(dot45, dot13), OtherValues, "", TremoloRepeatedNote, InputSubSubCategoryEnum.Tremolo64nd); // BANA 2015: 14.2, Table 14
            ii.Add(rawValues, new IntegerList(dot45, (dot3)), OtherValues, "", TremoloRepeatedNote, InputSubSubCategoryEnum.Tremolo128th); // BANA 2015: 14.2, Table 14



            InputSubCategoryEnum TremoloAlternatingNotes = InputSubCategoryEnum.OthervaluesTremoloAlternatingNotes;
            ii.Add(rawValues, new IntegerList(dot46, dot123), OtherValues, "", TremoloAlternatingNotes, InputSubSubCategoryEnum.Tremolo16th); // BANA 2015: 14.2, Table 14
            ii.Add(rawValues, new IntegerList(dot46, dot12), OtherValues, "", TremoloAlternatingNotes, InputSubSubCategoryEnum.Tremolo8th); // BANA 2015: 14.2, Table 14
            ii.Add(rawValues, new IntegerList(dot46, dot2), OtherValues, "", TremoloAlternatingNotes, InputSubSubCategoryEnum.Tremolo32nd); // BANA 2015: 14.2, Table 14
            ii.Add(rawValues, new IntegerList(dot46, (dot3)), OtherValues, "", TremoloAlternatingNotes, InputSubSubCategoryEnum.Tremolo128th); // BANA 2015: 14.2, Table 14
            // NOTE: InputSubCategoryEnum.OthervaluesTremoloAlternatingNotes.Tremolo64nd is temporarily disabled because it conflicts with MeasureDivision.
            // A valid solution will have to check against the duration of the previous note or chord
#if false
            ii.Add(rawValues, new IntegerList(dot46, dot13), OtherValues, "", TremoloAlternatingNotes, InputSubSubCategoryEnum.Tremolo64nd); // BANA 2015: 14.2, Table 14
#endif

            const int repeat = dot2356;
            ii.Add(rawValues, new IntegerList(repeat, repeat), InputCategoryEnum.PartMeasureRepeat, "2", InputSubCategoryEnum.PartMeasureRepeatTwice); // BANA 2015 Measure or part-measure repeat (18) 18.1–18.5
            // FullMeasureRepeat
            //            allInputInterpretations.Add(rawValues, new IntegerList(repeat, noDots), InputCategoryEnum.OtherValues, "", InputSubCategoryEnum.OthervaluesFullMeasureRepeatNoInitialBlank); // BANA 2015: 18.2. Full-Measure Repeats
            ii.Add(rawValues, new IntegerList(noDots, repeat), OtherValues, "1", InputSubCategoryEnum.OthervaluesFullMeasureRepeat); // BANA 2015: 18.2. Full-Measure Repeats
                                                                                                                                                                                    //allInputInterpretations.Add(rawValues, new IntegerList(repeat, noDots), InputCategoryEnum.OtherValues, "1", InputSubCategoryEnum.OthervaluesFullMeasureRepeat); // BANA 2015: 18.2. Full-Measure Repeats
            List<int> repeatNoDots = new List<int>() { repeat, noDots };
            List<int> noDotsRepeatNoDots = new List<int>() { noDots, repeat, noDots };
            List<int> noDotsList = new List<int>() { noDots };

            ii.Add(rawValues, new IntegerList(new List<int>() { noDots, repeat, noDots, dot3, dot3, dot3, dot3, dot3, noDots, repeat }), InputCategoryEnum.OtherValues, "NRN.....NR", InputSubCategoryEnum.OthervaluesFullMeasureRepeatTwice); // BANA 2015: 18.2. Full-Measure Repeats

            utils.AddFullMeasureRepeatTwice(rawValues, ii, repeatNoDots, noDotsRepeatNoDots);

            // Add some simple one-character tokens if present
            ii.Add(thisValue, dot3456, InputCategoryEnum.ToNumber);
            ii.Add(thisValue, dot345, InputCategoryEnum.ToWord);
            //            allInputInterpretations.Add(thisValue, dot14, InputCategoryEnum.Slur,"Normal slur",InputSubCategoryEnum.SlurNormal); // BANA 2015: 13.2. Short Slurred Phrases
            ii.Add(rawValues, new IntegerList(dot14), InputCategoryEnum.Slur, "", InputSubCategoryEnum.SlurNormal, InputSubSubCategoryEnum.OccursOnce);  // BANA 2015: 13.3.Longer Slurred Phrases
            ii.Add(rawValues, new IntegerList(dot14, dot14), InputCategoryEnum.Slur, "", InputSubCategoryEnum.SlurNormal, InputSubSubCategoryEnum.OccursTwice);  // BANA 2015: 13.3.Longer Slurred Phrases
            ii.Add(rawValues, new IntegerList(dot45, dot23), InputCategoryEnum.Slur, "", InputSubCategoryEnum.SlurEndBracketSlur);  // BANA 2015: 13.3.Longer Slurred Phrases
            ii.Add(rawValues, new IntegerList(dot56, dot12), InputCategoryEnum.Slur, "", InputSubCategoryEnum.SlurStartBracketSlur);  // BANA 2015: 13.3.Longer Slurred Phrases
            ii.Add(rawValues, new IntegerList(dot56, dot14), InputCategoryEnum.Slur, "", InputSubCategoryEnum.SlursThatDoNotLeadtoNotes);  // BANA 2015: 13.10.2. Slurs That Do Not Lead to Notes
    
            ii.Add(thisValue, noDots, InputCategoryEnum.Space);
            ii.Add(thisValue, noDots, InputCategoryEnum.NewMeasure);
            ii.Add(thisValue, dot123, InputCategoryEnum.UnusualBarLine, "", InputSubCategoryEnum.UnusualBarLineSpecialBrailleBarline); // BANA 2015 1.10.2. "Special Braille Bar Line"

            ii.Add(rawValues, new IntegerList(noDots, dot13, noDots), InputCategoryEnum.UnusualBarLine, "", InputSubCategoryEnum.UnusualBarLineSpecialPrintBarline);// BANA 2015 1.10.1 "Special print Barline"
            ii.Add(rawValues, new IntegerList(noDots, dot13), InputCategoryEnum.UnusualBarLine, "", InputSubCategoryEnum.UnusualBarLineSpecialPrintBarline);// BANA 2015 1.10.1 "Special print Barline" (At end of line!)
            // Double- and triple- punctuation use 2 or 3 characters:
            ii.Add(rawValues, new IntegerList(dot3), InputCategoryEnum.Punctuation, "", InputSubCategoryEnum.PunctuationSingle);
            ii.Add(rawValues, new IntegerList(dot3, dot3), InputCategoryEnum.Punctuation, "", InputSubCategoryEnum.PunctuationDouble);
            ii.Add(rawValues, new IntegerList(dot3, dot3, dot3), InputCategoryEnum.Punctuation, "", InputSubCategoryEnum.PunctuationTriple);

            //if ((thisValue == dot6) && (nextValue != dot3)) // Avoid clash with ToMusic
            //{
            //    allInputInterpretations.Add(thisValue, InputCategoryEnum.TextVersal);
            //}

            utils.AddDigits(ii, thisValue);
            utils.AddLoweredDigits(ii, thisValue);
            ii.Add(thisValue, dot34, InputCategoryEnum.DigitSpecialCharacter, "/",InputSubCategoryEnum.None);  // Used for descrbing year intervals : 1962/63
            ii.Add(thisValue, dot25, InputCategoryEnum.Hyphen,"-",InputSubCategoryEnum.None);
            utils.AddDenominator(ii, thisValue);
            utils.AddTextCharacter(ii, thisValue);
            utils.AddChordCharacter(ii, thisValue);

            // The tie, Danish "Bindebue". Differs from the slur, Danish "Legatobue" 
            ii.Add(rawValues, new IntegerList(dot4, dot14), InputCategoryEnum.Tie); // Refsnæs chapter 4

            // Tie, used during InAccord notation. NOTE Is identical to the "C" BeatType!
            ii.Add(rawValues, new IntegerList(dot46, dot14), InputCategoryEnum.InAccordTie, "");

            // Sectional Double bar and Final Double bar. NOTE: Final Double bar triggers a state transition into state "text". Sectional Double bar is just a bar with a slightly diferent graphis representation.
            ii.Add(rawValues, new IntegerList(dot126, dot13), InputCategoryEnum.FinalDoubleBar, ""); // Bana 2015 1.10.3 "Final double bar" Refsnæs 1, Chapter 6
            ii.Add(rawValues, new IntegerList(dot126, dot13, dot5), InputCategoryEnum.FinalDoubleBar, " + " + ResourcesForBrailleMusicDecoder.InputValueFullEnd_Reference); //Bana 2015 1.10.3 "Final double bar" Refsnæs 1, Chapter 6f and Chapter 6b
            ii.Add(rawValues, new IntegerList(dot126, dot13, dot3), InputCategoryEnum.SectionalDoubleBar, "");  // Bana 2015 1.10.3 "Sectional double bar" Refsnæs 1, Chapter 6e
            // Some transscrbers use an extra (and confusing) noDots AFTER the SectionalDoublebar. The following line "eats" that if found
            ii.Add(rawValues, new IntegerList(dot126, dot13, dot3,noDots), InputCategoryEnum.SectionalDoubleBar, "",InputSubCategoryEnum.SectionalDoubleBarFollowedByBarline);  // Bana 2015 1.10.3 "Sectional double bar" Refsnæs 1, Chapter 6e

            // Various items
            //            allInputInterpretations.Add(rawValues, new IntegerList(dot126, dot23), InputCategoryEnum.EndRepeat);
            ii.Add(rawValues, new IntegerList(dot6), InputCategoryEnum.TextVersal, "", InputSubCategoryEnum.TextVersalSymbol);
            ii.Add(rawValues, new IntegerList(dot6, dot6), InputCategoryEnum.TextVersal, "", InputSubCategoryEnum.TextVersalWord);
            ii.Add(rawValues, new IntegerList(dot6, dot6, dot6), InputCategoryEnum.TextVersal, "", InputSubCategoryEnum.TextVersalPassage);
            ii.Add(rawValues, new IntegerList(dot6, dot3), InputCategoryEnum.ToMusicBraille); // In non-music contexts this symbol also means "TextVersalTerminator"

            ii.Add(rawValues, new IntegerList(dot25, dot345), InputCategoryEnum.Chords, "");         // Refsnæs 2. page 39. Not described by BANA 2015!
            ii.Add(rawValues, new IntegerList(dot25, dot345, dot3), InputCategoryEnum.Chords, "+.");

            // Clefs (Use BANA 2015 definitions. See comments in BRailleBuilder.cs)
            ii.Add(rawValues, new IntegerList(dot345, dot34, dot123), InputCategoryEnum.Clef, "G", InputSubCategoryEnum.ClefG);
            ii.Add(rawValues, new IntegerList(dot345, dot34, dot123, dot3), InputCategoryEnum.Clef, "G.", InputSubCategoryEnum.ClefG);
            ii.Add(rawValues, new IntegerList(dot345, dot3456, dot123), InputCategoryEnum.Clef, "F", InputSubCategoryEnum.ClefF);
            ii.Add(rawValues, new IntegerList(dot345, dot3456, dot123, dot3), InputCategoryEnum.Clef, "F.", InputSubCategoryEnum.ClefF);
            // Clefs, preceeded by an empty space: Repeat the 4 lines above, preceeded with noDots. This prevents misinterpretation as "NewMeasure" followes by "Text"
            ii.Add(rawValues, new IntegerList(noDots, dot345, dot34, dot123), InputCategoryEnum.Clef, "G", InputSubCategoryEnum.ClefG);
            ii.Add(rawValues, new IntegerList(noDots, dot345, dot34, dot123, dot3), InputCategoryEnum.Clef, "G.", InputSubCategoryEnum.ClefG);
            ii.Add(rawValues, new IntegerList(noDots, dot345, dot3456, dot123), InputCategoryEnum.Clef, "F", InputSubCategoryEnum.ClefF);
            ii.Add(rawValues, new IntegerList(noDots, dot345, dot3456, dot123, dot3), InputCategoryEnum.Clef, "F.", InputSubCategoryEnum.ClefF);


            //Hands. (In some cases  a dot3 was appended to the Hand symbol (RefsNæs II blackpage 4)
            string stringRight = ResourcesForBrailleMusicDecoder.InputValueHand_Right; // NOTE: The part-name is needed by the MUSICXML generator
            string stringLeft = ResourcesForBrailleMusicDecoder.InputValueHand_Left; ; // NOTE: The part-name is needed by the MUSICXML generator
            ii.Add(rawValues, new IntegerList(HandRight, Hand), InputCategoryEnum.Hand, stringRight, InputSubCategoryEnum.HandRight);
            ii.Add(rawValues, new IntegerList(HandRight, Hand, Hand), InputCategoryEnum.Hand, stringRight + "*** ReadUpward ***", InputSubCategoryEnum.HandRight,InputSubSubCategoryEnum.HandRightIntervalsReadUpward);
            ii.Add(rawValues, new IntegerList(HandRight, Hand, dot3), InputCategoryEnum.Hand, stringRight, InputSubCategoryEnum.HandRight, "."); // Dot 3 appended
            ii.Add(rawValues, new IntegerList(HandLeft, Hand), InputCategoryEnum.Hand, stringLeft, InputSubCategoryEnum.HandLeft);
            ii.Add(rawValues, new IntegerList(HandLeft, Hand,Hand), InputCategoryEnum.Hand, stringLeft + "*** ReadDownWard ***", InputSubCategoryEnum.HandLeft,InputSubSubCategoryEnum.HandLeftIntervalsReadDownward);
            ii.Add(rawValues, new IntegerList(HandLeft, Hand, dot3), InputCategoryEnum.Hand, stringLeft, InputSubCategoryEnum.HandLeft, "."); // Dot 3 appended
            // "ToMusicBraille" followed by a Hand symbol is decoded as a simple token                                                                                                                              //
            ii.Add(rawValues, new IntegerList(dot6, dot3, HandRight, Hand), InputCategoryEnum.ToMusicBraille, stringRight, InputSubCategoryEnum.HandRight);
            ii.Add(rawValues, new IntegerList(dot6, dot3, HandRight, Hand, Hand), InputCategoryEnum.ToMusicBraille, stringRight + "*** ReadUpward ***", InputSubCategoryEnum.HandRight, InputSubSubCategoryEnum.HandRightIntervalsReadUpward);
            ii.Add(rawValues, new IntegerList(dot6, dot3, HandRight, Hand, dot3), InputCategoryEnum.ToMusicBraille, stringRight, InputSubCategoryEnum.HandRight, "."); // Dot 3 appended
            ii.Add(rawValues, new IntegerList(dot6, dot3, HandLeft, Hand), InputCategoryEnum.ToMusicBraille, stringLeft, InputSubCategoryEnum.HandLeft);
            ii.Add(rawValues, new IntegerList(dot6, dot3, HandLeft, Hand, Hand), InputCategoryEnum.ToMusicBraille, stringLeft + "*** ReadDownWard ***", InputSubCategoryEnum.HandLeft, InputSubSubCategoryEnum.HandLeftIntervalsReadDownward);
            ii.Add(rawValues, new IntegerList(dot6, dot3, HandLeft, Hand, dot3), InputCategoryEnum.ToMusicBraille, stringLeft, InputSubCategoryEnum.HandLeft, "."); // Dot 3 appended
            // Also when followed by carriagereturn,linefeed
            ii.Add(rawValues, new IntegerList(dot6, dot3, carriageReturn, lineFeed,  HandRight, Hand), InputCategoryEnum.ToMusicBraille, stringRight, InputSubCategoryEnum.HandRight);
            ii.Add(rawValues, new IntegerList(dot6, dot3, carriageReturn, lineFeed, HandRight, Hand, Hand), InputCategoryEnum.ToMusicBraille, stringRight + "*** ReadUpward ***", InputSubCategoryEnum.HandRight, InputSubSubCategoryEnum.HandRightIntervalsReadUpward);
            ii.Add(rawValues, new IntegerList(dot6, dot3, carriageReturn, lineFeed, HandRight, Hand, dot3), InputCategoryEnum.ToMusicBraille, stringRight, InputSubCategoryEnum.HandRight, "."); // Dot 3 appended
            ii.Add(rawValues, new IntegerList(dot6, dot3, carriageReturn, lineFeed, HandLeft, Hand), InputCategoryEnum.ToMusicBraille, stringLeft, InputSubCategoryEnum.HandLeft);
            ii.Add(rawValues, new IntegerList(dot6, dot3, carriageReturn, lineFeed, HandLeft, Hand, Hand), InputCategoryEnum.ToMusicBraille, stringLeft + "*** ReadDownWard ***", InputSubCategoryEnum.HandLeft, InputSubSubCategoryEnum.HandLeftIntervalsReadDownward);
            ii.Add(rawValues, new IntegerList(dot6, dot3, carriageReturn, lineFeed, HandLeft, Hand, dot3), InputCategoryEnum.ToMusicBraille, stringLeft, InputSubCategoryEnum.HandLeft, "."); // Dot 3 appended






            MusicBrailleMapper m = musicBrailleMapper; // Establish a local shorthand for better readability
            ii.Add(rawValues, new List<MusicBrailleMappingList>() { m.AllIntegersEmpty, m.HandLeft, m.Hand }, InputCategoryEnum.Hand, InputSubCategoryEnum.HandLeft); // Maybe "Marginal measure number ?
            // Same, but followed by dot3. "HACK for FourPiano Blues position 148, 1717,898,1849 "); // Maybe "Marginal measure number ?
            ii.Add(rawValues, new List<MusicBrailleMappingList>() { m.AllIntegersEmpty, m.HandLeft, m.Hand, m.Dot3List }, InputCategoryEnum.Hand, InputSubCategoryEnum.HandLeft); // Maybe "Marginal measure number ?


            // Note: The first block of 3 interpretations is used for interpreting informal Text Braille beat/beattype sprcifications found as simple text in the Title part and without a termonating noDots
            // Beats: Note: the first fraction (for instance "C") is for UI and debugging purposes, the second fraction (for instance "4/4") is for the StateMachine!
            // 3 lines for interpreting INFORMAL Music Braille beat/beattype sprcifications found in Title and not nessecarily containing terminating noDots or carriagereturn
            ii.Add(rawValues, new IntegerList(dot46, dot14), InputCategoryEnum.BeatAsText, "C", InputSubCategoryEnum.BeatTypeCommon, "4/4"); // NOTE: Identical to "InAccordTie" !!
            ii.Add(rawValues, new IntegerList(dot456, dot14), InputCategoryEnum.Beat, "alla breve", InputSubCategoryEnum.BeatTypeCut, "2/2");
            utils.AddNumericBeatTypeAsText(rawValues, ii); // Explicit N/N sucn as 3/4
            // 3 lines for interpreting FORMAL Music Braille beat/beattype sprcifications found in a part and containing terminating noDots or carriagereturn
            // BANA 2015 7.1. Meter Signatures: "A meter signature(time signature) is preceded (unless it immediately follows a key signature) and followed by blank spaces."
            // This rule allows for differntiating  BeatTimeCommon from "InAccordTie"
            // Following this rule we omit the time signatures which are not followed by an empty space "noDots" 
            ii.Add(rawValues, new IntegerList(dot46, dot14, noDots), InputCategoryEnum.Beat, "C", InputSubCategoryEnum.BeatTypeCommon, "4/4"); // NOTE: Identical to "InAccordTie" !!
            ii.Add(rawValues, new IntegerList(dot456, dot14, noDots), InputCategoryEnum.Beat, "alla breve", InputSubCategoryEnum.BeatTypeCut, "2/2");
            utils.AddNumericBeatType(rawValues, ii); // Explicit N/N sucn as 3/4

            const int Articulation = dot236;
            // Articulations: Note: All articulations end by (dot236) NOTE: Probably no need to localize these latin terms !!
            ii.Add(rawValues, new IntegerList(Articulation), InputCategoryEnum.Articulation, ResourcesForBrailleMusicDecoder.InputValueArticulation_Staccato, InputSubCategoryEnum.ArticulationStaccato,InputSubSubCategoryEnum.OccursOnce);
            ii.Add(rawValues, new IntegerList(Articulation,Articulation), InputCategoryEnum.Articulation, ResourcesForBrailleMusicDecoder.InputValueArticulation_Staccato, InputSubCategoryEnum.ArticulationStaccato,InputSubSubCategoryEnum.OccursTwice);
            ii.Add(rawValues, new IntegerList(dot6, Articulation), InputCategoryEnum.Articulation, ResourcesForBrailleMusicDecoder.InputValueArticulation_Staccatissimo, InputSubCategoryEnum.ArticulationStaccattissimo);
            ii.Add(rawValues, new IntegerList(dot46, Articulation), InputCategoryEnum.Articulation, ResourcesForBrailleMusicDecoder.InputValueArticulation_Accent, InputSubCategoryEnum.ArticulationAccent);
            ii.Add(rawValues, new IntegerList(dot236, dot46, Articulation), InputCategoryEnum.Articulation, ResourcesForBrailleMusicDecoder.InputValueArticulation_Staccato + " + " + ResourcesForBrailleMusicDecoder.InputValueArticulation_Accent, InputSubCategoryEnum.ArticulationStaccatoAccent);
            ii.Add(rawValues, new IntegerList(dot456, Articulation), InputCategoryEnum.Articulation, ResourcesForBrailleMusicDecoder.InputValueArticulation_Tenuto, InputSubCategoryEnum.ArticulationTenuto);
            ii.Add(rawValues, new IntegerList(dot5, Articulation), InputCategoryEnum.Articulation, ResourcesForBrailleMusicDecoder.InputValueArticulation_Portamento, InputSubCategoryEnum.ArticulationPortamento);
            ii.Add(rawValues, new IntegerList(dot14, Articulation), InputCategoryEnum.Articulation, ResourcesForBrailleMusicDecoder.InputValueArticulation_Portato, InputSubCategoryEnum.ArticulationPortato);
            // Maybe Arpeggio is not an articulation ??
            ii.Add(rawValues, new IntegerList(dot345, dot13), InputCategoryEnum.Articulation, ResourcesForBrailleMusicDecoder.InputValueArticulation_ArpeggioUp);
            ii.Add(rawValues, new IntegerList(dot345, dot13, dot13), InputCategoryEnum.Articulation, ResourcesForBrailleMusicDecoder.InputValueArticulation_ArpeggioDown);

            // Time modifications
            ii.Add(rawValues, new IntegerList(dot23), InputCategoryEnum.TimeModification, ResourcesForBrailleMusicDecoder.InputValueTimeModification_Triplet, InputSubCategoryEnum.TimeModificationTriplet);
            ii.Add(rawValues, new IntegerList(dot126, dot123), InputCategoryEnum.TimeModification,  ResourcesForBrailleMusicDecoder.InputValueTimeModification_Fermata,  InputSubCategoryEnum.TimeModificationFermata);

            // Commercial at "@"
            ii.Add(rawValues, new IntegerList(dot45, dot1), InputCategoryEnum.Character, "@");

            ii.Add(rawValues, new IntegerList(dot56, dot23), InputCategoryEnum.ToText);

#warning TODO add multicharacter ChordSymbols here

            // Symbols for describing timing within chords
            ii.Add(rawValues, new IntegerList(dot5, dot1346), InputCategoryEnum.ChordTiming, "8", InputSubSubCategoryEnum.NoteTypeEighthOr128th);
            ii.Add(rawValues, new IntegerList(dot5, dot1236), InputCategoryEnum.ChordTiming, "4", InputSubSubCategoryEnum.NoteTypeQuarterOr64th);
            ii.Add(rawValues, new IntegerList(dot5, dot136), InputCategoryEnum.ChordTiming, "2", InputSubSubCategoryEnum.NoteTypeHalfOr32nd);
            ii.Add(rawValues, new IntegerList(dot5, dot134), InputCategoryEnum.ChordTiming, "1", InputSubSubCategoryEnum.NoteTypeFullMeasureOrWholeOr16th);
            // The same symbols followed by a Dot3: "dotted"
            ii.Add(rawValues, new IntegerList(dot5, dot1346,dot3), InputCategoryEnum.ChordTiming, "8+.", InputSubSubCategoryEnum.NoteTypeEighthOr128thDotted);
            ii.Add(rawValues, new IntegerList(dot5, dot1236,dot3), InputCategoryEnum.ChordTiming, "4+.", InputSubSubCategoryEnum.NoteTypeQuarterOr64thDotted);
            ii.Add(rawValues, new IntegerList(dot5, dot136,dot3), InputCategoryEnum.ChordTiming, "2+.", InputSubSubCategoryEnum.NoteTypeHalfOr32ndDotted);
            ii.Add(rawValues, new IntegerList(dot5, dot134,dot3), InputCategoryEnum.ChordTiming, "1+.", InputSubSubCategoryEnum.NoteTypeFullMeasureOrWholeOr16thDotted);


            // Symbols for transscriberadded rests in note notation. IDENTICAL to the CHORDTIMING symbols just above, but used in a slightly different meaning and context.
            // This is implemented by using the value "InputCategoryEnum.InsertedRest" (different from InputCategoryEnum.Rest) while decoding normal parts (containing notes)
            // and "InputCategoryEnum.ChordTiming" while decoding Chord parts containing chords.
            // NOTE!! This again implies that InputCategoryEnum.InsertedRest and InputCategoryEnum.Rest MUST occupy separate categories.
            ii.Add(rawValues, new IntegerList(dot5, dot1346), InputCategoryEnum.InsertedRest, "8", InputSubSubCategoryEnum.NoteTypeEighthOr128th);
            ii.Add(rawValues, new IntegerList(dot5, dot1236), InputCategoryEnum.InsertedRest, "4", InputSubSubCategoryEnum.NoteTypeQuarterOr64th);
            ii.Add(rawValues, new IntegerList(dot5, dot136), InputCategoryEnum.InsertedRest, "2", InputSubSubCategoryEnum.NoteTypeHalfOr32nd);
            ii.Add(rawValues, new IntegerList(dot5, dot134), InputCategoryEnum.InsertedRest, "1", InputSubSubCategoryEnum.NoteTypeFullMeasureOrWholeOr16th);



            // We do not threat the digits 7 9 11 and 13 chords as number but as special symbols, thus we can remain in the "Chord" state
            //allInputInterpretations.Add(rawValues, new IntegerList(dot3456, (dot1 |  dot45)), InputCategoryEnum.Chord456791113, "4"); // NUMBER, 4  as in SUS4    NO sus2 and sus4 are made fixed chord symbols !
            ii.Add(rawValues, new IntegerList(dot3456, dot15), InputCategoryEnum.ChordNumericExtension, "5", InputSubCategoryEnum.ChordNumericExtension5); // NUMBER, 5  as in C5b    
            ii.Add(rawValues, new IntegerList(dot3456, dot15,AccidentalFlat), InputCategoryEnum.ChordNumericExtension, "5b", InputSubCategoryEnum.ChordNumericExtension5); // NUMBER, 5  as in C5b    
            ii.Add(rawValues, new IntegerList(dot3456, dot15,AccidentalSharp), InputCategoryEnum.ChordNumericExtension, "5#", InputSubCategoryEnum.ChordNumericExtension5); // NUMBER, 5  as in C5b    
            ii.Add(rawValues, new IntegerList(dot3456, dot124), InputCategoryEnum.ChordNumericExtension, "6", InputSubCategoryEnum.ChordNumericExtension6); // NUMBER, 6  as in C6    
            //allInputInterpretations.Add(rawValues, new IntegerList(dot3456, dot124,AccidentalFlat), InputCategoryEnum.ChordNumericExtension, "6b", InputSubCategoryEnum.ChordNumericExtension6); // NUMBER, 6  as in C6    
            //allInputInterpretations.Add(rawValues, new IntegerList(dot3456, dot124,AccidentalSharp), InputCategoryEnum.ChordNumericExtension, "6#", InputSubCategoryEnum.ChordNumericExtension6); // NUMBER, 6  as in C6    
            ii.Add(rawValues, new IntegerList(dot3456, dot1245), InputCategoryEnum.ChordNumericExtension, "7", InputSubCategoryEnum.ChordNumericExtension7); // NUMBER, 7  as in C7     
            //allInputInterpretations.Add(rawValues, new IntegerList(dot3456, dot1245,AccidentalFlat), InputCategoryEnum.ChordNumericExtension, "7b", InputSubCategoryEnum.ChordNumericExtension7); // NUMBER, 7  as in C7     
            ii.Add(rawValues, new IntegerList(dot3456, dot1245,AccidentalSharp), InputCategoryEnum.ChordNumericExtension, "7#", InputSubCategoryEnum.ChordNumericExtension7); // NUMBER, 7  as in C7     
            ii.Add(rawValues, new IntegerList(dot3456, dot24), InputCategoryEnum.ChordNumericExtension, "9", InputSubCategoryEnum.ChordNumericExtension9); // NUMBER, 9 as in C9
            ii.Add(rawValues, new IntegerList(dot3456, dot24,AccidentalFlat), InputCategoryEnum.ChordNumericExtension, "9b", InputSubCategoryEnum.ChordNumericExtension9); // NUMBER, 9 as in C9
            ii.Add(rawValues, new IntegerList(dot3456, dot24,AccidentalSharp), InputCategoryEnum.ChordNumericExtension, "9#", InputSubCategoryEnum.ChordNumericExtension9); // NUMBER, 9 as in C9
            ii.Add(rawValues, new IntegerList(dot3456, dot1, dot1), InputCategoryEnum.ChordNumericExtension, "11", InputSubCategoryEnum.ChordNumericExtension11); // NUMBER, 11 as in C11
            ii.Add(rawValues, new IntegerList(dot3456, dot1, dot1,AccidentalFlat), InputCategoryEnum.ChordNumericExtension, "11b", InputSubCategoryEnum.ChordNumericExtension11); // NUMBER, 11 as in C11
            ii.Add(rawValues, new IntegerList(dot3456, dot1, dot1,AccidentalSharp), InputCategoryEnum.ChordNumericExtension, "11#", InputSubCategoryEnum.ChordNumericExtension11); // NUMBER, 11 as in C11
            ii.Add(rawValues, new IntegerList(dot3456, dot1, dot4), InputCategoryEnum.ChordNumericExtension, "13", InputSubCategoryEnum.ChordNumericExtension13); // NUMBER, 13 as in C13
            ii.Add(rawValues, new IntegerList(dot3456, dot1, dot4,AccidentalFlat), InputCategoryEnum.ChordNumericExtension, "13b", InputSubCategoryEnum.ChordNumericExtension13); // NUMBER, 13 as in C13
            ii.Add(rawValues, new IntegerList(dot3456, dot1, dot4,AccidentalSharp), InputCategoryEnum.ChordNumericExtension, "13#", InputSubCategoryEnum.ChordNumericExtension13); // NUMBER, 13 as in C13
            // We find the following stem signs in chapter 5-30 in
            // "New International Manual Of Braille Music Notation by The Braille Music Subcommittee World Blind Union"
            // "Compiled by Bettye Krolick ISBN 90 9009269 2 1996"
            // This document was received from Susanne Nolsøe and is saved under "Documentation" in the Tactile MusicXmlReader solution.
            const int ChordStemSign = dot456;
            ii.Add(rawValues, new IntegerList(ChordStemSign, dot3), InputCategoryEnum.ChordStemSign, "1/1", InputSubCategoryEnum.ChordStemSign, "1"); // 
            ii.Add(rawValues, new IntegerList(ChordStemSign, dot13), InputCategoryEnum.ChordStemSign, "1/2", InputSubCategoryEnum.ChordStemSign, "2"); // 
            ii.Add(rawValues, new IntegerList(ChordStemSign, dot1), InputCategoryEnum.ChordStemSign, "1/4", InputSubCategoryEnum.ChordStemSign, "4"); // 
            ii.Add(rawValues, new IntegerList(ChordStemSign, dot12), InputCategoryEnum.ChordStemSign, "1/8", InputSubCategoryEnum.ChordStemSign, "8"); // 
            ii.Add(rawValues, new IntegerList(ChordStemSign, dot123), InputCategoryEnum.ChordStemSign, "1/16", InputSubCategoryEnum.ChordStemSign, "16"); // 
            ii.Add(rawValues, new IntegerList(ChordStemSign, dot2), InputCategoryEnum.ChordStemSign, "1/32", InputSubCategoryEnum.ChordStemSign, "32"); // 
            // Start Punctuation
            const int punctuation = dot3;
            ii.Add(rawValues, new IntegerList(ChordStemSign, dot3, punctuation), InputCategoryEnum.ChordStemSign, "1/1+.", InputSubCategoryEnum.ChordStemSignWithPunctuation, "1"); // 
            ii.Add(rawValues, new IntegerList(ChordStemSign, dot13, punctuation), InputCategoryEnum.ChordStemSign, "1/2+.", InputSubCategoryEnum.ChordStemSignWithPunctuation, "2"); // 
            ii.Add(rawValues, new IntegerList(ChordStemSign, dot1, punctuation), InputCategoryEnum.ChordStemSign, "1/4+.", InputSubCategoryEnum.ChordStemSignWithPunctuation, "4"); // 
            ii.Add(rawValues, new IntegerList(ChordStemSign, dot12, punctuation), InputCategoryEnum.ChordStemSign, "1/8+.", InputSubCategoryEnum.ChordStemSignWithPunctuation, "8"); // 
            ii.Add(rawValues, new IntegerList(ChordStemSign, dot123, punctuation), InputCategoryEnum.ChordStemSign, "1/16+.", InputSubCategoryEnum.ChordStemSignWithPunctuation, "16"); // 
            ii.Add(rawValues, new IntegerList(ChordStemSign, dot2, punctuation), InputCategoryEnum.ChordStemSign, "1/32+.", InputSubCategoryEnum.ChordStemSignWithPunctuation, "32"); // 
            // Start Double Punctuation
            ii.Add(rawValues, new IntegerList(ChordStemSign, dot3, punctuation, punctuation), InputCategoryEnum.ChordStemSign, "1/1+..", InputSubCategoryEnum.ChordStemSignWithDoublePunctuation, "1"); // 
            ii.Add(rawValues, new IntegerList(ChordStemSign, dot13, punctuation, punctuation), InputCategoryEnum.ChordStemSign, "1/2+..", InputSubCategoryEnum.ChordStemSignWithDoublePunctuation, "2"); // 
            ii.Add(rawValues, new IntegerList(ChordStemSign, dot1, punctuation, punctuation), InputCategoryEnum.ChordStemSign, "1/4+..", InputSubCategoryEnum.ChordStemSignWithDoublePunctuation, "4"); // 
            ii.Add(rawValues, new IntegerList(ChordStemSign, dot12, punctuation, punctuation), InputCategoryEnum.ChordStemSign, "1/8+..", InputSubCategoryEnum.ChordStemSignWithDoublePunctuation, "8"); // 
            ii.Add(rawValues, new IntegerList(ChordStemSign, dot123, punctuation, punctuation), InputCategoryEnum.ChordStemSign, "1/16+..", InputSubCategoryEnum.ChordStemSignWithDoublePunctuation, "16"); // 
            ii.Add(rawValues, new IntegerList(ChordStemSign, dot2, punctuation, punctuation), InputCategoryEnum.ChordStemSign, "1/32+..", InputSubCategoryEnum.ChordStemSignWithDoublePunctuation, "32"); // 

            ii.Add(rawValues, new IntegerList(dot1234, dot3456), InputCategoryEnum.PageNumber, ""); // The letter "P" followed by "ToNumber"
            ii.Add(rawValues, new IntegerList(noDots, noDots), InputCategoryEnum.NewMeasure,"",InputSubCategoryEnum.ExtraSpace1); // A measurebar followed by a single space
            ii.Add(rawValues, new IntegerList(noDots, noDots, noDots), InputCategoryEnum.NewMeasure, "", InputSubCategoryEnum.ExtraSpace2); // A measurebar followed by 2 empty spaces
            ii.Add(rawValues, new IntegerList(noDots, noDots, noDots,noDots), InputCategoryEnum.NewMeasure, "", InputSubCategoryEnum.ExtraSpace3); // A measurebar followed by 3 empty spaces
            ii.Add(rawValues, new IntegerList(carriageReturn, lineFeed,  noDots, noDots, noDots, noDots), InputCategoryEnum.LineContinuation,"", InputSubCategoryEnum.ExtraSpace3); // CR/LF followed by 4 empty spaces
            ii.Add(rawValues, new IntegerList(carriageReturn, lineFeed, noDots, noDots, noDots, noDots, noDots), InputCategoryEnum.LineContinuation,"", InputSubCategoryEnum.ExtraSpace4); // CR/LF followed by 5 empty spaces


            // The special representation of "First MeasureNUmber", "NumberOfMeasures" is started with a lowered ("0" or "1") followed by an underscore :
            // "0" means that the first measure is an anacrusus (Danish:"optakt") 

            const int charUnderScore = dot36;
            ii.Add(rawValues, new IntegerList(digit0Lowered, charUnderScore), InputCategoryEnum.ToNumberLowered, "Starter med optakt.  Antal takter=", InputSubCategoryEnum.FirstMeasureIs0);
            ii.Add(rawValues, new IntegerList(digit1Lowered, charUnderScore), InputCategoryEnum.ToNumberLowered, "Starter uden optakt. Antal takter=", InputSubCategoryEnum.FirstMeasureIs1);

            utils.AddKeySignature(rawValues, ii, InputCategoryEnum.KeySignature);
            utils.AddKeySignature(rawValues, ii, InputCategoryEnum.KeySignatureText); // While decoding Text
            utils.AddRepeatSequence(rawValues, ii);
            utils.AddEmbeddedText(rawValues, ii,textBrailleToTextConverter);
            utils.AddPrintPagination(rawValues, ii);
            utils.AddGuideDots(rawValues, ii);
            utils.AddNoDotsSequence(rawValues, ii);
            utils.AddSectionHeader(rawValues, ii);
            utils.AddComplexEnding(rawValues, ii);
            utils.AddAnyNumericEnding(rawValues, ii);

            ii.Add(rawValues, new IntegerList(dot56, dot3), InputCategoryEnum.GeneralSigns, "", InputSubCategoryEnum.SquareBracketBelowStaffStart);
            ii.Add(rawValues, new IntegerList(dot6, dot23), InputCategoryEnum.GeneralSigns, "", InputSubCategoryEnum.SquareBracketBelowStaffEnd);
#warning TODO add remaining brackets

          //allInputInterpretations.Add(rawValues, new IntegerList(dot126, dot14), InputCategoryEnum.GeneralSigns, "", InputSubCategoryEnum.SmallInvertedArchAboveNote);  // BANA 2015 16.7Small inverted arch above the note
            ii.Add(rawValues, new IntegerList(dot126, dot14), InputCategoryEnum.GeneralSigns, "", InputSubCategoryEnum.PianoPedalDown);  // BANA 2015 29.10-29.11.5

            // Handle CR,LF in the same way as Braille Characters 
            ii.Add(rawValues, new IntegerList(carriageReturn, lineFeed), InputCategoryEnum.ControlCharCRLF);
            ii.Add(rawValues, new IntegerList(formFeed), InputCategoryEnum.ControlCharFF);

            // Handle CR,LF followed by one or two digits (for instance measure numbers) in the same way as Braille Characters
            //MusicBrailleMapper m = musicBrailleMapper; // Establish a local shorthand for better readability
            //allInputInterpretations.Add(rawValues, new List<IntegerList>() { crList, lfList, allDigits }, InputCategoryEnum.ControlCharCRLF, InputSubCategoryEnum.ControlCharCRLFOneDigit);
            //allInputInterpretations.Add(rawValues, new List<IntegerList>() { crList, lfList, allDigits, allDigits }, InputCategoryEnum.ControlCharCRLF, InputSubCategoryEnum.ControlCharCRLFTwoDigits);
            ii.Add(rawValues, new List<MusicBrailleMappingList>() { m.CrList, m.LfList, m.AllIntegers }, InputCategoryEnum.ControlCharCRLFNumber, InputSubCategoryEnum.ControlCharCRLFOneDigit);
            ii.Add(rawValues, new List<MusicBrailleMappingList>() { m.CrList, m.LfList, m.AllIntegers, m.AllIntegers }, InputCategoryEnum.ControlCharCRLFNumber, InputSubCategoryEnum.ControlCharCRLFTwoDigits);
            ii.Add(rawValues, new List<MusicBrailleMappingList>() { m.CrList, m.LfList, m.AllIntegers, m.AllIntegers, m.AllIntegers }, InputCategoryEnum.ControlCharCRLFNumber, InputSubCategoryEnum.ControlCharCRLFThreeDigits);

            // Handle tempo specifications, given in Text Braille :
            //  BANA 2015 1.8. Metronome Indications:

            ii.Add(rawValues, new List<MusicBrailleMappingList>() { m.AllNoteTypes, m.EqualitySign, m.ToDigit, m.AllIntegers, m.AllIntegers}, InputCategoryEnum.Character, InputSubCategoryEnum.CharacterTempo2Digits);
            ii.Add(rawValues, new List<MusicBrailleMappingList>() { m.AllNoteTypes, m.EqualitySign, m.ToDigit, m.AllIntegers, m.AllIntegers, musicBrailleMapper.AllIntegers }, InputCategoryEnum.Character, InputSubCategoryEnum.CharacterTempo3Digits);

            // BANA 2015: 1.7.(b) The Music Heading
            // BANA 2015: 7.1     Meter Signatures
            ii.Add(rawValues, new List<MusicBrailleMappingList>() { m.MeterSignatures, m.MeterSignature }, InputCategoryEnum.Character, InputSubCategoryEnum.CharacterTimeSignature);

            // Handle "Music Hyphen" followed by CR,LF and 1 to 4 empty spaces. This allows continuation of the measure on the next Music Braille line
            // NOTE: Take care when attempting to ad the (continued) text here.It conflicts with a "Meas" text.
            InputSubCategoryEnum continued = InputSubCategoryEnum.ControlCharCRLFContinued;
            ii.Add(rawValues, new IntegerList(dot5, carriageReturn, lineFeed), InputCategoryEnum.ControlCharCRLF,"",continued);
            ii.Add(rawValues, new IntegerList(dot5, carriageReturn, lineFeed, noDots), InputCategoryEnum.ControlCharCRLF, "", continued);
            ii.Add(rawValues, new IntegerList(dot5, carriageReturn, lineFeed, noDots, noDots), InputCategoryEnum.ControlCharCRLF, "", continued);
            ii.Add(rawValues, new IntegerList(dot5, carriageReturn, lineFeed, noDots, noDots, noDots), InputCategoryEnum.ControlCharCRLF, "", continued);
            ii.Add(rawValues, new IntegerList(dot5, carriageReturn, lineFeed, noDots, noDots, noDots, noDots), InputCategoryEnum.ControlCharCRLF, "", continued);
            ii.Add(rawValues, new IntegerList(dot5, carriageReturn, lineFeed, noDots, noDots, noDots, noDots, noDots), InputCategoryEnum.ControlCharCRLF, "", continued);

            //string continued = "(continued)"; // To be used temporarily as friendlyValue
            // Handle "Music Hyphen" followed by 2*(CR,LF) and 1 to 4 empty spaces. This allows continuation of the measure on the next Music Braille line
            ii.Add(rawValues, new IntegerList(dot5, carriageReturn, lineFeed, carriageReturn, lineFeed), InputCategoryEnum.ControlCharCRLF,"",continued);
            ii.Add(rawValues, new IntegerList(dot5, carriageReturn, lineFeed, carriageReturn, lineFeed, noDots), InputCategoryEnum.ControlCharCRLF, "", continued);
            ii.Add(rawValues, new IntegerList(dot5, carriageReturn, lineFeed, carriageReturn, lineFeed, noDots, noDots), InputCategoryEnum.ControlCharCRLF, "", continued);
            ii.Add(rawValues, new IntegerList(dot5, carriageReturn, lineFeed, carriageReturn, lineFeed, noDots, noDots, noDots), InputCategoryEnum.ControlCharCRLF, "", continued);
            ii.Add(rawValues, new IntegerList(dot5, carriageReturn, lineFeed, carriageReturn, lineFeed, noDots, noDots, noDots, noDots), InputCategoryEnum.ControlCharCRLF, "", continued);
            ii.Add(rawValues, new IntegerList(dot5, carriageReturn, lineFeed, carriageReturn, lineFeed, noDots, noDots, noDots, noDots, noDots), InputCategoryEnum.ControlCharCRLF, "", continued);


            // The "Musical Hyphen" followed by a space. BANA 2015 1.11. "The Braille Music Hyphen"
            ii.Add(rawValues, new IntegerList(dot5, noDots), InputCategoryEnum.MusicalHyphenAndSpace, "-");




            // TypeForm Indicators such as "Bold", "Italics" or "underlined"
            const int typeFormIndicator = dot456;
            const int underlineStart = dot346;
            const int underlineEnd = dot156;
            ii.Add(rawValues, new IntegerList(typeFormIndicator, underlineStart), InputCategoryEnum.TypeFormIndicator,"",InputSubCategoryEnum.TypeFormIndicatorUnderlineStart);
            ii.Add(rawValues, new IntegerList(typeFormIndicator, underlineEnd), InputCategoryEnum.TypeFormIndicator,"", InputSubCategoryEnum.TypeFormIndicatorUnderlineEnd);

            ii.Add(rawValues, new IntegerList(dot45, dot14), InputCategoryEnum.Character, "©", InputSubCategoryEnum.CharacterSpecialSequence);

            // BANA 2015: "20.1. Da Capo and Dal Segno Procedures" and Table 20: Da Capo and Dal Segno Repeats (Pars. 20.1–20.3)
            ii.Add(rawValues, new IntegerList(dot345, dot145, dot3, dot14, dot3, dot345), InputCategoryEnum.DaCapoAndDalSegno, "PrintDaCapoOrDC", InputSubCategoryEnum.PrintDaCapoOrDC);
            ii.Add(rawValues, new IntegerList(dot345, dot145, dot14, dot3, dot345), InputCategoryEnum.DaCapoAndDalSegno, "BrailleOnlyDaCapo", InputSubCategoryEnum.BrailleOnlyDaCapo);
            ii.Add(rawValues, new IntegerList(dot346), InputCategoryEnum.DaCapoAndDalSegno,"",InputSubCategoryEnum.DaCapoAndDalSegnoPrintSegno);
            ii.Add(rawValues, new IntegerList(dot346, letterA), InputCategoryEnum.DaCapoAndDalSegno, "BrailleOnlySegnoWithLetterA", InputSubCategoryEnum.BrailleOnlySegnoWithLetter);
            ii.Add(rawValues, new IntegerList(dot346, letterB), InputCategoryEnum.DaCapoAndDalSegno, "BrailleOnlySegnoWithLetterB", InputSubCategoryEnum.BrailleOnlySegnoWithLetter); // etc
            ii.Add(rawValues, new IntegerList(dot5, dot346, letterA), InputCategoryEnum.DaCapoAndDalSegno, "BrailleOnlyDalSegnoWithLetterA", InputSubCategoryEnum.BrailleOnlyDalSegnoWithLetter);
            ii.Add(rawValues, new IntegerList(dot5, dot346, letterB), InputCategoryEnum.DaCapoAndDalSegno, "BrailleOnlyDalSegnoWithLetterB", InputSubCategoryEnum.BrailleOnlyDalSegnoWithLetter);
            ii.Add(rawValues, new IntegerList((dot16), noDots), InputCategoryEnum.DaCapoAndDalSegno, "EndOfBrailleOnlySegnoPassage", InputSubCategoryEnum.EndOfBrailleOnlySegnoPassage); // 20.1.3. End of the Repetition Leading to a Continuation
            ii.Add(rawValues, new IntegerList(dot346, dot123), InputCategoryEnum.DaCapoAndDalSegno, "PrintEncircledCrossCodaSign", InputSubCategoryEnum.PrintEncircledCrossCodaSign);

            return ii;
        }

        // Subclasses for handling specific parts of the main functionality
        private TextBrailleToTextConverter textBrailleToTextConverter;
        private MusicBrailleMapper musicBrailleMapper;
        private TokenReaderUtilities utils;

        // The main data structure  holding the Music Braille file.
        // The file is coded as simple integers, each representing a Braille6 pattern, a carriagereturn, a linefeed or a formfeed. Nothing else !
        private int[] brailleFileAsIntegers;  // The Unicode string to decode, but converted to internal integer representation. Initialized by Create()

        // Simple accessers for the main data structure
        public int GetBrailleFileAsInteger(int i) { return brailleFileAsIntegers[i]; }
        public IntegerList GetBrailleFileAsIntegers(int startIndex, int count)
        {
            IntegerList result = new IntegerList();
            int endIndex = Math.Min(brailleFileAsIntegers.Length, startIndex + count); // Limit count to rest of the array
            for (int i = startIndex; (i < endIndex); i++)
            {
                result.Add(brailleFileAsIntegers[i]);
            }
            return result;
        }

        

        private TokenReader(string brailleAsUnicode,RegionalOptions regionalOptions)
        {

            textBrailleToTextConverter = TextBrailleToTextConverter.Create(regionalOptions);
            musicBrailleMapper = MusicBrailleMapper.Create();
            utils = TokenReaderUtilities.Create(regionalOptions);
            if (!utils.CheckDefinitions(false))
            {
                throw new Exception("TokenReader.CheckDefinitions failed");
            }

            /// Convert to the format used internally: Braille6 characters use bit0 to bit5, Control characters use bit8 to bit 10
            int length = brailleAsUnicode.Length;
            int nErrors = 0;
            brailleFileAsIntegers = new int[length];
            for (int i = 0; (i < length); i++)
            {
                char c = brailleAsUnicode[i];
                if (utils.IsBraille6(c))
                {
                    int integerValue = utils.ToBraille(c);
                    brailleFileAsIntegers[i] = integerValue;
                }
                else
                {
                    int integerValue = utils.ToNotBraille(c);
                    brailleFileAsIntegers[i] = integerValue;
                    if (-1 == integerValue)                    
                    {
                        // The input file contains an illegal character, which is neither Braille6 or one of the 3 control characters CF LF or FF.
                        brailleFileAsIntegers[i] = noDots; // Attempt to continue by replacing the unknown character with a blank Braille character 
                        UserPositionInfo userPositionInfo = UserPositionInfo.Create(brailleAsUnicode, i);
                        utils.LogInvalidInputCharacter(brailleAsUnicode, userPositionInfo, ref nErrors);       
                    }
                }
            } 
        }

        public static TokenReader Create(string brailleAsUnicode, RegionalOptions regionalOptions)
        {
            return new TokenReader(brailleAsUnicode, regionalOptions);
        }
    }
}
