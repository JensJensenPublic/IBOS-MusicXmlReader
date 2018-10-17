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
         096,101,058,104,042,111,033,114,129,100,106,103,230,110,255,113,  // Maps the Unicode Interval starting at 0x2810
         133,229,063,234,150,117,181,118,152,238,248,235,158,120,232,231,  // Maps the Unicode Interval starting at 0x2820
         168,251,161,252,176,122,034,224,139,244,119,239,190,121,249,233}; // Maps the Unicode Interval starting at 0x2830

        public override string GetExtension()
        {
            return ".brl";
        }

        public override string GetFileFormat()
        {
            return "BRL_OctoBraille_1252";
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
