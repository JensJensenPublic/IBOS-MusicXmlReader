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
        /// </summary>
        /// <param name="input"></param>
        /// <param name="result"></param>
        /// <param name="lowValue"></param>
        /// <param name="highValue"></param>
        /// <param name="errorString"></param>
        /// <returns></returns>
        public static bool Parse(string input, ref int result, int lowValue, int highValue, string errorString)
        {
            if (!int.TryParse(input, out result))
            {
                Model.Log(string.Format("{0} Was'{1} Expected an integer", errorString, input));
                return false;
            }
            if (result < lowValue || (result > highValue))
            {
                Model.Log(string.Format("{0} Was'{1} Expected [{2}..{3}]", errorString, input, lowValue, highValue));
                return false;
            }
            return true;
        }

        /// <summary>
        /// Check valitity of an input parameter of type float
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
            if (!float.TryParse(input, out result))
            {
                Model.Log(string.Format("{0} Was'{1} Expected an integer", errorString, input));
                return false;
            }
            if (result < lowValue || (result > highValue))
            {
                Model.Log(string.Format("{0} Was'{1} Expected [{2}..{3}]", errorString, input, lowValue, highValue));
                return false;
            }
            return true;
        }

    }
}
