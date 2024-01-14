using System;
using System.Collections.Generic;
using System.Text;
using static LibLouisWrapper.Wrapper;
using MusicXmlReaderModel;
using System.IO;

namespace LibLouisWrapperTestCmd
{
    internal class Program
    {
        static string testInputDir;

        static private void Log(string s)
        {
            Console.WriteLine(s);
            Logger.LogCF1(s);    // Append Class and Function for the function calling Log()    
        }
        private static bool CheckTestFileInstallation()
        {
            string executingDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            testInputDir = Path.Combine(executingDirectory, "TestInputFiles");
            if (!DirectoryExists(testInputDir)) return false;
            string[] testFiles = Directory.GetFiles(testInputDir);
            Log(string.Format(": Found {0} testfiles in {1}:", testFiles.Length, testInputDir));
            foreach (string testFile in testFiles)
            {
                Log(string.Format("   {0}", Path.GetFileName(testFile)));
            }
            return true;
        }
      

        static void Main(string[] args)
        {            
            //MusicXmlReaderModel.Logger.LogCF(": Starting");
            Logger.Open(@"c:\temp\LibLouis\LibLouisWrapperTestCmd.log");
            Log(": ---------------------------------------------------");
            Log(string.Format(": Starting {0}",Environment.CommandLine.ToString()));
            Log(string.Format("Setting Console.OutputEncoding to {0} in order do display Braille symbols",Encoding.Unicode));
            Console.OutputEncoding = Encoding.Unicode; 

            if (!CheckTestFileInstallation()) return; // No reason to continue
   
            try
            {
                for (int i = 0; i < 1000; i++) // Prepare for "endurance" test
                {

                    using (TestHandler testHandlerForDanish = TestHandlerForDanish.Create(testInputDir))
                    {
                        testHandlerForDanish.ExecuteTests(); // The "using" clause will cause a call to Dispose()
                    }

                    using (TestHandler testHandlerForEnglish = TestHandlerForEnglish.Create(testInputDir))
                    {
                        testHandlerForEnglish.ExecuteTests(); // The "using" clause will cause a call to Dispose()
                    }

                }

                Log(string.Format(": No Exception was thrown during test."));
            }
            catch (Exception e)
            {
                Log(string.Format(": Main() failed because of an exception!  Exception.Message='{0}'", e.Message));                         
            }

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}
