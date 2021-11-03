using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXmlReaderModel
{

    /// <summary>
    /// For experiments without changing base class
    /// Here we change all non-capital input to capital before looking up in the tables. Seems to be needed for .brf files from Braille.org
    /// </summary>
    class BrailleFileHandler_BRF_ASCII_Ex : BrailleFileHandler_BRF_ASCII
    {
        internal BrailleFileHandler_BRF_ASCII_Ex(int charsPerLine, int linesPerForm) :base(charsPerLine,linesPerForm)
        {}


        protected override byte LetterToCapital(byte b)
        {
            char c = (char)b;
            int offset = (int) ('a' - 'A');
            string letters = "abcdefghijklmnopqrstuvwxyz";
            if (letters.Contains(c))
            {
                int result = ((int)b - offset);
                return (byte)result;
            }
            else
            {
                return b;
            }
        }
    }
}
