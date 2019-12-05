using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Text;

namespace MusicXmlReaderModel
{

    public enum ModelMessageEnum
    {
        unknown,
        MissingProgramFile,            // Typically native dlls distributed with the application
        FailedToConnectToScreenReader, // JAWS
        ConnectedToNonDefaultScreenReader,
        FileNotFound,
        DirectoryNotFound,
        FailedToStartProgram, // External program such as Sibelius, Notepad etc
        FailedToReadMusicXmlFile,
        UnspecifiedMusicXmlFile, // Unspecified error during reading and interpretation
        NotAllowedWhilePlaying, // Operation not allowed while playing music
        LocationNotDetermined,   // Location (of for instance JAWS settings file) could not be determined
        UnspecifiedInitializationError, // Last resort for otherwise unspecifiet error during program initializastion
        ToManyPartForExportToMusicBraille // To manyParts for Export to MusicBrailleBraille

    }

    public interface IUtilityClient
    {
        /// <summary>
        /// For simple messages to be shown in the UI
        /// </summary>
        /// <param name="textId">Identifies the text as a ModelMessageEnum. Used by clients implementing localization</param>
        /// <param name="parameter">En optional parameter. Used by clients implementing localization</param>
        /// <param name="text">The text to be shown. Used by simple clients, not implementing localization</param>
        void ShowMessage(int textId, string parameter,string text);

        /// <summary>
        /// For simple warnings to be shown in the UI
        /// </summary>
        /// <param name="textId">Identifies the text as a ModelMessageEnum. Used by clients implementing localization</param>
        /// <param name="parameter">En optional parameter. Used by clients implementing localization</param>
        /// <param name="text">The text to be shown. Used by simple clients, not implementing localization</param>
        void ShowWarning(int textId, string parameter,string text);
    }



    /// <summary>
    /// Contains logging and checking in order to avoid polluting the primary Model-logic
    /// </summary>
    static public class Utilities
    {
        private static string className = "Utilities";
        public static IUtilityClient UtilityClient;

        public const string ExplorerExe = "Explorer.exe";

        public static void ShowWarning(ModelMessageEnum textEnum, string parameter, string text)
        {
            if (null != UtilityClient)
            {
                UtilityClient.ShowWarning((int) textEnum,parameter,text);

            }
        }

        private static void ShowMessage(ModelMessageEnum textEnum,string parameter,string text)
        {
            if (null != UtilityClient)
            {
                UtilityClient.ShowMessage((int)textEnum,parameter,text);

            }
        }

        
        public static  string GetChildValue(XmlNode note, string name)
        {
            foreach (XmlNode n in note.ChildNodes)
            {
                if (name == n.Name) return n.InnerText;
            }
            return "";
        }

        
        /// <summary>
        /// Check valitity of an input parameter of type int
        /// The result parameter is left unchanged if the input fails validation
        /// </summary>
        /// <param name="input"></param>
        /// <param name="result"></param>
        /// <param name="lowValue"></param>
        /// <param name="highValue"></param>
        /// <param name="errorString"></param>
        /// <returns></returns>
        public static bool Parse(string input, ref int result, int lowValue, int highValue, string errorString,bool acceptEmptyAsDefault)
        {
            int tempResult;
            if ((acceptEmptyAsDefault) && (0 == input.Length))
            {
                return true; // Accept and keep the default value
            }

            if (!int.TryParse(input, out tempResult))
            {
                string s = string.Format("{0}: Got '{1}' Expected an integer", errorString, input);
#warning TODO Implement logging reporting 2 levels back !
                //Logger.Log(s);
                LogFormatter.Log(LogFormatter.LogOptions.Once | LogFormatter.LogOptions.ClassFunc3, s); // Show 3 levels of callers
                return false;
            }
            if (tempResult < lowValue || (tempResult > highValue))
            {
                Logger.Log(string.Format("{0}: Got'{1}' Expected [{2}..{3}]", errorString, input, lowValue, highValue));
                return false;
            }
            result = tempResult;
            return true;
        }


        /// <summary>
        /// Check valitity of an input parameter of type char
        /// The result parameter is left unchanged if the input fails validation
        /// </summary>
        /// <param name="input"></param>
        /// <param name="result"></param>
        /// <param name="lowValue"></param>
        /// <param name="highValue"></param>
        /// <param name="errorString"></param>
        /// <returns></returns>
        public static bool Parse(string input, ref string result, char lowValue, char highValue, string errorString, bool acceptEmptyString)
        {
            if (null == input)
            {
                Logger.Log(string.Format("{0}: Got '{1}' Expected a value in [{2}..{3}]", errorString, "NULL" , lowValue, highValue));
                return false;
            }

            // Accept either an empty string og a string consisting of a single character in the interval specified
            if (acceptEmptyString && (0 == input.Length) || ((1 == input.Length) && (input[0] >= lowValue) && ((input[0]) <= highValue)))
            {
                result = input;
                return true;
            }

            Logger.Log(string.Format("{0}: Got '{1}' Expected a value in [{2}..{3}]", errorString, (null == input) ? "NULL" : input, lowValue, highValue));
            return false;
        }

