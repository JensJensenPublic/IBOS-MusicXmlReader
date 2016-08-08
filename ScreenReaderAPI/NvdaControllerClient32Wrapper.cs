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
            return "nvdaControllerClient32.dll";
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