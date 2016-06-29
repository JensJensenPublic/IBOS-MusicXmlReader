using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrailleExperiments
{
    public class FSBrlDspAPIWrapper
    {

        // FSBRLAPI HANDLE WINAPI fbOpen(LPCSTR lpszPort,HWND hwndNotify,UINT umsgNotify); 
 //       [DllImport("FSBrlDspAPI.dll", CharSet = CharSet.Unicode)]
        [DllImport("FSBrlDspAPI.dll", CharSet = CharSet.Ansi)]
        public static extern int fbOpen(String portName,Int32 h,UInt32 umsgNotify ); // Works

        [DllImport("FSBrlDspAPI.dll", CharSet = CharSet.Ansi)] // Works
        public static extern bool fbBeep(Int32 h);

        [DllImport("FSBrlDspAPI.dll", CharSet = CharSet.Ansi)] // Works
        public static extern int fbGetCellCount(Int32 h);

        [DllImport("FSBrlDspAPI.dll", CharSet = CharSet.Ansi)] // Works
        public static extern bool fbGetDisplayName(Int32 h, StringBuilder name, int nMaxChars);

        [DllImport("FSBrlDspAPI.dll", CharSet = CharSet.Ansi)] // Works
        public static extern bool fbGetFirmwareVersion(Int32 h, StringBuilder name, int nMaxChars);

        [DllImport("FSBrlDspAPI.dll", CharSet = CharSet.Ansi)] // Works
        public static extern bool fbClose(Int32 h);

        [DllImport("FSBrlDspAPI.dll", CharSet = CharSet.Ansi)]
        public static extern bool fbWrite(Int32 h, int nStart, int nLength, IntPtr pBytes);

    }

}
