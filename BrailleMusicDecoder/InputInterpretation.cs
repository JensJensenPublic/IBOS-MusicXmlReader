using System.Collections.Generic;
using System.Text;
using System;
using System.Xml;
using MusicXmlReaderModel;

namespace BrailleMusicDecoder
{
    [Flags]
    public enum StringFormatOptions
    {
        none = 0x00,
        token = 0x02,
        categories = 0x04,
        value = 0x08,
        friendlyValue = 0x10,
        extraString = 0x20,
        apostrophesInValue = 0x40,
        pageNumber = 0x80, // Used by Decoder
        lineNumber = 0x100, // Used by Decoder
        spaceNumber = 0x200, // Used by Decoder
        xmlRepresentation = 0x400, // Used by Decoder
                defaultOptions = token | categories | value | friendlyValue | extraString // | xmlRepresentation
    }

    /// <summary>
    /// Represents a possible interpretation of a sequence of BrailleMusic characters 
    /// </summary>
    public class InputInterpretation
    {
        private IntegerList token; // The complete token used for this InputInterpretation
        public string TokenString { get { return token.ToString(); } }
        public int TokenLength { get { return token.List.Count; } }
        private InputCategoryEnum category; // The top-level category for this interpretation. Example: InputCategoryEnum.RepeatSequence
        private string unlocalizedFriendlyValue; // Primarily used for unstructured Non-localizable values to be shown directly in the UI. Example: "4/2" for a RepetitionSequence
        private InputSubCategoryEnum subCategory; // The second-level catecoyry
        private string subCategoryValue; // A value related to the subCategory
        private InputSubSubCategoryEnum subSubCategory; // The third-level catecoyry
        private List<string> values; // Primarily used for structured values to be used as parameters for generation ox MusicXml. Example: new List<string)() {4,2} for a repetitionSequence
                                     //        public static StringFormatOptions DefaultStringFormatOptions = StringFormatOptions.categories | StringFormatOptions.extraString | StringFormatOptions.friendlyValue | StringFormatOptions.token | StringFormatOptions.value;
        private string localizedFriendlyValue = null;
        private string localizedCategoryName = null;
        /// <summary>
        /// If <> null this attribute holds the localized value of friendlyValue. Only needed for a few values of InputCategory.
        /// Typically set  and read by MusicXmlBuilderStatrPart
        /// </summary>
        public string LocalizedFriendlyValue { get { return localizedFriendlyValue; } }
        public string LocalizedCategoryName { get { return localizedCategoryName; } }

        //private XmlNode xmlNode = null;
        private List<XmlNode> xmlNodeContainer = new List<XmlNode>(); // Allows for adding nodes at any time
        public List<XmlNode> XmlNodeContainer { get { return xmlNodeContainer; } }
        /// <summary>
        /// The XmlNode generated to represent this input if this input represents a sound (for instans a note):
        /// </summary>
        public XmlNode XmlNode
        {
            get { return (0 == xmlNodeContainer.Count) ? null : xmlNodeContainer[0]; }
            set { xmlNodeContainer.Clear();  xmlNodeContainer.Add(value); }
        }

        private UnAmbiguousNoteTypeEnum unAmbiguousNoteType;
        public UnAmbiguousNoteTypeEnum UnAmbiguousNoteType { get { return unAmbiguousNoteType; } set { unAmbiguousNoteType = value; } }



        /// <summary>
        /// Create a clone only containing the values needed for fixing type ambiguities
        /// </summary>
        /// <returns></returns>
        public InputInterpretation CloneOfTimingValues()
        {
            return new InputInterpretation(this);
        }

        private InputInterpretation(InputInterpretation that)
        {   
            // Copy all members that can are needed for fixing type ambiguity only (included members primarily needed for debugging)
            this.category = that.category;
            this.subCategory = that.subCategory;
            this.subSubCategory = that.subSubCategory; // Holds the ambiguous notetype 
            this.unAmbiguousNoteType = that.unAmbiguousNoteType; // Holds the unambiguous notetype 
            this.unlocalizedFriendlyValue = that.unlocalizedFriendlyValue; // Only for debugging.
        }


        private string ShowNull(object o)
        {
            return (o == null) ? "null" : o.ToString();
        }

        /// <summary>
        ///  Primarily for use during development and debugging
        /// </summary>
        /// <returns></returns>
        public string ToDebugString()
        {
            return ToDebugString("");
        }

        /// <summary>
        /// NOTE: The string presented to the user should be geneated by calling ToString(Options rawOptions), implemented later in this class NOTE
        /// Primarily for use during development and debugging.
        /// Only relaevant values are shown, neither "null" nor enum.None !
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public string ToDebugString(string prefix)
        {
            string tokenString = token.ToUnicodeString();
            string categoryString = string.Format("Category={0}", category); // Always show the category !
            string friendlyValueString = string.IsNullOrEmpty(FriendlyValue) ? "" : string.Format("  FriendlyValue={0}", ShowControlCharacters(FriendlyValue)); // FriendlyValue: Use Localized value if possible
            string subCategoryString = (InputSubCategoryEnum.None == subCategory) ? "" : string.Format("  SubCategory={0}", subCategory.ToString());
            string subCategoryValueString = string.IsNullOrEmpty(subCategoryValue) ? "" : string.Format("  SubCategoryValue={0}",subCategoryValue);
            string subSubCategoryString = (InputSubSubCategoryEnum.Unknown == subSubCategory) ? "" : string.Format("  SubSubCategory={0}", subSubCategory);
            string result = string.Format("{0}Token='{1}' => {2}{3}{4}{5}{6}",
                prefix, // 0
                tokenString, // 1
                categoryString, // 2
                friendlyValueString, // 3
                subCategoryString, // 4
                subCategoryValue, // 5
                subSubCategoryString); // 6
            return result;
        }

        private string ShowControlCharacters(string s)
        {
            if (string.IsNullOrEmpty(s)) return s; 
            string s1 = s.Replace("\r", "<CR>");
            string s2 = s1.Replace("\n", "<LF>");
            string result = s2.Replace("\f", "<FF>");
            return result;
        }


        /// <summary>
        /// Assure common algorithm for evaluating NoteType and Divisions
        /// UnAmbigiousNoteType takes precedence over AmbiguousNoteType.
        /// If this is not a note or a rest UnAmbiguousNoteTypeEnum.TypeUnknown is returned.
        /// New implementation 2021
        /// </summary>
        /// <returns></returns>
        public UnAmbiguousNoteTypeEnum GetNoteType()
        {
            if (UnAmbiguousNoteTypeEnum.TypeUnknown != this.unAmbiguousNoteType)
            {
                return this.unAmbiguousNoteType; // Use the unambiguous value
            }
            switch (this.subSubCategory)
            {
                // Make an assumption:
                case InputSubSubCategoryEnum.NoteTypeFullMeasureOrWholeOr16th: return UnAmbiguousNoteTypeEnum.Type16th;
                case InputSubSubCategoryEnum.NoteTypeHalfOr32nd: return UnAmbiguousNoteTypeEnum.TypeHalf;
                case InputSubSubCategoryEnum.NoteTypeQuarterOr64th: return UnAmbiguousNoteTypeEnum.TypeQuarter;
                case InputSubSubCategoryEnum.NoteTypeEighthOr128th: return UnAmbiguousNoteTypeEnum.TypeEight;
                default: return UnAmbiguousNoteTypeEnum.TypeUnknown;
                //default: throw new System.Exception("");
            }
        }

