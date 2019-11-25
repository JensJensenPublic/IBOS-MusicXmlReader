using System;
using MusicXmlReaderModel;

namespace MusicXmlReader
{

    class BrailleMusicSettingsFormEmbosserHandler : BrailleMusicSettingsFormHandler
    {
        public override string Title { get { return "Embosser settings:"; } }
        public override string GetLabelDeviceName() { return "Embosser name:"; }
        public override string LabelBrailleFileFormat { get { return "ASCII"; } }
        //public string LabelEscapeSequence = "Escape sequence";
        //public string LabelApplicationName = "Application name";
        //public string LabelApplicationLocation = "Application location";

    }


    class BrailleMusicSettingsFormNoteTakerHandler : BrailleMusicSettingsFormHandler
    {
        public override string Title { get { return "Notetaker settings:"; } }
        public override string GetLabelDeviceName() { return "Notetaker name"; }
        public override string LabelBrailleFileFormat { get { return "ASCII"; } }
    }


    class BrailleMusicSettingsFormGeneralHandler : BrailleMusicSettingsFormHandler
    {
        public override string Title { get { return "Generel Braille Music device settings:"; } }
        public override string GetLabelDeviceName() { return ""; }
        public override string LabelBrailleFileFormat { get { return "ASCII"; } }
    }



    abstract class BrailleMusicSettingsFormHandler
    {
        public abstract string Title {get; }
        public abstract string GetLabelDeviceName();
        public abstract string LabelBrailleFileFormat { get; }
        //public string LabelWidth = "Width";
        //public string LabelHeight = "Height";
        //public string LabelEscapeSequence = "";
        //public string LabelApplicationName = "";
        //public string LabelApplicationLocation = "";

        public static  BrailleMusicSettingsFormHandler Create(BrailleMusicSettingsForm.DeviceTypeEnum deviceTypeEnum)
        {

            switch (deviceTypeEnum)
            {
                case BrailleMusicSettingsForm.DeviceTypeEnum.Embosser: return new BrailleMusicSettingsFormEmbosserHandler();
                case BrailleMusicSettingsForm.DeviceTypeEnum.NoteTaker: return new BrailleMusicSettingsFormNoteTakerHandler();
                case BrailleMusicSettingsForm.DeviceTypeEnum.GeneralDevice: return new BrailleMusicSettingsFormGeneralHandler();
                    default:
                    Logger.LogCF(string.Format(": Unsupported device type '{0}'", deviceTypeEnum.ToString()));
                    return null;

            }
        }

    }
}
