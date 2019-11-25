using System;
using System.Windows.Forms;
using MusicXmlReaderModel;
using System.IO;
using System.Collections.Generic;

namespace MusicXmlReader
{
    public class EmbosserHandler
    {
        private OpenFileDialog openFileDialog;
        private PrintDialog printDialog;
        string applicationName = "";

        // As we can't have a list of all supported embossers, we can at least create a list of devices, that we do not expect to support:
        private List<string> notEmbossers;

        private void LogPrinterSettings(string text, System.Drawing.Printing.PrinterSettings printerSettings)
        {
            Logger.LogCF(string.Format(": {0} PrinterSettings='{1}'", text, printerSettings.ToString()));
        }


        private DialogResult ShowOpenFileDialog(string latestDirectory)
        {
            // The dialog has focus on the textbox for entering the file name.
            // Press <shift> <tab> twice to focus on the first line in the selection listbox.
            openFileDialog.Reset(); // Prevent survival of strange settings from latest usage of this reused OpenFileDialog
            openFileDialog.Title = ResourcesForUI.OpenFileDialog_Title;  // Just a neutral name "Open"
            openFileDialog.FileName = ""; // No default
            openFileDialog.Filter = string.Format("{0}|*.brf;*.brl", ResourcesForUI.OpenFileDialog_Filter); // Only present .brf files and .brl files
            openFileDialog.InitialDirectory = latestDirectory;
            openFileDialog.FileName = ""; // As we typically produce several Braille Music files at a time it makes no sense to select one of them.
            openFileDialog.CheckFileExists = true;
            openFileDialog.CheckPathExists = true;
            openFileDialog.Multiselect = false; // Do not allow selecting multiple files
            DialogResult result = openFileDialog.ShowDialog();
            Logger.LogCF(string.Format(": Result={0}", result.ToString()));
            return result;
        }

        private DialogResult ShowPrintDialog()
        {
            // Set up the PrintDialog for minimal functionality
            printDialog.AllowCurrentPage = false;
            printDialog.AllowPrintToFile = false;
            printDialog.AllowSelection = false;
            printDialog.AllowSomePages = false;
            LogPrinterSettings("Initial ", printDialog.PrinterSettings);
            printDialog.ShowHelp = false;
            printDialog.ShowNetwork = false;
            bool useEXDialog = printDialog.UseEXDialog;
            DialogResult result = printDialog.ShowDialog();
            Logger.LogCF(string.Format(": Result={0}", result.ToString()));
            return result;
        }



        private string AddEscapeSequence(string fullFileName, string escapeSequence)
        {
            byte[] bytesFromFile = File.ReadAllBytes(fullFileName); // Opens and closes the file
            int escapeSequencelength = escapeSequence.Length;
            int fileLength = bytesFromFile.Length;
            byte[] buffer = new byte[bytesFromFile.Length+1+ escapeSequencelength];       
            buffer[0] = 027;  // The ASCII escape character))
            for (int i = 0; i < escapeSequencelength; i++)
            {
                buffer[i + 1] = (byte) escapeSequence[i];
            }

            for (int i = 0; i < fileLength; i++)
            {
                buffer[escapeSequencelength + 1 + i ] = bytesFromFile[i];
            }

            string dir = Path.GetDirectoryName(fullFileName);
            string ext = Path.GetExtension(fullFileName);
            string temp = Path.Combine(dir, "temp");
            string result = Path.ChangeExtension(temp, ext); 
            File.WriteAllBytes(result, buffer);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="latestDirectory">The directory to use for the initial FileOpen Dialog</param>
        /// <param name="latestFile">The filename to use for the initial FileOpen Dialog</param>
        /// <returns></returns>
        public bool Emboss(string latestDirectory)
        {
            // Show the OPenFileDialog
            DialogResult openDialogResult = ShowOpenFileDialog(latestDirectory);
            if ((DialogResult.OK != openDialogResult) || string.IsNullOrEmpty(openFileDialog.FileName))
            {
                return false; // Let the user press ESC without warning him
            }
            
            string fullName = openFileDialog.FileName;
            if (!File.Exists(fullName))
            {
                Logger.LogCF(string.Format(": {0} Eksisterer ikke!", fullName));
                return false;
            }

            // Show the PrintDialog
            DialogResult printDialogResult = ShowPrintDialog();

            // Act on the result
            if (printDialogResult != DialogResult.OK)
            {
                Logger.LogCF(string.Format(": Operation cancelled by user: PrintDialogResult='{0}'", printDialogResult));
                return false;
            }
            
            System.Drawing.Printing.PrinterSettings selectedPrinterSettings = printDialog.PrinterSettings;
            LogPrinterSettings("Selected", printDialog.PrinterSettings); 

            //string printerName = "Index Basic-D V2";
            string printerName = selectedPrinterSettings.PrinterName; // Use the printer selected by the user.
            if (this.notEmbossers.Contains(printerName))
            {
                // This is for sure not an embosser ! Better warn the user!
                string warning = string.Format("'{0}' is not a supported embosser!",printerName);
                DialogResult dialogResult =  MessageBox.Show(warning, applicationName, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (!(DialogResult.OK == dialogResult))
                {
                    return false;
                }
            }

            // string printFileName = AddEscapeSequence(fullName, "DBT0"); // As suggested by Nils Huhta from Index Braille. Does not work yet !

            // Send to Embosser:
            bool result = false;
            string errorMessage = null;
            try
            {
                //result = RawPrinterHelper.RawPrinterHelper.SendFileToPrinter(printerName, fullName);
                result = RawPrinterHelper.RawPrinterHelper.SendFileToPrinter(printerName, fullName);
            }
            catch (Exception exeption)
            {
                Logger.LogCFE(exeption);
                errorMessage = exeption.Message;
            }

            if (!result)
            {
                // The operation failed. If an exeptionMessage is found we use it, otherwise we must build one by ourselves:
                if (string.IsNullOrEmpty(errorMessage))
                {
#warning TODO Localize
                    errorMessage = string.Format("Failed to emboss file '{0}' on embosser='{1}'", fullName, printerName);
                }
                Logger.LogCF(string.Format(": {0}", errorMessage));
                MessageBox.Show(errorMessage, applicationName,MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            else
            {
                string fileName = Path.GetFileName(fullName);
#warning TODo Localize
                string logMessage = string.Format("Successfully sent file '{0}' to embosser '{1}'",fileName , printerName);
                string uiMessage  = string.Format("Successfully sent file '{0}'\r\nto embosser '{1}'", fileName, printerName);
                Logger.LogCF(string.Format(": {0}", logMessage));
                MessageBox.Show(uiMessage, applicationName, MessageBoxButtons.OK,MessageBoxIcon.None);
            }

            return result;
        }


        private EmbosserHandler() { }

        private EmbosserHandler(OpenFileDialog openFileDialog, PrintDialog printDialog, string applicationName)
        {
            this.openFileDialog = openFileDialog;
            this.printDialog = printDialog;
            this.applicationName = applicationName;
            notEmbossers = new List<string> { "Fax", "Microsoft Print to PDF", "Microsoft XPS Document Writer", "OneNote", "Send To OneNote 2016"};
        }

        static public EmbosserHandler Create(OpenFileDialog openFileDialog, PrintDialog printDialog, string applicationName)
        {
            return new EmbosserHandler(openFileDialog, printDialog, applicationName);
        }
    }
}