        #region ShortDebugStrings

        //private string ToShortTypeString(InputSubSubCategoryEnum subSubCategory)
        //{
        //    switch (subSubCategory)
        //    {
        //        case InputSubSubCategoryEnum.NoteTypeHalfOr32nd: return "/2";
        //        case InputSubSubCategoryEnum.NoteTypeQuarterOr64th: return "/4";
        //        case InputSubSubCategoryEnum.NoteTypeEighthOr128th: return "/8";
        //        case InputSubSubCategoryEnum.NoteTypeFullMeasureOrWholeOr16th: return "/16";
        //        default:
        //            Logger.LogCF("");
        //            return "?";
        //    }
        //}

        private string ToShortPunctuationString(InputSubCategoryEnum subCategory)
        {
            switch (subCategory)
            {
                case InputSubCategoryEnum.PunctuationSingle: return "P";
                case InputSubCategoryEnum.PunctuationDouble: return "PP";
                case InputSubCategoryEnum.PunctuationTriple: return "PPP";
                default: return "?";
            }
        }


        private string ToShortHandString(InputSubCategoryEnum subCategory)
        {
            switch (subCategory)
            {
                case InputSubCategoryEnum.HandLeft: return "LEFT";
                case InputSubCategoryEnum.HandRight: return "RIGHT";
                default: return "HAND?";
            }
        }

        private string ToShortIntervalString(InputSubCategoryEnum subCategory)
        {
            switch (subCategory)
            {
                case InputSubCategoryEnum.IntervalFifth: return "5";
                case InputSubCategoryEnum.IntervalFourth: return "4";
                case InputSubCategoryEnum.IntervalOctave: return "8";
                case InputSubCategoryEnum.IntervalSecond: return "2";
                case InputSubCategoryEnum.IntervalSeventh: return "7";
                case InputSubCategoryEnum.IntervalSixth: return "6";
                case InputSubCategoryEnum.IntervalThird: return "3";
                default: return "?";  
            }
        }

        private string ToShortAccidentalString(InputSubCategoryEnum subCategory)
        {
            switch (subCategory)
            {
                case InputSubCategoryEnum.AccidentalCourtesySharp:
                case InputSubCategoryEnum.AccidentalSharp: return "#";
                case InputSubCategoryEnum.AccidentalCourtesyFlat:
                case InputSubCategoryEnum.AccidentalFlat: return "b";
                case InputSubCategoryEnum.AccidentalCourtesyNatural:
                case InputSubCategoryEnum.AccidentalNatural: return "Natural";
                default: return "ACC?";
            }
        }


        public string ToFullStepString()
        {
            switch (this.subCategory)
            {
                // By tradition and MusicXml rules root values are described by UPPERCASE letters
                case InputSubCategoryEnum.FullStepA: return "A";
                case InputSubCategoryEnum.FullStepB: return "B";
                case InputSubCategoryEnum.FullStepC: return "C";
                case InputSubCategoryEnum.FullStepD: return "D";
                case InputSubCategoryEnum.FullStepE: return "E";
                case InputSubCategoryEnum.FullStepF: return "F";
                case InputSubCategoryEnum.FullStepG: return "G";
                default:
                    string message = string.Format("Invalid subcategory={0} for Crtegory={1}", this.subCategory, this.category);
                    Logger.LogCF1(string.Format(": {0}",message));
                    throw new Exception(message);
            }
        }


        /// <summary>
        /// Assert that category is euther Note, Rest or InsertedRest
        /// </summary>
        private void AssertNoteOrRest()
        {
            const InputCategoryEnum mask = ~(InputCategoryEnum.Note | InputCategoryEnum.Rest | InputCategoryEnum.InsertedRest);
            if (0 != (this.category & mask))
            {
                string message = String.Format("Unexpected category={0}", category.ToString());
                Logger.LogCF1(": " + message);
                throw new Exception(message);
            }
        }


#warning ToDo Move the simiglar GetNoteType() from MusicXmlBuilderStatePart to here.
        private string ToShortTypeString()
        {
            AssertNoteOrRest();
            // First select the unambiguous notetipe if it exists. Otherways fall back to the ambiguous notetype, directly fetched from the Music Braille file.
            UnAmbiguousNoteTypeEnum u = GetNoteType();
            switch (u)
            {
#warning todi difference between whole and fullmeasure
                case UnAmbiguousNoteTypeEnum.TypeFullMeasure: return "/FullMeasure"; // Only used by Full measure rest which may differ from /1 in for instance 3/4 beat/beattype
                case UnAmbiguousNoteTypeEnum.TypeWhole: return "/1";
                case UnAmbiguousNoteTypeEnum.TypeHalf: return "/2";
                case UnAmbiguousNoteTypeEnum.TypeQuarter: return "/4";
                case UnAmbiguousNoteTypeEnum.TypeEight: return "/8";
                case UnAmbiguousNoteTypeEnum.Type16th: return "/16";
                case UnAmbiguousNoteTypeEnum.Type32nd: return "/32";
                case UnAmbiguousNoteTypeEnum.Type64th: return "/64";
                case UnAmbiguousNoteTypeEnum.Type128th: return "/128";
                default: throw new Exception(string.Format("GetNoteType: Unsupported parameter {0}", u.ToString()));
            }
        }


        private string ToShortTimeModificationString()
        {
            switch (this.subCategory) // Explicitly mention "this." to ease debugging !
            {
                case InputSubCategoryEnum.TimeModificationFermata: return "Fermata";
                case InputSubCategoryEnum.TimeModificationTriplet: return "Triplet";    
                default:
                    string message = string.Format("Unexpected SubCategory= {0}", subCategory.ToString());
                    Logger.LogCF(": " + message);
                    throw new Exception(message);
            }
        }


