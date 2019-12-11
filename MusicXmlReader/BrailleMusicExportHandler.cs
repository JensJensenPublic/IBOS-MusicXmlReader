using System;
using System.Collections.Generic;
using System.Text;
using MusicXmlReaderModel;
using System.IO;
using System.Windows.Forms;


namespace MusicXmlReader
{

    /// <summary>
    /// Class used for isolating UI code related to export of Music Braille i a class separate from MainForm
    /// </summary>
    public class BrailleMusicExportHandler
    {

        // The following 6 member variables are just references to the similar objects defined in MainForm. This is the price for isolating this code i a separate class! 
        Model model;
        bool developerMode;
        ParameterInputHandler parameterInputHandler;
        MessageHandler messageHandler;
        SaveFileDialog saveBrailleFileDialog;
        UserPreferencesHandler userPreferencesHandler;


        private BrailleFileHandler.FileEncoding GetCultureDependentEncoding(Model.BrailleDeviceEnum device)
        {
            BrailleFileHandler.FileEncoding fileEncoding = BrailleFileHandler.FileEncoding.BRF_Unicode; // Overall default
            string cultureString = ResourcesForUI.DirectoryNames_CultureString;
            switch (device)
            {
                case Model.BrailleDeviceEnum.Embosser:
                    {
                        switch (cultureString)
                        {
                            case "da-DK": fileEncoding = BrailleFileHandler.FileEncoding.BRF_ASCII; break; // CHANGED WITH RESPECT TO 3.0 after input from Lars Petersen
                            case "en-US": fileEncoding = BrailleFileHandler.FileEncoding.BRF_ASCII; break; // Same as in 3.0
                            default: break;
                        }
                        break;
                    }
                case Model.BrailleDeviceEnum.NoteTaker:
                    switch (cultureString)
                    {
                        case "da-DK": fileEncoding = BrailleFileHandler.FileEncoding.BRL_OctoBraille_1252; break; // Same as in 3.0
                        case "en-US": fileEncoding = BrailleFileHandler.FileEncoding.BRF_ASCII; break; // Same as in 3.0
                        default: break;
                    }
                    break;
                default: break;
            }
            Logger.LogCF(string.Format("({0}) returns {1}", device.ToString(), fileEncoding.ToString()));
            return fileEncoding;
        }




        public bool ScoreIsLoaded()
        {
            if (null == model.EventDescriptionList)
            {
                Logger.LogCF(": No MusicXml file is currently loaded!");
                Utilities.UtilityClient.ShowWarning((int)ModelMessageEnum.UnspecifiedMusicXmlFile, "", "");
                return false;
            }
            return true;
        }

        public bool ScoreIsSupported(Model.BrailleStyleEnum brailleFormat)
        {
            if (Model.BrailleStyleEnum.BANA2015 == brailleFormat) return true; // BANA2015 handles multiple staffs, so need to warn about it
            int numberOfParts = model.partList.NumberOfParts();
            int NumberOfEnabledMusicBrailleParts = model.NumberOfEnabledMusicBrailleParts;
            Logger.LogCF(string.Format(": Number of parts = {0} Number of Enabled Music Braille Parts = {1}", numberOfParts, NumberOfEnabledMusicBrailleParts));
            if (NumberOfEnabledMusicBrailleParts > 1)
            {
                Utilities.UtilityClient.ShowWarning((int)ModelMessageEnum.ToManyPartForExportToMusicBraille, "", "");
            }
            return true; // We issue a warning when more than one part is enabled, but we do not prevent the export.
        }

        public void ExportMusicBrailleToFile(Model.BrailleDeviceEnum device, int charsPerLine, int linesPerPage, Model.BrailleStyleEnum brailleStyle)
        {
            ExportMusicBrailleToFile(GetCultureDependentEncoding(device), charsPerLine, linesPerPage, brailleStyle);
        }


