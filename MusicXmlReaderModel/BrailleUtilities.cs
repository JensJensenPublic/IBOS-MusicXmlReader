using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXmlReaderModel
{
    static public class BrailleUtilities
    {
        const byte CarriageReturn = 13;
        const byte LineFeed = 10;
        const byte FormFeed = 12;

        /// <summary>
        /// Number of lines reserved for a caption on the first page
        /// </summary>
        public const int FirstPageCaptionSize = 3; 


        /// <summary>
        /// Simple formatting for notetaker devices. Just keep the existing format !
        /// </summary>
        /// <param name="unicodeBrailleList"></param>
        /// <returns></returns>
        static public string FormatForNoteTaker(List<string> unicodeBrailleList)
        {
            StringBuilder score = new StringBuilder();  // Represents the whole score
            foreach (string unicodeBraille in unicodeBrailleList)
            {
                score.Append(unicodeBraille);
                score.Append((char)CarriageReturn);
                score.Append((char)LineFeed);
            }
            return score.ToString();
        }


        /// <summary>
        /// Check that a caption does not exceed the number of lines statically defined by FirstPageCaptionSize.
        /// In this way we assure that the first page does not exceed the maximum number of lines
        /// </summary>
        /// <param name="caption"></param>
        /// <returns></returns>
        static public bool CheckCaption(string caption)
        {
            bool result = true;
            string[] splitCR = caption.Split('\r');
            int nCR = splitCR.Length - 1;
            string[] splitLF = caption.Split('\n');
            int nLF = splitLF.Length - 1;
            if ((nCR > FirstPageCaptionSize) || (nLF > FirstPageCaptionSize))
            {
                Logger.LogCF(string.Format(": nCR={0} or nLF={1} exceed BrailleUtilities.FirstPageCaptionSize={2}", nCR, nLF, FirstPageCaptionSize));
                result = false;
            }
            return result;
        }



    /// Formats a list of UNICODE strings, each representing a musical event  in to a single UNICODE,
    /// string taking into account the dimensions of the sheet to print on  
    /// </summary>
    /// <param name="unicodeBrailleList"></param>
    /// <param name="fullFileName"></param>
    /// <returns></returns>
    static public string Format(List<string> unicodeBrailleList,int lineWidth, int formHeight)
        {
            if (0 == lineWidth && (0 == formHeight))
            {
                return FormatForNoteTaker(unicodeBrailleList);
            }

            if (formHeight < FirstPageCaptionSize )
            {
                // The height of the form is less than the number of lines reserved for the caption.
                // This should be prevented in the code where the user sets up the format of the Music Braille representation! 
                string s = string.Format(": FormHeight={0} < FirstPageCaptionSize={1} ",  formHeight, FirstPageCaptionSize);
                Logger.LogCF(s);
                // Maybe we should throw an exception here ?
            }


            StringBuilder score = new StringBuilder();  // Represents the whole score
            int currentWidth = 0;
            int currentHeight = FirstPageCaptionSize; // Reserve a fixed number of lines on the first page for a caption
            int numberOfLines = 0; // For statistics only
            int numberOfForms = 1; // For statistics only



            foreach (string unicodeBraille in unicodeBrailleList)
            {
                string nextLine;
                string remainingChars = unicodeBraille; // Represents one single event
                bool done = false;
                while (!done)
                {
                    if (currentWidth + remainingChars.Length <= lineWidth)
                    {
                        nextLine = remainingChars;
                        done = true;
                        // No need to update remainingChars
                    }
                    else
                    {
                        // The next string (representing an event does not fit into the rest of this line, so we must add a new line
                        score.Append((char)CarriageReturn);
                        score.Append((char)LineFeed);
                        numberOfLines++;
                        currentWidth = 0;
                        currentHeight++;
                        int length = Math.Min(lineWidth, remainingChars.Length);
                        nextLine =  remainingChars.Substring(0,length);
                        remainingChars = remainingChars.Substring(length, remainingChars.Length - length);
                        if (currentHeight >= formHeight)
                        {
                            // Insert a FF
                            score.Append((char)FormFeed);
                            score.Append((char)CarriageReturn);
                            score.Append((char)LineFeed);                      
                            currentHeight = 0;
                            numberOfForms++;
                        }
                    }
                    currentWidth += nextLine.Length;
                    score.Append(nextLine);                

                    // In both cases split in forms if needed

            
                }
            }
            // Logger.LogCF(string.Format("(CharsPerLine={0} ,LinesPerForm={1}): Generated {2} forms containing {3} lines", lineWidth, formHeight, numberOfForms, numberOfLines));
            return score.ToString();
        }



    }
}
