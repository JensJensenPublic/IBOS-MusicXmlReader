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
        protected byte[] controlCharacters = new byte[] { CarriageReturn, LineFeed, FormFeed };

        protected int charsPerLine = 14;
        public int CharsPerLine { get { return charsPerLine; } }
        protected int linesPerForm = 32;
        public int LinesPerForm { get { return linesPerForm; } }

        protected byte[] byteMap; //  Maps from a UNICODE 0x2800..0x283F char to a byte.    Is filled in during initialization !
        protected char[] charMap; //  Maps from a byte to a UNICODE char in 0x2800..0x283F  Is filled in during initialization !

        public enum ByteOrderMarkEnum { Unknown, None, UFT8, UTF16BE, UTF16LE,UTF32BE, UTF32LE }

        public static ByteOrderMarkEnum GetByteOrderMark(string fileName)
        { 
            ByteOrderMarkEnum result = GetByteOrderMarkPrivate(fileName);
            Logger.LogCF(string.Format("{0} returns {1}", fileName, result));
            return result;
        }

        /// <summary>
        /// See  https://en.m.wikipedia.org/wiki/Byte_order_mark
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static ByteOrderMarkEnum GetByteOrderMarkPrivate(string fileName)
        {
            byte[] bytes = File.ReadAllBytes(fileName); // Let the system throw an exception if file is not found!
            if ((bytes.Length >= 3) & (bytes[0] == 0xEF) && (bytes[1] == 0xBB) & (bytes[2] == 0xBF)) return ByteOrderMarkEnum.UFT8;
            if ((bytes.Length >= 2) & (bytes[0] == 0xFE) && (bytes[1] == 0xFF)) return ByteOrderMarkEnum.UTF16BE;
            if ((bytes.Length >= 2) & (bytes[0] == 0xFF) && (bytes[1] == 0xFE))  return ByteOrderMarkEnum.UTF16LE;
            if ((bytes.Length >= 4) & (bytes[0] == 0x00) && (bytes[1] == 0x00) & (bytes[2] == 0xFE) && (bytes[3] == 0xFF)) return ByteOrderMarkEnum.UTF32BE;
            if ((bytes.Length >= 4) & (bytes[0] == 0xFF) && (bytes[1] == 0xFE) & (bytes[2] == 0x00) && (bytes[3] == 0x00)) return ByteOrderMarkEnum.UTF32LE;
            // Hopefully no more are needed
            return ByteOrderMarkEnum.None;
        }

        private bool IsUtf8(byte[] bytes)
        {
            // Check if a BOM (Byte Order Mark) is found
            if (bytes.Length < 3) return false;
            if (bytes[0] != 0xEF) return false;
            if (bytes[1] != 0xBB) return false;
            if (bytes[2] != 0xBF) return false;
            return true;
        }




        // Some general information about Braille file formats:
        //
        // https://www.ukaaf.org/wp-content/uploads/2017/03/ReadingElectronicBrailleFinal.pdf
        // https://en.wikipedia.org/wiki/Braille_ASCII 
        // https://en.wikipedia.org/wiki/Computer_Braille_Code 
        // https://en.wikipedia.org/wiki/Unified_English_Braille 
        // https://en.wikipedia.org/wiki/Braille_Patterns


        public enum FileEncoding
        {
            Unknown,
            BRF_ASCII,      
            PEF,
            BRL_OctoBraille_1252,
            BRF_Unicode,        // As with the UTF8 parameter, but starts without  EF BB BF. Works with IbPrint(65001). Uses StreamWriter(FileStream, FileMode.Create)
            BRF_Unicode_utf8,   // As without the UTF8 parameter, but starts with  EF BB BF.  Works with IbPrint(65001). Uses StreamWriter(FileStream, FileMode.Create,Encoding.UTF8) 
            BRF_Unicode_utf16,  // Seems to work. Best mapping for the 0x2800..0x28ff values Works with IbPrint(1200). Uses StreamWriter(FileStream, FileMode.Create,Encoding.UTF16)
            BRF_Unicode_utf32,   // Seems to generate OK bin pattern. IbPrint(65005) fails. Uses StreamWriter(FileStream, FileMode.Create,Encoding.UTF32)
            BRF_ASCII_Ex
        };

