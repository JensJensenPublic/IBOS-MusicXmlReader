using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


        public bool WriteToFile(List<byte> bytes, string fullFileName)
        {
            throw(new Exception("Not Implemented yet")); 
            bool result = true;
            return result;
        }

        public bool WriteToFile(string s, string fullFileName)
        {
            throw (new Exception("Not Implemented yet"));
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
