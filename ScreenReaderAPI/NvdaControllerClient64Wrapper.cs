using System;
using System.Runtime.InteropServices;

namespace JSJ.ScreenReaderAPI
{


    public class NvdaControlerClient64Wrapper : ScreenReaderAPI
    {
        static private string className = "NvdaControlerClient64Wrapper";
        static public NvdaControlerClient64Wrapper Create(IScreenReaderAPILogger logger)
        {
            string methodName = "Create";
            bool found = false;
            try
            {
                found = (0 == NativeMethods.Nvda64nvdaController_testIfRunning());
            }
            catch (Exception e)
            {
                LogException(logger,className, methodName, "nvdaController_testIfRunning()", e.Message);
            }
            return found ? new NvdaControlerClient64Wrapper(logger) : null;
        }

        // Prevent construction
        private NvdaControlerClient64Wrapper()
        {
            // Start the thread used for refreshing the display
            StartBrailleDisplayThread();
        }

        private NvdaControlerClient64Wrapper(IScreenReaderAPILogger logger) : base(logger)
        {
            // Start the thread used for refreshing the display
            StartBrailleDisplayThread();
        }

        protected override bool SpeakImplementation(string s)
        {
            return (0 == NativeMethods.Nvda64nvdaController_speakText(s));
        }

        protected override bool BrailleImplementation(string s)
        {
            return (0 == NativeMethods.Nvda64nvdaController_brailleMessage(s));
        }

        protected override bool SilenceImplementation()
        {
            return (0 == NativeMethods.Nvda64nvdaController_cancelSpeech());
        }

        protected override string GetScreenReaderNameImplementation()
        {
            return "NVDA";
        }

        protected override string GetScreenReaderDllNameImplementation()
        {
            return "nvdaControllerClient64.dll";
        }

        protected override ScreenReaderType GetScreenReaderTypeImplementation()
        {
            return ScreenReaderType.NVDA;
        }
        
    }

}