        public static bool Parse(string input, ref char result, char lowValue, char highValue, string errorString)
        {
            if (null == input)
            {
                Logger.Log(string.Format("{0}: Got '{1}' Expected a value in [{2}..{3}]", errorString, "NULL", lowValue, highValue));
                return false;
            }

            // Accept either an empty string og a string consisting of a single character in the interval specified
            if ( (1 == input.Length) && (input[0] >= lowValue) && ((input[0]) <= highValue))
            {
                result = input[0];
                return true;
            }

            Logger.Log(string.Format("{0}: Got '{1}' Expected a value in [{2}..{3}]", errorString, (null == input) ? "NULL" : input, lowValue, highValue));
            return false;
        }



        /// <summary>
        /// Check valitity of an input parameter of type float
        ///  The result parameter is left unchanged if the input fails validation
        /// </summary>
        /// <param name="input"></param>
        /// <param name="result"></param>
        /// <param name="lowValue"></param>
        /// <param name="highValue"></param>
        /// <param name="errorString"></param>
        /// <returns></returns>
        public static bool Parse(string input, ref float result, float lowValue, float highValue, string errorString)
        {
            //TO DO Fix ,/. issue
            string tempInput = input.Replace('.', ','); // This os NOT the correct way to do it ! Localisation etc...
            float tempResult;           
            if (!float.TryParse(tempInput, out tempResult))  
            {
                Logger.LogOnce(string.Format("{0}: Got '{1}' Expected an integer", errorString, input));
                return false;
            }
            if (tempResult < lowValue || (tempResult > highValue))
            {
                Logger.LogOnce(string.Format("{0}: Got '{1}' Expected [{2}..{3}]", errorString, input, lowValue, highValue));
                return false;
            }
            result = tempResult;
            return true;
        }


        /// <summary>
        /// Parses an a sting for the values of "yes" or "no"
        /// </summary>
        /// <param name="functionName">Only used for logging</param>
        /// <param name="attributeName">Only used for logging</param>
        /// <param name="attributeValue">The string to parse</param>
        /// <param name="result">Set depending of the attributeValue: "yes"-> true, "no"->false, default: unchanged</param>
        public static bool ParseYesNoAttributeValue(string functionName, string attributeName, string attributeValue, ref bool result)
        {
            switch (attributeValue)
            {
                case "yes": result = true; return true;
                case "no": result = false; return true;
                default:
                    Logger.LogOnce(string.Format("{0}: Unexpected value for attribute {1}: '{2}'",
                                                  functionName,  // 0
                                                  attributeName, // 1
                                                  attributeValue // 2
                                                  )); break;
            }
            return false;
        }

        /// <summary>
        /// Parses an a sting for the values of "start" or "stop"
        /// </summary>
        /// <param name="functionName">Only used for logging</param>
        /// <param name="attributeName">Only used for logging</param>
        /// <param name="attributeValue">The string to parse</param>
        /// <param name="result">Set depending of the attributeValue: "start"-> true, "stop"->false, default: unchanged</param>
        public static void ParseStartStopAttributeValue(string functionName, string attributeName, string attributeValue, ref bool result)
        {
            switch (attributeValue)
            {
                case "start": result = true; break;
                case "stop": result = false; break;
                default:
                    Logger.LogOnce(string.Format("{0}: Unexpected value for attribute {1}: '{2}'",
                                                  functionName,  // 0
                                                  attributeName, // 1
                                                  attributeValue // 2
                                                  )); break;
            }
        }




        private static bool CheckDll(string dllName, string directory,bool is64Bit)
        {
            return CheckDll(dllName, directory, is64Bit ? MachineType.IMAGE_FILE_MACHINE_AMD64 : MachineType.IMAGE_FILE_MACHINE_I386);
        }

        private static bool CheckDll(string dllName, string directory, MachineType expectedMachineType)
        {
            string fullFileName = Path.Combine(directory, dllName);
            if (!File.Exists(fullFileName))
            {
                Logger.Log(string.Format("Missing support-dll: {0}", dllName));
                return false;
            }
            else
            {
                FileInfo fi = new FileInfo(fullFileName);
                MachineType machineType = TryGetDllMachineType(fullFileName);
                string machineTypeWarning = (machineType == expectedMachineType) ? "" : string.Format(" (Expected {0} !!!)", expectedMachineType);
                Logger.Log(string.Format(" {0,-30} LastWriteTimeUtc={1} Length={2,-8} MachineType={3} {4}",
                                    fi.Name, fi.LastWriteTimeUtc, fi.Length, machineType, machineTypeWarning));
            }
            return true;
        }

        private static MachineType TryGetDllMachineType(string fullFileName)
        {
            MachineType machineType = MachineType.IMAGE_FILE_MACHINE_UNKNOWN;
            try
            {
                machineType = GetDllMachineType(fullFileName);
            }
            catch (Exception e)
            {
                Logger.Log(string.Format("Failed to obtain MachineType for {0} Exception.Message={1}", fullFileName, e.Message));
            }
            return machineType;
        }


