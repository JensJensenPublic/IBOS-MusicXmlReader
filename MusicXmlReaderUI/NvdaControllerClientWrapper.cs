using System;
using System.Runtime.InteropServices;
using System.Threading;

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

        private static string className = "NvdaControllerClientWrapper";
        private Thread brailleDisplayThread;
        private bool displaying = true;
        private string latestMessage = null; // Latest message sent to Braille display

        /// <summary>
        /// Thread needed for refreshing the MusicBraille Message sent to the Braille Display to prevent it from being overwritten by LyricBraille
        /// </summary>
        private void DisplayerThreadStart()
        {
            while (displaying)
            {
                Thread.Sleep(1000); // Refresh the display every second as long as needed
                {
                    if (!string.IsNullOrEmpty(latestMessage))
                    {
                        BrailleMessage(latestMessage);
                    }
                }
            }
        }




        public static NvdaControllerClientWrapper Create()
        {
            return new NvdaControllerClientWrapper();
        }

        private NvdaControllerClientWrapper()
        {
            {
                brailleDisplayThread = new System.Threading.Thread(new System.Threading.ThreadStart(DisplayerThreadStart));
                Model.Log(string.Format("Starting PlayerThread et priority={0}", brailleDisplayThread.Priority.ToString()));
                brailleDisplayThread.Start();
            }

        }

        private void LogException(string methodName, string message)
        {
            Model.Log(string.Format("{0}.{1} reported an exception: {2}", className, methodName, message));
        }

        private uint LogFailure(string methodName)
        {
            uint lastError = GetLastWin32Error();
            Model.Log(string.Format("{0}.{1} reported an error: {2}", className, methodName, lastError));
            return lastError;
        }

        public bool TestIfRunning(out uint errorCode)
        {
            errorCode = 0;
            try
            {
                int result = nvdaController_testIfRunning();
                if (0 == result)
                {
    
                    return true;
                }
                else
                {
                    errorCode = LogFailure("TestIfRunning");
                }

            }
            catch (Exception e)
            {
                LogException("TestIfRunning", e.Message);
            }
            return false;
        }

        public bool SpeakText(String text)
        {
            try
            {
                int result = nvdaController_speakText(text);
                if (0 != result)
                {
                    LogFailure("SpeakText");
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                LogException("SpeakText", e.Message);
            }
            return false;
        }

        public bool TempBrailleMessage(string text)
        {
            return BrailleMessage(text,false);
        }


        public bool BrailleMessage(String text)
        {
           return BrailleMessage(text, true);
        }


        private bool BrailleMessage(String text, bool startRefreshing)
        {
            try
            {
                int result = nvdaController_brailleMessage(text);
                if (0 != result)
                {
                    LogFailure("BrailleMessage");
                    return false;
                }
                latestMessage = startRefreshing ? text : String.Empty;  
                return true;
            }
            catch (Exception e)
            {
                LogException("BrailleMessage", e.Message);
            }
            return false;
        }

        public bool CancelSpeech()
        {
            try
            {
                int result = nvdaController_cancelSpeech();
                if (0 != result)
                {
                    LogFailure("cancelSpeech");
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                LogException("cancelSpeech", e.Message);
            }
            return false;
        }

        public void StopRefreshing()
        {
            latestMessage = string.Empty; // Stop refreshing the physical Braille Display
        }



        // Only for error reporting
        [DllImport("kernel32.dll")]
        static extern uint GetLastWin32Error();


        //******************************************************************
        // nvdaControllerClient32.dll implements the following 4 functions:
        //******************************************************************

        [DllImport("nvdaControllerClient32.dll", CharSet = CharSet.Unicode)]
        private static extern int nvdaController_testIfRunning();

        [DllImport("nvdaControllerClient32.dll", CharSet = CharSet.Unicode)]
        private static extern int nvdaController_speakText(String text);

        [DllImport("nvdaControllerClient32.dll", CharSet = CharSet.Unicode)]
        private static extern int nvdaController_brailleMessage(String braille);
        
        [DllImport("nvdaControllerClient32.dll", CharSet = CharSet.Unicode)]
        private static extern int nvdaController_cancelSpeech();
    }
}