using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MusicXmlReaderModel
{
    class BrailleFileHandler_Generic : BrailleFileHandler
    {
        private string extensionName = "";
        private Encoding encoding = Encoding.Default;

        public override int GetCodePage()
        {
            return encoding.CodePage;
        }

        public override string GetExtension()
        {
            return extensionName;
        }


        public override bool IsValidBrailleMusic(string fileName)
        {
            byte[] bytesReadFromFile = ReadAsBinary(fileName);
            return BrailleFileUnicodeTester.Create(this.encoding).Test(bytesReadFromFile,base.controlCharacters); 
        }


        public override string GetFileFormat()
        {
            if (Encoding.Unicode == encoding) return "Unicode(UTF-16)"; // Encoding.Unicode means rtf-16
            return encoding.EncodingName;
        }

        internal BrailleFileHandler_Generic(int charsPerLine, int linesPerForm, string extensionName, Encoding encoding)
        {
            this.charsPerLine = charsPerLine;
            this.linesPerForm = linesPerForm;
            this.extensionName = extensionName;
            this.encoding = encoding;
            // No initialisation of conversion tables are needed here !
        }


        public override bool WriteToFile(string unicodeBraille, string fullFileName, bool acceptControls)
        {
            bool result = false;
            using (StreamWriter sw = new StreamWriter(File.Open(fullFileName, FileMode.Create), encoding)) // As UTF8 parameter, but starts without  EF BB BF Works with IbPrint(65001)
            //using (StreamWriter sw = new StreamWriter(File.Open(fullFileName, FileMode.Create),Encoding.Default)) // Not the same as without parameter
            //using (StreamWriter sw = new StreamWriter(File.Open(fullFileName, FileMode.Create), Encoding.Unicode))  // Means utf-16 !! Seems to work Works with IbPrint(1200)
            //using (StreamWriter sw = new StreamWriter(File.Open(fullFileName, FileMode.Create),Encoding.UTF32)) // Seems to generate OK bin pattern IbPrint fails witn (65005)
            //using (StreamWriter sw = new StreamWriter(File.Open(fullFileName, FileMode.Create),Encoding.UTF8)) // As without parameter, but starts with  EF BB BF  Works with IbPrint(65001)
            {
                try
                {
                    sw.Write(unicodeBraille);
                    result = true;
                }
                catch (Exception e)
                {
                    Logger.LogCFE(e);
                }
            }
            return result;
        }

        /// <summary>
        ///  Reads a file containing MusicBraille information and returns its contents as a UNICODE string
        /// </summary>
        /// <param name="fullFileName"></param>
        /// <returns></returns>
        public override string ReadFromFile(string fullFileName)
        {
            string result = null;
            try
            {
                result = System.IO.File.ReadAllText(fullFileName,encoding);
                Logger.LogCF(string.Format(": read {0} characters from {1}", result.Length, fullFileName));
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
