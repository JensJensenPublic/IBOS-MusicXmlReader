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
            Log(string.Format(": TranslateStringTFE('{0}') returned {1}. Tfe.Length={2} Dots='{3}') ", text, ok, typeForms.Length, dots));

            string backTranslationResult;
            TypeformEnum[] typeFormsBack;
            ok = libLouisWrapper.BackTranslateStringTFE(dots, out backTranslationResult, out typeFormsBack);
            Log(string.Format(": BackTranslateStringTFE('{0}') returned {1}. Tfe.Length={2} Text='{3}') ", dots, ok, typeFormsBack.Length, backTranslationResult));

            bool equal = (0 == string.Compare(text, backTranslationResult));
            Log(string.Format(": BackTranslateStringTFE(TranslateStringTFE(text) {0} text", equal ? "==" : "<>"));

            return equal;
        }


        static bool StringToDotsToStringTest(string text)
        {
            string dots;
            bool ok;
         
            ok = libLouisWrapper.TranslateString1(text, out dots);
            Log(string.Format(": TranslateString('{0}') returned {1}. Dots='{2}') ", text, ok, dots));

            string backTranslationResult;    
            ok = libLouisWrapper.BackTranslateString1(dots, out backTranslationResult);
            Log(string.Format(": BackTranslateString('{0}') returned {1}. Text='{2}') ", dots, ok,  backTranslationResult));

            bool equal = (0 == string.Compare(text, backTranslationResult));
            Log(string.Format(": BackTranslateString(TranslateString(text) {0} text", equal ? "==" : "<>"));

            return equal;
        }



        static Wrapper libLouisWrapper;

        static void Main(string[] args)
        {
            //MusicXmlReaderModel.Logger.LogCF(": Starting");
            Logger.Open(@"c:\temp\LibLouis\LibLouisWrapperTestCmd.log");
            Log(": ---------------------------------------------------");
            Log(string.Format(": Starting {0}",Environment.CommandLine.ToString()));       
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
                }
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
