using System;
using System.IO;
using MusicXmlReaderModel;
using MusicXmlReader.Properties;

namespace MusicXmlReader
{


    public abstract class BrailleDevice
    {
        public abstract string Name{ get; set; }
        public abstract string EscapeSequence { get; set; }
        public abstract string FileFormat { get; set; }
        public abstract int PageWidth { get; set; }
        public abstract int PageHeight { get; set; }

        public void Log()
        {
            string className = this.GetType().Name;
            string deviceName = Name;
            string esc = EscapeSequence;
            int width = PageWidth;
            int height = PageHeight;
            Logger.LogCF(string.Format(": Class='{0}' Name='{1}' EscapeSequence='{2}' PageWidth={3} PageHeight={4}", className , deviceName, esc,width,height ));
        }
    }

    public class BrailleEmbosser : BrailleDevice
    {
        private Settings settings;
        public override string Name { get { return settings.EmbosserName; }  set { settings.EmbosserName = value; } }
        public override string EscapeSequence { get { return settings.EmbosserEscapeSequence; } set { settings.EmbosserEscapeSequence = value; } }
        public override string FileFormat {get { return settings.EmbosserFileFormat; } set { settings.EmbosserFileFormat = value; } }
        public override int PageWidth { get { return settings.EmbosserPageWidth; } set { settings.EmbosserPageWidth = value; } }
        public override int PageHeight { get { return settings.EmbosserPageHeight; } set { settings.EmbosserPageHeight = value; } }

        internal BrailleEmbosser(MusicXmlReader.Properties.Settings settings)
        {
            this.settings = settings;
        }
    }

    public class BrailleNoteTaker : BrailleDevice
    {
        private Settings settings;
        public override string Name { get { return settings.NoteTakerName; } set { settings.NoteTakerName = value; }  }
        public override string EscapeSequence { get { return settings.NoteTakerEscapeSequence; } set { settings.NoteTakerEscapeSequence = value; }      }
        public override string FileFormat { get { return settings.NoteTakerFileFormat; } set { settings.NoteTakerFileFormat = value; } }
        public override int PageWidth { get { return settings.NoteTakerPageWidth; } set { settings.NoteTakerPageWidth = value; } }
        public override int PageHeight { get { return settings.NoteTakerPageHeight; } set { settings.NoteTakerPageHeight = value; } }


        internal BrailleNoteTaker(MusicXmlReader.Properties.Settings settings)
        {
            this.settings = settings;
        }
    }


    /// <summary>
    /// Contains all application-wide settings that can be configured by the user
    /// These settings are found in C:\Users\(user)\AppData\Local\MusicXmlReader
    /// </summary>
    public class UserPreferencesHandler
    {
        /// <summary>
        /// The directory latest used by current user for opening a MusicXml file
        /// </summary>
//        public string MusicXmlDirectory { get { return s.MusicXmlDirectory; } set { s.MusicXmlDirectory = value; } }

        /// <summary>
        /// The MusicXml file  latest used by current user
        /// </summary>
        public string MusicXmlFile { get { return s.MusicXmlFile; } set { s.MusicXmlFile = value; } }

        /// <summary>
        /// The directory latest used by current user for openintg a BrailleMusic file
        /// </summary>
        public string BrailleMusicDirectory { get { return s.BrailleMusicDirectory; } set { s.BrailleMusicDirectory = value; } }

 
        /// <summary>
        /// The embosser used by the current user
        /// </summary>
        public BrailleDevice embosser;

        /// <summary>
        /// The Notetaker used by the current user
        /// </summary>
        public BrailleDevice noteTaker;

        public void Log()
        {
            //Log("MusicXmlDirectory", this.MusicXmlDirectory);
            Log("MusicXmlFile", this.MusicXmlFile);
            Log("BrailleMusicDirectory", this.BrailleMusicDirectory);
            embosser.Log();
            noteTaker.Log();
        }

        private Settings s;

        // Simple convenience method for making public methods more readable
        private string LogWarning(string passThrough, string errorMessage)
        {
            Logger.LogCF(string.Format(": {0}", errorMessage));
            return passThrough;
        }

