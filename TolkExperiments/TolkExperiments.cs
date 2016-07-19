using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using DavyKager; // Tolk

namespace TolkExperiments
{

    /// <summary>
    /// Simple independent program for testing Tolk.dll
    /// Is primarily used to verify that Tolk kan handle "raw" Braille display, i.e. can map
    /// Unicode values in the 0x2800 to 0x28ff area to the Braille display.
    /// 
    /// This program uses Tolk.dll, which is assumed to be located in the same directory as the .exe file.
    /// Tolk.dll is a 64-bit dll.
    /// In order to load Tolk.dll the .exe must be built for x64 configuration.
    /// This is done from Build->Configuration Manager:
    /// 
    /// 
    /// Project         Configuration Platform  Build
    /// -----------------------------------------------
    /// ToklExperiments Debug         x64       Checked
    /// 
    /// During debugging the .exe is located in
    /// C:\Users\Jens\Documents\Visual Studio 2015\Projects\MusicXmlReaderUI\TolkExperiments\bin\x64\Debug
    /// This directory must also contain 64-bit version of
    /// tolk.dll
    /// jfwapi.dll
    /// nvdaControllerClient64.dll 
    ///  
    /// </summary>
    class TolkExperiments
    {
        static int displaySize = 10;
        static readonly char UnicodeBrailleBase = (char)0x2800;
        static readonly char UnicodeBraille01 = (char)0x2801;
        static readonly char UnicodeBraille02 = (char)0x2802;
        static readonly char UnicodeBrailleAll8 = (char)0x28ff;

        static void Log(string s)
        {
            Console.WriteLine(s);
        }

        static void CheckDll(string dllName, string directory)
        {
            if (!File.Exists(Path.Combine(directory,dllName)))      
            {
                Log(string.Format("Missing support-dll: {0}", dllName));
            }
        }


        static void LogTolkApi()
        {
            string emptyBrailleString = new StringBuilder().Append(UnicodeBrailleBase, displaySize).ToString(); // Assume standard Braille Unicode
            string hex01BrailleString = new StringBuilder().Append(UnicodeBraille01, displaySize).ToString(); // Assume standard Braille Unicode
            string hex02BrailleString = new StringBuilder().Append(UnicodeBraille02, displaySize).ToString(); // Assume standard Braille Unicode
            string fullBrailleString = new StringBuilder().Append(UnicodeBrailleAll8, displaySize).ToString(); // Assume standard Braille Unicode
            string zeroString = new StringBuilder().Append((char)0, displaySize).ToString();    // Assume simple 0-based Braille coding
            string ffString = new StringBuilder().Append((char)0xff, displaySize).ToString(); // Assume simple 0-based Braille coding
            //string numberString = "12345678901234";


            StringBuilder sb = new StringBuilder();
            for (char ch = UnicodeBrailleBase; (ch < UnicodeBrailleBase + (char)displaySize); ch++)
            {
                sb.Append(ch);
            };
            string varyingBrailleString = sb.ToString();

            Tolk.Load();

            bool isLoaded = Tolk.IsLoaded();
            Log(string.Format("Tolk.IsLoaded() returned {0}", isLoaded));
            
            string screenReader = Tolk.DetectScreenReader();
            Log(string.Format("Tolk.DetectScreenReader found {0}", (screenReader == null) ? "No screenreader" : screenReader));


            if (Tolk.HasSpeech())
            {
                Console.WriteLine("This screen reader driver supports speech");
            }
            if (Tolk.HasBraille())
            {
                Console.WriteLine("This screen reader driver supports braille");
            }

            Console.WriteLine("Let's output some text...");
            if (!Tolk.Output("Hello, World!"))
            {
                Console.WriteLine("Failed to output text");
            }

            if (!Tolk.Braille(fullBrailleString)) // This works perfectly !!
            {
                Console.WriteLine("Failed to output Braille");
            }
            Thread.Sleep(1000);

            Console.WriteLine("Wrote Braille once");
            Console.WriteLine("Will now write variying Braille patterns for a very long time");

            Tolk.Silence();

            Thread.Sleep(1000);

            Tolk.Silence();

            Thread.Sleep(1000);

            for (int i = 0; (i < 10000); i++)
            {
                char c = (char) (UnicodeBrailleBase + (i % 256));
                string s = new StringBuilder().Append(c, displaySize).ToString();
                if (!Tolk.Braille(s))
                {
                    Console.WriteLine("Failed to output Braille");
                }
                Thread.Sleep(2000);
            }

            Console.WriteLine("Finalizing Tolk...");
            Tolk.Unload();

            Console.WriteLine("Done!");

        }

        static void Main(string[] args)
        {          
            string executingAssembly = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string executingDirectory = System.IO.Path.GetDirectoryName(executingAssembly);

            Log(""); // An empty line
            Log(string.Format("{0} started in '{1}'", System.IO.Path.GetFileName(executingAssembly), executingDirectory));
            Log("This directory contains the following files");
            string[] files = System.IO.Directory.GetFiles(executingDirectory);
            foreach (string fileName in files)
            {
                Log(" "+System.IO.Path.GetFileName(fileName));
            }
            // Report if any file is missing
            CheckDll("tolk.dll", executingDirectory);
            CheckDll("jfwapi.dll", executingDirectory);
            CheckDll("nvdaControllerClient64.dll",executingDirectory); 
            // Now for the real test
            LogTolkApi();
        }
    }
}
