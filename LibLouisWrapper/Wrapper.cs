using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.SqlServer.Server;
using MusicXmlReaderModel;

namespace LibLouisWrapper
{
    /// <summary>
    /// Simple wrapper class for using the LibLouis library (LibLouis.dll) from C#
    /// Intensionally only contains 4 public methods:
    ///  public static Wrapper Create()
    ///  public bool CharsToDots(string chars, out string dots, Typeforms[] sourceTypeformMap)
    ///  public bool DotsToChars(string dots, out string chars, Typeforms[] sourceTypeformMap)
    ///  public void Free()
    ///  
    /// More public methods can easily be added if needed. 
    /// 
    /// 
    /// Some ideas were stolen from the GitHub project LibLouis.Net 
    /// Official LibLouis documentation is found at
    /// https://liblouis.io/documentation/liblouis.html
    /// 
    /// Other recommended reading:  
    /// https://github.com/liblouis/liblouis/issues/1280
    /// https://stackoverflow.com/questions/20857649/c-dll-import-throws-marshall-directive-exception-in-c-sharp  
    /// Official LibLouis documentation is found at
    /// https://liblouis.io/documentation/liblouis.html
    /// </summary>

    public class Wrapper
    {

        /// <summary>
        /// As defined in liblouis.h
        /// </summary>
        [Flags]
        public enum TranslationModeEnum
        {
            NoContractions = 1,
            CompbrlAtCursor = 2,
            DotsIO = 4,
            // for historic reasons 8 and 16 are free
            CompbrlLeftCursor = 32,
            UnicodeBraille = 64, // In liblouis.h: ucBrl = 64,
            NoUndefined = 128,
            PartialTrans = 256
        }

        /// <summary>
        /// As defined in liblouis.h
        /// </summary>
        public enum Typeforms : ushort
        {
            None = 0,
            Italic = 1,
            Underline = 2,
            Bold = 4,
            Script = 8,
            TNEmbed = 16,
        }

        private static int globalErrorCount = 0; // Counts errors reported from LibLouis dll and is used for checking the Logger Callback mechanism

        const int translationMode = (int)(TranslationModeEnum.NoUndefined | TranslationModeEnum.UnicodeBraille); // Common for all member functions

        /// <summary>
        /// Path to be combined with tableName before passing to LibLouis
        /// Must contain the path to the conversion tables, relative to the path of LibLouis.dll.
        /// LibLouis.dll can find the exact absolute path to the tables using this information.
        /// </summary>
        private const string tableBase =  @"liblouis\share\liblouis\tables";  

        // The dll is placed in a folder named "binary" instead of "bin" to please the default GitExclude which wil not accept a "bin" folder.
        private const string LibLouisDll = @"Liblouis\binary\liblouis.dll";
        //private const string LibLouisDll = @"Liblouis\bin\liblouis.dll";

#region DllImport
        [DllImport(LibLouisDll, CallingConvention = CallingConvention.StdCall)]
        private static extern int lou_charSize();

