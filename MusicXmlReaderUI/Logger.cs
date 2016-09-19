using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Windows.Forms;

namespace MusicXmlReaderUI
{
    static public class Logger
    {
        // TODO: Adress possible multithreading problems !!


        // To use a console in a Windows Forms application: Project Properties -> Application -> Output Type -> Console Application
        // "Original value was "Windows Application"
        public static void Trace(string s)
        {
            if (useConsole) Console.WriteLine(s);
        }

        private static string logFileName = "MusicXmlReader.Log"; // This is a default whuch may be overwritten by the application
        private static readonly string mySubDirectoryName = "MusicXmlReader";
        private static bool useConsole = false;
        private static string logFileFullName;
        private static string logFileDirectory;
        private static LoggerCounters localCounters;  // For counting log lines local to one MusicXml file  (Used by all applications) 
        private static LoggerCounters globalCounters; // For counting a sum over a number og MusicXml files (Used for instance by  MusicXmlReaderCmd).


        /// <summary>
        /// Fixes all file and directory names once and for all
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool Open(string fileName)
        {
            bool result = false;
            try
            {
                if (!string.IsNullOrEmpty(fileName))
                {
                    // Replace default filename if desired
                    logFileName = fileName;
                }
                string musicXmlReaderTempDirectory = (System.IO.Path.Combine(System.IO.Path.GetTempPath(), mySubDirectoryName));
                if (!Directory.Exists(musicXmlReaderTempDirectory))
                {
                    Directory.CreateDirectory(musicXmlReaderTempDirectory);
                }
                logFileDirectory = musicXmlReaderTempDirectory;
                logFileFullName = Path.Combine(logFileDirectory, logFileName);

                localCounters = LoggerCounters.Create();
                globalCounters = LoggerCounters.Create();

                result = true;
            }
            catch (Exception)
            {
                // But what can we do ?
            }
            return result;
        }
     
        public static bool UseConsole
        {
            get
            {
                return useConsole;
            }

            set
            {
                useConsole = value;
            }
        }

        public static string LogFileFullName
        {
            get
            {
                return logFileFullName;
            }
        }

        public static string LogFileDirectory
        {
            get
            {
                return logFileDirectory;
            }
        }

        public static void Log(string s)
        {
            if (string.IsNullOrEmpty(logFileFullName)) return; // Open() must be called before using the Logger !
            try
            {
                Trace(s);
                System.DateTime now = System.DateTime.Now;
                string time = string.Format("{0}.{1,03}", now.ToLongTimeString(), now.Millisecond.ToString()); // Always use 3 digits for milliseconds
                System.IO.File.AppendAllText(logFileFullName, time + " " + s + "\r\n");
            }
            catch (Exception)
            {
                // What to do here ??
            }
        }

        /// <summary>
        /// Same as Log() but each string is only logged once !
        /// Instead a statistics is kept for counting how many times the string is logged.
        /// </summary>
        /// <param name="s"></param>
        public static void LogOnce(string s)
        {
            if (localCounters.Add(s))
            {
                Log(s);
            }
            globalCounters.Add(s);
        }

        public static void ClearStatistics()
        {
            localCounters.ClearStatistics();
        }

        /// <summary>
        /// Dumps all strings used as argument to LogOnce with the number of times it has been called
        /// since last call to ClearStatistics()
        /// </summary>
        public static void DumpStatistics()
        {
            Log("Logger.DumpStatistics started");
            int numberOfEntries;
            List<string> strings = localCounters.GetStatistics(out numberOfEntries);
            foreach (string s in strings)
            {
                Log(s);
            } 
            Log(string.Format("Logger.DumpStatistics completed with a total of {0} entries",numberOfEntries));
        }

        public static void DumpGlobalStatistics()
        {
            Log("Logger.DumpGlobalStatistics started");
            int numberOfEntries;
            List<string> strings = globalCounters.GetStatistics(out numberOfEntries);
            strings.Sort();
            foreach (string s in strings)
            {
                Log(s);
            }
            Log(string.Format("Logger.DumpGlobalStatistics completed with a total of {0} entries", numberOfEntries));
        }

        /// <summary>
        /// Log som interesting system parameters
        /// </summary>
        public static void LogSystemParameters()
        {
            bool screenReaderRunning;
            int lastWin32Error;
            string name = "SystemParametersiInfo.GetScreenReader";
            bool ok = SystemParametersiInfo.GetScreenReader(out screenReaderRunning, out lastWin32Error);
            if (ok)
            {
                Log(string.Format("{0} reported {1}", name, screenReaderRunning));
            }
            else
            {
                Log(string.Format("{0} failed. LastWin32Error = {1}", name, lastWin32Error));
            }
        }
        

    }
}
