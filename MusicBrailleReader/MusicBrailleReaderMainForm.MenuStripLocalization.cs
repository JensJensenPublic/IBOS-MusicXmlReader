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
            menuItemHandler.GenerateAccessibleName(filesToolStripMenuItem, "&Filer", Keys.None);
            menuItemHandler.GenerateAccessibleName(toolsToolStripMenuItem, "Værk&tøjer",Keys.None);

            // First sublevel
            // Children (and grandchildren) of  fileToolStripMenuItem
            menuItemHandler.GenerateAccessibleName(this.openUsingNOTAProfileToolStripMenuItem, "&Åbn MusicBraille fil", Keys.None);
            menuItemHandler.GenerateAccessibleName(this.openUsingNOTAProfileToolStripMenuItem, "Åbn med &NOTA profil", Keys.Control | Keys.O); // In the Danish version the CTRL-O shortcut goes here
            menuItemHandler.GenerateAccessibleName(this.openUsingBrailleOrchProfileToolStripMenuItem, "Åbn med &BrailleOrch profil", Keys.None);

            //menuItemHandler.GenerateAccessibleName(usingAutoselectedProfileToolStripMenuItem, "Åbn med &automatisk valgt profil", Keys.None);

            // Second sublevel below "Tools"
            menuItemHandler.GenerateAccessibleName(tactileMusicXmlReaderToolStripMenuItem, "&Tactile MusicXmlReader", Keys.None);
            menuItemHandler.GenerateAccessibleName(iBOSMusicXmlReaderToolStripMenuItem, "&IBOS Nodelæser", Keys.None);
            menuItemHandler.GenerateAccessibleName(museScoreToolStripMenuItem, "&MuseScore", Keys.None);
            menuItemHandler.GenerateAccessibleName(logfileLocationToolStripMenuItem, "&Logfil placering", Keys.None);

        }


    }

}
