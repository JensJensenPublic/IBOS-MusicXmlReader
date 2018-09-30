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

        // Some general information about Braille file formats:
        //
        // https://www.ukaaf.org/wp-content/uploads/2017/03/ReadingElectronicBrailleFinal.pdf
        // https://en.wikipedia.org/wiki/Braille_ASCII 
        // https://en.wikipedia.org/wiki/Computer_Braille_Code 
        // https://en.wikipedia.org/wiki/Unified_English_Braille 
        // https://en.wikipedia.org/wiki/Braille_Patterns


        public enum FileFormat{BRF_ASCII, BRF_Unicode, PEF};

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
