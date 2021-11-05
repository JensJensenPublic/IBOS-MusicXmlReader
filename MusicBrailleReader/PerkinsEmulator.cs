using System;
using System.Windows.Forms;
using System.Text;

namespace MusicBrailleReader
{
    public class PerkinsEmulator
    {

        [Flags]
        enum DotsEnum { dot1 = 1, dot2 = 2, dot3 = 4, dot4 = 8, dot5 = 16, dot6 = 32 }
        const DotsEnum allBits = DotsEnum.dot1 | DotsEnum.dot2 | DotsEnum.dot3 | DotsEnum.dot4 | DotsEnum.dot5 | DotsEnum.dot6;
        DotsEnum dots;
        DotsEnum stickyDots;
        public bool IsPerkinschar{ get { return (0 != (stickyDots & allBits)); } }
        //protected abstract string OnOtherKeyDown(Keys k);
        //protected abstract string OnOtherKeyUp(Keys k);



        /// <summary>
        /// Returns true if this is a Perkins emulated key: F,D,S,J,K,L
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public bool OnDown(Keys k)
        {
            switch (k)
            {
                case Keys.F: dots |= DotsEnum.dot1; break;
                case Keys.D: dots |= DotsEnum.dot2; break;
                case Keys.S: dots |= DotsEnum.dot3; break;
                case Keys.J: dots |= DotsEnum.dot4; break;
                case Keys.K: dots |= DotsEnum.dot5; break;
                case Keys.L: dots |= DotsEnum.dot6; break;
                default: return false;
            }
            stickyDots |= dots;
            return true;
        }
        /// <summary>
        /// Returns true if this is a Perkins emulated key
        /// </summary>
        /// <param name="k"></param>
        /// <param name="unicodeBraille"></param>
        /// <returns></returns>
        public bool OnUp(Keys k, out string unicodeBraille, out string dotNumbers)
        {
            unicodeBraille = null;
            dotNumbers = null;
            switch (k)
            {
                case Keys.F: dots &= ~DotsEnum.dot1; break;
                case Keys.D: dots &= ~DotsEnum.dot2; break;
                case Keys.S: dots &= ~DotsEnum.dot3; break;
                case Keys.J: dots &= ~DotsEnum.dot4; break;
                case Keys.K: dots &= ~DotsEnum.dot5; break;
                case Keys.L: dots &= ~DotsEnum.dot6; break;
                default: return false;

            }
            // When the last key is released we report and clear:
            if (0 == (dots & allBits))
            {
                int brailleInt = 0x2800 + (int)stickyDots;
                char brailleChar = (char)brailleInt;
                stickyDots = 0;
                unicodeBraille = brailleChar.ToString();
                dotNumbers = ToDotNumbers(brailleChar);
            }
            return true;
        }

        /// <summary>
        /// For debugging only
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public string ToDotNumbers(char c)
        {
            StringBuilder sb = new StringBuilder();
            if (IsBraille6(c))
            {
                if (0 != (c & 0x01)) sb.Append("1");
                if (0 != (c & 0x02)) sb.Append("2");
                if (0 != (c & 0x04)) sb.Append("3");
                if (0 != (c & 0x08)) sb.Append("4");
                if (0 != (c & 0x10)) sb.Append("5");
                if (0 != (c & 0x20)) sb.Append("6");
            }
            if (' ' == c) return "BLANK";
            return sb.ToString();
        }

        public string ToDotNumbers(string s)
        {
            if (null == s) return "";
            StringBuilder sb = new StringBuilder();
            foreach (char c in s)
            {
                sb.Append(ToDotNumbers(c));
                sb.Append(" ");
            }
            return sb.ToString();
        }


        public bool IsBraille6(char c)
        {
            if (c < 0x2800) return false;
            if (c > 0x28ff) return false;
            return true;
        }

        public string ToDebugString(string prefix,char c)
        {
            if (!IsBraille6(c)) return c.ToString();
            return prefix + ToDotNumbers(c);
        }

        public string ToDebugString(char c)
        {
            return ToDebugString("DOT",c);
        }

    }
}