        /// <summary>
        /// Generate a comprehensive stringrepresentation to be used for debugging
        /// NOTE: The string presented to the user should be geneated by calling ToString(Options rawOptions), implemented later in this class NOTE
        /// </summary>
        /// <returns></returns>
        public string ToShortDebugString()
        {
            switch (this.category)
            {
                case InputCategoryEnum.Rest:
                case InputCategoryEnum.InsertedRest: return string.Format("R{0}", ToShortTypeString());
                case InputCategoryEnum.Note: return string.Format("{0}{1}", ToFullStepString(),ToShortTypeString());
                case InputCategoryEnum.Punctuation: return string.Format("{0}", ToShortPunctuationString(SubCategory));
                case InputCategoryEnum.TimeModification: return string.Format("{0}", ToShortTimeModificationString());
                case InputCategoryEnum.PartMeasureRepeat: return string.Format("PartMeasureRepeat");
                case InputCategoryEnum.InAccordFullMeasure: return "IaFullMeasure";
                case InputCategoryEnum.InAccordPartMeasure: return "IaPartMeasure";
                case InputCategoryEnum.MeasureDivision: return "MeasureDivision";
                case InputCategoryEnum.NewMeasure: return "|";
                case InputCategoryEnum.SectionalDoubleBar: return "||";
                case InputCategoryEnum.FinalDoubleBar: return "||.";
                case InputCategoryEnum.Hand: return string.Format("{0}",  ToShortHandString(subCategory));
                case InputCategoryEnum.Octave: return string.Format("O{0}",FriendlyValue);
                case InputCategoryEnum.Interval: return string.Format("I{0}",ToShortIntervalString(subCategory));
                case InputCategoryEnum.KeySignature: return string.Format("KEY{0}", FriendlyValue);
                case InputCategoryEnum.Accidental: return string.Format("{0}", ToShortAccidentalString(subCategory));
                case InputCategoryEnum.ControlCharCRLF: return "CRLF";
                case InputCategoryEnum.ControlCharCRLFNumber: return string.Format("CRLF{0}",FriendlyValue);
                case InputCategoryEnum.ControlCharFF: return "FF";
                case InputCategoryEnum.Slur: return "Slur";
                case InputCategoryEnum.Tie: return "Tie";
                case InputCategoryEnum.InAccordTie: return "IaTie";
                case InputCategoryEnum.EmbeddedTextRepresentation: return "TEXT";
                case InputCategoryEnum.Articulation: return "Articuration";
                case InputCategoryEnum.GeneralSigns: return "GenericSign";
                case InputCategoryEnum.LineContinuation: return "LineContinuation ";
                case InputCategoryEnum.UnusualBarLine: return "(|)";
                case InputCategoryEnum.OtherValues: return ToShortDebugString(this.subCategory);
                case InputCategoryEnum.PrintPagination: return string.Format("PP{0}", FriendlyValue);
                case InputCategoryEnum.GuideDots: return string.Format("GuideDots({0})", FriendlyValue);
                case InputCategoryEnum.ToMusicBraille: return "ToMusicBralle";
                case InputCategoryEnum.RepeatSequence: return "RepeatSequence";
                case InputCategoryEnum.DaCapoAndDalSegno: return "DaCapoDalSegno";
                default: return category.ToString(); // Use the name of the category 
            }
        }


