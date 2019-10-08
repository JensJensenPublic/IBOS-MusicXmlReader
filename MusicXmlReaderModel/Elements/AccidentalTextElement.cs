using System.Xml;
using System.Collections.Generic;


namespace MusicXmlReaderModel
{
    // Seems to be almost identical to FormattedTextElement !

    // https://usermanuals.musicxml.com/MusicXML/Content/EL-MusicXML-accidental-text.htm

    class AccidentalTextElement : TextElementBase
    {
        private AccidentalTextElement(XmlNode node) : base(node,MyAttributes,MyChildNodes)
        { 
          
        }


        public static AccidentalTextElement Create(XmlNode node)
        {
            AccidentalTextElement result  = new AccidentalTextElement(node);
            LogFormatter.Log(once, string.Format("AccidentalTextElement({0})", result.ToDebugString()));
            return result;
        }

        private static List<string> MyAttributes = new List<string> {
            "print-object",
            "default-x",
            "default-y",
            "relative-x",
            "relative-y",
            "font-family",
            "font-style",
            "font-size",
            "font-weight",
            "color",
            "halign",
            "valign",
            "underline",
            "overline",
            "line-through",
            "rotation",
            "letter-spacing",
            "line-height",
             "xml:lang",
             "xml:space",
             "dir",
             "enclosure" };

        private static List<string> MyChildNodes = new List<string> { };
    }
}
