using MusicXmlReaderModel;

namespace MusicXmlReader
{
    /// <summary>
    /// Collect all keys used in App.Config at one place
    /// </summary>
    static public class AppConfigHandler
    {
        public enum KeyEnum {Unknown, MuseScoreExe, SibeliusExe, DeveloperCulture, DeveloperMode, ExperimentalCode }

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
                case KeyEnum.SibeliusExe: return "SibeliusExe";
                case KeyEnum.MuseScoreExe: return "MuseScoreExe";
                case KeyEnum.DeveloperCulture: return "DeveloperCulture";
                case KeyEnum.DeveloperMode: return "DeveloperMode";
                case KeyEnum.ExperimentalCode: return "ExperimentalCode";
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
    }
}
