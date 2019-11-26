using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MusicXmlReaderModel;

namespace MusicXmlReader
{
 

    public partial class BrailleMusicSettingsForm : Form
    {
        public enum DeviceTypeEnum { Unknown, Embosser, NoteTaker, GeneralDevice}
        private DeviceTypeEnum deviceTypeEnum;
        private string applicationName;
        private UserPreferencesHandler userPreferencesHandler;
        BrailleDevice brailleDevicePreferences = null;

        // If the text of a label is null or empty we mnake the label and its control invisible.
        private void Init(Label label, Control control, string labelName)
        {
            bool visible = !string.IsNullOrEmpty(labelName);
            label.Visible = visible;
            label.Text = labelName;
            control.Visible = visible;
        }


        public BrailleMusicSettingsForm(DeviceTypeEnum deviceTypeEnum, string applicationName, UserPreferencesHandler userPreferences)
        {
            this.deviceTypeEnum = deviceTypeEnum;
            this.applicationName = applicationName;
            this.userPreferencesHandler = userPreferences;
            InitializeComponent();


            // Fill in title and tabels
            BrailleMusicSettingsFormHandler settings = BrailleMusicSettingsFormHandler.Create(deviceTypeEnum);
            this.Text = this.applicationName + " " + settings.Title;

            // Initialize texts for labels and visibility for labels and other controls.
            Init(labelDeviceName, comboBoxDeviceName, settings.LabelDeviceName);
            Init(labelBrailleFileFormat, listBoxFileFormat, settings.LabelBrailleFileFormat);
            Init(labelWidth, numericUpDownWidth, settings.LabelWidth);
            Init(labelHeight, numericUpDownHeight, settings.LabelHeight);
            Init(labelEscapeSequence, textBoxEscapeSequence, settings.LabelEscapeSequence);
            Init(labelApplicationName, textBoxApplicationName, settings.LabelApplicationName);
            Init(labelApplicationLocation, textBoxApplicationExe, settings.LabelApplicationLocation);

            // Initialize values of contols

      

            switch (deviceTypeEnum)
            {
                case DeviceTypeEnum.Embosser: brailleDevicePreferences = userPreferencesHandler.embosser; break;
                case DeviceTypeEnum.NoteTaker: brailleDevicePreferences = userPreferencesHandler.noteTaker; break;
                //               case DeviceTypeEnum.GeneralDevice: brailleDevicePreferences = userPreferencesHandler.; break; // Generel device not implemented yet
                default:
                    Logger.LogCF(string.Format(": Device type not implemented: {0}", deviceTypeEnum.ToString()));
                    break;
            }

            // Get the values from UserPreferences.
            this.numericUpDownWidth.Value = brailleDevicePreferences.PageWidth;
            this.numericUpDownHeight.Value = brailleDevicePreferences.PageHeight;
            this.textBoxEscapeSequence.Text = brailleDevicePreferences.EscapeSequence;           

            //this.labelDeviceName.Text = settings.LabelDeviceName;
            //this.labelBrailleFileFormat.Text = settings.LabelBrailleFileFormat;
            //this.labelWidth.Text = settings.LabelWidth;
            //this.labelHeight.Text = settings.LabelHeight;
            //this.labelEscapeSequence.Text = settings.LabelEscapeSequence;
            //this.labelApplicationName.Text = settings.LabelApplicationName;
            //this.labelApplicationLocation.Text = settings.LabelApplicationLocation;

            this.Refresh();
            

            // Fill in values

            //UserPreferencesHandler userPreferencesHandler = UserPreferencesHandler.Create();
            //BrailleDevice brailleDevice = userPreferencesHandler.embosser;
            //if ((string.IsNullOrEmpty(brailleDevice.Name))) brailleDevice.Name = "Default Punktprinternavn"; 
            //Logger.LogCF(string.Format(": Name= {0}", brailleDevice.Name));


        }

        /// <summary>
        /// Save the settings left by the user back to the User preferences 
        /// </summary>
        public void SaveSettings()
        {
            brailleDevicePreferences.PageWidth = (int)this.numericUpDownWidth.Value;
            brailleDevicePreferences.PageHeight = (int)this.numericUpDownHeight.Value;
            brailleDevicePreferences.EscapeSequence = this.textBoxEscapeSequence.Text;
             // More to follow
        }



        private void textBoxApplicationName_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
