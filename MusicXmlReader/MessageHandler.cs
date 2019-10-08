using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MusicXmlReaderModel;

namespace MusicXmlReader
{
    public class MessageHandler
    {
        private string applicationName;

        private MessageHandler(string applicationName)
        {
            this.applicationName = applicationName;
        }

        // Decide how to show error messages and warnings          
        public void ShowMessage(int messageId, string parameter, string text)
        {
            MessageBox.Show(text); // To implement localization: Do as in ShowWarning !!
        }

        // Decide how to show a simple message directly from the UI       
        public void ShowMessage(string text)
        {
            if (null == text) return;
            MessageBox.Show( text,applicationName);
        }

        private string LocalizeMessage(ModelMessageEnum messageEnum)
        {
            switch (messageEnum)
            {
                case ModelMessageEnum.DirectoryNotFound: return ResourcesForUI.Message_DirectoryNotFound;
                case ModelMessageEnum.FailedToConnectToScreenReader: return ResourcesForUI.Message_JAWSScreenReaderIsNotRunning;
                case ModelMessageEnum.ConnectedToNonDefaultScreenReader: return ResourcesForUI.Message_ConnectedToNonDefaultScreenReader;
                case ModelMessageEnum.FailedToStartProgram: return ResourcesForUI.Message_FailedToStartProgram;
                case ModelMessageEnum.FileNotFound: return ResourcesForUI.Message_FileNotFound;
                case ModelMessageEnum.MissingProgramFile: return ResourcesForUI.Message_MissingProgramFile;
                case ModelMessageEnum.FailedToReadMusicXmlFile: return ResourcesForUI.Message_FailedToReadMusicXmlFile;
                case ModelMessageEnum.UnspecifiedMusicXmlFile: return ResourcesForUI.Message_UnspecifiedMusicXmlFile;
                case ModelMessageEnum.NotAllowedWhilePlaying: return ResourcesForUI.Message_NotAllowedWhilePlaying;
                case ModelMessageEnum.LocationNotDetermined: return ResourcesForUI.Message_LocationNotDetermined;
                case ModelMessageEnum.UnspecifiedInitializationError: return ResourcesForUI.Message_UnspecifiedInitializationError;
                case ModelMessageEnum.ToManyPartForExportToMusicBraille: return ResourcesForUI.Message_ExportOfMultiplePartsNotSupported;
                default: return string.Format("{0} {1}", ResourcesForUI.Message_UndefinedMessage, messageEnum.ToString());
            }
        }

        private string LocalizeExtraMessage(ModelMessageEnum messageEnum)
        {
            switch (messageEnum)
            {
                case ModelMessageEnum.DirectoryNotFound: return "";
                case ModelMessageEnum.FailedToConnectToScreenReader: return ResourcesForUI.Message_PleaseSeeLogFile;
                case ModelMessageEnum.ConnectedToNonDefaultScreenReader: return ResourcesForUI.Message_MayNotWorkAsExpected;
                case ModelMessageEnum.FailedToStartProgram: return ResourcesForUI.Message_PleaseSeeLogFile;
                case ModelMessageEnum.FileNotFound: return "";
                case ModelMessageEnum.MissingProgramFile: return ResourcesForUI.Message_PleaseSeeLogFile;
                case ModelMessageEnum.FailedToReadMusicXmlFile: return ResourcesForUI.Message_PleaseSeeLogFile;
                case ModelMessageEnum.NotAllowedWhilePlaying: return ResourcesForUI.Message_StopPlayingFirst;
                case ModelMessageEnum.UnspecifiedInitializationError: return ResourcesForUI.Message_PleaseSeeLogFile;
                case ModelMessageEnum.ToManyPartForExportToMusicBraille: return ResourcesForUI.Message_ConsiderExportingOnePartAtATime;
                default: return "";
            }
        }

        /// <summary>
        /// Build and show a message consisting of
        /// Line 1: A Message followed by possible parameters. Example: "File not found Stardust.xml"
        /// Line 2: (optional) an extra message. Example: "Plaese see Log File.."
        /// </summary>
        /// <param name="messageId"></param>
        /// <param name="text"></param>
        public void ShowWarning(int messageId, string parameter, string text)
        {
            //string parameter = "";
            string localizedMessage = LocalizeMessage((ModelMessageEnum)messageId);
            string localizedExtraMessage = LocalizeExtraMessage((ModelMessageEnum)messageId);
            string formattedMessage = string.Format("{0} {1} {2}",
                localizedMessage,                                                                   // The message
                string.IsNullOrEmpty(parameter) ? "" : "'" + parameter + "'",                           // Possible parameter
                string.IsNullOrEmpty(localizedExtraMessage) ? "" : "\r\n" + localizedExtraMessage);    // Possible extra message             
            MessageBox.Show(formattedMessage, applicationName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }


        public static MessageHandler Create(string applicationName)
        {
            return new MessageHandler(applicationName);
        }

    }
}