        /// <summary>
        /// Common procedure for all combinations of the 4 input parameters
        /// </summary>
        /// <param name="encoding"></param>
        /// <param name="charsPerLine"></param>
        /// <param name="linesPerPage"></param>
        /// <param name="format"></param>
        public void ExportMusicBrailleToFile(BrailleFileHandler.FileEncoding fileEncoding, int charsPerLine, int linesPerPage, Model.BrailleStyleEnum brailleStyle)
        {

            if (!ScoreIsLoaded()) return; // Beeps and logs.
            if (!ScoreIsSupported(brailleStyle)) return; // Shows warning dialog
            // In this simple implementation the file format is determined by the localization !

            //BrailleFileHandler.FileEncoding fileEncoding = GetCultureDependentEncoding();
            BrailleFileHandler brailleFileHandler = BrailleFileHandler.Create(fileEncoding, charsPerLine, linesPerPage);
            StaffList brailleRepresentations = model.GetBrailleRepresentation(brailleFileHandler.CharsPerLine, brailleFileHandler.LinesPerForm, brailleStyle);
            this.ExportMusicBrailleToFile(brailleFileHandler, brailleRepresentations);
        }




        /// <summary>
        /// Common handling of all file formats and styles
        /// </summary>
        /// <param name="brailleFileHandler"></param>
        public void ExportMusicBrailleToFile(BrailleFileHandler brailleFileHandler, StaffList brailleRepresentations)
        {
            this.WriteToFiles(model.TheMusicXmlFileName, brailleFileHandler, brailleRepresentations);
        }


        /// <summary>
        /// Common handling of all fileformats
        /// The user is prompted for the formatting parameters
        /// </summary>
        /// <param name="fileEncoding"></param>
        public void ExportMusicBrailleToFile(BrailleFileHandler.FileEncoding fileEncoding, Model.BrailleStyleEnum brailleStyle)
        {
            if (!ScoreIsLoaded()) return; // Beeps and logs.
            if (!ScoreIsSupported(brailleStyle)) return; // Shows warning dialog
            bool acceptCancel = false; //  Do not accept cancel as "use default parameters"
            bool validParams = parameterInputHandler.GetMusicBrailleFormatParameters(acceptCancel); // Prompt the user for formatting parameters.
            if (!validParams)
            {
                UiUtilities.Beep();
                return; // The user entered invalid values
            }
            BrailleFileHandler brailleFileHandler = BrailleFileHandler.Create(fileEncoding, model.UserPreferences.CharsPerLine, model.UserPreferences.LinesPerForm);
            StaffList brailleRepresentations = model.GetBrailleRepresentation(brailleFileHandler.CharsPerLine, brailleFileHandler.LinesPerForm, brailleStyle);
            this.ExportMusicBrailleToFile(brailleFileHandler, brailleRepresentations);
        }






