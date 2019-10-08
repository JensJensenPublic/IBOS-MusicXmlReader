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

            StringBuilder score = new StringBuilder();  // Represents the whole score
            int currentWidth = 0;
            int currentHeight = 0;
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
