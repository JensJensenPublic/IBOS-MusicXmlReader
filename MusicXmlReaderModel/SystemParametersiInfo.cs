using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace MusicXmlReaderModel
{

  
    static public class SystemParametersiInfo
    {

        //[DllImport("kernel32.dll")]
        //static extern uint GetLastError();

        [DllImport("kernel32.dll")]
        static extern uint GetLastWin32Error();

        //[DllImport("user32.dll", SetLastError = true)]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //static extern bool SystemParametersInfo(uint uiAction, uint uiParam, ref T pvParam, SPIF fWinIni); // T = any type

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
//        static extern bool SystemParametersInfo(uint uiAction, uint uiParam, IntPtr pvParam, uint fWinIni);
        static extern bool SystemParametersInfo(uint uiAction, uint uiParam, ref int param, uint fWinIni);

        // // For setting a string parameter
        // [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        // [return: MarshalAs(UnmanagedType.Bool)]
        // static extern bool SystemParametersInfo(uint uiAction, uint uiParam, String pvParam, SPIF fWinIni);

        // //For reading a string parameter
        //[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        // [return: MarshalAs(UnmanagedType.Bool)]
        // static extern bool SystemParametersInfo(uint uiAction, uint uiParam, StringBuilder pvParam, SPIF fWinIni);

        static public unsafe bool GetScreenReader(out bool bScreenReader, out int lastWin32Error)
        {
            const uint SPI_GETSCREENREADER = 0x0046;           
            int  iScreenReader = 0;
            bool ok = SystemParametersInfo(SPI_GETSCREENREADER, 0, ref iScreenReader, 0);
            bScreenReader  = ok ? (iScreenReader != 0) : false;
            lastWin32Error = ok ? 0 : Marshal.GetLastWin32Error();
            return ok;
        }   

        //[DllImport("user32.dll", SetLastError = true)]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //static extern bool SystemParametersInfo(SPI uiAction, uint uiParam, ref ANIMATIONINFO pvParam, SPIF fWinIni);


    }
}
