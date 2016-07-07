using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXmlReaderUI
{
    public class FSBrlDspAPIWrapper
    {

        private int handle;
        private int cellCount;

        private FSBrlDspAPIWrapper()
        { }

        public static FSBrlDspAPIWrapper Create()
        {
            return new FSBrlDspAPIWrapper();
        }

        public bool Open()
        {
            const int INVALID_HANDLE_VALUE = -1;
            handle = fbOpen("USB", 0, 42); // Fails, but survives
            //int handle = FSBrlDspAPIWrapper.fbOpen("", 0, 0);// Fails, but survives
            //int handle = FSBrlDspAPIWrapper.fbOpen(null, 0, 0);// Fails and crashes application
            if (INVALID_HANDLE_VALUE == handle)
            {
                Model.Log(string.Format("FSBrlDspAPIWrapper.fbOpen failed. Marshal.GetLastWin32Error returned {0}", Marshal.GetLastWin32Error()));
                return false;
            }
            else
            {
                Model.Log(string.Format("FSBrlDspAPIWrapper.fbOpen returned a valid handle {0}", handle));
            }

            bool result = false;

            result = fbBeep(handle);
            Model.Log(string.Format("FSBrlDspAPIWrapper.fbBeep {0}", result ? "succeeded" : "failed"));

            cellCount = fbGetCellCount(handle);
            Model.Log(string.Format("FSBrlDspAPIWrapper.fbGetCellCount {0}", (cellCount != 0) ? "succeeded" : "failed"));



            int maxNameSize = 100;
            StringBuilder sbName = new StringBuilder(maxNameSize);
            result = fbGetDisplayName(handle, sbName, maxNameSize);
            Model.Log(string.Format("FSBrlDspAPIWrapper.fbGetDisplayName {0}", result ? "succeeded" : "failed"));

            int maxVersionSize = 100;
            StringBuilder sbVersion = new StringBuilder(maxVersionSize);
            result = fbGetFirmwareVersion(handle, sbVersion, maxVersionSize);
            Model.Log(string.Format("FSBrlDspAPIWrapper.fbGetFirmwareVersion {0}", result ? "succeeded" : "failed"));

            Model.Log(string.Format("DeviceName={0} FirmwareVersion ={1} CellCount={2}", sbName.ToString(), sbVersion.ToString(), cellCount));

            return true;
        }



        public bool Write(byte[] buffer)
        {
            bool fbWriteResult = false;
            unsafe
            {
                fixed (byte* p = buffer)
                {
                    IntPtr ptr = (IntPtr)p;
                    fbWriteResult = FSBrlDspAPIWrapper.fbWrite(handle, 0, cellCount, ptr);
                    // do you stuff here
                }

            }

            Model.Log(string.Format("FSBrlDspAPIWrapper.fbWrite {0}", fbWriteResult ? "succeeded" : "failed"));

            return fbWriteResult;
        }




        public bool Close()
        {
            bool result = fbClose(handle);
            Model.Log(string.Format("FSBrlDspAPIWrapper.fbGetFirmwareVersion {0}", result ? "succeeded" : "failed"));
            return result;
        }



        [DllImport("FSBrlDspAPI.dll", CharSet = CharSet.Ansi)]
        public static extern int fbOpen(String portName, Int32 h, UInt32 umsgNotify); // Works

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

        [DllImport("FSBrlDspAPI.dll", CharSet = CharSet.Ansi)] // Works
        public static extern bool fbWrite(Int32 h, int nStart, int nLength, IntPtr pBytes);

    }

}
