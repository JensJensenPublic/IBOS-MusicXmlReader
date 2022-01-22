using System.Xml;
using MusicXmlReaderModel;

namespace BrailleMusicDecoder.MusicXmlElements
{
    /// <summary>
    ///  Conveniense methods for manipulation af MusicXml HarmonyElements
    /// </summary>
    public class MusicXmlHarmonyElement : MusicXmlElement
    {

// For values of "kind" see         https://usermanuals.musicxml.com/MusicXML/Content/ST-MusicXML-kind-value.htm

#warning TODO Handle major-seventh, major-ninth, major-11th, major-13

        private string AddToMajor(string s)
        {

            switch (s)
            {
                case "6": return "major-sixth";
                case "7": return "dominant";
                case "9": return "dominant-ninth";
                case "11": return "dominant-11th";
                case "13": return "dominant-13th";
                case "9b": AddDegree(9,-1) ; return "dominant";  // Build from a dominant (-seventh)!
                case "9#": AddDegree(9,+1) ;  return "dominant";  // Build from a dominant (-seventh)!
            }
            LogUserWarning(UnSupportedMessage(s), UserInfoFlagsEnum.UnsupportedExtensionToMajorChord);
            LogCF(UnSupportedMessage(s));
            return "major";
        }

        private void AddDegree(int degree, int alter)
        {
            XmlNode kindNode = SelectSingleNode("kind");
            XmlNode degreeElement = elementFactory.DegreeElement(degree,alter,"add");
            AppendChild(degreeElement);
        }

        private string AddToMinor(string s)
        {
            switch (s)
            {
                case "6": return "minor-sixth";
                case "7": return "minor-seventh";
                case "9": return "minor-ninth";
                case "11": return "minor-11th";
                case "13": return "minor-13th";
                case "9b": AddDegree(9, -1); return "minor-seventh"; // Build from a minor-seventh!
                case "9#": AddDegree(9, -1); return "minor-seventh";  // Build from a minor-seventh!
            }
            LogUserWarning(UnSupportedMessage(s), UserInfoFlagsEnum.UnsupportedExtensionToMinorChord);
            LogCF(UnSupportedMessage(s));
            return "minor";
        }

        public XmlNode AddChordNumericExtension(string inputValue)
        {
            XmlNode kindNode = SelectSingleNode("kind");
            string kind = kindNode.InnerText;
            string newKind = kind;
            switch (kind)
            {
                case "major": newKind = AddToMajor(inputValue); break;
                case "minor": newKind = AddToMinor(inputValue); break;
                default: break;
            }
            kindNode.InnerText = newKind;
            return this;
        }

        private string UnSupportedMessage(string s)
        {
            return string.Format(": Unsupported numeric extension:'{0}'", s);
        }


        private MusicXmlElementFactory elementFactory;
        public MusicXmlElementFactory ElementFactory { get { return elementFactory; } set { elementFactory = value; } }



        // Constructor, should only be called from MusicXmlDocument.CreateMusicXmlHarmonyElement  when a document is loaded from a file
        internal MusicXmlHarmonyElement(string s0, string s1, string s2, XmlDocument doc) : base(s0, s1, s2, doc)
        { }

        // Constructor, called when building a MusicXmlDocument, for instance from the MusicXmlBuilder class.
        internal MusicXmlHarmonyElement(string name, XmlDocument doc) : base("",name,"", doc)
        { }




        //private MusicXmlHarmonyElement(XmlNode xmlNode, MusicXmlBuilderElements elements) : base(xmlNode)
        //{
        //    this.elements = elements;
        //}

        //public static  MusicXmlHarmonyElement Create(XmlNode xmlNode, MusicXmlBuilderElements elements)
        //{
        //    return new MusicXmlHarmonyElement(xmlNode,elements);
        //}
    }
}