        /// <summary>
        /// Prompt the user for a path and writes the Music Braille information to one or more files.
        /// </summary>
        /// <param name="xmlFileName">The name of the MusicXml File to be exported</param>
        /// <param name="brailleFileHandler">The Braille File handler to use (contents Braille encoding, width, height etc)</param>
        /// <param name="brailleRepresentations">The StaffList containing the actual Music Braille information to export</param>
        public void WriteToFiles(string xmlFileName, BrailleFileHandler brailleFileHandler, StaffList brailleRepresentations)
        {
            string regressionTestDirectory = null; // Will be set to point to the latest directory containing the Music Braille files for the same score. Null if not found.
            BrailleFileHandler developerBrailleFileHandler = BrailleFileHandler.Create(BrailleFileHandler.FileEncoding.BRF_Unicode, 0, 0); // Only osed in Experimental Mode

            if (null == brailleRepresentations)
            {
#warning TODO Localize
                string message = "Failed to convert to Braille.";
                Logger.LogCF(string.Format(": {0})", message));
                UiUtilities.Beep();
#warning TODO Localize
                messageHandler.ShowMessage(message + "\nLogfile contains detailled information.");
                return;
            }

            // Conversion succeeded. Determine and create a directory for saving the files
            string fileFormatName = brailleFileHandler.GetFileFormat(); // Currently "BRF_Unicode" , "BRL_OctoBraille_1252" or "BRF_ASCII"     
            string initialDirectory = Path.Combine(Path.GetDirectoryName(xmlFileName), fileFormatName); // Such as: "Examples\BRF_ASCII"
            string scoreName = Path.GetFileNameWithoutExtension(xmlFileName); // Such as "Billie_Jean"
            initialDirectory = Path.Combine(initialDirectory, scoreName); // Such as // Such as: "Examples\BRF_ASCII\Billie_Jean"
            // If DeveloperMode is enabled we change the initialDirectory and determine a directory to be used by the regression test.
            initialDirectory = this.PepareRegressionTest(initialDirectory, xmlFileName, ref regressionTestDirectory); // Such as: "Examples\BRF_ASCII\Billie_Jean\2019.12.24"
            if (!Directory.Exists(initialDirectory))
            {
                Directory.CreateDirectory(initialDirectory);
                Logger.LogCF(string.Format(": Created directory '{0}'", initialDirectory));
            }

            //Prompt user for filename
            DialogResult dialogResult = PromptForSavePath(xmlFileName, brailleFileHandler, initialDirectory);
            if (DialogResult.OK != dialogResult)
            {
                Logger.LogCF(string.Format(": SaveDialog returned {0}", dialogResult.ToString()));
                return;
            }

            // Create and write the files
            bool allOk = true;
            List<string> fileNames = new List<string>(); // Only for collecting filenames for a MessageBox in case of success.
            foreach (Staff staff in brailleRepresentations.AllStaffs)
            {
                if (staff.Enabled)
                {
                    // Write the Music Braille representation of this staff to a file
                    string userFileName = this.GetFileName(saveBrailleFileDialog.FileName, staff.MusicBrailleFilenameAttribute); // Insert staff number within part for grand staffs
                    this.WriteToFile(brailleFileHandler, staff.FullBrailleRepresentation, userFileName, ref allOk, fileNames, staff.BrailleMusicFormattedPageSize);

                    // If DeveloperModeSupport is enabled Repeat for Developer representation: 
                    // Create and use a separate subdirectory under  for the Development files under the directory of saveBrailleFileDialog.FileName and place the Developer files there.
                    string developerFileName = this.GetDeveloperFileName(saveBrailleFileDialog.FileName, userFileName); // Returns null unless DeveloperModeSupport is enabled !!
                    this.WriteToFile(developerBrailleFileHandler, staff.FullDeveloperBrailleRepresentation, developerFileName, ref allOk, fileNames, staff.BrailleMusicFormattedPageSize);
                }
            }

            // Report result
            string directory = Path.GetDirectoryName(saveBrailleFileDialog.FileName);
            LogResult(brailleRepresentations.Staffs.Count, allOk,directory, fileNames); // To LogFile and MessageBox 
            if (allOk)
            {
                userPreferencesHandler.BrailleMusicDirectory = directory;
            }
            // If DeveloperMode is enabled we execute a simple regressiontest and report the result to the user/developer
            this.ExecuteRegressionTest(initialDirectory, Path.GetDirectoryName(saveBrailleFileDialog.FileName), regressionTestDirectory);
          
        }