        /// <summary>
        /// Only to be used for InputCategoryEnum.OtherValues! 
        /// NOTE: The string presented to the user should be geneated by calling ToString(Options rawOptions), implemented later in this class NOTE
        /// </summary>
        /// <param name="subCategory"></param>
        /// <returns></returns>
        private string ToShortDebugString(InputSubCategoryEnum subCategory)
        {
            if (InputCategoryEnum.OtherValues != this.category)
            {
                string message = String.Format("Unexpected Category={0}", Category);
                Logger.LogCF(string.Format(":{0}", message));
                throw new Exception(message);
            }

            switch (subCategory)
            {
                case InputSubCategoryEnum.OthervaluesDoubleBarFollowedByDots: return "RepeatStart";
                case InputSubCategoryEnum.OthervaluesDoubleBarPrecededByDots: return "RepeatEnd";
                case InputSubCategoryEnum.OthervaluesFullMeasureRepeat: return "FMRepeat";
                case InputSubCategoryEnum.OthervaluesFullMeasureRepeatNoInitialBlank: return "FMRepeatNB";
                case InputSubCategoryEnum.OthervaluesFullMeasureRepeatTwice: return "FMRepeat2";
                case InputSubCategoryEnum.OthervaluesLongAppoggiatura: return "LongAppogg";
                case InputSubCategoryEnum.OtherValuesOrnament: return "Ornament";
                case InputSubCategoryEnum.OthervaluesShortAppoggiatura: return "ShortAppogg";
                case InputSubCategoryEnum.OthervaluesTremoloAlternatingNotes: return "TremoloAlt";
                case InputSubCategoryEnum.OthervaluesTremoloRepeatedNote: return "TremoloRep";
                case InputSubCategoryEnum.OtherValuesTrill: return "Trill";
                case InputSubCategoryEnum.OthervaluesVolta1FirstEnding: return "Volta1";
                case InputSubCategoryEnum.OthervaluesVolta2SecondEnding: return "Volta2";
                case InputSubCategoryEnum.OthervaluesVoltaIntervalEnding: return "VoltaN-N";
                case InputSubCategoryEnum.OthervaluesVoltaNumericEnding: return "VoltaN";
                default:
                     string message = String.Format("Unexpected Subcategory={0}", subCategory);
                    Logger.LogCF(string.Format(":{0}", message));
                    throw new Exception(message);
            }
        }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="token">The sequence of BrailleMusic values, that this instance represents</param>
        /// <param name="inputCategory">The category of value that this instance represents, for instance a musical note, a digit or a letter</param>
        /// <param name="value">A string representation of the instance, within the inputCategory</param>
        public InputInterpretation(IntegerList token, InputCategoryEnum inputCategory, string value)
        {
            this.token = token;
            this.category = inputCategory;
            this.unlocalizedFriendlyValue = value;
            this.subCategory = InputSubCategoryEnum.None;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="token">The sequence of BrailleMusic values, that this instance represents</param>
        /// <param name="inputCategory">The category of value that this instance represents, for instance a musical note, a digit or a letter</param>
        /// <param name="value">A string representation of the instance, within the inputCategory</param>
        /// <param name="inputSubCategory">An enumeration further categorizing within the inputCategory</param>
        public InputInterpretation(IntegerList token, InputCategoryEnum inputCategory, string value, InputSubCategoryEnum inputSubCategory)
        {
            this.token = token;
            this.category = inputCategory;
            this.unlocalizedFriendlyValue = value;
            this.subCategory = inputSubCategory;
        }

        public InputInterpretation(IntegerList token, InputCategoryEnum inputCategory, string value, InputSubCategoryEnum inputSubCategory, InputSubSubCategoryEnum inputSubSubCategory)
        {
            this.token = token;
            this.category = inputCategory;
            this.unlocalizedFriendlyValue = value;
            this.subCategory = inputSubCategory;
            this.subCategoryValue = null;
            this.subSubCategory = inputSubSubCategory;
        }



        public InputInterpretation(IntegerList token, InputCategoryEnum inputCategory, string value, InputSubCategoryEnum inputSubCategory, string inputSubCategoryValue)
        {
            this.token = token;
            this.category = inputCategory;
            this.unlocalizedFriendlyValue = value;
            this.subCategory = inputSubCategory;
            this.subCategoryValue = inputSubCategoryValue;
        }

        public InputInterpretation(IntegerList token, InputCategoryEnum inputCategory, string value, List<string> values)
        {
            this.token = token;
            this.category = inputCategory;
            this.unlocalizedFriendlyValue = value;
            this.values = values;
        }

        public InputInterpretation(IntegerList token, InputCategoryEnum inputCategory, string value, InputSubCategoryEnum subCategory, List<string> values)
        {
            this.token = token;
            this.category = inputCategory;
            this.unlocalizedFriendlyValue = value;
            this.subCategory = subCategory;
            this.values = values;
        }




        /// <summary>
        /// This is the "one size fits all" version, taking all possible parameters!
        /// </summary>
        /// <param name="token"></param>
        /// <param name="inputCategory"></param>
        /// <param name="value"></param>
        /// <param name="inputSubCategory"></param>
        /// <param name="inputSubCategoryValue"></param>
        /// <param name="inputSubSubCategory"></param>
        public InputInterpretation(IntegerList token, InputCategoryEnum inputCategory, string value, InputSubCategoryEnum inputSubCategory, string inputSubCategoryValue, InputSubSubCategoryEnum inputSubSubCategory,List<string> values)
        {
            this.token = token;
            this.category = inputCategory;
            this.unlocalizedFriendlyValue = value;
            this.subCategory = inputSubCategory;
            this.subCategoryValue = inputSubCategoryValue;
            this.subSubCategory = inputSubSubCategory;
            this.values = values;
        }
       
        /// <summary>
        /// A string representation of the instance, within the inputCategory
        /// </summary>
        public string FriendlyValue
        {
            get
            {
                if (null == localizedFriendlyValue)
                {
                    return unlocalizedFriendlyValue;
                }
                return localizedFriendlyValue;

            }
            set
            {
                unlocalizedFriendlyValue = value;
            }
        }



        /// <summary>
        /// A string representation of the instance, within the inputCategory
        /// </summary>
        public List<string> Values
        {
            get
            {
                return values;
            }
            set
            {
                values = value;
            }
        }


        /// <summary>
        /// An enumeration representation of the instance, within the inputCategory
        /// </summary>
        public InputSubCategoryEnum SubCategory
        {
            get
            {
                return subCategory;
            }

        }

        /// <summary>
        /// An enumeration representation of the instance, within the inputCategory and subCategory
        /// </summary>
        public InputSubSubCategoryEnum SubSubCategory
        {
            get
            {
                return subSubCategory;
            }
        }


        /// <summary>
        /// A string representing further information related to the value of InputSubCategory
        /// </summary>
        public string SubCategoryValue
        {
            get
            {
                return subCategoryValue;
            }
        }


        private string GetExtraString(InputCategoryEnum inputCategoryEnum)
        {
            const string shortLine = "-----";
            const string longLine = "---------------------------------------------";
            switch (inputCategoryEnum)
            {
                case InputCategoryEnum.NewMeasure: return shortLine; // Make it easier to find each barline
                case InputCategoryEnum.Hand: return longLine; // Make it easier to find the start of notation for Left hand and Right hand respetively.
                case InputCategoryEnum.Chords: return longLine; // MAke it easier to find the start of notation for chords.
                default: return "";
            }

        }

        public override string ToString()
        {
#warning todo implement
            throw new Exception("");
        }


        /// <summary>
        /// Builds the string representation of this instance, as it will be presented to the user through the the UI.
        /// </summary>
        /// <param name="rawOptions">A simple enumeration parameter, containing flags for controlling the structure of the result</param>
        /// <returns>The string representation to be  presented to the user through UI</returns>
        public string ToString(Options rawOptions)
        {          
            switch (this.category)   // Exclusively for setting breakpoints for specific input categories (in this case InputCategoryEnum.Note) during debugging.
            {
                case InputCategoryEnum.Note: break;
                default: break;
            }
            
            StringFormatOptions stringFormatOptions = rawOptions.StringFormatOptions;
            string apostrophe =  0 != (stringFormatOptions & StringFormatOptions.apostrophesInValue) ?  "'" : "";
            string friendlyValueString =  string.IsNullOrEmpty(FriendlyValue) ? "" : string.Format("{0}{1}{2}", apostrophe, FriendlyValue, apostrophe); // Only show real values  FriendlyValue: Use Localized value if possible
            // We want to make it easy to find the NewMeasure items !
            string extraString = GetExtraString(category);
            string valuesString = "";
            if ((null != values) && (values.Count > 0))
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(" Values=(");
                string delimiter = "";
                foreach (string value in values)
                {
                    sb.Append(delimiter + value);
                    delimiter = ",";
                }
                sb.Append(") ");
                valuesString = sb.ToString(); 
            }
            // The CR, LF and FF  characters can not  be  shown as Unicode characters:
            string tokenString =(  (category == InputCategoryEnum.ControlCharCRLF)
                                || (category == InputCategoryEnum.ControlCharCRLFNumber)
                                || (category == InputCategoryEnum.ControlCharFF) )? "" : token.ToUnicodeString();
            string localizedString = ToLocalizedString(category, subCategory, subSubCategory, TokenLength, rawOptions.VisibleCategoryNames);
            string result = string.Format("{0} {1}{2}{3}{4} ",
                (0 != (stringFormatOptions & StringFormatOptions.token)) ? tokenString : "",
                (0 != (stringFormatOptions & StringFormatOptions.categories)) ? localizedString : "",
                (0 != (stringFormatOptions & StringFormatOptions.value)) ? valuesString : "",
                (0 != (stringFormatOptions & StringFormatOptions.friendlyValue)) ? friendlyValueString : "",
                (0 != (stringFormatOptions & StringFormatOptions.extraString)) ? extraString : "");
            return result;
        }


        private string ToLocalizedIntervalName(InputSubCategoryEnum inputSubCategory)
        {
            switch (inputSubCategory)
            {
                case InputSubCategoryEnum.IntervalSecond: return ResourcesForBrailleMusicDecoder.IntervalNameEnum_Second;
                case InputSubCategoryEnum.IntervalThird: return ResourcesForBrailleMusicDecoder.IntervalNameEnum_Third;
                case InputSubCategoryEnum.IntervalFourth: return ResourcesForBrailleMusicDecoder.IntervalNameEnum_Fourth;
                case InputSubCategoryEnum.IntervalFifth: return ResourcesForBrailleMusicDecoder.IntervalNameEnum_Fifth;
                case InputSubCategoryEnum.IntervalSixth: return ResourcesForBrailleMusicDecoder.IntervalNameEnum_Sixth;
                case InputSubCategoryEnum.IntervalSeventh: return ResourcesForBrailleMusicDecoder.IntervalNameEnum_Seventh;
                case InputSubCategoryEnum.IntervalOctave: return ResourcesForBrailleMusicDecoder.IntervalNameEnum_Octave;
                default: return null;
            }
        }


        public const int bitsPerByte = 8;
        public const int nCategories = sizeof(InputCategoryEnum) * bitsPerByte; // We can't use the "NumberOfCategories" mechanism because the values are flags!!
        public const int nSubCategories = (int)InputSubCategoryEnum.NumberOfSubCategories;
        public const int nSubSubCategories = (int)InputSubSubCategoryEnum.NumberOfSubSubCategories;

