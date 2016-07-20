using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using MusicXmlReaderUI;
using System.Threading;

//Unicode for Braille
//https://en.wikipedia.org/wiki/Braille_Patterns
//http://www.unicode.org/charts/PDF/U2800.pdf


namespace BrailleExperiments
{
 

    class BrailleExperiments
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


        static private string GetConstantBrailleString(int value,int length)
        {
            char c = (char)(UnicodeBrailleBase + value);
            return new StringBuilder().Append(c, length).ToString();         
        }

        static private bool LogRunFunction(string format, string value)
        {
            string function = string.Format(format, value);
            Log(function);
            Thread.Sleep(2000);
            bool result = JfwApiWrapper.JFWRunFunction(function);
            Log(string.Format("Result={0}", result));
            return result;
        }

        public static string f = "BrailleString(\"{0}\")";     // Attempts to be identical to the string generated by tolk.dll excluding termination zero
        public static string f0 = "BrailleString(\"{0}\")\0";   // Attempts to be identical to the string generated by tolk.dll including termination zero

        public static int displaySize = 14;
        public static readonly char UnicodeBrailleBase = (char)0x2800;
        public static readonly char UnicodeBraille01 = (char)0x2801;
        public static readonly char UnicodeBraille02 = (char)0x2802;
        public static readonly char UnicodeBrailleAll8 = (char)0x28ff;
        static private void LogJfwApi()
        {
            // Strings in thew Unicode (0x2800) domain
            string emptyBrailleString = new StringBuilder().Append(UnicodeBrailleBase, displaySize).ToString(); // Assume standard Braille Unicode
            string hex01BrailleString = new StringBuilder().Append(UnicodeBraille01, displaySize).ToString(); // Assume standard Braille Unicode
            string hex02BrailleString = new StringBuilder().Append(UnicodeBraille02, displaySize).ToString(); // Assume standard Braille Unicode
            string fullBrailleString = new StringBuilder().Append(UnicodeBrailleAll8, displaySize).ToString(); // Assume standard Braille Unicode
            string zeroString = new StringBuilder().Append((char)0, displaySize).ToString();    // Assume simple 0-based Braille coding
            string ffString = new StringBuilder().Append((char)0xff, displaySize).ToString(); // Assume simple 0-based Braille coding
            StringBuilder sb = new StringBuilder();
            for (char ch = UnicodeBrailleBase; (ch < UnicodeBrailleBase + (char)displaySize); ch++)
            {
                sb.Append(ch);
            };
            string varyingBrailleString = sb.ToString();


            // Strings in the normal character domain
            string numberString = "12345678901234";
            //string string0 = "ABCDEFGHIJ";
            //string string1 = "KLMNOPQRST";
            //string string2 = "UVWXYZ1234";
            //string string3 = "567890.,;:";
            //string string4 = "/?!@#+-*\"\"";
            //string string5 = "'<>()";



        

            bool result = false;
   
            result = JfwApiWrapper.JFWStopSpeech();
            Log(string.Format("JfwApiWrapper.JFWStopSpeech {0}", result ? "succeeded" : "failed**"));

            result = JfwApiWrapper.JFWSayString("HEJ");
            Log(string.Format("JfwApiWrapper.JFWSayString(\"HEJ\") {0}", result ? "succeeded" : "failed**"));

            //result = JfwApiWrapper.JFWBrailleW(emptyBrailleString);
            //Log(string.Format("JfwApiWrapper.JFWBrailleW(emptyBrailleString) {0}", result ? "succeeded" : "failed**"));
            //result = JfwApiWrapper.JFWBrailleW(fullBrailleString);
            //Log(string.Format("JfwApiWrapper.JFWBrailleW(fullBrailleString) {0}", result ? "succeeded" : "failed**"));

            //result = JfwApiWrapper.JFWBraille("HEJ");
            //Log(string.Format("JfwApiWrapper.JFWBraille(\"Braille\") {0}", result ? "succeeded" : "failed**"));

            //string function = string.Format("BrailleString(\"{0}\")", emptyBrailleString);
            //string function = string.Format("BrailleString,\"{0}\")", emptyBrailleString);

            string function;
            int delay = 1000;

            Thread.Sleep(5000);

            for (int c = 0; (c < 1000); c++)
            {
                Console.WriteLine("numberstring");
                function = string.Format("BrailleString({0}=", numberString); // Giver rigtige "1234567890" ASCII tegn  !!!!!!!!!!!!!!!!!!

                //function = string.Format("BrailleString(\"{0}\")\0", varyingBrailleString); 



                //function = string.Format("BrailleMessage(\"{0}\")", numberString); // Uklart
                //function = string.Format("BrailleMessage(\"{0}\")", varyingBrailleString); // Uklart
                //function = string.Format("BrailleMessage({0})", varyingBrailleString); // 
                //function = string.Format("BrailleMessage({0})", numberString); // Uklart
                //function = string.Format("BrailleMessage({0}", numberString); // In
                //function = string.Format("BrailleMessage({0}=", numberString); // Intet sker
                //function = string.Format("Braille({0}=", numberString); // Siger "Ukendt funktion Braille"

                //function = string.Format("BrailleString({0}=", varyingBrailleString); // Giver alle 0x34 tegn

                //function = string.Format("BrailleString(\"{0}\"", numberString); // Giver ASCII tegn men i forkert rækkefølge
                //function = string.Format("BrailleString({0})", numberString); // Giver ASCII tegn men i en mærkelig rækkefølge
                //function = string.Format("BrailleString('{0}=')", numberString); // Giver rigtige ASCII tegn
                //function = string.Format("BrailleString({0}=)", varyingBrailleString); 
                //function = string.Format("BrailleString({0})", numberString);
                result = JfwApiWrapper.JFWRunFunction(function);  // S
                //JfwApiWrapper.JFWStopSpeech();
                Console.WriteLine(string.Format("result={0}", result));
                Thread.Sleep(1000);
            }








            for (int i = 0; (i < 5); i++)
            {
                Console.WriteLine(string.Format("i={0}", i));

                //function = string.Format("BrailleMessage,\"{0}\")", emptyBrailleString); // Does something ?
                //result = JfwApiWrapper.JFWRunFunction(function); // Siger "Ukendt funktion"
                //Console.WriteLine(string.Format("result={0}", result));
                //Thread.Sleep(delay);

                //function = string.Format("BrailleString,\"{0}\")", emptyBrailleString);
                //result = JfwApiWrapper.JFWRunFunction(function); // Siger "Ukendt funktion"
                //Console.WriteLine(string.Format("result={0}", result));
                //Thread.Sleep(delay);

                //function = string.Format("BrailleMessage({0})", emptyBrailleString);
                //result = JfwApiWrapper.JFWRunFunction(function); // Siger ikke noget
                //JfwApiWrapper.JFWStopSpeech();
                //Console.WriteLine(string.Format("result={0}", result));
                //Thread.Sleep(delay);

                //function = string.Format("BrailleString({0}", emptyBrailleString);
                //result = JfwApiWrapper.JFWRunFunction(function); // Skriver ens tegn ud over hele displayet. Siger ikke noget
                //JfwApiWrapper.JFWStopSpeech();
                //Console.WriteLine(string.Format("result={0}", result));
                //Thread.Sleep(delay);

                //function = string.Format("BrailleString({0}", fullBrailleString);
                //result = JfwApiWrapper.JFWRunFunction(function);  // Skriver ens tegn ud over hele displayet. Siger ikke noget
                //JfwApiWrapper.JFWStopSpeech();
                //Console.WriteLine(string.Format("result={0}", result));
                //Thread.Sleep(delay);

                for (int k = 0; (k < 2); k++)
                {
                    string s = GetConstantBrailleString(k, 10); // Limit length to 10
                    Console.WriteLine(string.Format("k={0}", k));
                    function = string.Format("BrailleString(\"{0}\")\0", s); // testet mest

                    //function = string.Format("BrailleString('{0}'", varyingBrailleString);
                    //for (int u = 0; (u < function.Length); u++)
                    //{
                    //    int c = (int) function[u];
                    //    Console.WriteLine(c);
                    //}


                    result = JfwApiWrapper.JFWRunFunction(function);  // 
                    //JfwApiWrapper.JFWStopSpeech();
                    Console.WriteLine(string.Format("result={0}", result));
                    Thread.Sleep(delay);
                }

                Console.WriteLine("Bruger BrailleMessage");



                Console.WriteLine("zerostring");
                function = string.Format("BrailleMessage({0})", zeroString);
                result = JfwApiWrapper.JFWRunFunction(function);  // Skriver Braille(ingenting) tegn ud over hele displayet. Siger ikke noget
                JfwApiWrapper.JFWStopSpeech();
                Console.WriteLine(string.Format("result={0}", result));
                Thread.Sleep(delay);


                Console.WriteLine(" hex01BrailleString");
                function = string.Format("BrailleMessage({0})", hex01BrailleString);
                result = JfwApiWrapper.JFWRunFunction(function);  // Skriver {2,6} = 0x22 tegn ud over hele displayet. Siger ikke noget
                JfwApiWrapper.JFWStopSpeech();
                Console.WriteLine(string.Format("result={0}", result));
                Thread.Sleep(delay);


                Console.WriteLine(" hex02BrailleString");
                function = string.Format("BrailleMessage({0})", hex02BrailleString);
                result = JfwApiWrapper.JFWRunFunction(function);  // Skriver tegn ud over hele displayet. Siger ikke noget
                JfwApiWrapper.JFWStopSpeech();
                Console.WriteLine(string.Format("result={0}", result));
                Thread.Sleep(delay);

                Console.WriteLine("varyingBrailleString");
                function = string.Format("BrailleMessage({0})", varyingBrailleString);
                result = JfwApiWrapper.JFWRunFunction(function);
                JfwApiWrapper.JFWStopSpeech();
                Console.WriteLine(string.Format("result={0}", result));
                Thread.Sleep(delay);


                Console.WriteLine("ffstring");
                function = string.Format("BrailleMessage({0})", ffString);
                result = JfwApiWrapper.JFWRunFunction(function);  // Skriver {6} {2,3,4,5,6,8}  Braille tegn ud over hele displayet. Siger ikke noget
                JfwApiWrapper.JFWStopSpeech();
                Console.WriteLine(string.Format("result={0}", result));
                Thread.Sleep(delay);

                Console.WriteLine("numberstring");// Skriver forskellige Braille tegn ud over hele displayet. Siger ikke noget
                function = string.Format("BrailleMessage({0})", numberString);
                result = JfwApiWrapper.JFWRunFunction(function);  // ?S
                JfwApiWrapper.JFWStopSpeech();
                Console.WriteLine(string.Format("result={0}", result));
                Thread.Sleep(1000);

                Console.WriteLine("Bruger BrailleString");

                Console.WriteLine("zerostring");
                function = string.Format("BrailleString({0})", zeroString);
                result = JfwApiWrapper.JFWRunFunction(function);  // Skriver Braille(ingenting) tegn ud over hele displayet. Siger ikke noget
                JfwApiWrapper.JFWStopSpeech();
                Console.WriteLine(string.Format("result={0}", result));
                Thread.Sleep(delay);

                Console.WriteLine(" hex02BrailleString");
                function = string.Format("BrailleString({0})", hex02BrailleString);
                result = JfwApiWrapper.JFWRunFunction(function);  // Skriver tegn ud over hele displayet. Siger ikke noget
                JfwApiWrapper.JFWStopSpeech();
                Console.WriteLine(string.Format("result={0}", result));
                Thread.Sleep(delay);

                Console.WriteLine("varyingBrailleString");
                function = string.Format("BrailleString({0})", varyingBrailleString);
                result = JfwApiWrapper.JFWRunFunction(function);
                JfwApiWrapper.JFWStopSpeech();
                Console.WriteLine(string.Format("result={0}", result));
                Thread.Sleep(delay);


                Console.WriteLine("ffstring");
                function = string.Format("BrailleString({0})", ffString);
                result = JfwApiWrapper.JFWRunFunction(function);  // Skriver ens søre Braille tegn ud over hele displayet. Siger ikke noget
                JfwApiWrapper.JFWStopSpeech();
                Console.WriteLine(string.Format("result={0}", result));
                Thread.Sleep(delay);

                for (int c = 0; (c < 100); c++)
                {
                    Console.WriteLine("numberstring");
                    function = string.Format("BrailleString({0}=", numberString); // Giver rigtige ASCII tegn
                    //function = string.Format("BrailleString('{0}=')", numberString); // Giver rigtige ASCII tegn
                    //function = string.Format("BrailleString({0}=)", varyingBrailleString); 
                    //function = string.Format("BrailleString({0})", numberString);
                    result = JfwApiWrapper.JFWRunFunction(function);  // S
                    JfwApiWrapper.JFWStopSpeech();
                    Console.WriteLine(string.Format("result={0}", result));
                    Thread.Sleep(1000);
                }



            }

            Thread.Sleep(5000);
            for (int j = 0; (j < 5); j++)
            {
                // Ingen af disse siger noget. Har en virkning på displayet !
                Console.WriteLine(string.Format("j={0}", j));

                result = JfwApiWrapper.JFWRunFunction("BrailleString", emptyBrailleString); // Udskriver blanke over hele display
                JfwApiWrapper.JFWStopSpeech();
                Console.WriteLine(string.Format("result={0}",result));
                Thread.Sleep(delay);
                result = JfwApiWrapper.JFWRunFunction("BrailleString", varyingBrailleString); // Works ?
                JfwApiWrapper.JFWStopSpeech();
                Console.WriteLine(string.Format("result={0}", result));
                Thread.Sleep(delay);
                result = JfwApiWrapper.JFWRunFunction("BrailleString", fullBrailleString);
                JfwApiWrapper.JFWStopSpeech();
                Console.WriteLine(string.Format("result={0}", result));
                Thread.Sleep(delay);
                result = JfwApiWrapper.JFWRunFunction("BrailleMessage", emptyBrailleString);
                JfwApiWrapper.JFWStopSpeech();
                Console.WriteLine(string.Format("result={0}", result));
                Thread.Sleep(delay);
                result = JfwApiWrapper.JFWRunFunction("BrailleMessage", varyingBrailleString);
                JfwApiWrapper.JFWStopSpeech();
                Console.WriteLine(string.Format("result={0}", result));
                Thread.Sleep(delay);
                result = JfwApiWrapper.JFWRunFunction("BrailleMessage", fullBrailleString);
                JfwApiWrapper.JFWStopSpeech();
                Console.WriteLine(string.Format("result={0}", result));
                Thread.Sleep(delay);
            }
        }

