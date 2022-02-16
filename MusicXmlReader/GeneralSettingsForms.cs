using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using MusicXmlReaderModel;

namespace MusicXmlReader
{
    public partial class GeneralSettingsForms : Form
    {
        private string applicationName;
        private UserPreferencesHandler userPreferences;



        /// <summary>
        /// Initializes a pair, consisting of a textbox and a label with a name and sets the readOnly attribute of the TextBox
        /// </summary>
        /// <param name="textBox"></param>
        /// <param name="label"></param>
        /// <param name="name"></param>
        /// <param name="readOnly"></param>
        private void InitTextBoxAndLabel(TextBox textBox, Label label, string name, string value, bool readOnly)
        {
            label.Text = name;
            textBox.AccessibleName = name;
            textBox.Visible = !string.IsNullOrEmpty(name);       
            textBox.ReadOnly = readOnly;
            textBox.Text = value;
        }

        private string  SafeGetFileName(string fullFileName)
        {
            // Do not use Path.GetFileName() on a file without verifying it first. An empty path will cast an exception.
            string result = "";
            if (string.IsNullOrEmpty(fullFileName)) return result;               
            try
            {
                result = Path.GetFileNameWithoutExtension(fullFileName);
            }
            catch (Exception e)
            {
                Logger.LogCFE(e);
            }
            return result;
}



public GeneralSettingsForms(string applicationName,  UserPreferencesHandler userPreferences)
        {
            InitializeComponent();
            this.applicationName = applicationName;
            this.userPreferences = userPreferences;
            DialogResult = DialogResult.Cancel;

            // Title
            this.Text = this.applicationName + " " + ResourcesForSettings.General_Caption;
            string fullFileName = userPreferences.MusicXmlFile;
            string fileName = SafeGetFileName(fullFileName);
 
            // Labels and textboxes for MusicXml
            InitTextBoxAndLabel(textBoxMusicXmlFile, labelMusicXmlFile, ResourcesForSettings.General_LatestMusicXmlFile,fileName , true);
            InitTextBoxAndLabel(textBoxMusicXmlDirectory, labelMusicXmlDirectory, ResourcesForSettings.General_LatestMusicXmlPath, fullFileName, true);
            // Label and textbox for Braille Music
            InitTextBoxAndLabel(textBoxBrailleMusicPath, labelBrailleMusicPath, ResourcesForSettings.General_LatestBrailleMusicDirectory, userPreferences.BrailleMusicDirectory, true);

            InitTextBoxAndLabel(textBoxMuseScore, labelMuseScore, ResourcesForSettings.General_MuseScoreLocation, userPreferences.MuseScoreExe, false);
            InitTextBoxAndLabel(textBoxSibelius, labelSibelius, ResourcesForSettings.General_SibeliusLocation, userPreferences.SibeliusExe, false);
            InitTextBoxAndLabel(textBoxCapella, labelCapella, ResourcesForSettings.General_CapellaLocation, userPreferences.CapellaExe, false);
            InitTextBoxAndLabel(textBoxFinale, labelFinale, ResourcesForSettings.General_FinaleLocation, userPreferences.FinaleExe, false);

            InitTextBoxAndLabel(textBoxPhotoScore, labelPhotoScore, ResourcesForSettings.General_PhotoScoreLocation, userPreferences.PhotoScoreExe, false);
            InitTextBoxAndLabel(textBoxBrailleMusicEditor2, labelBrailleMusicEditor2, ResourcesForSettings.General_BrailleMusicEditor2Location, userPreferences.BrailleMusicEditor2Exe, false);

        }


        /// <summary>
        /// Save the settings left by the user back to the User preferences 
        /// </summary>
        public void SaveSettings()
        {
            // Implement if needed.
            userPreferences.MuseScoreExe = textBoxMuseScore.Text;
            userPreferences.SibeliusExe = textBoxSibelius.Text;
            userPreferences.CapellaExe = textBoxCapella.Text;
            userPreferences.FinaleExe = textBoxFinale.Text;

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
