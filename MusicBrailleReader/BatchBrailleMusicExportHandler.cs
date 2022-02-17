using System.Collections.Generic;
using MusicXmlReaderModel;
using System.IO;

namespace MusicBrailleReader
{
    /// <summary>
    /// Stolen from the BrailleMusicReader project.
    /// And renamed to avoid confusion.
    /// Kept structure but made a lot of simplifications because User interaction is not needed.
    /// </summary>
    class BatchBrailleMusicExportHandler
    {
        private Model model;

        /// <summary>
        /// The only public method. 
        /// Generates Music Braille files for the .musicxml file already loaded and decoded by the Model and available through this.model
        /// All Music Braille generation parameters are fixed values defined in the start of the method, but could easily be transferred as call parameters if desired.
        /// </summary>
        public void Write()
        {
            int charsPerLine = 40;
            int linesPerPage = 40;
            Model.BrailleStyleEnum brailleStyle = Model.BrailleStyleEnum.BANA2015;
            string profileName = "Punktprinter";
            BrailleFileHandler.FileEncoding fileEncoding = BrailleFileHandler.FileEncoding.BRF_ASCII;
            BrailleFileHandler brailleFileHandler = BrailleFileHandler.Create(fileEncoding, charsPerLine, linesPerPage, model.MetaInformation);
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
        /// Prompt the user for a path and writes the Music Braille information to one or more files.
        /// </summary>
        /// <param name="xmlFileName">The name of the MusicXml File to be exported</param>
        /// <param name="brailleFileHandler">The Braille File handler to use (contents Braille encoding, width, height etc)</param>
        /// <param name="brailleRepresentations">The StaffList containing the actual Music Braille information to export</param>
        private void WriteToFiles(string xmlFileName, BrailleFileHandler brailleFileHandler, StaffList brailleRepresentations, string profileName)
        {
            // Conversion succeeded. Determine and create a directory for saving the files
            // If a profilename exists it is used, otherwise we generate a name from the file format: Currently "BRF_Unicode" , "BRL_OctoBraille_1252" or "BRF_ASCII"   
            string fileFormatName = (string.IsNullOrEmpty(profileName)) ? brailleFileHandler.GetFileFormat() : profileName;
            string destinationDirectory = Path.Combine(Path.GetDirectoryName(xmlFileName), fileFormatName); // Such as: "Examples\BRF_ASCII"
            string scoreName = Path.GetFileNameWithoutExtension(xmlFileName); // Such as "Billie_Jean"
            if (!Directory.Exists(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
                Logger.LogCF(string.Format(": Created directory '{0}'", destinationDirectory));
            }
            
            bool allOk = true;
            List<string> fileNames = new List<string>(); // Only for collecting filenames for a MessageBox in case of success.
            foreach (Staff staff in brailleRepresentations.AllStaffs)
            {
                if (staff.Enabled)
                {
                    string extension = brailleFileHandler.GetExtension(); // Currently ".brf" or ".brl" Maybe later ".pef" ?  
                    string fileName = Path.ChangeExtension(xmlFileName, extension); 
                    // Write the Music Braille representation of this staff to a file
                    string userFileName = this.GetFileName(fileName, staff.MusicBrailleFilenameAttribute); // Insert staff number within part for grand staffs
                    string fullUserFileName = Path.Combine(destinationDirectory, Path.GetFileName(userFileName));
                    this.WriteToFile(brailleFileHandler, staff.FullBrailleRepresentation, fullUserFileName, ref allOk, fileNames, staff.BrailleMusicFormattedPageSize);
                }
            }
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
        

        BatchBrailleMusicExportHandler(Model model)
        {
            this.model = model;
        }
        
        public static BatchBrailleMusicExportHandler Create(Model model)
        {
            return new BatchBrailleMusicExportHandler(model);
        }

    }
}
