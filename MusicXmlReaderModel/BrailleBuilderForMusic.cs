using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicXmlReaderModel
{

    /// <summary>
    /// Class for generating any kind of Music Braille. The class hierachi is as follows, where each level of indentions represents a level of enheritage
    /// BrailleBuilderBase handles Braille  in general,
    ///   BrailleBuilderForMusic adds general details for handling Music Braille
    ///     BrailleBuilderForNotes adds further details for handling of notes.
    ///     BrailleHandlerForIntervalNotation adds further details for handling Interval notation 
    ///   BrailleBuilderForText  adds general knowledge for handling Text Braille
    /// </summary>
    public class BrailleBuilderForMusic : BrailleBuilderBase
    {
        // References:
        // Ref.1: http://usermanuals.musicxml.com/MusicXML/Content/EL-MusicXML-articulations.htm
        // Ref.2: https://en.wikipedia.org/wiki/Braille_music
        // Ref.3: https://www.rnib.org.uk/sites/default/files/New%20International%20Manual.pdf
        // Ref.4: "Elementær nodelære i Braille-skrift Enstemmig notation, Revideret udgave 1995", (SynsCenter Refsnæs)
        // Ref.5: "Elementær nodelære i Braille-skrift 2. del Akkordnotation" (SynsCenter Refsnæs)
        // Ref.6: "MUSIC BRAILLE CODE 1997" (BANA)  http://www.brl.org/music/  (Downloaded to C:\Users\Jens\Dropbox\Root\Dokumenter\music braille code.pdf)
        // Ref.7: "MUSIC BRAILLE CODE 2015" (BANA)  http://www.brailleauthority.org/music/Music_Braille_Code_2015.pdf

        // Symbols for indicating a change of state between various Braille formats: such as Music, Letters and digits
        // These symbols are not a part of the Braille Music definition, but are defined at a higher level. 
        public static readonly byte[] MusicBrailleIndicator = new byte[] { dot6, dot3 }; // Marks the start of a sequence of Music Braille.
        public static readonly byte[] GradeOneLetterIndicator = new byte[] { dot5 + dot6 }; // Marks the start of a sequence of letters.
        public static readonly byte[] NumberIndicator = new byte[] { dot3 + dot4 + dot5 + dot6 }; // Marks the start of a sequence of digits.
#warning TODO Check if UppercaseIndicator is dot6 (as used here) or dot4 + dot6
        public static readonly byte[] UppercaseIndicator = new byte[] { dot6 }; // Marks the start of a sequence of uppercase letters.


        // Ref.6 (BANA 1997) Page 1: Table of signs -> General table
        public static byte[] barline = { (noDots) };
        public static byte[] barlineUnusual = { (dot1 + dot2 + dot3) };
        public static byte[] barlineDotted = { (dot1 + dot3) };
        public static byte[] barlineDoubleAtEndOfComposition = { (dot1 + dot2 + dot6), (dot1 + dot3) };
        public static byte[] barlineDoubleAtEndOfMeasureOrSection = { (dot1 + dot2 + dot6), (dot1 + dot3), (dot3) };

        // Ref.6 (BANA 1997) Page 8: Table 4 -> RESTS 
        public static readonly byte[] FullMeasureRest = new byte[] { noDots, (dot1+dot3+dot4), noDots }; // According to Lars Petersen: space,m,space
        public static readonly byte[] FourMeasureRest = new byte[] { 60, 25, 13 };

        // Ref.6 (BANA 1997) various loations:
        public static readonly byte TransscriberAdded = dot5; // Used as a prefix to all items not found in the grapjic note, but added by the Braille Music tranascriber
        public static readonly byte Dot = 4;            // Dotted note. Double dotted notes are specifire by adding 2 dots etc.
        public static readonly byte MusicHyphen = 16; // This measure will be continued on the following line
        public static readonly byte Triplet = 6;
        public static readonly byte RepeatSign = 54; // A beat, a half measure or a full measure must be repeated
        public static readonly byte Slur = 9; // Connects 2 or more notes of different pitch. Danish: Legatobue
        public static readonly byte[] Tie = new byte[] { 8, 9 }; //  Connects 2 notes of the same pitch and makes them souna as one. Danish: Bindebue
        public static readonly byte[] ChordTie = new byte[] { 40, 9 };
        public static readonly byte[] BracketSlurStart = new byte[] { 48, 6 };
        public static readonly byte[] BracketClurEnd = new byte[] { 24, };
        public static readonly byte WordSign = 28; //  Must preceed all musical indications.
        public static readonly byte WordApostrophe = 32; // The word will be continued on the following line
        public static readonly byte[] CrescendoHairpin = new byte[] { 28, 9 };
        public static readonly byte[] DiminuendoendoHairpin = new byte[] { 28, 25 };
        public static readonly byte[] CrescendoHairpinEnd = new byte[] { 28, 18 };
        public static readonly byte[] DiminuendoendoHairpinEnd = new byte[] { 28, 50 };
        public static readonly byte[] Crescendo = new byte[] { 28, 9, 23 };
        public static readonly byte[] DimShape = new byte[] { 28, 25 };
        public static readonly byte[] Forte = new byte[] { 28, 11 };
        public static readonly byte[] Fortissimo = new byte[] { 28, 11, 11 };
        public static readonly byte[] Fff = new byte[] { 28, 11, 11, 11 };
        public static readonly byte[] MezzoForte = new byte[] { 28, 13, 11 };
        public static readonly byte[] Piano = new byte[] { 28, 15 };
        public static readonly byte[] Pianissimo = new byte[] { 28, 15, 15 };
        public static readonly byte[] MezzoPiano = new byte[] { 28, 13, 15 };
        public static readonly byte[] Dim = new byte[] { 28, 25, 10, 9 };
        public static readonly byte[] Rallentando = new byte[] { 28, 23, 1, 7, 7, 4 };
        public static readonly byte[] Ritardando = new byte[] { 28, 23, 10, 30 };
        public static readonly byte[] Ritenuto = new byte[] { 28, 23, 10, 30, 17, 4 };
        public static readonly byte Staccato = 38; // Articulation mark
        public static readonly byte[] Staccatissimo = new byte[] { 32, 38 };    // Articulation mark
        public static readonly byte[] Tenuto = new byte[] { 56, 38 };   // Articulation mark
        public static readonly byte[] TenutoStaccato = new byte[] { 16, 38 };// Articulation mark
        public static readonly byte[] Accent = new byte[] { (dot4 + dot6 ) ,( dot2 + dot3 + dot6) }; // Articulation mark ">"
        public static readonly byte[] StrongAccent = new byte[] { (dot5 + dot6) , (dot2 + dot3 + dot6) }; // Articulation mark "V" or "^"
        public static readonly byte[] Martellato = new byte[] { 48, 38 };// Articulation mark
        public static readonly byte[] CommaHalfBreath = new byte[] { 28, 2 };     //  Articulation mark Ref.3 Page 268
        public static readonly byte[] FullBreakOrBreath = new byte[] { 32, 12 };  //  Articulation mark Ref.3 Page 268
        public static readonly byte[] Swell = new byte[] { 33, 4 };// Articulation mark
        public static readonly byte[] FermatoOnNote = new byte[] { 57 };
        public static readonly byte[] FermataBetweenNotes = new byte[] { 16, 35, 7 };
        public static readonly byte[] FermataOverBarLine = new byte[] { 56, 35, 7 };
        public static readonly byte[] MeasureInAccord = new byte[] { 35, 28 };
        public static readonly byte[] PartMeasureInAccord = new byte[] { 16, 2 };
        public static readonly byte[] MeasureDivisionSign = new byte[] { 40, 5 };
        public static readonly byte Flat  = dot1 + dot2 + dot6;  // 35 // Verify !!
        public static readonly byte Sharp = dot1 + dot4 + dot6; //41; // Verify !!
        public static readonly byte Natural = dot1 + dot6; // Danish: "Opløsningstegn"
        public static readonly byte TupletOf3 = dot2 + dot3;
        public static readonly byte[] ArpeggioUp = new byte[] { dot3 + dot4 + dot5, dot1 + dot3 };
        public static readonly byte[] ArpeggioDown = new byte[] { (dot3 + dot4 + dot5), (dot1 + dot3) , (dot1 + dot3) };

        // The followung symbolr are used as the fixed symbol describing the key
        public static readonly byte KeySharp = dot1 + dot4 + dot6;
        public static readonly byte[] KeyDoubleSharp = { KeySharp, KeySharp };
        public static readonly byte KeyFlat  = dot1 + dot2 + dot6;
        public static readonly byte[] KeyDoubleFlat = { KeyFlat, KeyFlat };
        public static readonly byte KeyNatural  = dot1 + dot6;   // "opløsningstegn" 


        // Timing
        public static readonly byte[] timingCommon = new byte[] { (dot4 + dot6), (dot1 + dot4) };
        public static readonly byte[] timingCut    = new byte[] { (dot4 + dot5 +  dot6), (dot1 + dot4) };


        // Clefs
        // Several opinions seem to exists: All use the form { (dot3 + dot4 + dot5), "X", (dot1 + dot2 + dot3) } but the value of "X" differ 
#if false
        public static readonly byte[] clefG = new byte[] { (dot3 + dot4 + dot5), (dot2 + dot3 + dot5 + dot6), (dot1 + dot2 + dot3) }; //  requested by SN
        public static readonly byte[] clefF = new byte[] { (dot3 + dot4 + dot5), (dot2 + dot3 + dot5),        (dot1 + dot2 + dot3) }; // Used in version >= 3.5 as requested by SN
#endif
#if false
        public static readonly byte[] clefG = new byte[] { (dot3 + dot4 + dot5), (dot3 + dot4       ), (dot1 + dot2 + dot3) }; // Used in version <= 3.4
        public static readonly byte[] clefF = new byte[] { (dot3 + dot4 + dot5), (dot3 + dot4 + dot6), (dot1 + dot2 + dot3) }; // Refsnæs version Used in version <= 3.4
#endif

#if true
        public static readonly byte[] clefG = new byte[] { (dot3 + dot4 + dot5), (dot3 + dot4              ), (dot1 + dot2 + dot3) }; //  BANA and former NOTE employee  Used in version >= 3.5
        public static readonly byte[] clefF = new byte[] { (dot3 + dot4 + dot5), (dot3 + dot4 + dot5 + dot6), (dot1 + dot2 + dot3) }; //  BANA and former NOTE employee  Used in version >= 3.5
#endif
#warning TODO find out if we need to put a dot3 after the clef (And handsign) in some situations


        // Hands
        // Egtved Part 2 Chapter 1 claims that the folowing hand-signs should be placed first on the line and be followed by dot3 except in thecases 
        // when the following Braille char does not contain dot1, dot 2 or dot 3.
        // We prefer to add dot3 always in order to make things simpler !!
        public static readonly byte[] HandRight = new byte[] { (dot4 + dot6), (dot3 + dot4 + dot5) };
        public static readonly byte[] HandLeft = new byte[] { (dot4 + dot5 + dot6), (dot3 + dot4 + dot5) };
        public static readonly byte[] HandRightDot3 = new byte[] { (dot4 + dot6), (dot3 + dot4 + dot5), dot3 };
        public static readonly byte[] HandLeftDot3 = new byte[] { (dot4 + dot5 + dot6), (dot3 + dot4 + dot5), dot3 };



        //public static readonly byte[] clefF = new byte[] { (dot3 + dot4 + dot5), (dot3 + dot4 + dot5 + dot6), (dot1 + dot2 + dot3) }; // BANA
        public static readonly byte[] clefC = new byte[] { (dot3 + dot4 + dot5), (dot3 + dot4 + dot6), (dot1 + dot2 + dot3) }; // ENS

        // Repeat
        public static readonly byte[] repeatEnd   = new byte[] { (dot1 + dot2 + dot6), (dot2 + dot3) };
        public static readonly byte[] repeatStart = new byte[] { (dot1 + dot2 + dot6), (dot2 + dot3 + dot5 + dot6) } ;

        // Endings (According to Refsnæs, 1995 Part 1 Chapter 6e and 6f)
        // Note that Refsnæs sometimes adds the { dot6 , dot3} sequence in front of the "real" sequence. This is also the case here.
        // But as the { dot6 , dot3} sequence just means "Start of Music Braille" it is not a part of the halfEnd/FullEnd symbol described.
        // http://www.brl.org/codes/intmanual/tables/table09.html Table 9 also shows halfEnd/fullEnd without { dot6 , dot3 }
        public static readonly byte[] halfEnd = new byte[] { (dot1 + dot2 + dot6), (dot1 + dot3), dot3 }; // Danish "HalvSlutning" 6e
        public static readonly byte[] fullEnd = new byte[] { (dot1 + dot2 + dot6), (dot1 + dot3) }; // Danish "HelSlutning" 6f

        // Interval notation
        // MUSIC BRAILLE CODE 1997 Table 10 
        public static readonly byte[] inAccordFullMeasure   = new byte[] { (dot1 + dot2 + dot6), (dot3 + dot4 + dot5) };    // Danish "Stor Bistemme"
        public static readonly byte[] inAccordPartMeasure   = new byte[] { dot5, dot2 };                                    // Danish: "Lille Bistemme"
        public static readonly byte[] measureDivision       = new byte[] { (dot4 + dot6), (dot1 + dot3) };                  // Danish: "Skilletegn" 


        // Note: Articulation marks must be inserted BEFORE the note

        //*************************************************************************************************************************************

        // The following references are found at
        // http://www.brl.org/
        // http://www.brl.org/music/index.html

        // Table 1: Basic Signs
        // http://www.brl.org/codes/intmanual/tables/table01.html

        // Table 2 Clefs
        // http://www.brl.org/codes/intmanual/tables/table02.html

        // Table 3: Accidental, Keys & Time Signatures
        // http://www.brl.org/codes/intmanual/tables/table03.html
        // Most of these values are already defined above !

        // Table 4: Rythmic groups
        // http://www.brl.org/codes/intmanual/tables/table04.html
        public static readonly byte triplet = dot2 + dot3;
        public static readonly byte[] GroupOfTree = { (dot4 + dot5 + dot6), (dot2 + dot5), dot3 };
        public static readonly byte[] GroupOfFive = { (dot4 + dot5 + dot6), (dot2 + dot6), dot3 };
        public static readonly byte[] GroupOfSix  = { (dot4 + dot5 + dot6), (dot2 + dot3 + dot5), dot3 };

        // Table 5: Chords
        // http://www.brl.org/codes/intmanual/tables/table05.html

        // Table 6: Slurs and ties
        // http://www.brl.org/codes/intmanual/tables/table06.html
        public static byte slur = (dot1 + dot4);
        public static byte[] glissando = { (dot4), (dot1) };

        // Table 7: Tremolos
        // http://www.brl.org/codes/intmanual/tables/table07.html
        public static byte tremolo = (dot4 + dot5);
        public static byte[] tremoloIn4ths =    { tremolo, (dot1) };
        public static byte[] tremoloIn8ths =    { tremolo, (dot1 + dot2) };
        public static byte[] tremoloIn16ths =   { tremolo, (dot1 + dot2 + dot3) };
        public static byte[] tremoloIn32ths =   { tremolo, (dot2) };
        public static byte[] tremoloIn64ths =   { tremolo, (dot1 + dot3 ) };
        public static byte[] tremoloIn128ths =  { tremolo, (dot3) };

        // Table 8: Fingering
        // http://www.brl.org/codes/intmanual/tables/table08.html

        // Table 9: Bar Lines & Repeats
        // http://www.brl.org/codes/intmanual/tables/table09.html

        // Table 10: Nuances
        // http://www.brl.org/codes/intmanual/tables/table10.html
        public static byte[] fermata = { (dot1 + dot2 + dot6), (dot1 + dot2 + dot3) };
        public static byte[] martellato = { (dot5 + dot6), (dot2 + dot3 + dot6) };
        public static byte[] breath = { (dot6), (dot3 + dot4) };

        // Table 11: Ornaments
        // http://www.brl.org/codes/intmanual/tables/table11.html
        public static readonly byte trillMark = (dot2 + dot3 + dot5);
        public static readonly byte turnBetweenNotes = (dot2 + dot5 + dot6);
        public static readonly byte[] invertedTurnBetweenNotes = { (dot2 + dot5 + dot6), (dot1 + dot2 + dot3) };
        public static readonly byte[] turnAtNote = new byte[] {(dot6), (dot2 + dot5 + dot6) };
        public static readonly byte[] invertedTurnAtNote = {(dot6), (dot2 + dot5 + dot6), (dot1 + dot2 + dot3) };

        public static readonly byte[] mordent = { (dot5), (dot2 + dot3 + dot5), (dot1 + dot2 + dot3) };
        //public static readonly byte
        //public static readonly byte


        // Ornaments still missing:
        //delayedInvertedTurn,
        //delayedTurn,
        //invertedMordent,
        //otherOrnament,
        //schleifer,
        //shake,
        //tremolo,
        //verticalTurn,
        //wavyLine,
        //accidentalMark

        // Table 12: Theory
        // http://www.brl.org/codes/intmanual/tables/table12.html

        // Table 13: Modern notation
        // http://www.brl.org/codes/intmanual/tables/table13.html

        // Table 14: General Organization
        // http://www.brl.org/codes/intmanual/tables/table14.html

        // Table 15: Keyboard music
        // http://www.brl.org/codes/intmanual/tables/table15.html

        // Table 16: Vocal Music
        // http://www.brl.org/codes/intmanual/tables/table16.html

        // Table 17: String instruments 
        // http://www.brl.org/codes/intmanual/tables/table17.html

        // Table 18 : Winds and Percussion
        // http://www.brl.org/codes/intmanual/tables/table18.html

        // Table 19: Accordion
        // http://www.brl.org/codes/intmanual/tables/table19.html

        //*************************************************************************************************************************************

        // some special Unicode values matching some of the aboce Music Braille definitions 
        private const char UnicodeFlat      = (char)0x266D;
        private const char UnicodeNatural   = (char)0x266E; // Danish "Ophævelsestegn"
        private const char UnicodeSharp     = (char)0x266f;


        public enum Hand { Undefined, Left, Right };


        /// <summary>
        /// To force the use of the Create() method
        /// </summary>
        protected BrailleBuilderForMusic(Int64 timeStamp) : base(timeStamp)
        {           
        }



        public static BrailleBuilderForMusic Create(Int64 timeStamp)
        {
            return new BrailleBuilderForMusic(timeStamp);
        }

        private const string className = "BrailleBuilder"; // Only used for logging ! 

        private byte GetStepValue(PitchElement.FullStepEnum step) //  Returns the values for dot 1,2,4,5
        {
            switch (step)
            {
                case PitchElement.FullStepEnum.C: return dot1+dot4+dot5; // Pin 1,4,5
                case PitchElement.FullStepEnum.D: return dot1+dot5; // Pin 1,5
                case PitchElement.FullStepEnum.E: return dot1+dot2+dot4; // Pin 1,2,4,
                case PitchElement.FullStepEnum.F: return dot1+dot2+dot4+dot5; // Pin 1,2,4,5
                case PitchElement.FullStepEnum.G: return dot1+dot2+dot5;// Pin 1,2,5
                case PitchElement.FullStepEnum.A: return dot2+dot4; // Pin 2,4
                case PitchElement.FullStepEnum.B: return dot2+dot4+dot5; // Pin 2,4,5
                default:
                    Logger.Log(string.Format("{0}.GetStepValue({1}) Unknown step '{1}'", className, step));
                    return noDots;
            }
        }


        private byte GetTypeValue(NoteTypeEnum noteDuration) //  Returns the values for dot 3,6
        {
            switch (noteDuration)
            {
                case NoteTypeEnum.whole:
                case NoteTypeEnum.nt16th:  return dot3+dot6;

                case NoteTypeEnum.half:
                case NoteTypeEnum.nt32nd: return dot3;

                case NoteTypeEnum.quarter:
                case NoteTypeEnum.nt64th: return dot6;

                case NoteTypeEnum.eight:  
                case NoteTypeEnum.nt128th: return 0;

                case NoteTypeEnum.breve:
                    Logger.LogOnce(string.Format("{0}.GetTypeValue({1}) Unsupported noteDuration for export to MusicBraille: '{1}'", className, noteDuration.ToString()));
                    // Found in "132.Langebro.musicxml" in Højskolesangbogen.12
                    return noDots;

                default:
                    Logger.LogOnce(string.Format("{0}.GetTypeValue({1}) Unknown noteDuration '{1}'", className, noteDuration.ToString()));
                    return noDots;
            }
        }


        private byte GetRestValue(NoteTypeEnum noteDuration)
        {
            switch (noteDuration)
            {
                case NoteTypeEnum.whole:
                case NoteTypeEnum.nt16th: return dot1 + dot3 + dot4;

                case NoteTypeEnum.half:
                case NoteTypeEnum.nt32nd: return dot1 + dot3 + dot6;

                case NoteTypeEnum.quarter:
                case NoteTypeEnum.nt64th: return dot1 + dot2 + dot3 + dot6;

                case NoteTypeEnum.eight:
                case NoteTypeEnum.nt128th: return dot1 + dot3 + dot4 + dot6;

                default:
                    Logger.LogOnce(string.Format("{0}.GetRestValue({1}) Unknown noteDuration '{1}'", className, noteDuration.ToString()));
                    return noDots;
            }
        }


        protected byte[] GetOctaveMark(int octave)
        {
            switch (octave)
            {
                case 1: return new byte[] { 8 };
                case 2: return new byte[] { 24 };
                case 3: return new byte[] { 56 };
                case 4: return new byte[] { 16 };
                case 5: return new byte[] { 40 };
                case 6: return new byte[] { 48 };
                case 7: return new byte[] { 32 };
            }
            if (octave < 1) return new byte[] { 8, 8 };
            if (octave > 7) return new byte[] { 32, 32 };
            // This is an error. Log it an return something hopefully harmles
            Logger.LogCF(string.Format(": Called with illegal parametervalue octave={0}", octave));
            return new byte[] { 8, 8 };
        }


        // Adds "Dynamics" in MusicBraille known as "Nuances" and described in table 22 in BANA 2015        
        public void AddDynamics(DynamicsElement dynamics )
        {
            if (null == dynamics) return;

            Logger.LogCF(string.Format(": Dynamics={0}", dynamics.Value)); // Initial debugging
            switch (dynamics.Value)
            {
                case DynamicsEnum.Pianissimi: Append(Pianissimo, "pp"); break;
                case DynamicsEnum.Piano: Append(Piano, "p"); break;
                case DynamicsEnum.Forte: Append(Forte, "f"); break;
                case DynamicsEnum.Fortissimo: Append(Fortissimo, "ff"); break;
                case DynamicsEnum.mezzoforte: Append(MezzoForte, "mf"); break;
                case DynamicsEnum.mezzopiano: Append(MezzoPiano, "mp"); break;
                default:
                    Logger.LogCF(string.Format(": Unsupported Dynamics: Value={0}", dynamics.Value)); break;
            }
        }


        public void AddWedge(WedgeElement wedgeElement)
        {
            if (null == wedgeElement) return;
            switch (wedgeElement.Wedge)
            {
                
                case WedgeEnum.diminuendo: Append(DiminuendoendoHairpin, "diminuendo");  break; // Creschendo and Diminuendo are WedgeTypeElemkent
                case WedgeEnum.crescendo: Append(CrescendoHairpin, "crescendo"); break; // Creschendo and Diminuendo are WedgeTypeElemkent
                case WedgeEnum.stopWwedge: Append(DiminuendoendoHairpinEnd, "diminuendoEnd"); break;
                case WedgeEnum.continueWedge: Append(CrescendoHairpinEnd, "crescendoEnd"); break;
                default:
                    Logger.LogCF(string.Format(": Unsupported: WedgeElement={0}", wedgeElement.Wedge.ToString()));
                    break;
            }
        }



        public void AddBrailleNotationsBeforeNoteOrRest(NotationsElement notations) // Some notations are added Before the note itself
        {
            const string functionName = "AddBrailleNotationsBeforeNoteOrRest";
            if (null == notations) return;
            //if (null == notations.Articulations) return;

//            AddDynamics(notations.DynamicsElement);

            if (null != notations.TupletElement)
            {
                if (notations.TupletElement.StartStopContinueType == StartStopContinueElement.StartStopContinueTypeEnum.Start)
                {
                    Append(TupletOf3, "TupletStart");
                    // Logger.LogOnce(string.Format("{0}.{1} Added tuplet start", className, functionName));
                }
            }
                        
            if ((null != notations.Articulations) &&  (null != notations.Articulations.ArticulationList))
            {
                foreach (ArticulationsElement.Articulation articulation in notations.Articulations.ArticulationList)
                {
                    bool implemented = true;
                    switch (articulation)
                    {
                        // Cases are shown in the same saquence as in the MusicXml definition:
                        case ArticulationsElement.Articulation.accent: Append(Accent, "Accent"); break;
                        case ArticulationsElement.Articulation.breathmark: Append(CommaHalfBreath, "CommaHalfBreath"); break;
                        case ArticulationsElement.Articulation.caesura: implemented = false; break;
                        case ArticulationsElement.Articulation.detachedlegato: implemented = false; break;
                        case ArticulationsElement.Articulation.doit: implemented = false; break;
                        case ArticulationsElement.Articulation.falloff: implemented = false; break;
                        case ArticulationsElement.Articulation.otherarticulation: implemented = false; break;
                        case ArticulationsElement.Articulation.plop: implemented = false; break;
                        case ArticulationsElement.Articulation.scoop: implemented = false; break;
                        case ArticulationsElement.Articulation.spiccato: implemented = false; break;
                        case ArticulationsElement.Articulation.staccatissimo: Append(Staccatissimo, "Staccatissimo"); break;
                        case ArticulationsElement.Articulation.staccato: Append(Staccato, "Staccato"); break;
                        case ArticulationsElement.Articulation.stress: implemented = false; break;
                        case ArticulationsElement.Articulation.strongaccent: Append(StrongAccent, "StrongAccent"); break;
                        case ArticulationsElement.Articulation.tenuto: Append(Tenuto, "Tenuto"); break;
                        case ArticulationsElement.Articulation.unstress: implemented = false; break;
                        default:
                            Logger.LogOnce(string.Format("{0}.{1}: Unknown articulation: '{2}'", className, functionName, articulation)); break;
                    }
                    if (!implemented)
                    {
                        Logger.LogOnce(string.Format("{0}.{1}: Unimplemented articulation: '{2}'", className, functionName, articulation));
                    }
                }
            }
        }

        private void LogUninplementedNotationElement(Element notationsElement, string elementName)
        {
            string functionName = "LogUninplementedNotationElement";
            if (null == notationsElement) return;
            Logger.LogOnce(string.Format("{0}.{1}:: Unimplemented NotationElement:{2}", className, functionName, elementName));
        }

        private void LogUninplementedOrnamentsElement(OrnamentsElement ornamentsElement, string elementName)
        {
            string functionName = "LogUninplementedOrnamentsElement";
            if (null == ornamentsElement) return;
            string ornaments = ornamentsElement.UnlocalizedString();
            Logger.LogOnce(string.Format("{0}.{1}: Unimplemented NotationElement:{2} Value={3}", className, functionName, elementName,ornaments));
        }

        public void AddBrailleNotationsAfterNoteOrRest(NotationsElement notations)  // Some notations are added After the note itself
        {
            // Actually we dont know what is added before and what is added after.
            // This method is primarily used for logging unimplemented notations 

            const string functionName = "AddBrailleNotationsAfterNoteOrRest";
            if (null == notations) return;


            // Until we implement the shorthand for describing multiple triplets we do not need to mark the end of a triplet !
            //if (null != notations.TupletElement)
            //{
            //    if (notations.TupletElement.StartStopContinueType == StartStopContinueElement.StartStopContinueTypeEnum.Stop)
            //    {
            //        Append(TupletOf3, "TupletStop");
            //        // Logger.LogOnce(string.Format("{0}.{1} Added tuplet stop", className, functionName));
            //    }
            //}

            
            if (null != notations.TiedElement)
            {
                // Note: "Slur" == "Legatobue"   "Tie" == "Bindebue" See http://www.musicreadingsavant.com/whats-the-difference-between-ties-and-slurs/
                if
                (  (notations.TiedElement.StartStopContinueType == StartStopContinueElement.StartStopContinueTypeEnum.Start)
                || (notations.TiedElement.StartStopContinueType == StartStopContinueElement.StartStopContinueTypeEnum.Continue)
//                || (notations.TiedElement.StartStopContinueType == StartStopContinueElement.StartStopContinueTypeEnum.Stop) // Removed according to Lars Petersen. Error 178
                )
                {
                    Append(Tie, "Tie");
                }
            }

            if (null != notations.SlurElement) 
            {
                // Note: "Slur" == "Legatobue"   "Tie" == "Bindebue" See http://www.musicreadingsavant.com/whats-the-difference-between-ties-and-slurs/
                if
                (
                    (notations.SlurElement.StartStopContinueType == StartStopContinueElement.StartStopContinueTypeEnum.Start)
                ||  (notations.SlurElement.StartStopContinueType == StartStopContinueElement.StartStopContinueTypeEnum.Continue)
//                ||  (notations.SlurElement.StartStopContinueType == StartStopContinueElement.StartStopContinueTypeEnum.Stop)    // Removed according to similar removal for TiedElement                 
                )               
                Append(Slur, "Slur");
            }
                        

            if (null != notations.FermataElement)
            {
                if (   (notations.FermataElement.FermataType == FermataElement.FermataTypeEnum.inverted)
                    || (notations.FermataElement.FermataType == FermataElement.FermataTypeEnum.upright ))
                {
                    Append(fermata, "Fermata"); // This is the fermata             {(dot1+dot2+dot6),(dot1+dot2+dot3)} symbol as desired by Lars Petersen
                                                // Neither the FermatoOnNote       {(dot1+dot4+dot5+dot6)}
                                                // nor     the FermataBetweenNotes {(dot4),               (dot1+dot2+dot6),(dot1+dot2+dot3)}
                                                // nor     the FermataOverBarLine  {(dot4+dot5+dot6),     (dot1+dot2+dot6),(dot1+dot2+dot3)}
                   
                }
            }

            if (null != notations.ArpeggiateElement)
            {
                switch (notations.ArpeggiateElement.ArpeggiateDirection)
                {
                    case ArpeggiateDirectionEnum.down:
                        Append(ArpeggioDown, "ArpeggioDown"); break;
                    case ArpeggiateDirectionEnum.up:
                        Append(ArpeggioUp, "ArpeggioUp"); break;
                    default:
                        Logger.LogOnce(string.Format("{0}.{1} Unknown Arpeggio direction: {2}", className, functionName, notations.ArpeggiateElement.ArpeggiateDirection.ToString()));
                        break;
                }
            }

            if (null != notations.OrnamentsElement)
            {
                foreach (OrnamentsElement.OrnamentsTypeEnum ornament in notations.OrnamentsElement.Ornaments)
                {
                    bool supported = true;
                    switch (ornament)
                    {
                        case OrnamentsElement.OrnamentsTypeEnum.undefined: supported = false; break;
                        case OrnamentsElement.OrnamentsTypeEnum.delayedInvertedTurn: Append(invertedTurnBetweenNotes,"InvertedTurnBetweenNotes"); ; break;
                        case OrnamentsElement.OrnamentsTypeEnum.delayedTurn: Append(turnBetweenNotes,"TurnBetweenNotes"); break; 
                        case OrnamentsElement.OrnamentsTypeEnum.invertedMordent: Append(mordent,"Mordent"); break;  // Use samevalue  as for Mordent
                        case OrnamentsElement.OrnamentsTypeEnum.invertedTurn: Append(invertedTurnAtNote,"InvertedTurnAtNote"); break;
                        case OrnamentsElement.OrnamentsTypeEnum.mordent: Append(mordent,"Mordent"); break;
                        case OrnamentsElement.OrnamentsTypeEnum.otherOrnament: supported = false; break;
                        case OrnamentsElement.OrnamentsTypeEnum.schleifer: supported = false; break;
                        case OrnamentsElement.OrnamentsTypeEnum.shake: supported = false; break;
                        case OrnamentsElement.OrnamentsTypeEnum.tremolo0: Append(tremoloIn4ths,"TremoloIn4ths"); break;
                        case OrnamentsElement.OrnamentsTypeEnum.tremolo1: Append(tremoloIn8ths,"TremoloIn4ths"); break;
                        case OrnamentsElement.OrnamentsTypeEnum.tremolo2: Append(tremoloIn16ths,"TremoloIn4ths"); break;
                        case OrnamentsElement.OrnamentsTypeEnum.tremolo3: Append(tremoloIn32ths,"TremoloIn4ths"); break;
                        case OrnamentsElement.OrnamentsTypeEnum.tremolo4: Append(tremoloIn64ths,"TremoloIn4ths"); break;
                        case OrnamentsElement.OrnamentsTypeEnum.tremolo5: Append(tremoloIn128ths,"TremoloIn4ths"); break;
                        case OrnamentsElement.OrnamentsTypeEnum.tremolo6: supported = false; break;
                        case OrnamentsElement.OrnamentsTypeEnum.tremolo7: supported = false; break;
                        case OrnamentsElement.OrnamentsTypeEnum.trillMark: Append(trillMark,"trillMark"); break;
                        case OrnamentsElement.OrnamentsTypeEnum.turn: Append(turnAtNote,"turnAtNote"); break;
                        case OrnamentsElement.OrnamentsTypeEnum.verticalTurn: supported = false; break;
                        case OrnamentsElement.OrnamentsTypeEnum.wavyLine: supported = false; break;
                        case OrnamentsElement.OrnamentsTypeEnum.accidentalMarkUnknown: supported = false; break;
                        case OrnamentsElement.OrnamentsTypeEnum.accidentalMarkFlat: Append(KeyFlat,"KeyFlat"); break;
                        case OrnamentsElement.OrnamentsTypeEnum.accidentalMarkNatural: Append(KeyNatural,"KeyNatural"); break;
                        case OrnamentsElement.OrnamentsTypeEnum.accidentalMarkSharp: Append(KeySharp,"KeySharp"); break;
                        default: supported = false; break;
                    }
                    if (!supported)
                    {
                        Logger.LogOnce(string.Format("{0}.{1}: Unsupported ornament. Name={2}", className, functionName, ornament.ToString()));
                    }
#if false
                    else
                    {
                        Logger.LogOnce(string.Format("{0}.{1}: Added ornament. Name={2}", className, functionName, ornament.ToString()));
                    }
#endif

                }         
            }
             
            LogUninplementedNotationElement(notations.SlideElement, "SlideElement");  // TO DO
            LogUninplementedNotationElement(notations.GlissandoElement, "GlissandoElement"); // TO DO
            //LogUninplementedOrnamentsElement(notations.OrnamentsElement, "OrnamentsElement");

            // Add other notation elements here asthey are added in the parser.
        }


        /// <summary>
        /// Only to be used for generating text representation of Music Braille 
        /// </summary>
        /// <param name="noteTypeEnum"></param>
        /// <returns></returns>
        private string GetDurationString(NoteTypeEnum noteTypeEnum)
        {
            const string prolog = ""; // The alternative is "/", but we need a representation as short as possible!
            switch (noteTypeEnum)
            {
                case NoteTypeEnum.whole:    return prolog + "1";
                case NoteTypeEnum.half:     return prolog + "2";
                case NoteTypeEnum.quarter:  return prolog + "4";
                case NoteTypeEnum.eight:    return prolog + "8";
                case NoteTypeEnum.nt16th:   return prolog + "16";
                case NoteTypeEnum.nt32nd:   return prolog + "32";
                case NoteTypeEnum.nt64th:   return prolog + "64";
                case NoteTypeEnum.measure:  return prolog + "FM";
                case NoteTypeEnum.unknown:  return prolog + "?";
                default:                    return prolog + "?";
            }
        }



        /// <summary>
        /// Add a NoteElement
        /// </summary>
        /// <param name="noteElement">The NoteElement to add</param>
        public void AddNote(NoteElement noteElement,KeyElement currentKeyElement)
        {
            // string functionName = "AddNote";
            int fifths = (null == currentKeyElement) ? 0 : currentKeyElement.Fifths;

            // Get midified values for Alter and antural, taking in account the current key.
            int alter = noteElement.Alter;
            bool natural = false;
            if (!PitchMap.Map(noteElement.PitchValue, fifths, ref alter, ref natural))
            {
                // Logger.LogOnce(string.Format("{0}.{1}", className, functionName)); // No need tolog here. It is dine in PitchMap.Map !
            }

            if (natural)
            {
                // Add a "natural-sign" (Danish: "Opløsningstegn")
                Append(Natural, UnicodeNatural);
            }


            // Add alteration
            if (0 != alter)
            {
                Append(((noteElement.Alter > 0) ? Sharp : Flat), (alter > 0) ? UnicodeSharp : UnicodeFlat);
            }
      
            // Add Octavemark, either caused by the interval rule or because this is the first note in a scorepart
            if ((MusicBrailleState.NeedOctaveMark(noteElement.Octave, noteElement.PitchValue.SemiTonesAboveC0)) || noteElement.IsFirstNoteInScorePart)
            {
                byte[] octaveMark = GetOctaveMark(noteElement.Octave);
                Append(octaveMark, noteElement.Octave.ToString());
            }

            // Add the combined step value and type value
            byte stepPart = GetStepValue(noteElement.Step);          //  Returns the values for pin 1,2,4,5
            byte typePart = GetTypeValue(noteElement.NoteDuration);  //  Returns the values for pin 3,6
            byte note = (byte)((int)stepPart | (int)typePart);       //  Logical OR to get all 6 pin values 
            Append(note, string.Format("{0}{1}", noteElement.Step.ToString(), GetDurationString(noteElement.NoteDuration)));
   
            // Add punctuation value
            if (noteElement.Dot)
            {
                Append(Dot, ".");
            }

#if false
            // Start for debugging only:
            string brailleAsHex = BrailleToHexString();
            string s =  string.Format("step={0} alter={1} octave={2} semitone={3} ==>{4}",
                noteElement.Step, noteElement.Alter, noteElement.Octave, noteElement.PitchValue.SemiTonesAboveC0, brailleAsHex);
            Logger.Log(s);
            // End for debugging only:
#endif
        }

        public void AddFullEnd()
        {
            Append(BrailleBuilderForMusic.fullEnd);
            AppendText("FullEnd");
            //(BrailleBuilder.fullEnd, "FullEnd");
        }

        public void AddRepeatForward(RepeatElement repeatElement)
        {
            //const string functionName = "AddRepeatForward";
            Append(repeatStart);
            //Logger.LogOnce(string.Format("{0}.{1}", className, functionName));
        }
 
        public void AddRepeatBackward(RepeatElement repeatElement)
        {
            // const string functionName = "AddRepeatBackward";
            Append(repeatEnd);
            //Logger.LogOnce(string.Format("{0}.{1}", className, functionName));
        }        

        public void AddTime(TimeElement timeElement,string s)
        {
#warning TODO: After verifying the syntax: Make a more general algorithm, handling nominator and denominatoe separately !
            const string functionName = "BrailleBuilder.AddTime";

            byte[] timeSymbol = new byte[0] {};
            switch (timeElement.TimeSymbol)
            {
                case TimeElement.TimeSymbolEnum.common: timeSymbol= timingCommon; break;
                case TimeElement.TimeSymbolEnum.cut: timeSymbol = timingCut; break;
                default: break;
            }

            byte[] bytes = new byte[] { };
            byte beatType;
            switch (timeElement.BeatType)
            {
                case 1:
                    beatType = (dot2);
                    switch (timeElement.Beats)
                    {
                        case 2: bytes = new byte[] { Number, cipher2, beatType }; break;       // 2/1  Not specified by REFSNÆS, suggested by JSJ
                        case 3: bytes = new byte[] { Number, cipher3, beatType }; break;       // 3/1  Not specified by REFSNÆS, suggested by JSJ
                        case 4: bytes = new byte[] { Number, cipher4, beatType }; break;       // 4/1  Not specified by REFSNÆS, suggested by JSJ
                        default: break;
                    }
                    break;

                case 2:
                    beatType = (dot2 + dot3);
                    switch (timeElement.Beats)
                    {
                        case 2: bytes = new byte[] { Number, cipher2, beatType }; break;       // 2/2  Not specified by REFSNÆS, suggested by JSJ
                        case 3: bytes = new byte[] { Number, cipher3, beatType }; break;       // 3/2  Not specified by REFSNÆS, suggested by JSJ
                        case 4: bytes = new byte[] { Number, cipher4, beatType }; break;       // 4/2  Not specified by REFSNÆS, suggested by JSJ
                        default: break;
                    }
                    break;

                case 4:
                    beatType = (dot2 + dot5 + dot6);
                    switch (timeElement.Beats)
                    {
                        case 1: bytes = new byte[] { Number, cipher1, beatType }; break; // 1/4 Not specified by REFSNÆS, suggested by JSJ
                        case 2: bytes = new byte[] { Number, cipher2, beatType }; break; // 2/4
                        case 3: bytes = new byte[] { Number, cipher3, beatType }; break; // 3/4
                        case 4: bytes = new byte[] { Number, cipher4, beatType }; break; // 4/4
                        case 5: bytes = new byte[] { Number, cipher5, beatType }; break; // 5/4 Not specified by REFSNÆS, suggested by JSJ
                        case 6: bytes = new byte[] { Number, cipher6, beatType }; break; // 6/4 Not specified by REFSNÆS, suggested by JSJ
                        case 7: bytes = new byte[] { Number, cipher7, beatType }; break; // 7/4 Not specified by REFSNÆS, suggested by JSJ
                        case 8: bytes = new byte[] { Number, cipher8, beatType }; break; // 8/4 Not specified by REFSNÆS, suggested by JSJ
                        case 12: bytes = new byte[] { Number, cipher1, cipher2, beatType }; break; // 12/4 Not specified by REFSNÆS, suggested by JSJ
                        case 14: bytes = new byte[] { Number, cipher1, cipher4, beatType }; break; // 14/4 Not specified by REFSNÆS, suggested by JSJ
                        case 16: bytes = new byte[] { Number, cipher1, cipher6, beatType }; break; // 16/4 Not specified by REFSNÆS, suggested by JSJ
                        case 24: bytes = new byte[] { Number, cipher2, cipher4, beatType }; break; // 24/4 Not specified by REFSNÆS, suggested by JSJ
                        case 28: bytes = new byte[] { Number, cipher2, cipher8, beatType }; break; // 28/4 Not specified by REFSNÆS, suggested by JSJ
                        default:  break;
                    }
                    break;

                case 8:
                    beatType = (dot2 + dot3 + dot6);
                    switch (timeElement.Beats)
                    {
                        case 1: bytes = new byte[] { Number, cipher1, beatType }; break; // 1/8 Not specified by REFSNÆS, suggested by JSJ
                        case 2: bytes = new byte[] { Number, cipher2, beatType }; break; // 2/8 Not specified by REFSNÆS, suggested by JSJ
                        case 3: bytes = new byte[] { Number, cipher3, beatType }; break; // 3/8
                        case 4: bytes = new byte[] { Number, cipher4, beatType }; break; // 4/8
                        case 5: bytes = new byte[] { Number, cipher5, beatType }; break; // 5/8 Not specified by REFSNÆS, suggested by JSJ
                        case 6: bytes = new byte[] { Number, cipher6, beatType }; break; // 6/8 
                        case 7: bytes = new byte[] { Number, cipher7, beatType }; break; // 7/8 Not specified by REFSNÆS, suggested by JSJ
                        case 8: bytes = new byte[] { Number, cipher8, beatType }; break; // 8/8 Not specified by REFSNÆS, suggested by JSJ
                        case 9: bytes = new byte[] { Number, cipher9, beatType }; break; // 9/8 Not specified by REFSNÆS, suggested by JSJ
                        case 12: bytes = new byte[] { Number, cipher1, cipher2, beatType }; break; // 12/8 Not specified by REFSNÆS, suggested by JSJ
                        case 31: bytes = new byte[] { Number, cipher3, cipher1, beatType }; break; // 31/8 Not specified by REFSNÆS, suggested by JSJ
                        default:  break;
                    }
                    break;

                case 16:
                    byte beatType1 = (dot2); // Cipher1 lowered
                    byte beatType2 = (dot2 + dot3 + dot5); // Cipher6 lowered
                    switch (timeElement.Beats)
                    {
                        case 5: bytes = new byte[] { Number, cipher5, beatType1,beatType2 }; break; // 5/16 Not specified by REFSNÆS, suggested by JSJ
                        case 7: bytes = new byte[] { Number, cipher7, beatType1, beatType2 }; break; // 7/16 Not specified by REFSNÆS, suggested by JSJ
                        case 9: bytes = new byte[] { Number, cipher9, beatType1, beatType2 }; break; // 9/16 Not specified by REFSNÆS, suggested by JSJ
                        default: break;
                    }
                    break;

                default:break;
            }
            if (0 == bytes.Count())
            {
                Logger.LogOnce(string.Format("{0}: Unsupported time specification: Beats={1} BeatsType={2}", functionName, timeElement.Beats, timeElement.BeatType));
            }
            else
            {
                // Append(timeSymbol); // Contains the Braille Symbol for "C"  "cut C" used for A la breve. // Removed 2019.10.22 after request from Lars Petersen
                Append(bytes);
                Append(noDots); // Requested by Lars Petersen
                AppendText(s);
                //Logger.LogOnce(string.Format("{0}: Added time specification: {1}/{2}", functionName, timeElement.Beats, timeElement.BeatType));
            }
        }

        public void AddHand(byte[] hand, string s)
        {
            Append(hand);
            AppendText(s);
        }


        public void AddClef(ClefElement clefElement,string s)
        {
            const string functionName = "AddClef";   
            byte[] bytes = new byte[] { };
            switch (clefElement.Clef)
            {
                case ClefEnum.G: bytes = clefG; break;
                case ClefEnum.F: bytes = clefF; break;
                case ClefEnum.C: bytes = clefC; break;
                case ClefEnum.percussion:
                case ClefEnum.TAB :
                case ClefEnum.jianpu:
                case ClefEnum.none:
                    Logger.LogOnce(string.Format("{0}.{1} Clef={2} is not supported in Music Braille", className, functionName, clefElement.Clef.ToString())); break;
                default: Logger.LogOnce(string.Format("{0}.{1} Unknown clef={2}", className,functionName, clefElement.Clef.ToString())); break;
            }
            Append(bytes);
            Append(noDots); // Requested by Lars Petersen
            AppendText(s);          
        }


        public void AddKey(KeyElement keyElement,string s)
        {
            const string functionName = "AddKey";
            byte[] bytes = new byte[] { };
            switch (keyElement.Fifths)
            {
                case 0: break; //  Ask Lars Petersen !!! How do we return to C major or A minor from another key ??
                case 1: bytes = new byte[] { KeySharp }; break;
                case 2: bytes = new byte[] { KeySharp, KeySharp }; break;
                case 3: bytes = new byte[] { KeySharp, KeySharp, KeySharp }; break;
                case 4: bytes = new byte[] { Number, cipher4, KeySharp }; break;
                case 5: bytes = new byte[] { Number, cipher5, KeySharp }; break;
                case 6: bytes = new byte[] { Number, cipher6, KeySharp }; break;
                case 7: bytes = new byte[] { Number, cipher7, KeySharp }; break;
                case -1: bytes = new byte[] { KeyFlat }; break;
                case -2: bytes = new byte[] { KeyFlat, KeyFlat }; break;
                case -3: bytes = new byte[] { KeyFlat, KeyFlat, KeyFlat }; break;
                case -4: bytes = new byte[] { Number, cipher4, KeyFlat }; break;
                case -5: bytes = new byte[] { Number, cipher5, KeyFlat }; break;
                case -6: bytes = new byte[] { Number, cipher6, KeyFlat }; break;
                case -7: bytes = new byte[] { Number, cipher7, KeyFlat }; break;
                default: Logger.LogOnce(string.Format("{0}.{1}: Illegal number of fifths={2}", className,functionName, keyElement.Fifths)); break;
            }
            Append(bytes);
            Append(noDots); // Requested by Lars Petersen
            AppendText(s);       
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">From "whole" to 128nd</param>
        /// <param name="punctured">A puncture added</param>
        public void AddRest(NoteTypeEnum noteDuration, bool punctured) // ********************* FIX ! Temp signature
        {
            // const string functionName = "AddRest";
            //text.Append("R"); // For "Rest"
            AppendText("R");
            if (NoteTypeEnum.measure == noteDuration)
            {
                Append(FullMeasureRest, "/FM");
                //Logger.LogOnce(string.Format("{0}.{1}: Added FullMeasureRest", className, functionName));
            }
            else
            {
                byte rest = GetRestValue(noteDuration);
                Append(rest, GetDurationString(noteDuration));
                if (punctured)
                {
                    Append(Dot, ".");
                }
            }
        }


        public void AddFinger(Hand hand, int finger)
        {
            const byte left = 56;
            const byte right = 40;
            const byte handConst = 28;

            const byte finger1 = 1;
            const byte finger2 = 3;
            const byte finger3 = 7;
            const byte finger4 = 2;
            const byte finger5 = 5;          

            switch (hand)
            {
                case Hand.Left: Append(left); Append(handConst); AppendText("Left");  break;
                case Hand.Right: Append(right); Append(handConst); AppendText("Right"); break;
                case Hand.Undefined:
                    Logger.Log(string.Format("{0}.AddFinger() was called with illegal parameter hand={1}",className, hand.ToString())); break;
            }

            switch (finger)
            {
                case 1: Append(finger1); AppendText("f1"); break;
                case 2: Append(finger2); AppendText("f2"); break;
                case 3: Append(finger3); AppendText("f3"); break;
                case 4: Append(finger4); AppendText("f4"); break;
                case 5: Append(finger5); AppendText("f5"); break;
                default:
                    Logger.Log(string.Format("{0}.AddFinger() was called with illegal parameter finger={1}",className, finger.ToString())); break;
            }

        }

        /// <summary>
        /// // Marks the pount where normal notation ends and MeasureDivision notation start.
        /// </summary>
        public void AppendMeasureDivisionMarkAtEnd()
        {
            // In order to append it AFTER the last existing child we need to append it as a new child.
            BrailleBuilderForMusic bb = BrailleBuilderForMusic.Create(this.TimeStamp);
            bb.Append(measureDivision);
            bb.AppendText("<>");
            this.Append(bb); 
            // Logger.LogCF("----------------------------------------------------------------");
        }

        /// <summary>
        /// Marks the point where an InAccord sequence ends and the next one starts
        /// </summary>
        /// <param name="isFullMeasure"></param>
        public void AddInAccordMark(bool isFullMeasure)
        {
            Append(isFullMeasure ? BrailleBuilderForMusic.inAccordFullMeasure : BrailleBuilderForMusic.inAccordPartMeasure);
            AppendText(isFullMeasure ? "||" : "!!"); // Defined by JSJ for debugging only. )
            // Logger.LogCF(string.Format("({0})",isFullMeasure ? "FullMeasure" : "PartMeasure"));
        }


        public void AddInterval(int size)
        {
            const byte second   = 12;
            const byte third    = 44;
            const byte fourth   = 60;
            const byte fifth    = 20;
            const byte sixth    = 52;
            const byte seventh  = 18;
            const byte eight    = 36;

            switch (size)
            {
                case 2: Append(second); break;
                case 3: Append(third); break;
                case 4: Append(fourth); break;
                case 5: Append(fifth); break;
                case 6: Append(sixth); break;
                case 7: Append(seventh); break;
                case 8: Append(eight); break;
                default:
                    Logger.Log(string.Format("{0}.AddInterval() was called with illegal parameter size={1}", className,size.ToString())); break;
            }


        }

        // New code for handling Barlines

        /// <summary>
        /// Add a barline.
        /// In most cases the barline is an implicit barline, caused by the start of a new measure.
        /// If an explicit barline is found, it eill override the implicit barline.
        /// </summary>
        /// <param name="selectedBarStyles"></param>
        /// <param name="implicitBar"></param>
        public void AddBarline(List<BarStyleEnum> selectedBarStyles, bool implicitBar,int measureNumber)
        {
            // The MeasureNumber is only for debugging !
            string epilog = string.Format("{0} ",measureNumber); // Add an axtra space after each bar to make it more visible (and symmetrical)
            if ((null != selectedBarStyles) && (1 == selectedBarStyles.Count))
            {
                BarStyleEnum barStyle = selectedBarStyles[0];
                // Exactly one barstyle exists. Use it
                switch (barStyle)
                {
                    // Attempt to map the MusicXml BarStyle to the BrailleMusic barstyle.
                    case BarStyleEnum.regular:
                        Append(barline, "|" + epilog);
                        Logger.LogCFOnce(": Added a standard Barline");
                        break;
                    case BarStyleEnum.dotted:
                        Append(barlineDotted, "|.Dotted" + epilog);
                        Logger.LogCFOnce(": Added a dotted Barline");
                        break;
                    case BarStyleEnum.dashed:
                    case BarStyleEnum.heavy:
                    case BarStyleEnum.heavyHeavy:
                    case BarStyleEnum.heavyLight:
                    case BarStyleEnum.lightHeavy:
                    case BarStyleEnum.lightLight:
                    case BarStyleEnum.shortBarStyle:
                    case BarStyleEnum.tick:
                    case BarStyleEnum.none:
                        // Adding barLineUnusual here seems to confuse more than it helps !
                        //Append(barlineUnusual, "|.Unusual" + epilog);
                        //Logger.LogCFOnce(string.Format(": Added an unusual Barstyle={0}", barStyle.ToString()));
                        break;
                    case BarStyleEnum.unknown:
                        Append(barlineUnusual, "|.Unusual" + epilog);
                        Logger.LogCFOnce(string.Format(": Added an unknown Barstyle={0}", barStyle.ToString()));
                        break;
                    default:
                        Logger.LogCFOnce(string.Format(": Undefined Barstyle {0}", barStyle.ToString()));
                        break;
                }
            }
            else
            {
                if (implicitBar)
                {
                    Append(barline, "|" + epilog); // As default use a simple barline
                    //Logger.LogCFOnce(": Added an implicit Barline");
                }
            }
        } // AddBarline



        // TO DO: ********************************************************

        //public void AddConstant(Constant constant)
        //{ }


        /// <summary>
        /// Converts the internal contents from Music-Braille to a Litterary-Braille with the samme 
        /// Braille representation.
        /// This is needed to cheat the Braille ReaderList to represent MusicBraille values
        /// iven if it does not know of their existance.
        /// </summary>
        public override string ToString()
        {
            return "";
        }
    }
}
