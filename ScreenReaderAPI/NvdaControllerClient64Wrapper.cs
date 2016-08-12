using System;
using System.Runtime.InteropServices;

namespace JSJ.ScreenReaderAPI
{

    public class NvdaControlerClient64Wrapper : ScreenReaderAPI
    {

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

        protected override bool SpeakImplementation(string s)
        {
            return (0 == nvdaController_speakText(s));
        }

        protected override bool BrailleImplementation(string s)
        {
            return (0 == nvdaController_brailleMessage(s));
        }

        protected override bool SilenceImplementation()
        {
            return (0 == nvdaController_cancelSpeech());
        }

        protected override string GetScreenReaderNameImplementation()
        {
            return "NVDA";
        }

        protected override string GetScreenReaderDllNameImplementation()
        {
            return "nvdaControllerClient64.dll";
        }

        protected override ScreenReader GetScreenReader()
        {
            return ScreenReader.NVDA;
        }
        
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
