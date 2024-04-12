using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MusicXmlReaderModel
{
    /// <summary>
    /// For handling installation, manipulation and use of JAWS scripts
    /// </summary>
    public class ScriptHandlerForJAWS 
    {

        // List of locale names supported by JAWS 2024 and found at C:\Program Files\Freedom Scientific\JAWS\2024\GetVoices\Locale        
        private List<string> JawsLocales = new List<string>() { "arb", "cht", "csy", "dan", "deu", "esn", "eti", "fin", "fra", "frc", "heb", "hun", "ita", "jpn", "kor", "lvi", "mki", "nld", "nor", "plk", "ptb", "rus", "sky", "sqi", "sve", "trk", "ukr", "enu" };

        /// <summary>
        /// Last fallback if IBOS MusicXmlReader is not localized to the language used by JAWS
        /// Look for a directory with a valid name
        /// </summary>
        /// <param name="directoryName"></param>
        /// <param name="settingsDirectory"></param>
        /// <returns></returns>
        private string GetJawsFallbackSettingsDirectory(string settingsDirectory)
        {
            string result = "";
            string[] directories = Directory.GetDirectories(settingsDirectory);
            List<string> localeDirectories = new List<string>();
            foreach (string directory in directories)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(directory);
                string shortName = directoryInfo.Name;
                if (JawsLocales.Contains(shortName))
                {
                    localeDirectories.Add(directory);
                    Logger.LogCF(string.Format(": Found {0}", directory));
                }
            }
            if (localeDirectories.Count > 0)
            {
                result = localeDirectories[0];
                Logger.LogCF(string.Format(": Using {0}", result));
            }
            else
            {
                Logger.LogCF(string.Format(": No JAWS settings directory found"));
            }
            return result;
        }


        private string LogDirectories(string[] directories)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string directory in directories)
            {
                sb.AppendLine(""); // In order to align the directorynames
                sb.Append(" '" + directory + "'");
            }
            string s = sb.ToString();
            //if (!string.IsNullOrEmpty(s))
            //{
            //    Logger.Log(s);
            //}
            return s;
        }



        /// <summary>
        /// Returns the path to the directory holding the USER-SPECIFIC JAWS settings.
        /// In general:
        /// "C:\Users\{UserName}\AppData\Roaming\Freedom Scientific\JAWS\{JawsVersion}\Settings\{Locale}"
        /// where {UserName}, {JawsVersion} and {Locale} are filled in with actual values.        /// 
        /// For instance:
        /// "C:\Users\holme\AppData\Roaming\Freedom Scientific\JAWS\2024\Settings\dan"
        /// If more that one JAWS version is installed on the machine, the one with the highest version number will be returned. 
        /// </summary>
        /// <returns>
        /// Returns the path to the directory holding the USER-SPECIFIC JAWS settings.
        /// </returns>
        public string GetJawsSettingsDirectory()
        {
            // TODO Consider using a link file as for MuseScore and Sibelius !
            string directoryName = "";
            try
            {
                // Attempt to locate the JAWS settings file.
                // This includes variable directory names for user, version and locale  so we need to use a little heuristics !
                string roamingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string jawsDirectory = Path.Combine(roamingDirectory, @"Freedom Scientific\JAWS");
                // Find the highest version of JAWS, assuming the directories are listed in alphabetical order.
                string[] jawsVersionDirectories = Directory.GetDirectories(jawsDirectory);
                Logger.LogCF(string.Format(": Found {0} directories in {1}: {2}", jawsVersionDirectories.Length, jawsDirectory, LogDirectories(jawsVersionDirectories)));
                if (jawsVersionDirectories.Length != 0)
                {
                    string highestVersionDirectory = highestVersionDirectory = jawsVersionDirectories[jawsVersionDirectories.Length - 1];
                    string settingsDirectory = Path.Combine(highestVersionDirectory, "Settings");
                    string[] languageDirectories = Directory.GetDirectories(settingsDirectory);
                    Logger.LogCF(string.Format(": Found {0} directories in {1}: {2}", languageDirectories.Length, settingsDirectory, LogDirectories(languageDirectories)));
                    directoryName = Path.Combine(settingsDirectory, ResourcesForModel.JawsSettingsLanguageName); // "dan" for Danish
                    if (!Directory.Exists(directoryName)) // Invert the condition to test the fallback mechanism
                    {
                        Logger.LogCF(string.Format(": JAWS settings directory '{0}' does not exist. Looking for alternative", directoryName));
                        directoryName = GetJawsFallbackSettingsDirectory(settingsDirectory);
                    }
                }
            }
            catch (Exception e)
            {
                directoryName = "";
                Logger.LogCF(string.Format(": Exception caught while attempting to locate JAWS settings directory. Message='{0}'", e.Message));
            }
            return directoryName;
        }


        /// <summary>
        /// Returns the path to the directory holding the SHARED JAWS script files.
        /// For JAWS 2024 this will typically be:
        /// "C:\ProgramData\Freedom Scientific\JAWS\2024\scripts"
        /// If more that one JAWS version is installed on the machine, the one with the highest version number will be returned. 
        /// </summary>
        /// <returns>
        /// Returns the path to the directory holding the shared JAWS script files.
        /// </returns>
        public string GetJawsSharedScriptsDirectory()
        {
            string result = "";
            string programDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string jawsDirectory = Path.Combine(programDataDirectory, @"Freedom Scientific\JAWS");
            // Find the highest version of JAWS, assuming the directories are listed in alphabetical order.
            string[] jawsVersionDirectories = Directory.GetDirectories(jawsDirectory);
            Logger.Log(string.Format(": Found {0} directories in {1}: {2}", jawsVersionDirectories.Length, jawsDirectory, LogDirectories(jawsVersionDirectories)));
            if (jawsVersionDirectories.Length != 0)
            {
                string highestVersionDirectory = jawsVersionDirectories[jawsVersionDirectories.Length - 1];
                result = Path.Combine(highestVersionDirectory, "Scripts"); // For instance "C:\ProgramData\Freedom Scientific\JAWS\2024\Scripts"
            }
            return result;
        }

        /// <summary>
        /// For automatic installation of the JAWS script distributed with IBOS MusicXmlReader.
        /// </summary>
        public void OnProgramStart(string executingAssemblyFullPath)
        {
            string scriptDirectory = GetJawsSharedScriptsDirectory(); // We want to install the script as shared bewtween all users.
            if (string.IsNullOrEmpty(scriptDirectory)) return; // If JAWS is not installed on the machine the directory does not exist.
            int nCopied = CopyAllScriptFiles(executingAssemblyFullPath);
            Logger.LogCF(string.Format(": Copied {0} files", nCopied));
#warning TODO Implement more sophisticated rules for copying: Only copy nower files, Do not copy when files have explicitly been deleted.

        }

        public int CopyAllScriptFiles(string executingAssemblyFullPath)
        {
            int nCopiedFiles = 0;
            //throw new Exception("For test only");
            string JAWSScriptDirectoryName = GetJawsSharedScriptsDirectory(); // For instance "C:\ProgramData\Freedom Scientific\JAWS\2024\scripts"
            string executingAssemblyDirectory = Path.GetDirectoryName(executingAssemblyFullPath);
            string JAWSSourceDirectory = Path.Combine(executingAssemblyDirectory, "JAWS");
            string JAWSScriptSourceDirectory = Path.Combine(JAWSSourceDirectory, "Scripts");
            string[] Scriptfiles = Directory.GetFiles(JAWSScriptSourceDirectory);
            bool overWrite = true;
            foreach (string s in Scriptfiles)
            {
                string shortFileName = Path.GetFileName(s);
                string destination = Path.Combine(JAWSScriptDirectoryName, shortFileName);
                File.Copy(s, destination, overWrite);
                Logger.LogCF(string.Format(": Copied {0} to {1}", shortFileName, destination));
                nCopiedFiles++;
            }
            return nCopiedFiles;
        }


        public int DeleteAllScriptFiles(string fileNameForDeletion)
        {
            int nDeletedFiles = 0;
            string JAWSScriptDirectoryName = GetJawsSharedScriptsDirectory(); // For instance "C:\ProgramData\Freedom Scientific\JAWS\2024\scripts"
            string[] scriptFiles = Directory.GetFiles(JAWSScriptDirectoryName);
            List<string> scriptExtensions = new List<string>() { ".JSS", ".JSB", ".jsb", ".JSD", ".JKM" };
            foreach (string file in scriptFiles)
            {
                if (Path.GetFileNameWithoutExtension(file) == fileNameForDeletion)
                {
                    string extension = Path.GetExtension(file);
                    if (scriptExtensions.Contains(extension))
                    {
                        File.Delete(file);
                        Logger.LogCF(string.Format(": Deleted {0} JAWS script files", file));
                        nDeletedFiles++;
                    }
                }
            }
            return nDeletedFiles;
        }



    private ScriptHandlerForJAWS() { }


        public static ScriptHandlerForJAWS Create() { return new ScriptHandlerForJAWS(); } 
    }
}
