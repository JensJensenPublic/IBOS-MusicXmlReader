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

        const string separator = ":";
        int nSeparators = 0;

        public void OnNewInput(InputInterpretation inputInterpretation, int index)
        {      
            char c = fullSequence[index];    
            if (c == 0x2812)
            {
                nSeparators++;
            }
            else
            {
                if (nSeparators > 10)
                {
                    Logger.LogCF(": End of separator found.");
                    currentBrailleSequence.UpdateEndIndex(index);
                    Log(currentBrailleSequence, inputInterpretation.Category);
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