        /// <summary>
        /// http://stackoverflow.com/questions/1001404/check-if-unmanaged-dll-is-32-bit-or-64-bit/1002672#1002672
        /// </summary>
        /// <param name="dllPath"></param>
        /// <returns></returns>
        private static MachineType GetDllMachineType(string dllPath)
        {
            // See http://www.microsoft.com/whdc/system/platform/firmware/PECOFF.mspx
            // Offset to PE header is always at 0x3C.
            // The PE header starts with "PE\0\0" =  0x50 0x45 0x00 0x00,
            // followed by a 2-byte machine type field (see the document above for the enum).
            //
            FileStream fs = new FileStream(dllPath, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            fs.Seek(0x3c, SeekOrigin.Begin);
            Int32 peOffset = br.ReadInt32();
            fs.Seek(peOffset, SeekOrigin.Begin);
            UInt32 peHead = br.ReadUInt32();

            if (peHead != 0x00004550) // "PE\0\0", little-endian
                throw new Exception("Can't find PE header");

            MachineType machineType = (MachineType)br.ReadUInt16();

            if (br != null)
            {
                br.Close();
            }

            // Calling fs.Close() or fs.Dispose() here will cause a warning !

            return machineType;
        }


        private enum MachineType : ushort
        {
            IMAGE_FILE_MACHINE_UNKNOWN = 0x0,
            IMAGE_FILE_MACHINE_AM33 = 0x1d3,
            IMAGE_FILE_MACHINE_AMD64 = 0x8664,
            IMAGE_FILE_MACHINE_ARM = 0x1c0,
            IMAGE_FILE_MACHINE_EBC = 0xebc,
            IMAGE_FILE_MACHINE_I386 = 0x14c,
            IMAGE_FILE_MACHINE_IA64 = 0x200,
            IMAGE_FILE_MACHINE_M32R = 0x9041,
            IMAGE_FILE_MACHINE_MIPS16 = 0x266,
            IMAGE_FILE_MACHINE_MIPSFPU = 0x366,
            IMAGE_FILE_MACHINE_MIPSFPU16 = 0x466,
            IMAGE_FILE_MACHINE_POWERPC = 0x1f0,
            IMAGE_FILE_MACHINE_POWERPCFP = 0x1f1,
            IMAGE_FILE_MACHINE_R4000 = 0x166,
            IMAGE_FILE_MACHINE_SH3 = 0x1a2,
            IMAGE_FILE_MACHINE_SH3DSP = 0x1a3,
            IMAGE_FILE_MACHINE_SH4 = 0x1a6,
            IMAGE_FILE_MACHINE_SH5 = 0x1a8,
            IMAGE_FILE_MACHINE_THUMB = 0x1c2,
            IMAGE_FILE_MACHINE_WCEMIPSV2 = 0x169,
        }

        internal static bool CheckDlls(string directory, string caption, bool is64Bit)
        {
            // Report if any file is missing
            bool result = true;
            Logger.Log(string.Format("This program is compiled for a {0} bit architechture. It uses the following locally installed dlls", is64Bit ? "64" : "32"));
  
            // A few dlls have different names in 32 bit and 64 bit versions:
            if (is64Bit)
            {
                result &= CheckDll("jfwapi.dll", directory, true);                      // JAWS
                result &= CheckDll("nvdaControllerClient64.dll", directory, true);      // NVDA
            }
            else
            {
                result &= CheckDll("fsapi.dll", directory, false);                      // JAWS This seems to be the right JAWS interface in the  32 bit case 
                result &= CheckDll("nvdaControllerClient32.dll",directory, false);      // NVDA
            }


            //  Most of the dlls have the same name in 32 bit and 64 bit versions:
            // result &= CheckDll("tolk.dll", directory, is64Bit);                        // Generic access to screenreaders. Not really needed ! 
            result &= CheckDll("NAudio.dll", directory, is64Bit);                      // Generation of MIDI sound 
            result &= CheckDll("MusicSynthesis.dll", directory, is64Bit);              // Generation of MIDI sound 
            result &= CheckDll("FsBrlDspApi.dll", directory, is64Bit);                 // Direct access to Freedom Scientific Braille Display. Not really needed. 
            result &= CheckDll("7z.dll", directory, true);                           // For converting .mxl to .xml 
            result &= CheckDll("7z.exe", directory, true);                           // For converting .mxl to .xml  


            if (!result)
            {
                if (caption != null)
                {
                    ShowWarning(ModelMessageEnum.MissingProgramFile,"",
                        "Manglende programfil!\r\n"
                      + "Se venligst Logfilen! (Værktøjer->Log fil)");
                }
                else
                {
                    // The application is .cmd
                    Console.WriteLine("Manglende programfil! Se venligst Logfilen");
                }
            }

            return result;
        }


        internal static bool CheckScreenReader(string screenReaderName, string caption)
        {

            // screenReaderName = "NVDA"; // For test 
            switch (screenReaderName)
            {
                case "JAWS": return true;
                case "NVDA":
                    if (null != caption)
                    {                  
                        ShowWarning(ModelMessageEnum.ConnectedToNonDefaultScreenReader, "NVDA", "");       // The application has UI
                    }
                    else
                    {                   
                        Console.WriteLine("Connected To NVDA screenreader");      // The application is .cmd
                    }
                    return true; 
                default:
                    if (null != caption)
                    {            
                        // ShowWarning(ModelMessageEnum.FailedToConnectToScreenReader, "", "");     // The application has UI
                    }
                    else
                    {
                     
                        Console.WriteLine("Could not connect to screenreader!");    // The application is .cmd
                    }
                    return false;
            }
        }


        internal static bool CheckDirectoryExistance(string directoryName, string methodName)
        {
            if (!System.IO.Directory.Exists(directoryName))
            {
                Logger.Log(string.Format("{0} Directory {1} is not found", string.IsNullOrEmpty(methodName) ? "" : methodName + ":", directoryName));
                ShowWarning(ModelMessageEnum.DirectoryNotFound, directoryName, "");
                return false;
            }
            return true;
        }

        internal static bool CheckFileExistance(string fileName, string methodName)
        {
            if (!System.IO.File.Exists(fileName))
            {
                Logger.Log(string.Format("{0} File {1} is not found", string.IsNullOrEmpty(methodName) ? "" : methodName + ":", fileName));
                ShowWarning(ModelMessageEnum.FileNotFound, fileName, "");
                return false;
            }
            return true;
        }

        //internal static bool CheckFileExistance(string fileName, string methodName, bool dir)
        //{
        //    if (dir)
        //    {
        //        if (!System.IO.Directory.Exists(fileName))
        //        {
        //            Logger.Log(string.Format("{0} Directory {1} is not found", string.IsNullOrEmpty(methodName) ? "" : methodName + ":", fileName));
        //            ShowWarning(ModelMessageEnum.DirectoryNotFound,fileName,"");             
        //            return false;
        //        }
        //    }
        //    else
        //    {
        //        if (!System.IO.File.Exists(fileName))
        //        {
        //            Logger.Log(string.Format("{0} File {1} is not found", string.IsNullOrEmpty(methodName) ? "" : methodName + ":", fileName));
        //            ShowWarning(ModelMessageEnum.FileNotFound,fileName,"");
        //            return false;
        //        }
        //    }
        //    return true;
        //}

        public static string Quote(string argument)
        {
            if (string.IsNullOrEmpty(argument)) return argument;            
            if (('"' == argument[0]) && ('"' == argument[argument.Length])) return argument;
            return string.Format("\"{0}\"", argument);
        }

        public static bool RunExeWithUrlArgument(string url)
        {
            return RunExeWithArgument("iexplorer.exe", Quote(url));
        }


        /// <summary>
        /// Attempts to start an external program using a single filename as argument
        /// Errors are reportes through messageboxes and Model.Log()
        /// </summary>
        /// <param name="exeFileName">Name of program to start, with or without full path</param>
        /// <param name="argFileName">Name of file to use as argument when starting the program</param>
        /// <returns>true <==> succaee</returns>
        public static bool RunExeWithFileArgument(string exeFileName, string argFileName)
        {
            // Check that the File exists
            if (!System.IO.File.Exists(argFileName))
            {
                Utilities.UtilityClient.ShowWarning((int)ModelMessageEnum.UnspecifiedMusicXmlFile,"", "");
                return false;
            }
            return RunExeWithArgument(exeFileName, Quote(argFileName));
        }

                internal static bool RunExeWithDirArgument(string exeFileName, string argFileName)
        {
            // Check that the directory exists
            if (!System.IO.Directory.Exists(argFileName))
            {
                Utilities.UtilityClient.ShowWarning((int)ModelMessageEnum.DirectoryNotFound, "", "");
                return false;
            }
            return RunExeWithArgument(exeFileName, Quote(argFileName));
        }


        //internal static bool RunExeWithFileArgument(string exeFileName, string argFileName, bool dir)
        //{
        //    string methodName = "RunExeWithFileArgument";
        //    if ((!string.IsNullOrEmpty(argFileName)) && (!CheckFileExistance(argFileName, methodName, dir))) return false;
        //    return RunExeWithArgument(exeFileName, argFileName);
        //}

        internal static bool RunExeWithUrlArgument(string exeFileName, string url)
        {
            return   RunExeWithArgument(exeFileName,Quote(url));
        }


        /// <summary>
        /// Assumes that the "argument" parameter has already been checked according to its type of file, directory or ulr
        /// </summary>
        /// <param name="exeFileName">Name of .exe file to run</param>
        /// <param name="argument">Argument(s) for .exe file. Enclosed in "" if needed, for instance for file names containing spaces.</param>
        /// <returns>true  <==> success</returns>
        public static bool RunExeWithArgument(string exeFileName, string argument)
        {
            int exitCode = 0; // Needed as dummy argument
            return RunExeWithArgument(exeFileName, argument,false, out exitCode, System.Diagnostics.ProcessWindowStyle.Normal);
        }

        internal static bool RunExeWithArgumentAndWaitForExit(string exeFileName, string argument,out int exitCode)
        {
            return RunExeWithArgument(exeFileName, argument,true,out exitCode, System.Diagnostics.ProcessWindowStyle.Hidden);
        }

        private static bool RunExeWithArgument(string exeFileName, string argument,bool waitForExit,out int exitCode, System.Diagnostics.ProcessWindowStyle windowStyle)
        {
            string methodName = "RunExeWithArgument";
            exitCode = 0;         
            // Check arguments
            string exePathName = Path.GetDirectoryName(exeFileName);
            if ((!string.IsNullOrEmpty(exePathName)) && (!CheckFileExistance(exeFileName, className + "." + methodName))) return false;
            // Create process startinfo. Enclose all filenames and pathnames in "" in order to handle possible space characters!
            System.Diagnostics.Process pProcess = new System.Diagnostics.Process();
            pProcess.StartInfo.FileName = Quote(exeFileName); // Needed if the exeFileNAme contains spaces
            pProcess.StartInfo.WorkingDirectory = string.IsNullOrEmpty(exePathName) ? null : string.Format("\"{0}\"", exePathName);
            pProcess.StartInfo.Arguments = argument;
            pProcess.StartInfo.UseShellExecute = true; // Allows the system to search for the executable using PATH
            pProcess.StartInfo.RedirectStandardOutput = false;
            pProcess.StartInfo.WindowStyle = windowStyle;
            try
            {
                if (pProcess.Start())
                {
                    Logger.Log(string.Format("{0}.{1}: Started {2} with arguments='{3}'", className, methodName, pProcess.StartInfo.FileName,
                                                                                          (null != pProcess.StartInfo.Arguments) ? pProcess.StartInfo.Arguments : "null"));
                    if (waitForExit)
                    {
                        pProcess.WaitForExit();
                        exitCode = pProcess.ExitCode;
                        Logger.Log(string.Format("{0}.{1}: Exited with ExitCode={2}", className, methodName, exitCode));
                    }
                }
                else
                {
                    Logger.Log(string.Format("{0}.{1}: Failed to start {2}", className, methodName, pProcess.StartInfo.FileName));
                }
                // throw new Exception("test"); // For testing error handling only
            }       
            catch (Exception e)
            {
                Logger.Log(string.Format("{0}.{1}: Exception thrown while starting {2}: {3}", className, methodName, pProcess.StartInfo.FileName, e.Message));
               ShowWarning(ModelMessageEnum.FailedToStartProgram, exeFileName,"");
                return false;
            }
            return true;
        }


        public static void CreateEmptyTempDirectory(string directoryName)
        {
            if (Directory.Exists(directoryName))
            {
                // Delete all files in the temp directory
                FileInfo[] files = new DirectoryInfo(directoryName).GetFiles();
                foreach (FileInfo fileInfo in files)
                {
                    File.Delete(fileInfo.FullName); // Allows us to delete the directory
                }
            }
            else
            {
                // Create a new temp directory
                Directory.CreateDirectory(directoryName);
            }
        }


        static void DeleteTempDirectory(string directoryName)
        {
            string functionName = "DeleteTempDirectory";
            // Logger.Log(string.Format("{0}.{1}+", className, functionName));
            try
            {
                // Recursively delete all directories
                DirectoryInfo[] directories = new DirectoryInfo(directoryName).GetDirectories(); 
                foreach (DirectoryInfo directoryInfo in directories)
                {
                    DeleteTempDirectory(directoryInfo.FullName);
                }

                // delete all files
                FileInfo[] files = new DirectoryInfo(directoryName).GetFiles();
                foreach (FileInfo fileInfo in files)
                {
                    File.Delete(fileInfo.FullName); // Allows us to delete the directory
                }
                Directory.Delete(directoryName);
                // throw new Exception("For test only!");
            }
            catch (Exception e)
            {
                string message = string.Format("{0}.{1}({2}) failed. Exception.Message={3}", className, functionName, directoryName, e.Message);
                Logger.Log(message);
                throw new Exception(message, e); // Rethrow
            }
            // Logger.Log(string.Format("{0}.{1}-", className, functionName));
        }

        public static string GetExecutingDirectory()
        {
            string executingAssembly = System.Reflection.Assembly.GetExecutingAssembly().Location;
            return System.IO.Path.GetDirectoryName(executingAssembly);
        }

        public static string GetExecutingAssembly()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().Location;
        }


