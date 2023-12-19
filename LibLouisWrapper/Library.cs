using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LibLouisWrapper
{

    public static class Library
    {
        [DllImport("liblouis.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int lou_charSize();

        [DllImport("liblouis.dll", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public static extern string lou_version();

        [DllImport("liblouis.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int lou_charToDots(
            [MarshalAs(UnmanagedType.LPStr)]
            [In]
            string tableList,
            [In]
            [MarshalAs(UnmanagedType.LPArray)]
            byte[] inbuf,
            [Out]
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)]
            byte[] outbuf,
            [Out]
            int length,
            int mode
        );
    }
}
