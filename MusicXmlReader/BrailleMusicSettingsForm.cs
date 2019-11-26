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

        public BrailleMusicSettingsForm(DeviceTypeEnum deviceTypeEnum, string applicationName)
        {
            this.deviceTypeEnum = deviceTypeEnum;
            this.applicationName = applicationName;
            InitializeComponent();


            // Fill in title and tabels
            BrailleMusicSettingsFormHandler brailleMusicSettingsFormHandler = BrailleMusicSettingsFormHandler.Create(deviceTypeEnum);
            this.Text = this.applicationName + " " + brailleMusicSettingsFormHandler.Title;
            this.labelDeviceName.Text = brailleMusicSettingsFormHandler.LabelDeviceName;
            this.labelBrailleFileFormat.Text = brailleMusicSettingsFormHandler.LabelBrailleFileFormat;
            this.labelWidth.Text = brailleMusicSettingsFormHandler.LabelWidth;
            this.labelHeight.Text = brailleMusicSettingsFormHandler.LabelHeight;
            this.labelEscapeSequence.Text = brailleMusicSettingsFormHandler.LabelEscapeSequence;
            this.labelApplicationName.Text = brailleMusicSettingsFormHandler.LabelApplicationName;
            this.labelApplicationLocation.Text = brailleMusicSettingsFormHandler.LabelApplicationLocation;        

            this.Refresh();
            

            // Fill in values

            //UserPreferencesHandler userPreferencesHandler = UserPreferencesHandler.Create();
            //BrailleDevice brailleDevice = userPreferencesHandler.embosser;
            //if ((string.IsNullOrEmpty(brailleDevice.Name))) brailleDevice.Name = "Default Punktprinternavn"; 
            //Logger.LogCF(string.Format(": Name= {0}", brailleDevice.Name));


        }

        private void textBoxApplicationName_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
