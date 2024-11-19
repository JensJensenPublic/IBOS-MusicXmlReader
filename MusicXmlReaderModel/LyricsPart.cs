using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXmlReaderModel
{
    /// <summary>
    /// Represents all lyrics for a part within a score
    /// </summary>
    internal class LyricsPart
    {

        private List<LyricsVerse> verses = new List<LyricsVerse>();
        public List<LyricsVerse> Verses { get => verses; }

        private ScorePartElement scorePartElement;
        string id;
        string partName;

        private LyricsPart(PartDescription partDescription, PartlistElement partlistElement)
        {
            this.id = partDescription.Id;
            this.scorePartElement = partlistElement.GetPartFromId(partDescription.Id); // For later obtaining Partname Instrumentname etc. for this part
            this.partName = scorePartElement.PartName;
            LyricElementList lyricsForCurrentPart = LyricElementList.Create(partDescription.Id);
            Logger.LogCF(string.Format(": Part={0}", partDescription.Id));
            // Collect all LyricElements related to this part
            foreach (Element element in partDescription.Elements)
            {
                if (element is NoteElement)
                {
                    LyricElementList lyrics = (element as NoteElement).LyricElementList;
                    lyricsForCurrentPart.Append(lyrics);
                }
            }

            // lyricsForCurrentPart contains all lyrics for all verses within part
            // We need to split it into each verse.
            verses = new List<LyricsVerse>(lyricsForCurrentPart.LastVerse + 1);
            for (int i = 0; i < lyricsForCurrentPart.LastVerse + 1; i++)
            {
                verses.Add(LyricsVerse.Create(i.ToString()));
            }

            // Distribute the lyricElements with respect to verse number
            foreach (LyricElement lyricElement in lyricsForCurrentPart.List)
            {
                verses[lyricElement.Number].Append(lyricElement);
            }

        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("\r\n" + id + " " + partName + "\r\n");
            foreach (LyricsVerse lyricVerse in verses)
            {
                sb.Append(lyricVerse.ToString() + "\r\n");

            }
            sb.Append("\r\n\r\n");
            return sb.ToString();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="partDescription">Contains all timerelated information, such as notes and lyrics</param>
        /// <param name="partlistElement">Contains some static information, such as partname and instrument</param>
        /// <returns></returns>
        public static LyricsPart Create(PartDescription partDescription, PartlistElement partlistElement)
        {
            return new LyricsPart(partDescription, partlistElement);
        }
    }
}



