using MusicXmlReaderModel;

namespace MusicXmlReader
{
    /// <summary>
    /// Collect all keys used in App.Config at one place
    /// </summary>
    static public class AppConfigHandler
    {
        public enum KeyEnum
        { Unknown,
            // MuseScoreExe, // Now through userSettingsHandler
            // SibeliusExe, // Now through userSettingsHandler
            DeveloperCulture,
            DeveloperMode,
            ExperimentalCode,
            HandleGraphics,
            //EmbosserCharactersPerLine ,
            //EmbosserLinesPerPage,
            IBPrintExe
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
                //case KeyEnum.SibeliusExe: return "SibeliusExe"; // Now through userSettingsHandler
                //case KeyEnum.MuseScoreExe: return "MuseScoreExe"; // Now through userSettingsHandler
                case KeyEnum.DeveloperCulture: return "DeveloperCulture";
                case KeyEnum.DeveloperMode: return "DeveloperMode";
                case KeyEnum.ExperimentalCode: return "ExperimentalCode";
                case KeyEnum.HandleGraphics: return "HandleGraphics";
                //case KeyEnum.EmbosserCharactersPerLine: return "EmbosserCharactersPerLine";
                //case KeyEnum.EmbosserLinesPerPage: return "EmbosserLinesPerPage";
                case KeyEnum.IBPrintExe: return "IBPrintExe";
                default: return null;
            }
        }

        public static string GetValue(KeyEnum key)
        {
            string keyName = GetKeyName(key);
            string result  = System.Configuration.ConfigurationManager.AppSettings.Get(keyName);
            Logger.LogCF(string.Format("KeyEnum={0} KeyName={1} result={2}", key.ToString(), keyName, result));
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
