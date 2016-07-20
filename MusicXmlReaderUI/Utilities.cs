using System;
using System.IO;
using System.Windows.Forms;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXmlReaderUI
{

    /// <summary>
    /// Contains logging and checking in order to avoid polluting the primary Model-logic
    /// </summary>
    static class Utilities
    {

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
                Model.Log(string.Format("{0}: Got '{1}' Expected an integer", errorString, input));
                return false;
            }
            if (tempResult < lowValue || (tempResult > highValue))
            {
                Model.Log(string.Format("{0}: Got'{1}' Expected [{2}..{3}]", errorString, input, lowValue, highValue));
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
                Model.Log(string.Format("{0}: Got '{1}' Expected a value in [{2}..{3}]", errorString, "NULL" , lowValue, highValue));
                return false;
            }

            // Accept either an empty string og a string consisting of a single character in the interval specified
            if (acceptEmptyString && (0 == input.Length) || ((1 == input.Length) && (input[0] >= lowValue) && ((input[0]) <= highValue)))
            {
                result = input;
                return true;
            }

            Model.Log(string.Format("{0}: Got '{1}' Expected a value in [{2}..{3}]", errorString, (null == input) ? "NULL" : input, lowValue, highValue));
            return false;
        }

        public static bool Parse(string input, ref char result, char lowValue, char highValue, string errorString)
        {
            if (null == input)
            {
                Model.Log(string.Format("{0}: Got '{1}' Expected a value in [{2}..{3}]", errorString, "NULL", lowValue, highValue));
                return false;
            }

            // Accept either an empty string og a string consisting of a single character in the interval specified
            if ( (1 == input.Length) && (input[0] >= lowValue) && ((input[0]) <= highValue))
            {
                result = input[0];
                return true;
            }

            Model.Log(string.Format("{0}: Got '{1}' Expected a value in [{2}..{3}]", errorString, (null == input) ? "NULL" : input, lowValue, highValue));
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
                Model.Log(string.Format("{0}: Got '{1}' Expected an integer", errorString, input));
                return false;
            }
            if (tempResult < lowValue || (tempResult > highValue))
            {
                Model.Log(string.Format("{0}: Got '{1}' Expected [{2}..{3}]", errorString, input, lowValue, highValue));
                return false;
            }
            result = tempResult;
            return true;
        }


        private static bool CheckDll(string dllName, string directory)
        {
            string fullFileName = Path.Combine(directory, dllName);
            if (!File.Exists(fullFileName))
            {
                Model.Log(string.Format("Missing support-dll: {0}", dllName));
                return false;
            }
            else
            {
                FileInfo fi = new FileInfo(fullFileName);
                MachineType machineType = TryGetDllMachineType(fullFileName);       
                Model.Log(string.Format("Using {0,-30} LastWriteTimeUtc={1} Length={2,-6} MachineType= {3}",
                                    fi.Name, fi.LastWriteTimeUtc, fi.Length, machineType.ToString()));
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
                Model.Log(string.Format("Failed to obtain MachineType for {0} Exception.Message={1}", fullFileName, e.Message));
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

        internal static bool CheckDlls(string directory, string caption)
        {
            // Report if any file is missing
            bool result = true;
            bool is64Bit = IntPtr.Size == 8;
            Model.Log(string.Format("This program is compiled for a {0} bit architechture.", is64Bit ? "64" : "32"));
            if (is64Bit)
            {
                result &= CheckDll("tolk.dll", directory);
                result &= CheckDll("jfwapi.dll", directory);
                result &= CheckDll("nvdaControllerClient64.dll", directory);
                result &= CheckDll("NAudio.dll", directory);
                result &= CheckDll("MusicSynthesis.dll", directory);
            }
            else
            {
                result &= CheckDll("tolk.dll", directory);
                result &= CheckDll("jfwapi.dll", directory);
                result &= CheckDll("nvdaControllerClient32.dll",directory);
                result &= CheckDll("NAudio.dll", directory);
                result &= CheckDll("MusicSynthesis.dll",directory);
            }

            // result = false; // Used during test only !!

            if ((null != caption) && !result)
            {
                MessageBox.Show("Manglende programfil ! Se venligst Logfilen!",caption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            return result;
        }


        internal static bool CheckTolk(bool result, string caption)
        {
            if (!result && (null!= caption))
            {       
                MessageBox.Show("Kunne ikke forbinde til skærmlæser!\r\n"
                                        + "Understøttede skærmlæsere er 'JAWS' og 'NVDA'\r\n"
                                        + "Se venligst logfilen (Værktøjer->Log fil)",
                                        caption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return result;
        }
    }

}
