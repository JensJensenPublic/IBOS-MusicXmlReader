using System;
using System.Runtime.InteropServices;

namespace JSJ.ScreenReaderAPI
{
    /// <summary>
    /// Collects all DllImports into one single class "NAtiveMethods" to please the Visual Studion Analyze facility.
    /// The "EntryPoint" parameter is needed to call the same entry point in different dlls
    /// </summary>
    internal static class NativeMethods
    {

        #region fsapi
        [DllImport("fsapi.dll", EntryPoint = "JFWStopSpeech", CharSet = CharSet.Ansi)]
        internal static extern bool FsApiJFWStopSpeech();

        [DllImport("fsapi.dll", EntryPoint = "JFWSayString",  CharSet = CharSet.Unicode)]            // Says the string, then crashes with an unbalanced stack
        internal static extern bool FsApiJFWSayString(String text);

        [DllImport("fsapi.dll", EntryPoint = "JFWRunFunction", CharSet = CharSet.Unicode)]
        internal static extern bool FsApiJFWRunFunction(String text);

        //[DllImport("fsapi.dll", EntryPoint = "JFWRunFunction", CharSet = CharSet.Unicode)]   
        //internal static extern bool FsApiJFWRunFunction(String function, String param1); // Probably not needed !
        #endregion

        #region jfwapi
        [DllImport("jfwapi.dll", EntryPoint = "JFWStopSpeech", CharSet = CharSet.Ansi)]
        internal static extern bool JfwApiJFWStopSpeech();

        [DllImport("jfwapi.dll", EntryPoint = "JFWSayString", CharSet = CharSet.Unicode)]
        internal static extern bool JfwApiJFWSayString(String text);

        [DllImport("jfwapi.dll", EntryPoint = "JFWRunFunction", CharSet = CharSet.Unicode)]
        internal static extern bool JfwApiJFWRunFunction(String text);
        #endregion


        #region NVDA32
        [DllImport("nvdaControllerClient32.dll", EntryPoint = "nvdaController_testIfRunning", CharSet = CharSet.Unicode)]
        internal static extern int Nvda32nvdaController_testIfRunning();

        [DllImport("nvdaControllerClient32.dll", EntryPoint = "nvdaController_speakText", CharSet = CharSet.Unicode)]
        internal static extern int Nvda32nvdaController_speakText(String text);

        [DllImport("nvdaControllerClient32.dll", EntryPoint = "nvdaController_brailleMessage", CharSet = CharSet.Unicode)]
        internal static extern int Nvda32nvdaController_brailleMessage(String braille);

        [DllImport("nvdaControllerClient32.dll", EntryPoint = "nvdaController_cancelSpeech", CharSet = CharSet.Unicode)]
        internal static extern int Nvda32nvdaController_cancelSpeech();
        #endregion

        #region NVDA64
        [DllImport("nvdaControllerClient64.dll", EntryPoint = "nvdaController_testIfRunning", CharSet = CharSet.Unicode)]
        internal static extern int Nvda64nvdaController_testIfRunning();

        [DllImport("nvdaControllerClient64.dll", EntryPoint = "nvdaController_speakText", CharSet = CharSet.Unicode)]
        internal static extern int Nvda64nvdaController_speakText(String text);

        [DllImport("nvdaControllerClient64.dll", EntryPoint = "nvdaController_brailleMessage", CharSet = CharSet.Unicode)]
        internal static extern int Nvda64nvdaController_brailleMessage(String braille);

        [DllImport("nvdaControllerClient64.dll", EntryPoint = "nvdaController_cancelSpeech", CharSet = CharSet.Unicode)]
        internal static extern int Nvda64nvdaController_cancelSpeech();
        #endregion

    }
}
