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



        /// <summary>
        /// Writes a string of Braille (in Unicode representation) to a file after converting it to ASCII
        /// </summary>
        /// <param name="unicodeBraille"></param>
        /// <param name="fullFileName"></param>
        /// <param name="acceptControls"></param>
        /// <returns></returns>
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
        /// <returns>true <==> success</returns>
        private bool WriteToFile(byte[] byteArray, string fullFileName)
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


        /// <summary>
        /// Reads a file containing MusicBraille information and returns its contents as a UNICODE string
        /// </summary>
        /// <param name="fullFilefileName"></param>
        public override string ReadFromFile(string fullFileName)
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
    }





}
