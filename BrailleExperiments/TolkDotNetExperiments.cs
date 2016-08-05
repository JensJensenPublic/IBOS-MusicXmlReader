using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrailleExperiments
{

    public abstract class TolkDotNet
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
        public static TolkDotNet Create()
        {

            bool is64Bit = (IntPtr.Size == 8); // Find out wheather we are compiled for 32 bit or 64 bit architechture.
            string architechture = is64Bit ? " (64 bit)" : " (32 bit)";

            // First check if JAWS is running 
            TolkDotNet tolkDotNet;
            if (is64Bit)
            {
                tolkDotNet = JfwApiWrapperExperiments.Create();
            }
            else
            {
                tolkDotNet = FSAPIWrapperExperiments.Create();
            }
            if (null != tolkDotNet)
            {
                screenReaderName = "JAWS" + architechture;    
                return tolkDotNet;
            }

            // Secondly check if NVDA is running
            if (is64Bit)
            {
                tolkDotNet = NvdaControlerClient64WrapperExperiments.Create();
            }
            else
            {
                tolkDotNet = NvdaControlerClientWrapperExperiments.Create();
            }

            if (null != tolkDotNet)
            {
                screenReaderName = "NVDA"+architechture;
                return tolkDotNet;
            }

            // Insert checks for more screen readers here...

            return null;
        }

        // Prevent construction
        protected TolkDotNet() { }

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


