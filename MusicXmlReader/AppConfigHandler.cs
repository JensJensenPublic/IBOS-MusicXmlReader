using MusicXmlReaderModel;

namespace MusicXmlReader
{
    /// <summary>
    /// Collect all keys used in App.Config at one place
    /// </summary>
    static public class AppConfigHandler
    {
        public enum KeyEnum
        {
            Unknown,
            DeveloperCulture,
            DeveloperMode,
            ExperimentalCode,
            HandleGraphics,
            DecoderDeveloperMode,
            UseExternal7Zip,
            ExportToMusicXml
        }

        /// <summary>
        /// The switch in this method must contain all keys from the App.Config file
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private static string GetKeyName(KeyEnum key)
        {
            switch (key)
            {
                case KeyEnum.Unknown: return "";
                case KeyEnum.DeveloperCulture: return "DeveloperCulture";
                case KeyEnum.DeveloperMode: return "DeveloperMode";
                case KeyEnum.ExperimentalCode: return "ExperimentalCode";
                case KeyEnum.HandleGraphics: return "HandleGraphics";
                case KeyEnum.DecoderDeveloperMode: return "DecoderDeveloperMode";
                case KeyEnum.UseExternal7Zip: return "UseExternal7Zip";
                case KeyEnum.ExportToMusicXml: return "ExportToMusicXml";
                default: return null;
            }
        }

        public static string GetValue(KeyEnum key)
        {
            string keyName = GetKeyName(key);
            string result  = System.Configuration.ConfigurationManager.AppSettings.Get(keyName);
            Logger.LogCF(string.Format(": KeyEnum={0} KeyName={1} result={2}", key.ToString(), keyName, result));
            return result;
        }


        public static int GetIntValue(AppConfigHandler.KeyEnum key, int defaultValue)
        {
            int temp;
            string s = AppConfigHandler.GetValue(key);
            bool b = int.TryParse(s, out temp);
            int result = b ? temp : defaultValue;
            Logger.LogCF(string.Format(": Key={0} returned {1} {2}", key.ToString(), result, b ? "found in App.Config" : "(Using default value. No valid value  for this key found in App.Config)"));
            return result;
        }


    }
}
