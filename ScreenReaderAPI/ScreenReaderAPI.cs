using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace JSJ.ScreenReaderAPI
{
    /// <summary>
    /// Class for transparent access to various screenreader APIs: JAWS, NVDA etc
    /// Inspired by the "Tolk" project by Davy Kager
    /// </summary>
    public abstract class ScreenReaderAPI
    {
        static string className = "ScreenReaderAPI";
        protected bool refreshing = false;
        private IScreenReaderAPILogger logger;
        protected IScreenReaderAPILogger Logger { get { return logger; } }
        private string screenReaderName = "";
        public string ScreenReaderName
        {
            get
            {
                return screenReaderName;
            }

        }

        public enum ScreenReaderType { Dummy, JAWS, NVDA };

        protected bool StartBrailleDisplayThread()
        {
#if false
            // If we start the brailleDisplayThread we must explicitly stop it again when the program exits!
            // Otherwise we will leave an instance of MusicXmlReader running as a background process.
            // For the time being we do not start the brailleDisplayThread in order to avoid this.
            // If the brailleDisplayThread is really needed we must start it and implement a mechanism for stopping it again !
            return false;
#else
            // Start a thread used for refreshing the display
            brailleDisplayThread = new Thread(new ThreadStart(DisplayerThreadStart));
            Log(string.Format("Starting BrailleDiaplayThread at priority={0}", brailleDisplayThread.Priority.ToString()));
            brailleDisplayThread.Start();
            return true;
#endif
        }


        /// <summary>
        /// Create a common API for JAWS and NVDA for 32Bit and 64Bit implementations
        /// </summary>
        /// <returns></returns>
        public static ScreenReaderAPI Create(bool is64Bit, IScreenReaderAPILogger logger)
        {
            string methodName = "Create"; 
            ScreenReaderAPI screenReaderAPI = null;

            // First check if JAWS is available
            screenReaderAPI = (is64Bit) ? (ScreenReaderAPI)JfwApiWrapper.Create(logger) : FSAPIWrapper.Create(logger);

            // Secondly check if NVDA is available
            // NOTE ! For the time being the program crashes when running on NVDA under the Visual Studio debugger
#warning    // TO DO: Make the program run on NVDA under the Visual Studio Debugger
            if (null == screenReaderAPI)
            {
                screenReaderAPI = (is64Bit) ? (ScreenReaderAPI)NvdaControlerClient64Wrapper.Create(logger) : NvdaControlerClient32Wrapper.Create(logger);
                if ((null != logger) && (null != screenReaderAPI))
                {
                    logger.LogEvent(string.Format("{0}.{1}: Detected NVDA. Crashes under the Visual Studio Debugger, but seems to work when run outside debugger.", className, methodName));
                }
            }

            //
            // Insert checks for more screen readers here...
            //

            // As a last resort create a dummy  (in order to simplify application code!)
            if (null == screenReaderAPI)
            {
                screenReaderAPI = DummyScreenReader.Create(logger);
                if (null != logger) logger.LogEvent(string.Format("{0}.{1}: Created Dummy ScreenReaderAPI",className,methodName));
            }
            else
            {
                if (null != logger) logger.LogEvent(string.Format("{0}.{1}: Created ScreenReaderAPI for {2} using {3} ",
                                                                    className, methodName, screenReaderAPI.GetScreenReaderNameImplementation(), screenReaderAPI.GetScreenReaderDllNameImplementation()));
            }


            screenReaderAPI.screenReaderName = screenReaderAPI.GetScreenReaderNameImplementation();

            // screenReaderAPI.LogError("For test only!", false);
            // screenReaderAPI.LogException("For test only!", new Exception("For test only!"));

            return screenReaderAPI;
        }

        // Prevent construction
        protected ScreenReaderAPI() { }

        protected ScreenReaderAPI(IScreenReaderAPILogger logger)
        {
            // Attach the logger specified
            this.logger = logger;
        }


        // All screanreader API-implementations must implement the following methods:
        protected abstract bool SpeakImplementation(string s);
        protected abstract bool BrailleImplementation(string s);
        protected abstract bool SilenceImplementation();
        protected abstract string GetScreenReaderNameImplementation();     // Primarily for debugging
        protected abstract string GetScreenReaderDllNameImplementation();  // Primarily for debugging
        protected abstract ScreenReaderType GetScreenReaderTypeImplementation();  // Primarily for debugging

        // And now for the public methods. Most of the code is boilerplate exception handling, centralized here!

        private void LogException(string function, Exception e)
        {
            int lastWin32Error = Marshal.GetLastWin32Error();
            Logger.LogEvent(string.Format("{0}.{1} threw an exception with Message='{2}' Marshal.GetLastWin32Error()={3}",
                                           screenReaderName, function, e.Message, lastWin32Error));
        }

        private bool LogError(string function, bool ok)
        {
            
            if (ok)
            {
                // Logger.LogEvent(string.Format("{0}.{1} returned ok", screenReaderName,function));
            }
            else
            {
                int lastWin32Error = Marshal.GetLastWin32Error();
                Log(string.Format("{0}.{1} failed. Marshal.GetLastWin32Error()={2}", screenReaderName, function, lastWin32Error));
            }
            return ok;
        }

  

        static protected void LogException(IScreenReaderAPILogger logger, string className, string methodName, string nativeMethodName, string exceptionMessage)
        {
            if (null != logger)
            {
                logger.LogEvent(string.Format("{0}.{1}: {2} threw an exception. Message={3}", className, methodName, nativeMethodName, exceptionMessage));
            }
        }

        public bool Speak(string s)
        {
            string function = "Speak";
            bool result = false;
            // Calls into native code!
            try
            {
                result = LogError(function,SpeakImplementation(s));
            }
            catch (Exception e)
            {
                LogException(function, e);
            }
            return result;
        }

        public bool Braille(string s)
        {
            string function = "Braille";
            bool result = false;
            // Calls into native code!
            try
            {
                result = LogError(function,BrailleImplementation(s));
            }
            catch (Exception e)
            {
                LogException(function, e);
            }
            return result;
        }

        public bool Silence()
        {
            string function = "Silence";
            bool result = false;
            // Calls into native code!
            try
            {
                result = LogError(function,SilenceImplementation());
            }
            catch (Exception e)
            {
                LogException(function, e);
            }
            return result;
        }

        public string GetScreenReaderName()
        {
            return this.GetScreenReaderNameImplementation();
        }

        public string GetScreenReaderDllName()
        {
            return this.GetScreenReaderDllNameImplementation();
        }

        public ScreenReaderType GetScreenReaderType()
        {
            return this.GetScreenReaderTypeImplementation();
        }


        public bool Braille(string s, bool refresh)
        {
            latestMessage = s;
            refreshing = refresh;
            return this.Braille(latestMessage);
        }

        public void StopRefreshing()
        {
            refreshing = false;
        }

        private Thread brailleDisplayThread;
        private bool displaying = true;
        protected string latestMessage = null; // Latest message sent to Braille display

        /// <summary>
        /// Thread needed for refreshing the MusicBraille Message sent to the Braille Display to prevent it from being overwritten by LyricBraille
        /// </summary>
        private void DisplayerThreadStart()
        {
            while (displaying)
            {
                Thread.Sleep(1000); // Refresh the display every second as long as needed
                {
                    if (refreshing)
                    {
                        TraceChar('+'); // Shows that we are refreshing
                        BrailleImplementation(latestMessage);
                    }
                    else
                    {
                        TraceChar('-'); // Shows that we are not refreshing
                    }
                }
            }
        }

        public bool OnApplicationExit()
        {
            string functionName = "OnApplicationExit";
            Log(string.Format("{0}.{1}", className, functionName));
            displaying = false; // Stops the DisplayerThread
            return true;
        }

#region LogAndTrace
        private void Log(string s)
        {
            if (null == Logger) return;
            Logger.LogEvent(s);
        }

        private void LogEvent(string s)
        {
            if (null == Logger) return;
            Logger.LogEvent(s);
        }

        private void TraceChar(char c)
        {
            if (null == Logger) return;
            Logger.TraceChar(c);
        }
#endregion

        //// Only for error reporting
        //[DllImport("kernel32.dll")]
        //static extern uint GetLastWin32Error();


        // JSJ: Start of the original Tolk API ---------------------------------------------------------------------------------------
        //// Prevent construction
        //private Tolk() { }

        //public static void Load() { Tolk_Load(); }
        //public static bool IsLoaded() { return Tolk_IsLoaded(); }
        //public static void Unload() { Tolk_Unload(); }
        //public static void TrySAPI(bool trySAPI) { Tolk_TrySAPI(trySAPI); }
        //public static void PreferSAPI(bool preferSAPI) { Tolk_PreferSAPI(preferSAPI); }
        //// Prevent the marshaller from freeing the unmanaged string
        //public static String DetectScreenReader() { return Marshal.PtrToStringUni(Tolk_DetectScreenReader()); }
        //public static bool HasSpeech() { return Tolk_HasSpeech(); }
        //public static bool HasBraille() { return Tolk_HasBraille(); }
        //public static bool Output(String str, bool interrupt = false) { return Tolk_Output(str, interrupt); }
        //public static bool Speak(String str, bool interrupt = false) { return Tolk_Speak(str, interrupt); }
        //public static bool Braille(String str) { return Tolk_Braille(str); }
        //public static bool IsSpeaking() { return Tolk_IsSpeaking(); }
        //public static bool Silence() { return Tolk_Silence(); }
        // JSJ: End of the original Tolk API ---------------------------------------------------------------------------------------

    }

}
