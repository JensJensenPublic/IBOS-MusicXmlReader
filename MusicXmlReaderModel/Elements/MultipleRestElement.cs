using System.Xml;

namespace MusicXmlReaderModel
{
    // https://usermanuals.musicxml.com/MusicXML/Content/EL-MusicXML-multiple-rest.htm

    class MultipleRestElement
    {
        private MultipleRestElement(XmlNode node)
        {
            // The documentation is unclear and we have no MusicXml file containing this element, so we do no decoding!
        }

        public override string ToString()
        {
            return "flertaktspause"; // to do Localize
        }

        public static MultipleRestElement Create(XmlNode node)
        {
            return new MultipleRestElement(node);
        }

    }
}
