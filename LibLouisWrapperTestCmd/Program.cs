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
        static void Main(string[] args)
        {
            //MusicXmlReaderModel.Logger.LogCF(": Starting");
            Logger.Open(@"c:\temp\LibLouis\LibLouisWrapperTestCmd.log");
            Logger.LogCF(": Starting");
            bool result = true;
            int charSize = -1;
          
            Console.WriteLine(string.Format("Starting."));
            try
            {
                charSize = LibLouisWrapper.Wrapper.lou_charSize();
                Console.WriteLine(string.Format("CharSize = {0}", charSize));

                string text = "abcdefghxxxxx";
                Typeforms[] sourceTypeformMap = null;
#if false
                string s = TranslateString(text, sourceTypeformMap);
                Console.WriteLine(string.Format("Translatestring('{0}') returned '{1}'", text, s));
#endif
                string dots = "";
                bool ok = CharsToDots(text, out dots,sourceTypeformMap);
                Console.WriteLine(string.Format("CharsToDots('{0}') returned '{1}'", text, dots));
                Logger.LogCF(string.Format(": CharsToDots('{0}') returned {1}.   Dots={2}) ", text, ok ? "Success" : "Error!", dots));

                string charsResult = "";
                bool okDotsToChars = DotsToChar(dots, out charsResult, sourceTypeformMap);
                Logger.LogCF(string.Format(": DotsToChar('{0}') returned {1}.   Chars={2}) ", dots, ok ? "Success" : "Error!", charsResult));



                // string LibLouisNativeDllVersion; ;
                // LibLouisNativeDllVersion = LibLouisWrapper.Wrapper.lou_version(); // Does not work, and hangs the program !

                Console.WriteLine(string.Format("No Exception"));
            }
            catch (Exception e)
            {
                Console.WriteLine(string.Format("Exception.Message='{0}'", e.Message));
                result = false;            
            }
            Console.WriteLine(string.Format("Result = {0}", result));
            Console.ReadLine();

        }
    }
}
