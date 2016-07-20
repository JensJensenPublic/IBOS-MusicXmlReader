using System;
using System.IO;
using System.Windows.Forms;
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
            if (!File.Exists(Path.Combine(directory, dllName)))
            {
                Model.Log(string.Format("Missing support-dll: {0}", dllName));
                return false;
            }
            return true;
        }

        internal static bool CheckDlls(string directory, bool show)
        {
            // Report if any file is missing
            bool result = true;
            bool is64Bit = IntPtr.Size == 8;
            Model.Log(string.Format("This program is compiled for is a {0} bit ", is64Bit ? "64" : "32"));
            if (is64Bit)
            {
                result &= CheckDll("tolk.dll", directory);
                result &= CheckDll("jfwapi.dll", directory);
                result &= CheckDll("nvdaControllerClient64.dll", directory);
            }
            else
            {
                result &= CheckDll("tolk.dll", directory);
                result &= CheckDll("jfwapi.dll", directory);
                result &= CheckDll("nvdaControllerClient32.dll", directory);
            }

            // result = false; // Used during test only !!

            if (show & !result)
            {
                MessageBox.Show("Manglende programfil ! Se venligst Logfilen!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return result;
        }


        internal static bool CheckTolk(bool result, bool showMessageBoxOnError)
        {
            if (!result & showMessageBoxOnError)
            {
                string caption = "Kunne ikke forbinde til skærmlæser!";
                MessageBox.Show("Kunne ikke forbinde til skærmlæser!\r\n"
                                        + "Understøttede skærmlæsere er 'JAWS' og 'NVDA'\r\n"
                                        + "Se venligst logfilen (Værktøjer->Log fil)",
                                        caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return result;
        }
    }

}
