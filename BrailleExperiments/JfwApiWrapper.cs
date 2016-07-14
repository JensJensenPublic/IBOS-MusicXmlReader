using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace BrailleExperiments
{
    class JfwApiWrapper
    {

        /* 
          C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC\bin>dumpbin /exports C:\temp\JAWS\jfwapi.dll
          1    0 00001FAC JFWGetActiveVoiceProfileIndex
          2    1 000017E4 JFWGetLanguageA
          3    2 00001870 JFWGetLanguageW
          4    3 00001740 JFWGetOption
          5    4 00001F04 JFWGetSpeechOutputModeValue
          6    5 00002DC4 JFWGetVoiceProfileNames
          7    6 00001568 JFWRunFunction
          8    7 0000165C JFWRunFunctionA
          9    8 00002234 JFWRunFunctionW
         10    9 00001504 JFWRunScript
         11    A 00001654 JFWRunScriptA
         12    B 000021E8 JFWRunScriptW
         13    C 000015C0 JFWSayString
         14    D 00001664 JFWSayStringA
         15    E 00001DF0 JFWSayStringEx
         16    F 00001E10 JFWSayStringEx1
         17   10 000018FC JFWSayStringExA
         18   11 00001B78 JFWSayStringExA1
         19   12 00001A34 JFWSayStringExW
         20   13 00001CB0 JFWSayStringExW1
         21   14 00002280 JFWSayStringW
         22   15 00002048 JFWSetActiveVoiceProfile
         23   16 0000166C JFWSetOption
         24   17 00001E30 JFWSetSpeechOutputModeValue
         25   18 0000161C JFWStopSpeech 
        */

        // (First jfwapi.dll is found in the debug directory)

        [DllImport("jfwapi.dll", CharSet = CharSet.Ansi)] 
        public static extern bool JFWStopSpeech();
    }
}
