using System;

namespace JSJ.ScreenReaderAPI
{
    /// <summary>
    /// Class for transparent access to various screenreader APIs: JAWS, NVDA etc
    /// Inspired by the "Tolk" project by Davy Kager
    /// </summary>
    public abstract class ScreenReaderAPI
    {

        private static string screenReaderName = "";
        public static string ScreenReaderName
        {
            get
            {
                return screenReaderName;
            }
        }

        /// <summary>
        /// Create a common API for JAWS and NVDA for 32Bit and 64Bit implementations
        /// </summary>
        /// <returns></returns>
        public static ScreenReaderAPI Create()
        {

            bool is64Bit = (IntPtr.Size == 8); // Find out wheather we are compiled for 32 bit or 64 bit architechture.
            string architechture = is64Bit ? " (64 bit)" : " (32 bit)";

            // First check if JAWS is running 
            ScreenReaderAPI screenReaderAPI;
            if (is64Bit)
            {
                screenReaderAPI = JfwApiWrapper.Create();
            }
            else
            {
                screenReaderAPI = FSAPIWrapper.Create();
            }
            if (null != screenReaderAPI)
            {
                screenReaderName = "JAWS" + architechture;
                return screenReaderAPI;
            }

            // Secondly check if NVDA is running
            if (is64Bit)
            {
                screenReaderAPI = NvdaControlerClient64Wrapper.Create();
            }
            else
            {
                screenReaderAPI = NvdaControlerClient32Wrapper.Create();
            }

            if (null != screenReaderAPI)
            {
                screenReaderName = "NVDA" + architechture;
                return screenReaderAPI;
            }

            // Insert checks for more screen readers here...

            return null;
        }

        // Prevent construction
        protected ScreenReaderAPI() { }

        // Primarily for debugging:
        public abstract string GetScreenReaderDllName();

        // All screanreader wrappers must implement the following methods:
        public abstract bool Speak(string s);
        public abstract bool Braille(string s);
        public abstract bool Silence();


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
