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
    }
}
