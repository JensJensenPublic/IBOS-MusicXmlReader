using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using MusicXmlReaderModel; 

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
    /// Official LibLouis documentation is found at
    /// https://liblouis.io/documentation/liblouis.html
    /// 
    /// 
    /// </summary>




    public class Wrapper
        {

        /// <summary>
        /// As defined in liblouis.h
        /// </summary>
        [Flags]
        public enum TranslationModeEnum {
            NoContractions = 1,
            CompbrlAtCursor = 2,
            DotsIO = 4,
            // for historic reasons 8 and 16 are free
            CompbrlLeftCursor = 32,
            UnicodeBraille = 64 , // In liblouis.h: ucBrl = 64,
            NoUndefined = 128,
            PartialTrans = 256
        }

        const int translationMode = (int)(TranslationModeEnum.NoUndefined | TranslationModeEnum.UnicodeBraille); // Common for all member functions


        // The dll is placed in a folder named "binary" instead of "bin" to please the default GitExclude which wil not accept a "bin" folder.
        private const string LibLouisDll = @"Liblouis\binary\liblouis.dll";
        //private const string LibLouisDll = @"Liblouis\bin\liblouis.dll";

        [DllImport(LibLouisDll, CallingConvention = CallingConvention.StdCall)]
            public static extern int lou_charSize();

            [DllImport(LibLouisDll, CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.LPStr)]
            public static extern string lou_version();

            [DllImport(LibLouisDll, CallingConvention = CallingConvention.StdCall)]
            public static extern int lou_charToDots(
                [In]  [MarshalAs(UnmanagedType.LPStr)] string tableList,
                [In]  [MarshalAs(UnmanagedType.LPArray)] byte[] inbuf,
                [Out] [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] byte[] outbuf,
                [In] int length,
                [In] int mode
            );


            [DllImport(LibLouisDll, CallingConvention = CallingConvention.StdCall)]
            public static extern int lou_dotsToChar(
            [In][MarshalAs(UnmanagedType.LPStr)] string tableList,
            [In][MarshalAs(UnmanagedType.LPArray)] byte[] inbuf,
            [Out][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] byte[] outbuf,
            [In] int length,
            [In] int mode
            );

            [DllImport(LibLouisDll, CallingConvention = CallingConvention.StdCall)]
            public static extern void lou_free();

#if false
        [DllImport(@"liblouis.dll", CharSet = CharSet.Unicode)]
        private static extern unsafe int lou_translateString(
                [In][MarshalAs(UnmanagedType.LPStr)] string tableList,
                [In] byte[] inbuf,
                [In, Out] IntPtr inlen,
                [Out] byte[] outbuf,
                [In, Out] IntPtr outlen,
                [In] Typeforms[] typeform,
                [MarshalAs(UnmanagedType.LPStr)] string spacing,
                int mode
         );
#endif


        public bool CharsToDots(string chars, out string dots, Typeforms[] sourceTypeformMap)
        {
            dots = ""; 
            byte[] converted = encoding.GetBytes(chars); // Encode the input string and set up buffers and int pointers.     
            int maxOutSize = Math.Max(chars.Length * (charSize * 2), 4096);
            byte[] outBuff = new byte[maxOutSize]; 

            // Note: Liblouis docs on typeforms says the input buffer should be the size of the max output buffer.
            // Yet this works current at the size of input.

            int result = lou_charToDots(tables, converted, outBuff, chars.Length, translationMode); // Call native code to translate
            // Log(string.Format("lou_charToDots() returned result={0}", result));
            if (0 == result) return false; 
            string translation = encoding.GetString(outBuff);      //Encode the translation
            dots = translation.TrimEnd(new char[] {'\0'} ); // Remove all trailing null characters        
            return true;
        }


        public bool DotsToChars(string dots, out string chars, Typeforms[] sourceTypeformMap)
        {
            chars = "";           
            byte[] converted = encoding.GetBytes(dots);  // Encode the input string and set up buffers.
            int maxOutSize = Math.Max(dots.Length * (charSize * 2), 4096);
            byte[] outBuff = new byte[maxOutSize];

            // Note: Liblouis docs on typeforms says the input buffer should be the size of the max output buffer.
            // Yet this works current at the size of input.

            int result = lou_dotsToChar(tables, converted, outBuff, dots.Length, translationMode); // Call native code to translate
            // Log(string.Format("lou_charToDots() returned result={0}", result));
            if (0 == result) return false;
            string translation = encoding.GetString(outBuff);      //Encode the translation
            chars = translation.TrimEnd(new char[] { '\0' }); // Remove all trailing null characters        
            return true;
        }

        public void Free()
        {
            lou_free();
        }


        private void Log(string s)
        {
            Console.WriteLine(s);
            Logger.LogCF1(": " + s);    // Append Class and Function for the function calling Log()    
        }



#if false
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
            var tables = @"liblouis\share\liblouis\tables\en-ueb-g2.ctb";
            //var tables = @"en-ueb-g2.ctb";


            unsafe
            {
                var intPtr = new IntPtr(&maxInSize);
                var outPrt = new IntPtr(&maxOutSize);
                int mode = (int)(TranslationModeEnum.NoUndefined | TranslationModeEnum.UnicodeBraille);

                //Note: Liblouis docs on typeforms says the input buffer should be the size of the max output buffer.
                //Yet this works current at the size of input.
                //Run the translation
                lou_translateString(tables, converted, intPtr,
                    outBuff, outPrt, sourceTypeformMap, null, mode);

                Array.Resize(ref outBuff, maxOutSize * size);
                //Decode the translation
                translation = encoding.GetString(outBuff);
            }

            //trim out any empty characters.
            return translation;
        }
#endif
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

        public int CharSize { get { return charSize; } }

        private int charSize;
        Encoding encoding;
        string tables;

        private Wrapper()
        {
            // Get the encoding type based on the lou_charSize.
            charSize = lou_charSize(); 
            encoding = GetEncoding(charSize);
            // Get the translation table
            tables = @"liblouis\share\liblouis\tables\en-ueb-g2.ctb"; // Only one table used in this case
        }
        public static Wrapper Create()
        { return new Wrapper(); }   

    }
}
