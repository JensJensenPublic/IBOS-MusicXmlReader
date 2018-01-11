using System;
using System.Runtime.InteropServices;
using System.Text;

namespace PlatformDependencies
{
    public class FSBrlDspAPIWrapper
    {

        private int handle;
        private int cellCount;

        private void Log(string s)
        {
        }


        private FSBrlDspAPIWrapper()
        { }

        public static FSBrlDspAPIWrapper Create()
        {
            return new FSBrlDspAPIWrapper();
        }

        public bool Open()
        {
            const int INVALID_HANDLE_VALUE = -1;
            int lastWin32Error = 0;
            try
            {
                handle = NativeMethods.fbOpen("USB", 0, 42); // Fails, but survives
                lastWin32Error = Marshal.GetLastWin32Error();
            }
            catch (Exception e)
            {
                Log(string.Format("FSBrlDspAPIWrapper.fbOpen threw an exception. Message={0}", e.Message));
                return false;
            }
            //int handle = FSBrlDspAPIWrapper.fbOpen("", 0, 0);// Fails, but survives
            //int handle = FSBrlDspAPIWrapper.fbOpen(null, 0, 0);// Fails and crashes application
            if (INVALID_HANDLE_VALUE == handle)
            {            
                Log(string.Format("FSBrlDspAPIWrapper.fbOpen failed. Marshal.GetLastWin32Error returned {0}", lastWin32Error));
                return false;
            }
            else
            {
                Log(string.Format("FSBrlDspAPIWrapper.fbOpen returned a valid handle {0}", handle));
            }

            bool result = false;

            result = NativeMethods.fbBeep(handle);
            Log(string.Format("FSBrlDspAPIWrapper.fbBeep {0}", result ? "succeeded" : "failed"));

            cellCount = NativeMethods.fbGetCellCount(handle);
            Log(string.Format("FSBrlDspAPIWrapper.fbGetCellCount {0}", (cellCount != 0) ? "succeeded" : "failed"));



            int maxNameSize = 100;
            StringBuilder sbName = new StringBuilder(maxNameSize);
            result = NativeMethods.fbGetDisplayName(handle, sbName, maxNameSize);
            Log(string.Format("FSBrlDspAPIWrapper.fbGetDisplayName {0}", result ? "succeeded" : "failed"));

            int maxVersionSize = 100;
            StringBuilder sbVersion = new StringBuilder(maxVersionSize);
            result = NativeMethods.fbGetFirmwareVersion(handle, sbVersion, maxVersionSize);
            Log(string.Format("FSBrlDspAPIWrapper.fbGetFirmwareVersion {0}", result ? "succeeded" : "failed"));

            Log(string.Format("DeviceName={0} FirmwareVersion ={1} CellCount={2}", sbName.ToString(), sbVersion.ToString(), cellCount));

            return true;
        }



        public bool Write(byte[] buffer)
        {
#if Windows
            bool fbWriteResult = false;
            unsafe
            {
                fixed (byte* p = buffer)
                {
                    IntPtr ptr = (IntPtr)p;
                    fbWriteResult = NativeMethods.fbWrite(handle, 0, cellCount, ptr);
                    // do you stuff here
                }

            }

            Log(string.Format("FSBrlDspAPIWrapper.fbWrite {0}", fbWriteResult ? "succeeded" : "failed"));

            return fbWriteResult;

#elif Android
            return false;
#else
// #error Compiling for unknown platform
#endif


        }




        public bool Close()
        {
            bool result = NativeMethods.fbClose(handle);
            Log(string.Format("FSBrlDspAPIWrapper.fbGetFirmwareVersion {0}", result ? "succeeded" : "failed"));
            return result;
        }

    }

}
