using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BrailleMusicDecoder
{
    internal class BrailleSubSequenceList
    {
        private string fullSequence;
        private List<BrailleSubSequence> list;
        public List<BrailleSubSequence> List { get { return list; } }

        public void Add(BrailleSubSequence sequence)
        { 
            this.list.Add(sequence);
        }

        public static BrailleSubSequenceList Create(string fullSequence)
        {
            return new BrailleSubSequenceList(fullSequence);
        }


        private BrailleSubSequenceList(string fullSequence)
        {
            this.list = new List<BrailleSubSequence>();
            this.fullSequence = fullSequence;
        }
        private BrailleSubSequenceList() { }

    }



    /// <summary>
    /// Experimental class for splítting MusicBraille files, that contain more than 1 score into scores
    /// </summary>
    internal class BrailleSubSequence
    {
        private string fullSequence;
        private int startIndex;
        private int length;
        private string name;

        public string Contents { get { return fullSequence.Substring(startIndex, length); } }
        public string Name { get { return name; } } 

        public static BrailleSubSequence Create(string fullSequencem, int startIndex, int length, string name)
        {
        return new BrailleSubSequence(fullSequencem, startIndex, length, name);
        }

        public BrailleSubSequence(string fullSequence, int startIndex, int length, string name)
        {
            this.fullSequence = fullSequence;
            this.startIndex = startIndex;
            this.length = length;
            this.name = name;
        
        }
        private BrailleSubSequence() { }
    }
}
