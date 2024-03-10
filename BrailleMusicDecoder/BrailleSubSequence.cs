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

        // The remainig index are primarily for debugging purposes:
        private int toMusicBrailleIndex = -1;
        public int ToMusicBrailleIndex { get { return toMusicBrailleIndex; } set { toMusicBrailleIndex = value; } }
        private int rightHandIndex = -1;
        public int RightHandIndex { get => rightHandIndex; set => rightHandIndex = value; }

        private int leftHandIndex = -1;
        public int LeftHandIndex { get => leftHandIndex; set => leftHandIndex = value; }

        private int pedalHandIndex = -1;
        public int PedalHandIndex { get => pedalHandIndex; set => pedalHandIndex = value; }

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
