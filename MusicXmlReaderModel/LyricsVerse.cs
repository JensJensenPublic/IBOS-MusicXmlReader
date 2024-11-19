using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXmlReaderModel
{
    /// <summary>
    /// Represents all lyrics for a verse (within a part)
    /// </summary>
    internal class LyricsVerse
    {
        private LyricElementList lyricElements;
        public LyricElementList LyricElementList { get => lyricElements;}

        string id;
        public string Id { get => id; set => id = value; }     


        public void Append(LyricElement lyricElement)
        {
            this.lyricElements.Append(lyricElement);
        }

        public override string ToString()
        {
            string result = lyricElements.ToString();
            return result;
        }


        private LyricsVerse(string id)
        {
            this.id = id;
            this.lyricElements = LyricElementList.Create(id);
        }
 

        public static LyricsVerse Create(string id)
        {
            return new LyricsVerse(id);
        }


    }
}
