using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibLouisWrapper;
using static LibLouisWrapper.Wrapper;

namespace LibLouisWrapperTestCmd
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool result = true;
            int charSize = -1;
            string LibLouisNativeDllVersion; ;
            Console.WriteLine(string.Format("Starting."));
            try
            {
                charSize = LibLouisWrapper.Wrapper.lou_charSize();
                Console.WriteLine(string.Format("CharSize = {0}", charSize));

                string text = "Hej";
                Typeforms[] sourceTypeformMap = null;
                string s = TranslateString(text, sourceTypeformMap);

                Console.WriteLine(string.Format("Translatestring('{0}') returned '{1}'", text, s));

                // LibLouisNativeDllVersion = LibLouisWrapper.Wrapper.lou_version();
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
