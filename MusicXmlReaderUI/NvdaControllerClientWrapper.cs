using System;
using System.Runtime.InteropServices;
namespace MusicXmlReaderUI
{

    // JSJ:
    // The sample code used for inspiration was downloaded  from
    // http://forum.audiogames.net/viewtopic.php?id=12026
    // The (binary) nvdaControllerClient32.dll was downloaded from  from
    // http://community.nvda-project.org/nvdaC … 0100219.7z
    // to C:\Users\Jens\Downloads\NVDAControllerClient\nvdaControllerClient_20100219.7z
    // and extracted there to
    //
    // Directory of C:\Users\Jens\Downloads\NVDAControllerClient
    //
    // 25-04-2016  10:02    <DIR>          .
    // 25-04-2016  10:02    <DIR>        ..
    // 22-01-2010  09:23               503 example_c.c
    // 25-01-2010  04:26               791 example_python.py
    // 22-01-2010  09:23            26.434 license.txt
    // 19-02-2010  03:21             2.353 nvdaController.h
    // 19-02-2010  03:20           132.608 nvdaControllerClient32.dll
    // 19-02-2010  03:20             1.296 nvdaControllerClient32.exp
    // 19-02-2010  03:20             2.944 nvdaControllerClient32.lib
    // 19-02-2010  03:21           153.600 nvdaControllerClient64.dll
    // 19-02-2010  03:21             1.280 nvdaControllerClient64.exp
    // 19-02-2010  03:21             2.892 nvdaControllerClient64.lib
    // 24-04-2016  23:01           124.767 nvdaControllerClient_20100219.7z
    // 25-01-2010  09:55             2.673 readme.html
    //               13 File(s)        452.141 bytes
    //                2 Dir(s)  792.042.774.528 bytes free
    //
    //
    // The file  nvdaControllerClient32.dll must be placed in the executing directory.
    //  nvdaControllerClient32.dll again communicates with the NVDA server through RPC
    //



    /// <summary>
    /// For this class to work as expected, a 32-bit application should define a conditional variable named x86.
    /// In addition, 32-bit applications should reference JFWAPICTRLLib, and 64-bit applications should reference FSAPILib.
    /// This can be done by adding the COM references under the "References" node in the project solution.
    /// Also, the NVDA API should exist in the same directory as the executable. 32-bit applications should use nvdaControllerClient32.dll and 64-bit applications should use nvdaControllerClient64.dll.
    /// </summary>
    public class NvdaControllerClientWrapper
    {
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