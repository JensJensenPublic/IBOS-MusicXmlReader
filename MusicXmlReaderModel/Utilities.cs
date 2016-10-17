using System;
using System.IO;
using System.Xml;
//using System.Windows.Forms;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXmlReaderUI
{

    public enum ModelMessageEnum
    {
        unknown,
        MissingProgramFile,            // Typically native dlls distributed with the application
        FailedToConnectToScreenReader, // JAWS
        FileNotFound,
        DirectoryNotFound,
        FailedToStartProgram, // External program such as Sibelius, Notepad etc
        FailedToReadMusicXmlFile // Unspecified error during reading and interpretation
    }

    public interface IMessageShower
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

        public static IMessageShower MessageShower;

        private static void ShowWarning(ModelMessageEnum textEnum, string parameter, string text)
        {
            if (null != MessageShower)
            {
                MessageShower.ShowWarning((int) textEnum,parameter,text);

            }
        }

        private static void ShowMessage(ModelMessageEnum textEnum,string parameter,string text)
        {
            if (null != MessageShower)
            {
                MessageShower.ShowMessage((int)textEnum,parameter,text);

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
                Logger.Log(string.Format("{0}: Got '{1}' Expected an integer", errorString, input));
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
                Logger.Log(string.Format("{0}: Got '{1}' Expected an integer", errorString, input));
                return false;
            }
            if (tempResult < lowValue || (tempResult > highValue))
            {
                Logger.Log(string.Format("{0}: Got '{1}' Expected [{2}..{3}]", errorString, input, lowValue, highValue));
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
        public static void ParseYesNoAttributeValue(string functionName, string attributeName, string attributeValue, ref bool result)
        {
            switch (attributeValue)
            {
                case "yes": result = true; break;
                case "no":  result = false; break;
                default: Logger.LogOnce(string.Format("{0}: Unexpected value for attribute {1}: '{2}'",
                                                       functionName,  // 0
                                                       attributeName, // 1
                                                       attributeValue // 2
                                                       )); break;            }
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
                Logger.Log(string.Format(" {0,-30} LastWriteTimeUtc={1} Length={2,-6} MachineType={3} {4}",
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
            br.Close();
            fs.Close();
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
            result &= CheckDll("FsBrlDspApi.dll", directory, is64Bit);                 // Derect access to Freedom Scientific Braille Display. Not really needed. 


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


        internal static bool CheckScreenReader(bool result, string caption)
        {
             if (!result)
            {

                if (null != caption)
                {
                    // The application has UI
                    ShowWarning(ModelMessageEnum.FailedToConnectToScreenReader,"JAWS",
                        "Kunne ikke forbinde til skærmlæser!\r\n"
                      + "Understøttede skærmlæsere er 'JAWS' og 'NVDA'\r\n"
                      + "Se venligst logfilen (Værktøjer->Log fil)"
                      );
                }
                else
                {
                    // The application is .cmd
                    Console.WriteLine("Kunne ikke forbinde til skærmlæser!");
                }
            }
            return result;
        }

        internal static bool CheckFileExistance(string fileName, string methodName, bool dir)
        {
            if (dir)
            {
                if (!System.IO.Directory.Exists(fileName))
                {
                    Logger.Log(string.Format("{0} Directory {1} is not found", string.IsNullOrEmpty(methodName) ? "" : methodName + ":", fileName));
                    ShowMessage(ModelMessageEnum.DirectoryNotFound,"", string.Format("Mappen {0} findes ikke", fileName));             
                    return false;
                }
            }
            else
            {
                if (!System.IO.File.Exists(fileName))
                {
                    Logger.Log(string.Format("{0} File {1} is not found", string.IsNullOrEmpty(methodName) ? "" : methodName + ":", fileName));
                    ShowMessage(ModelMessageEnum.FileNotFound,"", string.Format("Filen {0} findes ikke", fileName));
                    return false;
                }
            }
            return true;
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
            return RunExeWithFileArgument(exeFileName, argFileName, false);
        }

        internal static bool RunExeWithDirArgument(string exeFileName, string argFileName)
        {
            return RunExeWithFileArgument(exeFileName, argFileName, true);
        }


        internal static bool RunExeWithFileArgument(string exeFileName, string argFileName, bool dir)
        {
            string methodName = "RunExeWithFileArgument";
            // Check arguments
            string exePathName = Path.GetDirectoryName(exeFileName);
            if ((!string.IsNullOrEmpty(exePathName)) && (!CheckFileExistance(exeFileName, methodName, false))) return false;
            if ((!string.IsNullOrEmpty(argFileName)) && (!CheckFileExistance(argFileName, methodName, dir))) return false;
            // Create process startinfo. Enclose all filenames and pathnames in "" in order to handle possible space characters!
            System.Diagnostics.Process pProcess = new System.Diagnostics.Process();
            pProcess.StartInfo.FileName = string.Format("\"{0}\"", exeFileName);
            pProcess.StartInfo.WorkingDirectory = string.IsNullOrEmpty(exePathName) ? null : string.Format("\"{0}\"", exePathName);
            pProcess.StartInfo.Arguments = string.Format("\"{0}\"", argFileName);
            pProcess.StartInfo.UseShellExecute = true; // Allows the system to search for the executable using PATH
            pProcess.StartInfo.RedirectStandardOutput = false;
            pProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
            try
            {
                pProcess.Start();
            }
            catch (Exception e)
            {
                Logger.Log(string.Format("ReadFileByExecutable: Exception thrown while starting {0}: {1}", pProcess.StartInfo.FileName, e.Message));
                ShowMessage(ModelMessageEnum.FailedToStartProgram,"", string.Format("Kunne ikke starte programmet \r\n'{0}'\r\nmed filen\r\n'{1}'", exeFileName, argFileName));
                return false;
            }
            return true;
        }




    }

}
