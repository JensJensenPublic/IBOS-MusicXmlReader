using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicXmlReaderModel;

namespace BrailleMusicDecoder
{
    class MusicXmlRepeatHandler
    {
        //private StringBuilder currentString ;
        private const int undefined = -1;
        private int currentNumber;
        private List<int> currentNumbers;
        private bool Defined { get { return (currentNumber != undefined); } }

        private void LogCF(string s)
        {
            Logger.LogCF1(s);
        }

        public void OnToDigit()
        {
            LogCF("");
            if (Defined)
            {
                currentNumbers.Add(currentNumber);
            }
            currentNumber = undefined;
        }

        public void OnDigit(int digit)
        {
            LogCF(string.Format(": Digit= {0}",digit));
            if (Defined)
            {
                currentNumber = currentNumber * 10 + currentNumber;
            }
            else
            {
                currentNumber = digit;
            }
        }

        public bool OnSpace(out int offset, out int length)
        {
            offset = 0;
            length = 0;
            bool result = false;
            if (Defined)
            {
                currentNumbers.Add(currentNumber); // Add the last number to currentNumbers
            }
            StringBuilder sb = new StringBuilder();
            foreach (int i in currentNumbers)
            {
                sb.Append(i.ToString() + " ");
            }

            LogCF(string.Format(": CurrentNumbers=({0})",sb.ToString()));

            // Act on currentString
            if (2 == currentNumbers.Count)
            {
                offset = currentNumbers[0];
                length = currentNumbers[1];
                result = true;
            }
            else
            {
                LogCF(string.Format(": Illigal repeat parameters: {0}", sb.ToString()));
            }

            // Make ready for the next.
            currentNumber = undefined;
            currentNumbers.Clear();
            return result;
        }

        private MusicXmlRepeatHandler()
        {
            //currentString = new StringBuilder();
            currentNumbers = new List<int>(); 
            currentNumber = -1;
        }

        public static MusicXmlRepeatHandler Create()
        {
            return new MusicXmlRepeatHandler();
        }

    }
}
