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
            ok = libLouisWrapper.CharsToDots1(text, out dots);
            Log(string.Format(": CharsToDots('{0}') returned {1}.   Dots={2}) ", text, ok ? "Success" : "Error!", dots));

            string charsResult;
            ok = libLouisWrapper.DotsToChars1(dots, out charsResult);
            Log(string.Format(": DotsToChar('{0}') returned {1}.   Text={2}) ", dots, ok ? "Success" : "Error!", charsResult));

            bool equal = (0 == string.Compare(text, charsResult));
            Log(string.Format(": DotsToChars(CharsToDots(text)) {0} text", equal ? "==" : "<>"));

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

            string backTranslationResult;
            TypeformEnum[] typeFormsBack;
            ok = libLouisWrapper.BackTranslateStringTFE(dots, out backTranslationResult, out typeFormsBack);
            Log(FormatTranslateResultTFE("BackTranslateStringTFE", dots, ok,  backTranslationResult, typeFormsBack));

            bool equal = (0 == string.Compare(text, backTranslationResult));
            Log(string.Format(": {0} BackTranslateStringTFE(TranslateStringTFE(text)) {1} text", equal ? "PASSED" : "FAILED" , equal ? "==" : "<>"));

            return equal;
        }


        static bool StringToDotsToStringTest(string text)
        {
            string dots;
            bool ok;
         
            ok = libLouisWrapper.TranslateString1(text, out dots);
            Log(FormatTranslateResult("TranslateString", text, ok, dots));

            string backTranslationResult;    
            ok = libLouisWrapper.BackTranslateString1(dots, out backTranslationResult);
            Log(FormatTranslateResult("BackTranslateString",dots, ok, backTranslationResult));

            bool equal = (0 == string.Compare(text, backTranslationResult));
            Log(string.Format(": BackTranslateString(TranslateString(text)) {0} text for text[{1}]='{2}'", equal ? "==" : "<>",text.Length,text));

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



        private static bool CheckInstallation()
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


        static Wrapper libLouisWrapper;
        static string testInputDir;

        static void Main(string[] args)
        {
            //MusicXmlReaderModel.Logger.LogCF(": Starting");
            Logger.Open(@"c:\temp\LibLouis\LibLouisWrapperTestCmd.log");
            Log(": ---------------------------------------------------");
            Log(string.Format(": Starting {0}",Environment.CommandLine.ToString()));
            if (!CheckInstallation()) return;
            bool result = true;
            try
            {
                //libLouisWrapper = Wrapper.Create("en-ueb-g2.ctb,en-ueb-math.ctb"); // Two tables used in this case );
                libLouisWrapper = Wrapper.Create("da-dk-g26.ctb"); //  Danish table for 6 dots grade 2 forward and backward translation (2022)
                if (null == libLouisWrapper)
                {
                    Log(string.Format(": LibLouis directory of file is missing. Please see logfile for details."));
                    return;
                }

                //string text = "The quick brown fox jumps over the lazy dog";
                string text = "abcdefghijklmnopqrstuvwxyzæøå";

                for (int i = 0;((result) && (i < 1)); i++)
                { 
                   result &= CharsToDotsToCharsTest(text.ToLower());     // Seems NOT to handle Capital letters !
                   result &= StringToDotsToStringTest(text);               // Seems to handle Capital letters !
                   result &= StringToDotsToStringTFETest(text);             // Seems to handle Capital letters !
                   if (!result) throw new Exception("Test failed!");
                }

                // Run all tests described in the TestFiles directory
                foreach (string file in Directory.GetFiles(testInputDir))
                {
                    Log(string.Format("\r\n\r\n>>>>>>>>>>TestFileName='{0}'<<<<<<<<<<\r\n", Path.GetFileName(file)));
                    string[] lines = File.ReadAllLines(file);
                    foreach (string line in lines)
                    {
                        result &= StringToDotsToStringTest(line); // StringToDotsToStringTestTFE(texy) fails with text="012345678abcdefghijklmnopqrstuvwxyzæøåABCDEFGHIJKLMNOPQRSTUV"
                    }
                }

                Log(string.Format("\r\n\r\n>>>>>>>>>>(End of testTiles)<<<<<<<<<<\r\n"));

                libLouisWrapper.Free();
                Log(": LibLouisWrapper.Free() returned.");
                Log(string.Format(": No Exception was thrown during test."));
            }
            catch (Exception e)
            {
                Log(string.Format(": Main() failed because of an exception!  Exception.Message='{0}'", e.Message));
                result = false;            
            }
            Log(string.Format(": Result = {0}", result));          

           
        }
    }
}
