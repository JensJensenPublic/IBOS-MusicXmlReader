using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXmlReaderModel
{
    public class LyricElementList
    {
        private List<LyricElement> list = new List<LyricElement>();

        private string id;

        private int firstVerse = int.MaxValue;
        public int FirstVerse { get => firstVerse; }
        private int lastVerse = 0;
        public int LastVerse { get => lastVerse; }

        public List<LyricElement> List { get => list; }


        public void AddElement(LyricElement lyricElement)
        {
            list.Add(lyricElement);
            firstVerse = Math.Min(firstVerse, lyricElement.Number);
            lastVerse = Math.Max(lastVerse, lyricElement.Number);
        }

        public void Append(LyricElementList that)
        {
            this.list.AddRange(that.list);
            this.firstVerse = Math.Min(this.firstVerse, that.firstVerse);
            this.lastVerse = Math.Max(this.lastVerse, that.lastVerse);
        }


        public void Append(LyricElement lyricElement)
        {
            this.list.Add(lyricElement);
            this.firstVerse = Math.Min(this.firstVerse, lyricElement.Number);
            this.lastVerse = Math.Max(this.lastVerse, lyricElement.Number);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (LyricElement lyricElement in this.list)
            {
                // Set up default prefix and postfix
                string prefix = "";
                string postfix = " ";
                if (null != lyricElement.SyllabicElement)
                {
                    switch (lyricElement.SyllabicElement.SyllabicEnum)
                    {
                        case SyllabicEnum.unknown: break;
                        case SyllabicEnum.begin: postfix = ""; break;
                        case SyllabicEnum.end: break;
                        case SyllabicEnum.middle: postfix = ""; break;
                        case SyllabicEnum.single: break;
                        default: Logger.LogCF(string.Format(": Unexpected value of SybellicEnum={0}", lyricElement.SyllabicElement.SyllabicEnum)); break;
                    }
                }
                sb.Append(prefix + lyricElement.Text + postfix);
            }
            return sb.ToString();
        }

        private LyricElementList()
        { }

        private LyricElementList(string id)
        {
            this.id = id;
        }

        public static LyricElementList Create(string id)
        {
            return new LyricElementList(id);
        }
    }
}
