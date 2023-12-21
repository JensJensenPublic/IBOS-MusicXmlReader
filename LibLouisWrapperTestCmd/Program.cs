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
            Logger.LogCF1(": " + s);    // Append Class anf Function for the function calling Log()    
        }

        static void Main(string[] args)
        {
            //MusicXmlReaderModel.Logger.LogCF(": Starting");
            Logger.Open(@"c:\temp\LibLouis\LibLouisWrapperTestCmd.log");
            Log("---------------------------------------------------");
            Log("Starting");
            Wrapper libLouisWrapper;
            bool result = true;    
            try
            {
                libLouisWrapper = Wrapper.Create();
                Log(string.Format("CharSize = {0}", libLouisWrapper.CharSize));

                string text = "abcdefghxxxxx";
                Typeforms[] sourceTypeformMap = null;
#if false
                string s = TranslateString(text, sourceTypeformMap);
                Console.WriteLine(string.Format("Translatestring('{0}') returned '{1}'", text, s));
#endif
                string dots = "";
                bool ok = libLouisWrapper.CharsToDots(text, out dots,sourceTypeformMap);
                Log(string.Format("CharsToDots('{0}') returned {1}.   Dots={2}) ", text, ok ? "Success" : "Error!", dots));

                string charsResult = "";
                bool okDotsToChars = libLouisWrapper.DotsToChars(dots, out charsResult, sourceTypeformMap);
                Log(string.Format("DotsToChar('{0}') returned {1}.   Chars={2}) ", dots, ok ? "Success" : "Error!", charsResult));



                // string LibLouisNativeDllVersion; ;
                // LibLouisNativeDllVersion = LibLouisWrapper.Wrapper.lou_version(); // Does not work, and hangs the program !

                libLouisWrapper.Free();
                Log("LibLouisWrapper.Free() returned.");
                Log(string.Format("No Exception"));
            }
            catch (Exception e)
            {
                Log(string.Format("Exception.Message='{0}'", e.Message));
                result = false;            
            }
            Log(string.Format("Result = {0}", result));

          

            Log(string.Format("Press any key to exit"));
            //Console.ReadLine();           
        }
    }
}
