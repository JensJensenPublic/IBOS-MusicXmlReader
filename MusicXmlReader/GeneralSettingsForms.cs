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
            InitTextBoxAndLabel(textBoxMusicXmlFile, labelMusicXmlFile, "MusicXml file", userPreferences.MusicXmlFile, true);
            // Label and textbox for Braille Music
            InitTextBoxAndLabel(textBoxBrailleMusicPath, labelBrailleMusicPath, "Braille Music directory", userPreferences.BrailleMusicDirectory, true);
          
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
