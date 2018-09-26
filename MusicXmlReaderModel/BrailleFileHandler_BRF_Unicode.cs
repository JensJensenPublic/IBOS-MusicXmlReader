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
            try
            {
                System.IO.File.WriteAllText(fullFileName, unicodeBraille);
                result = true;
            }
            catch (Exception e)
            {
                Logger.LogCFE(e);
            }
            return result;
        }

        public override string ReadFromFile(string fullFileName)
        {
            string result = "";
            using (BinaryReader br = new BinaryReader(File.Open(fullFileName, FileMode.Open)))
            {
                result = System.IO.File.ReadAllText(fullFileName);
            }
            return result;
        }
    }


}