        /// <summary>
        /// This is actually an abuse of the Windows Forms SaveFileDialog as a "FolderBrowserDialog"
        /// But everybody claims that the "FolderBrowserDialog" has a verty bad reputation, so we tweek the SaveFileDialog a little instead:
        /// </summary>
        /// <param name="xmlFileName">The name including full path of the original MusicXml file</param>
        /// <param name="brailleFileHandler">The BrailleFileHAndler used for generation of the file(s) to be saved</param>
        /// <param name="initialDirectory">The suggested directory for saving the file(s)</param>
        /// <returns></returns>
        DialogResult PromptForSavePath(string xmlFileName, BrailleFileHandler brailleFileHandler, string initialDirectory)
        {
            string fileFormatName = brailleFileHandler.GetFileFormat(); // Currently "BRF_Unicode" , "BRL_OctoBraille_1252" or "BRF_ASCII"
            string extension = brailleFileHandler.GetExtension(); // Currently ".brf" or ".brl" Maybe later ".pef" ?  
            saveBrailleFileDialog.InitialDirectory = initialDirectory;
            string fileName = Path.GetFileName(xmlFileName); // Without path.
            string fileNameWithNewExtension = Path.ChangeExtension(fileName, extension);
            saveBrailleFileDialog.FileName = fileNameWithNewExtension;
            saveBrailleFileDialog.DefaultExt = extension;
            saveBrailleFileDialog.Filter = string.Format("{0}|*{1}", fileFormatName, extension); // Shown as for instance "BRF_ASCII (*.brf)" accepting all .brf files.
            DialogResult dialogResult = saveBrailleFileDialog.ShowDialog();
            return dialogResult;
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
        bool WriteToFile(BrailleFileHandler brailleFileHandler, string contents, string fileName, ref bool allOk, List<string> fileNames, int nCharacters)
        {
            if (null == fileName) return true;
            bool result = brailleFileHandler.WriteToFile(contents, fileName, true); // Taking in account width and height
            Logger.LogCF(string.Format(": Export of single file containing {0} Braille Characters to '{1}' {2}", nCharacters, fileName, result ? "succeded" : "failed"));
            allOk = allOk && result;
            fileNames.Add(Path.GetFileName(fileName) + "\r\n");
            return result;
        }


        /// <summary>
        /// Simple convenience method for cleaning up main code
        /// </summary>
        /// <param name="numberOfStaffs"></param>
        /// <param name="allOk"></param>
        /// <param name="exportPath"></param>
        /// <param name="fileNames"></param>
        private void LogResult(int numberOfStaffs, bool allOk, string exportPath, List<string> fileNames)
        {
            // allOk = false; // For test only !

            string logLine = string.Format("Export of {0} staffs to {1} files {2}. The files were exported to: \r\n\r\n{3}\r\n", numberOfStaffs, fileNames.Count, allOk ? "succeded" : "failed", exportPath);
            Logger.LogCF(": " + logLine);
            // We need to localize the messagebox, so we use a less complicated text:
            string exportOfMusicBraille = "Export of Music Braille";
            string result = allOk ? "Succeeded" : "Failed";
            string directory = "Directory";
            string files = "Files";

            StringBuilder message = new StringBuilder();
            message.AppendLine(string.Format("{0} {1}.",exportOfMusicBraille,result));
            if (allOk)
            {
                // This information only makes sense in case of success.
                message.AppendLine();
                message.AppendLine(string.Format("{0}:", directory));
                message.AppendLine();
                message.AppendLine(exportPath);
                message.AppendLine();
                message.AppendLine(string.Format("{0} :", files));
                message.AppendLine();
                foreach (string fileName in fileNames)
                {
                    message.Append(fileName); // The filename already contains \r\n
                }
            }
            messageHandler.ShowMessage(message.ToString());
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
            string result = Path.ChangeExtension(baseFileName, fileNameAttribute + Path.GetExtension(baseFileName));
            return result;
        }

        #region developerModeSupport

        /// <summary>
        ///  Prepare for a simple regression test between the currently generated MusicBraille files and the treviously generated MusicBraille files, representing the same score.
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public string PepareRegressionTest(string initialDirectory, string theMusicXmlFileName, ref string regressionTestDirectory)
        {
            if (!this.developerMode) return initialDirectory; // Unchanged
            string newInitialDirectory = null;
            // For debugging purposes we save in different directories in order to be able to check with previous versions !
            //string scoreName = Path.GetFileNameWithoutExtension(theMusicXmlFileName);
            string nowString = System.DateTime.Now.ToString();
            nowString = nowString.Replace('-', '.'); // Get rid of chars illegal in file names
            nowString = nowString.Replace(':', '.');
            //string initialDirectoryWithScoreName = Path.Combine(initialDirectory, scoreName);
            newInitialDirectory = Path.Combine(initialDirectory, nowString);
            // Now find the outcome of the latest conversion to Braille of this score:
            if (Directory.Exists(initialDirectory))
            {
                string[] olderDirectories = System.IO.Directory.GetDirectories(initialDirectory);
                DateTime latestCreationTime = DateTime.MinValue;

                foreach (string olderDirectory in olderDirectories)
                {
                    DateTime creationTime = Directory.GetCreationTimeUtc(olderDirectory);
                    if (creationTime > latestCreationTime)
                    {
                        regressionTestDirectory = olderDirectory;
                        latestCreationTime = creationTime;
                    }
                }
            }
            Logger.LogCF(string.Format(": InitialDirectory  =      {0}", initialDirectory));
            Logger.LogCF(string.Format(": LatestInitialDirectory = {0}", (null == regressionTestDirectory) ? "null" : regressionTestDirectory));
            // Returns the path  of the newest Music Braille files for this score. 
            return newInitialDirectory;
        }


        /// <summary>
        ///  Do a simple regression test between the currently generated MusicBraille files and the previously generated MusicBraille files, representing the same score.
        /// </summary>
        /// <param name="currentDirectory"></param>
        /// <param name="savePath"></param>
        /// <returns></returns>
        public string ExecuteRegressionTest(string currentDirectory, string savePath, string regressionTestDirectory)
        {
            if (!this.developerMode) return null;
            if (null == regressionTestDirectory)
            {
                string format = "{0}Regressiontest can not be executed.{1}No old version to compare to{2}{3}";
                messageHandler.ShowMessage(string.Format(format,"","\r\n","\r\n",currentDirectory));
                Logger.LogCF(string.Format(format,": ", " ", " ",currentDirectory));
                return null;
            }
            // Do a simple regression-test based on the result of phe previous operation on the same score

            // First generate a string for Logging and UI
            const int nLevels = 2;
            string oldPath = Utilities.GetEndOfPath(regressionTestDirectory, nLevels);
            int oldCount = Directory.GetFiles(regressionTestDirectory).Length;
            string newPath = Utilities.GetEndOfPath(currentDirectory, nLevels);
            int newFiles = Directory.GetFiles(currentDirectory).Length;
            string text = string.Format("Regression-test comparing directories:\r\n...{0} ({1} files)\r\n...{2} ({3} files)", oldPath,oldCount, newPath, newFiles);

            // Then compare the directories
            string result;
            string difs = null;
            difs = Utilities.CompareDirectories(regressionTestDirectory, currentDirectory);
            if (null == difs)
            {
                result = string.Format("Regression-test passed, all pairs of files are equal");
            }
            else
            {
                Logger.LogCF(string.Format(": Regression-test failed {0}", difs));
                result = string.Format("{0}", difs);
            }
            // During development we want to open Windows Explorer here to inspect the result:
            string developerPath = Path.Combine(savePath, "Developer");
            Logger.LogCF(string.Format(": Directory={0}", developerPath));
            messageHandler.ShowMessage(text + "\r\n" + result);
            Utilities.RunExeWithArgument(Utilities.ExplorerExe, developerPath);
            return text + "\r\n" + result;
        }


        /// <summary>
        /// Create a "Developer" subdirectory for all files created in DeveloperMode
        /// </summary>
        /// <param name="baseFileName"></param>
        /// <param name="fileNameAttribute"></param>
        /// <returns></returns>
        public string GetDeveloperFileName(string baseFileName, string userFileName)
        {
            if (!this.developerMode) return null;
            string directory = Path.Combine(Path.GetDirectoryName(baseFileName), "Developer");
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
            string result = Path.Combine(directory, Path.ChangeExtension(Path.GetFileName(userFileName), "txt"));
            return result;
        }

   


        #endregion


        private BrailleMusicExportHandler()
        { }


        private BrailleMusicExportHandler(Model model, ParameterInputHandler parameterInputHandler,MessageHandler messageHandler, SaveFileDialog saveBrailleFileDialog, bool developerMode, UserPreferencesHandler userPreferencesHandler)
        {
            this.model = model;
            this.parameterInputHandler = parameterInputHandler;
            this.messageHandler = messageHandler;
            this.saveBrailleFileDialog = saveBrailleFileDialog;
            this.developerMode = developerMode;
            this.userPreferencesHandler = userPreferencesHandler;
            //developerModeSupport = DeveloperModeSupport.Create(developerMode);
        }


        public static BrailleMusicExportHandler Create( Model model, ParameterInputHandler parameterInputHandler,MessageHandler messageHandler,SaveFileDialog saveBrailleFileDialog, bool developerMode,UserPreferencesHandler userPreferencesHandler)
        {
            return new BrailleMusicExportHandler(model, parameterInputHandler, messageHandler,saveBrailleFileDialog, developerMode,  userPreferencesHandler);
        }
    }
}
