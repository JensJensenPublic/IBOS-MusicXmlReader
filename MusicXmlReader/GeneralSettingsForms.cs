using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MusicXmlReader
{
    public partial class GeneralSettingsForms : Form
    {
        private string applicationName;
        private UserPreferencesHandler userPreferences;
  


        public GeneralSettingsForms(string applicationName,  UserPreferencesHandler userPreferences)
        {
            InitializeComponent();
            this.applicationName = applicationName;
            this.userPreferences = userPreferences;
            DialogResult = DialogResult.Cancel;
#warning TODO Localize
            // Title
            this.Text = this.applicationName + " " + "General settings";

            // Label and textbox for MusicXml
            this.labelMusicXmlFile.Text     = "MusicXml file";
            this.textBoxMusicXmlFile.ReadOnly = true;
            this.labelBrailleMusicPath.AccessibleName = this.labelMusicXmlFile.Text;
            this.textBoxMusicXmlFile.Text = userPreferences.MusicXmlFile;

            // Label and textbox for Braille Music
            this.labelBrailleMusicPath.Text = "Braille Music directory";
            this.textBoxBrailleMusicPath.ReadOnly = true;
            this.labelBrailleMusicPath.AccessibleName = this.labelBrailleMusicPath.Text;
            this.textBoxBrailleMusicPath.Text = userPreferences.BrailleMusicDirectory;
          
        }


        /// <summary>
        /// Save the settings left by the user back to the User preferences 
        /// </summary>
        public void SaveSettings()
        {
            // Implement if needed.
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
