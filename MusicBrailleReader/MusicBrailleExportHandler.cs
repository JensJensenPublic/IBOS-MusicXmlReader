using MusicXmlReaderModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MusicBrailleReader
{
    internal class MusicBrailleExportHandler
    {
        private string GetFileNameForSaving(string suggestedFileName, string filterMask)
        {
#warning todo Refactor into UserSave()
            // Let the User select directory and filename, but suggest decent default values
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = Path.GetFileName(suggestedFileName);
            saveFileDialog.Filter = filterMask;
            DialogResult dialogResult = saveFileDialog.ShowDialog();
            if (DialogResult.OK != dialogResult) return null;
            return saveFileDialog.FileName;
        }

        private string GetFilterMask(BrailleFileHandler.FileEncoding encoding)
        {
            switch (encoding)
            {
                case BrailleFileHandler.FileEncoding.BRF_Unicode_utf8: return string.Format("{0}|*{1}", "UNICODE(Utf-8)", ".txt"); // No need to localize !
                case BrailleFileHandler.FileEncoding.BRF_Unicode_utf16: return string.Format("{0}|*{1}", "UNICODE(Utf-16)", ".txt"); // No need to localize !
                case BrailleFileHandler.FileEncoding.BRF_Unicode_utf32: return string.Format("{0}|*{1}", "UNICODE(Utf-32)", ".txt"); // No need to localize !
                case BrailleFileHandler.FileEncoding.BRF_ASCII: return string.Format("{0}|*{1}", "ASCII", ".brf"); // No need to localize !
                case BrailleFileHandler.FileEncoding.BRL_OctoBraille_1252: return string.Format("{0}|*{1}", "OctoBraille", ".brl"); // No need to localize !
                case BrailleFileHandler.FileEncoding.PEF: return string.Format("{0}|*{1}", "PEF", ".pef"); // No need to localize !
                default:
                    Logger.LogCF(string.Format(": Unsupported encoding {0}", encoding));
                    return string.Format("{0}|*{1}", "", ".*"); // No need to localize !
            }
        }


        public void OnExport(BrailleFileHandler.FileEncoding encoding, DecoderOptions.RegionalOptionsEnum regionalOptions, string fullFileName)
        {
            Logger.LogCF(string.Format("({0},{1},FullFileName={2})", encoding, regionalOptions, fullFileName));
            int nFiles;
            string errorMessage = decoderHandler.GetNumberOfSeparateScores(out nFiles);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                Logger.LogCF(string.Format(": {0}", errorMessage));
                MessageBox.Show(errorMessage, applicationName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string suggestedFileName = decoderHandler.GetSuggestedBrailleFileName(fullFileName, encoding);
            //string filterMask = string.Format("{0}|*{1}", "MusicXml", ".musicxml"); // No need to localize !
            string filterMask = this.GetFilterMask(encoding);
            string selectedFileName = GetFileNameForSaving(suggestedFileName, filterMask);

            // Always export the whole file to the format specified.
            errorMessage = decoderHandler.ExportToSingleScore(encoding, regionalOptions, selectedFileName);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                Logger.LogCF(string.Format(": {0}", errorMessage));
                MessageBox.Show(errorMessage, applicationName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                string message = string.Format("Export to {0} succeeded for \r\n{1}", encoding.ToString(), Path.GetFileName(selectedFileName)); // One line for filename alone
                MessageBox.Show(message, applicationName, MessageBoxButtons.OK, MessageBoxIcon.None);
            }

            if (nFiles <= 1) return; // All done!

            {
                // Suggest to export as separate scores
                string messageBoxText = String.Format("The Music Braille file contains multiple scores.\r\nDo you want to export it as separate scores as well?");
                DialogResult dialogResult = MessageBox.Show(messageBoxText, applicationName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                errorMessage = decoderHandler.ExportToSeparateScores(encoding, regionalOptions, selectedFileName);
                if (string.IsNullOrEmpty(errorMessage))
                {
#warning Todo Localize
                    MessageBox.Show(string.Format("{0} scores succesfully exported!", nFiles), applicationName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(errorMessage, applicationName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }


        static public MusicBrailleExportHandler Create(DecoderHandler decoderHandler, string applicationName)
        {
            return new MusicBrailleExportHandler(decoderHandler,applicationName);
        }

        DecoderHandler decoderHandler;
        string applicationName;


        private MusicBrailleExportHandler(DecoderHandler decoderHandler, string applicationName)
        {
            this.decoderHandler = decoderHandler;
            this.applicationName = applicationName;         
        }
    }
}
