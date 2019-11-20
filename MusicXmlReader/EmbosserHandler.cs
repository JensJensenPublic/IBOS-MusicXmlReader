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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="latestDirectory">The directory to use for the initial FileOpen Dialog</param>
        /// <param name="latestFile">The filename to use for the initial FileOpen Dialog</param>
        /// <returns></returns>
        public bool Emboss(string latestDirectory)
        {
            openFileDialog.FileName = ""; // No default
            openFileDialog.Filter = string.Format("{0}|*.brf;*.brl", ResourcesForUI.OpenFileDialog_Filter); // Only present .brf files and .brl files
            openFileDialog.InitialDirectory = latestDirectory;
            openFileDialog.FileName = ""; // As we typically produce several Braille Music files at a time it makes no sense to select one of them.
            openFileDialog.CheckFileExists = true;
            openFileDialog.CheckPathExists = true;
            openFileDialog.Multiselect = false; // Do not allow selecting multiple files

            openFileDialog.Title = "Open file for embossing";
            DialogResult openDialogResult = openFileDialog.ShowDialog();
            Logger.LogCF(string.Format(": OpenDialogResult={0}", openDialogResult.ToString()));

            // The dialog has focus on the textbox for entering the file name.
            // Press <shift> <tab> twice to focus on the first line in the selection listbox.

            if (string.IsNullOrEmpty(openFileDialog.FileName))
            {
                return false; // Let the user press ESC without warning him
            }

            //string fileName = "Danmark nu blunder den lyse nat.P1.Soprano.brf";
            //string fullName = Path.Combine(model.LatestBrailleFileSaveDirectory, fileName);

            string fullName = openFileDialog.FileName;
            if (!File.Exists(fullName))
            {
                Logger.LogCF(string.Format(": {0} Eksisterer ikke!", fullName));
                return false;
            }

            // Set up the PrintDialog for minimal functionality
            printDialog.AllowCurrentPage = false;
            printDialog.AllowPrintToFile = false;
            printDialog.AllowSelection = false;
            printDialog.AllowSomePages = false;
            LogPrinterSettings("Initial ", printDialog.PrinterSettings);

            printDialog.ShowHelp = false;
            printDialog.ShowNetwork = false;
            bool useEXDialog = printDialog.UseEXDialog;

            // Show the PrintDialog
            DialogResult printDialogResult = printDialog.ShowDialog();
            Logger.LogCF(string.Format(": PrintDialogResult='{0}'", printDialogResult.ToString()));

            // Act on the result
            if (printDialogResult != DialogResult.OK)
            {
                Logger.LogCF(string.Format(": Operation cancelled by user: PrintDialogResult='{0}'", printDialogResult));
                return false;
            }
            
            System.Drawing.Printing.PrinterSettings selectedPrinterSettings = printDialog.PrinterSettings;
            LogPrinterSettings("Selected", selectedPrinterSettings); 

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

            bool result = false;
            string errorMessage = null;
            try
            {
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
