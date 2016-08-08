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

        public override bool Speak(string s)
        {
            return (0 == nvdaController_speakText(s));
        }

        public override bool Braille(string s)
        {
            latestMessage = s;
            return (0 == nvdaController_brailleMessage(s));
        }

        public override bool Silence()
        {
            return (0 == nvdaController_cancelSpeech());
        }

        public override string GetScreenReaderName()
        {
            return "NVDA";
        }

        public override string GetScreenReaderDllName()
        {
            return "nvdaControllerClient64.dll";
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
