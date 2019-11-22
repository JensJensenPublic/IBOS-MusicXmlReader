using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using MusicXmlReaderModel;

namespace MusicXmlReader
{
    /// <summary>
    /// For isolating parts of the MainForm functionality
    /// </summary>
    public class ImportHandler
    {
        private Model model;
        private MainForm mainForm;
        private string className = "ImportHandler";
        private string applicationName;

        /// <summary>
        /// Prevent construction
        /// </summary>
        private ImportHandler()
        {
        }

        private ImportHandler(Model model, MainForm mainForm, string applicationName)
        {
            this.model = model;
            this.mainForm = mainForm;
            this.applicationName = applicationName;
        }

        private string GetImportMessage(List<string> fileNames)
        {
            if (1 == fileNames.Count)
            {
                return string.Format("{0} {1}", ResourcesForUI.Status_Imported, Path.GetFileNameWithoutExtension(fileNames[0])); // Exactly one file: Show the name: "Copied Stardust.xml"   
            }
            else
            {
                return string.Format("{0} {1}  MusicXml {2}", ResourcesForUI.Status_Imported, fileNames.Count, ResourcesForUI.Status_files); // Any other number: "Copied n files"
            }
        }

        private void ShowImportMessageBox(string message, List<string> fileNames)
        {
            int maxCount = 20; // Limited by the size of the massageBox
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(message);
            sb.AppendLine();
            if (fileNames.Count > 1) // If exactly 1 file was imported it was listed in the caption. No need to repeat it here !
            {
                for (int i = 0; (i < maxCount) && (i < fileNames.Count); i++)
                {
                    sb.AppendLine(Path.GetFileNameWithoutExtension(fileNames[i])); // Hide the extension
                }

                if (fileNames.Count >= maxCount)
                {
                    sb.AppendLine("..."); // Localize later if wanted !
                }
            }
            MessageBox.Show(sb.ToString(), applicationName, MessageBoxButtons.OK);
        }

        private void Import(List<string> selectedFiles)
        {
            mainForm.WriteStatusInformation(ResourcesForUI.Status_ImportingSelectedFiles);
            List<string> importedFiles = model.ImportSelectedDownloads(selectedFiles);
            string message = GetImportMessage(importedFiles);
            mainForm.WriteStatusInformation(message);
            ShowImportMessageBox(message, importedFiles);
        }

        public void ImportNewestDownloads()
        {
            List<string> selectedFiles = model.SelectNewestDownloads(); // Select all files, downloaded today
            Import(selectedFiles);
        }

        public List<string> SelectFilesForImport(OpenFileDialog openFileDialog)
        {
            string functionName = "SelectFilesForImport";
            List<string> fileNameList = new List<string>();
            openFileDialog.Title = ResourcesForUI.SelectDownloadFileImportDialog_Title;
            openFileDialog.InitialDirectory = KnownFolders.GetPath(KnownFolder.Downloads, false); // defaultuser = false: Get the path to the current user.
            openFileDialog.FileName = ""; // No default
            openFileDialog.Filter = string.Format("{0}|*.xml;*.mxl", ResourcesForUI.OpenFileDialog_Filter); // Only present .xml files and .mxl files  
            openFileDialog.CheckFileExists = true;
            openFileDialog.CheckPathExists = true;
            openFileDialog.Multiselect = true; // Allow inporting several files at once
            openFileDialog.ShowDialog(); 


            // The dialog has focus on the textbox for entering the file name.
            // Press <shift> <tab> twice to focus on the first line in the selection listbox.

            if (0 == openFileDialog.FileNames.GetLength(0))
            {
                Logger.Log(String.Format("{0}.{1} No files selected by user", className, functionName));
                return fileNameList;
            }

            foreach (string fileName in openFileDialog.FileNames)
            {
                Logger.Log(String.Format("{0}.{1} User selected {2}", className, functionName, fileName));
                fileNameList.Add(fileName);
            }

            return fileNameList;
        }

        public void ImportDownloads(OpenFileDialog openFileDialog)
        {
            List<string> selectedFiles = SelectFilesForImport(openFileDialog); // Let user select files 
            Import(selectedFiles);
        }

        public void ImportNewSample()
        {
            int nDirs = 0;
            int nFiles = 0;
            // The samples are structured in a directory structure, so the code difffers from the code in the "Import Download" cases
            List<string> sampleFiles = model.ImportNewSampleFiles(ref nFiles, ref nDirs);
            mainForm.WriteStatusInformation(ResourcesForUI.Status_ImportingSampleFiles);
            string message = GetImportMessage(sampleFiles);
            mainForm.WriteStatusInformation(message);
            ShowImportMessageBox(message, sampleFiles);
        }

        public static ImportHandler Create(Model model, MainForm mainForm, string applicationName)
        {
            return new ImportHandler(model, mainForm, applicationName);
        }
    }
}
