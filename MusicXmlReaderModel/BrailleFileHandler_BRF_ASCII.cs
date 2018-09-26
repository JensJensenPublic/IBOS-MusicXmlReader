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
        private byte[] byteMap; //  Maps from a UNICODE 0x2800..0x283F char to a byte.    Is filled in during initialization !
        private char[] charMap; //  Maps from a byte to a UNICODE char in 0x2800..0x283F  Is filled in during initialization !

#if false
        // According to https://en.wikipedia.org/wiki/Computer_Braille_Code: (Manually derived from the Web page:
        // 0x2800
        // + 0x10 *                                       0                1               2               3
        // + 0x01 *                                       0123456789ABCDEF 0123456789ABCDEF0123456789ABCDEF012 3456789ABCDEF 
        private const string Computer_Braille_Code_map = " a1b'k2l@cif/msp\"e3h9o6r^djg>ntq,*5<-u8v.%[$+x!&;:4\\0z7( ?w]#y)="; // No code for 0x38 !!
#endif

        public override string GetExtension()
        {
            return ".rbf";
        }

        public override string GetFileFormat()
        {
            return "BRF_ASCII";
        }

        /// <summary>
        /// Constructor
        /// </summary>
        internal BrailleFileHandler_BRF_ASCII()
        {
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


        /// <summary>
        /// Converts a (Unicode-based) string of Braille characters (0x2800..0x283F) to its RBF-ASCII representation
        /// </summary>
        /// <param name="unicodeBraille"></param>
        /// <returns></returns>
        private byte[] ToRbfASCII(string unicodeBraille, bool acceptControls)
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



        private byte[] ToRbfASCII(string unicodeBraille)
        {
            return ToRbfASCII(unicodeBraille, false);
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

        public override bool WriteToFile(string unicodeBraille, string fullFileName, bool acceptControls)
        {
            byte[] byteArray = ToRbfASCII(unicodeBraille, acceptControls);
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
        /// Reads a file containing MusicBraille information and returns its contents as a UNICODE string
        /// If the file is not already in UNICODE format (0x2800..0x283F), the contents is converted to UNICODE representation on the fly
        /// </summary>
        /// <param name="fullFilefileName"></param>
        public override string ReadFromFile(string fullFileName)
        {
            byte[] bytes;
            using (BinaryReader br = new BinaryReader(File.Open(fullFileName, FileMode.Open)))
            {
                //bytes = br.ReadBytes(int.MaxValue);
#warning ToDo fix constant
                bytes = br.ReadBytes(10000);
            }
            string result = ToUnicode(bytes);
            return result;
        }
    }





}
