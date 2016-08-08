using System;
using System.Runtime.InteropServices;

namespace JSJ.ScreenReaderAPI
{

    public class NvdaControlerClient64Wrapper : ScreenReaderAPI
    {
        private IScreenReaderAPILogger logger;
        internal IScreenReaderAPILogger Logger
        {
            set
            {
                logger = value;
            }
        }

        static public NvdaControlerClient64Wrapper Create()
        {
            bool found = false;
            try
            {
                found = (0 == nvdaController_testIfRunning());
            }
            catch (Exception)
            {
            }
            return found ? new NvdaControlerClient64Wrapper() : null;
        }

        // Prevent construction
        private NvdaControlerClient64Wrapper()
        {
        }

        public override bool Speak(string s)
        {
            return (0 == nvdaController_speakText(s));
        }

        public override bool Braille(string s)
        {
            return (0 == nvdaController_brailleMessage(s));
        }

        public override bool Silence()
        {
            return (0 == nvdaController_cancelSpeech());
        }
        
        public override string GetScreenReaderDllName()
        {
            return "nvdaControllerClient64.dll";
        }
        
        /// <summary>
        /// For this class to work as expected, a 32-bit application should define a conditional variable named x86.
        /// Also, the NVDA API should exist in the same directory as the executable. 32-bit applications should use nvdaControllerClient32.dll and 64-bit applications should use nvdaControllerClient64.dll.
        /// </summary>

        [DllImport("nvdaControllerClient64.dll", CharSet = CharSet.Unicode)]
        private static extern int nvdaController_testIfRunning();

        [DllImport("nvdaControllerClient64.dll", CharSet = CharSet.Unicode)]
        private static extern int nvdaController_speakText(String text);

        [DllImport("nvdaControllerClient64.dll", CharSet = CharSet.Unicode)]
        private static extern int nvdaController_brailleMessage(String braille);

        [DllImport("nvdaControllerClient64.dll", CharSet = CharSet.Unicode)]
        private static extern int nvdaController_cancelSpeech();
    }

}
