using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MusicXmlReaderModel
{
    // https://usermanuals.musicxml.com/MusicXML/Content/EL-MusicXML-lyric.htm
    //
    // The lyric type represents text underlays for lyrics, based on Humdrum with support for other formats.
    // Two text elements that are not separated by an elision element are part of the same syllable, but may have different text formatting.
    // The MusicXML 2.0 XSD is more strict than the 2.0 DTD in enforcing this by disallowing a second syllabic element unless preceded by an elision element.
    // The lyric number indicates multiple lines, though a name can be used as well (as in Finale's verse / chorus / section specification).
    // Justification is center by default; placement is below by default.
    // The content of the elision type is used to specify the symbol used to display the elision.
    // Common values are a no-break space (Unicode 00A0), an underscore (Unicode 005F), or an undertie (Unicode 203F).


    public class LyricElement
    {
        private int number;
        public  int Number     {  get => number; }
        public  string Text { get => text; }
       

        private string text;
        private SyllabicElement syllabicElement;
        public SyllabicElement SyllabicElement { get => syllabicElement; }

        private LyricElement() { }
        private LyricElement(XmlNode node)
        {
            // Dig out attributes
            foreach (XmlAttribute a in node.Attributes)
            {
                switch (a.Name)
                {
                    case "number": number = int.Parse(a.Value); break;
                    default: Logger.LogCF(string.Format(": Usupported element: {0}", a.Name)); break;
                }
            }

            // Dig out elements
            foreach (XmlNode child in node.ChildNodes)
            { 
                string name = child.Name;
                switch (name)
                {
                    case "text": text=child.InnerText; break; // No need complicate things by looking at text as a "TextElement"
                    case "syllabic": syllabicElement = SyllabicElement.Create(child);   break;
                    case "extend":
                    case "humming":
                    case "laughing":
                    case "eliason": Logger.LogCF(string.Format(": Unsupported element: {0}", name)); break;

                    default: Logger.LogCF(string.Format(": Unexpected element: {0}", name)); break;
                }
            }

            string syllabicString = (null == syllabicElement) ? "" : string.Format("Syllabic={0}", syllabicElement.SyllabicEnum);
            Logger.LogCF(string.Format(": Number={0} Text={1} {2}", number, text, syllabicString));
        }
   

        public static LyricElement Create(XmlNode node)
        {
            return new LyricElement(node);
        }
    

    }

    //****************************************************************************************************************************



    public class LyricElementList
    {  
        private List<LyricElement> list = new List<LyricElement>();

        private string id;

        private int firstVerse = int.MaxValue;
        public int FirstVerse { get => firstVerse; }
        private int lastVerse = 0;
        public int LastVerse { get => lastVerse;  }

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
                        case SyllabicEnum.end:  break;
                        case SyllabicEnum.middle: postfix = ""; break;
                        case SyllabicEnum.single:   break;
                        default: Logger.LogCF(string.Format(": Unexpected value of SybellicEnum={0}", lyricElement.SyllabicElement.SyllabicEnum)); break;
                    }
                }
                sb.Append(prefix + lyricElement.Text + postfix);
            }
            return sb.ToString();
        }

        private LyricElementList()
        {}

        private LyricElementList(string id)
        {
            this.id = id;
        }

        public static LyricElementList Create(string id)
        {
            return new LyricElementList(id);
        }
    }

} // NameSpace
