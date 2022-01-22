namespace MusicXmlReaderModel
{
    using System;

    /// <summary>
    /// Identifies the exact type of information (Usable as case in a switch) or as masks
    /// </summary>
    [Flags]
    public enum UserInfoEnum : ulong  // "ulong" enables use of up to 64 bits instead of 31. Same trick as in DecoderOPtions.cs, where the 64 bits are actueally needed!
    {
        // The following values represent 3 diffent kinds of unspecified warnings, using bit 0 to 7
        NoFlags               = 0x00000000,
        AllUnspecifiedFlags   = 0x000000ff,
        Unspecified           = 0x00000001,
        InterpretationWarning = 0x00000002,
        // The following values represent warnings only intended for the developer during development, , using bit 8 to 15
        AllDeveloperFlags     = 0x0000ff00,
        Insertion             = 0x00000100,
        Replacement           = 0x00000200,
        Deletetion            = 0x00000400,
        // The following  values represent warnings intended for the normal end user after File->Open:
        AllBrailleMusicInterpretationFlags = 0x00ff0000,
        NoInterpretationFound              = 0x00010000, 
        MoreThanOneInterpretationFound     = 0x00020000,
        AddedSelectedEvent                 = 0x00040000,
        UnSupportedInput                   = 0x00080000,
        EmptyMeasureIsIgnored              = 0x00100000,
        UnExpectedInputCharacter           = 0x00200000,
        // The following values represent warnings intended for the developer after File->Export to MusicXml
        AllMusicXmlGEnerationFlags         = 0xff000000, 
        UnsupportedMuxicXmlElement         = 0x01000000,
        FailedToFixDuration                = 0x02000000,
        UnSupportedVoiceList               = 0x04000000,
        UnsupportedInAccordConfiguration   = 0x08000000,   
        UnsupportedExtensionToMinorChord   = 0x10000000,
        UnsupportedExtensionToMajorChord   = 0x20000000,
        AllFlags                   = 0xffffffffffffffff
    }

    /// <summary>
    /// Base class for all classes describing user information, such as warnings and errors to be shown in the UI,
    /// either in a MessageBox, in the decoded file or in various types of logfiles.
    /// All information for the user must at least contain a filename and a message
    /// </summary>
    public abstract class UserInfoBase
    {
        protected string fileName;
        public string FileName { get { return fileName; } set { fileName = value; } }

        public abstract string ToString(int fileNameLength);

        public abstract UserInfoEnum GetInfoEnum();

        protected string message;
        public string Message { get { return message; } set { message = value; } }
    }

    /// <summary>
    /// For reporting errors from the XmlBuilder
    /// </summary>
    public abstract class MusicXmlBuilderInfoBase : UserInfoBase
    {
        // To be defined
    }


    /// <summary>
    /// For reporting errors from the MusicXmlReader 
    /// </summary>
    public abstract class MusicXmlReaderBase : UserInfoBase
    {
        // To be defined
    }


    /// <summary>
    /// For reporting errors from the Music Braille Decoder 
    /// Contain positioninformation, which is all contained in the userPositionInfo.  
    /// </summary>
    public abstract class DecoderUserInfoBase : UserInfoBase
    {
        public int Index { get { return userPositionInfo.Index; } }
        public int Page  { get { return userPositionInfo.Form;  } }
        public int Line  { get { return userPositionInfo.Line;  } }
        public int Space { get { return userPositionInfo.Space; } }

        protected UserPositionInfo userPositionInfo = UserPositionInfo.Create(-1, -1, -1, -1); // Avoid silly crashes in derived classes not using the userPositionInfo.
        public UserPositionInfo UserPositionInfo { get { return userPositionInfo; } }

        /// <summary>
        /// The absolute index within the Unicode MusicBraille file
        /// </summary>
        public string IndexString { get { return string.Format("Index={0,-4}", Index); } }
    }

    public class DecoderUserInsertionInfo : DecoderUserInfoBase
    {
        protected string unicodeToInsert;
        public string UnicodeToInsert { get { return unicodeToInsert; } }

        protected string dotsToInsert;
        public string DotsToInsert { get { return dotsToInsert; } }

        public DecoderUserInsertionInfo(string message, UserPositionInfo userPositionInfo, string unicodeToInsert, string dotsToInsert)
        {
            base.message = message;
            base.userPositionInfo = userPositionInfo;
            this.unicodeToInsert = unicodeToInsert;
            this.dotsToInsert = dotsToInsert;
        }

        public override string ToString()
        {
            return ToString(FileName); // No padding
        }

        public override string ToString(int fileNameLength)
        {
            return ToString(FileName.PadRight(fileNameLength));
        }

        private string ToString(string paddedfileName)
        {
            string position = userPositionInfo.ToString();
            return string.Format("{0} {1} {2} DOT {3}", paddedfileName, position, message, dotsToInsert);
        }

        public override UserInfoEnum GetInfoEnum() { return UserInfoEnum.Insertion; } 
    }

    public class DecoderUserReplacementInfo : DecoderUserInfoBase
    {
        protected string oldContents;
        public string OldContents { get { return oldContents; } }
        protected string oldContentsAsDots;
        public string OldContentsAsDots { get { return oldContentsAsDots; } }
        protected string newContents;
        public string NewContents { get { return newContents; } }
        protected string newContentsAsDots;
        public string NewContentsAsDots { get { return newContentsAsDots; } }

        public DecoderUserReplacementInfo(string message, UserPositionInfo userPositionInfo,string oldContents, string oldContentsAsDots, string newContents, string newContentsAsDots)
        {
            base.message = message;
            base.userPositionInfo = userPositionInfo;
            this.oldContents = oldContents;
            this.oldContentsAsDots = oldContentsAsDots;
            this.newContents = newContents;
            this.newContentsAsDots = newContentsAsDots;
        }

        public override string ToString(int fileNameLength)
        {
            return ToString(FileName.PadRight(fileNameLength));
        }

        private string ToString(string paddedfileName)
        {
            string position = userPositionInfo.ToString();
            return string.Format("{0} {1} Replaced Braille={2} (DOTS={3}) by Braille={4} (DOTS={5})", paddedfileName ,position , oldContents, oldContentsAsDots, newContents, newContentsAsDots);
        }

        public override UserInfoEnum GetInfoEnum() { return UserInfoEnum.Replacement; }
    }

    /// <summary>
    /// For retrieving various Decoder state-information to be presented for the user from from anywhere in the project.
    /// </summary>
    public class DecoderUserInfo : DecoderUserInfoBase
    {
        public enum FormattingOptionEnum { Default, Insertion, Replacement, Removal };

        string brailleValue { get; } // Unicode [0x2800..0x283f]
        string brailleDotNumbers { get; } // "123456" for brailleValue = 0x283f

        private FormattingOptionEnum formattingOption = FormattingOptionEnum.Default;
        public FormattingOptionEnum FormattingOption { set { formattingOption = value; } }

        public DecoderUserInfo(string fileName, UserPositionInfo userPosition, string brailleValue, string brailleDotNumbers)
        {
            base.fileName = fileName;
            base.userPositionInfo = userPosition;
            base.message = "";
            this.brailleValue = brailleValue; //  Represents a Unicode character in the Braille6 interval 0x2800..0x283f
            this.brailleDotNumbers = brailleDotNumbers;    
        }


        /// <summary>
        /// NEW CODE
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="userInfoPosition"></param>
        /// <param name="message"></param>
        public DecoderUserInfo(string fileName, UserPositionInfo userPositionInfo, string message)
        {
            base.fileName = fileName;
            base.userPositionInfo = userPositionInfo;            
            base.message = message;
            this.brailleValue = userPositionInfo.BrailleAsUnicode.ToString();
            this.brailleDotNumbers = userPositionInfo.BrailleDotNumbers;
        }


        /// <summary>
        /// A primitive implementation without localization, primarily intended for logging and debugging. 
        /// The application should supply its own localized formatting instead!!
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3}  {4}", fileName, IndexString, PageLineSpace, BrailleDataString, message);
        }

        public override string ToString(int fileNameLength)
        {
            string paddedFileName = FileName.PadRight(fileNameLength);
            return string.Format("{0} {1} {2} {3}  {4}", paddedFileName, IndexString, PageLineSpace, BrailleDataString, message);
        }

        /// <summary>
        /// The posithin within the Unicode Braille file expressed as a page number, a line number within that page and a space number within that line
        /// </summary>
        public string PageLineSpace { get { return string.Format("Page={0,-2}  Line={1,-3}  Space={2,-2} ", Page, Line, Space); } }

        /// <summary>
        /// The Braille data expressed as a Unicode string (containing 1 character within the [0x2800..0x28ff] interval and a string in teh form DOT123456
        /// </summary>
        public string BrailleDataString { get { return string.Format("Braille={0} DOT{1,-6}", brailleValue, brailleDotNumbers); } }

        /// <summary>
        /// The Braille data expressed as a Unicode string (containing 1 character within the [0x2800..0x28ff] interval and a string in teh form DOT123456
        /// </summary>
        public string BrailleDotNumberString { get { return string.Format("DOT{0,-6}",brailleDotNumbers); } }

        private UserInfoEnum userInfoEnum = UserInfoEnum.InterpretationWarning;
        public UserInfoEnum UserInfoEnum { set { userInfoEnum = value; } } // Allow for further specifying the warning type

        public override UserInfoEnum GetInfoEnum() { return userInfoEnum; }

    }




    ///// <summary>
    /////  For retrieving various Decoder state-informationfrom to be presented for the user from anywhere in the project.
    ///// </summary>
    //public interface IDecoderUserInfo
    //{
    //    // string GetLocationinformation();
    //    DecoderUserInfo GetDecoderUserInfo();
    //}
}
