using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MusicXmlReaderModel
{
    // https://usermanuals.musicxml.com/MusicXML/Content/EL-MusicXML-syllabic.htm
    //
    // Lyric hyphenation is indicated by the syllabic type.
    // The single, begin, end, and middle values represent single-syllable words, word-beginning syllables, word-ending syllables, and mid-word syllables, respectively.
    //


    public enum SyllabicEnum {unknown, begin, end,middle, single }

    public class SyllabicElement
    {
        private SyllabicEnum syllabicEnum = SyllabicEnum.unknown;

        public SyllabicEnum SyllabicEnum { get => syllabicEnum;}

        private SyllabicEnum ToSyllabicEnum(string s)
        {
            SyllabicEnum result = SyllabicEnum.unknown;
            switch (s)
            {
                case "begin": result = SyllabicEnum.begin; break;
                case "end": result = SyllabicEnum.end; break;
                case "middle": result = SyllabicEnum.middle; break;
                case "single": result = SyllabicEnum.single; break;
                default: Logger.LogCF(string.Format(": Unexpectedted syllabic: {0}",s)); break;
            
            }
            // Logger.LogCF(string.Format(": returned {0}", result));
            return result;  
        }


        private SyllabicElement() { }
        private SyllabicElement(XmlNode node)
        {
            // Dig out attributes
            foreach (XmlAttribute a in node.Attributes)
            {
                switch (a.Name)
                {
                    default: Logger.LogCF(string.Format(": Usupported element: {0}", a.Name)); break;
                }
            }

            // Dig out elements
            foreach (XmlNode child in node.ChildNodes)
            {  
                string name = child.Name;
                switch (name)
                {
                    case "#text": syllabicEnum = ToSyllabicEnum(child.Value); break;
                    default: Logger.LogCF(string.Format(": Usupported element: {0}", name)); break;
                }
            }

            // Logger.LogCF(string.Format(": Number={0} Text={1}", number, text));
        }


        public static SyllabicElement Create(XmlNode node)
        {
            return new SyllabicElement(node);
        }
    }
}
