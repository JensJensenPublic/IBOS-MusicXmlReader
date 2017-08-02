using System;
using System.Runtime.InteropServices;

namespace JSJ.ScreenReaderAPI
{

    public class NvdaControlerClient32Wrapper : ScreenReaderAPI
    {
        static private string className = "NvdaControlerClient32Wrapper";
        static public NvdaControlerClient32Wrapper Create(IScreenReaderAPILogger logger)
        {
            string methodName = "Create";
            bool found = false;
            try
            {
                found = (0 == NativeMethods.Nvda32nvdaController_testIfRunning());
            }
            catch (Exception e)
            {
                LogException(logger, className, methodName, "nvdaController_testIfRunning()", e.Message);
            }
            return found ? new NvdaControlerClient32Wrapper(logger) : null;
        }

        // Prevent construction
        private NvdaControlerClient32Wrapper()
        {
            // Start the thread used for refreshing the display
            StartBrailleDisplayThread();
        }

        protected NvdaControlerClient32Wrapper(IScreenReaderAPILogger logger) : base(logger)
        {
            // Start the thread used for refreshing the display
            StartBrailleDisplayThread();
        }

        protected override bool SpeakImplementation(string s)
        {
            return (0 == NativeMethods.Nvda32nvdaController_speakText(s));
        }

        protected override bool BrailleImplementation(string s)
        {
            return (0 == NativeMethods.Nvda32nvdaController_brailleMessage(s));
        }

        protected override bool SilenceImplementation()
        {
            return (0 == NativeMethods.Nvda32nvdaController_cancelSpeech());
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


    }
}