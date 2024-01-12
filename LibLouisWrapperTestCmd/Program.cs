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

        static private void Log(string s)
        {
            Console.WriteLine(s);
            Logger.LogCF1(s);    // Append Class and Function for the function calling Log()    
        }

        private static List<string> errorList = new List<string>();

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
   
        static string testInputDir;

        static void Main(string[] args)
        {            
            //MusicXmlReaderModel.Logger.LogCF(": Starting");
            Logger.Open(@"c:\temp\LibLouis\LibLouisWrapperTestCmd.log");
            Log(": ---------------------------------------------------");
            Log(string.Format(": Starting {0}",Environment.CommandLine.ToString()));
            Log(string.Format("Setting Console.OutputEncoding to {0} in order do display Braille symbols",Encoding.Unicode));
            Console.OutputEncoding = Encoding.Unicode; 

            if (!CheckTestFileInstallation()) return;
            bool result = true;
            try
            {         
                TestHandlerForDanish testHandlerForDanish = TestHandlerForDanish.Create(testInputDir);
                testHandlerForDanish.ExecuteTests();  

                TestHandlerForEnglish testHandlerForEnglish = TestHandlerForEnglish.Create(testInputDir);
                testHandlerForEnglish.ExecuteTests();

                Log(string.Format(": No Exception was thrown during test."));
            }
            catch (Exception e)
            {
                Log(string.Format(": Main() failed because of an exception!  Exception.Message='{0}'", e.Message));
                result = false;            
            }

            // Report overall test result

            Log(string.Format(": Test {0} ****************************************************************************************************", result ? "PASSED" : "FAILED")  );
            if (result) return;

            // In case of errors report any error information:
            StringBuilder sb = new StringBuilder(); 
            sb.AppendLine(string.Format(": {0} Error{1} detected:", errorList.Count, (1 == errorList.Count) ? "" : "s"));
            foreach (string error in errorList)
            {
                sb.AppendLine("  " + error);               
            }
            string logString = sb.ToString();
            Log(logString);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();

        }
    }
}
