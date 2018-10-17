using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MusicXmlReaderModel
{
    class BrailleFileHandler_BRF_ASCII : BrailleFileHandler
    {
        // According to https://en.wikipedia.org/wiki/Braille_ASCII the following string maps from the Unicode intervel 0x2800.. 0x283F
        // to the following Braille glyphs : "⠀⠁⠂⠃⠄⠅⠆⠇⠈⠉⠊⠋⠌⠍⠎⠏⠐⠑⠒⠓⠔⠕⠖⠗⠘⠙⠚⠛⠜⠝⠞⠟⠠⠡⠢⠣⠤⠥⠦⠧⠨⠩⠪⠫⠬⠭⠮⠯⠰⠱⠲⠳⠴⠵⠶⠷⠸⠹⠺⠻⠼⠽⠾⠿"   
        // 0x2800
        // + 0x10 *                 0                1               2               3
        // + 0x01 *                 0123456789ABCDEF 0123456789ABCDEF0123456789ABCDEF012 3456789ABCDEF  
        private const string map = " A1B'K2L@CIF/MSP\"E3H9O6R^DJG>NTQ,*5<-U8V.%[$+X!&;:4\\0Z7(_?W]#Y)="; // Note the 2 '\' used as escape characters !


#if false
        // According to https://en.wikipedia.org/wiki/Computer_Braille_Code: (Manually derived from the Web page:
        // 0x2800
        // + 0x10 *                                       0                1               2               3
        // + 0x01 *                                       0123456789ABCDEF 0123456789ABCDEF0123456789ABCDEF012 3456789ABCDEF 
        private const string Computer_Braille_Code_map = " a1b'k2l@cif/msp\"e3h9o6r^djg>ntq,*5<-u8v.%[$+x!&;:4\\0z7( ?w]#y)="; // No code for 0x38 !!
#endif

        public override string GetExtension()
        {
            return ".brf";
        }

        public override string GetFileFormat()
        {
            return "BRF_ASCII";
        }

        /// <summary>
        /// Constructor
        /// </summary>
        internal BrailleFileHandler_BRF_ASCII(int charsPerLine, int linesPerForm)
        {
            this.charsPerLine = charsPerLine;
            this.linesPerForm = linesPerForm;

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

            AddControls();
            CheckTables(map.Length);
        }



        private byte[] ToBytes(string unicodeBraille)
        {
            return ToBytes(unicodeBraille, false);
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


        /// <summary>
        /// Reads a file containing MusicBraille information and returns its contents as a UNICODE string
        /// </summary>
        /// <param name="fullFilefileName"></param>
        public override string ReadFromFile(string fullFileName)
        {
            return ReadBytesFromFile(fullFileName);
        }
    }





}
