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
    abstract class BrailleFileHandler
    {
        protected const byte CarriageReturn = 13;
        protected const byte LineFeed = 10;
        protected const byte FormFeed = 12;

        // Some general information about Braille file formats:
        //
        // https://www.ukaaf.org/wp-content/uploads/2017/03/ReadingElectronicBrailleFinal.pdf
        // https://en.wikipedia.org/wiki/Braille_ASCII 
        // https://en.wikipedia.org/wiki/Computer_Braille_Code 
        // https://en.wikipedia.org/wiki/Unified_English_Braille 
        // https://en.wikipedia.org/wiki/Braille_Patterns


        public enum FileFormat{BRF_ASCII, BRF_Unicode, PEF};

        abstract public string GetExtension();
        abstract public string GetFileFormat(); 
        abstract public bool WriteToFile(string unicodeBraille, string fullFileName, bool acceptControls);


        /// Formats a list of UNICODE strings, each representing a musical event  in to a single UNICODE,
        /// string taking into account the dimensions of the sheet to print on  
        /// </summary>
        /// <param name="unicodeBrailleList"></param>
        /// <param name="fullFileName"></param>
        /// <returns></returns>
        public string Format(List<string> unicodeBrailleList)
        {
            int lineWidth = 8; //  8 characters per line
            int formHeight = 20; // 20 lines per form

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
                        score.Append((char)CarriageReturn);
                        score.Append((char)LineFeed);
                        numberOfLines++;
                        currentWidth = 0;
                        int length = Math.Min(lineWidth, remainingChars.Length);
                        nextLine =  remainingChars.Substring(0,length);
                        remainingChars = remainingChars.Substring(length, remainingChars.Length - length);
                    }
                    currentWidth += nextLine.Length;
                    score.Append(nextLine);
                    currentHeight++;

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
            Logger.LogCF(string.Format(": Generated {0} forms containing {1} lines", numberOfForms, numberOfLines));
            return score.ToString();
        }

        
        public bool WriteToFile(List<string> unicodeBrailleList, string fullFileName)
        {
            string score = Format(unicodeBrailleList);
            return WriteToFile(score, fullFileName, true); // Call the abstract implementation
        }



        ///// <summary>
        ///// Writes the byteList to the file specified without any conversion !
        ///// </summary>
        ///// <param name="byteList"></param>
        ///// <param name="fullFileName"></param>
        ///// <returns></returns>
        //public bool WriteToFile(List<byte> byteList, string fullFileName)
        //{
        //    byte[] byteArray = byteList.ToArray();
        //    return WriteToFile(byteArray, fullFileName);
        //}


//        /// <summary>
//        /// Writes the byteArray to the file without any conversion
//        /// </summary>
//        /// <param name="byteArray"></param>
//        /// <param name="fullFileName"></param>
//        /// <returns></returns>
//        public bool WriteToFile(byte[] byteArray, string fullFileName)
//        {
//            using (BinaryWriter bw = new BinaryWriter(File.Open(fullFileName, FileMode.Create)))
//            {
//                bw.Write(byteArray);
//            }
//#warning ToDO Error handling
//            bool result = true;
//            return result;
//        }



        /// <summary>
        /// Generates the Music Mraille representation for the score described in events.
        /// </summary>
        /// <param name="events"></param>
        /// <param name="userSettings"></param>
        /// <param name="fullFileName"></param>
        /// <returns></returns>
        public bool WriteToFile(EventDescriptionList events, UserSettings userSettings,  string fullFileName)
        {
#warning TODO Save and restore USerSettings                   
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
            Logger.LogCF(string.Format(": Writing {0} Braille characters from {1} eventdescriptions to {2}", score.Length, eventList.Count,fullFileName));
            // return this.WriteToFile(score, fullFileName);  // Ignoring width and height
            return this.WriteToFile(eventList, fullFileName); // Taking in account width and height
        }

        ///// <summary>
        /////  Write an array of 16-bit characters to a file
        ///// </summary>
        ///// <param name="chars"></param>
        ///// <returns></returns>
        //public bool WriteToFile(List<Char> chars, string fullFileName)
        //{
        //    throw (new Exception("Not Implemented yet"));
        //    bool result = true;
        //    return result;
        //}
        //// Tests



        //public bool TestByteList(string fileName)
        //{
        //    List<byte> byteList = new List<byte>();
        //    for (byte i = 0; (i < 64); i++)
        //    {
        //        byteList.Add(i);
        //    }
        //    return this.WriteToFile(byteList,fileName + "ByteList.bin"); // Calls a non-abstract method for test
        //}

        //public bool TestByteArray(string fileName)
        //{
        //    List<byte> testBytes = new List<byte>();
        //    for (byte i = 0; (i < 64); i++)
        //    {
        //        testBytes.Add(i);
        //    }
        //    List<byte> byteArray = new List<byte>(testBytes);
        //    return this.WriteToFile(byteArray,fileName + "ByteArray.bin"); // Calls a non-abstract method for test
        //}

        //public bool TestUnicodeBraille(string fileName)
        //{
        //    int length = 64;
        //    StringBuilder sb = new StringBuilder(length);
        //    for (int i = 0; (i < length); i++)
        //    {
        //        char c = (char) (i + 0x2800);
        //        sb.Append(c);
        //    }
        //    string s = sb.ToString();
        //    return this.WriteToFile(s,fileName + "Unicode.bin",false);
        //}


        // Test


        public abstract string ReadFromFile(string fullFileName);


        /// <summary>
        /// Generate a testpattern containing all 64 possible 6-point Braille glyphs and write it to a file
        /// </summary>
        /// <param name="directoryName"></param>
        public void GenerateTestpattern(string directoryName)
        {
            string fullFileName = Path.Combine(directoryName, "MusicBrailleTestPattern" + "."  + GetFileFormat() + "." + GetExtension());
            StringBuilder sb = new StringBuilder();
            {
                for (int i = 0x2800; i < 0x2840; i++)
                {
                    char c = (char)(i);
                    sb.Append(c);
                }
            }
            string testPattern = sb.ToString();
            List<string> list = new List<string>();
            list.Add(testPattern); // A list containing only one item!
            this.WriteToFile(list, fullFileName);
            Logger.LogCF(string.Format(": Wrote testpattern to {0}", fullFileName));

        }


        // Construction

        public static BrailleFileHandler Create(FileFormat fileFormat)
        {
            switch (fileFormat)
            {
                case FileFormat.BRF_ASCII: return new BrailleFileHandler_BRF_ASCII();
                case FileFormat.BRF_Unicode: return new BrailleFileHandler_BRF_Unicode();
                default:
                    Logger.LogCF(string.Format(": Unsupported file format {0}", fileFormat.ToString()));
                    return null;
            }
        }

        public static BrailleFileHandler Create()
        {
            return BrailleFileHandler.Create(FileFormat.BRF_ASCII);
        }

    }

    //****************************************************************

    //****************************************************************

}
