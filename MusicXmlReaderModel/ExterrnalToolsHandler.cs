using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicXmlReaderUI;
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


        #region LogFile
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
                return;
            }
            string dir = Path.GetDirectoryName(theMusicXmlFileName);
            if (System.IO.Directory.Exists(dir))
            {
                Utilities.RunExeWithDirArgument("explorer.exe", dir);
            }
        }

        #endregion


        #region InterpretationFile
        public static string InterpretationFileName = "MusicReader.txt";
        public void ReadInterpretation(List<MusicXmlObject> allMusicXmlObjecsts)
        {
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
#endregion

#region MusicXmlFile
        public void ReadMusicXmlFile(string theMusicXmlFileName)
        {
            if (System.IO.File.Exists(theMusicXmlFileName))
            {
                //ReadFileByNotepad(theMusicXmlFileName);
                //ReadFileByExecutable("iexplore.exe",theMusicXmlFileName);
                Utilities.RunExeWithFileArgument("iexplore.exe", theMusicXmlFileName);

            }
        }
#endregion


#region MuseScore
        public void StartMuseScore(string theMusicXmlFileName)
        {
            if (System.IO.File.Exists(theMusicXmlFileName))
            {
                //string exeFileName = @"C:\Program Files(x86)\MuseScore 2\bin\MuseScore.exe";
                string exeFileName = @"C:\Program Files (x86)\MuseScore 2\bin\MuseScore.exe";
                //ReadFileByMuseScore(exeFileName, theMusicXmlFileName);
                Utilities.RunExeWithFileArgument(exeFileName, theMusicXmlFileName);

            }
        }
        #endregion

        public void StartSibelius(string theMusicXmlFileName)
        {
            // ToDo: Implement!
        }  

    }
}
