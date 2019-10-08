using System;
using System.Xml;
using System.Collections.Generic;


namespace MusicXmlReaderModel
{

    // http://usermanuals.musicxml.com/MusicXML/MusicXML.htm#CT-MusicXML-empty-print-object-style-align.htm

    class PrintEmptyObjectStyleAlignElement : TextElementBase
    {

   


        protected PrintEmptyObjectStyleAlignElement(XmlNode node) : base(node, MyAttributes, MyChildNodes)
        {
        }

        public static PrintEmptyObjectStyleAlignElement Create(XmlNode node)
        {
            PrintEmptyObjectStyleAlignElement result = new PrintEmptyObjectStyleAlignElement(node);
            LogFormatter.Log(once, string.Format("PrintEmptyObjectStyleAlignElement({0})", result.ToDebugString())); // Implicitly calling base.ToDebugString()
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
            "valign" };

        //public override string ToDebugString()
        //{
        //    string result = base.ToDebugString();
        //    return result;
        //}


        private static List<string> MyChildNodes = new List<string> { };

    }
}
