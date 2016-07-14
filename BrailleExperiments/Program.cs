using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using MusicXmlReaderUI;

//Unicode for Braille
//https://en.wikipedia.org/wiki/Braille_Patterns
//http://www.unicode.org/charts/PDF/U2800.pdf


namespace BrailleExperiments
{
    class Program
    {

        const int INVALID_HANDLE_VALUE = -1;

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
            }

            catch (Exception e)
            {
                Log(string.Format("LogNvdaInterface() threw an exception: {0}", e.Message));
                return false;
            }
            return true;
        }


        static private bool LogNvdaSpeechTest()
        {
            Log("+LogNvdaSpeechTest");
            for (int i = 0; (i < text.Length); i++)
            {
                string s = text[i].ToString();
                S(s);
                System.Threading.Thread.Sleep(250); // Allow the speach to propagate through the system
            }

            int resCancelSpeech = NvdaControllerClientWrapper.nvdaController_cancelSpeech();
            if (0 != resCancelSpeech)
            {
                Log(string.Format("Failed to cancel speech through NVDA ControllerClient: NvdaControllerClientWrapper.NvdaController_cancelSpeech() returned WINERROR={0}", resCancelSpeech));
                return false;
            }

            Log("-LogNvdaSpeechTest");
            return true;
        }

        static private bool LogNvdaBrailleMessageTest()
        {

            Log("+LogNvdaBrailleMessageTest");
            char c1 = (char)0x2801;
            char c2 = (char)0x2802;
            char c3 = (char)0x2803;
            string braille = c1.ToString() + c2.ToString() + c3.ToString();
            int resBrailleMessage = NvdaControllerClientWrapper.nvdaController_brailleMessage(braille);
            for (int i = 0; (i < 64); i++) // Iterate over all 6-point patterns
            {
                StringBuilder sb = new StringBuilder();
                for (int j = 0; (j < 14); j++) // Shos same pattern everywhere
                {
                    sb.Append((char)(0x2800 + i));
                }
                int resBrailleMessage1 = NvdaControllerClientWrapper.nvdaController_brailleMessage(sb.ToString());
                //      int resBrailleMessage = NvdaControllerClientWrapper.nvdaController_brailleMessage(braille);
                System.Threading.Thread.Sleep(500);
            }
            Log("-LogNvdaBrailleMessageTest");
            return true;
        }



        private static bool WriteBytes(int handle, byte[] bytes)
        {
            bool fbWriteResult = false;
            unsafe
            {

                fixed (byte* p = bytes)
                {
                    IntPtr ptr = (IntPtr)p;
                    fbWriteResult = FSBrlDspAPIWrapper.fbWrite(handle, 0, bytes.Length, ptr);
                    // do you stuff here
                }
            }
            return fbWriteResult;
        }


        // Toggle the whole display for 2 minutes
        private static void Toggle(int handle, int cellCount)
        {
            byte[] bytes00 = new byte[cellCount];
            byte[] bytesFF = new byte[cellCount];
            for (int i = 0; (i < cellCount); i++)
            {
                bytes00[i] = (byte)0;
                bytesFF[i] = (byte)0xff; ;
            }

            for (int j = 0; (j < 1000); j++)
            {
                WriteBytes(handle, bytes00);
                System.Threading.Thread.Sleep(1000);
                WriteBytes(handle, bytesFF);
                System.Threading.Thread.Sleep(1000);
            }
        }




        /// <summary>
        /// 
        /// </summary>
        static private void LogFSInterface()
        {

            string directoryName32 = @"C:\Windows\System32";
            //string directoryName64 = @"C:\Windows\SysWOW64";
            string fileName = "fsbrldspapi.dll";
            string fullFileName = Path.Combine(directoryName32, fileName);

            if (!File.Exists(fullFileName))
            {
                Log(string.Format("Driver file for Freedom Scientific Braille Display {0} not found", fullFileName));
                return;
            }
            else
            {
                Log(string.Format("Using {0}", fullFileName));
            }

            // Open 

            // Should use ANSI ??


            int handle = FSBrlDspAPIWrapper.fbOpen("USB", 0, 42); // Fails, but survives
            //int handle = FSBrlDspAPIWrapper.fbOpen("", 0, 0);// Fails, but survives
            //int handle = FSBrlDspAPIWrapper.fbOpen(null, 0, 0);// Fails and crashes application
            if (INVALID_HANDLE_VALUE == handle)
            {
                Log(string.Format("FSBrlDspAPIWrapper.fbOpen failed. Marshal.GetLastWin32Error returned {0}", Marshal.GetLastWin32Error()));
            }
            else
            {
                Log(string.Format("FSBrlDspAPIWrapper.fbOpen returned a valid handle {0}", handle));
            }

            bool result = false;

            result = FSBrlDspAPIWrapper.fbBeep(handle);
            Log(string.Format("FSBrlDspAPIWrapper.fbBeep {0}", result ? "succeeded" : "failed"));

            int cellCount = FSBrlDspAPIWrapper.fbGetCellCount(handle);
            Log(string.Format("FSBrlDspAPIWrapper.fbGetCellCount {0}", (cellCount != 0) ? "succeeded" : "failed"));

            int maxNameSize = 100;
            StringBuilder sbName = new StringBuilder(maxNameSize);
            result = FSBrlDspAPIWrapper.fbGetDisplayName(handle, sbName, maxNameSize);
            Log(string.Format("FSBrlDspAPIWrapper.fbGetDisplayName {0}", result ? "succeeded" : "failed"));

            int maxVersionSize = 100;
            StringBuilder sbVersion = new StringBuilder(maxVersionSize);
            result = FSBrlDspAPIWrapper.fbGetFirmwareVersion(handle, sbVersion, maxVersionSize);
            Log(string.Format("FSBrlDspAPIWrapper.fbGetFirmwareVersion {0}", result ? "succeeded" : "failed"));


            Log(string.Format("DeviceName={0} FirmwareVersion ={1} CellCount={2}", sbName.ToString(), sbVersion.ToString(), cellCount));

            //Byte[] bytes = new Byte[100];                
            //IntPtr pBytes = new IntPtr(bytes);
            //bool fbWriteResult = FSBrlDspAPIWrapper.fbWrite(handle,1,1,pBytes);

            bool fbWriteResult = false;
            unsafe
            {

                byte[] buffer = new byte[255];
                for (byte i = 0; (i < 255); i++)
                {
                    buffer[i] = i;
                }
                fixed (byte* p = buffer)
                {
                    IntPtr ptr = (IntPtr)p;
                    fbWriteResult = FSBrlDspAPIWrapper.fbWrite(handle, 0, cellCount, ptr);
                    // do you stuff here
                }

            }

            Log(string.Format("FSBrlDspAPIWrapper.fbWrite {0}", fbWriteResult ? "succeeded" : "failed"));

            Toggle(handle, cellCount);


            result = FSBrlDspAPIWrapper.fbClose(handle);
            Log(string.Format("FSBrlDspAPIWrapper.fbClose {0}", result ? "succeeded" : "failed"));



        }
        
        static private void LogJfwApi()
        {
            bool result = false;
            result = JfwApiWrapper.JFWStopSpeech();
            Log(string.Format("JfwApiWrapper.JFWStopSpeech {0}", result ? "succeeded" : "failed**"));
        }

        static void Main(string[] args)
        {

            //LogJfwApi();

            //LogFSInterface();
            if (LogNvdaInterface())
            {

                LogNvdaSpeechTest();            // Use speech only, no Braille!
                LogNvdaBrailleMessageTest();    // Use Braille only, no speech!
            }
        }
    }
}