        //*********************************************************************************************

        static private void LogFSApi()
        {
            // Strings in thew Unicode (0x2800) domain
            string emptyBrailleString = new StringBuilder().Append(UnicodeBrailleBase, displaySize).ToString(); // Assume standard Braille Unicode
            string hex01BrailleString = new StringBuilder().Append(UnicodeBraille01, displaySize).ToString(); // Assume standard Braille Unicode
            string hex02BrailleString = new StringBuilder().Append(UnicodeBraille02, displaySize).ToString(); // Assume standard Braille Unicode
            string fullBrailleString = new StringBuilder().Append(UnicodeBrailleAll8, displaySize).ToString(); // Assume standard Braille Unicode
            string zeroString = new StringBuilder().Append((char)0, displaySize).ToString();    // Assume simple 0-based Braille coding
            string ffString = new StringBuilder().Append((char)0xff, displaySize).ToString(); // Assume simple 0-based Braille coding
            StringBuilder sb = new StringBuilder();
            for (char ch = UnicodeBrailleBase; (ch < UnicodeBrailleBase + (char)displaySize); ch++)
            {
                sb.Append(ch);
            };
            string varyingBrailleString = sb.ToString();

            bool result = false;

            result = FSAPIWrapper.JFWStopSpeech();
            Log(string.Format("FSAPIWrapper.JFWStopSpeech {0}", result ? "succeeded" : "failed**"));

           

            // The next operation will cause the debugger to report a problem about an unbalanced stack.
            // This has temporarily been disabled in the debugger settings as it seems to cause no harm !!   TODO Find out more!!
            result = FSAPIWrapper.JFWSayString("HEJ"); // CRASHES !
            Log(string.Format("FSAPIWrapper.JFWSayString(\"HEJ\") {0}", result ? "succeeded" : "failed**"));

                      
            string sayStringFunction = string.Format("SayString(\"{0}\")", "HALLO");
            result = FSAPIWrapper.JFWRunFunction(sayStringFunction);
            Console.WriteLine(string.Format("FSAPIWrapper.JFWRunFunction(sayStringFunction) returned result={0}", result));
            Thread.Sleep(1000);


            string brailleStringFunction;
            Thread.Sleep(5000);

            for (int c = 0; (c < 1000); c++)
            {
                brailleStringFunction = string.Format("BrailleString(\"{0}\")", varyingBrailleString);
                Console.WriteLine("varyingBrailleString");
                result = FSAPIWrapper.JFWRunFunction(brailleStringFunction);
                Console.WriteLine(string.Format("result={0}", result));
                Thread.Sleep(1000);

                brailleStringFunction = string.Format("BrailleString(\"{0}\")", fullBrailleString);
                Console.WriteLine("fullBrailleString");
                result = FSAPIWrapper.JFWRunFunction(brailleStringFunction);
                Console.WriteLine(string.Format("result={0}", result));
                Thread.Sleep(1000);
            }
        }


        static void Main(string[] args)
        {
            string executingAssembly = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string executingDirectory = System.IO.Path.GetDirectoryName(executingAssembly);
            Log(""); // An empty line
            Log(string.Format("{0} started in '{1}'", System.IO.Path.GetFileName(executingAssembly), executingDirectory));

            //LogJfwApi();
            LogFSApi();

            //LogFSInterface();
            if (LogNvdaInterface())
            {

                LogNvdaSpeechTest();            // Use speech only, no Braille!
                LogNvdaBrailleMessageTest();    // Use Braille only, no speech!
            }
        }
    }
}
