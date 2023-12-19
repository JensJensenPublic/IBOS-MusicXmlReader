using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibLouisWrapper;

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
                LibLouisNativeDllVersion = LibLouisWrapper.Wrapper.lou_version();
            }
            catch (Exception)
            {
                result = false;            
            }
            Console.WriteLine(string.Format("Result = {0}", result));
            Console.ReadLine();

        }
    }
}