// These encodings wil generate the following contents for the testpattern containing 256 Braille patterns in binary order:
// Octo1252 20 61 2C 62 2E 6B 3B 6C   27 63 69 66 BF 6D 73 70 ( 287 bytes)
// ASCII:   20 41 31 42 27 4B 32 4C   40 43 49 46 2F 4D 53 50 ( 287 bytes)
// Unicode: E2 A0 80 E2 A0 81 E2 A0   82 E2 A0 83 E2 A0 84 E2 ( 799 bytes)
// UTF-8:   EF BB BF E2 A0 80 E2 A0   81 E2 A0 82 E2 A0 83 E2 ( 802 bytes)
// UTF-16:  FF FE 00 28 01 28 02 28   03 28 04 28 05 28 06 28 ( 576 bytes) 
// UTF-32:  FF FE 00 00 00 28 00 00   01 28 00 00 02 28 00 00 (1152 bytes)

       

        // The following methods need separate implementations 
        abstract public string GetExtension();
        abstract public string GetFileFormat();
        abstract public int GetCodePage();
        abstract public bool IsValidBrailleMusic(string fileName);

        /// <summary>
        ///  /// Reads a file containing MusicBraille information and returns its contents as a UNICODE string
        /// </summary>
        /// <param name="fullFileName"></param>
        /// <returns></returns>
        abstract public string ReadFromFile(string fullFileName);

        abstract public bool   WriteToFile(string unicodeBraille, string fullFileName, bool acceptControls);

        /// <summary>
        /// Common convenience method for encodings using simple binary format such as ASCII or OctoBraille_1252
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        protected bool IsValidBrailleMusic(byte[] bytes)
        {
            if (null == bytes) return false;
            int position = 0;
            int errors = 0;
            foreach (byte b in bytes)
            {
                if (!((byteMap.Contains<byte>(b)) || (controlCharacters.Contains<byte>(b))))
                {
                    if (0 == errors++)
                    {
                        Logger.LogCF(string.Format(": Illegal value 0x{0:x} at position {1}", b, position));
                    }
                    errors++;
                }
                position++;
            }
            Logger.LogCF(string.Format(": Found {0} illegal bytes", errors));
            return true;
        }


        /// <summary>
        /// Simple convenience method
        /// </summary>
        /// <param name="fullFileName"></param>
        /// <returns></returns>
        protected byte[] ReadAsBinary(string fullFileName)
        {
            byte[] bytes = null;
            try
            {
               bytes = System.IO.File.ReadAllBytes(fullFileName);
            }
            catch (Exception e)
            {
                Logger.LogCFE(e);
                return null;
            }
            Logger.LogCF(string.Format(": Read {0} bytes from '{1}'", bytes.Length, fullFileName));
            return bytes;
        }

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

        protected virtual byte LetterToCapital(byte b)
        {
            return b;
        }


        /// <summary>
        /// Common mechanism for converting the contents of a Braille file, coded in either ASCII or OctoBraille_1252 encoding, to Unicode.
        /// Implemented by a simple lookup table "charMap", which is filled in during initialization.
        /// </summary>
        /// <param name="bytes">The contents of the inputfile, as read by File.ReadAllBytes()</param>
        /// <returns>A string containing the Unicode representing of the inputfile. All chars are CarriageReturn, LineFeed, FormFeed or in the Unicode Braille interval [0x2800..0x28ff]</returns>
        /// <exception cref="InvalidDataException">Thrown when the input file contains an value, which is invalid in the current encoding</exception>
        protected string ToUnicode(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder(); // The valid bytes
            StringBuilder ib = new StringBuilder(); // The invalid bytes for logging
            List<byte> lb = new List<byte>(); // Individiual invalid bytes for logging. Each byte only once !     
            for (int i = 0; (i < bytes.Length); i++)
            {
                byte b = bytes[i];
                char c = charMap[LetterToCapital(b)];
                if (0 != c)
                {
                    sb.Append(c);
                }
                else
                {
                    ib.Append(string.Format(" {0}", b.ToString()));
                    //Logger.LogCF(string.Format(": The value {0} is not valid!", b.ToString())); // Replaced by a single logline !
                    //sb.Append("?");
                    sb.Append((char) b); // Without mapping !!
                    if (!lb.Contains(b))
                    {
                        lb.Add(b);
                    }
                }
            }
            if (0 != ib.Length)
            {
                Logger.LogCF(string.Format(": Invalid values: {0}", ib.ToString()));
            }
            if (0 != lb.Count)
            {
                StringBuilder sbc = new StringBuilder();
                StringBuilder sbd = new StringBuilder();
                foreach (byte b in lb)
                {
                    sbc.Append((char)(b) + " ");
                    sbd.Append(((int)b).ToString() + " ");
                }
                Logger.LogCF(string.Format(": Invalid values (as char   ): {0}", sbc.ToString()));
                Logger.LogCF(string.Format(": Invalid values (as decimal): {0}", sbd.ToString()));
            }

            int errorCount = lb.Count;
            // errorCount = 1; // For debugging only
            if (0 != errorCount)
            {
                string exceptionMessage = string.Format("Found {0} invalid data values",errorCount);
                Logger.LogCF(string.Format(": {0}", exceptionMessage));
                throw new InvalidDataException(exceptionMessage);
            }
            

            return sb.ToString();
        }



        /// <summary>
        /// Reads a file containing MusicBraille information and returns its contents as a UNICODE string
        /// </summary>
        /// <param name="fullFilefileName"></param>
        protected virtual string ReadBytesFromFile(string fullFileName)
        {
            byte[] bytes;
            string result = null;
            try
            {
                bytes = File.ReadAllBytes(fullFileName);
                Logger.LogCF(string.Format(": File.ReadAllBytes() read {0} bytes from {1}", bytes.Length, fullFileName));
                result = ToUnicode(bytes);
            }
            catch (Exception e)
            {
                result = null;
                Logger.LogCFE(e);
                throw; // Rethrow the exception in order to let UI handle it
            }
            return result;
        }

        
        public bool WriteToFile(List<string> unicodeBrailleList, string fullFileName)
        {
            string score = BrailleUtilities.Format(unicodeBrailleList,this.charsPerLine,this.linesPerForm);
            return WriteToFile(score, fullFileName, true); // Call the abstract implementation
        }

