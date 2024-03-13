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
    /// Experimental class for splítting MusicBraille files, containing more than 1 score, into scores
    /// Must be used in conjunction with the BrailleSubSequenceList class
    /// </summary>
    public class BrailleSubSequence
    {
        private readonly char[] invalidCharsForFileName = Path.GetInvalidFileNameChars();
        private string fullSequence; // The full Braille-string, containing this SubSequence
        private int startIndex;      // The startindex of this subsequence within this.fullSequence
        private int endIndex;        // The endindex of this subsequence within this.fullSequence                                   

        // Other member variables describing tyis SubSequence 
        // Many of these variables are primarily for debugging purposes:
        public const int NotFound = -1;
 
        private int toMusicBrailleIndex = NotFound;
        public int ToMusicBrailleIndex { get { return toMusicBrailleIndex; }  }
        private int rightHandIndex = NotFound;
        public int RightHandIndex { get => rightHandIndex; }

        private int leftHandIndex = NotFound;
        public int LeftHandIndex { get => leftHandIndex;  }

        private int pedalHandIndex = NotFound;
        public int PedalHandIndex { get => pedalHandIndex; }

        public int Length { get { return endIndex - startIndex; } }

        private string name; // A unique name for this Subsequence, defined by the constructoe

        /// <summary>
        /// The Braille representation of of the first textsequence in this.fullSequence.
        /// Can be used as a base for translation using Liblouis, if transltation of contracted Braille is required.
        /// After translation the result must be filtered by this.ToValidFileName(s) befoe being used as a filename !
        /// </summary>
        public string TitleAsBraille { get { return this.fullSequence.Substring(startIndex, endIndex - startIndex);} }

        private enum TitleStateEnum { Before, During, After }
        private TitleStateEnum titleState = TitleStateEnum.Before;
        private StringBuilder title = new StringBuilder(); // For immediate interpretating as simple uncontracted Braille
        private StringBuilder titleAsFileName = new StringBuilder(); // For immediate interpretating as simple uncontracted Braille
        private int titleStart; // For later interpreting as contracted Braille
        private int titleEnd;   // For later interpreting as contracted Braille

        public string Title { get { return title.ToString(); } }
        public string TitleAsFileName { get { return ToValidFileName(Title,"-"); } }


        private void KeepFirstValue(ref int oldValue, int newValue)
        {
            if (oldValue != NotFound) return;
            oldValue = newValue;
        }

        /// <summary>
        /// Simple statemachine for isolating the first sequence of the sequence as a possible title, also usable as a filename
        /// </summary>
        /// <param name="isTitleInput"></param>
        /// <param name="input"></param>
        /// <param name="index"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnNewInput(bool isTitleInput, string input, int index)
        {
            switch (titleState)
            {
                case TitleStateEnum.Before:
                    if (isTitleInput)
                    {
                        titleStart = index;
                        title.Append(input);
                        titleState = TitleStateEnum.During;
                    }
                    break;
                case TitleStateEnum.During:
                    if (isTitleInput)
                    {
                        title.Append(input);
                    }
                    else
                    {
                        titleEnd = index;
                        titleState = TitleStateEnum.After;
                    }
                    break; 
                case TitleStateEnum.After:
                    break;
                default: throw new NotImplementedException();   
            }
        }


        public void OnNewInput(InputInterpretation inputInterpretation, int index)
        {
            if (null == inputInterpretation) return;
            bool isTitleInput = false;
            switch (inputInterpretation.Category)
            {
                // The following cases: Digit, Character, Space and TextVersal are all accepted as a part of a title
                case InputCategoryEnum.Digit: isTitleInput = true; break;
                case InputCategoryEnum.Character: isTitleInput = true; break;
                case InputCategoryEnum.Space: isTitleInput = true; break;
                case InputCategoryEnum.TextVersal: isTitleInput = true; break;

                // The following cases are used for determining some characteristica of the sequence and are primarily used for debugging
                case InputCategoryEnum.ToMusicBraille: KeepFirstValue(ref toMusicBrailleIndex,index); return;
                case InputCategoryEnum.Hand:
                    switch (inputInterpretation.SubCategory)
                    {
                        case InputSubCategoryEnum.HandRight: KeepFirstValue(ref rightHandIndex,index); break;
                        case InputSubCategoryEnum.HandLeft: KeepFirstValue(ref leftHandIndex,index); break;
                        case InputSubCategoryEnum.HandPedal: KeepFirstValue(ref pedalHandIndex,index); break;
                        default: break;
                    }
                    break;

                default: break;
            }
            OnNewInput(isTitleInput, inputInterpretation.FriendlyValue, index);
        }


        /// <summary>
        /// Replaces all characters, that are illegal in a filename with a specified string.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public string ToValidFileName(string s, string replacement)
        {
            StringBuilder result = new StringBuilder();
            foreach (char c in s)
            {
                if (invalidCharsForFileName.Contains(c))
                {
                    result.Append(replacement);
                }
                else
                {
                    result.Append(c);
                }
            }
            return result.ToString();   

        }


        public override string ToString()
        {
            return string.Format("StartIndex={0} EndIndex={1} ToMusicBraille={2} Right={3} Left={4} Pedal={5} Name='{6}' Title='{7}' TitleAsFileName='{8}' TitleAsBraille={9}",
                                  startIndex,    endIndex,    toMusicBrailleIndex, rightHandIndex, leftHandIndex,pedalHandIndex,name, Title, TitleAsFileName, TitleAsBraille);
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
