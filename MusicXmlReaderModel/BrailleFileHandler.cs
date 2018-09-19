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
        public enum FileFormat{BRF_ASCII,BBRF_Unicode,PEF};
        private FileFormat fileFormat;

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
