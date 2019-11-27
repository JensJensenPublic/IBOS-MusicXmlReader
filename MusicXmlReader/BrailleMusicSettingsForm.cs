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
            Init(labelDeviceName, textBoxDeviceName, settings.LabelDeviceName);
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
            // Nopte thate the BrailleDevice may represent an embosser, a notetaker or a completely general device.
            this.numericUpDownWidth.Value = brailleDevicePreferences.PageWidth;
            this.numericUpDownHeight.Value = brailleDevicePreferences.PageHeight;
            this.textBoxEscapeSequence.Text = brailleDevicePreferences.EscapeSequence;
            this.textBoxApplicationName.Text = brailleDevicePreferences.ApplicationName;
            this.textBoxApplicationExe.Text = brailleDevicePreferences.ApplicationLocation;
            this.textBoxDeviceName.Text = brailleDevicePreferences.DeviceName;

            List<BrailleFileHandler.FileEncoding> encodingsShown = new List<BrailleFileHandler.FileEncoding>() { BrailleFileHandler.FileEncoding.BRF_ASCII, BrailleFileHandler.FileEncoding.BRL_OctoBraille_1252, BrailleFileHandler.FileEncoding.BRF_Unicode_utf8 };
            //EncodingItem selectedEncodingItem = null;
            foreach (BrailleFileHandler.FileEncoding encoding in encodingsShown)
            {
                // Load the listbox and select the item pointed out by brailleDevicePreferences.BrailleFileFormat                
                EncodingItem encodingItem = new EncodingItem(encoding);                
                this.listBoxFileFormat.Items.Add(encodingItem);
                if (encodingItem.Encoding == brailleDevicePreferences.BrailleFileFormat)
                {
                    this.listBoxFileFormat.SelectedItem = encodingItem;
                }
            }  
            this.Refresh();
        }

        /// <summary>
        /// Save the settings left by the user back to the User preferences 
        /// </summary>
        public void SaveSettings()
        {
            brailleDevicePreferences.PageWidth = (int)this.numericUpDownWidth.Value;
            brailleDevicePreferences.PageHeight = (int)this.numericUpDownHeight.Value;
            brailleDevicePreferences.EscapeSequence = this.textBoxEscapeSequence.Text;
            brailleDevicePreferences.ApplicationName = this.textBoxApplicationName.Text;
            brailleDevicePreferences.ApplicationLocation = this.textBoxApplicationExe.Text;
            if ((null != this.listBoxFileFormat.SelectedItem) && (this.listBoxFileFormat.SelectedItem is EncodingItem))
            {
                brailleDevicePreferences.BrailleFileFormat = (this.listBoxFileFormat.SelectedItem as EncodingItem).Encoding;
            }
            else
            {
                // Maybe this is not the right place to declare a default value ??
                brailleDevicePreferences.BrailleFileFormat = BrailleFileHandler.FileEncoding.BRF_ASCII;
            }
            // brailleDevicePreferences.BrailleFileFormat = this. 
           
            // More to follow
        }



        private void textBoxApplicationName_TextChanged(object sender, EventArgs e)
        {

        }

        private void labelApplicationLocation_Click(object sender, EventArgs e)
        {

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

        private void BrailleMusicSettingsForm_FormClosed(object sender, FormClosedEventArgs e)
        {

        }
    }


    /// <summary>
    /// Simple class for defining UI representations for the various Braille file formats. The values may be localized if required.
    /// </summary>
    internal class EncodingItem
    {
        private BrailleFileHandler.FileEncoding encoding;
        public BrailleFileHandler.FileEncoding Encoding { get { return encoding; } }
        public override string ToString()
        {
            switch (encoding)
            {
                case BrailleFileHandler.FileEncoding.BRF_ASCII: return "ASCII";
                case BrailleFileHandler.FileEncoding.BRF_Unicode: return "Unicode";
                case BrailleFileHandler.FileEncoding.BRF_Unicode_utf16: return "Unicode(utf16)";
                case BrailleFileHandler.FileEncoding.BRF_Unicode_utf32: return "Unicode(utf32)";
                case BrailleFileHandler.FileEncoding.BRF_Unicode_utf8: return "Unicode(rtf8)";
                case BrailleFileHandler.FileEncoding.BRL_OctoBraille_1252: return "OctoBraille 1252";
                case BrailleFileHandler.FileEncoding.PEF: return "PEF";
                case BrailleFileHandler.FileEncoding.Unknown: return "";
                default: Logger.LogCF(string.Format(": Unsupported fileencoding '{0}'", encoding.ToString())); return "";
            }
        }

        public EncodingItem(BrailleFileHandler.FileEncoding encoding)
        {
            this.encoding = encoding;
        }
    }



}
