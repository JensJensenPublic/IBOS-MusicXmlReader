using MusicXmlReaderModel;
using System;
using System.Collections.Generic;
using System.Linq;
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
