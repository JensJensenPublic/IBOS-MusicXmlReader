using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrailleExperiments
{
    class FSAPIWrapper
    {
        [DllImport("fsapi.dll", CharSet = CharSet.Ansi)]
        public static extern bool JFWStopSpeech();

        //[DllImport("fsapi.dll", CharSet = CharSet.Unicode]            // Says the string, then crashes with an unbalanced stack
        //[DllImport("fsapi.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]     // Says the string, then crashes with an unbalanced stack
        //[DllImport("fsapi.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]   // Says the string, then crashes with an unbalanced stack
        //[DllImport("fsapi.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)] // Says the string, then crashes with an unbalanced stack
        //[DllImport("fsapi.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.FastCall)] // Siger intet, crasher
        //[DllImport("fsapi.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.ThisCall)]// Siger intet, crasher

        [DllImport("fsapi.dll", CharSet = CharSet.Unicode)]            // Says the string, then crashes with an unbalanced stack
        public static extern bool JFWSayString(String text);

        [DllImport("fsapi.dll", CharSet = CharSet.Unicode)]         // WORKS!!!
        public static extern bool JFWRunFunction(String text);

        [DllImport("fsapi.dll", CharSet = CharSet.Unicode)]   
        public static extern bool JFWRunFunction(String function, String param1); // Probably not needed !


    }
}
