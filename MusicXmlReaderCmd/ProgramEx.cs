using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicXmlReaderModel;
using System.IO;

namespace MusicXmlReaderUI
{
    partial class Program
    {

        static private bool CheckDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Logger.LogCFOnce(string.Format(": Directory does not exist: {0}", path));
                return false;
            }
            return true;
        }

        static private void  CleanBaseDirectory(string baseDirectory)
        {
            if (!Directory.Exists(baseDirectory))
            {
                Logger.LogCFOnce(string.Format(": BaseDirectory '{0}' is not found. Nothing to clean.", baseDirectory));
                return;
            }

            FileInfo[] baseFiles = new DirectoryInfo(baseDirectory).GetFiles();
            int count = 0;
            foreach (FileInfo fileInfo in baseFiles)
            {
                string fullPath = Path.Combine(baseDirectory, fileInfo.Name);
                File.Delete(fullPath);
                count++;
            }
            Logger.LogCFOnce(string.Format(": Deleted {0} files from BaseDirectory '{1}'", count, baseDirectory));

        }


        /// <summary>
        /// Checks all files just generated against the similar files in the reference directory
        /// </summary>
        static private void CheckAgainstReference(string baseDirectory)
        {
            string referenceDirectory = Path.Combine(baseDirectory, "Reference");
            if (!CheckDirectory(baseDirectory)) return;
            if (!CheckDirectory(referenceDirectory)) return;
            FileInfo[] baseFiles = new DirectoryInfo(baseDirectory).GetFiles();
            FileInfo[] refFiles = new DirectoryInfo(referenceDirectory).GetFiles();
            int filesInBase = baseFiles.Length;
            int filesInReference = refFiles.Length;
            Logger.LogCFOnce(string.Format(": The BaseDirectory      contains {0} files: '{1}'", filesInBase, baseDirectory));
            Logger.LogCFOnce(string.Format(": The ReferenceDirectory contains {0} files: '{1}'", filesInReference, referenceDirectory));
            if (filesInBase != filesInReference)
            {
                Logger.LogCFOnce(string.Format("Base={0}", baseDirectory));
                Logger.LogCFOnce(string.Format("Reference={0}", referenceDirectory));
                Logger.LogCFOnce(string.Format(": Different number of files: Base={0} Reference={1}", filesInBase, filesInReference));
                return;
            }
            int successes = 0;
            // The two directories exist and contain the same number of files
            foreach (FileInfo fileInfo in refFiles)
            {
                string fileName = fileInfo.Name;
                string fullRefName = Path.Combine(referenceDirectory, fileName);
                string fullBaseName = Path.Combine(baseDirectory, fileName);
                if (!File.Exists(fullBaseName))
                {
                    Logger.LogCFOnce(string.Format(": File not found {0}", fullBaseName));
                }
                else
                {
                    long refLength = new FileInfo(fullRefName).Length;
                    long baseLength = new FileInfo(fullBaseName).Length;
                    if (refLength != baseLength)
                    {
                        Logger.LogCFOnce(string.Format(": Different length for file {0} Ref={1} base= {2}", fileName, refLength, baseLength));
                    }
                    else
                    {
                        string refstring = File.ReadAllText(fullRefName);
                        string basestring = File.ReadAllText(fullBaseName);
                        if (0 != string.Compare(refstring, basestring))
                        {
                            Logger.LogCFOnce(string.Format(": Equal size but different contents for {0}", fileName));
                        }
                        else
                        {
                            successes++;
                        }
                    }
                }
            }
            Logger.LogCFOnce(string.Format(": Compared {0} files. {1} were identical.", filesInReference, successes));
        }

        static int charactersPerLine = 32;
        static int linesPerPage = 32;
        static private BrailleFileHandler developerFileHandler = BrailleFileHandler.Create(BrailleFileHandler.FileEncoding.BRF_Unicode_utf8, charactersPerLine, linesPerPage);


        static string GetBaseDirectory(BrailleFileHandler.FileEncoding fileEncoding)
        {
            string subDir = "";
            switch (fileEncoding)
            {
                case BrailleFileHandler.FileEncoding.BRF_Unicode_utf8: subDir = "Unicode (UTF-8)"; break;
                case BrailleFileHandler.FileEncoding.BRF_ASCII:         subDir = "BRF_ASCII"; break;
                case BrailleFileHandler.FileEncoding.BRL_OctoBraille_1252: subDir = "BRL_OctoBraille_1252"; break;
            }
            return Path.Combine(Logger.LogFileDirectory, subDir);
        }

        static string GetDeveloperBaseDirectory(BrailleFileHandler.FileEncoding fileEncoding)
        {
            return Path.Combine(GetBaseDirectory(fileEncoding), "Developer");
        }

        /// <summary>
        /// Determine which fileEncodings to test
        /// </summary>
        static List<BrailleFileHandler.FileEncoding> fileEncodingsToTest = new List<BrailleFileHandler.FileEncoding>
        {
            BrailleFileHandler.FileEncoding.BRF_Unicode_utf8,
            BrailleFileHandler.FileEncoding.BRF_ASCII,
            BrailleFileHandler.FileEncoding.BRL_OctoBraille_1252
        };

            /// <summary>
            /// Generates all BrailleMusic files for the currently loaded MusicXml file
            /// </summary>
        static private bool GenerateBrailleMusicFiles()
        {

            //BrailleFileHandler.FileEncoding fileEncoding = BrailleFileHandler.FileEncoding.BRF_Unicode_utf8;
            ////BrailleFileHandler.FileEncoding fileEncoding = BrailleFileHandler.FileEncoding.BRF_ASCII;
            ////BrailleFileHandler.FileEncoding fileEncoding = BrailleFileHandler.FileEncoding.BRL_OctoBraille_1252;
            //BrailleFileHandler brailleFileHandler = BrailleFileHandler.Create(fileEncoding, charactersPerLine, linesPerPage);
            ////developerFileHandler = BrailleFileHandler.Create(BrailleFileHandler.FileEncoding.BRF_Unicode_utf8, charactersPerLine, linesPerPage);
            //if (ExportMusicBrailleToFile(brailleFileHandler)) filesExported++;

            model.ExperimentalCode = true;
            //BrailleFileHandler developerBrailleFileHandler = BrailleFileHandler.Create(BrailleFileHandler.FileEncoding.BRF_Unicode, 0, 0); // Only osed in Experimental Mode
            StaffList brailleRepresentations = model.GetBrailleRepresentation(32,32,Model.BrailleStyleEnum.BANA2015);
            if (null == brailleRepresentations)
            {
                Logger.LogCF(string.Format(": Failed to convert to Braille"));
                return false;
            }

            foreach (BrailleFileHandler.FileEncoding fileEncoding in fileEncodingsToTest)
            {
                BrailleFileHandler brailleFileHandler = BrailleFileHandler.Create(fileEncoding, charactersPerLine, linesPerPage);
                if (ExportMusicBrailleToFile(brailleFileHandler,brailleRepresentations)) filesExported++;
            }
            return true;

        }


        static private bool WriteToFile(BrailleFileHandler brailleFileHandler, string data, string directory, string fileName)
        {
            bool ok;
            string fullFileName = "";
            try
            {
                fullFileName = Path.Combine(directory, fileName);
                ok = brailleFileHandler.WriteToFile(data, fullFileName, true);
            }
            catch (Exception e)
            {
                Logger.LogCF(string.Format(": Exception caught while writing to {0} Message={1}", fullFileName, e.Message));
                return false;
            }
            return ok;
        }


        static private void CheckFileExistence(string directoryName, string fileName)
        {
            string fullFileName = Path.Combine(directoryName, fileName);
            if (File.Exists(fullFileName))
            {
                Logger.LogCFOnce(string.Format(": File already exists: '{0}'", fullFileName));
                staffsOverWritten++;
            }
        }

        /// <summary>
        /// Exports all staffs for a single MusicXml file to files, one file per staff
        /// Common handling of all file formats. Stolen from MainForm
        /// </summary>
        /// <param name="brailleFileHandler"></param>
        static private bool ExportMusicBrailleToFile(BrailleFileHandler brailleFileHandler, StaffList brailleRepresentations)
        {
            //model.ExperimentalCode = true;
            ////BrailleFileHandler developerBrailleFileHandler = BrailleFileHandler.Create(BrailleFileHandler.FileEncoding.BRF_Unicode, 0, 0); // Only osed in Experimental Mode
            //StaffList brailleRepresentations = model.GetBrailleRepresentation(brailleFileHandler.CharsPerLine, brailleFileHandler.LinesPerForm);
            //if (null == brailleRepresentations)
            //{
            //    Logger.LogCF(string.Format(": Failed to convert to Braille"));
            //    return false;
            //}

            // Conversion succeeded.
            // Keep all constants out of the loop: 

            string fileFormatName = brailleFileHandler.GetFileFormat(); // Currently "BRF_Unicode" , "BRL_OctoBraille_1252" or "BRF_ASCII" 
            string initialDirectory = Path.Combine(Logger.LogFileDirectory, fileFormatName);
            Utilities.CreateDirectory(initialDirectory);
            string developerDirectory = Path.Combine(initialDirectory, "Developer");
            Utilities.CreateDirectory(developerDirectory);
            string extension = brailleFileHandler.GetExtension(); // Currently ".brf" or ".brl" Maybe later ".pef" ?  
            string fileNameAttribute = brailleRepresentations.MusicBrailleFilenameAttribute;
            string tempFileName = Path.GetFileName(model.TheMusicXmlFileName);

            // Iterate over all staffs:
            bool allOk = true;
            bool ok = true;
            foreach (Staff staff in brailleRepresentations.Staffs)
            {
                model.UserSettings.SelectedStaffs = new List<StaffSelector>();
                model.UserSettings.SelectedStaffs.Add(StaffSelector.Create(staff.PartId, staff.StaffNumber)); // Select this specific staff
                if (staff.Enabled)
                {
                    ok = true;
                    string fileName = tempFileName + "." + staff.MusicBrailleFilenameAttribute + extension; // Use the staff id as a part of the file nams
                    CheckFileExistence(initialDirectory, fileName);
                    ok = ok && WriteToFile(brailleFileHandler, staff.FullBrailleRepresentation, initialDirectory, fileName);
                    string developerFileName = Path.ChangeExtension(fileName, "txt");
                    CheckFileExistence(developerDirectory, developerFileName);
                    ok = ok && WriteToFile(developerFileHandler, staff.FullDeveloperBrailleRepresentation, developerDirectory,developerFileName);
                    if (ok) staffsExported++;
                    allOk = allOk && ok;
                }
            }
            string logLine = string.Format("Export of {0} files to '{1}' {2}", brailleRepresentations.Staffs.Count, tempFileName, allOk ? "succeded" : "failed");
            Logger.LogCF(": " + logLine);
            return allOk;
        }


        /// <summary>
        /// Insert fileNameAttribute just in front of the existing extension.
        /// For instance ".brf" becomes ".Piano.1.bfr"
        /// </summary>
        /// <param name="baseFileName"></param>
        /// <param name="fileNameAttribute"></param>
        /// <returns></returns>
        private static string GetFileName(string baseFileName, string fileNameAttribute)
        {
            string result = Path.ChangeExtension(baseFileName, fileNameAttribute + Path.GetExtension(baseFileName));
            return result;
        }


        /// <summary>
        /// Create a "Developer" subdirectory for all files created in DeveloperMode
        /// </summary>
        /// <param name="baseFileName"></param>
        /// <param name="fileNameAttribute"></param>
        /// <returns></returns>
        private static string GetDeveloperFileName(string baseFileName, string fileNameAttribute)
        {
            string directory = Path.Combine(Path.GetDirectoryName(baseFileName), "Developer");
            string fileName = Path.GetFileName(GetFileName(baseFileName, fileNameAttribute));
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
            string result = Path.Combine(directory, Path.ChangeExtension(fileName, "txt"));
            return result;
        }

    }
}
