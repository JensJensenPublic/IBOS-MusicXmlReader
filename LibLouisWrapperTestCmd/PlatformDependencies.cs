using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicXmlReaderModel;

namespace LibLouisWrapperTestCmd
{
    /// <summary>
    /// Class for handling all platform dependencies at one place.
    /// Must exist for each platform
    /// This variant makes use of the functionality in the MusicXmlReaderModel namespace
    /// </summary>
    internal class PlatformDependencies
    {
        static internal void OpenLogFile(string path)
        {
            Logger.Open(path);  
        }

        static internal void Log(string s)
        {
            Console.WriteLine(s);   
            Logger.LogCF1(s); // Add one extra stacklevel
        }

        static internal void OnWrapperLog(string s)
        {
            Console.WriteLine(s);
            Logger.LogCF2(s);
        }
        static internal void OnLibLouisLog(string s)
        {
            Console.WriteLine(s);
            Logger.LogCF3(s);
        }


    }
}
