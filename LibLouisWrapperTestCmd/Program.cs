using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibLouisWrapper;
using static LibLouisWrapper.Wrapper;
using MusicXmlReaderModel;
using static System.Net.Mime.MediaTypeNames;
using System.IO;



namespace LibLouisWrapperTestCmd
{
    internal class Program
    {

        static private void Log(string s)
        {
            Console.WriteLine(s);
            Logger.LogCF1(s);    // Append Class anf Function for the function calling Log()    
        }


        /// <summary>
        /// Tests the roundtrip CharsToDots, DotsToChars
        /// </summary>
        /// <param name="text">The string to take through the roundtrip</param>
        /// <returns>True <==> success</returns>
        static bool CharsToDotsToCharsTest( string text)
        {
            string dots;
            bool ok;
            ok = libLouisWrapper.CharsToDots(text, out dots);     
            Log(FormatTranslateResult("CharsToDot", text, ok, dots));

            string newText;
            ok = libLouisWrapper.DotsToChars(dots, out newText);
            Log(FormatTranslateResult("DotsToChar", dots, ok, newText));

            bool equal = (0 == string.Compare(text, newText));
            string message = string.Format(": DotsToChars(CharsToDots(text)) {0} text", equal ? "==" : "<>");
            Log(message);
            if (!equal)
            {
                errorList.Add(Logger.GetCF(message));
            }
            return equal;
        }

        /// <summary>
        /// Tests the roundtrip TranslateString, BackTranslateString
        /// </summary>
        /// <param name="text">The string to take through the roundtrip</param>
        /// <returns>True <==> success</returns>
        static bool StringToDotsToStringTFETest(string text)
        {
            string dots;
            bool ok;

            TypeformEnum[] typeForms;
            ok = libLouisWrapper.TranslateStringTFE(text, out dots, out typeForms);
            Log(FormatTranslateResultTFE("TranslateStringTFE",  text, ok, dots, typeForms));

            string newText;
            TypeformEnum[] typeFormsBack;
            ok = libLouisWrapper.BackTranslateStringTFE(dots, out newText, out typeFormsBack);
            Log(FormatTranslateResultTFE("BackTranslateStringTFE", dots, ok,  newText, typeFormsBack));

            bool equal = (0 == string.Compare(text, newText));
            string message = string.Format(": {0} BackTranslateStringTFE(TranslateStringTFE(text)) {1} text", equal ? "PASSED" : "FAILED" , equal ? "==" : "<>");
            Log(message);
            if (!equal)
            {
                errorList.Add(Logger.GetCF(message));
            }
            return equal;
        }

        private static List<string> errorList = new List<string>();
        private static int successes = 0;


        static bool StringToDotsToStringTest(string text)
        {
            string dots;
            bool ok;
         
            ok = libLouisWrapper.TranslateString(text, out dots);
            Log(FormatTranslateResult("TranslateString", text, ok, dots));

            string newText;    
            ok = libLouisWrapper.BackTranslateString(dots, out newText);
            Log(FormatTranslateResult("BackTranslateString",dots, ok, newText));

            bool equal = (0 == string.Compare(text, newText));

            string messageStart = string.Format(": BackTranslateString(TranslateString(text))[{0}] {1} text[{2}]", text.Length, equal ? "==" : "<>", newText.Length);
            string message;
            if (equal)
            {
                message = string.Format("{0}='{1}'", messageStart, text); // Report successes in one line
            }
            else
            {
                message = string.Format("{0}:\r\n{1}\r\n{2}",messageStart, text, newText); // Report failures in 3 lines
            }
            Log(message);
            if (!equal)
            {
                errorList.Add(Logger.GetCF(message));
            }
            else
            {
                successes++;
            }
            return equal;
        }

        private static string FormatTranslateResult(string method,string input, bool result, string output)
        {
            return string.Format(": {0}('{1}') returned {2}. OutPut[{3}]='{4}') ",method, input, result, output.Length, output);
        }

