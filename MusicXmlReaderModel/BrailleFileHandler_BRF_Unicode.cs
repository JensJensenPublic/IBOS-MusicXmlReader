using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MusicXmlReaderModel
{
    class BrailleFileHandler_BRF_Unicode : BrailleFileHandler
    {
        public override int GetCodePage()
        {
            return 65001;
        }


        /// <summary>
        /// Simple mechanism for checking BrailleMusic encoded using UTF-8
        /// Use real Windows classes such as StreamReader with Encoding parameter for the actual decoding!
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public override bool IsValidBrailleMusic(string fileName)
        {
            byte[] bytesReadFromFile = ReadAsBinary(fileName);
            return BrailleFileUnicodeTester.Create().Test(bytesReadFromFile, base.controlCharacters);
        }

 

        public override string GetExtension()
        {
            return ".brf";
        }

        public override string GetFileFormat()
        {
            return "BRF_Unicode";
        }

        internal BrailleFileHandler_BRF_Unicode(int charsPerLine, int linesPerForm)
        {
            this.charsPerLine = charsPerLine;
            this.linesPerForm = linesPerForm;

            // No initialisation of conversion tables are needed here !
        }



        public override bool WriteToFile(string unicodeBraille, string fullFileName, bool acceptControls)
        {
            bool result = false;
            using (StreamWriter sw = new StreamWriter(File.Open(fullFileName, FileMode.Create))) // As UTF8 parameter, but starts without  EF BB BF Works with IbPrint(65001)
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
                result = System.IO.File.ReadAllText(fullFileName);
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
