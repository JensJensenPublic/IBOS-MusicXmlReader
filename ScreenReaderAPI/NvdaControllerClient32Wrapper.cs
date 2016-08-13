using System;
using System.Runtime.InteropServices;

namespace JSJ.ScreenReaderAPI
{

    public class NvdaControlerClient32Wrapper : ScreenReaderAPI
    {

        static public NvdaControlerClient32Wrapper Create()
        {
            bool found = false;
            try
            {
                found = (0 == nvdaController_testIfRunning());
            }
            catch (Exception)
            {
            }
            return found ? new NvdaControlerClient32Wrapper() : null;
        }

        // Prevent construction
        private NvdaControlerClient32Wrapper()
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
            return "nvdaControllerClient32.dll";
        }

        protected override ScreenReaderType GetScreenReaderTypeImplementation()
        {
            return ScreenReaderType.NVDA;
        }


        [DllImport("nvdaControllerClient32.dll", CharSet = CharSet.Unicode)]
        public static extern int nvdaController_testIfRunning();

        [DllImport("nvdaControllerClient32.dll", CharSet = CharSet.Unicode)]
        public static extern int nvdaController_speakText(String text);

        [DllImport("nvdaControllerClient32.dll", CharSet = CharSet.Unicode)]
        public static extern int nvdaController_brailleMessage(String braille);

        [DllImport("nvdaControllerClient32.dll", CharSet = CharSet.Unicode)]
        public static extern int nvdaController_cancelSpeech();
    }
}