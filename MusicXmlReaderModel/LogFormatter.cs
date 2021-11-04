using System;
using System.Xml;

namespace MusicXmlReaderModel
{
    static public class LogFormatter
    {
        /// <summary>
        /// Values for describing Log options.
        /// In order to reduce the amount of code where the public functions in this class are called by exteansiv use of default values
        /// all options are described in 1 parameter.
        /// 4 independent options are described: Preamble, Occurance, Description and Level of details
        /// Each option uses 4 bits 
        /// Values within each option are mutually exclusive, so they are sequentially numbered, 1,2,3,4,5, NOT 1,2,4,8,16 !!
        /// </summary>
        public enum LogOptions
        {
            // Preamble
            ClassFunc   = 0x00000000, // Default. "<Class>.<Function>" Caller
            Nothing     = 0x00000001, // 
            ClassFunc2  = 0x00000002, // "<Class>.<Function>-><Class>.<Function>" 2 levels of callers
            ClassFunc3  = 0x00000003, // "<Class>.<Function>-><Class>.<Function>-><Class>.<Function>" 3 levels of callers
            PreambleMask = 0x0000000f,
            // Frequency of occurance
            Always      = 0x00000000,  // Default
            Once        = 0x00000010,
            Never       = 0x00000020,
            FreqencyMask= 0x000000f0,
            // Description of what happend
            Unknown     = 0x00000000, // Default
            Unsupported = 0x00000100,
            DescMask    = 0x00000f00, 
            // Level of details 
            Verbose     = 0x00000000, // Default
            Quiet       = 0x00001000,
            Dumb        = 0x00002000,
            LevelMask   = 0x0000f000,
            // More options can be added if required in the fpllowing way:
            // XxDefault= 0x00000000, // Defaolt
            // XxOPtion1= 0x00010000,
            // XxOPtion2= 0x00020000,
            // XxOPtion3= 0x00030000,
            // XxMask   = 0x000f0000,
            IllegalValue = -1 // 0xffffffff will contain illegal values for all options. Used for test only !

        }

        private  static void LogInternalError(LogOptions options)
        {
            string caller1 = Logger.GetCallingMethodInternalImplementation(1);
            string caller2 = Logger.GetCallingMethodInternalImplementation(2);
            string caller3 = Logger.GetCallingMethodInternalImplementation(3);
            Logger.LogOnce(string.Format("{0} called from {1} called from {2}: Unexpected value of MaskedOptions='{3}' ", caller1, caller2,caller3, options));
        }

        private static void InternalLog(LogOptions options, string line)
        {
            LogOptions maskedOptions = LogOptions.FreqencyMask & options;
            switch (maskedOptions)
            {
                case LogOptions.Always: Logger.Log(line); return;
                case LogOptions.Once:   Logger.LogOnce(line); return;
                case LogOptions.Never:  return;
                default:
                    //Logger.LogCF(string.Format(": Unexpected value of options ={0}", maskedOptions));
                    LogInternalError(maskedOptions);
                    break;
            }
        }

        private static string GetLogDescription(LogOptions options)
        {
            LogOptions maskedOptions = LogOptions.DescMask & options;
            switch (maskedOptions)
            {
                case LogOptions.Unknown: return "Unknown";          // NOT just maskedOptions.ToString() because 0 means many different things!
                case LogOptions.Unsupported: return "Unsupported";
                default:
                    //Logger.LogCF(string.Format(": Unexpected value of options ={0}", maskedOptions));
                    LogInternalError(maskedOptions);
                    return "";
            }
        }

        private static string GetLogValue(LogOptions options, XmlNode node)
        {
            LogOptions maskedOptions = LogOptions.LevelMask & options;
            switch (maskedOptions)
            {
                case LogOptions.Verbose: return string.Format("InnerXml='{0}'", node.InnerXml);
                case LogOptions.Quiet: return string.Format("InnerText='{0}'", node.InnerText);
                case LogOptions.Dumb: return "";
                default:  
                    LogInternalError(maskedOptions);
                    return "";
            }

        }

        private static string GetCallingMethod(LogOptions options)
        {
            LogOptions maskedOptions = LogOptions.PreambleMask & options;
            string result = "";
            switch (maskedOptions)
            {
                case LogOptions.Nothing: break;
                case LogOptions.ClassFunc: result = Logger.GetCallingMethodInternalImplementation(2); break;
                case LogOptions.ClassFunc2: result = string.Format("{0}->{1}", Logger.GetCallingMethodInternalImplementation(3), Logger.GetCallingMethodInternalImplementation(2)); break;
                case LogOptions.ClassFunc3: result = string.Format("{0}->{1}->{2}", Logger.GetCallingMethodInternalImplementation(4), Logger.GetCallingMethodInternalImplementation(3), Logger.GetCallingMethodInternalImplementation(2)); break;
                default:  LogInternalError(maskedOptions); break;
            }
            return result;
        }


        // Public methods


        public static void Log(LogOptions options, string text)
        {
            //options = LogOptions.IllegalValue; // For test only !!
            string caller = GetCallingMethod(options);
            string logLine = string.Format("{0}: {1}", caller, text);
            InternalLog(options, logLine);
        }

        public static void Log(LogOptions options, XmlNode node)
        {
            // options = LogOptions.IllegalValue; // For test only !!
            string caller = GetCallingMethod(options);
            string description = GetLogDescription(options);
            string value = GetLogValue(options, node);
            string logLine = string.Format("{0}: {1} element: Name='{2}' {3} ", caller, description, node.Name, value);
            InternalLog(options, logLine);
        }

        public static void Log(LogOptions options, XmlAttribute attribute)
        {
            //options = LogOptions.IllegalValue; // For test only !!
            string caller = GetCallingMethod(options);
            string description = GetLogDescription(options);
            string logLine = string.Format("{0}: {1} attribute: Name='{2}' Value='{3}'", caller, description, attribute.Name, attribute.Value);
            InternalLog(options, logLine);
        }



    }
}