        /// <summary>
        /// Returns the category number of this.
        /// The category number is defined as the bitnumber which is the set in the InputCategoryEnum. For instance:
        /// </summary>
        /// <returns>0x0000000000000001 retruns 0,   0x8000000000000000 returns 63,  An illegal value returns -1</returns>
        public int CategoryNumber
        {
            get
            {
                ulong mask = 1;
                for (int i = 0; (i < nCategories); i++)
                {
                    if (0 != (this.category & (InputCategoryEnum)mask))
                    {
                        return i;
                    }
                    mask = mask << 1;
                }
                // No mask is found. Report it as -1
                return -1;
            }
        }

///// <summary>
///// Option for hiding specifig category names, typically used when all information is found in friendlyValue. 
///// </summary>
//private static InputCategoryEnum hiddedInputCategories = InputCategoryEnum.Hand | InputCategoryEnum.Articulation | InputCategoryEnum.EmbeddedTextRepresentation | InputCategoryEnum.Note;
//public static InputCategoryEnum HiddedInputCategories { get { return hiddedInputCategories; } set { hiddedInputCategories = value; } }

string ToLocalizedString(InputCategoryEnum inputCategory,InputSubCategoryEnum subCategory, InputSubSubCategoryEnum subSubCategory,  int tokenLength,InputCategoryEnum visibleCategoryNames)
        {
            if (0 == (inputCategory & visibleCategoryNames))
            {
                return "";
            }

            switch (inputCategory)
            {
                case InputCategoryEnum.Accidental:
                    switch (subCategory)
                    {
                        case InputSubCategoryEnum.AccidentalCourtesyFlat: return ResourcesForBrailleMusicDecoder.InputSubCategoryEnum_AccidentalCourtesyFlat;
                        case InputSubCategoryEnum.AccidentalCourtesyNatural: return ResourcesForBrailleMusicDecoder.InputSubCategoryEnum_AccidentalCourtesyNatural;
                        case InputSubCategoryEnum.AccidentalCourtesySharp: return ResourcesForBrailleMusicDecoder.InputSubCategoryEnum_AccidentalCourtesySharp;
                        case InputSubCategoryEnum.AccidentalFlat: return ResourcesForBrailleMusicDecoder.InputSubCategoryEnum_AccidentalFlat;
                        case InputSubCategoryEnum.AccidentalNatural: return ResourcesForBrailleMusicDecoder.InputSubCategoryEnum_AccidentalNatural;
                        case InputSubCategoryEnum.AccidentalSharp: return ResourcesForBrailleMusicDecoder.InputSubCategoryEnum_AccidentalSharp;
                    }
                    // This is an error:
                    return "???";
                case InputCategoryEnum.Articulation: return ResourcesForBrailleMusicDecoder.InputCategoryEnum_Articulation;
                case InputCategoryEnum.Beat: return ResourcesForBrailleMusicDecoder.InputCategoryEnum_Beat;
                case InputCategoryEnum.Character:
                    switch (subCategory)
                    {
                        // We want slightly different presentations of the "Character" category depending on its subcategory:
                        case InputSubCategoryEnum.CharacterBlank: return ResourcesForBrailleMusicDecoder.InputSubCategoryEnum_Blank;
                        case InputSubCategoryEnum.CharacterBlankSequence: return tokenLength.ToString() + " " + ResourcesForBrailleMusicDecoder.InputSubCategoryEnum_BlankSequence; // English for instance: "19 Empty spaces"
                        case InputSubCategoryEnum.CharacterExpandedContraction: return ResourcesForBrailleMusicDecoder.InputSubCategoryEnum_Contraction;
                        default: return ResourcesForBrailleMusicDecoder.InputCategoryEnum_Character + " "; // Need a space between the category and the value for readability
                    }
                case InputCategoryEnum.Clef: return ResourcesForBrailleMusicDecoder.InputCategoryEnum_Clef + " ";
                case InputCategoryEnum.Denominator: return ResourcesForBrailleMusicDecoder.InputCategoryEnum_Denominator;
                case InputCategoryEnum.Digit: return ResourcesForBrailleMusicDecoder.InputCategoryEnum_Digit;
                //                case InputCategoryEnum.EndRepeat: return ResourcesForBrailleMusicDecoder.InputCategoryEnum_EndRepeat;
                case InputCategoryEnum.Finger: return ResourcesForBrailleMusicDecoder.InputCategoryEnum_Finger;
                case InputCategoryEnum.FinalDoubleBar: return ResourcesForBrailleMusicDecoder.InputCategoryEnum_FinalDoubleBar;
                case InputCategoryEnum.SectionalDoubleBar:
                    switch (subCategory)
                    {
                        case InputSubCategoryEnum.SectionalDoubleBarFollowedByBarline: return ResourcesForBrailleMusicDecoder.InputCategoryEnum_SectionalDoubleBar + " + " + ResourcesForBrailleMusicDecoder.InputCategoryEnum_NewMeasure;
                        default: return ResourcesForBrailleMusicDecoder.InputCategoryEnum_SectionalDoubleBar;
                    }
                case InputCategoryEnum.Hand: return ResourcesForBrailleMusicDecoder.InputCategoryEnum_Hand; // Al information found in friendlyValue
                case InputCategoryEnum.InAccordFullMeasure: return ResourcesForBrailleMusicDecoder.InputCategoryEnum_InAccordFullMeasure;
                case InputCategoryEnum.InAccordPartMeasure: return ResourcesForBrailleMusicDecoder.InputCategoryEnum_InAccordPartMeasure;
                case InputCategoryEnum.Interval:
                    string intervalName = ToLocalizedIntervalName(subCategory);
                    string occurances = (subSubCategory == InputSubSubCategoryEnum.OccursTwice) ? " * 2 " : "";
                    string intervalResult = string.Format("{0} {1}{2}", ResourcesForBrailleMusicDecoder.InputCategoryEnum_Interval, intervalName, occurances);
                    return intervalResult;
                case InputCategoryEnum.MeasureDivision: return ResourcesForBrailleMusicDecoder.InputCategoryEnum_MeasureDivision;
                case InputCategoryEnum.NewMeasure: return ResourcesForBrailleMusicDecoder.InputCategoryEnum_NewMeasure;
                case InputCategoryEnum.Note: return ResourcesForBrailleMusicDecoder.InputCategoryEnum_Note;
                case InputCategoryEnum.Octave: return ResourcesForBrailleMusicDecoder.InputCategoryEnum_Octave;
                case InputCategoryEnum.PartMeasureRepeat:
                    {
                        switch (subCategory)
                        {
                            case InputSubCategoryEnum.PartMeasureRepeatOnce: return ResourcesForBrailleMusicDecoder.InputCategoryEnum_OthervaluesPartMeasureRepeat;
                            case InputSubCategoryEnum.PartMeasureRepeatTwice: return ResourcesForBrailleMusicDecoder.InputSubCategoryEnum_OtherValuesPartMeasureRepeatTwice;
                            default:
                                Logger.LogCF(string.Format(": Unimplemented other value {0}", subCategory));
                                return "?";
                        }
                    }
                case InputCategoryEnum.OtherValues:
                    switch (subCategory)
                    {
                        case InputSubCategoryEnum.OthervaluesShortAppoggiatura: return ResourcesForBrailleMusicDecoder.InputCategoryEnum_OthervaluesShortAppoggiatura;
                        case InputSubCategoryEnum.OthervaluesLongAppoggiatura: return ResourcesForBrailleMusicDecoder.InputCategoryEnum_OthervaluesLongAppoggiatura;
                        case InputSubCategoryEnum.OtherValuesOrnament: return ResourcesForBrailleMusicDecoder.InputCategoryEnum_Othervalues_Ornament;
                        //case InputSubCategoryEnum.OtherValuesPartMeasureRepeat: return ResourcesForBrailleMusicDecoder.InputCategoryEnum_OthervaluesPartMeasureRepeat;
                        //case InputSubCategoryEnum.OtherValuesPartMeasureRepeatTwice: return ResourcesForBrailleMusicDecoder.InputSubCategoryEnum_OtherValuesPartMeasureRepeatTwice;
                        case InputSubCategoryEnum.OtherValuesTrill: return ResourcesForBrailleMusicDecoder.InputCategoryEnum_OthervaluesTrill;
                        case InputSubCategoryEnum.OthervaluesDoubleBarFollowedByDots: return ResourcesForBrailleMusicDecoder.InputCategoryEnum_OthervaluesDoubleBarFollowedByDots;
                        case InputSubCategoryEnum.OthervaluesDoubleBarPrecededByDots: return ResourcesForBrailleMusicDecoder.InputCategoryEnum_OthervaluesDoubleBarPrecededByDots;
                        // Endings
                        case InputSubCategoryEnum.OthervaluesVolta1FirstEnding: return ResourcesForBrailleMusicDecoder.InputCategoryEnum_OthervaluesPrimaVoltaFirstEnding;
                        case InputSubCategoryEnum.OthervaluesVoltaIntervalEnding: return ResourcesForBrailleMusicDecoder.InputCategoryEnum_OthervaluesIntervalEnding;
                        case InputSubCategoryEnum.OthervaluesVoltaNumericEnding: return ResourcesForBrailleMusicDecoder.InputCategoryEnum_OthervaluesNumericEnding;
                        case InputSubCategoryEnum.OthervaluesVolta2SecondEnding: return ResourcesForBrailleMusicDecoder.InputCategoryEnum_OthervaluesPrimaVoltaSecondEnding; // Seconda Secunda
                                                                                                                                                                             // Tremoloes
                        case InputSubCategoryEnum.OthervaluesTremoloRepeatedNote: return ResourcesForBrailleMusicDecoder.InputSubCategoryEnum_OthervaluesTremoloRepeatedNote;
                        case InputSubCategoryEnum.OthervaluesTremoloAlternatingNotes: return ResourcesForBrailleMusicDecoder.InputSubCategoryEnum_OthervaluesTremoloAlternatingNotes;
                        case InputSubCategoryEnum.OthervaluesFullMeasureRepeat: return ResourcesForBrailleMusicDecoder.InputSubCategoryEnum_OthervaluesFullMeasureRepeat;
//                        case InputSubCategoryEnum.OthervaluesFullMeasureRepeatOnce: return ResourcesForBrailleMusicDecoder.InputSubCategoryEnum_OthervaluesFullMeasureRepeatOnce;
                        case InputSubCategoryEnum.OthervaluesFullMeasureRepeatTwice: return ResourcesForBrailleMusicDecoder.InputSubCategoryEnum_OthervaluesFullMeasureRepeatTwice;
                        case InputSubCategoryEnum.OthervaluesFullMeasureRepeatNoInitialBlank: return ResourcesForBrailleMusicDecoder.InputSubCategoryEnum_OthervaluesFullMeasureRepeatNoInitialBlank;
                        default:
                            Logger.LogCF(string.Format(": Unimplemented other value {0}", subCategory));
                            return "?";
                    }
                case InputCategoryEnum.Punctuation:
                    switch (subCategory)
                    {
                        case InputSubCategoryEnum.PunctuationSingle: return ResourcesForBrailleMusicDecoder.InputCategoryEnum_Punctuation;
#warning todo localization
                        case InputSubCategoryEnum.PunctuationDouble: return "Dobbelt " + ResourcesForBrailleMusicDecoder.InputCategoryEnum_Punctuation;
                        case InputSubCategoryEnum.PunctuationTriple: return "Tredobbelt " + ResourcesForBrailleMusicDecoder.InputCategoryEnum_Punctuation;
                        default:
                            Logger.LogCF(string.Format(": Unimplemented punctuation {0}", subCategory));
                            return "?";
                    }
                case InputCategoryEnum.Rest: return ResourcesForBrailleMusicDecoder.InputCategoryEnum_Rest;
                case InputCategoryEnum.Slur:
                    switch (subCategory)
                    {
                        case InputSubCategoryEnum.SlurNormal:
                            return (subSubCategory == InputSubSubCategoryEnum.OccursTwice) ? ResourcesForBrailleMusicDecoder.InputSubCategoryEnum_SlurDouble : ResourcesForBrailleMusicDecoder.InputSubCategoryEnum_SlurNormal;
                        case InputSubCategoryEnum.SlurDouble: return ResourcesForBrailleMusicDecoder.InputSubCategoryEnum_SlurDouble;
                        case InputSubCategoryEnum.SlurStartBracketSlur: return ResourcesForBrailleMusicDecoder.InputSubCategoryEnum_SlurStartBracketSlur;
                        case InputSubCategoryEnum.SlurEndBracketSlur: return ResourcesForBrailleMusicDecoder.InputSubCategoryEnum_SlurEndBracketSlur;
                        case InputSubCategoryEnum.SlursThatDoNotLeadtoNotes: return ResourcesForBrailleMusicDecoder.InputSubCategoryEnum_SlursThatDoNotLeadtoNotes;
                        default:
                            Logger.LogCF(string.Format(": Unimplemented slur {0}", subCategory));
                            return ResourcesForBrailleMusicDecoder.InputSubCategoryEnum_SlurDefault;
                    }
                case InputCategoryEnum.Space: return ResourcesForBrailleMusicDecoder.InputCategoryEnum_Space;
                case InputCategoryEnum.TextVersal:
                    switch (subCategory)
                    {
                        case InputSubCategoryEnum.TextVersalSymbol: return ResourcesForBrailleMusicDecoder.InputCategoryEnum_TextVersal;
                        case InputSubCategoryEnum.TextVersalWord: return ResourcesForBrailleMusicDecoder.InputCategoryEnum_TextVersals;
                        case InputSubCategoryEnum.TextVersalPassage: return ResourcesForBrailleMusicDecoder.InputCategoryEnum_TextVersals;
                    }
                    return ResourcesForBrailleMusicDecoder.InputCategoryEnum_TextVersal;
                case InputCategoryEnum.Tie: return ResourcesForBrailleMusicDecoder.InputCategoryEnum_Tie;
                case InputCategoryEnum.TimeModification: return ResourcesForBrailleMusicDecoder.InputCategoryEnum_TimeModification;
                case InputCategoryEnum.ToMusicBraille:
                    // Add a concatenator if followed by a hand specification to make text more readable for JAWS
                    string concatenator = ((InputSubCategoryEnum.HandLeft == subCategory) || (InputSubCategoryEnum.HandRight == subCategory)) ? " " : "";
                    return ResourcesForBrailleMusicDecoder.InputCategoryEnum_ToMusicBraille + concatenator; 
                case InputCategoryEnum.ToNumber: return ResourcesForBrailleMusicDecoder.InputCategoryEnum_ToNumber;
                case InputCategoryEnum.ToText: return ResourcesForBrailleMusicDecoder.InputCategoryEnum_ToText;
                case InputCategoryEnum.ToWord: return ResourcesForBrailleMusicDecoder.InputCategoryEnum_ToWord;
                case InputCategoryEnum.UnusualBarLine: return ResourcesForBrailleMusicDecoder.InputCategoryEnum_UnusualBarLine;

                case InputCategoryEnum.Chords: return ResourcesForBrailleMusicDecoder.InputCategoryEnum_Chords; //"Becifringer";
                case InputCategoryEnum.PrintPagination: return ResourcesForBrailleMusicDecoder.InputCategoryEnum_PrintPagination;
                case InputCategoryEnum.ControlCharCRLF:
                    switch (subCategory)
                    {
                        case InputSubCategoryEnum.None: return ResourcesForBrailleMusicDecoder.InputCategoryEnum_ControlCharCRLF;
                        case InputSubCategoryEnum.ControlCharCRLFContinued: return ResourcesForBrailleMusicDecoder.InputCategoryEnum_ControlCharCRLFContinuedLine;                      
                    }
                    return "?";
                case InputCategoryEnum.ControlCharFF: return ResourcesForBrailleMusicDecoder.InputCategoryEnum_ControlCharFF;
                case InputCategoryEnum.ControlCharCRLFNumber: return ResourcesForBrailleMusicDecoder.InputCategoryEnum_ControlCharCRLFMeasureNumber;
                case InputCategoryEnum.EmbeddedTextRepresentation:
                    switch (subCategory)
                    {
                        case InputSubCategoryEnum.TextCrescentoEnd: return ResourcesForBrailleMusicDecoder.InputSubCategoryEnum_CrescendoEnd;
                        case InputSubCategoryEnum.TextCrescentoStart: return ResourcesForBrailleMusicDecoder.InputSubCategoryEnum_CrescendoStart;
                        case InputSubCategoryEnum.TextDiminiuendoEnd: return ResourcesForBrailleMusicDecoder.InputSubCategoryEnum_DiminuendoEnd;
                        case InputSubCategoryEnum.TextDiminiuendoStart: return ResourcesForBrailleMusicDecoder.InputSubCategoryEnum_DiminuendoStart;
                        // The remaining fixed text need no localization because they are always expressed in italian language
                        case InputSubCategoryEnum.TextForte: return "Forte";
                        case InputSubCategoryEnum.TextForteFortissimo: return "Forte Fortissimo";
                        case InputSubCategoryEnum.TextMezzoForte: return "MezzoForte";
                        case InputSubCategoryEnum.TextMezzoPiano: return "MezzoPiano";
                        case InputSubCategoryEnum.TextPiano: return "Piano";
                        case InputSubCategoryEnum.TextPianoPianissimo: return "Piano Pianissimo";
                        case InputSubCategoryEnum.TextFreeText:
                            return ResourcesForBrailleMusicDecoder.InputCategoryEnum_EmbeddedTextFreeText;
                        default: return "?";
                    }
                case InputCategoryEnum.LineContinuation: return ResourcesForBrailleMusicDecoder.InputCategoryEnum_LineContinuation;
                case InputCategoryEnum.InAccordTie: return ResourcesForBrailleMusicDecoder.InputCategoryEnum_InAccordTie;
                case InputCategoryEnum.InsertedRest: return ResourcesForBrailleMusicDecoder.InputCategoryEnum_InsertedRest;
                case InputCategoryEnum.GeneralSigns:
                    switch (subCategory)
                    {
#warning todo localize
                        case InputSubCategoryEnum.PianoPedalDown: return ResourcesForBrailleMusicDecoder.InputSubCategoryEnum_PianoPedalDown;
                        case InputSubCategoryEnum.SmallInvertedArchAboveNote: return ResourcesForBrailleMusicDecoder.InputCategoryEnum_SmallInvertedArchAboveNote;
                        case InputSubCategoryEnum.SquareBracketBelowStaffStart: return ResourcesForBrailleMusicDecoder.InputSubCategoryEnum_SquareBracketBelowStaffStart;
                        case InputSubCategoryEnum.SquareBracketBelowStaffEnd: return ResourcesForBrailleMusicDecoder.InputSubCategoryEnum_SquareBracketBelowStaffEnd;
                        default: return "?";
                    }

                //case InputCategoryEnum.ChordSymbol: return ResourcesForBrailleMusicDecoder.InputCategoryEnum_ChordSymbol;
                case InputCategoryEnum.ChordSymbol: // Compare to similar code in MusicXmlStateHarmonyPart.cs
                    unlocalizedFriendlyValue = ""; // Hide the original contents of unlocalizedFriendlyValue for all ChordSymbols
                    string s = ResourcesForBrailleMusicDecoder.InputCategoryEnum_ChordSymbol + " ";
                    switch (subCategory)
                    {
                        // More cases to come here ! Find  the exact chord kinds in MidiCorrd.cs
                        case InputSubCategoryEnum.ChordSymbolSus2: return s + ResourcesForBrailleMusicDecoder.InputSubCategoryEnum_ChordSus2;
                        case InputSubCategoryEnum.ChordSymbolSus4: return s + ResourcesForBrailleMusicDecoder.InputSubCategoryEnum_ChordSus4;
                        case InputSubCategoryEnum.ChordSymbolMinor: return s + ResourcesForBrailleMusicDecoder.InputSubCategoryEnum_ChordMinor;
                        case InputSubCategoryEnum.ChordSymbolDim: return s + ResourcesForBrailleMusicDecoder.InputSubCategoryEnum_ChordDiminished; // A circle without a crossing line
                        case InputSubCategoryEnum.ChordSymbolMaj: return s + ResourcesForBrailleMusicDecoder.InputSubCategoryEnum_ChordMaj7;
                        case InputSubCategoryEnum.ChordSymbolAug: return s + ResourcesForBrailleMusicDecoder.InputSubCategoryEnum_ChordAugmented;
                        // case InputSubCategoryEnum.ChordSymbolMinus: return s + "-";
                        case InputSubCategoryEnum.ChordSymbolHalfDim: return s + ResourcesForBrailleMusicDecoder.InputSubCategoryEnum_ChordHalfDiminished; // A circle with a crossing line
                        case InputSubCategoryEnum.ChordSymbolNatural: return s + ResourcesForBrailleMusicDecoder.InputSubCategoryEnum_ChordSymbolNatural;
                        case InputSubCategoryEnum.ChordSymbolSharp: return s + ResourcesForBrailleMusicDecoder.InputSubCategoryEnum_ChordSymbolSharp;
                        case InputSubCategoryEnum.ChordSymbolFlat: return s + ResourcesForBrailleMusicDecoder.InputSubCategoryEnum_ChordSymbolFlat;

                        case InputSubCategoryEnum.None: return s; // Represents "Major"
                        default:
                            Logger.LogCF(string.Format(": Unexpectedted InputCategoryEnum.ChordSymbol={0}", SubCategory));
                            return "?";
                    }

                case InputCategoryEnum.KeySignature: return ResourcesForBrailleMusicDecoder.InputCategoryEnum_KeySignature;
                case InputCategoryEnum.KeySignatureText:
                    this.localizedFriendlyValue = GetLocalizedKeySignatureText(); // Especially for this explicit category: Handle the localization here to help the MuxicXml generator
                    this.localizedCategoryName = ResourcesForBrailleMusicDecoder.InputCategoryEnum_KeySignatureText;
                    return ResourcesForBrailleMusicDecoder.InputCategoryEnum_KeySignatureText;
#warning TODO Localize remaining categories
                case InputCategoryEnum.GuideDots: return ResourcesForBrailleMusicDecoder.InputCategoryEnum_GuideDots;

                // The following categories actually occur in the current test files, so they have top priority for implementation!
                // Denne morgens mulighed:
                case InputCategoryEnum.ToNumberLowered: return ResourcesForBrailleMusicDecoder.InputCategoryEnum_ToNumberLowered;
                case InputCategoryEnum.LoweredDigit: return ResourcesForBrailleMusicDecoder.InputCategoryEnum_LoweredDigit;
                case InputCategoryEnum.ChordSymbolBass: return ResourcesForBrailleMusicDecoder.InputCategoryEnum_ChordSymbolBass;
                case InputCategoryEnum.ChordSymbolRoot:
                    switch (subCategory)
                    {
                        case InputSubCategoryEnum.ChordSymbolRootNoRoot: return ResourcesForBrailleMusicDecoder.InputSubCategoryEnum_ChordSymbolRootNoRoot;
                        default: return ResourcesForBrailleMusicDecoder.InputCategoryEnum_ChordSymbolRoot;
                    }
                case InputCategoryEnum.ChordStemSign: return ResourcesForBrailleMusicDecoder.InputCategoryEnum_ChordStemSign;
                case InputCategoryEnum.ChordTiming:  return ResourcesForBrailleMusicDecoder.InputCategoryEnum_ChordTiming;
                case InputCategoryEnum.ChordNumericExtension: return ResourcesForBrailleMusicDecoder.InputCategoryEnum_ChordNumericExtension;
                // Ulandsvise
                case InputCategoryEnum.DigitSpecialCharacter: return ResourcesForBrailleMusicDecoder.InputCategoryEnum_DigitSpecialCharacter;
                case InputCategoryEnum.PageNumber:   return ResourcesForBrailleMusicDecoder.InputCategoryEnum_PageNumber; 
                case InputCategoryEnum.Hyphen:       return ResourcesForBrailleMusicDecoder.InputCategoryEnum_Hyphen;

                // Nu er jord og himmel stille
                case InputCategoryEnum.RepeatSequence: return this.OnRepeatSequence();

                case InputCategoryEnum.TypeFormIndicator:
                    switch (subCategory)
                    {
                        case InputSubCategoryEnum.TypeFormIndicatorUnderlineStart: return ResourcesForBrailleMusicDecoder.InputSubCategoryEnum_TypeFormIndicatorUnderlineStart;
                        case InputSubCategoryEnum.TypeFormIndicatorUnderlineEnd: return ResourcesForBrailleMusicDecoder.InputSubCategoryEnum_TypeFormIndicatorUnderlineEnd;
                        default: return "?";
                    }

                case InputCategoryEnum.SectionHeader: return ResourcesForBrailleMusicDecoder.InputCategoryEnum_SectionHeader;

                case InputCategoryEnum.DaCapoAndDalSegno:
                    switch (subCategory)
                    {
                        case InputSubCategoryEnum.DaCapoAndDalSegnoPrintSegno: return ResourcesForBrailleMusicDecoder.InputSubCategoryEnum_GraphicSymbolSegno; 

                        case InputSubCategoryEnum.EndOfBrailleOnlySegnoPassage:
                            // Logger.LogCF(string.Format(": Category={0} Unimplemented subcategory={1} !!!TO BE IMPLEMENTED!!!", category, subCategory));
                            return "NOT IMPLEMENTED YET: EndOfBrailleOnlySegnoPassage  NOT IMPLEMENTED YET!!!";

                        case InputSubCategoryEnum.PrintDaCapoOrDC:
                        case InputSubCategoryEnum.BrailleOnlyDaCapo:
                        case InputSubCategoryEnum.BrailleOnlySegnoWithLetter:
                        case InputSubCategoryEnum.BrailleOnlyDalSegnoWithLetter:
                        case InputSubCategoryEnum.PrintEncircledCrossCodaSign:
                            Logger.LogCF(string.Format(": Category={0} Unimplemented subcategory={1}", category, subCategory));
                            return "?";

                        default:
                            Logger.LogCF(string.Format(": Category={0} Unexpected subcategory={1}", category, subCategory));
                            return "?";
                    }

                default: return inputCategory.ToString(); // Default for switch inputCategory
            }

        }

