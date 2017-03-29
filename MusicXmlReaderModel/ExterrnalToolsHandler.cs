using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MusicXmlReaderModel
{
    public class ExternalToolsHandler
    {
        private string className = "ExternalToolsHandler"; 

        // Force use of Create() method
        private ExternalToolsHandler()
        {
        }

        public static ExternalToolsHandler Create()
        {
            return new ExternalToolsHandler();
        }


        // To use a console in a Windows Forms application: Project Properties -> Application -> Output Type -> Console Application
        // "Original value was "Windows Application"

        public void ReadLogFile()
        {
            Utilities.RunExeWithFileArgument("notepad.exe", System.IO.Path.Combine(System.IO.Path.GetTempPath(), Logger.LogFileFullName));
        }

        public void OpenLogFileLocation()
        {
            Utilities.RunExeWithDirArgument("explorer.exe", Logger.LogFileDirectory);
        }

        public void OpenMusicXmlFileLocation(string theMusicXmlFileName)
        {
            if (String.IsNullOrEmpty(theMusicXmlFileName))
            {
                Utilities.UtilityClient.ShowWarning((int)ModelMessageEnum.UnspecifiedMusicXmlFile, theMusicXmlFileName, "");
                return;
            }
            string dir = Path.GetDirectoryName(theMusicXmlFileName);
            if (!System.IO.Directory.Exists(dir))
            {
                // Do not report the path "dir" in the error message, i may be very long
                Utilities.UtilityClient.ShowWarning((int)ModelMessageEnum.DirectoryNotFound, "", "");
            }
            Utilities.RunExeWithDirArgument("explorer.exe", dir);
        }





        public static string InterpretationFileName = "MusicReader.txt";
        public void ReadInterpretation(List<MusicXmlObject> allMusicXmlObjecsts, string theMusicXmlFileName)
        {
            if (!System.IO.File.Exists(theMusicXmlFileName))
            {
                Utilities.UtilityClient.ShowWarning((int)ModelMessageEnum.UnspecifiedMusicXmlFile, "", "");
                return;
            }

            string fileName = System.IO.Path.Combine(System.IO.Path.GetTempPath(), InterpretationFileName);
            //System.IO.FileStream  fileStream = System.IO.File.OpenWrite(InterpretationFileName);
            // Create contents
            System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(fileName);
            if ((null != allMusicXmlObjecsts) && (allMusicXmlObjecsts.Count > 0))
            {
                foreach (object o in allMusicXmlObjecsts)
                {
                    streamWriter.WriteLine(o.ToString());
                }

            }
            else
            {
                streamWriter.WriteLine("No MusicXml objects found");
            }

            streamWriter.Close();
            Utilities.RunExeWithFileArgument("notepad.exe", System.IO.Path.Combine(System.IO.Path.GetTempPath(), fileName));

            //ReadTempFileByNotepad(fileName);
        }

        public void ReadMusicXmlFile(string theMusicXmlFileName)
        {
            Utilities.RunExeWithFileArgument("iexplore.exe", theMusicXmlFileName);
        }

        /// <summary>
        /// Find a shortcut on the desktop with the name specified
        /// </summary>
        /// <param name="shortcutName">NAme of shortcut</param>
        /// <returns>The name of the link forun in the shortcut</returns>
        private string GetLinkFromShortcutAtDesktop(string shortcutName)
        {
            string functionName = "GetLinkFromShortcutAtDesktop";
            try
            {
                string desktopDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                string extension = "lnk";
                string directory = Path.Combine(desktopDirectory, shortcutName);
                string linkFileName = Path.ChangeExtension(directory, extension);
                if (File.Exists(linkFileName))
                {
                    Logger.Log(string.Format("{0}.{1} found shortcut='{2}'", className, functionName, shortcutName));
                    return linkFileName;
                }
            }
            catch (Exception e)
            {
                Logger.Log(string.Format("{0}.{1} threw an exception. Message='{2}'", className, functionName, e.Message));
            }
            Logger.Log(string.Format("{0}.{1} failed to find a shortcut {2}", className, functionName, shortcutName));
            return null;
        }
        

        public void StartMuseScore(string theMusicXmlFileName)
        {
            string linkName = GetLinkFromShortcutAtDesktop(ResourcesForModel.Shortcut_MuseScore);
            string exeFileName = @"C:\Program Files (x86)\MuseScore 2\bin\MuseScore.exe";
            // Use the link if found, otherwise the hardwired location
            string executable = string.IsNullOrEmpty(linkName) ? exeFileName : linkName;
            Utilities.RunExeWithFileArgument(exeFileName, theMusicXmlFileName);
        }

        public void StartSibelius(string theMusicXmlFileName)
        {
            string linkName = GetLinkFromShortcutAtDesktop(ResourcesForModel.Shortcut_Sibelius);
            string exeFileName = @"C:\Program Files\Avid\Sibelius\Sibelius.exe";
            // Use the link if found, otherwise the hardwired location
            string executable = string.IsNullOrEmpty(linkName) ? exeFileName : linkName; 
            Utilities.RunExeWithFileArgument(executable, theMusicXmlFileName);            
        }

        public void OpenUrl(string url)
        {
            Utilities.RunExeWithUrlArgument("iexplore.exe", url);
        }


        private string LogDirectories(string[] directories)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string directory in directories)
            {
                sb.Append(" '" +  directory + "'");
            }
            string s = sb.ToString();
            if (!string.IsNullOrEmpty(s))
            {
                Logger.Log(s);
            }
            return s;
        }


        /// <summary>
        /// Show the JAWS application specific configuration file in notepad.
        /// TO DO: The directory name is JAWS version specific "17.0" and language specific "dan". Fix this !!
        /// </summary>
        public void ReadJawsSettingsFile(string fileName)
        {
            // TODO Consider using a link file as for MuseScore and Sibelius !
            string methodName = "ReadJawsSettingsFile";
            string extension = "JCF"; // JAWS configuration file
            string fileNameWithExtension = Path.ChangeExtension(fileName, extension);
            string directoryName = "";
            try
            {
                // Attempt to locate the JAWS settings file.
                // This includes variable directory names for user, version and locale  so we need to use a little heuristics !
                string roamingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string jawsDirectory = Path.Combine(roamingDirectory, @"Freedom Scientific\JAWS");
                // Find the highest version of JAWS, assuming the directories are listed in alphabetical order.
                string[] jawsVersionDirectories = Directory.GetDirectories(jawsDirectory);
                Logger.Log(string.Format("{0}.{1} Found {2} directories in {3}", className, methodName, jawsVersionDirectories.Length, jawsDirectory));
                LogDirectories(jawsVersionDirectories); // Only for debugging
                if (jawsVersionDirectories.Length != 0)
                {
                    string highestVersionDirectory = highestVersionDirectory = jawsVersionDirectories[jawsVersionDirectories.Length - 1];
                    string settingsDirectory = Path.Combine(highestVersionDirectory, "Settings");
                    string[] languageDirectories = Directory.GetDirectories(settingsDirectory);
                    Logger.Log(string.Format("{0}.{1} Found {2} directories in {3}", className, methodName, languageDirectories.Length, settingsDirectory));
                    LogDirectories(languageDirectories); // Only for debugging
                    directoryName = Path.Combine(settingsDirectory, ResourcesForModel.JawsSettingsLanguageName); // "dan" for Danish                     
                }
            }
            catch (Exception e)
            {
                directoryName = "";
                Logger.Log(string.Format("{0}.{1} Exception caught while attempting to locate JAWS settings directory. Message='{2}'", className, methodName, e.Message));
            }
            //string directoryName = @"C:\Users\Jens\AppData\Roaming\Freedom Scientific\JAWS\17.0\Settings\dan"; // Before version 1.0.0.0

            if (string.IsNullOrEmpty(directoryName))
            {
                // This includes explicitly detected errors as wells as exceptions !
                Utilities.ShowWarning(ModelMessageEnum.LocationNotDetermined, fileNameWithExtension, "");
                // Show as messagebox
            }
            else
            {  
                string JawsSettingsFullFileName = Path.Combine(directoryName, fileNameWithExtension);
                if (
                   Utilities.CheckDirectoryExistance(directoryName, methodName)
                && Utilities.CheckFileExistance(JawsSettingsFullFileName, methodName))
                {
                    Utilities.RunExeWithFileArgument("notepad.exe", JawsSettingsFullFileName);
                }
            }
        }

    }
}
