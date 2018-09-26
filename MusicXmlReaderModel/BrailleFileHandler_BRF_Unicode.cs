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
        public override string GetExtension()
        {
            return ".rbf";
        }

        public override string GetFileFormat()
        {
            return "BRF_Unicode";
        }

        public override bool WriteToFile(string unicodeBraille, string fullFileName, bool acceptControls)
        {
            bool result = false;
            using (StreamWriter sw = new StreamWriter(File.Open(fullFileName, FileMode.Create)))
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
