using MusicXmlReaderModel;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BrailleMusicDecoder
{
    /// <summary> 
    /// Experimental class for splítting MusicBraille files, containing more than 1 score, into scores
    /// Must be used in conjunction with the BrailleSubSequence class
    /// </summary>
    public class BrailleSubSequenceList
    {
        private string fullSequence;
        private List<BrailleSubSequence> list;
        private BrailleSubSequence currentBrailleSequence;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();       
            sb.Append(string.Format(": Number of scores = {0}", list.Count));
            foreach (BrailleSubSequence bss in list)
            {
                sb.Append(string.Format(": MusicSequence = {0}", bss.ToString()));
                sb.Append(string.Format(": {0}{1}", "\r\n", bss.Contents));
            }
            return sb.ToString();
        }

        public List<BrailleSubSequence> List { get { return list; } }

        public void Add(BrailleSubSequence sequence)
        {
            this.list.Add(sequence);
        }

        // Unicode 0x2812 represents Braille DOT2 + DOT5 which is used by NOTA as separating character because a sequence of there forms a horisontal line
        // When interpreted as a (Danish) letter it becomes the ":" (Colon)
        const char separator = (char) 0x2812;
        int nSeparators = 0;

        /// <summary>
        /// Ad hoc code for splitting NOTA's large file "Koralbog_til_Den_danske_salmebog_2003__bind_1" (in .brf, .brf or Unicode) into 
        /// more than 500 separate files, each containing a single score or an explaining text.
        /// Each score is temporarily repsesented as BrailleSubSequence object within a BrailleSubSequenceList.
        /// </summary>
        /// <param name="inputInterpretation">The next interpretated value to handle</param>
        /// <param name="index">The index within the "fullSequence" string, where the interpretation starts</param>
        public void OnNewInput(InputInterpretation inputInterpretation, int index)
        {      
            char c = fullSequence[index];    
            if (c == separator)
            {
                nSeparators++;
            }
            else
            {
                if (nSeparators > 10)
                {
                    Logger.LogCF(": End of separator found.");
                    currentBrailleSequence.UpdateEndIndex(index);
                    Log(currentBrailleSequence,  (null == inputInterpretation) ? InputCategoryEnum.None  : inputInterpretation.Category); // Avoid nullreference !
                    currentBrailleSequence = BrailleSubSequence.Create(fullSequence, index, fullSequence.Length, string.Format("Score {0}", this.list.Count));
                    this.Add(currentBrailleSequence);
                }
                nSeparators = 0;
            }

            currentBrailleSequence.OnNewInput(inputInterpretation, index);         
        }

        public void AfterLastInput()
        {
            Log(currentBrailleSequence, InputCategoryEnum.None); // Be sure to log the last subsequence
        }


        private void Log(BrailleSubSequence bss, InputCategoryEnum category)
        {
            string s = string.Format("({0}): {1} Contents=\r\n{2}", category, bss.ToString(), bss.Contents);
            Logger.LogCF1(s);
            //Logger.LogCF1(string.Format("({0}): {1}{2}", category, "\r\n",bss.Contents));
        }

        public static BrailleSubSequenceList Create(string fullSequence)
        {
            return new BrailleSubSequenceList(fullSequence);
        }

        private BrailleSubSequenceList(string fullSequence)
        {
            this.list = new List<BrailleSubSequence>();
            this.fullSequence = fullSequence;
            this.currentBrailleSequence = BrailleSubSequence.Create(this.fullSequence, 0, 0, "Sequence 0");
            this.List.Add(currentBrailleSequence);
        }
        private BrailleSubSequenceList() { }

    }


}
