using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.SqlServer.Server;
using MusicXmlReaderModel;
using static System.Net.Mime.MediaTypeNames;
using static LibLouisWrapper.Wrapper;

namespace LibLouisWrapper
{
    /// <summary>
    /// Simple wrapper class for using the LibLouis library (LibLouis.dll) from C#
    /// Intensionally only contains 6 public methods:
    ///  public static Wrapper Create()
    ///  public bool CharsToDots(string chars, out string dots, Typeforms[] sourceTypeformMap)
    ///  public bool DotsToChars(string dots, out string chars, Typeforms[] sourceTypeformMap)
    ///  public bool TranslateString(string text, out string dots, Typeforms[] sourceTypeformMap)
    ///  public bool BackTranslateString(string inputDots, out string outputText, Typeforms[] sourceTypeformMap)
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
        public enum TypeformEnum : ushort
        {
            None = 0,
            Italic = 1,
            Underline = 2,
            Bold = 4,
            Script = 8,
            TNEmbed = 16,
        }

        private int depricatedModeParameter = 0;
        private static int globalErrorCount = 0; // Counts errors reported from LibLouis dll and is used for checking the Logger Callback mechanism

        const int translationMode = (int)(TranslationModeEnum.NoUndefined | TranslationModeEnum.UnicodeBraille | TranslationModeEnum.DotsIO); // Common for all member functions
        const int translationMode1 = (int)(TranslationModeEnum.UnicodeBraille); // For experiment

        /// <summary>
        /// Path to be combined with tableName before passing to LibLouis
        /// Must contain the path to the conversion tables, relative to the path of LibLouis.dll.
        /// LibLouis.dll can find the exact absolute path to the tables using this information.
        /// </summary>
        private const string tableBase = @"liblouis\share\liblouis\tables";

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
                [In] TypeformEnum[] typeform,                             // formtype *typeform 
                [MarshalAs(UnmanagedType.LPStr)] string spacing,       // char *spacing
                int mode                                               //  int mode 
         );
#endif

#if true
        [DllImport(@"liblouis.dll", CharSet = CharSet.Unicode)]
        private static extern unsafe int lou_backTranslateString(
                [In][MarshalAs(UnmanagedType.LPStr)] string tableList, // const char *tableList
                [In] byte[] inbuf,                                     // const widechar *inbuf
                [In, Out] IntPtr inlen,                                // int *inlen
                [Out] byte[] outbuf,                                   // widechar *outbuf 
                [In, Out] IntPtr outlen,                               // int *outlen  
                [In] TypeformEnum[] typeform,                             // formtype *typeform 
                [MarshalAs(UnmanagedType.LPStr)] string spacing,       // char *spacing
                int mode                                               //  int mode 
         );
#endif

#if true
        private enum NativeFunctionEnum
        {
            charsToDots,
            dotsToChars,
            translateString,
            backTranslateString 
        }

        public bool CharsToDots1(string chars, out string dots) { return CommonNativeCall(chars, out dots, NativeFunctionEnum.charsToDots); }
        public bool DotsToChars1(string dots, out string chars) { return CommonNativeCall(dots, out chars, NativeFunctionEnum.dotsToChars); }
        public bool TranslateString1(string text, out string dots) { return CommonNativeCall(text, out dots, NativeFunctionEnum.translateString); }
        public bool BackTranslateString1(string inputDots, out string outputText) { return CommonNativeCall(inputDots, out outputText, NativeFunctionEnum.backTranslateString); }

        private int GetOutputLength(int inputLength, NativeFunctionEnum nativeFunctionEnum)
        {
            int defaultResult = Math.Max((inputLength * 2), 1024);  // Twice the inputbuffer size, but at least 1kB
            switch (nativeFunctionEnum)
            {
                case NativeFunctionEnum.charsToDots: break;
                case NativeFunctionEnum.dotsToChars: break;
                case NativeFunctionEnum.translateString: break;
                case NativeFunctionEnum.backTranslateString: break;
            }
            return defaultResult;
        }

        private bool CommonNativeCall(string input, out string output, NativeFunctionEnum nativeFunctionEnum )
        {     
            output = null;
            int inputLength = input.Length;          
            byte[] inBuf = encoding.GetBytes(input);
            int outputLength = GetOutputLength(inBuf.Length, nativeFunctionEnum);
            byte[] outBuf = new byte[outputLength]; 
            int result = 0;
            unsafe
            {
                IntPtr inPtr = new IntPtr(&inputLength);
                IntPtr outPrt = new IntPtr(&outputLength);

                fixed (byte* pInBuf = inBuf, pOutBuf = outBuf) // Prevents GarbageCollector from moving the buffers
                {
                    switch (nativeFunctionEnum)
                    {
                        case NativeFunctionEnum.charsToDots: result = lou_charToDots(tablePaths, inBuf, outBuf, inputLength, translationMode); break;
                        case NativeFunctionEnum.dotsToChars: result = lou_dotsToChar(tablePaths, inBuf, outBuf, inputLength, depricatedModeParameter); break;
                        case NativeFunctionEnum.translateString: result = lou_translateString( tablePaths,inBuf, inPtr,  outBuf, outPrt,  null, null, translationMode); break;
                        case NativeFunctionEnum.backTranslateString: result = lou_backTranslateString(tablePaths, inBuf, inPtr, outBuf, outPrt, null, null, depricatedModeParameter); break;                                                
                    } 
                }
            }
            if (1 != result) return OnError( "1 != result");          
            if (null == outBuf) return OnError("null == outBuf");      
            string s = encoding.GetString(outBuf);  // Decode
            output = s.TrimEnd(new char[] { '\0' }); // Remove all trailing null characters 
            Logger.LogCF(string.Format("({0},'{1}' = '{2}'", nativeFunctionEnum, input, output));
            return true;
        }

        private bool OnError(string s)
        {
            Logger.LogCF1(string.Format(": Error: '{0}'", s));
            return false;        
        }


