using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrailleMusicDecoder
{

    /// <summary>
    /// This class contains a lot of tables defining simple mappings between MusicBraille symbols and their textual interpretations
    /// This simplifies conversion of  semi-complex tokens such as numbers consisting of mutliple digits
    /// </summary>
    class MusicBrailleMapper
    {
        // Convenience shorthands
        const int dot1 = TokenReader.dot1;
        const int dot2 = TokenReader.dot2;
        const int dot3 = TokenReader.dot3;
        const int dot4 = TokenReader.dot4;
        const int dot5 = TokenReader.dot5;
        const int dot6 = TokenReader.dot6;
        //const int dot7 = TokenReader.dot7;
        //const int dot8 = TokenReader.dot8;

        private MusicBrailleMappingList allIntegers;
        public MusicBrailleMappingList AllIntegers { get { return allIntegers; } }
        private MusicBrailleMappingList allIntegersEmpty;
        public MusicBrailleMappingList AllIntegersEmpty { get { return allIntegersEmpty; } }
        private MusicBrailleMappingList allNoteTypes;
        public MusicBrailleMappingList AllNoteTypes { get { return allNoteTypes; } }
        private MusicBrailleMappingList equalitySign;
        public MusicBrailleMappingList EqualitySign { get { return equalitySign; } }
        private MusicBrailleMappingList toDigit;
        public MusicBrailleMappingList ToDigit { get { return toDigit; } }
        private MusicBrailleMappingList meterSignatures;
        public MusicBrailleMappingList MeterSignatures { get { return meterSignatures; } }
        private MusicBrailleMappingList meterSignature;
        public MusicBrailleMappingList MeterSignature { get { return meterSignature; } }

        // Hand
        private MusicBrailleMappingList hand;
        public MusicBrailleMappingList Hand { get { return hand; } }
        private MusicBrailleMappingList handRight;
        public MusicBrailleMappingList HandRight { get { return handRight; } }
        private MusicBrailleMappingList handLeft;
        public MusicBrailleMappingList HandLeft { get { return handLeft; } }

        private MusicBrailleMappingList dot3List;
        public MusicBrailleMappingList Dot3List { get { return dot3List; } }

        private MusicBrailleMappingList crList;
        public MusicBrailleMappingList CrList { get { return crList; } }
        private MusicBrailleMappingList lfList;
        public MusicBrailleMappingList LfList { get { return lfList; } }

        private MusicBrailleMapper()
        {

            // All integers
            allIntegers = new MusicBrailleMappingList();
            allIntegers.Add(TokenReader.digit0, "0");
            allIntegers.Add(TokenReader.digit1, "1");
            allIntegers.Add(TokenReader.digit2, "2");
            allIntegers.Add(TokenReader.digit3, "3");
            allIntegers.Add(TokenReader.digit4, "4");
            allIntegers.Add(TokenReader.digit5, "5");
            allIntegers.Add(TokenReader.digit6, "6");
            allIntegers.Add(TokenReader.digit7, "7");
            allIntegers.Add(TokenReader.digit8, "8");
            allIntegers.Add(TokenReader.digit9, "9");

            // All integers, but with an empty interpretation
            allIntegersEmpty = new MusicBrailleMappingList();
            allIntegersEmpty.Add(TokenReader.digit0, "");
            allIntegersEmpty.Add(TokenReader.digit1, "");
            allIntegersEmpty.Add(TokenReader.digit2, "");
            allIntegersEmpty.Add(TokenReader.digit3, "");
            allIntegersEmpty.Add(TokenReader.digit4, "");
            allIntegersEmpty.Add(TokenReader.digit5, "");
            allIntegersEmpty.Add(TokenReader.digit6, "");
            allIntegersEmpty.Add(TokenReader.digit7, "");
            allIntegersEmpty.Add(TokenReader.digit8, "");
            allIntegersEmpty.Add(TokenReader.digit9, "");



            //// All note types used in the Musical header
            allNoteTypes = new MusicBrailleMappingList();
            allNoteTypes.Add(TokenReader.noteTypeEightC, "1/8");
            allNoteTypes.Add(TokenReader.noteTypeQuarterC, "1/4");
            allNoteTypes.Add(TokenReader.noteTypeHalfC, "1/2");
            allNoteTypes.Add(TokenReader.noteTypeFullC, "1/1");

            // A few special values used in the musical header
            // BANA 2015: 1.7.(b) The Music Heading
            // BANA 2015 1.8. Metronome Indications:
            equalitySign = new MusicBrailleMappingList();
            equalitySign.Add(TokenReader.EqualitySymbol, "=");
            toDigit = new MusicBrailleMappingList();
            toDigit.Add(TokenReader.ToDigitSymbol, ""); // Interpreted as an empty string 

            // BANA 2015: 1.7.(b) The Music Heading
            // BANA 2015: 7.1     Meter Signatures
            meterSignatures = new MusicBrailleMappingList();
            meterSignatures.Add(dot4 | dot6, "Common-time");
            meterSignatures.Add(dot4 | dot5 | dot6 , "Cut-time");
            meterSignature = new MusicBrailleMappingList();
            meterSignature.Add(dot1 | dot4, "");

            // Lists containing a single control character

            crList = new MusicBrailleMappingList();
            crList.Add(TokenReader.carriageReturn, "");
            lfList = new MusicBrailleMappingList();
            lfList.Add(TokenReader.lineFeed, "");

            // Hand symbols 
            hand = new MusicBrailleMappingList();
            hand.Add(TokenReader.Hand, "");
            handLeft = new MusicBrailleMappingList();
            handLeft.Add(TokenReader.HandLeft, ResourcesForBrailleMusicDecoder.InputValueHand_Left); // NOTE: The part-name is needed by the MUSICXML generator);
            handRight = new MusicBrailleMappingList();
            handRight.Add(TokenReader.HandRight, ResourcesForBrailleMusicDecoder.InputValueHand_Right); // NOTE: The part-name is needed by the MUSICXML generator);

            dot3List = new MusicBrailleMappingList();
            dot3List.Add(TokenReader.dot3, "");

        }


        public static MusicBrailleMapper Create()
        {
            return new MusicBrailleMapper();
        }
    }


    /// <summary>
    ///  Simple class for building tables of simple mappings from a MusicBraille value to an interpretation.
    /// </summary>
    class MusicBrailleMappingList
    {
        private List<MusicBrailleMapping> musicBrailleMappings;

        public void Add(int musicBraille6Key, string value)
        {
            this.musicBrailleMappings.Add(new MusicBrailleMapping(musicBraille6Key, value));
        }

        /// <summary>
        /// Returns a MuaicBrailleMapping with the key specified key if found. Otherwise null
        /// </summary>
        /// <param name="keyToMap"></param>
        /// <returns></returns>
        public MusicBrailleMapping Map(int keyToMap)
        {
            foreach (MusicBrailleMapping mapping in musicBrailleMappings)
            {
                if (keyToMap == mapping.MusicBrailleKey)
                {
                    return mapping;
                }
            }
            return null; // No mapping found
        }

        internal MusicBrailleMappingList()
        {
            musicBrailleMappings = new List<MusicBrailleMapping>();
        }
    }


    /// <summary>
    /// Simple class for building tables of simple mappings from a MusicBraille value to an interpretation.
    /// </summary>
    class MusicBrailleMapping
    {
        private int musicBrailleKey;
        public int MusicBrailleKey { get { return musicBrailleKey; } }
        private string stringValue;
        public string StringValue { get { return stringValue; } }

        public override string ToString()
        {
            return stringValue;
        }

        internal MusicBrailleMapping(int MusicBrailleKey, string stringValue)
        {
            this.musicBrailleKey = MusicBrailleKey;
            this.stringValue = stringValue;            
        }
    }
}
