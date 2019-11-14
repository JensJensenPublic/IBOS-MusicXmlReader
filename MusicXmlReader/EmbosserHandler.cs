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


        public bool Emboss(string latestDirectory)
        {
            openFileDialog.FileName = ""; // No default
            openFileDialog.Filter = string.Format("{0}|*.brf;*.brl", ResourcesForUI.OpenFileDialog_Filter); // Only present .brf files and .mxl files
            openFileDialog.InitialDirectory = latestDirectory;
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
            System.Drawing.Printing.PrinterSettings printerSettings = printDialog.PrinterSettings;
            Logger.LogCF(string.Format(": PrinterSettings={0}", printerSettings));
            printDialog.ShowHelp = false;
            printDialog.ShowNetwork = false;
            bool useEXDialog = printDialog.UseEXDialog;

            // Show the PrintDialog
            DialogResult printDialogResult = printDialog.ShowDialog();
            Logger.LogCF(string.Format(": PrintDialogResult={0}", printDialogResult.ToString()));

            // Act on the result
            if (printDialogResult == DialogResult.OK)
            {
                Logger.LogCF(string.Format(": PrinterName={0}", printerSettings.PrinterName));
            }
            else
            {
                return false;
            }
             


            //string printerName = "Index Basic-D V2";
            string printerName = printerSettings.PrinterName; // Use the printer selected by the user.
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
            try
            {
                result = RawPrinterHelper.RawPrinterHelper.SendFileToPrinter(printerName, fullName);
            }
            catch (Exception exeption)
            {
                Logger.LogCFE(exeption);
            }

            if (!result)
            {
                string message = string.Format("Failed to emboss file {0} on {1}", fullName, printerName);
                MessageBox.Show(message, applicationName);
            }

            return true;
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