        /// <summary>
        /// Simple convenience method for extracting a valid base directory name from a fileName
        /// If an existing directory is not found the defaultPath is returned.
        /// </summary>
        /// <param name="fullFileName"></param>
        /// <param name="defaultPath"></param>
        /// <returns></returns>
        public string GetExistingBaseDirectory(string fullFileName, string defaultPath)
        {
            // Only return the value from the User preferences if it represents a valid directory
            if (string.IsNullOrEmpty(fullFileName)) return LogWarning(defaultPath,"Filename is null or empty");
            // We don'care if the file exists! We only care about the directory !
            string directoryName = Path.GetDirectoryName(fullFileName);
            if (string.IsNullOrEmpty(directoryName)) return LogWarning(defaultPath,"DirectoryName is null or empty");
            if (!Directory.Exists(directoryName)) return LogWarning(defaultPath,string.Format(": Directory '{0}' does not exist",directoryName));
            Logger.LogCF(string.Format(": Returned '{0}'", directoryName));
            return directoryName;
        }


        /// <summary>
        /// Simple convenience method for returning a default value if a directory is not found
        /// </summary>
        /// <param name="directoryName"></param>
        /// <param name="defaultPath"></param>
        /// <returns></returns>
        public string GetExistingDirectory(string directoryName, string defaultPath)
        {
            // Only return the value from the User preferences if it represents a valid directory
            if (string.IsNullOrEmpty(directoryName)) return LogWarning(defaultPath, "DirectoryName is null or empty");
            if (!Directory.Exists(directoryName)) return LogWarning(defaultPath, string.Format(": Directory '{0}' does not exist", directoryName));
            Logger.LogCF(string.Format(": Returned '{0}'", directoryName));
            return directoryName;
        }

        /// <summary>
        /// Simple convenience method for extracting a valid (short) filename from a (full) fileName
        /// If an exixting file is not found the defaultPath is returned.
        /// </summary>
        /// <param name="fullFileName"></param>
        /// <param name="defaultPath"></param>
        /// <returns></returns>
        public string GetExistingFile(string fullFileName, string defaultPath)
        {
            // Only return the value from the User preferences if it represents a valid file
            if (string.IsNullOrEmpty(fullFileName)) return LogWarning(defaultPath, "Filename is null or empty");
            if (!File.Exists(fullFileName)) return LogWarning(defaultPath, string.Format(": File '{0}' does not exist", fullFileName));
            string directoryName = Path.GetDirectoryName(fullFileName);
            if (string.IsNullOrEmpty(directoryName)) return LogWarning(defaultPath, "DirectoryName is null or empty");
            if (!Directory.Exists(directoryName)) return LogWarning(defaultPath, string.Format(": Directory '{0}' does not exist", directoryName));
            string fileName = Path.GetFileName(fullFileName); 
            Logger.LogCF(string.Format(": Returned '{0}'", fileName));
            return fileName;
        }





        private void Log(string name, string value)
        {
            Logger.LogCF(string.Format(": Name='{0}' Value='{1}'", name, value));
        }

        UserPreferencesHandler()
        {
            Logger.LogCF(": Entry");
            try
            {
                s = MusicXmlReader.Properties.Settings.Default; // Establish a shorthand notation
                embosser = new BrailleEmbosser(s);
                noteTaker = new BrailleNoteTaker(s);
            }
            catch (Exception e)
            {
                Logger.LogCFE(e);
            }
            Logger.LogCF(": Exit");
        }

        public void Save()
        {
            Logger.LogCF(": Entry");
            Log();
            try
            {       
                s.Save();
            }
            catch (Exception e)
            {
                Logger.LogCFE(e);
            }
            Logger.LogCF(": Exit");
        }

        public void Reset()
        {
            Logger.LogCF(": Entry");
            Log();
            try
            {
                s.Reset();
            }
            catch (Exception e)
            {
                Logger.LogCFE(e);
            }
            Log();
            Logger.LogCF(": Exit");
        }


        public static UserPreferencesHandler Create()
        {
            UserPreferencesHandler result = new UserPreferencesHandler();
            result.Log(); // In this way we can verify that the mechanism works by calling Log() AFTER the construction !
            return result;
        }
    }
}