        /// <summary>
        /// Convert a .mxl file (compressed MusicXml) to .xml (MusicXml) relying on the external program 7z.exe
        /// which is a part of the IBOS MusicXmlReader distribution. Other implementations may follow if needed !
        /// </summary>
        /// <param name="fullMxlFileName">Full path  of .mxl file to be converted </param>
        /// <returns>Full path of the resulting .xml file</returns>
        public static string MxlToXml(string fullMxlFileName,string destinationDirectory)
        {
            string methodName = "MxlToXml";
            string executingDirectory = GetExecutingDirectory();
            string result = "";
            // Use the temp directory created and used by the Logger
            string tempDirectory = Path.Combine(Logger.MusicXmlReaderTempDirectory, "tempDirectoryUsedByMxlToXml"); // Probably a unique name
            Logger.Log(string.Format("{0}.{1}({2},{3}) started.", className, methodName, fullMxlFileName, executingDirectory));
            CreateEmptyTempDirectory(tempDirectory);
            //string exeFileName = @"C:\Program Files\7-Zip\7z.exe";
            string exeFileName = Path.Combine(executingDirectory,@"7z.exe"); // Assumes that 7z.exe and 7z.dll are found in the execution directory !
            string command = "e";
            string switches = string.Format("-aoa -o\"{0}\"", tempDirectory); // -aoa: Overwrite existing files, -0: Specify output directory
            string argument = string.Format("{0} \"{1}\" {2}", command, fullMxlFileName, switches); // The filename may contain spaces so we need ""
            int exitCode = 0;
            if (Utilities.RunExeWithArgumentAndWaitForExit(exeFileName, argument, out exitCode))
            {
                if (0 != exitCode)
                {         
                    Logger.Log(string.Format("{0}.{1}({2}) failed: {3} returned exitcode={4}. ", className, methodName, fullMxlFileName, exeFileName, exitCode));
                    return result;
                }

                // Move the newly generated .xml file from the temp directory to the original directory.
                FileInfo[] files = new DirectoryInfo(tempDirectory).GetFiles();
                Logger.Log(string.Format("{0}.{1}: TempDirectory={2} contains {3} files:", className, methodName, tempDirectory, files.Length));
                // As default place the .xml file in the same directory as the .mxl file 
                string destXlm = string.IsNullOrEmpty(destinationDirectory) ? fullMxlFileName : Path.Combine(destinationDirectory, Path.GetFileName(fullMxlFileName));
                string destXml = Path.ChangeExtension(destXlm, "xml");
                int numberOfFiles = 0;
                foreach (FileInfo fileInfo in files)
                {
                    Logger.Log(string.Format(" {0}",fileInfo.Name));
                    if ("container.xml" != fileInfo.Name)
                    {
                        string source = fileInfo.FullName;           

                        bool overwriteExisting = true;
                        //Logger.LogCF(string.Format(": Starting File.Copy({0} to {1}", source, destXml));
                        File.Copy(source, destXml, overwriteExisting);
                        //Logger.LogCF(string.Format(": Finished File.Copy({0} to {1}", source, destXml));
                        result = destXml;
                        numberOfFiles++;
                    }
                    File.Delete(fileInfo.FullName); // Allows us to delete the directory
                }
                if (1 != numberOfFiles)
                {
                    Logger.LogOnce(string.Format("{0}.{1}: Unexpectedly found {2} files", className, methodName, numberOfFiles));
                }
            }
          

            DeleteTempDirectory(tempDirectory);
            Logger.Log(string.Format("{0}.{1}({2},{3}) returned {4}.", className, methodName, fullMxlFileName, executingDirectory, (null == result) ? "null" : result));
            return result;
        }
        

