using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics; // For finding calling method
using System.Reflection;  // For finding calling method


namespace MusicXmlReaderModel
{
    static public class Logger
    {
        static string className = "Logger";
        // TODO: Adress possible multithreading problems !!

        // Used for instance for appending the name of the .xml file under test when running a test application
        static private string postString = null;
        static public string PostString
        {
            get
            {
                return (postString== null) ? "" : postString;
            }
            set
            {
                postString = value;
            }
        }

        static private string currentEncoding;
        static public string CurrentEncoding
        {
            get { return currentEncoding; }
            set {
                currentEncoding = value;
                LogCF(string.Format(": CurrentEncoding = {0}", currentEncoding));
            }
        }


        //        private static string GetPlatformTempDirectory()
        //        {
        //#if Windows
        //            return System.IO.Path.GetTempPath();
        //#elif Android
        //            return (string)Android.OS.Environment.ExternalStorageDirectory;
        //#else
        //#error Compiling for unknown platform
        //#endif
        //        }


        // To use a console in a Windows Forms application: Project Properties -> Application -> Output Type -> Console Application
        // "Original value was "Windows Application"
        public static void Trace(string s)
        {
            if (useConsole) Console.WriteLine(s);
        }

        private static string logFileName = "MusicXmlReader.Log"; // Name of current log file. This is a default which may be overwritten by the application 
        private static string oldLogFileName = "MusicXmlReader.Old.Log"; // Name of old log file. This is a default which may be overwritten by the application 
        private static readonly string mySubDirectoryName = "MusicXmlReader";
        private static bool useConsole = false;
        private static string logFileFullName;
        private static string oldLogFileFullName;
        private static string logFileDirectory;
        private static LoggerCounters localCounters;  // For counting log lines local to one MusicXml file  (Used by all applications) 
        private static LoggerCounters globalCounters; // For counting a sum over a number og MusicXml files (Used for instance by  MusicXmlReaderCmd).
        private static readonly long maxLogfileLength = 1024 * 1024;  // Max length of the current log file before we rename it at start a new logfile
        private static string musicXmlReaderTempDirectory;

        private static int numberOfLogLines = 0;
        public static int NumberOfLogLines
        {
            get
            {
                return numberOfLogLines;
            }
        }


        public static string MusicXmlReaderTempDirectory
        {
            get
            {
                return musicXmlReaderTempDirectory;
            }
        }


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

                musicXmlReaderTempDirectory = (System.IO.Path.Combine(PlatformDependencies.StaticFunctions.GetPlatformTempDirectory(), mySubDirectoryName));

                if (!Directory.Exists(musicXmlReaderTempDirectory))
                {
                    Directory.CreateDirectory(musicXmlReaderTempDirectory);
                }
                logFileDirectory = musicXmlReaderTempDirectory;
                logFileFullName = Path.Combine(logFileDirectory, logFileName);
                oldLogFileFullName = Path.Combine(logFileDirectory, oldLogFileName);

                // Avoid growing the logfile without limited

                if ((File.Exists(logFileFullName)
                &&  (new FileInfo(logFileFullName)).Length > maxLogfileLength))
                {
                    // Delete the existing "Old" logfile and mark the current logfile to "Old" by renaming it
                    if (File.Exists(oldLogFileFullName))
                    {
                        File.Delete(oldLogFileFullName);
                    }
                    File.Move(logFileFullName, oldLogFileFullName); //Actually a rename !
                }

                localCounters = LoggerCounters.Create();
                globalCounters = LoggerCounters.Create();

                result = true;
            }
            catch (Exception e)
            {
                string s = e.Message; // For debugging
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


        public static string MySubDirectoryName
        {
            get
            {
                return mySubDirectoryName;
            }
        }

        public static void List(String s)
        {
            Log(s, false);
        }


        public static void Log(string s)
        {
            Log(s, true);
        }


        /// <summary>
        /// Same as Log, but automatically adds ClassName and FunctionNAme of the calling function.
        /// </summary>
        /// <param name="s"></param>
        public static void LogCF(string s)
        {
            StackTrace stackTrace = new StackTrace();
            MethodBase methodBase = stackTrace.GetFrame(1).GetMethod();
            Type type = methodBase.ReflectedType;
            //string Namespace = type.Namespace;
            Log(string.Format("{0}.{1}{2}", type.Name, methodBase.Name, s));
            //Console.WriteLine(Namespace + "." + Class.Name + "." + methodBase.Name);
        }

        /// <summary>
        /// Same as LogOnce, but automatically adds ClassName and FunctionNAme of the calling function.
        /// </summary>
        /// <param name="s"></param>
        public static void LogCFOnce(string s)
        {
            StackTrace stackTrace = new StackTrace();
            MethodBase methodBase = stackTrace.GetFrame(1).GetMethod();
            Type type = methodBase.ReflectedType;
            //string Namespace = type.Namespace;
            LogOnce(string.Format("{0}.{1}{2}", type.Name, methodBase.Name, s));
            //Console.WriteLine(Namespace + "." + Class.Name + "." + methodBase.Name);
        }

        private static void Log(string s, bool showTimeStamp)
        {
            if (string.IsNullOrEmpty(logFileFullName)) return; // Open() must be called before using the Logger !
            try
            {
                Trace(s);
                string time = "";
                if (showTimeStamp)
                {
                    System.DateTime now = System.DateTime.Now;
                    time = string.Format("{0}.{1,03}", now.ToLongTimeString(), now.Millisecond.ToString()); // Always use 3 digits for milliseconds
                }
                System.IO.File.AppendAllText(logFileFullName, time + " " + s + "\r\n");
                numberOfLogLines++;
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
        public static void LogOnce(string s0)
        {
            string s = s0 + PostString;
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


        public static List<string> GetStatistics()
        {
            int numberOfEntries;
            return localCounters.GetStatistics(out numberOfEntries); ;
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


        public static List<string> GetGlobalStatistics()
        {
            int numberOfEntries;
            return globalCounters.GetStatistics(out numberOfEntries);
        }


        /// <summary>
        /// Log som interesting system parameters
        /// </summary>
        public static void LogSystemParameters()
        {
            bool screenReaderRunning;
            int lastWin32Error;
            string name = "SystemParametersiInfo.GetScreenReader";
            bool ok = PlatformDependencies.SystemParametersiInfo.GetScreenReader(out screenReaderRunning, out lastWin32Error);
            if (ok)
            {
                Log(string.Format("{0} reported {1}", name, screenReaderRunning));
            }
            else
            {
                Log(string.Format("{0} failed. LastWin32Error = {1}", name, lastWin32Error));
            }
        }

        public static void LogDebuggerAttachment()
        {
            string methodName = "LogDebuggerAttachment";
            bool b = System.Diagnostics.Debugger.IsAttached;
            Log(string.Format("{0}.{1} reported {2}",className, methodName, b));
        }


    }
}
