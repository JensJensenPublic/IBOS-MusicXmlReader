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
    class BrailleFileHandler
    {
        const byte CarriageReturn = 13;
        const byte LineFeed = 10;
        const byte FormFeed = 12;

        // Some general information about Braille file formats:
        //
        // https://www.ukaaf.org/wp-content/uploads/2017/03/ReadingElectronicBrailleFinal.pdf
        // https://en.wikipedia.org/wiki/Braille_ASCII 
        // https://en.wikipedia.org/wiki/Computer_Braille_Code 
        // https://en.wikipedia.org/wiki/Unified_English_Braille 
        // https://en.wikipedia.org/wiki/Braille_Patterns


        // According to https://en.wikipedia.org/wiki/Braille_ASCII the following string maps from the Unicode intervel 0x2800.. 0x283F
        // to the following Braille glyphs : "⠀⠁⠂⠃⠄⠅⠆⠇⠈⠉⠊⠋⠌⠍⠎⠏⠐⠑⠒⠓⠔⠕⠖⠗⠘⠙⠚⠛⠜⠝⠞⠟⠠⠡⠢⠣⠤⠥⠦⠧⠨⠩⠪⠫⠬⠭⠮⠯⠰⠱⠲⠳⠴⠵⠶⠷⠸⠹⠺⠻⠼⠽⠾⠿"   
        // 0x2800
        // + 0x10 *                 0                1               2               3
        // + 0x01 *                 0123456789ABCDEF 0123456789ABCDEF0123456789ABCDEF012 3456789ABCDEF  
        private const string map = " A1B'K2L@CIF/MSP\"E3H9O6R^DJG>NTQ,*5<-U8V.%[$+X!&;:4\\0Z7(_?W]#Y)="; // Note the 2 '\' used as escape characters !
        private byte[] byteMap; //  Maps from a UNICODE 0x2800..0x283F char to a byte.    Is filled in during initialization !
        private char[] charMap; //  Maps from a byte to a UNICODE char in 0x2800..0x283F  Is filled in during initialization !

        // According to https://en.wikipedia.org/wiki/Computer_Braille_Code: (Manually derived from the Web page:
        // 0x2800
        // + 0x10 *                                       0                1               2               3
        // + 0x01 *                                       0123456789ABCDEF 0123456789ABCDEF0123456789ABCDEF012 3456789ABCDEF 
        private const string Computer_Braille_Code_map = " a1b'k2l@cif/msp\"e3h9o6r^djg>ntq,*5<-u8v.%[$+x!&;:4\\0z7( ?w]#y)="; // No code for 0x38 !!

        public enum FileFormat{BRF_ASCII, BRF_Unicode, PEF};
        private FileFormat fileFormat;

        public string Extension
        {
            get
            {
                switch (this.fileFormat)
                {
                    case FileFormat.BRF_ASCII: return "brf";
                    case FileFormat.BRF_Unicode: return "brf";
                    case FileFormat.PEF: return "pef";
                    default:
                        Logger.LogCF(string.Format(": Unsupported fileFormat {0}", this.fileFormat));
                        return "";
                }
            }
        }

        //private byte ToByte(Char unicodeValue)
        //{
        //    int index = unicodeValue - 0x2800;
        //    byte b = byteMap[index];
        //    return b;
        //}

        //public List<byte> ToBytes(EventDescriptionList events, UserSettings userSettings)
        //{
        //    throw (new Exception("Not Implemented yet"));
        //    List<byte> bytes = new List<byte>();
        //    return bytes;
        //}

        //public List<Char> ToChars(EventDescriptionList events, UserSettings userSettings)
        //{
        //    throw (new Exception("Not Implemented yet"));
        //    List<Char> bytes = new List<Char>();
        //    return bytes;
        //}


        /// <summary>
        /// Converts a (Unicode-based) string of Braille characters (0x2800..0x283F) to its RBF-ASCII representation
        /// </summary>
        /// <param name="unicodeBraille"></param>
        /// <returns></returns>
        private byte[] ToRbfASCII(string unicodeBraille)
        {
            int length = unicodeBraille.Length;
            byte[] byteArray = new byte[length];
            for (int i = 0; (i < length); i++)
            {
                Char c = unicodeBraille[i];
                if ((c < 0x2800) || (c > 0x283F))
                {
                    string message = string.Format("Illegal value for Unicode Braille = 0x{0:x}", c);
                    byteArray[i] = 0x20; // Insert an empty Braille Character  
                    Logger.LogCF(string.Format(": {0}", message));
                }
                else
                {
                    byte mappedByte = byteMap[c - 0x2800]; // Map from the 0x2800..0x283F interval to the corresponding valie to write to the file
                    byteArray[i] = mappedByte;
                }
            }
            return byteArray;
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


        public bool WriteToFile(string unicodeBraille, string fullFileName)
        {
            byte[] byteArray = ToRbfASCII(unicodeBraille);
            return WriteToFile(byteArray, fullFileName);
        }


        /// <summary>
        /// Convert  a list of strings to brf(ASCII) taking into account the dimensions of the sheet to print on  
        /// </summary>
        /// <param name="unicodeBrailleList"></param>
        /// <param name="fullFileName"></param>
        /// <returns></returns>
        public bool WriteToFile(List<string> unicodeBrailleList, string fullFileName)
        {
            int lineWidth = 32; // 32 characters per line
            int formHeight = 20; // 20 lines per form

            List<byte> scoreByteList = new List<byte>();  // Represents the whole score
            int currentWidth = 0;
            int currentHeight = 0;
            int numberOfLines = 0; // For statistics only
            int numberOfForms = 0; // For statistics only

            foreach (string unicodeBraille in unicodeBrailleList)
            {
                byte[] eventByteArray = ToRbfASCII(unicodeBraille); // Represents one single event
                if (currentWidth + eventByteArray.Length > lineWidth)
                {
                    // Insert a CR+LF

                    scoreByteList.Add(CarriageReturn);
                    scoreByteList.Add(LineFeed);
                    numberOfLines++;
                    currentWidth = 0;
                    currentHeight += 1;
                    if (currentHeight >= formHeight)
                    {
                        // Insert a FF
                        scoreByteList.Add(FormFeed);
                        currentHeight = 0;
                        numberOfForms++;
                    }
                    currentHeight++;
                }
                currentWidth += eventByteArray.Length;
                scoreByteList.AddRange(eventByteArray);
            }

            Logger.LogCF(string.Format(": Generated {0} forms containing {1} lines", numberOfForms, numberOfLines));
            return WriteToFile(scoreByteList, fullFileName);
        }




        /// <summary>
        /// Writes the byteList to the file specified without any conversion !
        /// </summary>
        /// <param name="byteList"></param>
        /// <param name="fullFileName"></param>
        /// <returns></returns>
        public bool WriteToFile(List<byte> byteList, string fullFileName)
        {
            byte[] byteArray = byteList.ToArray();
            return WriteToFile(byteArray, fullFileName);
        }


        /// <summary>
        /// Writes the byteArray to the file without any conversion
        /// </summary>
        /// <param name="byteArray"></param>
        /// <param name="fullFileName"></param>
        /// <returns></returns>
        public bool WriteToFile(byte[] byteArray, string fullFileName)
        {
            using (BinaryWriter bw = new BinaryWriter(File.Open(fullFileName, FileMode.Create)))
            {
                bw.Write(byteArray);
            }
#warning ToDO Error handling
            bool result = true;
            return result;
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
        ///  Write an array of 16-bit characters to a file
        /// </summary>
        /// <param name="chars"></param>
        /// <returns></returns>
        public bool WriteToFile(List<Char> chars, string fullFileName)
        {
            throw (new Exception("Not Implemented yet"));
            bool result = true;
            return result;
        }
        // Tests



        public bool TestByteList(string fileName)
        {
            List<byte> byteList = new List<byte>();
            for (byte i = 0; (i < 64); i++)
            {
                byteList.Add(i);
            }
            return this.WriteToFile(byteList,fileName + "ByteList.bin");
        }

        public bool TestByteArray(string fileName)
        {
            List<byte> testBytes = new List<byte>();
            for (byte i = 0; (i < 64); i++)
            {
                testBytes.Add(i);
            }
            List<byte> byteArray = new List<byte>(testBytes);
            return this.WriteToFile(byteArray,fileName + "ByteArray.bin");
        }

        public bool TestUnicodeBraille(string fileName)
        {
            int length = 64;
            StringBuilder sb = new StringBuilder(length);
            for (int i = 0; (i < length); i++)
            {
                char c = (char) (i + 0x2800);
                sb.Append(c);
            }
            string s = sb.ToString();
            return this.WriteToFile(s,fileName + "Unicode.bin");
        }


        // Test


        /// <summary>
        /// Reads a file containing MusicBraille information and returns its contents as a UNICODE string
        /// If the file is not already in UNICODE format (0x2800..0x283F), the contents is converted to UNICODE representation on the fly
        /// </summary>
        /// <param name="fullFilefileName"></param>
        public string ReadFromFile(string fullFileName)
        {
            string result = "";
            switch (fileFormat)
            {
                case FileFormat.BRF_ASCII:
                    {
                        byte[] bytes;
                        using (BinaryReader br = new BinaryReader(File.Open(fullFileName, FileMode.Open)))
                        {
                            //bytes = br.ReadBytes(int.MaxValue);
#warning ToDo fix constant
                            bytes = br.ReadBytes(10000);
                        }
                        result = ToUnicode(bytes);
                        break;
                    }

                case FileFormat.BRF_Unicode:
                    {                       
                        using (BinaryReader br = new BinaryReader(File.Open(fullFileName, FileMode.Open)))
                        {
                            result = System.IO.File.ReadAllText(fullFileName); 
                        }
                        break;
                    }
                default:
                    Logger.LogCF(string.Format(" Unsupported file format '{0}'", fileFormat.ToString()));
                    break;
                    
            }
            return result;          
        }


        // Construction


        private BrailleFileHandler()
        {
        }

        private BrailleFileHandler(FileFormat fileFormat)
        {
            this.fileFormat = fileFormat;
            // Init the byteMap for fast and easy easy conversion later.
            byteMap = new byte[map.Length];
            for (int i = 0; (i < map.Length); i++)
            {
                char c = map[i];
                byteMap[i] = (byte)(c % 256);
            }
            // Init the charMap for fast and easy conversion later
            charMap = new char[256];
            for (int i = 0; (i < map.Length); i++)
            {
                int index = map[i];
                charMap[index] = (char)(0x2800 + i);
            }
            // Also map 3 controls to their Unicode equivalents
            charMap[LineFeed] = (char)LineFeed;
            charMap[FormFeed] = (char)FormFeed;
            charMap[CarriageReturn] = (char)CarriageReturn;

            // Check both
            const int BrailleBase = 0x2800;
            for (int i = BrailleBase; i < BrailleBase + map.Length; i++)
            {
                byte b = byteMap[i - BrailleBase];
                int result = charMap[b];
                if (i != result)
                {
                    Logger.LogCF(string.Format(": Initialization error: char=0x{0}:x maps to byte={1} which maps to 0x{2:x}", i, b, result));
                }
            }
        }

        public static BrailleFileHandler Create(FileFormat fileFormat)
        {
            return new BrailleFileHandler(fileFormat);
        }

        public static BrailleFileHandler Create()
        {
            return new BrailleFileHandler(FileFormat.BRF_ASCII);
        }

    }
}
