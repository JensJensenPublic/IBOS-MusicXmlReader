using System;
using MusicXmlReaderModel;

namespace MusicXmlReader
{

    class BrailleMusicSettingsFormEmbosserHandler : BrailleMusicSettingsFormHandler
    {
        public override string Title { get { return "Embosser settings:"; } }
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
        public override string Title { get { return "Notetaker settings:"; } }
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
        public override string Title { get { return "Generic Braille Music device settings:"; } }
        public override string LabelDeviceName { get { return "Device name:"; } }
        // LabelBrailleFileFormat: Inherit value
        public override string LabelWidth { get { return "Device width"; } }
        public override string LabelHeight { get { return "Device height"; } } 
        // LabelEscapeSequence: Inherrit value 
        // LabelApplicationName: Inherrit value
        // LabelApplicationLocation: Inherrit value

    }



    abstract class BrailleMusicSettingsFormHandler
    {
        // Fixed localized texts: Dialog Title and lable names:
        public abstract string Title { get; }
        public abstract string LabelDeviceName { get; }
        public virtual string LabelBrailleFileFormat { get { return "Braille file format"; } } // Override and return "" to hide label and text
        public abstract string LabelWidth { get; }
        public abstract string LabelHeight { get; }
        public virtual string LabelEscapeSequence { get { return "Escape sequence"; } }
        public virtual string LabelApplicationName { get{ return"Application name"; } }
        public virtual string LabelApplicationLocation { get { return "Application location"; } }

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
