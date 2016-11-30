using System;
using System.Collections.Generic;
using System.IO;

namespace MusicXmlReaderModel
{
    public class ExterrnalToolsHandler
    {

        // Force use of Create() method
        private ExterrnalToolsHandler()
        {
        }

        public static ExterrnalToolsHandler Create()
        {
            return new ExterrnalToolsHandler();
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

        public void StartMuseScore(string theMusicXmlFileName)
        {
            string exeFileName = @"C:\Program Files (x86)\MuseScore 2\bin\MuseScore.exe";
            Utilities.RunExeWithFileArgument(exeFileName, theMusicXmlFileName);
        }

        public void StartSibelius(string theMusicXmlFileName)
        {
            string exeFileName = @"C:\Program Files (x86)\Sibelius.exe"; // TO DO: Specify path for Sibelius !!
            Utilities.RunExeWithFileArgument(exeFileName, theMusicXmlFileName);
        }

        public void OpenUrl(string url)
        {
            Utilities.RunExeWithUrlArgument("iexplore.exe", url);
        }


        /// <summary>
        /// Show the JAWS application specific configuration file in notepad.
        /// TO DO: The directory name is JAWS version specific "17.0" and language specific "dan". Fix this !!
        /// </summary>
        public void ReadJawsSettingsFile()
        {
            string directoryName = @"C:\Users\Jens\AppData\Roaming\Freedom Scientific\JAWS\17.0\Settings\dan";
            string fileName = "IBOS MusicXmlReader";
            string extension = "JCF"; // JAWS configuration file
            string JawsSettingsFullFileName = Path.ChangeExtension(Path.Combine(directoryName, fileName), extension);
            Utilities.RunExeWithFileArgument("notepad.exe", JawsSettingsFullFileName);
        }

    }
}
