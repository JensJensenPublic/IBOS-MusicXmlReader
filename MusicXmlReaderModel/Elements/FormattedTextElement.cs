using System.Xml;
using System.Collections.Generic;


namespace MusicXmlReaderModel
{

    // https://usermanuals.musicxml.com/MusicXML/Content/CT-MusicXML-formatted-text.htm

        // Seems to be almost identical to AccidentalTextElement !

    class FormattedTextElement : TextElementBase
    {

        private FormattedTextElement(XmlNode node) : base(node, MyAttributes, MyChildNodes) 
        {   
        }

        public static FormattedTextElement Create(XmlNode node)
        {
            FormattedTextElement result =  new FormattedTextElement(node);
            LogFormatter.Log(once, string.Format("FormattedTextElement({0})", result.ToDebugString()));
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
    };
  
}

