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
        BrailleMusicSettingsFormHandler settingsHandler;

        // If the text of a label is null or empty we mnake the label and its control invisible.
        private void Init(Label label, Control control, string labelName)
        {
            bool visible = !string.IsNullOrEmpty(labelName);
            label.Visible = visible;
            label.Text = labelName;
            control.Visible = visible;
            control.AccessibleName = labelName; // Allows JAWS to speak the same text as the names of the labels!
        }


        /// <summary>
        /// Some parts of the UI must be initialized in the constructor, but also later, if changes to other values occur.
        /// These parts are initialized in a separate method:
        /// </summary>
        private void InitDynamic()
        {
            Init(labelWidth, numericUpDownWidth, SizeIsVisible ?  settingsHandler.LabelWidth : "");
            Init(labelHeight, numericUpDownHeight, SizeIsVisible ? settingsHandler.LabelHeight: "");
        }

        // The size ( pageWidth and pageHeight ) is visible unless for a notetaker device with IBOS layout selected. (For the reason of backwards compatibility)
        private bool SizeIsVisible { get { return !((DeviceTypeEnum.NoteTaker == this.deviceTypeEnum) && (Model.BrailleStyleEnum.IBOS == this.userPreferencesHandler.noteTaker.BraillePageLayout)); } }

        public BrailleMusicSettingsForm(DeviceTypeEnum deviceTypeEnum, string applicationName, UserPreferencesHandler userPreferences)
        {
            this.deviceTypeEnum = deviceTypeEnum;
            this.applicationName = applicationName;
            this.userPreferencesHandler = userPreferences;
            InitializeComponent();
 

            // Fill in title and tabels
            this.settingsHandler = BrailleMusicSettingsFormHandler.Create(deviceTypeEnum);
            this.Text = this.applicationName + " " + settingsHandler.Title;

            // Initialize texts for labels and visibility for labels and other controls.
            Init(labelDeviceName, textBoxDeviceName, settingsHandler.LabelDeviceName);
            Init(labelBrailleFileFormat, listBoxFileFormat, settingsHandler.LabelBrailleFileFormat);
            Init(labelBraillePageLayout, listBoxBraillePageLayout, settingsHandler.LabelBraillePageLayout);
            Init(labelEscapeSequence, textBoxEscapeSequence, settingsHandler.LabelEscapeSequence);
            Init(labelApplicationName, textBoxApplicationName, settingsHandler.LabelApplicationName);
            Init(labelApplicationLocation, textBoxApplicationExe, settingsHandler.LabelApplicationLocation);

            InitDynamic(); // Initialize parts of the UI that may need reinitialization          

            // Initialize values of contols      

            switch (deviceTypeEnum)
            {
                case DeviceTypeEnum.Embosser: brailleDevicePreferences = userPreferencesHandler.embosser; break;
                case DeviceTypeEnum.NoteTaker: brailleDevicePreferences = userPreferencesHandler.noteTaker; break;
                case DeviceTypeEnum.GeneralDevice: brailleDevicePreferences = userPreferencesHandler.genericBrailleDevice; break; 
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

            // Fill in listBoxFileFormats
            foreach (BrailleFileHandler.FileEncoding encoding in settingsHandler.EnabledEncodings)
            {
                // Load the listbox and select the item pointed out by brailleDevicePreferences.BrailleFileFormat                
                EncodingItem encodingItem = new EncodingItem(encoding);                
                this.listBoxFileFormat.Items.Add(encodingItem);
                if (encodingItem.Encoding == brailleDevicePreferences.BrailleFileFormat)
                {
                    this.listBoxFileFormat.SelectedItem = encodingItem;
                }
            }

            // Fill in listBoxBraillePageLayouts
            foreach (Model.BrailleStyleEnum brailleStyleEnum in settingsHandler.BraillePageLayouts)
            {
                // Load the listbox and select the item pointed out by brailleDevicePreferences.BrailleFileFormat                
                PageLayoutItem pageLayoutItem =  new PageLayoutItem(brailleStyleEnum);
                this.listBoxBraillePageLayout.Items.Add(pageLayoutItem);
                if (pageLayoutItem.PageLayout == brailleDevicePreferences.BraillePageLayout)
                {
                    this.listBoxBraillePageLayout.SelectedItem = pageLayoutItem;
                }
            }

            listBoxBraillePageLayout.SelectedIndexChanged += ListBoxBraillePageLayout_SelectedIndexChanged;

            this.Refresh();
        }


        /// <summary>
        /// When the PageLayout for a NoteTaker changes we may need to update the visibility of the PageWidth and PAgeHEight controls.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBoxBraillePageLayout_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.deviceTypeEnum != DeviceTypeEnum.NoteTaker)
            {
                Logger.LogCF(string.Format(": DeviceType = {0} No action taken.", this.deviceTypeEnum.ToString()));
                return;
            }
            // This is a Notetaker profile so we need to reinitialize the the dynamic controls (pageWitth and pageHeight)
            bool done = false;
            try
            {
                int newIndex = listBoxBraillePageLayout.SelectedIndex;
                object o = listBoxBraillePageLayout.Items[newIndex];
                if (o is PageLayoutItem)
                {
                    this.userPreferencesHandler.noteTaker.BraillePageLayout = (o as PageLayoutItem).PageLayout;
                    Logger.LogCF(string.Format(": Notetaker.BraillePageLayout was changed to {0}", this.userPreferencesHandler.noteTaker.BraillePageLayout));
                    this.InitDynamic();
                    this.Refresh();
                    done = true;
                }
            }
            catch (Exception ex)
            {
                Logger.LogCFE(ex);
            }
            if (!done) Logger.LogCF(string.Format(": Failed!"));
        }

        /// <summary>
        /// This method is called when the user clicks the OK button in the BrailleMusicSettings Form.
        /// </summary>
        public void SaveSettings()
        {
            brailleDevicePreferences.PageWidth = (int)this.numericUpDownWidth.Value;
            brailleDevicePreferences.PageHeight = (int)this.numericUpDownHeight.Value;
            brailleDevicePreferences.EscapeSequence = this.textBoxEscapeSequence.Text;
            brailleDevicePreferences.DeviceName = this.textBoxDeviceName.Text;
            brailleDevicePreferences.ApplicationName = this.textBoxApplicationName.Text;
            brailleDevicePreferences.ApplicationLocation = this.textBoxApplicationExe.Text;

#warning TODO Check if the following default-setting can be replaced by a value in the settings spreadsheet
            if ((null != this.listBoxFileFormat.SelectedItem) && (this.listBoxFileFormat.SelectedItem is EncodingItem))
            {
                brailleDevicePreferences.BrailleFileFormat = (this.listBoxFileFormat.SelectedItem as EncodingItem).Encoding;
            }
            else
            {
                // Maybe this is not the right place to declare a default value ?? NO It is better done in the Settings spreadsheet
                brailleDevicePreferences.BrailleFileFormat = BrailleFileHandler.FileEncoding.BRF_ASCII;
            }
#warning TODO Check if the following default-setting can be replaced by a value in the settings spreadsheet
            if ((null != this.listBoxBraillePageLayout.SelectedItem) && (this.listBoxBraillePageLayout.SelectedItem is PageLayoutItem))
            {
                brailleDevicePreferences.BraillePageLayout = (this.listBoxBraillePageLayout.SelectedItem as PageLayoutItem).PageLayout;
            }
            else
            {
                // Maybe this is not the right place to declare a default value ?? NO It is better done in the Settings spreadsheet
                brailleDevicePreferences.BraillePageLayout = Model.BrailleStyleEnum.BANA2015;
            }



            // brailleDevicePreferences.BrailleFileFormat = this. 

            // More to follow
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
                default: Logger.LogCF(string.Format(": Unsupported fileencoding '{0}'", encoding.ToString())); return ""; // ToString on encoding, not on EncodingItem !
            }
        }

        public EncodingItem(BrailleFileHandler.FileEncoding encoding)
        {
            this.encoding = encoding;
        }
    }

    /// <summary>
    /// Simple class for defining UI representations for the various Braille file formats. The values may be localized if required.
    /// </summary>
    internal class PageLayoutItem
    {
        private Model.BrailleStyleEnum pageLayout;
        public Model.BrailleStyleEnum PageLayout{ get { return pageLayout; } }
        public override string ToString()
        {
            switch (pageLayout)
            {
#warning ToDo Localize
                case Model.BrailleStyleEnum.IBOS: return "IBOS";
                case Model.BrailleStyleEnum.BANA2015: return "BANA";
                default: Logger.LogCF(string.Format(": Unsupported pageLayout '{0}'", pageLayout.ToString())); return ""; // ToString on pageLayout, not on PAgeLAyoutItem !
            }
        }

        public PageLayoutItem( Model.BrailleStyleEnum pageLayout )
        {
            this.pageLayout = pageLayout;
        }
    }

}
