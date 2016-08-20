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
        protected bool refreshing = false;
        protected IScreenReaderAPILogger Logger;
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
            // Start a thread used for refreshing the display
            brailleDisplayThread = new Thread(new ThreadStart(DisplayerThreadStart));
            Log(string.Format("Starting BrailleDiaplayThread at priority={0}", brailleDisplayThread.Priority.ToString()));
            brailleDisplayThread.Start();
            return true;
        }


        /// <summary>
        /// Create a common API for JAWS and NVDA for 32Bit and 64Bit implementations
        /// </summary>
        /// <returns></returns>
        public static ScreenReaderAPI Create(bool is64Bit, IScreenReaderAPILogger logger)
        {
            ScreenReaderAPI screenReaderAPI;

            // First check if JAWS is available
            screenReaderAPI = (is64Bit) ? (ScreenReaderAPI)JfwApiWrapper.Create() : FSAPIWrapper.Create();

            // Secondly check if NVDA is available
            if (null == screenReaderAPI)
            {
                screenReaderAPI = (is64Bit) ? (ScreenReaderAPI)NvdaControlerClient64Wrapper.Create() : NvdaControlerClient32Wrapper.Create();
            }

            //
            // Insert checks for more screen readers here...
            //

            // As a last resort create a dummy  (in order to simplify application code!)
            if (null == screenReaderAPI)
            {
                screenReaderAPI = DummyScreenReader.Create();
                if (null != logger) logger.LogEvent("Created Dummy ScreenReaderAPI");
            }
            else
            {
                if (null != logger) logger.LogEvent(string.Format("Created ScreenReaderAPI for {0} using {1} ", screenReaderAPI.GetScreenReaderNameImplementation(), screenReaderAPI.GetScreenReaderDllNameImplementation()));
            }

            // Attach the logger specified to the newly created ScreenReaderAPI
            screenReaderAPI.Logger = logger;

            screenReaderAPI.screenReaderName = screenReaderAPI.GetScreenReaderNameImplementation();
            return screenReaderAPI;
        }

        // Prevent construction
        protected ScreenReaderAPI() { }


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
            Logger.LogEvent(string.Format("{0}.{1} threw an exception with Message='{2}' GetLastWin32Error={3}",
                                           screenReaderName, function, e.Message, GetLastWin32Error()));
        }

        private bool LogError(string function, bool ok)
        {
            
            if (ok)
            {
                // Logger.LogEvent(string.Format("{0}.{1} returned ok", screenReaderName,function));
            }
            else
            { 
                Log(string.Format("{0}.{1} failed. GetLastWin32Error={2}", screenReaderName, function, GetLastWin32Error()));
            }
            return ok;
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

        // Only for error reporting
        [DllImport("kernel32.dll")]
        static extern uint GetLastWin32Error();


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
