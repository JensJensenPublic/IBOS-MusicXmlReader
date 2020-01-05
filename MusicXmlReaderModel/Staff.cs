using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MusicXmlReaderModel
{

    /// <summary>
    /// Almost matches the the representation of a staff in a graphic score.
    /// Typically one part is represented in one staff. But for some instruments, such as piano, organ and harp a part can be split in 2 or even more staffs,
    /// each staff representing what is played with one hand or foot
    /// The Staff class is primarily, but not exclusively, used for generating BrailleMusic.
    /// In general, a object of the Staff class can be used for representing:
    /// 1) A part. (This is the typical case)
    /// 2) A subset of a part. (For instance left or right hand for piano, organ or harp part)
    /// 3) A set of parts ( For instance the Soprano + Alto or Tenor + Bas parts when generating Music Braille representation of SATB scores, to be used by a piano or organ player)
    /// </summary>
    public class Staff
    {
        private NoteElementList noteElements;
        public NoteElementList Notes { get { return noteElements; } }
        private int staffNumber;

        /// <summary>
        /// The staff number within the part to which the staff belongs
        /// </summary>
        public int StaffNumber { get { return staffNumber; } }
        private string name = "";
        public string Name { get { return name; } set { name = value; } }
        private readonly string MusicBrailleIndicatorString = BrailleBuilder.Create(0).ToUnicodeString(BrailleBuilder.MusicBrailleIndicator); // 

        private ScorePartElement scorePartElement = null;
        public ScorePartElement ScorePartElement { get { return scorePartElement; } set { scorePartElement = value; } } // Holds a lot of extra information about the part
        // Lots of important information can be derived form the scorePartElement:
        public string PartName
        {
 //           get { return (null == scorePartElement) ? "UndefinedPartName" : Utilities.ToValidFileName(scorePartElement.partName); }
            get { return (null == scorePartElement) ? "" : Utilities.ToValidFileName(scorePartElement.partName); }
        }

        public string PartId
        { get
            {
//                return (null == scorePartElement) ? "UndefinedPartId" : Utilities.ToValidFileName(scorePartElement.partId);
                return (null == scorePartElement) ? "" : Utilities.ToValidFileName(scorePartElement.partId);
            }
        }

        public string ScoreTitle
        {
            get
            {
                string result = "";
                string defaultResult = "NoTitle";
                try
                {
                    // Attempt to avoid null references even if we can catch them.
                    if ((null == owningStaffList) || null == (owningStaffList.MetaInformation))
                    {
                        // We need the Metainformation in all cases, so start by checking if it is available:
                        Utilities.Beep();
                        Logger.LogCF(string.Format(": No metaInformation found. Returning ScoreTitle={0}", defaultResult));
                        return defaultResult;
                    }
                    MetaInformation metaInformation = owningStaffList.MetaInformation;
                    result = "";

                    Logger.LogCF(": Check if MovementTitle.Value is usable");
                    if (null != metaInformation.MovementTitle)
                    {
                        result = metaInformation.MovementTitle.Value;
                    }

                    if (string.IsNullOrEmpty(result))
                    {
                        Logger.LogCF(": Check if Work.Value is usable");
                        if (null != metaInformation.Work)
                        {
                            result = metaInformation.Work.Value;
                        }
                        // Logger.LogCFOnce(string.Format(": Using MetaInformation.Work.Value={0}'", result));
                    }

                    if (string.IsNullOrEmpty(result))
                    {
                        Logger.LogCF(": Check if FileName.Value is usable");
                        if ((null != metaInformation.FileName) && (null != metaInformation.FileName.Value))
                        {
                            string temp = owningStaffList.MetaInformation.FileName.Value;
                            result = string.IsNullOrEmpty(temp) ? "" : Path.GetFileNameWithoutExtension(temp);
                            // Logger.LogCFOnce(string.Format(": Using MetaInformation.Filename.Value={0}'", result));
                        }
                    }

                    if (string.IsNullOrEmpty(result))
                    {
                        result = defaultResult;
                        Logger.LogCF(string.Format(": No title found. Using Title={0}", result));                     
                    }
                    else
                    {
                        Logger.LogCF(string.Format(": Title='{0}'", result));
                    }
  
                }
                catch (Exception e)
                {
                    Logger.LogCFE(e);
                    result = defaultResult;
                }
                return result;
            }
        }


        public string Caption1 { get { return Utilities.ToOneLine(ScoreTitle); } } // ScoreTitle in first line, 
        public string Caption2 { get { return MusicBrailleFilenameAttribute; } } //PartName and StaffNumber in second line

        public BrailleBuilderForText BrailleCaption1
        {
            get
            {
                BrailleBuilderForText bb = BrailleBuilderForText.Create(0);
                bb.AddNormalText(Caption1);
                return bb;
            }
        }

        public BrailleBuilderForText BrailleCaption2
        {
            get
            {
                BrailleBuilderForText bb = BrailleBuilderForText.Create(0);
                bb.AddNormalText(Caption2);
                return bb;
            }
        }

        /// <summary>
        /// The full (End User) Braille representation of this Staff, including a Caption with Title, partName and StaffNumber
        /// </summary>
        public string FullBrailleRepresentation
        {
            get {
                return BrailleCaption1.ToBrailleString() + "\r\n" + //  Title in Braille
                       BrailleCaption2.ToBrailleString() + "\r\n" + // PartName and staff number in Braille
                       MusicBrailleIndicatorString + "\r\n" +       // Indicate the start of Music Braille interpretation
                       BrailleMusicFormattedPage;}}                 // BrailleMusic  in Braille


        /// <summary>
        /// Generate a string for putting on the office door !
        /// </summary>
        public string StringForOfficeDoor
        {
            get
            {
                BrailleBuilderForText bb1 = BrailleBuilderForText.Create(0);
                bb1.AddNormalText("Jens Sundgaard Jensen");
                BrailleBuilderForText bb2 = BrailleBuilderForText.Create(0);
                bb2.AddNormalText("IBOS Nodelæser projektet");
                BrailleBuilderForText bb3 = BrailleBuilderForText.Create(0);
                bb3.AddNormalText("Modul B142");
                string result =
                    bb1.ToBrailleString() + "\r\n" +
                    bb2.ToBrailleString() + "\r\n" +
                    "\r\n" + "\r\n" + "\r\n" + "\r\n" + "\r\n" + "\r\n" + "\r\n" + "\r\n" +
                    bb3.ToBrailleString();
                return result;
            }
        }


        /// <summary>
        /// The full (Developer) Braille representation of this Staff, including a Caption with Title, partName and StaffNumber
        /// </summary>
        public string FullDeveloperBrailleRepresentation
        {
            get { return BrailleCaption1.ToBrailleString() + "\r\n" +                       //  Title in Braille
                         BrailleCaption1.ToEquvivalentTextRepresentation() + "\r\n" +       // Title in plain text
                         BrailleCaption2.ToBrailleString() + "\r\n" +                       // PartName and staff number in Braille
                         BrailleCaption2.ToEquvivalentTextRepresentation() + "\r\n" +       // PartName and staff number in plain text
                         MusicBrailleIndicatorString + "\r\n" +                             // Indicate the start of Music Braille interpretation in Braille
                         "MUSICBRAILLE:" +"\r\n" +                                          // Indicate the start of Music Braille interpretation in plain text
                         BrailleMusicDeveloperPage;                                         // BrailleMusic and text
            }                                           
        }


        public int PartNumber { get { return partNumber; } }
        private int partNumber;

        public bool IsPartOfGrandStaff { get { return isPartOfGrandStaff; } }
        private bool isPartOfGrandStaff;

        private bool enabled;
        public bool Enabled { get { return enabled; } set { enabled = value; } }

        public StaffList OwningStaffList { get { return owningStaffList; } }
        private StaffList owningStaffList;

        /// <summary>
        /// Holds the BrailleMusic representation for this staff as a list of BrailleBuilders (containing timestamps)
        /// </summary>
        public List<BrailleBuilder> BrailleMusicBrailleBuilders { get { return brailleMusicBrailleBuilders; } set { brailleMusicBrailleBuilders = value; } }
        private List<BrailleBuilder> brailleMusicBrailleBuilders = new List<BrailleBuilder>();

        /// <summary>
        /// Holds the BrailleMusic representation for this staff as a list of Unicode strings in the [0x2800..0x28ff] range
        /// </summary>
        public List<string> BrailleMusicStrings { get { return brailleMusicStrings; } }
        private List<string> brailleMusicStrings = new List<string>();

        /// <summary>
        /// Holds the BrailleMusic representation for this staff in a homemade text format only used for debugging.
        /// </summary>
        public List<string> BrailleMusicTexts { get { return brailleMusicTexts; } }
        private List<string> brailleMusicTexts = new List<string>();


        /// <summary>
        /// Holds the BrailleMusic representation for this staff in its final, formatted format, ready for embosser og notetaker
        /// </summary>
        public string BrailleMusicFormattedPage { get { return brailleMusicFormattedPage; } }
        private string brailleMusicFormattedPage = "";

        public int BrailleMusicFormattedPageSize { get { return brailleMusicFormattedPage.Length; } }


        /// <summary>
        /// Holds a special representation used for debugging and development:
        /// Even lines hold the normal BrailleMusic representation (with padding charactres)
        /// Odd lines hold the corresponding Text representation (with padding characters)
        /// The padding characters are inserted to keep the 2 representations vertically aligned !
        /// </summary>
        public string BrailleMusicDeveloperPage { get { return developerPage; } }
        private string developerPage;


        /// <summary>
        /// Converts the list of BrailleBuilders to equvalent lists of BrailleSymbols and testx
        /// </summary>
        public void Unpack(int charsPerLine, int linesPerForm)
        {
            brailleMusicStrings = new List<string>();
            brailleMusicTexts = new List<string>();
            foreach (BrailleBuilder bb in brailleMusicBrailleBuilders)
            {
                brailleMusicStrings.Add(bb.ToBrailleString());
                brailleMusicTexts.Add(bb.ToEquvivalentTextRepresentation());
            }
            brailleMusicFormattedPage = BrailleUtilities.Format(brailleMusicStrings,charsPerLine,linesPerForm);
        }


        private void Transfer(StringBuilder buffer, StringBuilder newValue)
        {
            buffer.Append( newValue);
            buffer.Append("\r\n");
            // Logger.LogCF(string.Format(": {0}", newValue.ToString()));
            newValue.Clear();
        }

        /// <summary>
        /// Developer code for generating an aligned 2-line representation, where the even lines show the BrailleMusic and the odd lines show the corresponding text.        ///
        /// For each BrailleBuilder merges the line of "Real" Braille Information found in BrailleMusicString with the Developer text-information found in BrailleMusicText
        /// generating a pair of lines: The first line of each pair is "Real Braille", the second line is Developer Information of the same.
        /// The lines are padded with "'" in order to align them vertically.
        /// </summary>
        /// <param name="lineLength"></param>
        public void Merge(int lineLength, int linesPerForm)
        {
            StringBuilder brailleMusicString = new StringBuilder();
            StringBuilder brailleMusicText = new StringBuilder();
            StringBuilder combinedString = new StringBuilder();
            int length = 0; // The length of the current BrailleMusic string WITHOUT padding
            //int height = 32;

            int lines = 0;
            foreach (BrailleBuilder bb in brailleMusicBrailleBuilders)
            {
                string nextBraille  = bb.ToBrailleString();
                string nextText =  bb.ToEquvivalentTextRepresentation();
                if ((length + nextBraille.Length) > lineLength)
                {
                    // No room for the next Braille character. We must first transfer to the combined buffer
                    Transfer(combinedString, brailleMusicString);   // One line of BrailleMusic
                    Transfer(combinedString, brailleMusicText);     // One (vertically aligned) line of plain text
                    length = 0;
                    if (lines++ == linesPerForm)                  
                    // if (brailleMusicString.ToString().Contains((char)0x12))
                    {
                        combinedString.Append("\r\n<FORMFEED>\r\n\r\n"); // Test
                        lines = 0;
                    }

                }
                // In order to align vertically between the 2 representations, pad them to the same length.
                int maxLength = Math.Max(nextBraille.Length, nextText.Length);
                // Append the new strings and align lengths by padding.
                length += nextBraille.Length;
                brailleMusicString.Append(nextBraille.PadRight(maxLength, '-'));
                brailleMusicText.Append(nextText.PadRight(maxLength, '-'));
            }
            // Finally transfer the remaining contents
            Transfer(combinedString, brailleMusicString);
            Transfer(combinedString, brailleMusicText);
            this.developerPage = combinedString.ToString();
            // Logger.LogCF(string.Format(": Length of DeveloperPage={0}", this.developerPage.Length));
        }




        public string MusicBrailleFilenameAttribute
        {
            get
            {
                if (string.IsNullOrEmpty(name))
                {
                    //string partString = NoCrLf(PartName);
                    string partString = PartId + "." + Utilities.ToValidFileName(PartName);
                    return IsPartOfGrandStaff ? partString + "." + staffNumber : partString;
                }
                return name;
            }
        }

        /// <summary>
        /// Generate a string to use for debugging
        /// </summary>
        /// <returns></returns>
        public string ToDebugString()
        { 
            if (null == scorePartElement)
            {
                Logger.LogCFOnce(string.Format(": ScorePartElement is null"));
                return "";            
            }
            try
            {
                string partAbbreviation = Utilities.ToValidFileName(scorePartElement.PartAbbreviation);
                string partName = Utilities.ToValidFileName(scorePartElement.partName);
                string instrumentName = "";
                if ((null != scorePartElement.ScoreInstrumentElement) && (null != scorePartElement.ScoreInstrumentElement.InstrumentName))
                {
                    instrumentName = scorePartElement.ScoreInstrumentElement.InstrumentName;
                }
                string message = string.Format(": StaffNumber={0}  Part: Id={1,3} ShortName={2,-10} Name={3,-30} Instrument={4,-30}",
                    staffNumber, PartId, partAbbreviation, partName, instrumentName);
                return message;
            }
            catch (Exception e)
            {            
                Logger.LogCFOnce(string.Format(": Exception.Message={0}", e.Message));               
            }
            return ""; 
        }




        private Staff()
        { }

        private Staff(int partNumber, int staffNumber, bool isPartOfGrandStaff,StaffList owningStaffList)
        {
            this.partNumber = partNumber;
            //this.partId = partId;
            this.staffNumber = staffNumber;
            this.isPartOfGrandStaff = isPartOfGrandStaff;
            this.noteElements = NoteElementList.Create();
            this.owningStaffList = owningStaffList;        
        }


        private Staff(string name, StaffList owningStaffList)
        { 
            this.name = name;
            this.isPartOfGrandStaff = false;
            this.noteElements = NoteElementList.Create();
            this.owningStaffList = owningStaffList;
        }


        /// <summary>
        /// For backward compatibility with 3.0 while still showing Title and showing "IBOS instead of pertnr.staffnr
        /// </summary>
        /// <param name="formattedString"></param>
        /// <param name="owningStaffList"></param>
        /// <param name="name"></param>
        private Staff(string formattedString,StaffList owningStaffList,string name)
        {
            this.brailleMusicFormattedPage = formattedString;
            this.owningStaffList = owningStaffList;
            this.name = name;
            //this.name = "IBOS"; // This will replace the <PartName>.<PartNumber> used by BANA layouts
        }

        public static Staff Create(int partNumber, int staffNumber, bool isPartOfGrandStaff, StaffList owningStaffList)
        {
            return new Staff(partNumber, staffNumber, isPartOfGrandStaff, owningStaffList);
        }

        public static Staff Create(string name, StaffList owningStaffList)
        {
            return new Staff(name, owningStaffList);
        }


        /// <summary>
        /// Used for creating a staff to be used fpr IBOS LAYOUT, assuring compatibility with version 3.0 
        /// </summary>
        /// <param name="formattedString"></param>
        /// <returns></returns>
        public static Staff Create(string formattedString, StaffList owningStaffList, string name)
        {
            return new Staff(formattedString,owningStaffList,name);
        }
    }
}