#endif

        public void Free()
        {
            lou_free();
        }


        private static void Log(string s)
        {
            Console.WriteLine(s);
            Logger.LogCF1(s);    // Append Class and Function for the function calling Log()    
        }



#if false

        public bool CharsToDots(string chars, out string dots)
        {
            dots = "";
            BufferStructure bs = BufferStructure.Create(chars, encoding);     

            // Note: Liblouis docs on typeforms says the input buffer should be the size of the max output buffer.
            // Yet this works current at the size of input.

            int result = lou_charToDots(tablePaths, bs.InputBuffer, bs.OutputBuffer, chars.Length, translationMode); // Call native code to translate
            // Log(string.Format("lou_charToDots() returned result={0}", result));
            if (0 == result) return false;
            return bs.GetTranslation(out dots);        
        }


        public bool DotsToChars(string dots, out string chars)
        {
            chars = "";
            BufferStructure bs = BufferStructure.Create(dots, encoding);

            // Note: Liblouis docs on typeforms says the input buffer should be the size of the max output buffer.
            // Yet this works current at the size of input.
          
            int result = lou_dotsToChar(tablePaths, bs.InputBuffer, bs.OutputBuffer, dots.Length, depricatedModeParameter); // Call native code to translate. The "mode" parameter is deprivated and set to 0
            // Log(string.Format("lou_charToDots() returned result={0}", result));
            if (0 == result) return false;
            return  bs.GetTranslation(out chars); 
        }

   


        public bool TranslateString(string text, out string dots, out TypeformEnum[] typeformEnums)
        {
            dots = "";
            typeformEnums = null;
            BufferStructure bs = BufferStructure.Create(text, encoding);
            int inSize =  bs.MaxInSize;    // Will be changed during the operation
            int outSize = bs.MaxOutSize;  // Will be changed during the operation
            int iResult = 0;

            unsafe
            {
                IntPtr inPtr = new IntPtr(&inSize);
                IntPtr outPrt = new IntPtr(&outSize);

                //Note: Liblouis docs on typeforms says the input buffer should be the size of the max output buffer.
                //Yet this works current at the size of input.

                //Run the translation
                iResult = lou_translateString(
                    tablePaths,         // const char *tableList
                    bs.InputBuffer,     // const widechar *inbuf,
                    inPtr,              // int * inlen,
                    bs.OutputBuffer,    // widechar *outbuf,
                    outPrt,             // int *outlen,
                    bs.TypeFormBuffer,  // formtype *typeform,
                    null,               // char *spacing
                    translationMode);   // int mode
            }
            Log(string.Format(": lou_translateString('{0}') returned {1}", text, iResult));
            if ( 0 == iResult) return false;
            bool ok = bs.GetTranslation(out dots);
            Log(string.Format(": lou_translateString='{0}'  ", dots));
            Log(string.Format(": lou_translateString() changed InSize from {0} to {1} Changed OutSize from {2} to {3}", bs.MaxInSize, inSize, bs.MaxOutSize, outSize));
            bs.GetTypeForms(out typeformEnums,outSize); 
            return ok;
        }



        public bool BackTranslateString(string inputDots, out string outputText, out TypeformEnum[] typeformEnums)
        {
            outputText = "";
            typeformEnums = null;
            BufferStructure bs = BufferStructure.Create(inputDots, encoding);
            int inSize = bs.MaxInSize;    // Will be changed during the operation
            int outSize = bs.MaxOutSize;  // Will be changed during the operation
            int iResult = 0;

            unsafe
            {
                IntPtr inPtr = new IntPtr(&inSize);
                IntPtr outPrt = new IntPtr(&outSize);

                //Note: Liblouis docs on typeforms says the input buffer should be the size of the max output buffer.
                //Yet this works current at the size of input.

                //Run the translation
                iResult = lou_backTranslateString(
                    tablePaths,         // const char *tableList
                    bs.InputBuffer,     // const widechar *inbuf,
                    inPtr,              // int * inlen,
                    bs.OutputBuffer,    // widechar *outbuf,
                    outPrt,             // int *outlen,
                    bs.TypeFormBuffer,  // formtype *typeform,
                    null,               //  char *spacing
                    depricatedModeParameter);  //  int mode. Depricated for this function. MUST BE SET TO 0 !!
            }

            Log(string.Format(": lou_backTranslateString('{0}') returned {1}", inputDots, iResult));
            if (0 == iResult) return false;
            bool ok = bs.GetTranslation(out outputText);
            Log(string.Format(": lou_backTranslateString='{0}'  ", outputText));
            Log(string.Format(": lou_backTranslateString() changed InSize from {0} to {1} Changed OutSize from {2} to {3}", bs.MaxInSize, inSize, bs.MaxOutSize, outSize));
            bs.GetTypeForms(out typeformEnums, outSize);
            return ok;
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
            CharsToDots1("x", out teststring); // Is expected to fail and thereby to increase globalErrorCount;
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
        {
            Wrapper wrapper =  new Wrapper(tableNames);
            //string s;
            //wrapper.CommonNativeCall("X", out s, NativeFunctionEnum.charsToDots);
            return wrapper;
        }

    }
}
