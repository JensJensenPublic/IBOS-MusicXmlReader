using LibLouisWrapper;
using MusicXmlReaderModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LibLouisWrapper.Wrapper;

namespace LibLouisWrapperTestCmd
{


    internal abstract class TestHandler : IDisposable
    {
        protected Wrapper libLouisWrapper;

        //protected bool testResult = true; // Untill an error id found
        protected string testInputDir;
        protected string tableName;

        internal static readonly string UnifiedEnglishBrailleGrade2 = "en-ueb-g2.ctb";
        internal static readonly string DanishBrailleGrade2 = "da-dk-g26.ctb";
        internal static readonly string GermanBrailleGrade2 = "de-g2-detailed.ctb";


        protected TestResult testResult = TestResult.Create();

        internal abstract TestResult ExecuteTests();

        internal int GlobalLibLouisErrorCount {get{ return Wrapper.GlobalLibLouisErrorCount; } }

        public void Dispose()
        {     
            libLouisWrapper.Dispose();                 
               
        }

        protected void Log(string s)
        {
            Console.WriteLine(s);
            Logger.LogCF1(s);    // Append Class anf Function for the function calling Log()    
        }

        protected TestHandler(string tableName, string testInputDir)
        {
            this.testInputDir = testInputDir;
            this.tableName = tableName; 
            libLouisWrapper = Wrapper.Create(tableName, OptionsEnum.UseLogCallback); //  Danish table for 6 dots grade 2 forward and backward translation (2022) 
        }

        protected bool CheckWrapper()
        {
            if (null == libLouisWrapper)
            {
                Log(string.Format(": At least one LibLouis directory or file is missing. Please see the logfile for details."));
                Log(string.Format(": Developer: To add all relevant LibLouis directories and files to your project - do the following:"));
                Log(string.Format(@": 1) Assure that the PostBuild event of your main project calls 'LibLouisWrapper\publish.cmd'"));
                Log(string.Format(@": 2) Temporarily remove the 'exit' command from 'LibLouisWrapper\publish.cmd' while rebuilding once."));
                const int exitCode = 1;
                Log(string.Format(": The application will now exit with an ExitCode of {0}", exitCode));
                System.Environment.Exit(exitCode); // This is for a simple Console application: A simple Exit with an exitcode <> 0.  (0 means success.)
                return false;
            }
            return true;
        }

        protected void FreeWrapper()
        {
            libLouisWrapper.Free();
        }


        /// <summary>
        /// Tests the roundtrip CharsToDots, DotsToChars
        /// </summary>
        /// <param name="text">The string to take through the roundtrip</param>
        /// <returns>True <==> success</returns>
        protected bool CharsToDotsToCharsTest(string text)
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
                testResult.ErrorList.Add(Logger.GetCF(message));
            }
            return equal;
        }

  

        protected bool StringToDotsToStringTest(string text)
        {
            string dots;
            bool ok;

            ok = libLouisWrapper.TranslateString(text, out dots);
            Log(FormatTranslateResult("TranslateString", text, ok, dots));

            string newText;
            ok = libLouisWrapper.BackTranslateString(dots, out newText);
            Log(FormatTranslateResult("BackTranslateString", dots, ok, newText));

#if true
            text = text.TrimStart('\t'); // Remove all leading tabs
            newText = newText.TrimStart(' '); // Remove all leading spaces
#endif


            bool equal = (0 == string.Compare(text, newText));

            string messageStart = string.Format(": BackTranslateString(TranslateString(text))[{0}] {1} text[{2}]", text.Length, equal ? "==" : "<>", newText.Length);
            string message;
            if (equal)
            {
                message = string.Format("{0}='{1}'", messageStart, text); // Report successes in one line
                testResult.Successes++;
            }
            else
            {
                string diffReport = GetDiffReport(text, newText);
                message = string.Format("{0}: {1}\r\n{2}\r\n{3}", messageStart, diffReport, text, newText); // Report failures in 3 lines
                testResult.ErrorList.Add(Logger.GetCF(message));
            }
            Log(message);
            return equal;
        }

        private  string GetDiffReport(string t0, string t1)
        {
            if ((string.IsNullOrEmpty(t0)) || (string.IsNullOrEmpty(t1))) return "At least one argument is null or empty";
            int minLength = Math.Min(t0.Length, t1.Length);
            for (int i = 0; i < minLength; i++)
            {
                if (t0[i] != t1[i])
                {
                    char c0 = t0[i];
                    char c1 = t1[i];
                    string diff = string.Format("(Chars:'{0}' <> '{1}')   (Integers:{2} <> {3})", c0, c1, (int)c0, (int)c1);
                    testResult.AllDiffs.Add(diff);
                    return string.Format("First diff found at index {0}: {1}", i, diff);

                }
            }
            return "No difference found";
        }

        private string FormatTranslateResult(string method, string input, bool result, string output)
        {
            return string.Format(": {0}('{1}') returned {2}. OutPut[{3}]='{4}') ", method, input, result, output.Length, output);
        }

        protected bool RunTestFile(string fullFileName)
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


        protected void OnEndOfTestFiles(string language)
        {
            Log(string.Format("\r\n\r\n>>>>>>>>>>(End of testFiles for {0})<<<<<<<<<<\r\n", language));
            Log(string.Format(": Test {0} ****************************************************************************************************", testResult.Result ? "PASSED" : "FAILED"));
            if (!testResult.Result)
            {
                // In case of errors report any error information:
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(string.Format(": {0} Error{1} detected:", testResult.ErrorList.Count, (1 == testResult.ErrorList.Count) ? "" : "s"));
                foreach (string error in testResult.ErrorList)
                {
                    sb.AppendLine("  " + error);
                }
                string logString = sb.ToString();
                Log(logString);
            }

            foreach (Diff diff in testResult.AllDiffs.Diffs)
            {
                string s = string.Format("{0,-45}: Count={1}", diff.Description, diff.Count);
                Log(s);
            }
            Log(string.Format("Successes={0} Errors={1}", testResult.Successes, testResult.ErrorList.Count));
        }

    }
}
