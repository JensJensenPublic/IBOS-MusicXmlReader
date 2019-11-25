using System;
using MusicXmlReaderModel;

namespace MusicXmlReader
{

    class BrailleMusicSettingsFormEmbosserHandler : BrailleMusicSettingsFormHandler
    {
        public override string GetTitle() { return "Embosser settings:"; }
        public override string GetLabelDeviceName() { return "Embosser name:"; }
        //public string LabelEscapeSequence = "Escape sequence";
        //public string LabelApplicationName = "Application name";
        //public string LabelApplicationLocation = "Application location";

    }


    class BrailleMusicSettingsFormNoteTakerHandler : BrailleMusicSettingsFormHandler
    {
        public override string GetTitle() { return "Notetaker settings:"; }
        public override string GetLabelDeviceName() { return "Notetaker name"; }
    }


    class BrailleMusicSettingsFormGeneralHandler : BrailleMusicSettingsFormHandler
    {
        public override string GetTitle() { return "Generel Braille Music device settings:"; }
        public override string GetLabelDeviceName() { return ""; }
    }



    abstract class BrailleMusicSettingsFormHandler
    {
        public abstract string GetTitle();
        public abstract string GetLabelDeviceName();
        //public string LabelBrailleFileFormat = "Braille file format";
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
