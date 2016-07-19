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
        https://github.com/qtnc/UniversalSpeech/blob/master/src/windows/engines.c
        UniversalSpeech/engines.c at master · qtnc/UniversalSpeech · GitHub
Skip to content
Banner
Homepage
Navigeringsregion
Personal
Open source
Business
Explore
Navigeringsregion slut
Sign up
Sign in
Navigeringsregion
Pricing
Blog
Support
Søgeregion
This repository
Search
Søgeregion slut
Navigeringsregion slut
Banner slut
hovedregion
liste over 3 emner
You must be signed in to watch a repository
4
You must be signed in to star a repository
4
You must be signed in to fork a repository
3
liste slut

qtnc
/
UniversalSpeech
Navigeringsregion
Code
Issues0
Pull requests0
 Pulse
 Graphs
Navigeringsregion slut
Switch branches or tags
Find file
Copy file path to clipboard
UniversalSpeech/
src/
windows/
engines.c
ecc3ae4
on 3 Nov 2015
@qtnc
qtnc
Modified to have better compatibility with MSVC, +added python 3 binding
1 contributor
Raw
Blame
History
Open this file in GitHub Desktop
You must be signed in to make or propose changes
You must be signed in to make or propose changes
70 lines (62 sloc)  3.53 KB

Tabel med 2 kolonner og 69 rækker
1
/* 
2
Copyright (c) 2011-2015, Quentin Cosendey 
3
This code is part of universal speech which is under multiple licenses. 
4
Please refer to the readme file provided with the package for more information. 
5
 
6
#include"../../include/UniversalSpeech.h" 
7
#include"../private.h" 
8
#include<windows.h> 
9  
10  
11
export intjfwIsAvailable(void); 
12
export intjfwSayW(constwchar_t*, int); 
13
export intjfwBrailleW(constwchar_t*); 
14
export intjfwStopSpeech(void); 
15
export intjfwUnload(void); 
16
export intnvdaIsAvailable(void); 
17
export intnvdaUnload(void); 
18
export intnvdaSayW(constwchar_t*, int); 
19
export intnvdaBraille(constwchar_t*); 
20
export intnvdaStopSpeech(void); 
21
export intweIsAvailable(void); 
22
export intweSayW(constwchar_t*, int); 
23
export intweBrailleW(constwchar_t*); 
24
export intweStopSpeech(void); 
25
export intweUnload(void); 
26
export intsaUnload(void); 
27
export intsaIsAvailable(void); 
28
export intsaSayW(constwchar_t*, int); 
29
export intsaBrailleW(constwchar_t*); 
30
export intsaStopSpeech(void); 
31
export intdolUnload(void); 
32
export intdolIsAvailable(void); 
33
export intdolSay(constwchar_t*, int); 
34
export intdolStopSpeech(void); 
35
export intcbrUnload(void); 
36
export intcbrIsAvailable(void); 
37
export intcbrSayW(constwchar_t*, int); 
38
export intcbrBrailleW(constwchar_t*); 
39
export intcbrStopSpeech(void); 
40
export intztLoad(void); 
41
export intztUnload(void); 
42
export intztIsAvailable(void); 
43
export intztSayW(constwchar_t*, int); 
44
export intztStopSpeech(void); 
45
export intsapiIsAvailable(void); 
46
export intsapiSayW(constwchar_t*, int); 
47
export intsapiStopSpeech(void); 
48
export intsapiUnload(void); 
49
export intsapiSetValue(int, int); 
50
export intsapiGetValue(int); 
51
export constvoid* sapiGetString(int); 
52  
53
staticintdoNothing() { return1; } 
54  
55
const engine engines[] = {
56
{ .name=L"Jaws", .isAvailable=jfwIsAvailable, .unload=jfwUnload, .say=jfwSayW, .stop=jfwStopSpeech, .braille=jfwBrailleW, .setValue=NULL, .getValue=NULL, .setString=NULL, .getString=NULL  },
57
{ .name=L"Windows eye", .isAvailable=weIsAvailable, .unload=weUnload, .say=weSayW, .stop=weStopSpeech, .braille=weBrailleW, .setValue=NULL, .getValue=NULL, .setString=NULL, .getString=NULL  },
58
{ .name=L"NVDA", .isAvailable=nvdaIsAvailable, .unload=nvdaUnload, .say=nvdaSayW, .braille=nvdaBraille, .stop=nvdaStopSpeech, .setValue=NULL, .getValue=NULL, .setString=NULL, .getString=NULL  },
59
{ .name=L"System access", .isAvailable=saIsAvailable, .unload=saUnload, .say=saSayW, .stop=saStopSpeech, .braille=saBrailleW, .setValue=NULL, .getValue=NULL, .setString=NULL, .getString=NULL  },
60
{ .name=L"Supernova", .isAvailable=dolIsAvailable, .unload=dolUnload, .say=dolSay, .stop=dolStopSpeech, .braille=doNothing, .setValue=NULL, .getValue=NULL, .setString=NULL, .getString=NULL  },
61
{ .name=L"ZoomText", .isAvailable=ztIsAvailable, .unload=ztUnload, .say=ztSayW, .stop=ztStopSpeech, .braille=doNothing, .setValue=NULL, .getValue=NULL, .setString=NULL, .getString=NULL  },
62
{ .name=L"Cobra", .isAvailable=cbrIsAvailable, .unload=cbrUnload, .say=cbrSayW, .stop=cbrStopSpeech, .braille=cbrBrailleW, .setValue=NULL, .getValue=NULL, .setString=NULL, .getString=NULL  },
63
{ .name=L"SAPI5", .isAvailable=sapiIsAvailable, .unload=sapiUnload, .say=sapiSayW, .stop=sapiStopSpeech, .braille=doNothing, .setValue=sapiSetValue, .getValue=sapiGetValue, .setString=NULL, .getString=sapiGetString  },
64
{ .name=NULL, .isAvailable=NULL, .unload=NULL, .stop=NULL, .say=NULL, .braille=NULL, .setValue=NULL, .getValue=NULL, .setString=NULL, .getString=NULL }
65
}; 
66
constint numEngines = sizeof(engines) / sizeof(engine) - 1; 
67  
68  
69 
tabel slut
hovedregion slut

Oplysninger om indholdstype
liste over 6 emner
Contact GitHub
API
Training
Shop
Blog
About
liste slut
Homepage
liste over 6 emner
© 2016 GitHub, Inc.
Terms
Privacy
Security
Status
Help
liste slut
Oplysninger om indholdstype slut


 */



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

        /*

        typedef BOOL (WINAPI *JFWRunFunctionType)(LPCTSTR lpszFuncName);
        typedef BOOL (WINAPI *JFWSayStringType)(LPCTSTR lpszStringToSpeak,BOOL bInterrupt);
        typedef BOOL (WINAPI *JFWStopSpeechType)(void);
        typedef BOOL (WINAPI *JFWRunScriptType)(LPCTSTR lpszScriptName);


export BOOL jfwGetRunningVersion (char* buf, int bufmax) {
if (!FindProcess("jfw.exe", buf, bufmax)) return FALSE;
return GetProcessVersionInfo(buf, 1, buf, bufmax);
}


         */


        // (First jfwapi.dll is found in the debug directory)

        // It looks like the project must be x64 ahd that jfwapi.dll must be present in 
        // C:\Users\Jens\Documents\Visual Studio 2015\Projects\MusicXmlReaderUI\BrailleExperiments\bin\x64\Debug

        [DllImport("jfwapi.dll", CharSet = CharSet.Ansi)]
        public static extern bool JFWStopSpeech();              // Works

        [DllImport("jfwapi.dll", CharSet = CharSet.Unicode)]    // Works
        public static extern bool JFWSayString(String text);

        //  [DllImport("jfwapi.dll", CharSet = CharSet.Unicode)]
        //  public static extern bool JFWBrailleW(String text);     // Is not implemented in current version of jfwapi.dll


        [DllImport("jfwapi.dll", CharSet = CharSet.Unicode)]    // Under development. Returnerer true, og kan skrive mappede tegn ud, men ikke 0x2800 serien !
        public static extern bool JFWRunFunction(String text);

        [DllImport("jfwapi.dll", CharSet = CharSet.Unicode)]    // Under development. Returnerer true, men har ingen virkning
        public static extern bool JFWRunFunction(String function, String param1);

    }
}
