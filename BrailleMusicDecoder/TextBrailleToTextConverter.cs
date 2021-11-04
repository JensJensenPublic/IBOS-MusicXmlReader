using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrailleMusicDecoder
{


    /// <summary>
    /// Class for handling simple TEXT-Braille strings, including parentesis structures.
    /// For this reason the interpretation is build up from TextItems (each representing a string) instead of strings.
    /// This allows for simple handling of parentesis structures.
    /// </summary>
    public class TextBrailleToTextConverter
    {

        /// <summary>
        /// For simple "Braille6 to string" conversion one Braille6 character at a time.
        /// This function only returns a value for inputs with same meaning in all regions.
        /// This function is designed to be called from 2 different places:
        /// 1) Tokenreader.GetInputInterpretations() during conversion a plain text Braille string one symbol at a time.
        /// 2) this.Convert during conversion of Braille symbols that are embedded in a Music Braille string one symbol at a time. (Example: "ff", "pp", "somewhat slower" and other free text directives) 
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public string GetCharacter(int i)
        {
            bool dummy = false;
            return GetCharacter(i, ref dummy);
        }

        private string GetCharacter(int i, ref bool isDigit)
        {
            switch (i)
            {
                // Primitive mapping. Add more as needed !
                // NOTE: Regional differences are handled by the RegionalOptions class. This table only contains symbols common for ALL supported regions!
                // Thus no danish "Æ", "Ø" or "Å"
                case 00: return " ";
                case 01: return "a";
                case 03: return "b";
                case 09: return "c";
                case 25: return "d";
                case 17: return "e";
                case 11: return "f";
                case 27: return "g";
                case 19: return "h";
                case 10: return "i";
                case 26: return "j";
                case 05: return "k";
                case 07: return "l";
                case 13: return "m";
                case 29: return "n";
                case 21: return "o";
                case 15: return "p";
                case 31: return "q";
                case 23: return "r";
                case 14: return "s";
                case 30: return "t";
                case 37: return "u";
                case 39: return "v";
                case 58: return "w";
                case 45: return "x";
                case 61: return "y";
                case 53: return "z";
                //                case 28: return "æ";
                //                case 42: return "ø";
                //                case 33: return "å";
                //                case 32: return "VERSAL";
                //                case 60: return "CIFFER";
                case 50: return ".";
                case 02: return ",";
                case 38: return "?";
                case 06: return ";";
                case 22: return "!";
                case 54: return "/";
                case 36: return "-";
                case 04: return ".";
                case 18: return ":"; // Or "-" ??
                                     //                case 56: return ResourcesForBrailleMusicDecoder.InputCharacter_Uppercase;
                                     //                case 56: return "'dot456'"; // Found in "Four Piano Blues" but not recognized. During debugging we show "dot456"
                                     //                case 56: return "UPPERCASE";
                case 12: return "/";
                case 08: return "'"; // Found in title of NOTA transscription of "Højskoledsangbogens Melodibog 2012":"Nr. 33 Lyset springer pludslig ud.txt" Not documented anywhere else"
                case 60: isDigit = true; return null; // The Braille ToDigit dot3456
                default: return null;
            }
        }


        private string GetDigit(int i, ref bool isDigit)
        {
            switch (i)
            {
                case 00: return " ";
                case 01: return "1";
                case 03: return "2";
                case 09: return "3";
                case 25: return "4";
                case 17: return "5";
                case 11: return "6";
                case 27: return "7";
                case 19: return "8";
                case 10: return "9";
                case 26: return "0";
                default: isDigit = false; return "";
            }
        }

        /// <summary>
        /// Returns the interpretation of the braillevalue.
        /// </summary>
        /// <param name="brailleValue">The value to interpret</param>
        /// <param name="isDigit">A state variable deciding if the brailleValue should be interpreted as an alphanumeric character or as a digit</param>
        /// <returns></returns>
        public string GetCharacterOrDigit(int brailleValue, ref bool isDigit)
        {
            if (isDigit)
            {
                return GetDigit(brailleValue, ref isDigit);
            }
            else
            {
                return GetCharacter(brailleValue, ref isDigit);
            }
        }

        private TextItem leftParentesis;


        /// <summary>
        /// For conversion of Braille TEXT strings, possibly containing parentesis structures.
        /// </summary>
        /// <param name="textAsBraille"></param>
        /// <returns></returns>
        public string Convert(List<int> textAsBraille)
        {
            leftParentesis = null;
            List<TextItem> textItems = new List<TextItem>();
            bool isDigit = false;
            for (int i = 0; (i < textAsBraille.Count); i++)
            {
                int brailleValue = textAsBraille[i];
                string text = GetCharacterOrDigit(brailleValue,ref isDigit);
                TextItem item = new TextItem(text);
                if (0 != string.Compare("/", text))
                {
                    // This item needs no special threatment
                    textItems.Add(item);
                }
                else
                {
                    // This item may stand for "/", "(" or ")" depending on the cotext!
                    if (null == leftParentesis)
                    {
                        // Prepare for handling this item as a left parentesis if a match is found later.
                        leftParentesis = item;
                        textItems.Add(item);
                    }
                    else
                    {
                        // This item matches a previously found item. 
                        leftParentesis.Text = "("; // Change the contents of the previously found item to a left parentesis
                        leftParentesis = null; // Note that the left parentesis has now been matched.
                        textItems.Add(new TextItem(")")); // Add the matching right parentesis
                    }
                }

            }
            StringBuilder sb = new StringBuilder();
            foreach (TextItem textItem in textItems)
            {
                sb.Append(textItem.Text);
            }
            string textAsUnicode = sb.ToString(); // The embedded text in Unicode representation
            return textAsUnicode;
        }


        /// <summary>
        /// Returns tne value of a contracted symblo using the current regional options.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string ExpandContraction(int value)
        {
            if (null == regionalOptions) return null;
            return regionalOptions.ExpandContraction(value);
        }


        private RegionalOptions regionalOptions;

        private TextBrailleToTextConverter(RegionalOptions regionalOptions)
        {
            this.regionalOptions = regionalOptions;
        }

        private TextBrailleToTextConverter()
        {}

        public static TextBrailleToTextConverter Create(RegionalOptions regionalOptions)
        {
            return new TextBrailleToTextConverter(regionalOptions);
        }
    }
}