        [DllImport(LibLouisDll, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private static extern string lou_version();

        [DllImport(LibLouisDll, CallingConvention = CallingConvention.StdCall)]
        private static extern int lou_charToDots(
            [In][MarshalAs(UnmanagedType.LPStr)] string tableList,
            [In][MarshalAs(UnmanagedType.LPArray)] byte[] inbuf,
            [Out][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] byte[] outbuf,
            [In] int length,
            [In] int mode
        );


        [DllImport(LibLouisDll, CallingConvention = CallingConvention.StdCall)]
        private static extern int lou_dotsToChar(
        [In][MarshalAs(UnmanagedType.LPStr)] string tableList,
        [In][MarshalAs(UnmanagedType.LPArray)] byte[] inbuf,
        [Out][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] byte[] outbuf,
        [In] int length,
        [In] int mode
        );

        [DllImport(LibLouisDll, CallingConvention = CallingConvention.StdCall)]
        private static extern void lou_free();
#endregion

#region LogCallBack
        private delegate void Func(int level, string message);
        private static void MyFunc(int level, string message)
        {
            Log(string.Format(": Received callback from LibLouis, describing an error: Level={0} Message={1}", level, message));
            globalErrorCount++;
        }
        [DllImport(LibLouisDll, CallingConvention = CallingConvention.StdCall)]
        private static extern void lou_registerLogCallback(Func callback);
#endregion 

#if true
        [DllImport(@"liblouis.dll", CharSet = CharSet.Unicode)]
        private static extern unsafe int lou_translateString(
                [In][MarshalAs(UnmanagedType.LPStr)] string tableList, // const char *tableList
                [In] byte[] inbuf,                                     // const widechar *inbuf
                [In, Out] IntPtr inlen,                                // int *inlen
                [Out] byte[] outbuf,                                   // widechar *outbuf 
                [In, Out] IntPtr outlen,                               // int *outlen  
                [In] Typeforms[] typeform,                             // formtype *typeform 
                [MarshalAs(UnmanagedType.LPStr)] string spacing,       // char *spacing
                int mode                                               //  int mode 
         );
#endif


        public bool CharsToDots(string chars, out string dots)
        {
            dots = "";
            byte[] converted = encoding.GetBytes(chars); // Encode the input string and set up buffers and int pointers.     
            int maxOutSize = Math.Max(chars.Length * (charSize * 2), 4096);
            byte[] outBuff = new byte[maxOutSize];

            // Note: Liblouis docs on typeforms says the input buffer should be the size of the max output buffer.
            // Yet this works current at the size of input.

            int result = lou_charToDots(tablePaths, converted, outBuff, chars.Length, translationMode); // Call native code to translate
            // Log(string.Format("lou_charToDots() returned result={0}", result));
            if (0 == result) return false;
            string translation = encoding.GetString(outBuff);      //Encode the translation
            dots = translation.TrimEnd(new char[] { '\0' }); // Remove all trailing null characters        
            return true;
        }


        public bool DotsToChars(string dots, out string chars)
        {
            chars = "";
            byte[] converted = encoding.GetBytes(dots);  // Encode the input string and set up buffers.
            int maxOutSize = Math.Max(dots.Length * (charSize * 2), 4096);
            byte[] outBuff = new byte[maxOutSize];

            // Note: Liblouis docs on typeforms says the input buffer should be the size of the max output buffer.
            // Yet this works current at the size of input.

            int result = lou_dotsToChar(tablePaths, converted, outBuff, dots.Length, translationMode); // Call native code to translate
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


        private static void Log(string s)
        {
            Console.WriteLine(s);
            Logger.LogCF1(s);    // Append Class and Function for the function calling Log()    
        }



#if true
        public string TranslateString(string text, Typeforms[] sourceTypeformMap)
        {
            //Encode the input string and set up buffers and int pointers.
            var converted = encoding.GetBytes(text);
            var maxInSize = text.Length * charSize;

            //Set up the output buffers.
            var maxOutSize = Math.Max(text.Length * (charSize * 2), 4096);
            var outBuff = new byte[maxOutSize];

            var translation = "";

            unsafe
            {
                var intPtr = new IntPtr(&maxInSize);
                var outPrt = new IntPtr(&maxOutSize);
                int mode = (int)(TranslationModeEnum.NoUndefined | TranslationModeEnum.UnicodeBraille);

                //Note: Liblouis docs on typeforms says the input buffer should be the size of the max output buffer.
                //Yet this works current at the size of input.
                //Run the translation
                lou_translateString(
                    tablePaths, // const char *tableList
                    converted,  // const widechar *inbuf,
                    intPtr,     // int * inlen,
                    outBuff,    // widechar *outbuf,
                    outPrt,     // int *outlen,
                    sourceTypeformMap, // formtype *typeform,
                    null,       //  char *spacing
                    mode);      //  int mode

                Array.Resize(ref outBuff, maxOutSize * charSize);
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

        // Member variables:
        private int charSize;
        private Encoding encoding;
        private string tablePaths;

        /// <summary>
        /// Simple mechanism used by the constructor only.
        /// Tests the LibLouis Log-Callback mechanism.
        /// </summary>
        private void ExecuteCallbackTest()
        {
            string testItem = " the LibLouis Log-Callback mechanism!";
            Log(string.Format(": Simulating error on order to test{0}",testItem));
            string teststring;
            int oldErrorCount = globalErrorCount;
            CharsToDots("x", out teststring); // Is expected to fail and thereby to increase globalErrorCount;
            bool ok = (globalErrorCount > oldErrorCount);          
            Log(string.Format(": TEST {0}! Simulated error was {1} reported from LibLouis by{2} !", ok ? "PASSED" : "FAILED", ok ? "": "NOT", testItem));       
        }



/// <summary>
/// Private constructor. Use Wrapper.Create() from the outside.
/// </summary>
private Wrapper(string tableNames)
        {
            Log(string.Format(": TableNames='{0}'", tableNames));
            Log(string.Format(": Registering LibLouis LogCallback function"));
            lou_registerLogCallback(MyFunc); // Register the static function MyFunc as a callback""
            charSize = lou_charSize();
            Log(string.Format(": CharSize={0}", charSize));
            encoding = GetEncoding(charSize);  // Get the encoding type based on the lou_charSize.
            Log(string.Format(": Encoding={0}", encoding.ToString()));

            // Check the Logging callback mechanism:
            tablePaths = Path.Combine(tableBase, "DoesNotExist.xxx"); // Temporarily set up a nonexisting tablepath while checking
            ExecuteCallbackTest();       

            // Set up the real translation table
            tablePaths = Path.Combine(tableBase,tableNames); // According to the documentation only the first name needs to contain the tableBase !! 
            Log(string.Format(": Tables='{0}'", tablePaths));
        }

        /// <summary>
        /// Pevent use of default constructor
        /// </summary>
        private Wrapper(){ }

        public static Wrapper Create(string tableNames)
        { return new Wrapper(tableNames); }

    }
}
