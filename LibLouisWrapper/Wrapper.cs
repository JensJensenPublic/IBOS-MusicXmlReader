using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LibLouisWrapper
{
    /// <summary>
    /// Ideas stolen from the GitHub project LibLouis.Net
    /// 
    /// Other recommended reading: 
    /// 
    /// https://github.com/liblouis/liblouis/issues/1280
    /// https://stackoverflow.com/questions/20857649/c-dll-import-throws-marshall-directive-exception-in-c-sharp
    /// 
    /// 
    /// </summary>


        public static class Wrapper
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


        [DllImport(@"liblouis.dll", CharSet = CharSet.Unicode)]
        private static extern unsafe int lou_translateString(
     [MarshalAs(UnmanagedType.LPStr)]
            [In] string tableList,
     [In] byte[] inbuf,
     [In, Out] IntPtr inlen,
     [Out] byte[] outbuf,
     [In, Out] IntPtr outlen,
     [In] Typeforms[] typeform,
     [MarshalAs(UnmanagedType.LPStr)]
            string spacing,
     int mode);



        public static string TranslateString(string text, Typeforms[] sourceTypeformMap)
        {
            //Get the encoding type based on the lou_charSize.
            var size = lou_charSize();
            var encoding = GetEncoding(size);

            //Encode the input string and set up buffers and int pointers.
            var converted = encoding.GetBytes(text);
            var maxInSize = text.Length * size;

            //Set up the output buffers.
            var maxOutSize = Math.Max(text.Length * (size * 2), 4096);
            var outBuff = new byte[maxOutSize];

            var translation = "";

            //Get the translation table
            //var tables = @"liblouis\tables\en-ueb-g2.ctb";
            var tables = @"en-ueb-g2.ctb";


            unsafe
            {
                var intPtr = new IntPtr(&maxInSize);
                var outPrt = new IntPtr(&maxOutSize);

                //Note: Liblouis docs on typeforms says the input buffer should be the size of the max output buffer.
                //Yet this works current at the size of input.
                //Run the translation
                lou_translateString(tables, converted, intPtr,
                    outBuff, outPrt, sourceTypeformMap, null, 128);

                Array.Resize(ref outBuff, maxOutSize * size);
                //Decode the translation
                translation = encoding.GetString(outBuff);
            }

            //trim out any empty characters.
            return translation;
        }

        /// <summary>
        /// Gets the encoding based on the character size from libluois
        /// </summary>
        /// <param name="size">Character size, 4 bytes is UTF-32, anything else is UTF-16</param>
        /// <returns>Character encoding</returns>
        private static Encoding GetEncoding(int size)
        {
            if (size == 4)
            {
                return Encoding.GetEncoding("UTF-32");
            }

            return Encoding.GetEncoding("UTF-16");
        }

        public enum Typeforms : ushort
        {
            None = 0,
            Italic = 1,
            Underline = 2,
            Bold = 4,
            Script = 8,
            TNEmbed = 16,
        }



    }
}
