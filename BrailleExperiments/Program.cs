using System;
using System.IO;
using MusicXmlReaderUI;

namespace BrailleExperiments
{
    class Program
    {

        static void Log(string s)
        {
            Console.WriteLine(s);
        }

        static int B(string s)
        {
            int result = NvdaControllerClientWrapper.nvdaController_brailleMessage(s);
            if (0 != result)
            {
                Log(string.Format("Failed to output to Braille display through NVDA ControllerClient: NvdaControllerClientWrapper.NvdaController_brailleMessage() returned WINERROR={0}", result));
            }
            return result;
        }

        static int S(string s)
        {
            int result = NvdaControllerClientWrapper.nvdaController_speakText(s);
            if (0 != result)
            {
                Log(string.Format("Failed to speak directly through NVDA ControllerClient: NvdaControllerClientWrapper.NvdaController_speakText() returned WINERROR={0}", result));
            }
            return result;
        }


        static string text = "0123456789abcdefghijklmnopqrstuvxyzæøå";

        static private bool LogNvdaInterface()
        {
            try
            {
                // First check if the nvdaControllerClient32.dll is found in the execution directory. TODO
                string fileName = "nvdaControllerClient32.dll";
                string fullFileName = Path.Combine(Environment.CurrentDirectory, fileName);
                if (!File.Exists(fullFileName))
                {
                    Log(string.Format("{0} is not found. NVDA ScreenReader can not be controlled through NVDA ControllerClient", fullFileName));
                    return false;
                }

                int resRunning = NvdaControllerClientWrapper.nvdaController_testIfRunning();
                Log(string.Format("NVDA ControllerServer for NVDA ControllerClient is{0}running.", (0 != resRunning) ? " NOT " : " "));
                if (0 != resRunning)
                {
                    if (resRunning != 1722) // 1722 is the expected error in this case : "RPC server is not available. 
                    {
                        Log(string.Format("NvdaControllerClientWrapper.nvdaController_testIfRunning() failed WINERROR={0}", resRunning));
                    }
                    return false;
                }

                System.Threading.Thread.Sleep(2000); // Allow the previous speach to propagate through the system
                int resSpeak = NvdaControllerClientWrapper.nvdaController_speakText("N V D A ControllerClient");
                System.Threading.Thread.Sleep(2000); // Allow the speach to propagate through the system
                if (0 != resSpeak)
                {
                    Log(string.Format("Failed to speak directly through NVDA ControllerClient: NvdaControllerClientWrapper.NvdaController_speakText() returned WINERROR={0}", resSpeak));
                    return false;
                }

                NvdaControllerClientWrapper.nvdaController_cancelSpeech();
                S("Speach");
                B("                       ");
                B("Braille");
                for (int i = 0; (i < text.Length); i++)
                {
                    string s = text[i].ToString();
                    S(s);               
                    B(s + s + s + s + s + s + s + s + s + s + s + s + s + s); // 14
                    System.Threading.Thread.Sleep(2000); // Allow the speach to propagate through the system

                }

                int resCancelSpeech = NvdaControllerClientWrapper.nvdaController_cancelSpeech();
                if (0 != resCancelSpeech)
                {
                    Log(string.Format("Failed to cancel speech through NVDA ControllerClient: NvdaControllerClientWrapper.NvdaController_cancelSpeech() returned WINERROR={0}", resCancelSpeech));
                    return false;
                }

            }
            catch (Exception e)
            {
                Log(string.Format("LogNvdaInterface() threw an exception: {0}", e.Message));
                return false;
            }
            return true;

        }




        static void Main(string[] args)
        {
            LogNvdaInterface();
        }
    }
}
