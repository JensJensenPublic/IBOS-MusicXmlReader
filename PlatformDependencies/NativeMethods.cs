using System;
using System.Runtime.InteropServices;
using System.Text;

namespace PlatformDependencies
{
    internal class NativeMethods
    {
#if false
#region FSBrlDspAPI

        // These native methods were used by FSBrlDspAPIWrapper.cs, which has been excluded from the MusicXmlREaderModel.

        [DllImport("FSBrlDspAPI.dll", CharSet = CharSet.Ansi)]
        internal static extern int fbOpen(String portName, Int32 h, UInt32 umsgNotify); // Works

        [DllImport("FSBrlDspAPI.dll", CharSet = CharSet.Ansi)] // Works
        internal static extern bool fbBeep(Int32 h);

        [DllImport("FSBrlDspAPI.dll", CharSet = CharSet.Ansi)] // Works
        internal static extern int fbGetCellCount(Int32 h);

        [DllImport("FSBrlDspAPI.dll", CharSet = CharSet.Ansi)] // Works
        internal static extern bool fbGetDisplayName(Int32 h, StringBuilder name, int nMaxChars);

        [DllImport("FSBrlDspAPI.dll", CharSet = CharSet.Ansi)] // Works
        internal static extern bool fbGetFirmwareVersion(Int32 h, StringBuilder name, int nMaxChars);

        [DllImport("FSBrlDspAPI.dll", CharSet = CharSet.Ansi)] // Works
        internal static extern bool fbClose(Int32 h);

        [DllImport("FSBrlDspAPI.dll", CharSet = CharSet.Ansi)] // Works
        internal static extern bool fbWrite(Int32 h, int nStart, int nLength, IntPtr pBytes);
#endregion
#endif

#region SystemParametersInfo
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        // internal       static extern bool SystemParametersInfo(uint uiAction, uint uiParam, IntPtr pvParam, uint fWinIni);
        internal static extern bool SystemParametersInfo(uint uiAction, uint uiParam, ref int param, uint fWinIni);
#endregion

#region SHGetKnownFolderPath
        [DllImport("Shell32.dll")]
        internal static extern int SHGetKnownFolderPath(    [MarshalAs(UnmanagedType.LPStruct)]Guid rfid, uint dwFlags, IntPtr hToken,    out IntPtr ppszPath);
#endregion SHGetKnownFolderPath


    }
}
