using MusicXmlReaderModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrailleMusicDecoder
{
    public class BrailleSubSequenceList
    {
        private string fullSequence;
        private List<BrailleSubSequence> list;
        private BrailleSubSequence currentBrailleSequence;
        //private int latestToMusicBrailleIndex;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (null == initialTextBrailleSequence)
            {
                sb.AppendLine("Initial textSequence is null");
            }
            else
            {
                sb.AppendLine(string.Format(": Initial textSequence: {0}", initialTextBrailleSequence.ToString()));
                sb.AppendLine(string.Format(": {0}{1}", "\r\n", initialTextBrailleSequence.Contents));
            }
            sb.Append(string.Format(": Number of scores = {0}", list.Count));
            foreach (BrailleSubSequence bss in list)
            {
                sb.Append(string.Format(": MusicSequence = {0}", bss.ToString()));
                sb.Append(string.Format(": {0}{1}", "\r\n", bss.Contents));
            }
            return sb.ToString();
        }


        private BrailleSubSequence initialTextBrailleSequence = null; // Anything before the first ToMusicBraille symbol
        public BrailleSubSequence InitialTextBrailleSequence { get { return initialTextBrailleSequence; } }

        public List<BrailleSubSequence> List { get { return list; } }

        public void Add(BrailleSubSequence sequence)
        {
            this.list.Add(sequence);
        }

        int separatingColons = 0;

        public void OnNewInput(InputInterpretation inputInterpretation, int index)
        {
            if (null == inputInterpretation) return;
            switch (inputInterpretation.Category)
            {
                case InputCategoryEnum.ToMusicBraille:
                    if (null == currentBrailleSequence)
                    {
                        currentBrailleSequence.ToMusicBrailleIndex = index;
                    }

                    //if (null == initialTextBrailleSequence)
                    //{
                    //    initialTextBrailleSequence = BrailleSubSequence.Create(fullSequence, 0, index, "Initial text");
                    //    Log(initialTextBrailleSequence, inputInterpretation.Category);
                    //}
                    //if (null != currentBrailleSequence)
                    //{
                    //    currentBrailleSequence.UpdateEndIndex(index); // Probably the FinalDoubleBar was missing
                    //    Log(currentBrailleSequence, inputInterpretation.Category);
                    //}
                    //Initially assume that the rest of the inputSequence belongs to this sequence
                    //currentBrailleSequence = BrailleSubSequence.Create(fullSequence,index,fullSequence.Length,string.Format("Score {0}",this.list.Count));
                    //this.Add(currentBrailleSequence);
                    break;
                case InputCategoryEnum.Hand:
                    if (null != currentBrailleSequence)
                    {
                        switch (inputInterpretation.SubCategory)
                        {
                            case InputSubCategoryEnum.HandRight: currentBrailleSequence.RightHandIndex = index; break;
                            case InputSubCategoryEnum.HandLeft: currentBrailleSequence.LeftHandIndex = index; break;
                            case InputSubCategoryEnum.HandPedal: currentBrailleSequence.PedalHandIndex = index; break;
                            default: break;
                        }


                    }

                    break;


                case InputCategoryEnum.FinalDoubleBar: // Occurs for each part!!
                    //currentBrailleSequence.UpdateEndIndex(index + 2); // 2 is the length of the FinalDoubleBar, which must be included !
                    //Log(currentBrailleSequence, inputInterpretation.Category);
                    break;

                case InputCategoryEnum.Character:
                    if (inputInterpretation.FriendlyValue == ":")
                    {
                        separatingColons++;
                    }
                    else
                    {
                        if (separatingColons > 10)
                        {
                            if (null == initialTextBrailleSequence)
                            {
                                initialTextBrailleSequence = BrailleSubSequence.Create(fullSequence, 0, index, "Initial text");
                                Log(initialTextBrailleSequence, inputInterpretation.Category);
                            }

                            Logger.LogCF(": End of separator found.");
                            if (null != currentBrailleSequence)
                            {
                                currentBrailleSequence.UpdateEndIndex(index);
                                Log(currentBrailleSequence, inputInterpretation.Category);
                            }
                            currentBrailleSequence = BrailleSubSequence.Create(fullSequence, index, fullSequence.Length, string.Format("Score {0}", this.list.Count));
                            this.Add(currentBrailleSequence);
                        }
                        separatingColons = 0;
                    }
                    break;


            }
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
        }
        private BrailleSubSequenceList() { }

    }


}
