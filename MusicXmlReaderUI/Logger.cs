using System;
using System.Collections.Generic;
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
        private static bool useConsole = false;

     
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

        public static string LogFileName
        {
            get
            {
                return logFileName;
            }

            set
            {
                logFileName = value;
            }
        }

        public static string LogFileFullName
        {
            get
            {
                return System.IO.Path.Combine(System.IO.Path.GetTempPath(), LogFileName);
            }
 
        }

        public static void Log(string s)
        {
            try
            {
                Trace(s);
                System.DateTime now = System.DateTime.Now;
                string time = string.Format("{0}.{1,03}", now.ToLongTimeString(), now.Millisecond.ToString()); // Always use 3 digits for milliseconds
                System.IO.File.AppendAllText(System.IO.Path.Combine(System.IO.Path.GetTempPath(), LogFileName), time + " " + s + "\r\n");
            }
            catch (Exception)
            {
                // What to do here ??
            }
        }


        // Two parallel lists:
        private static List<string> strings = new List<string>();
        private static List<int> counters = new List<int>();


        /// <summary>
        /// Same as Log() but each string is only logged once !
        /// Instead a statistics is kept for counting how many times the string is logged.
        /// </summary>
        /// <param name="s"></param>
        public static void LogOnce(string s)
        {
            bool found = false;  
            for (int i = 0; ((i < strings.Count) && !found); i++)
            {
                if (0 == strings[i].CompareTo(s))
                {
                    // The new string is already in the list
                    (counters[i])++;
                    found = true;
                }
            }
            if (!found)
            {
                strings.Add(s);
                counters.Add(1); // Count this occurrance
                Log(s);
            }
        }

        public static void ClearStatistics()
        {
            strings = new List<string>();
            counters = new List<int>();
        }

        /// <summary>
        /// Dumps all strings used as afgument to LogOnce with the number of times it has been called
        /// since last call to ClearStatistics()
        /// </summary>
        public static void DumpStatistics()
        {
            Log("Logger.DumpStatistics start");
            for (int i = 0; (i < strings.Count); i++)
            {
                Log(string.Format("  {0}:{1}", strings[i], counters[i])); // Indent by 2 positions
            }
            Log("Logger.DumpStatistics end");
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
