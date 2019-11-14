using System;
using System.Windows.Forms;
using MusicXmlReaderModel;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXmlReader
{
    public class EmbosserHandler
    {
        private OpenFileDialog openFileDialog;
        private PrintDialog printDialog;


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

            // return false;

            //string printerName = "Index Basic-D V2";
            string printerName = printerSettings.PrinterName; // Use the printer selected by the user.
            try
            {
                RawPrinterHelper.RawPrinterHelper.SendFileToPrinter(printerName, fullName);
            }
            catch (Exception exeption)
            {
                Logger.LogCFE(exeption);

            }


            return true;
        }




        private EmbosserHandler() { }

        private EmbosserHandler(OpenFileDialog openFileDialog, PrintDialog printDialog)
        {
            this.openFileDialog = openFileDialog;
            this.printDialog = printDialog;
        }

        static public EmbosserHandler Create(OpenFileDialog openFileDialog, PrintDialog printDialog)
        {
            return new EmbosserHandler(openFileDialog, printDialog);
        }
    }
}
