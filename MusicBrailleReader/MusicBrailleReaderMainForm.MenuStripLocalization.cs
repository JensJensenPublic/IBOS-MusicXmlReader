using System.Windows.Forms;
using UiAccessibilityModel;

namespace MusicBrailleReader
{
    partial class MusicBrailleReaderMainForm
    {
        /// <summary>
        /// Handles all localization of the texts in the MusicBrailleReaderMainForm menustrip
        /// Originally based on ...\Solutions\Tactile MusicXmlReader\MusicXmlReader\MainForm.MenuStripLocalization.cs
        /// Now using the implementation in UiAccessibleModel.MenuItemHandler
        /// </summary>
        private void LocalizeMenuStrip()
        {
           // The MenuHandler is only needed in this method, so no need to make it a member variable.
            MenuItemHandler menuItemHandler = MenuItemHandler.Create("KONTROL", "ALT", "SKIFT"); // Use simple danish terms until we get localization files.
           // MenuItemHandler  menuItemHandler = MenuItemHandler.Create(); // Use the simple version, implicitly causing English localization of the MEnuItemHandler.

            // Children of MenuStrip
            menuItemHandler.GenerateAccessibleName(filesToolStripMenuItem, ResourcesForMusicBrailleReaderMainForm.ToolStripMenuItem_Files, Keys.None);
            menuItemHandler.GenerateAccessibleName(toolsToolStripMenuItem, ResourcesForMusicBrailleReaderMainForm.ToolStripMenuItem_Tools, Keys.None);

            // First sublevel
            // Children (and grandchildren) of  fileToolStripMenuItem
            // menuItemHandler.GenerateAccessibleName(this.openUsingNOTAProfileToolStripMenuItem, "&Åbn punktnode fil", Keys.None);
            menuItemHandler.GenerateAccessibleName(this.openUsingNOTAProfileToolStripMenuItem, ResourcesForMusicBrailleReaderMainForm.ToolStripMenuItem_Files_OpenUsingNOTA, Keys.Control | Keys.O); // In the Danish version the CTRL-O shortcut goes here
            menuItemHandler.GenerateAccessibleName(this.openUsingBrailleOrchProfileToolStripMenuItem, ResourcesForMusicBrailleReaderMainForm.ToolStripMenuItem_Files_OpenUsingBrailleOrch, Keys.None);
            menuItemHandler.GenerateAccessibleName(this.exporterSomMusicXmlToolStripMenuItem, ResourcesForMusicBrailleReaderMainForm.ToolStripMenuItem_Files_ExportAsMusicXml, Keys.None);
            menuItemHandler.GenerateAccessibleName(this.exporterSomTextToolStripMenuItem, ResourcesForMusicBrailleReaderMainForm.ToolStripMenuItem_Files_ExportAsText, Keys.None);

           //menuItemHandler.GenerateAccessibleName(usingAutoselectedProfileToolStripMenuItem, "Åbn med &automatisk valgt profil", Keys.None);

           // Second sublevel below "Tools" // No need to localize as only used in developer mode !
            menuItemHandler.GenerateAccessibleName(this.iBOSMusicXmlReaderToolStripMenuItem, "&IBOS Nodelæser", Keys.None);
            menuItemHandler.GenerateAccessibleName(this.museScoreToolStripMenuItem, "Start &MuseScore", Keys.None);
            menuItemHandler.GenerateAccessibleName(this.logfileLocationToolStripMenuItem, "Åbn &Logfil placering", Keys.None);
            menuItemHandler.GenerateAccessibleName(this.jAWSSettingsToolStripMenuItem, "Inspicér nuværende &JAWS indstillinger", Keys.None);
            menuItemHandler.GenerateAccessibleName(this.jAWSSettingsUpdateToolStripMenuItem,"Gendan standard JAWS indstillinger ",Keys.None);

        }


    }

}
