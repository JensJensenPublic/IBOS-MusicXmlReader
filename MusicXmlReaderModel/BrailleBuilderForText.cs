using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXmlReaderModel
{
    public class BrailleBuilderForText : BrailleBuilderBase
    {

        private const int unmappedStart = 0x4000; // Start of explicitly unmapped UNICODE area in this application

        private const byte UpperCase = dot6;
        public enum Category { Unknown, SpecialChar, UpperCaseChar, LowerCaseChar, Digit, BrailleChar };
        public enum State { Normal, Number, UpperCase };

        private class BrailleValue
        {
            private char inputChar;
            public char InputChar { get { return inputChar; } }
            private byte braillePattern;
            public byte BraillePattern { get { return braillePattern; } }
            private Category category;
            public Category Category { get { return category; } }

            internal BrailleValue(char c, byte braillePattern, Category category)
            {
                this.inputChar = c;
                this.braillePattern = braillePattern;
                this.category = category;
            }
        }

        /// <summary>
        /// https://www.pharmabraille.com/european-braille-guidance/ebu-european-braille-code-table/
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private BrailleValue ToBrailleValue(char c)
        {
            switch (c)
            {
                // NOTE: c is a UNICODE value, but the table is arranged according to ASCII values, just for convenience.

                // ASCII 0x00..0x1F are control characters. Most of them are not mapped
                // case '\r': return new BrailleValue(c, noDots, Category.SpecialChar); // CR
                // case '\n': return new BrailleValue(c, noDots, Category.SpecialChar); // NL

                // ASCII 20..2F
                case ' ': return new BrailleValue(c, noDots, Category.SpecialChar); // 0x20
                case '!': return new BrailleValue(c,dot2 + dot3 + dot4 + dot6 , Category.SpecialChar); // 0x21
                case '"': return new BrailleValue(c,dot5 , Category.SpecialChar); // 0x22
                case '#': return new BrailleValue(c, dot3 + dot4 + dot5 + dot6, Category.SpecialChar); // 0x23
                case '$': return new BrailleValue(c, dot1 + dot2 + dot4 + dot6, Category.SpecialChar); // 0x24
                case '%': return new BrailleValue(c, dot1 + dot4 + dot6, Category.SpecialChar); // 0x25
                case '&': return new BrailleValue(c, dot1 + dot2 + dot3 + dot4 + dot6, Category.SpecialChar); // 0x26
                case '\'': return new BrailleValue(c, dot3, Category.SpecialChar); // 0x27
                case '(': return new BrailleValue(c,dot1 + dot2 + dot3 +dot5 + dot6 , Category.SpecialChar); // 0x28
                case ')': return new BrailleValue(c, dot2 + dot3 + dot4 + dot5 + dot6, Category.SpecialChar); // 0x29
                case '*': return new BrailleValue(c, dot1 + dot6 , Category.SpecialChar); // 0x2A
                case '+': return new BrailleValue(c, dot3 + dot4 + dot6, Category.SpecialChar); // 0x2B
                case ',': return new BrailleValue(c,dot6 , Category.SpecialChar); // 0x2C
                case '-': return new BrailleValue(c,dot3 + dot6 , Category.SpecialChar); // 0x2D
#warning todo handle national differences in Braille and Music Braille
                // case '.': return new BrailleValue(c, dot4 + dot6, Category.SpecialChar); // 0x2E  https://en.wikipedia.org/wiki/Braille_pattern_dots-46
                case '.': return new BrailleValue(c, dot3, Category.SpecialChar); // 0x2E ); // PharmaBraille. Changed in 3.4 from dot4 + dot6 
                case '/': return new BrailleValue(c, dot3 + dot4, Category.SpecialChar); // 0x2F
                // ASCII 0x30..0x39
                case '0': return new BrailleValue(c, dot2 + dot4 + dot5, Category.Digit);
                case '1': return new BrailleValue(c, dot1, Category.Digit);
                case '2': return new BrailleValue(c, dot1 + dot2, Category.Digit);
                case '3': return new BrailleValue(c, dot1 + dot4, Category.Digit);
                case '4': return new BrailleValue(c, dot1 + dot4 + dot5, Category.Digit);
                case '5': return new BrailleValue(c, dot1 + dot5, Category.Digit);
                case '6': return new BrailleValue(c, dot1 + dot2 + dot4, Category.Digit);
                case '7': return new BrailleValue(c, dot1 + dot2 + dot4 + dot5, Category.Digit);
                case '8': return new BrailleValue(c, dot1 + dot2 + dot5, Category.Digit);
                case '9': return new BrailleValue(c, dot2 + dot4, Category.Digit);

                // ASCII 0x3A..0x3F
                case ':': return new BrailleValue(c, dot1 + dot5 + dot6, Category.SpecialChar); // 0x3A
                case ';': return new BrailleValue(c, dot5 + dot6, Category.SpecialChar); // 0x3B
                case '<': return new BrailleValue(c, dot1 + dot2 + dot6, Category.SpecialChar); // 0x3C
                case '=': return new BrailleValue(c, dot1 + dot2 + dot3 + dot4 + dot5 + dot6, Category.SpecialChar); // 0x3D
                case '>': return new BrailleValue(c, dot3 + dot4 + dot5, Category.SpecialChar); // 0x3E
                case '?': return new BrailleValue(c, dot1 + dot4 + dot5 + dot6, Category.SpecialChar); // 0x3F


                // 0x41..0x5A
                case 'A': return new BrailleValue(c, dot1, Category.UpperCaseChar);
                case 'B': return new BrailleValue(c, dot1 + dot2, Category.UpperCaseChar);
                case 'C': return new BrailleValue(c, dot1 + dot4, Category.UpperCaseChar);
                case 'D': return new BrailleValue(c, dot1 + dot4 + dot5, Category.UpperCaseChar);
                case 'E': return new BrailleValue(c, dot1 + dot5, Category.UpperCaseChar);
                case 'F': return new BrailleValue(c, dot1 + dot2 + dot4, Category.UpperCaseChar);
                case 'G': return new BrailleValue(c, dot1 + dot2 + dot4 + dot5, Category.UpperCaseChar);
                case 'H': return new BrailleValue(c, dot1 + dot2 + dot5, Category.UpperCaseChar);
                case 'I': return new BrailleValue(c, dot2 + dot4, Category.UpperCaseChar);
                case 'J': return new BrailleValue(c, dot2 + dot4 + dot5, Category.UpperCaseChar);
                case 'K': return new BrailleValue(c, dot1 + dot3, Category.UpperCaseChar);
                case 'L': return new BrailleValue(c, dot1 + dot2 + dot3, Category.UpperCaseChar);
                case 'M': return new BrailleValue(c, dot1 + dot3 + dot4, Category.UpperCaseChar);
                case 'N': return new BrailleValue(c, dot1 + dot3 + dot4 + dot5, Category.UpperCaseChar);
                case 'O': return new BrailleValue(c, dot1 + dot3 + dot5, Category.UpperCaseChar);
                case 'P': return new BrailleValue(c, dot1 + dot2 + dot3 + dot4, Category.UpperCaseChar);
                case 'Q': return new BrailleValue(c, dot1 + dot2 + dot3 + dot4 + dot5, Category.UpperCaseChar);
                case 'R': return new BrailleValue(c, dot1 + dot2 + dot3 + dot5, Category.UpperCaseChar);
                case 'S': return new BrailleValue(c, dot2 + dot3 + dot4, Category.UpperCaseChar);
                case 'T': return new BrailleValue(c, dot2 + dot3 + dot4 + dot5, Category.UpperCaseChar);
                case 'U': return new BrailleValue(c, dot1 + dot3 + dot6, Category.UpperCaseChar);
                case 'V': return new BrailleValue(c, dot1 + dot2 + dot3 + dot6, Category.UpperCaseChar);
                case 'W': return new BrailleValue(c, dot2 + dot4 + dot5 + dot6, Category.UpperCaseChar);
                case 'X': return new BrailleValue(c, dot1 + dot3 + dot4 + dot6, Category.UpperCaseChar);
                case 'Y': return new BrailleValue(c, dot1 + dot3 + dot4 + dot5 + dot6, Category.UpperCaseChar);
                case 'Z': return new BrailleValue(c, dot1 + dot3 + dot5 + dot6, Category.UpperCaseChar);
                // 0x0B..0x5D
                case '[':
                case 'Æ': return new BrailleValue(c, dot3 + dot4 + dot5, Category.UpperCaseChar); // 0x5B  
                case 'Ø': return new BrailleValue(c, dot2 + dot4 + dot6, Category.UpperCaseChar); // 0x5C
                case ']':
                case 'Å': return new BrailleValue(c, dot1 + dot6, Category.UpperCaseChar); // 0x5D
                // 0x5E..0x60 
                case '^': return new BrailleValue(c, dot4 + dot5, Category.SpecialChar); // 5E
                case '_': return new BrailleValue(c, dot4 + dot5 + dot6 , Category.SpecialChar); // 5F
                case '`': return new BrailleValue(c, dot3, Category.SpecialChar); // 60
                // 0x61..0x7A
                case 'a': return new BrailleValue(c, dot1, Category.LowerCaseChar);
                case 'b': return new BrailleValue(c, dot1 + dot2, Category.LowerCaseChar);
                case 'c': return new BrailleValue(c, dot1 + dot4, Category.LowerCaseChar);
                case 'd': return new BrailleValue(c, dot1 + dot4 + dot5, Category.LowerCaseChar);
                case 'e': return new BrailleValue(c, dot1 + dot5, Category.LowerCaseChar);
                case 'f': return new BrailleValue(c, dot1 + dot2 + dot4, Category.LowerCaseChar);
                case 'g': return new BrailleValue(c, dot1 + dot2 + dot4 + dot5, Category.LowerCaseChar);
                case 'h': return new BrailleValue(c, dot1 + dot2 + dot5, Category.LowerCaseChar);
                case 'i': return new BrailleValue(c, dot2 + dot4, Category.LowerCaseChar);
                case 'j': return new BrailleValue(c, dot2 + dot4 + dot5, Category.LowerCaseChar);
                case 'k': return new BrailleValue(c, dot1 + dot3, Category.LowerCaseChar);
                case 'l': return new BrailleValue(c, dot1 + dot2 + dot3, Category.LowerCaseChar);
                case 'm': return new BrailleValue(c, dot1 + dot3 + dot4, Category.LowerCaseChar);
                case 'n': return new BrailleValue(c, dot1 + dot3 + dot4 + dot5, Category.LowerCaseChar);
                case 'o': return new BrailleValue(c, dot1 + dot3 + dot5, Category.LowerCaseChar);
                case 'p': return new BrailleValue(c, dot1 + dot2 + dot3 + dot4, Category.LowerCaseChar);
                case 'q': return new BrailleValue(c, dot1 + dot2 + dot3 + dot4 + dot5, Category.LowerCaseChar);
                case 'r': return new BrailleValue(c, dot1 + dot2 + dot3 + dot5, Category.LowerCaseChar);
                case 's': return new BrailleValue(c, dot2 + dot3 + dot4, Category.LowerCaseChar);
                case 't': return new BrailleValue(c, dot2 + dot3 + dot4 + dot5, Category.LowerCaseChar);
                case 'u': return new BrailleValue(c, dot1 + dot3 + dot6, Category.LowerCaseChar);
                case 'v': return new BrailleValue(c, dot1 + dot2 + dot3 + dot6, Category.LowerCaseChar);
                case 'w': return new BrailleValue(c, dot2 + dot4 + dot5 + dot6, Category.LowerCaseChar);
                case 'x': return new BrailleValue(c, dot1 + dot3 + dot4 + dot6, Category.LowerCaseChar);
                case 'y': return new BrailleValue(c, dot1 + dot3 + dot4 + dot5 + dot6, Category.LowerCaseChar);
                case 'z': return new BrailleValue(c, dot1 + dot3 + dot5 + dot6, Category.LowerCaseChar);
                // 0x7B..0x7D
                case 'æ': return new BrailleValue(c, dot3 + dot4 + dot5, Category.LowerCaseChar);
                case 'ø': return new BrailleValue(c, dot2 + dot4 + dot6, Category.LowerCaseChar);
                case 'å': return new BrailleValue(c, dot1 + dot6, Category.LowerCaseChar);

                // 0x7E..0x7F
                //case '~': return new BrailleValue(c, dot4 + dot5, Category.SpecialChar); // 7E "tilde"
                //case '': return new BrailleValue(c, dot4 + dot5 + dot6, Category.SpecialChar); // 7 [DEL]

                // Outside the ASCII range:
                case 'á': return new BrailleValue(c, dot1, Category.LowerCaseChar); // Map as an 'a'
                case 'ě':
                case 'è':
                case 'é':
                case 'ê': return new BrailleValue(c, dot1 + dot5, Category.LowerCaseChar); // Map as an 'e'
                case 'í': return new BrailleValue(c, dot2 + dot4, Category.LowerCaseChar); // Map as an 'i'
                case 'ó':
                case 'ö': return new BrailleValue(c, dot1 + dot3 + dot5, Category.LowerCaseChar); // Map as an 'o'

                // The "Special" Unicode block 
                // https://en.wikipedia.org/wiki/Specials_(Unicode_block)
                case (char)0xfff9:
                case (char)0xfffA:
                case (char)0xfffB:
                case (char)0xfffC:
                case (char)0xfffD: //  REPLACEMENT CHARACTER used to replace an unknown, unrecognized or unrepresentable character
                case (char)0xfffE:
                case (char)0xfffF:
                    Logger.LogCFOnce(string.Format(": Unmapped value '{0}'=0x{1:X}", c, (int)c));
                    return new BrailleValue(c, noDots, Category.Unknown);

                default:
             
                    if (unmappedStart <= c)
                    {
                        Logger.LogCFOnce(string.Format(": Unmapped value >= 0x{0:X}", unmappedStart));
                    }
                    else
                    {
                        Logger.LogCFOnce(string.Format(": Unmapped value '{0}'=0x{1:X}", c, (int)c));
                    }
                    return new BrailleValue(c, noDots, Category.Unknown);
            }

        }


        /// <summary>
        /// Add a normal text (i.e NOT a Musical sequence)
        /// </summary>
        /// <param name="normalText"></param>
        public void AddNormalText(string normalText)
        {
            int inputLength = normalText.Length;
            BrailleValue[] brailleValues = new BrailleValue[inputLength];
            int i; // generally used index

            for (i = 0; (i < inputLength); i++)
            {
                brailleValues[i] = ToBrailleValue(normalText[i]); // Collect, translate and categorize
            }

            State state = State.Normal;
            for (i = 0; i < inputLength; i++)
            {
                BrailleValue inputValue = brailleValues[i];
                switch (state)
                {
                    case State.Normal:
                        switch (inputValue.Category)
                        {
                            case Category.Digit:
                                state = State.Number;
                                Append(Number); // The "Number" mark: dot3+dot4+dot5+dot6
                                break;
                            case Category.UpperCaseChar:
                                // Remain in state 
                                Append(UpperCase); // The "UpperCase" mark: dot6
                                break;
                            default:
                                // Remailn in state
                                break;
                        }
                        break;


                    case State.Number:
                        switch (inputValue.Category)
                        {
                            case Category.Digit:
                                // Remain in state and reuse the existing Number mark
                                break;
                            case Category.UpperCaseChar:
                                state = State.UpperCase; // Leave the Number state 
                                Append(UpperCase); // The "UpperCase" mark: dot6
                                break;
                            default:
                                state = State.Normal;
                                break;
                        }
                        break;

#warning todo Use  State.UpperCase to handle long sequences of uppercase characters.   
                    case State.UpperCase:
                    default:
                        break;
                }

                // Finally ALWAYS add the raw Braille pattern (and the explaining text to help the programmer during debugging!)
                Append(inputValue.BraillePattern); // The raw Braille code for the digit
                AppendText(inputValue.InputChar.ToString());

            }
        }

        private BrailleBuilderForText(Int64 timeStamp) : base(timeStamp)
        {
        }

        public static BrailleBuilderForText Create(Int64 timeStamp)
        {
            return new BrailleBuilderForText(timeStamp);
        }


#if false

            BrailleValue brailleValue;
            Category currentCategory = Category.Unknown; // Initial state
            for (int i = 0; (i < normalText.Length); i++)
            {
                char normalChar = normalText[i];
                brailleValue = ToBrailleValue(c,normalChar);
                if (brailleValue.Category != currentCategory)
                {
                    byte modifier = noDots; 
                    switch (brailleValue.Category)
                    {
                        case Category.BrailleChar: break;
                        case Category.ControlChar: break;
                        case Category.Digit: modifier = dot3 + dot4 + dot5 + dot6; break; // Mark as Digit
                        case Category.LowerCaseChar: break;
                        case Category.Unknown: break;
                        case Category.UpperCaseChar: modifier = // Mark as UpperCase
                        default: modifier = 

                    }
                    currentCategory = brailleValue.Category;
                    if (noDots != modifier)
                    {
                        braille.Add(modifier);
                        this.text.Append(" "); // Placeholder needed because the modifier was added
                    }
                }
                braille.Add(brailleValue.BraillePattern);
                this.text.Append(normalChar);
            }
        }
#endif

        //public string ToBraille(string normalText)
        //{
        //}



        //private BrailleText(string normalText)
        //{

        //}

        //static public BrailleText Create(string normalText)
        //{
        //    return new BrailleText(normalText)
        //}
    }

}
