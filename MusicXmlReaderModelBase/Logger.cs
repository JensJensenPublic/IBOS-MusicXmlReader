using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics; // For finding calling method
using System.Reflection;  // For finding calling method
using System.Text;
using System.Threading;

// NOTE: 
//-------------------------------------------------------------------------------------------------------------------------------------------------------------------
// The MusicXmlReaderModelBase project is designed to avoit circular references and must NOT reference any other MusicXmlREader projects excelt PlatformDependencies.
// It originally contains 3 files from the MusicXmlReaderModel projects: Logger.cs,   LoggerCounters and LoggerDelayCounters.

namespace MusicXmlReaderModel
{
    static public class Logger
    {
        static string className = "Logger";
        // TODO: Adress possible multithreading problems !!

        static Mutex mutex = new Mutex();

        static private bool developerMode = false;
        static public bool DeveloperMode { get { return developerMode; } set { developerMode = value; } }

        static private bool exportToMusicXml = false;
        static public bool ExportToMusicXml { get { return exportToMusicXml; } set { exportToMusicXml = value; } }

        #region DelayMeasurement
        // Overall delays. Only ExecutionDelay is relevant for the UI version    
        static public LoggerDelayCounter ExecutionDelay = LoggerDelayCounter.Create();          // The "real" execution delay, representing delays also relevant for the UI version  
        static public LoggerDelayCounter CleanDirectoriesDelay = LoggerDelayCounter.Create();   // Only relevant for the Cmd version
        static public LoggerDelayCounter CheckDirectoriesDelay = LoggerDelayCounter.Create();   // Only relevant for the Cmd version
        // Ehe excecution delay, relevant for the UI version consists of the sum of the following delays:
        static public LoggerDelayCounter DocLoadDelay = LoggerDelayCounter.Create();
        static public LoggerDelayCounter DocParseDelay = LoggerDelayCounter.Create();
        static public LoggerDelayCounter StructureInitDelay = LoggerDelayCounter.Create();
        static public LoggerDelayCounter BrailleMusicGenerationDelay = LoggerDelayCounter.Create();
        static public void LogDelays()
        {
            string overallDelayMessage = string.Format(" Overall Delays  (in seconds): CleanDirectories={0} Execution={1} CheckDirectories={2}",
                CleanDirectoriesDelay.ToSeconds(), ExecutionDelay.ToSeconds(), CheckDirectoriesDelay.ToSeconds());
            LogCF(overallDelayMessage);

            string executionDelayMessage = string.Format(" ExecutionDelays (in seconds): DocLoad={0}, DocParse={1} StructureInit={2} BraillMusicGeneration={3}",
            DocLoadDelay.ToSeconds(), DocParseDelay.ToSeconds(), StructureInitDelay.ToSeconds(), BrailleMusicGenerationDelay.ToSeconds());
            LogCF(executionDelayMessage);

        }
        #endregion

        static private string currentMusicXmlFile;
        static public string CurrentMusicXmlFile { get { return (null == currentMusicXmlFile) ? "" : currentMusicXmlFile; } set { currentMusicXmlFile = value; } }


        static private string currentMusicXmlPath;
        static public string CurrentMusicXmlPath { get { return (null == currentMusicXmlPath) ? "" : currentMusicXmlPath; } set { currentMusicXmlPath = value; } }

        static private int currentMusicBrailleMeasureNumber;
        /// <summary>
        /// Exclusively used for logging and debuggging
        /// </summary>
        static public int CurrentMusicBrailleMeasureNumber { get { return currentMusicBrailleMeasureNumber; }  set{ currentMusicBrailleMeasureNumber = value; } } 


        static private Int64 globalCount = 0;
        static public Int64 GlobalCount { get { return globalCount; } }

        static public string GetCallingMethod()
        {
            return GetCallingMethodInternalImplementation(2); //  because we use this extra level for calling GetCallingMethod(int levels) !
        }

        static public string GetCallingMethod(int extraLevels)
        {
            return GetCallingMethodInternalImplementation(2 + extraLevels); //  because we use this extra level for calling GetCallingMethod(int levels) !
        }


