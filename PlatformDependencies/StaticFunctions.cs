using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformDependencies
{

    public enum PlatformEnum { Windows, Android, iOS };

    public class StaticFunctions
    {

        public PlatformEnum Platform
        {
            get
            {
#if Windows
                return PlatformEnum.Windows;
#elif Android
                     return PlatformEnum.Android;
#elif iOS
                     return PlatformEnum.iOS;          
#else
#error Compiling for unknown platform
#endif
            }
        }

        public static string GetPlatformTempDirectory()
        {
#if Windows
            return System.IO.Path.GetTempPath();
#elif Android
            return (string)Android.OS.Environment.ExternalStorageDirectory;
#else
#error Compiling for unknown platform
#endif
        }


        public static int GetKnownFolderPath(Guid guid, uint dwFlags, IntPtr hToken, out IntPtr ppszPath)
        {
#if Windows
            return NativeMethods.SHGetKnownFolderPath(guid, dwFlags, hToken, out ppszPath);
#elif Android
            // Not implemented (yet)
            ppszPath = null;
            return -1;
#else
#error Compiling for unknown platform
#endif
        }


        public static string GetSystemMessage(int errorCode)
        {
#if Windows
            return NativeMethods.GetSystemMessage(errorCode);
#elif Android
            // Not implemented (yet)
            ppszPath = null;
            return -1;
#else
#error Compiling for unknown platform
#endif
        }

        /// <summary>
        /// Associate the estension with the application
        /// </summary>
        /// <param name="extension"></param>
        /// <param name="applicationPath"></param>
        /// <returns></returns>
        public static void RegisterForFileExtension(string extension, string applicationPath)
        {
#if Windows
            NativeMethods.AssociateFileExtension(extension, "IBOS_MusicXmlReader", applicationPath);
            //NativeMethods.RegisterForFileExtension(extension, applicationPath);
#elif android
#else
#error Compiling for unknown platform
#endif
        }


        /// <summary>
        /// Primarily for debugging: 
        /// Lists the assocations for a given file extension, as found in CurrentUser, LocalMAchine and ClassesRoot
        /// </summary>
        /// <param name="extension">The extension to look for</param>
        /// <returns></returns>
        public static List<string> GetAssociationInformation(string extension)
        {
#if Windows   
  
            List<string> result = new List<string>();
            RegistryKey FileReg = null;

            string HKCU_value = "";
            try
            {
                FileReg = Registry.CurrentUser.CreateSubKey("Software\\Classes\\" + extension);
                HKCU_value = FileReg.OpenSubKey("shell\\open\\command").GetValue("").ToString();
            }
            catch (Exception e)
            {
                HKCU_value = "Can not be read! Message=" + e.Message;
            }
            if (null != FileReg) {FileReg.Close();}

            string HKLM_value = "";
            try
            {
                FileReg = Registry.LocalMachine.CreateSubKey("Software\\Classes\\" + extension); 
                HKLM_value = FileReg.OpenSubKey("shell\\open\\command").GetValue("").ToString();
            }
            catch (Exception e)
            {
                HKLM_value = "Can not be read! Message=" + e.Message;
            }
            if (null != FileReg) { FileReg.Close(); }


            string HKCR_value = "";
            try
            {
                FileReg = Registry.ClassesRoot.CreateSubKey("" + extension);
                HKCR_value = FileReg.OpenSubKey("shell\\open\\command").GetValue("").ToString();
            }
            catch (Exception e)
            {
                HKCR_value = "Can not be read! Message=" + e.Message;
            }
            if (null != FileReg) { FileReg.Close(); }
      

            result.Add("HKCU: " + HKCU_value);
            result.Add("HKLM: " + HKLM_value);
            result.Add("HKCR: " + HKCR_value);
            return result;
        }
#elif android
return  new List<string>();;
#else
#error Compiling for unknown platform
#endif
    }

}
