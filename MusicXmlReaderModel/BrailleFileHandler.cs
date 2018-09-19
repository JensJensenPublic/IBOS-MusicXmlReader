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
        // According to https://en.wikipedia.org/wiki/Braille_ASCII the following string maps from the Unicode intervel 0x2800.. 0x283F
        // to the following Braille glyphs : "⠀⠁⠂⠃⠄⠅⠆⠇⠈⠉⠊⠋⠌⠍⠎⠏⠐⠑⠒⠓⠔⠕⠖⠗⠘⠙⠚⠛⠜⠝⠞⠟⠠⠡⠢⠣⠤⠥⠦⠧⠨⠩⠪⠫⠬⠭⠮⠯⠰⠱⠲⠳⠴⠵⠶⠷⠸⠹⠺⠻⠼⠽⠾⠿"         
        private const string map = " A1B'K2L@CIF/MSP\"E3H9O6R^DJG>NTQ,*5<-U8V.%[$+X!&;:4\\0Z7(_?W]#Y)=";
        private byte[] byteMap;//  = new byte[64]();

        public enum FileFormat{BRF_ASCII,BBRF_Unicode,PEF};
        private FileFormat fileFormat;

        public string Extension
        {
            get
            {
                switch (this.fileFormat)
                {
                    case FileFormat.BRF_ASCII: return "brf";
                    case FileFormat.BBRF_Unicode: return "brf";
                    case FileFormat.PEF: return "pef";
                    default:
                        Logger.LogCF(string.Format(": Unsupported fileFormat {0}", this.fileFormat));
                        return "";
                }
            }
        }

        private byte ToByte(Char unicodeValue)
        {
            int index = unicodeValue - 0x2800;
            byte b = byteMap[index];
            return b;
        }

        public List<byte> ToBytes(EventDescriptionList events, UserSettings userSettings)
        {
            throw (new Exception("Not Implemented yet"));
            List<byte> bytes = new List<byte>();
            return bytes;
        }

        public List<Char> ToChars(EventDescriptionList events, UserSettings userSettings)
        {
            throw (new Exception("Not Implemented yet"));
            List<Char> bytes = new List<Char>();
            return bytes;
        }


        public bool WriteToFile(string unicodeBraille, string fullFileName)
        {
            int length = unicodeBraille.Length;
            byte[] byteArray = new byte[length];
            for(int i = 0; (i<length); i++)
            {
                Char c = unicodeBraille[i];
                if ((c < 0x2800) || (c > 0x283F))
                {
                    string message = string.Format("Illegal value for Unicode Braille = 0x{0:x}", c);
                    Logger.LogCF(string.Format(": {0}", message));
                    return false;
                }
                byte mappedByte = byteMap[c - 0x2800]; // Map from the 0x2800..0x283F interval to the corresponding valie to write to the file
                byteArray[i] = mappedByte;
            }
            return WriteToFile(byteArray, fullFileName);
        }


        /// <summary>
        /// Writes the byteList to the file specified without any conversion !
        /// </summary>
        /// <param name="byteList"></param>
        /// <param name="fullFileName"></param>
        /// <returns></returns>
        public bool WriteToFile(List<byte> byteList, string fullFileName)
        {
            //throw(new Exception("Not Implemented yet"));
            int length = byteList.Count;
            byte[] byteArray = new byte[length];
            byteArray = byteList.ToArray();
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
