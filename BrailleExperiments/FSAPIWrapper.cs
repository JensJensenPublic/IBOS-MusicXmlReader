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

        [DllImport("fsapi.dll", CharSet = CharSet.Unicode)]   
        public static extern bool JFWSayString(String text);

        //  [DllImport("jfwapi.dll", CharSet = CharSet.Unicode)]
        //  public static extern bool JFWBrailleW(String text);     // Is not implemented in current version of FSapi.dll


        [DllImport("fsapi.dll", CharSet = CharSet.Unicode)]   
        public static extern bool JFWRunFunction(String text);

        [DllImport("fsapi.dll", CharSet = CharSet.Unicode)]   
        public static extern bool JFWRunFunction(String function, String param1);


    }
}
