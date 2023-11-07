using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXmlReaderModel
{
    class BrailleFileHandler_BRL_OctoBraille_1252 : BrailleFileHandler
    {
        // According to "Braille Sense U2 brugermanual(Ver 8 5_dk)" the following array of integers maps from the Unicode intervel 0x2800.. 0x283F
        // to the following Braille glyphs : "⠀⠁⠂⠃⠄⠅⠆⠇⠈⠉⠊⠋⠌⠍⠎⠏⠐⠑⠒⠓⠔⠕⠖⠗⠘⠙⠚⠛⠜⠝⠞⠟⠠⠡⠢⠣⠤⠥⠦⠧⠨⠩⠪⠫⠬⠭⠮⠯⠰⠱⠲⠳⠴⠵⠶⠷⠸⠹⠺⠻⠼⠽⠾⠿"   
        // 0x2800
        // + 0x10 *                 0                1               2               3
        // + 0x01 * 

        // The danish 8-dot table almost matches Windows codepage CP1252, except that the table also uses some values which do not map to any character in CP1252,
        // that is 0x81, 0x8d, 0x8f, 0x90 og 0x9d. (Decimal 129, 141, 143, 144, 157.) 
        // As one of these values (Decimal 129) is used for mapping a 6 dot value we represent the map as integers instead of characters:
        private readonly byte[] map = new byte[] 
        // 0   1   2   3   4   5   6   7   8   9   A   B   C   D   E   F         
        {032,097,044,098,046,107,059,108,039,099,105,102,191,109,115,112,  // Maps the Unicode Interval starting at 0x2800
         096,101,058,104,042,111,033,114,129,100,106,103,230,110,116,113,  // Maps the Unicode Interval starting at 0x2810
         133,229,063,234,150,117,181,118,152,238,248,235,158,120,232,231,  // Maps the Unicode Interval starting at 0x2820
         168,251,161,252,176,122,034,224,139,244,119,239,190,121,249,233}; // Maps the Unicode Interval starting at 0x2830
                                                                           // Please find more information in the 2 files (both found in "...\Dropbox\Root\Visual Studio 2015\Projects\MusicXmlReaderUI\Documentation"):
                                                                           // "da-ansi8.dis" and "Braille Sense U2 brugermanual(Ver 8 5_dk).docx"

        public override int GetCodePage()
        {
            return 1252;
        }


        public override bool IsValidBrailleMusic(string fileName)
        {
            byte[] bytesReadFromFile = ReadAsBinary(fileName);
            return IsValidBrailleMusic(bytesReadFromFile);
        }
        
        public override string GetExtension()
        {
            return ".brl"; 
        }

        public override string GetFileFormat()
        {
            return "BRL_OctoBraille_1252";
        }

        /// <summary>
        /// For handling either plain OctoBraille_1252, or OctoBraille_1252, wrapped into an extra layer of utf8: 
        /// Overrides the default method (implemented in BrailleFileHandler.cs), which always internally uses File.ReadAllBytes().
        /// Instead the current implementation may choose first to use System.IO.File.ReadAllText(fullFileName, Encoding.UTF8) to remove the extra layer of utf8 encoding 
        /// This is needed in cases where the input file contains a "BOM" ("Byte Order Mark"): The hexadecimal sequence 0xEF,0xBB, 0XBF)
        /// Please see for instance: https://en.m.wikipedia.org/wiki/Byte_order_mark
        /// Bu using this approach we leave the problems of recognizing the BOM and decoding the outer layer of utf8 to System.IO.
        /// </summary>
        /// <param name="fullFileName"></param>
        /// <returns></returns>
        protected override string ReadBytesFromFile(string fullFileName)
        {
            // Start new         
            ByteOrderMarkEnum byteOrderMark = BrailleFileHandler.GetByteOrderMark(fullFileName);
            // byteOrderMark = ByteOrderMarkEnum.Unknown; // ONLY for testing the exception mechanism
            switch (byteOrderMark)
            {
                case ByteOrderMarkEnum.None: return base.ReadBytesFromFile(fullFileName); // The normal case: Plain OctoBraille_1252 encoding
                case ByteOrderMarkEnum.UFT8: // Special case: OctoBraille_1252, wrapped into an extra layer of utf8
                    // Handles a file, originally encoded as OctoBraille_1252, but later wrapped into a a layer of utf8, for instance by a simple text editor.
                    string octoBraille_1252 = System.IO.File.ReadAllText(fullFileName, Encoding.UTF8); // Read the file, encoded as utf8. Deliver a Unicode string in OctoBraille_1252 representation!                                                                                // 
                    Logger.LogCF(string.Format(": File.ReadAllText()  read {0} chars from {1}", octoBraille_1252.Length, fullFileName));
                    Logger.LogCF(string.Format("Characters read=\r\n{0}", octoBraille_1252));
                    byte[] bytesOctoBraille_1252 = ToBytes(octoBraille_1252);
                    string resultUnicodeBraille =  ToUnicode(bytesOctoBraille_1252);
                    return resultUnicodeBraille;
                default: // Unimplemented special cases: OctoBraille wrapped into something else
                    string message = string.Format("ByteOrderMark={0} is not implemented for encapsulating OctoBraille_1252", byteOrderMark);
                    Logger.LogCF(string.Format(": {0}", message));
                    throw new NotImplementedException(message); // The exception is expected to be handled by some higher layer, typically by the UI
            }
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

        private byte[] ToBytes(string s)
        {
            char minChar = char.MaxValue; // For error reporting
            char maxChar = char.MinValue; // For error reporting
            byte[] result = new byte[s.Length];
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                if (c < minChar) minChar = c;
                if (c > maxChar) maxChar = c;
                result[i] = (byte)c;
            }
            Logger.LogCF(string.Format(": The string contains characters in the interval from {0} to {1}", (ushort)minChar, (ushort)maxChar));
            return result;
        }


        /// <summary>
        /// Constructor
        /// </summary>
        internal BrailleFileHandler_BRL_OctoBraille_1252(int charsPerLine, int linesPerForm)
        {
            this.charsPerLine = charsPerLine;
            this.linesPerForm = linesPerForm;

            // Init the byteMap for fast and easy easy conversion later.
            byteMap = new byte[map.Length];
            for (int i = 0; (i < map.Length); i++)
            {
                char c = (char) map[i];
                byteMap[i] = (byte)(c % 256);
            }
            // Init the charMap for fast and easy conversion later
            charMap = new char[256];
            for (int i = 0; (i < map.Length); i++)
            {
                int index = map[i];
                charMap[index] = (char)(0x2800 + i);
            }

            AddControls();
            CheckTables(map.Length);  
        }
        

        /// <summary>
        /// Reads a file containing MusicBraille information and returns its contents as a UNICODE string
        /// </summary>
        /// <param name="fullFilefileName"></param>
        public override string ReadFromFile(string fullFileName)
        {
            return ReadBytesFromFile(fullFileName);
        }
        
        /// <summary>
        /// Writes a string of Braille (in Unicode representation) to a file after converting it to ASCII
        /// </summary>
        /// <param name="unicodeBraille"></param>
        /// <param name="fullFileName"></param>
        /// <param name="acceptControls"></param>
        /// <returns></returns>
        public override bool WriteToFile(string unicodeBraille, string fullFileName, bool acceptControls)
        {
            byte[] byteArray = ToBytes(unicodeBraille, acceptControls);
            return WriteToFile(byteArray, fullFileName);
        }
        

    }
}
