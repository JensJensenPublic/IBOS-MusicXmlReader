using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXmlReaderUI
{
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
        public static bool Parse(string input, ref int result, int lowValue, int highValue, string errorString)
        {
            int tempResult;
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

    }
}
