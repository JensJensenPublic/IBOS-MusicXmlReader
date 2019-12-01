using System;
using System.Collections.Generic;
using MusicXmlReaderModel;

namespace MusicXmlReader
{

#warning TODO Localize
    class BrailleMusicSettingsFormEmbosserHandler : BrailleMusicSettingsFormHandler
    {
        public override string Title { get { return "Embosser settings"; } }
        public override string LabelDeviceName { get { return "Embosser name:"; } }
        // LabelBrailleFileFormat: Inherit value
        public override string LabelWidth { get { return "Page width"; } }
        public override string LabelHeight { get { return "Page height"; } }
        // LabelEscapeSequence: Inherrit value 
        // LabelApplicationName: Inherrit value
        // LabelApplicationLocation: Inherrit value

    }


    class BrailleMusicSettingsFormNoteTakerHandler : BrailleMusicSettingsFormHandler
    {
        public override string Title { get { return "Notetaker settings"; } }
        public override string LabelDeviceName { get { return "Notetaker name"; } }
        // LabelBrailleFileFormat: Inherit value
        public override string LabelWidth { get { return "Line width"; } }
        public override string LabelHeight { get { return ""; } } // Setting the label to "" makes label and entry field invisible!
        public override string LabelEscapeSequence { get { return ""; } }  // Setting the label to "" makes label and entry field invisible!
        public override string LabelApplicationName { get { return ""; } } // Setting the label to "" makes label and entry field invisible!
        public override string LabelApplicationLocation { get { return ""; } } // Setting the label to "" makes label and entry field invisible!
    }


    class BrailleMusicSettingsFormGenericHandler : BrailleMusicSettingsFormHandler
    {
        public override string Title { get { return "Generic Braille device settings"; } }
        public override string LabelDeviceName { get { return "Device name"; } }
        // LabelBrailleFileFormat: Inherit value
        public override string LabelWidth { get { return "Device width"; } }
        public override string LabelHeight { get { return "Device height"; } }
        // LabelEscapeSequence: Inherrit value 
        // LabelApplicationName: Inherrit value
        // LabelApplicationLocation: Inherrit value
        public override List<BrailleFileHandler.FileEncoding> EnabledEncodings { get { return allEncodings; } }
    }


    /// <summary>
    /// Abstract helper class for supporting the BrailleMusicSettingsForm.
    /// 3 classes are derived for Embosser, Notetaker and Generic Braille Device
    /// Establishes the interface towards the UI,
    /// (The UserPreferencesHandler establishes the interface towarts the Windows Configuration system)
    /// </summary>
    abstract class BrailleMusicSettingsFormHandler
    {
        // Private definitions used for export to Braille Music
        private const Model.BrailleStyleEnum IbosStyle = Model.BrailleStyleEnum.IBOS;
        private const Model.BrailleStyleEnum BanaStyle = Model.BrailleStyleEnum.BANA2015;

        // Shorthand for the 3 most common encodings
        protected List<BrailleFileHandler.FileEncoding> defaultEncodings = new List<BrailleFileHandler.FileEncoding>()
        {
            BrailleFileHandler.FileEncoding.BRF_ASCII,
            BrailleFileHandler.FileEncoding.BRL_OctoBraille_1252,
            BrailleFileHandler.FileEncoding.BRF_Unicode_utf8
        };

        // Shorthand for all encodings implemented
        protected List<BrailleFileHandler.FileEncoding> allEncodings = new List<BrailleFileHandler.FileEncoding>()
        {
            BrailleFileHandler.FileEncoding.BRF_ASCII,
            BrailleFileHandler.FileEncoding.BRL_OctoBraille_1252,
            BrailleFileHandler.FileEncoding.BRF_Unicode_utf8,
            BrailleFileHandler.FileEncoding.BRF_Unicode,
            BrailleFileHandler.FileEncoding.BRF_Unicode_utf16,
            BrailleFileHandler.FileEncoding.BRF_Unicode_utf32,
            BrailleFileHandler.FileEncoding.PEF
        };

        // Shorthand for all Braille Page Layouts implemented
        protected List<Model.BrailleStyleEnum> braillePageLayouts = new List<Model.BrailleStyleEnum>
        {
            Model.BrailleStyleEnum.BANA2015,
            Model.BrailleStyleEnum.IBOS
        };

#warning ToDo Localize

        // Fixed localized texts: Dialog Title and lable names:
        public abstract string Title { get; }
        public abstract string LabelDeviceName { get; }
        public virtual string LabelBrailleFileFormat { get { return "Braille file format"; } } // Override and return "" to hide label and text
        public virtual string LabelBraillePageLayout { get { return "Praille page layout"; } }
        public abstract string LabelWidth { get; }
        public abstract string LabelHeight { get; }
        public virtual string LabelEscapeSequence { get { return "Escape sequence"; } }
        public virtual string LabelApplicationName { get{ return"Application name"; } }
        public virtual string LabelApplicationLocation { get { return "Application location"; } }
        public virtual List<BrailleFileHandler.FileEncoding> EnabledEncodings { get { return defaultEncodings; } }
        public virtual List<Model.BrailleStyleEnum> BraillePageLayouts { get { return braillePageLayouts; } }

        public static  BrailleMusicSettingsFormHandler Create(BrailleMusicSettingsForm.DeviceTypeEnum deviceTypeEnum)
        {

            switch (deviceTypeEnum)
            {
                case BrailleMusicSettingsForm.DeviceTypeEnum.Embosser: return new BrailleMusicSettingsFormEmbosserHandler();
                case BrailleMusicSettingsForm.DeviceTypeEnum.NoteTaker: return new BrailleMusicSettingsFormNoteTakerHandler();
                case BrailleMusicSettingsForm.DeviceTypeEnum.GeneralDevice: return new BrailleMusicSettingsFormGenericHandler();
                    default:
                    Logger.LogCF(string.Format(": Unsupported device type '{0}'", deviceTypeEnum.ToString()));
                    return null;

            }
        }

    }
}
