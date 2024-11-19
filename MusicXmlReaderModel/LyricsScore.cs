using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXmlReaderModel
{
    /// <summary>
    /// Represents the full lyrics of a score, including app parts, each part including all verses
    /// </summary>
    internal class LyricsScore
    {
        private List<LyricsPart> lyricsParts = new List<LyricsPart>();
        public List<LyricsPart> LyricsParts { get => lyricsParts; }

        private LyricsScore(PartDescriptionList partDescriptions, PartlistElement partlistElement)
        {
            Logger.LogCF(":+");
            int nParts = partDescriptions.parts.Count;
            List<string> result = new List<string>();
            // Simplify by handling one part at a time!
            foreach (PartDescription partDescription in partDescriptions.parts)
            {
                LyricsPart part = LyricsPart.Create(partDescription, partlistElement);
                lyricsParts.Add(part);
            }


        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(); 
            foreach (LyricsPart part in lyricsParts)
            {
            sb.Append(part.ToString()); 
            }
            return sb.ToString();
        }


        public static LyricsScore   Create(PartDescriptionList partDescriptions, PartlistElement partlistElement)
        {
            return new LyricsScore(partDescriptions, partlistElement);
        }
    }
}