        static public string GetCallingMethodInternalImplementation(int levels)
        {
            try
            {
                StackTrace stackTrace = new StackTrace();
                MethodBase methodBase = stackTrace.GetFrame(levels + 1).GetMethod(); // "levels+1" in order to compensate for calling "GetCallingMethod()"
                Type type = methodBase.ReflectedType;
                return string.Format("{0}.{1}", type.Name, methodBase.Name);
            }
            catch (Exception e)
            {
                return string.Format("Logger.GetCallingMethod threw an exception: {0}", e.ToString());
            }
        }



        // Used for instance for appending the name of the .xml file under test when running a test application
        static private string postString = null;
        static public string PostString
        {
            get
            {
                return (postString == null) ? "" : postString;
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
            set
            {
                currentEncoding = value;
                LogCF(string.Format(": CurrentEncoding = {0}", currentEncoding));
            }
        }


        static private string currentMusicBrailleSourceFileName;
        /// <summary>
        /// Mechanism for communicating filename throughout all code. For logging purposes only
        /// </summary>
        static public string CurrentMusicBrailleSourceFileName { get { string s = currentMusicBrailleSourceFileName; return (s == null) ? "" : s; } set { currentMusicBrailleSourceFileName = value; } }


        //static private int decoderOptions;
        //static public int DecoderOptions { get { return decoderOptions; } set { decoderOptions = value; } }


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
        private static LoggerCounters localCounters = LoggerCounters.Create();  // For counting log lines local to one MusicXml file  (Used by all applications) 
        private static LoggerCounters globalCounters = LoggerCounters.Create(); // For counting a sum over a number og MusicXml files (Used for instance by  MusicXmlReaderCmd).
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
                && (new FileInfo(logFileFullName)).Length > maxLogfileLength))
                {
                    // Delete the existing "Old" logfile and mark the current logfile to "Old" by renaming it
                    if (File.Exists(oldLogFileFullName))
                    {
                        File.Delete(oldLogFileFullName);
                    }
                    File.Move(logFileFullName, oldLogFileFullName); //Actually a rename !
                }

//                localCounters = LoggerCounters.Create();
//                globalCounters = LoggerCounters.Create();