#if false
        /// <summary>
        /// Generate a testpattern containing all 64 possible 6-point Braille glyphs and write it to a file
        /// This is the old version from before March 2024, writing the test pattern directly to the file after formatting
        /// by calling WriteToFile(List<string>...)
        /// </summary>
        /// <param name="directoryName"></param>
        public string GenerateTestpattern(string directoryName)
        {
            string fullFileName = Path.Combine(directoryName, "MusicBrailleTestPattern" + "."  + GetFileFormat() + GetExtension());
            StringBuilder sb = new StringBuilder();
            {
                for (int i0 = 0; (i0 < 4); i0++) // Repeat the test pattern 4 times, thus generating 4 * 64 = 256 characters. This could for instance end up in a 16 * 16 matrix
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
            return fullFileName;
        }
#else
        /// <summary>
        /// Generate a testpattern containing all 64 possible 6-point Braille glyphs and write it to a file
        /// This is a new, simple version from March 2024, writing the test pattern directly to the file
        /// by calling WriteToFile(string...)
        /// </summary>
        /// <param name="directoryName"></param>
        public string GenerateTestpattern(string directoryName)
        {
            string fullFileName = Path.Combine(directoryName, "BrailleTestPattern" + "." + GetFileFormat() + GetExtension());
            StringBuilder sb = new StringBuilder();
            {
                int width = 0;
                for (int i1 = 0x2800; i1 < 0x2840; i1++) // Testpattern: All Unicode values from 0x2800 to 0x283f
                {
                    char c = (char)(i1);
                    sb.Append(c);
                    width++;
                    if (width == this.charsPerLine) // Use the max possible linewidth, avoiding line wrapping and continuation marks
                    {
                        sb.Append("\r\n");
                        width = 0;
                    }
                }
            }
            string testPattern = sb.ToString();
            this.WriteToFile(testPattern, fullFileName, true); // true meas "AcceptControls"
            Logger.LogCF(string.Format(": Wrote testpattern to {0}", fullFileName));
            return fullFileName;
        }
#endif





        /// <summary>
        /// Format a Unicode string of BrailleMusic information into forms and lines, according to its contents
        /// </summary>
        /// <param name="brailleMusicString"></param>
        /// <returns></returns>
        public List<string> Format(string brailleMusicString)
        {
            List<string> result;
            int maxLineLength = 0;
            Format(brailleMusicString, ref maxLineLength);  // First call computes the maxLineLength and ignores the result.
            result = Format(brailleMusicString, ref maxLineLength); // Second call computes the result, using maxLineLength
            return result;
        }
        

        /// <summary>
        /// Use essentially the same algorithm for (initially) finding the max line length and for creating the final result.
        /// </summary>
        /// <param name="brailleMusicString"></param>
        /// <param name="maxLineLength"></param>
        /// <returns></returns>
        private List<string> Format(string brailleMusicString, ref int maxLineLength) // Call with maxLineLength = 0 wil compute the max line length
        {
            //   The following code is inspired by Model.DetailsDescription[] GetBrailleFileDetails()
            bool mute = (0 == maxLineLength);
            List<string> result = new List<string>();
            string[] forms = brailleMusicString.Split((char)FormFeed); // Split into a number of forms
            if (!mute) Logger.LogCF(string.Format("NumberOfChars={0} NumberOfForms={1}", brailleMusicString.Length, forms.Length));
            int formNumber = 0;
            //char[] lineSplitChars = new[] { '\r', '\n' };
            char[] lineSplitChars = new[] { (char)CarriageReturn, (char) LineFeed }; 
            int localMaxLength = 0;
            foreach (string form in forms)
            {
                formNumber++;
                int lineNumber = 0;
                string[] lines = form.Split(lineSplitChars, StringSplitOptions.RemoveEmptyEntries); // Split each form into a number of lines. Handles any combination of cr and lf! 
                foreach (string line in lines)
                {
                    lineNumber++;
                    localMaxLength = Math.Max(localMaxLength, line.Length);
                    if (!mute)
                    {
                        string paddedLine = line.PadRight(maxLineLength); // Pad right with space characters if needed.
                        string lineAsText = Utilities.BrailleToDotNumbers(line);
//                        string logLine = string.Format(": F={0,-03} L={1,-03} {2}  ({3})", formNumber, lineNumber, paddedLine, lineAsText); // ",-03": Left align using 3 positions
                        string logLine = string.Format("{0}  {1}", paddedLine, lineAsText); 
                        Logger.Log(logLine);
                        result.Add(logLine);
                    }
                }                
            }
            maxLineLength = localMaxLength;
            return result;
        }




        // Construction

        /// <summary>
        /// For construction from all kinds of testprograms, not needing metainformation. Maps to the other Create() with metaInformation = null
        /// </summary>
        /// <param name="fileEncoding"></param>
        /// <param name="charsPerLine"></param>
        /// <param name="linesPerForm"></param>
        /// <returns></returns>
        public static BrailleFileHandler Create(FileEncoding fileEncoding, int charsPerLine, int linesPerForm)
        {
            return BrailleFileHandler.Create(fileEncoding, charsPerLine, linesPerForm, null);
        }

        /// <summary>
        /// For creation from the MusicXmlReader application
        /// </summary>
        /// <param name="fileEncoding"></param>
        /// <param name="charsPerLine"></param>
        /// <param name="linesPerForm"></param>
        /// <param name="metaInformation"></param>
        /// <returns></returns>
        public static BrailleFileHandler Create(FileEncoding fileEncoding, int charsPerLine, int linesPerForm, MetaInformation metaInformation)
        {
            switch (fileEncoding)
            {
                // Uses spscific classes: 
                case FileEncoding.BRF_ASCII: return new BrailleFileHandler_BRF_ASCII(charsPerLine, linesPerForm);
                case FileEncoding.BRF_ASCII_Ex: return new BrailleFileHandler_BRF_ASCII_Ex(charsPerLine, linesPerForm);
                case FileEncoding.BRF_Unicode: return new BrailleFileHandler_BRF_Unicode(charsPerLine, linesPerForm);
                case FileEncoding.BRL_OctoBraille_1252: return new BrailleFileHandler_BRL_OctoBraille_1252(charsPerLine, linesPerForm);
                case FileEncoding.PEF: return new BrailleFileHandler_PEF(charsPerLine, linesPerForm,metaInformation);
                // Uses the generic class and an Encoding parameter
                case FileEncoding.BRF_Unicode_utf8: return new BrailleFileHandler_Generic(charsPerLine, linesPerForm, ".txt", Encoding.UTF8);
                case FileEncoding.BRF_Unicode_utf16: return new BrailleFileHandler_Generic(charsPerLine, linesPerForm, ".txt", Encoding.Unicode);
                case FileEncoding.BRF_Unicode_utf32: return new BrailleFileHandler_Generic(charsPerLine, linesPerForm, ".txt", Encoding.UTF32);


                default:
                    Logger.LogCF(string.Format(": Unsupported file format {0}", fileEncoding.ToString()));
                    return null;
            }

        }  




                //public static BrailleFileHandler Create()
        //{
        //    return BrailleFileHandler.Create(FileEncoding.BRF_ASCII);
        //}

    }

}
