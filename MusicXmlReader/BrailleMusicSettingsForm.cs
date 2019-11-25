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

        public BrailleMusicSettingsForm(DeviceTypeEnum deviceTypeEnum)
        {
            this.deviceTypeEnum = deviceTypeEnum;
            InitializeComponent();

            // Fill in title and tabels
            BrailleMusicSettingsFormHandler brailleMusicSettingsFormHandler = BrailleMusicSettingsFormHandler.Create(deviceTypeEnum);
            this.Text = brailleMusicSettingsFormHandler.Title;
            this.labelDeviceName.Text = brailleMusicSettingsFormHandler.GetLabelDeviceName();

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