                result = true;
            }
            catch (Exception e)
            {
                string s = e.Message; // For debugging
                // But what can we do ?
            }
            return result;
        }

        private static bool showTimeStampInLog = true; 
        public static bool ShowTimeStampInLog { get { return showTimeStampInLog; } set { showTimeStampInLog = value; } }


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


        private static StringBuilder cachedLines = null;
        public static void StartCaching()
        {
            Log("StartCaching"); // Log BEFORE changing state
            // Start caching  Lines instead of flushing them to disk
            cachedLines = new StringBuilder(1024 * 1024); // We might as well set aside 1MB from the start ! 
            useConsole = false;
        }

        public static void EndCaching()
        {
            int numberOfChars = 0;
            if (null != cachedLines)
            {
                numberOfChars = cachedLines.Length;
                System.IO.File.AppendAllText(logFileFullName, cachedLines.ToString());
            }
            cachedLines = null;
            useConsole = true;
            Log(string.Format("EndCaching ({0} characters)", numberOfChars)); // Log AFTER changing state
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
            Log(s, showTimeStampInLog);
        }

        public static void LogCFE(Exception e)
        {
            StackTrace stackTrace = new StackTrace();
            MethodBase methodBase = stackTrace.GetFrame(1).GetMethod();
            Type type = methodBase.ReflectedType;
            string message = string.Format("{0}.{1}: Exception: Message='{2}' StackTrace=\r\n{3}", type.Name, methodBase.Name, e.Message, e.StackTrace.ToString());
            Log(message);
            LogOnce(message);
        }

        /// <summary>
        /// Same as Log, but automatically adds ClassName and FunctionNAme of the calling function.
        /// </summary>
        /// <param name="s"></param>
        public static void LogCF(string s)
        {
#if false
            StackTrace stackTrace = new StackTrace();
            MethodBase methodBase = stackTrace.GetFrame(1).GetMethod();
            Type type = methodBase.ReflectedType;
            //string Namespace = type.Namespace;
            Log(string.Format("{0}.{1}{2}", type.Name, methodBase.Name, s));
#else
            Log(string.Format("{0}{1}", GetCallingMethod(), s));
#endif
            //Console.WriteLine(Namespace + "." + Class.Name + "." + methodBase.Name);
        }

        /// <summary>
        /// Same as LogCF(s) except that the resulting string is returned, not logged.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string GetCF(string s)
        {
            string timeStamp = GetLogTime(showTimeStampInLog);
            return string.Format("{0} {1}{2}",timeStamp, GetCallingMethod(), s);
        }


        /// <summary>
        /// Same as LogCF1(s) except that the resulting string is returned, not logged.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string GetCF1(string s)
        {
            string timeStamp = GetLogTime(showTimeStampInLog);
            return string.Format("{0} {1}{2}", timeStamp, GetCallingMethod(1), s);      
        }

        /// <summary>
        /// Same as LogCF, but reports the ClassName and MethodName of the method one extra level up the stack.
        /// Useful for centralizing all calls to Logger.LogCF at one single place in a class and its derived classes.
        /// </summary>
        /// <param name="s">String to be logged</param>
        public static void LogCF1(string s)
        {
            Log(string.Format("{0}{1}", GetCallingMethod(1), s));
        }


        /// <summary>
        /// Same as LogOnce, but automatically adds ClassName and FunctionNAme of the calling function.
        /// </summary>
        /// <param name="s"></param>
        public static void LogCFOnce(string s)
        {
#if false
            StackTrace stackTrace = new StackTrace();
            MethodBase methodBase = stackTrace.GetFrame(1).GetMethod();
            Type type = methodBase.ReflectedType;
            //string Namespace = type.Namespace;
            LogOnce(string.Format("{0}.{1}{2}", type.Name, methodBase.Name, s));
#else
            LogOnce(string.Format("{0}{1}", GetCallingMethod(), s));
#endif
            //Console.WriteLine(Namespace + "." + Class.Name + "." + methodBase.Name);
        }

        private static string GetLogTime(bool show)
        {
            if (!show) return "";
            System.DateTime now = System.DateTime.Now;
            string time = string.Format("{0}.{1:D3}", now.ToLongTimeString(), now.Millisecond); // Always use 3 digits for milliseconds
            return time;
        }


        private static void Log(string s, bool showTimeStamp)
        {
            globalCount++; // Increment a global counter, usable for diagnostics
            if (string.IsNullOrEmpty(logFileFullName)) return; // Open() must be called before using the Logger !      
                                                               // Several threads may use the Log file, but we do not want an explicit critical region
            string exceptionMessage = null;
            bool signaled = false;
            try
            {
                Trace(s); // We will se repeated lines in the Trace if the file operation failes !
                string time = GetLogTime(showTimeStamp); 
                // string r = (0 == i) ? "" : string.Format("R={0} ", i); // Illustrate that  the file write operation has been retried R times
                string line = time + " " + s + "\r\n";
                signaled = mutex.WaitOne(1000); // Wait up to 1000 mS 
                if (null == cachedLines)
                {
                    System.IO.File.AppendAllText(logFileFullName, line);
                }
                else
                {
                    cachedLines.Append(line); // Much better performance
                }
                numberOfLogLines++;
                mutex.ReleaseMutex();
            }
            catch (Exception e)
            {
                exceptionMessage = e.Message;
            }

            // If in developermode we add a message to the Trace in order to investigate any problem, otherwise we attempt to ignore the problem !           
            if (developerMode && ((null != exceptionMessage) || (!signaled)))
            {
                string message = string.Format("--->>> Logger.Log: DeveloperMode={0} Signaled={1} ExceptionMessage={2} <<<---", developerMode, signaled, NullAsText(exceptionMessage));
                Trace(message);
            }
        }

        private static string NullAsText(string s)
        {
            if (null == s) return "null";
            return s;
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
            Log(string.Format("Logger.DumpStatistics completed with a total of {0} entries", numberOfEntries));
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

        public static void LogArguments(string[] arguments)
        {
            Log("Command line argument:");
            for (int i = 0 ; (i < arguments.Length); i++) 
            {
                Log(string.Format("[{0}] {1}",i, arguments[i]));
            }
        }


        public static void LogDebuggerAttachment()
        {
            string methodName = "LogDebuggerAttachment";
            bool b = System.Diagnostics.Debugger.IsAttached;
            Log(string.Format("{0}.{1} reported {2}", className, methodName, b));
        }


    }
}
