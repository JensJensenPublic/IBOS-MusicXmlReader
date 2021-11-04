using System.Xml;

namespace BrailleMusicDecoder.MusicXmlElements
{
    public class MusicXmlMeasureElement : MusicXmlElement
    {
        private MusicXmlElementFactory elementFactory;
        public MusicXmlElementFactory ElementFactory { get { return elementFactory; } set { elementFactory = value; } }

        // Should only be called from MusicXmlDocument.CreateMusicXmlNoteElement  when a document is loaded from a file
        internal MusicXmlMeasureElement(string s0, string s1, string s2, XmlDocument doc) : base(s0, s1, s2, doc)
        { }

        // Called when building a MusicXmlDocument, for instance from the MusicXmlBuilder class.
        internal MusicXmlMeasureElement(string name, XmlDocument doc) : base("", name, "", doc)
        { }

        public void Method1() { }
        public void Method2() { }
        public string ElementName { get { return this.Name; } }
        public string ElementType { get { return this.GetType().ToString(); } }

        private XmlNode numberAttribute { get { return Attributes.GetNamedItem("number"); } }
        /// <summary>
        /// The measurenumber as specified in gth "number" attribute
        /// </summary>
        public int MeasureNumber { get { return int.Parse(numberAttribute.Value); } set { numberAttribute.Value = value.ToString(); } }

        /// <summary>
        /// Return a clone of this MusicXmlMeasureElement, but with the specified measureNumber
        /// </summary>
        /// <param name="newMeasureNumber"></param>
        /// <returns></returns>
        public MusicXmlMeasureElement Clone(int newMeasureNumber)
        {
            MusicXmlMeasureElement result = elementFactory.MeasureElement(newMeasureNumber);
            result.InnerXml = string.Copy(this.InnerXml); // Copy the XML
            return result;
        }


    }
}
