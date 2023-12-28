using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibLouisWrapper;
using static LibLouisWrapper.Wrapper;
using MusicXmlReaderModel;


namespace LibLouisWrapperTestCmd
{
    internal class Program
    {

        static private void Log(string s)
        {
            Console.WriteLine(s);
            Logger.LogCF1(s);    // Append Class anf Function for the function calling Log()    
        }

        static void Main(string[] args)
        {
            //MusicXmlReaderModel.Logger.LogCF(": Starting");
            Logger.Open(@"c:\temp\LibLouis\LibLouisWrapperTestCmd.log");
            Log(": ---------------------------------------------------");
            Log(string.Format(": Starting {0}",Environment.CommandLine.ToString()));
            Wrapper libLouisWrapper;
            bool result = true;    
            try
            {
                libLouisWrapper = Wrapper.Create("en-ueb-g2.ctb,en-ueb-math.ctb"); // Two tables used in this case );  

                string text = "the quick brown fox jumps over the lazy dog";
                string dots = "";

                bool okCharsToDots = libLouisWrapper.CharsToDots1(text, out dots);
                Log(string.Format(": CharsToDots('{0}') returned {1}.   Dots={2}) ", text, okCharsToDots ? "Success" : "Error!", dots));

                string charsResult = "";

                bool okDotsToChars = libLouisWrapper.DotsToChars1(dots, out charsResult);
                Log(string.Format(": DotsToChar('{0}') returned {1}.   Text={2}) ", dots, okDotsToChars ? "Success" : "Error!", charsResult));

                bool equal = (0 == string.Compare(text, charsResult));
                Log(string.Format(": DotsToChars(CharsToDots(text)) {0} text", equal ? "==" : "<>"));

                dots = "";
                TypeformEnum[] typeForms;
                bool okTranslateString = libLouisWrapper.TranslateString1(text, out dots, out typeForms);
                Log(string.Format(": TranslateString('{0}') returned {1} Tfe.Length={2} Dots='{3}') ", text, okTranslateString, typeForms.Length,dots));

                string backTranslationResult = "";
                TypeformEnum[] typeFormsBack;
                bool okBackTranslateString = libLouisWrapper.BackTranslateString1(dots, out backTranslationResult,out typeFormsBack);
                Log(string.Format(": BackTranslateString('{0}') returned {1} Tfe.Length={2} Text='{3}') ", dots, okBackTranslateString, typeFormsBack.Length, backTranslationResult));

                bool equalTranstation = (0 == string.Compare(text, backTranslationResult)) ;
                Log(string.Format(": BackTranslateString(TranslateString(text) {0} text", equalTranstation ? "==" : "<>"));


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
