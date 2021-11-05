using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MusicXmlReaderModel; // As Namespace, not as reference

namespace MusicBrailleReader
{
    class BatchTransscriber
    {
        private Model model;


        public bool Transscribe(string sourceDir)
        {
            BatchBrailleMusicExportHandler brailleMusicExportHandler = BatchBrailleMusicExportHandler.Create(model);
            Logger.LogCF(string.Format("({0})",sourceDir));
            string[] sourceFiles = Directory.GetFiles(sourceDir);
            Logger.LogCF(string.Format(": Transscribing {0} files.", sourceFiles.Length));
            foreach (string fileName in sourceFiles)
            {
                string ext = Path.GetExtension(fileName);
                if (0 == string.Compare(".musicxml",  ext))
                {
                    model.LoadMusicXmlFile(fileName, true); // True <==> USe default settings
                    Logger.LogCF(string.Format(": Loaded {0}", fileName));

                    brailleMusicExportHandler.Write();
                }

            }
            Logger.LogCF("-");
            return true;
        }

#if false
        private void Write()
        {
            int charsPerLine = 40;
            int linesPerPage = 40;
            Model.BrailleStyleEnum brailleStyle = Model.BrailleStyleEnum.BANA2015;
            string profileName = "test";

            BrailleFileHandler.FileEncoding fileEncoding = BrailleFileHandler.FileEncoding.BRF_ASCII;
            BrailleFileHandler brailleFileHandler = BrailleFileHandler.Create(fileEncoding, charsPerLine, linesPerPage);
            StaffList brailleRepresentations = model.GetBrailleRepresentation(brailleFileHandler.CharsPerLine, brailleFileHandler.LinesPerForm, brailleStyle);
            this.ExportMusicBrailleToFile(brailleFileHandler, brailleRepresentations, profileName);
        }


        /// <summary>
        /// Insert fileNameAttribute just in front of the existing extension.
        /// For instance ".brf" becomes ".Piano.1.bfr"
        /// </summary>
        /// <param name="baseFileName"></param>
        /// <param name="fileNameAttribute"></param>
        /// <returns></returns>
        private string GetFileName(string baseFileName, string fileNameAttribute)
        {
            char[] trimChars = { '.' };
            string trimmedfileNameAttribute = fileNameAttribute.TrimEnd(trimChars); // Avoid ".." if the partname ends with "."
            string result = Path.ChangeExtension(baseFileName, trimmedfileNameAttribute + Path.GetExtension(baseFileName));
            return result;
        }



        /// <summary>
        /// Common procedure for all combinations of the 4 input parameters Stolen from BrailleMusicExportHandler
        /// </summary>
        /// <param name="encoding"></param>
        /// <param name="charsPerLine"></param>
        /// <param name="linesPerPage"></param>
        /// <param name="format"></param>
        private void ExportMusicBrailleToFile(BrailleFileHandler.FileEncoding fileEncoding, int charsPerLine, int linesPerPage, Model.BrailleStyleEnum brailleStyle, string profileName)
        {


            //BrailleFileHandler.FileEncoding fileEncoding = GetCultureDependentEncoding();
            BrailleFileHandler brailleFileHandler = BrailleFileHandler.Create(fileEncoding, charsPerLine, linesPerPage);
            StaffList brailleRepresentations = model.GetBrailleRepresentation(brailleFileHandler.CharsPerLine, brailleFileHandler.LinesPerForm, brailleStyle);
            this.ExportMusicBrailleToFile(brailleFileHandler, brailleRepresentations, profileName);
        }


        /// <summary>
        /// Common handling of all file formats and styles
        /// </summary>
        /// <param name="brailleFileHandler"></param>
        private void ExportMusicBrailleToFile(BrailleFileHandler brailleFileHandler, StaffList brailleRepresentations, string profileName)
        {
            this.WriteToFiles(model.TheMusicXmlFileName, brailleFileHandler, brailleRepresentations, profileName);
        }

        /// <summary>
        /// Essentially just wraps brailleFileHandler.WriteToFile into a lot of error reporting etc
        /// </summary>
        /// <param name="brailleFileHandler"></param>
        /// <param name="contents"></param>
        /// <param name="fileName"></param>
        /// <param name="allOk"></param>
        /// <param name="fileNames"></param>
        /// <param name="nCharacters"></param>
        /// <returns></returns>
        private bool WriteToFile(BrailleFileHandler brailleFileHandler, string contents, string fileName, ref bool allOk, List<string> fileNames, int nCharacters)
        {
            if (null == fileName) return true;
            bool result = brailleFileHandler.WriteToFile(contents, fileName, true); // Taking in account width and height
            Logger.LogCF(string.Format(": Export of single file containing {0} Braille Characters to '{1}' {2}", nCharacters, fileName, result ? "succeded" : "failed"));
            allOk = allOk && result;
            fileNames.Add(Path.GetFileName(fileName) + "\r\n");
            return result;
        }
        
        /// <summary>
        /// Prompt the user for a path and writes the Music Braille information to one or more files.
        /// </summary>
        /// <param name="xmlFileName">The name of the MusicXml File to be exported</param>
        /// <param name="brailleFileHandler">The Braille File handler to use (contents Braille encoding, width, height etc)</param>
        /// <param name="brailleRepresentations">The StaffList containing the actual Music Braille information to export</param>
        private void WriteToFiles(string xmlFileName, BrailleFileHandler brailleFileHandler, StaffList brailleRepresentations, string profileName)
        {
            string regressionTestDirectory = null; // Will be set to point to the latest directory containing the Music Braille files for the same score. Null if not found.
            BrailleFileHandler developerBrailleFileHandler = BrailleFileHandler.Create(BrailleFileHandler.FileEncoding.BRF_Unicode, 0, 0); // Only osed in Experimental Mode


            // Conversion succeeded. Determine and create a directory for saving the files
            // If a profilename exists it is used, otherwise we generate a name from the file format: Currently "BRF_Unicode" , "BRL_OctoBraille_1252" or "BRF_ASCII"   
            string fileFormatName = (string.IsNullOrEmpty(profileName)) ? brailleFileHandler.GetFileFormat() : profileName;
            string initialDirectory = Path.Combine(Path.GetDirectoryName(xmlFileName), fileFormatName); // Such as: "Examples\BRF_ASCII"
            string scoreName = Path.GetFileNameWithoutExtension(xmlFileName); // Such as "Billie_Jean"
 
            bool allOk = true;
            List<string> fileNames = new List<string>(); // Only for collecting filenames for a MessageBox in case of success.
            foreach (Staff staff in brailleRepresentations.AllStaffs)
            {
                if (staff.Enabled)
                {
                    // Write the Music Braille representation of this staff to a file
                    string userFileName = this.GetFileName(saveBrailleFileDialog.FileName, staff.MusicBrailleFilenameAttribute); // Insert staff number within part for grand staffs
                    this.WriteToFile(brailleFileHandler, staff.FullBrailleRepresentation, userFileName, ref allOk, fileNames, staff.BrailleMusicFormattedPageSize);
                }
            }   

        }
#endif




        private BatchTransscriber(Model model)
        {
            this.model = model;
        }

        public static BatchTransscriber Create(Model model)
        {
            return new BatchTransscriber(model);
        }
    }
}
