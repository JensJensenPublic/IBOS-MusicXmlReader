using System;
using System.ComponentModel;
using System.Diagnostics.SymbolStore;
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

        // liblouis.h contains: LIBLOUIS_API const char *EXPORT_CALL lou_version(void);
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
            globalErrorCount++;
            if (ignoreFirstError) return; // Do not log simulated  error generated for test-purposes !
            Log(string.Format(": Received callback from LibLouis, describing an error: Level={0} Message={1}", level, message));
           
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
            translateString,       // Do NOT Use the TypeFormEnum parameter
            translateStringTfe,    // Use the TypeFormEnum parameter
            backTranslateString,    // Do NOT Use the TypeFormEnum parameter
            backTranslateStringTfe // Use the TypeFormEnum parameter
        }

        //private  TypeformEnum[] dummyTfe = null;

        public bool CharsToDots1(string chars, out string dots) { return CommonNativeCall(NativeFunctionEnum.charsToDots, chars, out dots); }
        public bool DotsToChars1(string dots, out string chars) { return CommonNativeCall(NativeFunctionEnum.dotsToChars, dots, out chars); }
        public bool TranslateString1(string text, out string dots) { return CommonNativeCall(NativeFunctionEnum.translateString, text, out dots); }
        public bool TranslateStringTFE(string text, out string dots, out TypeformEnum[] tfe) { return CommonNativeCall(NativeFunctionEnum.translateStringTfe, text, out dots, out tfe); }
        public bool BackTranslateString1(string dots, out string text) { return CommonNativeCall(NativeFunctionEnum.backTranslateString, dots, out text); }
        public bool BackTranslateStringTFE(string dots, out string text, out TypeformEnum[] tfe) { return CommonNativeCall(NativeFunctionEnum.backTranslateStringTfe, dots, out text, out tfe); }

        public string GetVersion()
        {
#warning TODO Implement  using lou_version()          
            string result = "could not be determined !";
            // 
            // This is just a silly, temporary solution !
            try
            {
                //throw new Exception(""); // For test
                string executingDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                string fileName = Path.Combine(executingDirectory, @"liblouis\lib\pkgconfig\liblouis.pc");
                if (File.Exists(fileName))
                {
                    Log(string.Format(": Found file {0}", fileName));
                    string[] lines = File.ReadAllLines(fileName);
                    string versionPrompt = "Version:";
                    foreach (string line in lines)
                    {
                        if (line.StartsWith(versionPrompt))
                        {
                            return line.Replace(versionPrompt,"");
                        }                    
                    }
                }
            }
            catch (Exception e)
            {
                Log(string.Format(": Exception caught while attempting to read LibLouis version. Message={0}", e.Message));
            
            }
            return result;            
        }

        private int GetOutputLength(int inputLength, NativeFunctionEnum nativeFunctionEnum)
        {
            int defaultResult = Math.Max((inputLength * 2), 1024);  // Twice the inputbuffer size, but at least 1kB
            switch (nativeFunctionEnum)
            {
                case NativeFunctionEnum.charsToDots: break;
                case NativeFunctionEnum.dotsToChars: break;
                case NativeFunctionEnum.translateStringTfe: break;
                case NativeFunctionEnum.backTranslateStringTfe: break;
            }
            return defaultResult;
        }

        

        private int GetTfeLength(int inputLength, NativeFunctionEnum nativeFunctionEnum)
        { 
            switch (nativeFunctionEnum) 
            {
                case NativeFunctionEnum.translateStringTfe:
                case NativeFunctionEnum.backTranslateStringTfe:return (inputLength * 2); // Twice the inputbuffer size,
            }
            return 0;
        }

        private bool CommonNativeCall(NativeFunctionEnum nativeFunctionEnum, string input, out string output)
        {
            TypeformEnum[] dummyTfe = new TypeformEnum[0];
            return CommonNativeCall(nativeFunctionEnum, input, out output, out dummyTfe);
        }


        private bool CommonNativeCall(NativeFunctionEnum nativeFunctionEnum, string input, out string output, out TypeformEnum[] tfe)
        {     
            output = null;
            tfe = null;
            int inputLength = input.Length;          
            byte[] inBuf = encoding.GetBytes(input);
            int outputLength = GetOutputLength(inBuf.Length, nativeFunctionEnum);
            int initialOutputLength = outputLength; // Only used for logging 
            byte[] outBuf = new byte[outputLength];
            int tfeLength = GetTfeLength(input.Length, nativeFunctionEnum);
            TypeformEnum[] tfeBuf = new TypeformEnum[tfeLength];
            int result = 0;
            unsafe
            {
                IntPtr inPtr = new IntPtr(&inputLength);
                IntPtr outPrt = new IntPtr(&outputLength);

                fixed (byte* pInBuf = inBuf, pOutBuf = outBuf) // Prevents GarbageCollector from moving the buffers
                {
                    fixed (TypeformEnum* pTfeBuf = tfeBuf) // Two levels are needed for fixing different types !
                    {
                        switch (nativeFunctionEnum)
                        {
                            case NativeFunctionEnum.charsToDots: result = lou_charToDots(tablePaths, inBuf, outBuf, inputLength, translationMode); break;
                            case NativeFunctionEnum.dotsToChars: result = lou_dotsToChar(tablePaths, inBuf, outBuf, inputLength, depricatedModeParameter); break;
                            case NativeFunctionEnum.translateString:        result = lou_translateString(tablePaths, inBuf, inPtr, outBuf, outPrt, null, null, translationMode); break;
                            case NativeFunctionEnum.translateStringTfe:     result = lou_translateString(tablePaths, inBuf, inPtr, outBuf, outPrt, tfeBuf, null, translationMode); break;
                            case NativeFunctionEnum.backTranslateString:    result = lou_backTranslateString(tablePaths, inBuf, inPtr, outBuf, outPrt, null, null, depricatedModeParameter); break;
                            case NativeFunctionEnum.backTranslateStringTfe: result = lou_backTranslateString(tablePaths, inBuf, inPtr, outBuf, outPrt, tfeBuf, null, depricatedModeParameter); break;
                        }
                        fixed (byte* pInBufAfter = inBuf, pOutBufAfter = outBuf)
                        {
                            CheckPinning("InBuf ", (int)pInBuf, (int)pInBufAfter);
                            CheckPinning("OutBuf", (int)pOutBuf, (int)pOutBufAfter);
                        }
                        fixed (TypeformEnum*  pTfeBufAfter = tfeBuf)
                        {   
                            CheckPinning("TfeBuf ", (int)pTfeBuf, (int)pTfeBufAfter);         
                        }
                    }
                }
            }
            if (1 != result) return OnError( "1 != result");          
            if (null == outBuf) return OnError("null == outBuf");
            Log(string.Format(": OutputLength changed from {0} to {1}", initialOutputLength, outputLength));
            output = GetOutputString(nativeFunctionEnum, outBuf, outputLength, charSize);
            tfe = tfeBuf;           
            Log(string.Format("({0},'{1}')='{2}'", nativeFunctionEnum, input, output));
            Log(string.Format("(...) Tfe: {0}", TfeToString(tfe)));
            Log(string.Format("(...) Outbuf.Length={0} output.Length={1}", outBuf.Length, output.Length)); // During initial debugging  
            return true;
        }

        /// <summary>
        /// If the length of the outputbuffer received from native code is known we use that information.
        /// Otherwise we just remove any tariling null-vharacters.
        /// </summary>
        /// <param name="nativeFunctionEnum"></param>
        /// <param name="output"></param>
        /// <param name="outputLength"></param>
        /// <param name="charSize"></param>
        /// <returns></returns>
        private string GetOutputString(NativeFunctionEnum nativeFunctionEnum, byte[] output, int outputLength, int charSize)
        {
            string s;
            if (OutputLengthIsKnown(nativeFunctionEnum))
            {
                s = encoding.GetString(output, 0, outputLength * charSize); // Only use the relevalt part of the outputbuffer
            }
            else
            {
                s = encoding.GetString(output); // The whole outputbuffer
            }
            return s.TrimEnd(new char[] { '\0' }); // Remove all trailing null characters
        }

        private bool OutputLengthIsKnown(NativeFunctionEnum nativeFunctionEnum)
        {  
            switch (nativeFunctionEnum)
            {
                case NativeFunctionEnum.translateString: return true; 
                case NativeFunctionEnum.backTranslateString: return true;
                case NativeFunctionEnum.translateStringTfe: return true; 
                case NativeFunctionEnum.backTranslateStringTfe: return true;
            }
            return false;
        }



        private string TfeToString(TypeformEnum[] tfe)
        {
            if (null == tfe) return "null";
            StringBuilder sb = new StringBuilder();
            foreach (TypeformEnum t in tfe)
            {
                sb.Append(String.Format("{0:x} ", (int)t));
            }
            return(string.Format("Length={0} HexValues={1}", tfe.Length, sb.ToString()));
        }

        private void CheckPinning(string id, int pBefore, int pAfter)
        {
            if (pBefore == pAfter)
            {
                // Log(string.Format(": Passed!"));
                return; 
            }
            string message = string.Format(": The buffer '{0}' changed from {1} to {2} during call to native code - even if it was supposed to be pinned!", id, pBefore, pAfter);
            Log(message);
            throw new Exception(message);
        }


        private bool OnError(string s)
        {
            Log(string.Format(": Error: '{0}'", s));
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
        private static bool ignoreFirstError = false;

        /// <summary>
        /// Simple mechanism used by the constructor only.
        /// Tests the LibLouis Log-Callback mechanism.
        /// </summary>
        private void ExecuteCallbackTest()
        {
           
            string testItem = " the LibLouis Log-Callback mechanism!";
            Log(string.Format(": Simulating error in order to test{0}",testItem));
            string teststring;
            int oldErrorCount = globalErrorCount;
            ignoreFirstError = true;
            CharsToDots1("x", out teststring); // Is expected to fail and thereby to increase globalErrorCount;
            ignoreFirstError = false;
            bool ok = (globalErrorCount > oldErrorCount);          
            Log(string.Format(": TEST {0}! Simulated error was {1} reported from LibLouis by{2} !", ok ? "PASSED" : "FAILED", ok ? "": "NOT", testItem));       
        }

        private readonly Func myFunc; // Only for preventing GC from collecting the delegate

        /// <summary>
        /// Private constructor. Use Wrapper.Create() from the outside.
        /// </summary>
        private Wrapper(string tableNames)
        {
            myFunc = MyFunc;
            Log(string.Format(": TableNames='{0}'", tableNames));
            Log(string.Format(": Registering LibLouis LogCallback function"));
            lou_registerLogCallback(MyFunc); // Register the static function MyFunc as a callback""
            string version = GetVersion();
            Log(string.Format(": LibLouis Version {0}", version));
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

        private static bool OnCreationError(string s)
        {
            Log(s);
            return false;
        }

        private static bool DirectoryExists(string path)
        {
            if (Directory.Exists(path)) return true;
            return OnMissingItem("Directory", path);
        }

        private static bool FileExists(string path)
        {
            if (File.Exists(path)) return true;
            return OnMissingItem("File", path);
        }

        private static bool OnMissingItem(string itemType, string path)
        {
            Log(string.Format("{0} does not exist: '{1}'", itemType, path));
            return false;
        }

        /// <summary>
        /// Simple code for checking that all directories and files needed by liblouis are found at the right locations
        /// </summary>
        /// <param name="tableNames"></param>
        /// <returns></returns>
        private static bool CheckInstallation(string tableNames)
        {
            string executingDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string liblouisDir = Path.Combine(executingDirectory, "liblouis");
            if (!DirectoryExists(liblouisDir)) return false;
            string binaryDir = Path.Combine(liblouisDir, "binary");
            if (!DirectoryExists(binaryDir)) return false;
            string liblouisDll = Path.Combine(binaryDir, "liblouis.dll");
            if (!FileExists(liblouisDll)) return false;
            string shareDir = Path.Combine(liblouisDir, "share");
            if (!DirectoryExists(shareDir)) return false;
            string libLouisDir2 = Path.Combine(shareDir, "liblouis");
            if (!DirectoryExists(libLouisDir2)) return false;
            string tablesDir = Path.Combine(libLouisDir2, "tables");
            if (!DirectoryExists(tablesDir)) return false;
      
            string[] names = tableNames.Split(',');
            {                          
                foreach (string name in names)
                {
                    // Only the first name contains the full path !
                    string shortName = Path.GetFileName(name);
                    string fullPath = (Path.Combine(tablesDir, shortName));
                    if (!FileExists(fullPath)) return false;                 
                }
            }
            Log(string.Format(": All tables in '{0}' were found", tableNames));
            return true;
        }
            


        public static Wrapper Create(string tableNames)
        {
            if (! CheckInstallation(tableNames)) return null;        
            Wrapper wrapper =  new Wrapper(tableNames);
            return wrapper;
        }

    }
}