        private static string FormatTranslateResultTFE(string method, string input, bool result, string output, TypeformEnum[] tfe)
        {
            return string.Format(": {0}('{1}') returned {2}. Tfe.Length={3} Output[{4}]='{5}') ",method, input, result, tfe,output.Length, output);
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

        private static bool RunAllTestFiles()
        {
            bool result = true;
            // Run all tests described in the TestFiles directory
            foreach (string fullFileName in Directory.GetFiles(testInputDir))
            {
                result &= RunTestFile(fullFileName);
            }
            return result;
        }

        static bool RunTestFile(string fullFileName)
        {
            bool result = true;
            Log(string.Format("\r\n\r\n>>>>>>>>>>TestFileName='{0}'<<<<<<<<<<\r\n", Path.GetFileName(fullFileName)));
            string[] lines = File.ReadAllLines(fullFileName);
            foreach (string line in lines)
            {
                result &= StringToDotsToStringTest(line); // StringToDotsToStringTestTFE(texy) fails with text="012345678abcdefghijklmnopqrstuvwxyzæøåABCDEFGHIJKLMNOPQRSTUV"
            }
            return result;
        }


        static Wrapper libLouisWrapper;
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
                //libLouisWrapper = Wrapper.Create("en-ueb-g2.ctb,en-ueb-math.ctb"); // Two tables used in this case );
                libLouisWrapper = Wrapper.Create("da-dk-g26.ctb"); //  Danish table for 6 dots grade 2 forward and backward translation (2022)
                if (null == libLouisWrapper)
                {
                    Log(string.Format(": LibLouis directory or file is missing. Please see logfile for details."));
                    return;
                }

                //string text = "The quick brown fox jumps over the lazy dog";
                string danishCharacters = "abcdefghijklmnopqrstuvwxyzæøå";

                for (int i = 0;((result) && (i < 1)); i++)
                { 
                   result &= CharsToDotsToCharsTest(danishCharacters.ToLower());     // Seems NOT to handle Capital letters !
                   result &= StringToDotsToStringTest(danishCharacters);               // Seems to handle Capital letters !
                   result &= StringToDotsToStringTFETest(danishCharacters);             // Seems to handle Capital letters !               
                }

                // Run explicitly named testfiles
                result &= RunTestFile(Path.Combine(testInputDir, "Danish.txt"));            
                result &= RunTestFile(Path.Combine(testInputDir, "DanishGraphics.txt")); // https://blind.dk/punktskrift-2022    Den danske punktskrift 2022    "÷" will fail       
                result &= RunTestFile(Path.Combine(testInputDir, "SpecialCharacters.txt"));
                result &= RunTestFile(Path.Combine(testInputDir, "EscapeSequences.txt"));

                //
                // Start testing with  English table
                //
                bool englishResult = true;
                libLouisWrapper.Free();
                libLouisWrapper = Wrapper.Create("en-ueb-g2.ctb");  
                if (null == libLouisWrapper)
                {
                    Log(string.Format(": LibLouis directory or file is missing. Please see logfile for details."));
                    return;
                }

                string englishCharacters = "abcdefghijklmnopqrstuvwxyz"; // No æøå
                for (int i = 0; ((englishResult) && (i < 1)); i++)
                {
                    englishResult &= CharsToDotsToCharsTest(englishCharacters.ToLower());     // Seems NOT to handle Capital letters !
                    englishResult &= StringToDotsToStringTest(englishCharacters);               // Seems to handle Capital letters !
                    englishResult &= StringToDotsToStringTFETest(englishCharacters);             // Seems to handle Capital letters !               
                }

                englishResult &= RunTestFile(Path.Combine(testInputDir, "EscapeSequences.txt"));
                englishResult &= RunTestFile(Path.Combine(testInputDir, "SpecialCharacters.txt")); // ";" will fail !
                englishResult &= RunTestFile(Path.Combine(testInputDir, "EnglishExperiment.txt"));

                //englishResult &= RunTestFile(Path.Combine(testInputDir, "English.txt"));
                //englishResult &= RunTestFile(Path.Combine(testInputDir, "EnglishWithoutTabs.txt")); 


                Log(string.Format("\r\n\r\n>>>>>>>>>>(End of testFiles)<<<<<<<<<<\r\n"));

              
                Log(": LibLouisWrapper.Free() returned.");
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

            Log(string.Format("Successes={0} Errors={1}",successes,errorList.Count));   

        }
    }
}
