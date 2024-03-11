using MusicXmlReaderModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BrailleMusicDecoder
{
 

    /// <summary>
    /// Experimental class for splítting MusicBraille files, that contain more than 1 score into scores
    /// </summary>
    public class BrailleSubSequence
    {
        private readonly char[] invalidCharsForFileName = Path.GetInvalidFileNameChars();
        private string fullSequence;
        private int startIndex;
        private int endIndex;

        public const int NotFound = -1;


        
        /// <summary>
        /// Keep first value
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        private void KeepFirstValue(ref int oldValue, int newValue)
        {
            if (oldValue != NotFound) return;
            oldValue = newValue;            
        }


        // The remaining index are primarily for debugging purposes:
        private int toMusicBrailleIndex = NotFound;
        public int ToMusicBrailleIndex { get { return toMusicBrailleIndex; } set { KeepFirstValue( ref toMusicBrailleIndex, value); } }
        private int rightHandIndex = NotFound;
        public int RightHandIndex { get => rightHandIndex; set => KeepFirstValue(ref rightHandIndex, value); }

        private int leftHandIndex = NotFound;
        public int LeftHandIndex { get => leftHandIndex; set => KeepFirstValue(ref leftHandIndex, value); }

        private int pedalHandIndex = NotFound;
        public int PedalHandIndex { get => pedalHandIndex; set => KeepFirstValue(ref pedalHandIndex, value); }

        public int Length { get { return endIndex - startIndex; } }
        private string name;

        private StringBuilder text = new StringBuilder();
        private StringBuilder textForFileName = new StringBuilder();

        /// <summary>
        /// Contains all normal Text Braille contained in this BrailleSubSequence 
        /// </summary>
        public string Text { get => text.ToString(); }
        public string TextForFileName { get => textForFileName.ToString(); }

        private bool textForFileNameFound = false;

        /// <summary>
        /// Return the first text sequence of the BrailleSubsequence, typically a caption containing the name of the score described in the Subsequence
        /// </summary>
        public string Caption
        {
            get
            {
                string s = text.ToString();
                int i = s.IndexOf('\r'); // Index of first CR
                if (-1 == i) return s; // If not found
                return s.Substring(0, i);
            }
        }

        public void OnNewInput(InputInterpretation inputInterpretation, int index)
        {
            if (null == inputInterpretation) return;
            switch (inputInterpretation.Category)
            {
                case InputCategoryEnum.ToMusicBraille:  ToMusicBrailleIndex = index; return;
                case InputCategoryEnum.Hand:
                    switch (inputInterpretation.SubCategory)
                    {
                        case InputSubCategoryEnum.HandRight: rightHandIndex = index; break;
                        case InputSubCategoryEnum.HandLeft: leftHandIndex = index; break;
                        case InputSubCategoryEnum.HandPedal: pedalHandIndex = index; break;
                        default: break;
                    }
                    break;
                case InputCategoryEnum.FinalDoubleBar: break; // Occurs for each part!!
                case InputCategoryEnum.ControlCharCRLFNumber: OnBlackText("\r\n"); break;
                case InputCategoryEnum.ControlCharCRLF: OnBlackText("\r\n"); break;

                case InputCategoryEnum.Digit: OnBlackText(inputInterpretation.FriendlyValue);break;               

                case InputCategoryEnum.Character: OnBlackText(inputInterpretation.FriendlyValue); break;
                default: break;
            }
        }

        private void OnBlackText(string newText)
        {
            // Build a string containing all text
            this.text.Append(newText);
            // Build a string, usable as a filename
            textForFileNameFound |= newText.Contains('\r');
            if (!textForFileNameFound)
            {
                foreach (char c in invalidCharsForFileName)
                {
                    newText = newText.Replace(c, '-');
                }
                this.textForFileName.Append(newText);
            }
        }

        public override string ToString()
        {
            return string.Format("StartIndex={0} EndIndex={1} ToMusicBraille={2} Right={3} Left={4} Pedal={5} Name='{6}'",
                                  startIndex,    endIndex,    toMusicBrailleIndex, rightHandIndex, leftHandIndex,pedalHandIndex,name);
        }

        public void UpdateEndIndex(int endIndex)
        {
            this.endIndex = endIndex;
        }

 
        public string Contents { get {
                int i = fullSequence.Length;
                return fullSequence.Substring(startIndex, Length); } }
        public string Name { get { return name; } }

     

        public static BrailleSubSequence Create(string fullSequencem, int startIndex, int endIndex, string name)
        {
        return new BrailleSubSequence(fullSequencem, startIndex, endIndex, name);
        }

        public BrailleSubSequence(string fullSequence, int startIndex, int endIndex, string name)
        {
            this.fullSequence = fullSequence;
            this.startIndex = startIndex;
            this.endIndex = endIndex;
            this.name = name;
        
        }
        private BrailleSubSequence() { }
    }
}