        private string OnRepeatSequence()
        {
            // Here we need to access some of the member variables to build a decent localization: Something like ""
            string repeatSequenceString = ResourcesForBrailleMusicDecoder.InputCategoryEnum_RepeatSequence;

            string offsetString = this.Values[0];
            string lengthString = this.Values[1];
            int offset = int.Parse(offsetString);
            int length = int.Parse(lengthString);
            string result = string.Format("{0} ({1},{2})", repeatSequenceString, offset, length); // 
            if (offset == length)
            {
                // The simple case: We can make it look better:
                string measure = ResourcesForBrailleMusicDecoder.InputCategoryEnum_RepeatSequence_Measure;
                string measures = ResourcesForBrailleMusicDecoder.InputCategoryEnum_RepeatSequence_Measures;
                result = string.Format("{0} {1} {2}", repeatSequenceString, length, (1 == length) ? measure : measures);
            }
            return result;
        }



        private string GetLocalizedKeySignatureText()
        {
            string result = null;
            int n = 0;
            if (!(int.TryParse(unlocalizedFriendlyValue, out n) || (n < -7) || (n > 7))) 
            {
                Logger.LogCF(string.Format(": Unexpected friendlyValue {0} Not a valid number", unlocalizedFriendlyValue));
                return null;
            }
            if (0 == n)
            {
                return ResourcesForBrailleMusicDecoder.InputCategoryEnum_KeySignatureTextNone; // "None" / "Ingen"
            }  
            // We need to find out which Key signature          
            char c = unlocalizedFriendlyValue[0];
            switch (c)
            {
                case '+': result= string.Format("{0} {1}", +n, ResourcesForBrailleMusicDecoder.InputCategoryEnum_KeySignatureTextSharp); break;
                case '-': result= string.Format("{0} {1}", -n, ResourcesForBrailleMusicDecoder.InputCategoryEnum_KeySignatureTextFlat); break;
                default: result = string.Format("{0} {1}",  n, ResourcesForBrailleMusicDecoder.InputCategoryEnum_KeySignatureTextNatural); break;
            }
            if (null == result)
            {
                Logger.LogCF(string.Format(": Unexpected friendlyValue {0}", unlocalizedFriendlyValue));
            }
            return result;
        }


        // The category of value that this instance represents, for instance a musical note, a digit or a letter
        internal InputCategoryEnum Category
        {
            get
            {
                return category;
            }
        }

    }
}