        /// <summary>
        /// https://msdn.microsoft.com/en-us/library/bb762914(v=vs.110).aspx
        /// </summary>
        /// <param name="sourceDirName"></param>
        /// <param name="destDirName"></param>
        /// <param name="copySubDirs"></param>
        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs,ref int nFiles, ref int nDirs, List<string> fileNames)
        {            
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
                nDirs++;
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                if (!File.Exists(temppath))
                {
                    file.CopyTo(temppath, false);
                    if (null != fileNames)
                    {
                        fileNames.Add(file.Name);
                    }
                    nFiles++;
                }
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs,ref nFiles, ref nDirs, fileNames);
                }
            }
        }


        /// <summary>
        /// Create the directory specified, if it does not already exist
        /// </summary>
        /// <param name="path"></param>
        static public void CreateDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                Logger.LogCF(string.Format(": Created directory '{0}'", path));
            }
        }

    /// <summary>
    /// Remove any occurance of the "&" character from the string.
    /// Used in the UI for using the same localized text resource both for assigning an ALT shortcut (marked by the "&") and as a normal string (aftert removing the "&")
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string RemoveAmpersant(string s)
        {
            if (null == s) return s;
            return s.Replace("&", "");
        }

        public static string GetShortcutName(string s)
        {
            if (null == s) return "";
            int index = s.IndexOf('&'); // returns -1 if not found
            if (-1 == index) return "";
            if (s.Length <= (index + 1)) return "";
            return s[index + 1].ToString();
        }

        public static string BrailleToDotNumbers(char c)
        {
            StringBuilder sb = new StringBuilder();
            if (0 != (c & 0x01)) sb.Append(" 1");
            if (0 != (c & 0x02)) sb.Append(" 2");
            if (0 != (c & 0x04)) sb.Append(" 3");
            if (0 != (c & 0x08)) sb.Append(" 4");
            if (0 != (c & 0x10)) sb.Append(" 5");
            if (0 != (c & 0x20)) sb.Append(" 6");
            if (0 != (c & 0x40)) sb.Append(" 7");
            if (0 != (c & 0x80)) sb.Append(" 8");
            if (0 == (sb.Length))
            {
                sb.Append(" 0"); // Use '0' as a place holder for the empty Braille character
            }
            string s = sb.ToString();
            return string.Format("{0}{1}{2}","{",s.Substring(1),"}"); // Drop the first character, which is always a space.
        } 


        /// <summary>
        /// Converts a string of Braille code to it's text representation, ignoring characters outside 0x2800..0x28ff
        /// </summary>
        /// <param name="braille"></param>
        public static string BrailleToDotNumbers(string braille)
        {
            StringBuilder sbLine = new StringBuilder();
            string delimiter = ""; // Used between Braille chars, not inside a Braille char
            foreach (char c in braille)
            {
                if ((c >= 0x2800) && (c <= 0x283F))
                {
                    // This is a valid UNICODE BRaille char in 0x280.. 0x283f. Show the dots
                    StringBuilder sbChar = new StringBuilder(); // Represents a single Braille char
                    sbLine.Append(delimiter); // " " 
                    sbLine.Append(BrailleToDotNumbers(c));
                    //delimiter = ","; // From now on use a visible delimiter between Braille chars
                    sbLine.Append(sbChar.ToString());
                }
                else
                {
                    sbLine.Append(string.Format("(?={0})", (int)c)); //  The "?" indicates that this char was not a valid Braille-6 value. Show the decimal value of the char.
                }
            }
            string result = sbLine.ToString();
            // Logger.LogCF(string.Format(": Converted {0} to {1}", braille, result));
            return result;

        }

        private static readonly char[] trimChars = new char[] { ' ' };
        public static string ToOneLine(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            string s1 = s.Replace('\r', ' ');
            string s2 = s1.Replace('\n', ' ');
            string s3 = s2.TrimStart(trimChars); // Remove heading spaces
            string s4 = s3.TrimEnd(trimChars);   // Remove trailing spaces
            if (0 != s.CompareTo(s4))
            {
                //Logger.LogCFOnce(string.Format(": Removed CR and LF. Returned '{0}'", s4));
            }
            return s4;
        }



        /// <summary>
        /// Some partnames contain carriage return, linefeed and slash which makes any listing look strange and results in invalid filenames
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ToValidFileName(string s)
        {
            if (null == s) return "";
            // s1-s4 are used for generating a valid filename
            // t1-t2 are used for generating readable text for loglines in one line
            string s1 = s.Replace("\n", "n");  // LineFeed replaced by "n" in filenames
            string t1 = s.Replace("\n", "\\n"); // Linefeed replaced by "\n" in loglines 
            string s2 = s1.Replace("\r", "r");  // Carriage return replaced by "r" in filenames
            string t2 = t1.Replace("\r", "\\r"); // Carriage return replaced by "\r" in loglines
            string s3 = s2.Replace("/", ""); // Forward slash
            string s4 = s3.Replace("\\", ""); // A single backslash
            string s5 = s4.Replace("\"", ""); // An "
            if (0 != string.Compare(s, s5))
            {
                // Logger.LogCFOnce(string.Format(": Changed '{0}' to '{1}'", t2, s5));
            }
            return s5;
        }


        /// <summary>
        /// Assure common implementation of Beep();
        /// NOTE:
        /// Sometimes the Beep stops working for a single user!.
        /// In that case:
        /// 1)  Go to ControlPanel->Sound->Sounds->ProgramEvents
        /// 2)  Select "DefaultBeep" and change the value from "Windows Background.wav" to something else - and back!
        /// 3)  Press OK
        /// </summary>
        public static void Beep()
        {
#if false
#warning ToDo Find out why SystemSounds.Beep.Play does not work on JSJ's private PC when User= JSJ ! And remove the hack below!
            bool ok = false;
            string fileName = @"C:\Windows\media\Windows Background.wav";
            try
            {           
                new System.Media.SoundPlayer(fileName).Play();
                ok = true;
            }
            catch (Exception)
            {
                Logger.LogCF(string.Format(": Failed to play {0}", fileName));
            }
            if (!ok)
#endif
            {
                Logger.LogCF(": Beep!"); // Primarily for debugging. On JSJ's private PC the Beep sound is unstable !
                System.Media.SystemSound myBeep = System.Media.SystemSounds.Beep;
                myBeep.Play();
            }
        }


        public static void CloneFile(string fileName, string newExtension)
        {
            string newFileName = Path.ChangeExtension(fileName, newExtension);
            if (!File.Exists(fileName))
            {
                Logger.LogCF(string.Format(": File '{0}' does not exist",fileName));
                return;
            }
            if (File.Exists(newFileName))
            {
                File.Delete(newFileName);
            }
            File.Copy(fileName, newFileName);
            Logger.LogCF(string.Format(": Cloned '{0}' to '{1}'", fileName, Path.GetFileName(newFileName)));
        }

        private static string CheckDirectoryPath(string path)
        {
            if ((null == path) || !Directory.Exists(path))
            {
                return string.Format("Directory {0} not found", null == path ? "null" : path);
            }
            return null;
        }

        static string ReadToEnd(string path, string name)
        {
            StreamReader sr = null;
            string result = null; 
            // Read the contents, catching exceptions.            
            try
            {
                sr = new StreamReader(Path.Combine(path, name));
                result = sr.ReadToEnd();
            }
            catch (Exception e)
            {
                Logger.LogCFE(e);
            }

            // Close the StreamREeader, catching exceptions.
            try
            {
                if (null != sr)
                {
                    sr.Close();
                }
            }
            catch (Exception e)
            {
                Logger.LogCFE(e);
            }

            return result;
        }

        /// <summary>
        /// (Primarily to be used for regression tests).
        /// Check the contents of two directories for identity.
        /// If the directories have identical contents null is returned.
        /// Otherwise a string describing the first difference encountered is returned
        /// </summary>
        /// <param name="path0">The old directory to compare against</param>
        /// <param name="path1">The new directory to compare against the old one</param>
        /// <returns></returns>
        public static string CompareDirectories(string path0, string path1)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                string result0 = CheckDirectoryPath(path0);
                string result1 = CheckDirectoryPath(path1);

                if (null != result0) return result0;
                if (null != result1) return result1;


                string[] files0 = System.IO.Directory.GetFiles(path0);
                string[] files1 = System.IO.Directory.GetFiles(path1);
                string path0Time = new System.IO.DirectoryInfo(path0).CreationTime.ToString();
                string path1Time = new System.IO.DirectoryInfo(path1).CreationTime.ToString();
                if (files0.Length != files1.Length)
                {
                    string logMessage =
                      string.Format(": Failed: Different number of files found.\r\n")
                    + string.Format("Path0='{0}' Time='{1}' Count={2}\r\n", path0, path0Time, files0.Length)
                    + string.Format("Path1='{0}' Time='{1}' Count={2}", path1, path1Time, files1.Length);
                    Logger.LogCF(logMessage);

                    return string.Format(": Failed: Different number of files found.\r\n"); 
                }

                // Same number of files. Assume same ordering:
                int numberOfFiles = files0.Length;
             
                for (int i = 0; (i < numberOfFiles); i++)
                {
                    string file0 = files0[i];
                    string file1 = files1[i];

                    // Compare names
                    string name0 = Path.GetFileName(file0);
                    string name1 = Path.GetFileName(file1);
                    if (0 != string.Compare(name0, name1))
                    {
                        sb.AppendLine( string.Format("File names[{0}] differ: Name0='{1}'  Name1='{2}'", i, name0, name1));
                        continue; // To next i
                    }

                    // Compare lengths
                    long length0 = new FileInfo(file0).Length;
                    long length1 = new FileInfo(file1).Length;
                    if (length0 != length1)
                    {
                        sb.AppendLine(string.Format("Lengths differ:'{0}':{1}/{2}", name0, length0, length1));
                        continue; // To next i
                    }

                    // Compare contents
                    string contents0 = ReadToEnd(path0, name0);
                    string contents1 = ReadToEnd(path1, name1);
                    if (0 != string.Compare(contents0, contents1))
                    {
                        sb.AppendLine(string.Format("Contents of files differ for FileName='{0}' ", name0));
                        continue; // To next i
                    }

                }
                if (0 == sb.Length)
                {
                    Logger.LogCF(string.Format(": All {0} pairs of files have same contents", numberOfFiles));
                }

            }
            catch (Exception e)
            {
                Beep();
                Logger.LogCFE(e);
                return e.Message;
            }
            return (0 == sb.Length) ? null : sb.ToString(); // No difference found 
        }


        /// <summary>
        /// Split a given path into it levels (separated by the system-dependent DirectorySeparatorChar)
        /// and return the last n levens
        /// </summary>
        /// <param name="path">The path to operate on</param>
        /// <param name="nLevels"
        /// >The number of levels (measured from the end of the path) to return</param>
        /// <returns>
        /// A string as described above.
        /// If n is negative or zero an empty string is returned
        /// If n is larger than the number of levels in the path, the full path is returned
        /// </returns>
        public static string GetEndOfPath(string path, int nLevels)
        {
            // Extract the n last levels of a given directoryName or filename
            string[] parts = path.Split(Path.DirectorySeparatorChar);
            int length = parts.Length;
            StringBuilder sb = new StringBuilder();
            for (int i = length - nLevels; i < length; i++)
            {
                if (i > 0)
                {
                    sb.Append(Path.DirectorySeparatorChar);
                    sb.Append(parts[i]);
                }
            }
            return sb.ToString();
        }


        private static void LogSpecialFolder(Environment.SpecialFolder folder)
        {
            string symbolicName = folder.ToString();
            string actualName = Environment.GetFolderPath(folder);
            LogSpecialFolder(symbolicName, actualName);
        }

        private static void LogSpecialFolder(string symbolicName, string actualName)
        {
            Logger.LogCF(string.Format(": {0,-30} {1}", symbolicName, actualName));
        }


        /// <summary>
        /// Simple developer tool for logging the names of some special folders
        /// </summary>
        public static void LogSpecialFolders(bool log)
        {
            if (!log) return;

            // http://stackoverflow.com/questions/915210/how-can-i-get-the-path-of-the-current-users-application-data-folder
            try
            {
                LogSpecialFolder(Environment.SpecialFolder.LocalApplicationData);               // C:\Users\Jens\AppData\Local
                LogSpecialFolder(Environment.SpecialFolder.ApplicationData);                    // C:\Users\Jens\AppData\Roaming
                LogSpecialFolder(Environment.SpecialFolder.System);                             // C:\WINDOWS\system32
                LogSpecialFolder("Environment.SystemDirectory", Environment.SystemDirectory);   // C:\WINDOWS\System32
                LogSpecialFolder(System.Environment.SpecialFolder.MyDocuments);                 // C:\Users\Jens\Documents
                LogSpecialFolder(System.Environment.SpecialFolder.Recent);                      // C:\Users\Jens\AppData\Roaming\Microsoft\Windows\Recent
                LogSpecialFolder("DownloadDirectory",KnownFolders.GetPath(KnownFolder.Downloads, false));    // C:\Users\Jens\Downloads
            }
            catch (Exception e)
            {
                Logger.LogCFE(e);
            }
        }

    }


}
