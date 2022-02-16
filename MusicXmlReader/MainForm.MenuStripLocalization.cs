using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXmlReader
{
    public partial class MainForm
    {

        /// <summary>
        /// Handles all localization of the texts in the MAinForm menustrip
        /// </summary>
        private void LocalizeMenuStrip()
        {
            // Children of MenuStrip
            filesToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Files;
            editToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Edit;
            viewToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_View;
            settingsToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Settings;
            toolsToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Tools;
            archivesToolStripMenuItem.Text = ResourcesForUI.ToolsStripMenuItem_Archives;
            helpToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Help;
#warning TODO Replace GenerateAccessibleName by UiAccessibilityModel.MenuItemHandler.GenerateAccessibleName and save a lot of lines !
            // Children (and grandchildren) of  fileToolStripMenuItem
            openMusicXmlFileToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Files_OpenMusicXmlFile;
            openMusicXmlFileToolStripMenuItem.ShortcutKeys = ShortcutHandler.openMusicXmlFile;
            openRecentMusicXmlFileToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Files_OpenMusicXmlFileRecent;
            openRecentMusicXmlFileToolStripMenuItem.ShortcutKeys = ShortcutHandler.openRecentMusicXmlFileToolStripMenuItem;
            GenerateAccessibleName(ref openMusicXmlFileToolStripMenuItem);
            openMusicXmlFileUsingDefaultSettingsToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Files_OpenMusicXmlFile_DefaultSettings;
            openMusicXmlFileUsingDefaultSettingsToolStripMenuItem.ShortcutKeys = ShortcutHandler.openMusicXmlFileUsingDefaultSettings;
            GenerateAccessibleName(ref openMusicXmlFileUsingDefaultSettingsToolStripMenuItem);
            importDownloadsToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Files_ImportDownloads;
            importNewestDownloadsToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Files_ImportNewestDownloads;
            importNewSampleFilesToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Files_ImportNewestSamples;
            exitToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Files_Exit;
            exitToolStripMenuItem.ShortcutKeys = ShortcutHandler.exitApplication;
            GenerateAccessibleName(ref exitToolStripMenuItem);
            // Export Music Braille:
            exportMusicBrailleToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Files_ExportMusicBrailleToFile; // First level
            // Print Music Braille
            // printMusicBrailleUsingIBPrintToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Files_PrintMusicBrailleUsingProgram;
            printMusicBrailleToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Files_PrintMusicBraille;
            viaWindowsPrintDialogToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Files_Print_via_Windows;
            usingExternalProgramToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Files_Print_using_External;
            // exportMusicBrailleToolStripMenuItem.ShortcutKeys = ShortcutHandler.exportMusicBraille;
            // GenerateAccessibleName(ref exportMusicBrailleToolStripMenuItem);

            // The next 4 lines could be replaced by a call to LocalizeToolMenuItems(null) See implementation below !
            toNotetakerToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Files_ToNoteTaker; //  Second level
            toEmbosserToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Files_ToEmbosser; //   Second level
            toHighSpeedEmbosserToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Files_ToHighSpeedEmbosser; //   Second level
            toGenericDeviceToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Files_ToGenericDevice; // Second level
            inOptionalFormatToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Files_InOptionalFormat; //  Second level

            txtOctoBraille1252ToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Files_TxtOctoBraille1252; // Third level
            brfASCIIToolStripMenuItem1.Text = ResourcesForUI.ToolStripMenuItem_Files_brf_ASCII; // Third level
            brfUnicodeToolStripMenuItem1.Text = ResourcesForUI.ToolStripMenuItem_Files_BrfUnicode; // Third level

            copyMusicBrailleToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Files_CopyMusicBraille;

            // Children of editToolStripMenuItem, referring to the Treeview
            // Texts:  NOTE! Use the same texts as used in the treeview to which these items refer!!
            allItemsToolStripMenuItem.Text = ResourcesForUI.TreeView_All_Items;
            filterItemsToolStripMenuItem.Text = ResourcesForUI.TreeWiew_Items;
            musicRepresentationToolStripMenuItem.Text = ResourcesForUI.TreeView_MusicAsSound;
            textRepresentationToolStripMenuItem.Text = ResourcesForUI.TreeView_MusicAsSpeech;
            brailleRepresentationToolStripMenuItem.Text = ResourcesForUI.TreeView_MusicAsBraille;
            partsToolStripMenuItem.Text = ResourcesForUI.TreeView_MusicAsSound_Parts;
            detailsToolStripMenuItem.Text = ResourcesForUI.TreeView_MusicAsSound_Details;
            uncheckAllToolStripMenuItem.Text = ResourcesForUI.TreeView_UncheckAll;
            checkAllToolStripMenuItem.Text = ResourcesForUI.TreeView_CheckAll;

            // Children of editToolStripMenuItem referring to the ParameterInputForm
            repeatToolStripMenuItem.Text = ResourcesForUI.ParameterInputForm_Repeat;
            goToToolStripMenuItem.Text = ResourcesForUI.ParameterInputForm_GoTo;
            ofNominalTempoToolStripMenuItem.Text = ResourcesForUI.ParameterInputForm_PctOfNominalTempo;


            // Direct Shortcuts 
            // allItemsToolStripMenuItem.ShortcutKeys = ShortcutHandler.editAllItems; // See comment in ShortcutHandler.cs
            filterItemsToolStripMenuItem.ShortcutKeys = ShortcutHandler.editFilter;             // Select the Filter top node
            GenerateAccessibleName(ref filterItemsToolStripMenuItem);
            musicRepresentationToolStripMenuItem.ShortcutKeys = ShortcutHandler.editMusic;      // Select the Music Filter top node
            GenerateAccessibleName(ref musicRepresentationToolStripMenuItem);
            textRepresentationToolStripMenuItem.ShortcutKeys = ShortcutHandler.editText;        // Select the Text filter top node
            GenerateAccessibleName(ref textRepresentationToolStripMenuItem);
            brailleRepresentationToolStripMenuItem.ShortcutKeys = ShortcutHandler.editBraille;  // Select the Braille Music filter top node
            GenerateAccessibleName(ref brailleRepresentationToolStripMenuItem);
            // partsToolStripMenuItem.ShortcutKeys = ShortcutHandler.editParts;  // See comment in ShortcutHandler.cs
            // detailsToolStripMenuItem.ShortcutKeys = ShortcutHandler.editDetails; // See comment in ShortcutHandler.cs
            uncheckAllToolStripMenuItem.ShortcutKeys = ShortcutHandler.uncheckAll;
            GenerateAccessibleName(ref uncheckAllToolStripMenuItem);
            checkAllToolStripMenuItem.ShortcutKeys = ShortcutHandler.checkAll;
            GenerateAccessibleName(ref checkAllToolStripMenuItem);


            // Direct shortcuts for the 3 commands opening the ParameterInputForm
            repeatToolStripMenuItem.ShortcutKeys = ShortcutHandler.CommandRepeat;
            GenerateAccessibleName(ref repeatToolStripMenuItem);
            goToToolStripMenuItem.ShortcutKeys = ShortcutHandler.CommandGoto;
            GenerateAccessibleName(ref goToToolStripMenuItem);
            ofNominalTempoToolStripMenuItem.ShortcutKeys = ShortcutHandler.CommandTempo;
            GenerateAccessibleName(ref ofNominalTempoToolStripMenuItem);


            //Children of viewToolStripMenuItem:
            instrumentsToolStripMenuItem.Text = ResourcesForUI.ToolsStripMenuItem_View_Instruments;
            instrumentsToolStripMenuItem.ShortcutKeys = ShortcutHandler.DetailsInstruments;
            GenerateAccessibleName(ref instrumentsToolStripMenuItem);
            this.brailleFileToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_View_MusicBrailleForNoteTaker;

            // Children of SettingsToolStripMenuItem

            generelSettingsToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Settings_General;
            embosserSettingsToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Settings_Embosser;
            highSpeedEmbosserSettingsToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Settings_HighspeedEmbosser;
            notetakerSettingsToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Settings_NoteTaker;
            musicBrailleSettingsToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Settings_MusicBraille;
            resetAllUserSettingsToDefaultValuesToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Settings_ResetAll;


            // Children of  toolsToolStripMenuItem
            musicBrailleReaderToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Tools_MusicBrailleReader;
            museScoreToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Tools_MuseScore;
            sibeliusToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Tools_Sibelius;
            startCapellaToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Tools_Capella;
            startFinaleToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Tools_Finale;
            startBrailleMusicEditor2ToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Tools_BrailleMusicEditor2;
            startPhotoScoreToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Tools_PhotoScore;
            logfileToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Tools_Logfile;
            openXMLFileLocationToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Tools_OpenXmlFileLocation;
            openLogFileLocationToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Tools_Logfile_Location;
            inspectAsXMLToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Tools_InspectAsXml;
            inspectAsXMLUsingGoogleChromeToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Tools_InspectAsXml_Using_Chrome;
            
            viewAsInterpretedXMLToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Tools_ViewAsInterpretedXml;
            jAWSSettingsToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Tools_JAWS_Settings;
            userSettingsToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Tools_User_Settings;
            generateMusicBrailleTestpatternToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Tools_GenerateMusicBrailleTestPattern;
      

            // Children of  helpToolStripMenuItem
            this.aboutIBOSMusicXmlReaderToolStripMenuItem.Text = string.Format("{0} {1}", ResourcesForUI.ToolStripMenuItem_Help_About, applicationName);
            this.keyboardShortcutsToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Help_Shortcuts;
            this.linkToNewestSoftwareToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Help_SoftwareUpdate;
            this.usersManualToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Help_ShowUsersManual;
        }

        // Special implementation for the itemStrips referencing BrailleDevices, allowing the ItemStrip to show the current settings
        // When called with (uph == null) it implements normal localozation.
        private void LocalizeToolsMenuItems(UserPreferencesHandler uph)
        {
            //MusicXmlReaderModel.Logger.LogCF(": Entry");
            toNotetakerToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Files_ToNoteTaker + ((uph == null) ? "" : SpaceOnException(uph.noteTaker.MenuItemString));
            toEmbosserToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Files_ToEmbosser + ((uph == null) ? "" : SpaceOnException(uph.embosser.MenuItemString));
            toHighSpeedEmbosserToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Files_ToHighSpeedEmbosser + ((uph == null) ? "" : SpaceOnException(uph.highSpeedEmbosser.MenuItemString));
            toGenericDeviceToolStripMenuItem.Text = ResourcesForUI.ToolStripMenuItem_Files_ToGenericDevice + ((uph == null) ? "" : SpaceOnException(uph.genericBrailleDevice.MenuItemString));
            //MusicXmlReaderModel.Logger.LogCF(": Exit");
        }

        private string SpaceOnException(string s)
        {
            try
            {
                // throw new Exception("For test only");
                return s;
            }
            catch (Exception e)
            {
                MusicXmlReaderModel.Logger.LogCFE(e);
            }
            return " ";
        }

    }
}
