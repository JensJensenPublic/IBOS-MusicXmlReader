using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MusicXmlReaderModel
{
    internal class LyricsHandler
    {
        internal List<string> GetLyricsForParts(PartDescriptionList partDescriptions)
        {
            Logger.LogCF(":+");
            int nParts = partDescriptions.parts.Count;
            List<string> result = new List<string>();
            // Simplify by handling one part at a time!
            foreach (PartDescription partDescription in partDescriptions.parts)
            {
                LyricElementList lyricsForCurrentPart = LyricElementList.Create(partDescription.Id);              
                Logger.LogCF(string.Format(": Part={0}", partDescription.Id));
                // Collect all LyricElements related to this part
                foreach (Element element in partDescription.Elements)
                {
                    if (element is NoteElement)
                    {
                        LyricElementList lyrics =  (element as NoteElement).LyricElementList;
                        lyricsForCurrentPart.Append(lyrics); 
                    }
                }

                // lyricsForCurrentPart contains all lyrics for all verses within part
                // We need to split it into each verse.
                List<LyricElementList> verses = new List<LyricElementList>(lyricsForCurrentPart.LastVerse+1);
                for (int i = 0; i < lyricsForCurrentPart.LastVerse+1; i++)
                {
                    verses.Add( LyricElementList.Create(i.ToString()));
                }

                // Distribute the lyricElements with respect to verse number
                foreach (LyricElement lyricElement in lyricsForCurrentPart.List)
                {
                    verses[lyricElement.Number].Append(lyricElement);
                }

                // Convert the lyric for each verse into a string
                StringBuilder sb = new StringBuilder();
                foreach (LyricElementList verse in verses)
                { 
                    sb.Append(verse.ToString());
                    sb.Append("\r\n"); // Empty line between verses
                }
                // Add the string representation for all verses within the current part
                result.Add(sb.ToString());
            }
            Logger.LogCF(":-");
            return result;
        }

        private LyricsHandler()
        {         
        }


        internal static LyricsHandler Create()
        {
            return new LyricsHandler();
        }

    }
}
