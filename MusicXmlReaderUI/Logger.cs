using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MusicXmlReaderUI
{
    static class Logger
    {



        // To use a console in a Windows Forms application: Project Properties -> Application -> Output Type -> Console Application
        // "Original value was "Windows Application"
        public static void Trace(string s)
        {
            if (useConsole) Console.WriteLine(s);
        }

        public static string LogFileName = "MusicXmlReader.Log";
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



        public static void LogSystemInformation()
        {
            Log(string.Format("ComputerName={0} UserName={1} UserDomainName={2}",
                SystemInformation.ComputerName, SystemInformation.UserName, SystemInformation.UserDomainName));
            Log(string.Format("OSVersion={0} ProcessorCount={1} Is64BitOperatingSystem={2} Is64BitProcess={3}",
            System.Environment.OSVersion, System.Environment.ProcessorCount, System.Environment.Is64BitOperatingSystem, System.Environment.Is64BitProcess));
        }




    }
}
