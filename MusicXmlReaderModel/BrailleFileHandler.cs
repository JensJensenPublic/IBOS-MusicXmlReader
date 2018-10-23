using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MusicXmlReaderModel
{

    /// <summary>
    /// Handles generation of files containing Braille information
    /// </summary>
    public abstract class BrailleFileHandler
    {
        protected const byte CarriageReturn = 13;
        protected const byte LineFeed = 10;
        protected const byte FormFeed = 12;

        protected int charsPerLine = 14;
        protected int linesPerForm = 32;

        protected byte[] byteMap; //  Maps from a UNICODE 0x2800..0x283F char to a byte.    Is filled in during initialization !
        protected char[] charMap; //  Maps from a byte to a UNICODE char in 0x2800..0x283F  Is filled in during initialization !

        // Some general information about Braille file formats:
        //
        // https://www.ukaaf.org/wp-content/uploads/2017/03/ReadingElectronicBrailleFinal.pdf
        // https://en.wikipedia.org/wiki/Braille_ASCII 
        // https://en.wikipedia.org/wiki/Computer_Braille_Code 
        // https://en.wikipedia.org/wiki/Unified_English_Braille 
        // https://en.wikipedia.org/wiki/Braille_Patterns


        public enum FileFormat{Unknown, BRF_ASCII, BRF_Unicode, PEF, BRL_OctoBraille_1252};

        // The following methods need separate implementations 
        abstract public string GetExtension();
        abstract public string GetFileFormat();

        /// <summary>
        ///  /// Reads a file containing MusicBraille information and returns its contents as a UNICODE string
        /// </summary>
        /// <param name="fullFileName"></param>
        /// <returns></returns>
        abstract public string ReadFromFile(string fullFileName);

        abstract public bool   WriteToFile(string unicodeBraille, string fullFileName, bool acceptControls);


        protected void AddControls()
        {
            // Map 3 controls to their Unicode equivalents
            charMap[LineFeed] = (char)LineFeed;
            charMap[FormFeed] = (char)FormFeed;
            charMap[CarriageReturn] = (char)CarriageReturn;
        }

        protected void CheckTables(int mapLength)
        {
            // Check both
            bool ok = true;
            const int BrailleBase = 0x2800;
            for (int i = BrailleBase; i < BrailleBase + mapLength; i++)
            {
                byte b = byteMap[i - BrailleBase];
                int result = charMap[b];
                if (i != result)
                {
                    Logger.LogCF(string.Format(": Initialization error: char=0x{0:x} maps to byte={1} which maps to 0x{2:x}", i, b, result));
                    ok = false;
                }
            }
            if (ok)
            {
                Logger.LogCF(": Passed");
            }
        }


        /// <summary>
        /// Converts a (Unicode-based) string of Braille characters (0x2800..0x283F) to its byte-representation (RBF-ASCII of OctoBraille)
        /// </summary>
        /// <param name="unicodeBraille"></param>
        /// <returns></returns>
        protected byte[] ToBytes(string unicodeBraille, bool acceptControls)
        {
            int length = unicodeBraille.Length;
            byte[] byteArray = new byte[length];
            for (int i = 0; (i < length); i++)
            {
                Char c = unicodeBraille[i];
                if ((c >= 0x2800) && (c <= 0x283F))
                {
                    byte mappedByte = byteMap[c - 0x2800]; // Map from the 0x2800..0x283F interval to the corresponding valie to write to the file
                    byteArray[i] = mappedByte;
                }
                else if (acceptControls && ((c == (char)CarriageReturn) || (c == (char)LineFeed) || (c == (char)FormFeed)))
                {
                    byteArray[i] = (byte)c;
                }

                else
                {
                    string message = string.Format("Illegal value for Unicode Braille = 0x{0:x}", c);
                    byteArray[i] = 0x20; // Insert an empty Braille Character  
                    Logger.LogCF(string.Format(": {0}", message));
                }

            }
            return byteArray;
        }



        /// <summary>
        /// Writes the byteArray to the file without any conversion
        /// </summary>
        /// <param name="byteArray"></param>
        /// <param name="fullFileName"></param>
        /// <returns>true <==> success</returns>
        protected bool WriteToFile(byte[] byteArray, string fullFileName)
        {
            bool result = true;
            // Let the system handle resources:
            using (BinaryWriter bw = new BinaryWriter(File.Open(fullFileName, FileMode.Create)))
            {
                try
                {
                    bw.Write(byteArray);
                }
                catch (Exception e)
                {
                    Logger.LogCFE(e);
                    result = false;
                }
            }
            return result;
        }


        private string ToUnicode(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; (i < bytes.Length); i++)
            {
                byte b = bytes[i];
                sb.Append(charMap[b]);
            }
            return sb.ToString();
        }



        /// <summary>
        /// Reads a file containing MusicBraille information and returns its contents as a UNICODE string
        /// </summary>
        /// <param name="fullFilefileName"></param>
        protected string ReadBytesFromFile(string fullFileName)
        {
            byte[] bytes;
            string result = null;
            try
            {
                bytes = File.ReadAllBytes(fullFileName);
                Logger.LogCF(string.Format(": read {0} bytes from {1}", bytes.Length, fullFileName));
                result = ToUnicode(bytes);
            }
            catch (Exception e)
            {
                result = null;
                Logger.LogCFE(e);
            }
            return result;
        }


        /// <summary>
        /// Simple formatting for notetaker devices. Just keep the existing format !
        /// </summary>
        /// <param name="unicodeBrailleList"></param>
        /// <returns></returns>
        private string FormatForNoteTaker(List<string> unicodeBrailleList)
        {
            StringBuilder score = new StringBuilder();  // Represents the whole score
            foreach (string unicodeBraille in unicodeBrailleList)
            {
                score.Append(unicodeBraille);
                score.Append((char)CarriageReturn);
                score.Append((char)LineFeed);
            }
            return score.ToString();
        }



        /// Formats a list of UNICODE strings, each representing a musical event  in to a single UNICODE,
        /// string taking into account the dimensions of the sheet to print on  
        /// </summary>
        /// <param name="unicodeBrailleList"></param>
        /// <param name="fullFileName"></param>
        /// <returns></returns>
        public string Format(List<string> unicodeBrailleList)
        {
            int lineWidth  =  this.charsPerLine;   
            int formHeight =  this.linesPerForm;

            if (0 == lineWidth && (0 == formHeight))
            {
                return FormatForNoteTaker(unicodeBrailleList);
            }

            StringBuilder score = new StringBuilder();  // Represents the whole score
            int currentWidth = 0;
            int currentHeight = 0;
            int numberOfLines = 0; // For statistics only
            int numberOfForms = 0; // For statistics only

            foreach (string unicodeBraille in unicodeBrailleList)
            {
                string nextLine;
                string remainingChars = unicodeBraille; // Represents one single event
                bool done = false;
                while (!done)
                {
                    if (currentWidth + remainingChars.Length <= lineWidth)
                    {
                        nextLine = remainingChars;
                        done = true;
                        // No need to update remainingChars
                    }
                    else
                    {
                        if (0 != currentWidth)
                        {
                            // No need to insert cf/lf in the start of a form
                            score.Append((char)CarriageReturn);
                            score.Append((char)LineFeed);
                        }
                        numberOfLines++;
                        currentWidth = 0;
                        currentHeight++;
                        int length = Math.Min(lineWidth, remainingChars.Length);
                        nextLine =  remainingChars.Substring(0,length);
                        remainingChars = remainingChars.Substring(length, remainingChars.Length - length);
                    }
                    currentWidth += nextLine.Length;
                    score.Append(nextLine);                

                    // In both cases split in forms if needed

                    if (currentHeight >= formHeight)
                    {
                        // Insert a FF
                        score.Append((char)FormFeed);
                        currentHeight = 0;
                        numberOfForms++;
                    }
                }
            }
            Logger.LogCF(string.Format("(CharsPerLine={0} ,LinesPerForm={1}): Generated {2} forms containing {3} lines", lineWidth, formHeight, numberOfForms, numberOfLines));
            return score.ToString();
        }

        
        public bool WriteToFile(List<string> unicodeBrailleList, string fullFileName)
        {
            string score = Format(unicodeBrailleList);
            return WriteToFile(score, fullFileName, true); // Call the abstract implementation
        }


        /// <summary>
        /// Generates the Music Mraille representation for the score described in events.
        /// </summary>
        /// <param name="events"></param>
        /// <param name="userSettings"></param>
        /// <param name="fullFileName"></param>
        /// <returns></returns>
        public string Format(EventDescriptionList events, UserSettings userSettings)
        {
#warning TODO Save and restore UserPreferences                   
            // Set up for Music Braille. No normal text 
            userSettings.SetAllMusicBrailleSettings(true); // Select all Music Braille Settings (For each part selected above)
            userSettings.SetAllNormalTextSettings(false);   // Select no Normal Text settings   (For each part selected above)
            // Convert the parsed file to MusicBraille
            List<string> eventList = new List<string>();                // Keeps the structure: One event per list element
            StringBuilder scoreAsMusicBraille = new StringBuilder();    // Ignores the structure
            for (int i = 0; (i < events.Events.Count); i++)
            {
                object o = events.Events[i];
                if (o is EventDescription)
                {
                    EventDescription eventDescription = o as EventDescription;
                    BrailleBuilder bb = eventDescription.ToMusicBrailleString();
                    string eventAsMusicBraille = bb.ToBrailleString();
                    eventList.Add( eventAsMusicBraille);                // Keep the structure !
                    scoreAsMusicBraille.Append(eventAsMusicBraille);    // Just append eerything
                }
            }
            string score = scoreAsMusicBraille.ToString();
            //Logger.LogCF(string.Format(": Writing {0} Braille characters from {1} eventdescriptions to {2}", score.Length, eventList.Count,fullFileName));
            // return this.WriteToFile(score, fullFileName);  // Ignoring width and height 
            return this.Format(eventList);

            //return this.WriteToFile(eventList, fullFileName); // Taking in account width and height
        }

        /// <summary>
        /// Generate a testpattern containing all 64 possible 6-point Braille glyphs and write it to a file
        /// </summary>
        /// <param name="directoryName"></param>
        public void GenerateTestpattern(string directoryName)
        {
            string fullFileName = Path.Combine(directoryName, "MusicBrailleTestPattern" + "."  + GetFileFormat() + GetExtension());
            StringBuilder sb = new StringBuilder();
            {
                for (int i0 = 0; (i0 < 16); i0++) // Repeat the test pattern several times
                {
                    for (int i1 = 0x2800; i1 < 0x2840; i1++) // Testpattern: All Unicode values from 0x2800 to 0x283f
                    {
                        char c = (char)(i1);
                        sb.Append(c);
                    }
                }
            }
            string testPattern = sb.ToString();
            List<string> list = new List<string>();
            list.Add(testPattern); // A list containing only one item!
            this.WriteToFile(list, fullFileName); // Ends up in the abstract implementation
            Logger.LogCF(string.Format(": Wrote testpattern to {0}", fullFileName));

        }


        // Construction

        public static BrailleFileHandler Create(FileFormat fileFormat, int charsPerLine, int linesPerForm)
        {
            switch (fileFormat)
            {
                case FileFormat.BRF_ASCII: return new BrailleFileHandler_BRF_ASCII(charsPerLine,linesPerForm);
                case FileFormat.BRF_Unicode: return new BrailleFileHandler_BRF_Unicode(charsPerLine,linesPerForm);
                case FileFormat.BRL_OctoBraille_1252: return new BrailleFileHandler_BRL_OctoBraille_1252(charsPerLine, linesPerForm);
                default:
                    Logger.LogCF(string.Format(": Unsupported file format {0}", fileFormat.ToString()));
                    return null;
            }

        }

        //public static BrailleFileHandler Create()
        //{
        //    return BrailleFileHandler.Create(FileFormat.BRF_ASCII);
        //}

    }

}
